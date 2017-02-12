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

        public float DrawingLinkRumbleIntensity = 0.4f;
        public float SeverLinkRumbleIntensity = 0.7f;
        public ushort SeverLinkRumbleLength = 3000;
        public ushort SeverLinkRumbleDecay = 300;

        public LinkControllerState State;

        private static GameObject linkControllerContainer;
        private static LinkController linkController;

        private GameObject CurrentLink;
        private GameObject LastLink;

        private PacketSink NearSink;

        private CursorEventArgs cursorEventArgs;

        public static LinkController GetInstance()
        {
            if (linkController == null)
            {
                var linkControllerContainer = new GameObject("[LinkController]");
                linkController = linkControllerContainer.AddComponent<LinkController>(); 
            }

            return linkController;
        }

        // Use this for initialization
        public void Initialize(Player p)
        {
            Player = p;

            ObjectId = GetInstanceID();
            cursorEventArgs.senderId = ObjectId;

            if (Cursor == null)
            {
                Cursor = p.LinkCursor;
            }

            State = LinkControllerState.Inactive;

            Cursor.OnControllerFound += InitializeInput;

            LoadAudioClips();
        }

        private void InitializeInput(VRTK.VRTK_ControllerEvents input)
        {
            input.TriggerPressed += TriggerDown;
            input.TriggerReleased += TriggerUp;
        }

        private void LoadAudioClips()
        {
            if (AudioSource == null)
            {
                AudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
            }

            if (AuxillaryAudioSource == null)
            {
                AuxillaryAudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
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

        public void StartLink(PacketSource source, Transform linkPointer)
        {
            Debug.Log("Starting link!");
            if (CurrentLink == null
                && !Player.IsOutOfBandwidth()
                && source.QueuedPackets.Count > 0)
            {
                PlayClip(LinkSoundEffect.LinkDrawing);

                GameObject LinkContainer = LinkFactory.CreateLink(source);
                var linkSegment = LinkContainer.GetComponent<Link>();
                linkSegment.Initialize(source, linkPointer);

                // Listen for sever events.
                linkSegment.OnSever += LinkSegment_OnSever;
                linkSegment.OnConstructionProgress += LinkSegment_OnConstructionProgress;

                CurrentLink = LinkContainer;

                State = LinkControllerState.DrawingLink;
                Cursor.OnGrab(cursorEventArgs);
            }
        }

        public void EndLink(PacketSink sink = null)
        {
            if (sink == null)
            {
                // End the current link in the air.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();

                if (currentLinkComponent != null)
                {
                    currentLinkComponent.End();

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
            else
            {
                // End the current link at the sink.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();

                // Only finish the link if the destination matches the packet from the source.
                if (currentLinkComponent.Packet.Destination == sink.Address)
                {
                    PlayClip(LinkSoundEffect.LinkCompleted);

                    currentLinkComponent.End(sink);

                    DestroyLink();

                    State = LinkControllerState.Inactive;
                    Cursor.OnExit(cursorEventArgs);
                }
            }
        }

        public void TriggerDown(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            
        }

        public void TriggerUp(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            if (CurrentLink != null && NearSink == null)
            {
                // End the current link in the air.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();

                if (currentLinkComponent != null)
                {
                    currentLinkComponent.End();

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
            float volume = AudioMix.GeneralSoundEffectVolume;

            switch (effect)
            {
                case LinkSoundEffect.LinkBandwidthExceeded:
                    volume = AudioMix.LinkDepletedSoundEffectVolume;
                    clip = LinkDepleted;
                    break;
                case LinkSoundEffect.LinkCompleted:
                    volume = AudioMix.LinkCompletedSoundEffectVolume;
                    clip = LinkConnected;
                    break;
                case LinkSoundEffect.LinkDrawing:
                    volume = AudioMix.LinkDrawingSoundEffectVolume;
                    repeat = true;
                    clip = LinkDrawing;
                    break;
                case LinkSoundEffect.LinkSevered:
                    source = AuxillaryAudioSource;
                    volume = AudioMix.LinkSeveredSoundEffectVolume;
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
                if (currentLinkComponent.Packet.Destination == NearSink.Address)
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
            if (cause == SeverCause.Player || cause == SeverCause.PlayerPreventedVirus)
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
                if (Cursor.ControllerActions != null)
                {
                    Cursor.ControllerActions.TriggerHapticPulse(DrawingLinkRumbleIntensity);
                }
            }
        }

        private IEnumerator RumbleWithDecay(ushort startIntensity, ushort decay)
        {
            ushort intensity = startIntensity;
            while (intensity > 0)
            {
                if (Cursor.ControllerActions != null)
                {
                    Cursor.ControllerActions.TriggerHapticPulse(SeverLinkRumbleIntensity);
                }

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

