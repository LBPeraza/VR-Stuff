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

        [Header("Rumble Settings")]
        public float DrawingLinkRumbleIntensity = 0.1f;
        public float SeverLinkRumbleIntensity = 0.8f;
        public ushort SeverLinkRumbleLength = 150;
        public ushort SeverLinkRumbleInterval = 10;
        public float EndLinkRumbleBaseIntensity = 0.3f;
        public ushort EndLinkRumbleLength = 150;
        public ushort EndLinkRubmleInterval = 30;

        public LinkControllerState State;

        private static GameObject linkControllerContainer;
        private static LinkController linkController;

        private GameObject CurrentLink;
        private GameObject LastLink;

        private PacketSink NearSink;

        private CursorEventArgs cursorEventArgs;

        public static void ResetInstance()
        {
            if (linkController != null)
            {
                DestroyImmediate(linkController.gameObject);
            }
            linkController = null;
        }

        public static LinkController GetInstance()
        {
            if (linkController == null)
            {
                var linkControllerContainer = new GameObject("[LinkController]");
                linkController = linkControllerContainer.AddComponent<LinkController>(); 
            }

            return linkController;
        }

        public void LoadResources()
        {
            LoadAudioClips();
        }

        // Use this for initialization
        public void Initialize(Player p)
        {
            LoadResources();

            Player = p;

            ObjectId = GetInstanceID();
            cursorEventArgs.senderId = ObjectId;

            if (Cursor == null)
            {
                Cursor = p.LinkCursor;
            }

            State = LinkControllerState.Inactive;

            Cursor.OnControllerFound += InitializeInput;
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
                LinkConnected = Resources.Load<AudioClip>("Audio/link_connect");
            }

            if (LinkDepleted == null)
            {
                LinkDepleted = Resources.Load<AudioClip>("Audio/link_depleted");
            }

            if (LinkSevered == null)
            {
                LinkSevered = Resources.Load<AudioClip>("Audio/link_sever");
            }

            if (LinkDrawing == null)
            {
                LinkDrawing = Resources.Load<AudioClip>("Audio/link_drawing");
            }
        }

        private void InitializeInput(VRTK.VRTK_ControllerEvents input)
        {
        }

        public Link StartLink(Cursor cursor, PacketSource source, Connector connector)
        {
            if (CurrentLink == null
                && !Player.IsOutOfBandwidth()
                && source.QueuedPackets.Count > 0)
            {
                Cursor = cursor;

                PlayClip(LinkSoundEffect.LinkDrawing);

                GameObject LinkContainer = LinkFactory.CreateLink(source, source.Peek());
                connector.transform.SetParent(LinkContainer.transform);

                var linkSegment = LinkContainer.GetComponent<Link>();
                linkSegment.Initialize(source, connector);

                // Listen for sever events.
                linkSegment.OnSever += LinkSegment_OnSever;
                linkSegment.ConstructionProgress += LinkSegment_OnConstructionProgress;

                CurrentLink = LinkContainer;

                State = LinkControllerState.DrawingLink;

                return linkSegment;
            }

            return null;
        }

        public void OnConnectorSnapping(Connector c)
        {
            StartCoroutine(RumbleWithDecay(
                EndLinkRumbleLength,
                EndLinkRubmleInterval,
                EndLinkRumbleBaseIntensity));
        }

        public void EndLink(PacketSink sink = null)
        {
            if (CurrentLink != null)
            {
                // End the current link in the air.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();

                if (sink == null)
                {
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
                    Cursor.OnDrop(cursorEventArgs);

                    StopClips();

                    DestroyLink();
                }
                else
                {
                    // Only finish the link if the destination matches the packet from the source.
                    if (currentLinkComponent == null)
                    {
                        Debug.LogError("current link component is invalid! :(");
                    }

                    if (currentLinkComponent.Packet.Destination == sink.Address)
                    {
                        PlayClip(LinkSoundEffect.LinkCompleted);

                        currentLinkComponent.End(sink);

                        State = LinkControllerState.Inactive;
                        Cursor.OnDrop(cursorEventArgs);

                        DestroyLink();
                    }
                }
            }
            else
            {
                Debug.LogWarning("LinkController cannot end a link when one is not started.");
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

        private void LinkSegment_OnSever(Link severed, SeverCause cause, float totalLength)
        {
            if (cause == SeverCause.Player || cause == SeverCause.PlayerPreventedVirus)
            {
                if (!(severed.Packet.Payload is Virus))
                {
                    // Report dropped packet.
                    GameManager.GetInstance().ReportPacketDropped(severed.Packet);
                }

                PlayClip(LinkSoundEffect.LinkSevered);

                // Rumble controller on sever.
                StartCoroutine(RumbleWithDecay(
                    SeverLinkRumbleLength, 
                    SeverLinkRumbleInterval, 
                    SeverLinkRumbleIntensity));
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

                    Cursor.OnDrop(cursorEventArgs);
                    State = LinkControllerState.Inactive;
                }

                // Trigger haptic feedback proportional to the deltaLength as we draw the link.
                if (Cursor.ControllerActions != null)
                {
                    Cursor.ControllerActions.TriggerHapticPulse(DrawingLinkRumbleIntensity);
                }
            }
        }

        private IEnumerator RumbleWithDecay(ushort duration, ushort interval, float intensity)
        {
            ushort length = 0;
            float intervalInSeconds = (float)(interval) / 1000.0f;
            while (length < duration)
            {
                if (Cursor.ControllerActions != null)
                {
                    Cursor.ControllerActions.TriggerHapticPulse(intensity);
                }

                length += interval;
                yield return new WaitForSeconds(intervalInSeconds);
            }
        }

        private void DestroyLink()
        {
            CurrentLink = null;
        }
    }
}

