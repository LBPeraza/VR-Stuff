using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum LinkControllerState
    {
        Inactive,
        OverSource,
        DrawingLink
    }

    public enum LinkSoundEffect
    {
        LinkSevered,
        LinkCompleted,
        LinkDrawing,
        LinkBandwidthExceeded
    }

    public class LinkController : MonoBehaviour
    {
        public bool IsRightHand;
        public Player Player;
        public Cursor Cursor;
        public AudioSource AudioSource;
        public AudioSource AuxillaryAudioSource;
        public AudioClip LinkSevered;
        public AudioClip LinkDrawing;
        public AudioClip LinkConnected;
        public AudioClip LinkDepleted;
        public int ObjectId;

        public float DrawingLinkRumbleBaseLength = 0.01f;
        public ushort DrawingLinkRumbleLength = 100;
        public ushort SeverLinkRumbleLength = 3000;
        public ushort SeverLinkRumbleDecay = 300;

        public LinkControllerState State;

        private GameObject CurrentLink;
        private GameObject LastLink;

        private PacketSource NearSource;
        private PacketSink NearSink;

        private CursorEventArgs cursorEventArgs;

        // Use this for initialization
        public void Initialize(Player p, bool isRightHand)
        {
            Player = p;
            IsRightHand = isRightHand;

            ObjectId = GetInstanceID();
            cursorEventArgs.senderId = ObjectId;

            if (Cursor == null)
            {
                Cursor = GetComponent<Cursor>();
            }

            State = LinkControllerState.Inactive;

            if (IsRightHand)
            {
                InputManager.RightTriggerClicked += TriggerDown;
                InputManager.RightTriggerUnclicked += TriggerUp;
            }
            else
            {
                InputManager.LeftTriggerClicked += TriggerDown;
                InputManager.LeftTriggerUnclicked += TriggerUp;
            }

            LoadAudioClips();
        }

        private void LoadAudioClips()
        {
            if (AudioSource == null)
            {
                AudioSource = transform.GetComponentInChildren<AudioSource>();
            }

            if (AuxillaryAudioSource == null)
            {
                AuxillaryAudioSource = this.gameObject.AddComponent<AudioSource>();
            }

            if (LinkConnected == null)
            {
                LinkConnected = Resources.Load<AudioClip>("link_connect");
            }

            if (LinkDepleted == null)
            {
                LinkDepleted = Resources.Load<AudioClip>("link_depleted");
            }

            if (LinkSevered == null)
            {
                LinkSevered = Resources.Load<AudioClip>("link_sever");
            }

            if (LinkDrawing == null)
            {
                LinkDrawing = Resources.Load<AudioClip>("link_drawing");
            }
        }

        private void AddLink()
        {
            GameObject LinkContainer = LinkFactory.CreateLink(NearSource);
            var linkSegment = LinkContainer.GetComponent<Link>();
            linkSegment.Initialize(NearSource, this.transform);

            // Listen for sever events.
            linkSegment.OnSever += LinkSegment_OnSever;
            linkSegment.OnConstructionProgress += LinkSegment_OnConstructionProgress;

            CurrentLink = LinkContainer;

            State = LinkControllerState.DrawingLink;
            Cursor.OnGrab(cursorEventArgs);
        }

        public void TriggerDown(object sender, ClickedEventArgs args)
        {
            if (CurrentLink == null 
                && NearSource != null
                && !Player.IsOutOfBandwidth()
                && NearSource.QueuedPackets.Count > 0)
            {
                PlayClip(LinkSoundEffect.LinkDrawing);

                AddLink();
            }
        }

        public void TriggerUp(object sender, ClickedEventArgs args)
        {
            if (CurrentLink != null && NearSink == null)
            {
                // End the current link in the air.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();

                if (currentLinkComponent != null)
                {
                    currentLinkComponent.End();
                    currentLinkComponent.Source.OnPacketDropped(currentLinkComponent.Packet);
                    GameManager.ReportPacketDropped(currentLinkComponent.Source.DequeuePacket());

                    if (currentLinkComponent.Segments.Count > 0)
                    {
                        var lastSegment = currentLinkComponent.Segments[currentLinkComponent.Segments.Count - 1];
                        currentLinkComponent.Sever(SeverCause.UnfinishedLink, lastSegment);
                    }
                }
                else
                {
                    Debug.LogError("Link Controller: 'current link component' is null on a trigger up event.");
                }

                State = LinkControllerState.Inactive;
                Cursor.OnExit(cursorEventArgs);

                StopClips();

                DestroyLink();
            }
        }

        public void StopClips()
        {
            AudioSource.Stop();
        }

        public void PlayClip(LinkSoundEffect effect)
        {
            AudioSource source = AudioSource;
            AudioClip clip = LinkSevered;
            bool repeat = false;
            float volume = 0.5f;

            switch (effect)
            {
                case LinkSoundEffect.LinkBandwidthExceeded:
                    clip = LinkDepleted;
                    break;
                case LinkSoundEffect.LinkCompleted:
                    volume = 1.0f;
                    clip = LinkConnected;
                    break;
                case LinkSoundEffect.LinkDrawing:
                    repeat = true;
                    clip = LinkDrawing;
                    break;
                case LinkSoundEffect.LinkSevered:
                    source = AuxillaryAudioSource;
                    volume = 1.0f;
                    clip = LinkSevered;
                    break;
            }

            if (clip != null)
            {
                source.Stop();
                source.volume = volume;
                source.clip = clip;
                source.loop = repeat;
                source.Play();
            }
            
        }

        public void OnTriggerEnter(Collider other)
        {
            if (CurrentLink == null && other.CompareTag("Source"))
            {
                // In close proximity of packet source.
                NearSource = other.GetComponent<PacketSource>();

                State = LinkControllerState.OverSource;
                Cursor.OnEnter(cursorEventArgs);
            }
            else if (CurrentLink != null && other.CompareTag("Sink"))
            {
                // In close proximity of packet sink.
                NearSink = other.GetComponent<PacketSink>();

                // End the current link at the sink.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();

                // Only finish the link if the destination matches the packet from the source.
                if (currentLinkComponent.Source.Peek().Destination == NearSink.Address)
                {
                    PlayClip(LinkSoundEffect.LinkCompleted);

                    currentLinkComponent.End(NearSink);

                    DestroyLink();

                    State = LinkControllerState.Inactive;
                    Cursor.OnExit(cursorEventArgs);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Source"))
            {
                NearSource = null;

                if (State == LinkControllerState.OverSource)
                {
                    State = LinkControllerState.Inactive;
                    Cursor.OnExit(cursorEventArgs);
                }
            }
            else if (other.CompareTag("Sink"))
            {
                NearSink = null;
            }
        }

        private void LinkSegment_OnSever(Link severed, SeverCause cause, float totalLength)
        {
            if (cause == SeverCause.Player)
            {
                if (!(severed.Packet is Virus))
                {
                    // Report dropped packet.
                    GameManager.ReportPacketDropped(severed.Packet);
                }

                PlayClip(LinkSoundEffect.LinkSevered);

                // Rumble controller on sever.
                StartCoroutine(RumbleWithDecay(SeverLinkRumbleLength, SeverLinkRumbleDecay));
            }

            // Restore the bandwidth that this link was using.
            Player.TotalBandwidth += totalLength;
        }

        private void LinkSegment_OnConstructionProgress(float deltaLength, 
            float totalLengthSoFar)
        {
            if (CurrentLink != null)
            {
                Player.TotalBandwidth -= deltaLength;
                if (Player.IsOutOfBandwidth())
                {
                    PlayClip(LinkSoundEffect.LinkBandwidthExceeded);

                    CurrentLink.GetComponent<Link>().End();

                    Cursor.OnExit(cursorEventArgs);
                    State = LinkControllerState.Inactive;
                }
                
                // Trigger haptic feedback proportional to the deltaLength as we draw the link.
                InputManager.RumbleController(IsRightHand, 
                    (ushort) (DrawingLinkRumbleLength * (deltaLength / DrawingLinkRumbleBaseLength)));
            }
        }

        private IEnumerator RumbleWithDecay(ushort startIntensity, ushort decay)
        {
            ushort intensity = startIntensity;
            while (intensity > 0)
            {
                InputManager.RumbleController(IsRightHand, intensity);
                if (decay > intensity)
                {
                    intensity = 0;
                }
                else
                {
                    intensity -= decay;
                }

                yield return null;
            }
        }

        private void DestroyLink()
        {
            CurrentLink = null;
        }
    }
}

