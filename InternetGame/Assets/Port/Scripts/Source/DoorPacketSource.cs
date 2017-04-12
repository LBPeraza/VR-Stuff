using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    enum DoorSoundEffect
    {
        DoorOpen,
        DoorClosed
    }

    public class DoorPacketSource : PacketSource
    {
        [Header("Backlight Settings")]
        public GameObject Backlight;
        public Color BacklightOffColor = Color.black;

        [Header("Warning Light")]
        public FlashingLight WarningLight;

        [Header("Shutter Settings")]
        public GameObject Shutter;
        public float DoorOpenRate;

        [Header("Particle Settings")]
		public DoorPacketSourceParticles Particles;
        
        [Header("Connector Settings")]
        public bool EnableConnector = true;
        public Transform ConnectorHidden;
        public Transform ConnectorExposed;

        protected bool IsOpen = false;
        protected AudioSource DoorSounds;
        protected AudioClip DoorOpenedSoundEffect;
        protected AudioClip DoorClosedSoundEffect;

        private float currentDoorSetting;

        Coroutine doorAnimation;

        public override void InitializeAudio()
        {
            base.InitializeAudio();

            DoorSounds = AudioMix.AddAudioSourceTo(this.gameObject);
        }

        private void TryInstantiateConnector()
        {
            if (Connector == null)
            {
                Connector = ConnectorFactory.CreateDefaultConnector(this, transform);
                Connector.transform.localPosition = ConnectorHidden.localPosition;
            }
        }

        public override void LoadResources()
        {
            base.LoadResources();

            DoorOpenedSoundEffect = Resources.Load<AudioClip>("Audio/door_opened");
            DoorClosedSoundEffect = Resources.Load<AudioClip>("Audio/door_closed");
        }

        public override void Initialize()
        {
            base.Initialize();

            Particles.Initialize();
            WarningLight.Initialize();

            currentDoorSetting = 100.0f; // Completely closed.
            SetApertureClose(currentDoorSetting);

            DisableBacklight();
        }

        protected override void OnNewPacketOnDeck(Packet p)
        {
            base.OnNewPacketOnDeck(p);

            p.Saved += OnPacketSaved;
            
            if (EnableConnector)
            {
                TryInstantiateConnector();         
            }
            
            UpdateBacklight();
        }

        protected override void OnEmptied()
        {
            base.OnEmptied();

            SetFlashing(false);

            UpdateBacklight();
        }

        public override void OnPacketWarning(Packet p)
        {
            base.OnPacketWarning(p);

            SetFlashing(true);
        }

        public void OnPacketSaved(Packet p)
        {
            SetFlashing(false);
        }

        public override void OnLevelCleared()
        {
            base.OnLevelCleared();

            WarningLight.FlashRainbow();
        }

        protected void SetFlashing(bool flashing)
        {
            WarningLight.SetFlashing(flashing);
        }

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (cause == SeverCause.UnfinishedLink)
            {
                if (!HasUnfinishedLink() && !IsEmpty())
                {
                    if (EnableConnector)
                    {
                        TryInstantiateConnector();
                    }
                } else
                {
                    DisableBacklight();
                }
            }
        }

        public override void OnLinkStarted(Link l)
        {
            base.OnLinkStarted(l);

            Connector = null;
        }

        public void OnTriggerEnter(Collider other)
        {
            var cursor = GameManager.GetInstance().Player.GetCursorFromCollider(other);
            if (other.CompareTag("Player") 
                && other.name == "FingersHitbox"
                && cursor.IsPrimary)
            {
                // Don't open if the player is drawing a link.
                bool cursorIsDrawingLink = LinkController.GetInstance().State == LinkControllerState.DrawingLink;
                if (!cursorIsDrawingLink && !IsOpen && !IsEmpty())
                {
                    cursor.OnEnter(Cursor.DefaultCursorEventArgs);

                    StartOpenDoor();

                    IsOpen = true;
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.name == "FingersHitbox")
            {
                if (IsOpen)
                {
                    Cursor cursor = GameManager.GetInstance().Player.GetCursorFromCollider(other);
                    cursor.OnExit(Cursor.DefaultCursorEventArgs);

                    StartCloseDoor();

                    IsOpen = false;
                }
            }
        }

        private void PlayDoorSoundEffect(DoorSoundEffect soundEffect)
        {
            AudioSource source = DoorSounds;
            float volume = AudioMix.GeneralSoundEffectVolume;
            AudioClip clip = DoorClosedSoundEffect;
            float offset = 0.0f;

            switch (soundEffect)
            {
                case DoorSoundEffect.DoorClosed:
                    clip = DoorClosedSoundEffect;
                    volume = AudioMix.PortDoorClosesSoundEffectVolume;
                    offset = AudioMix.PortDoorClosesSoundEffectOffset;
                    break;
                case DoorSoundEffect.DoorOpen:
                    clip = DoorOpenedSoundEffect;
                    volume = AudioMix.PortDoorOpensSoundEffectVolume;
                    offset = AudioMix.PortDoorOpensSoundEffectOffset;
                    break;
            }

            source.Stop();
            source.clip = clip;
            source.volume = volume;
            source.time = offset;
            source.Play();
        }

        private void UpdateBacklight()
        {
            if (IsEmpty())
            {
                DisableBacklight();
            }
            else
            {
                SetBacklight((Color)GameUtils.AddressToColor[Peek().Destination]);
            }
        }

        private void SetBacklight(Color c)
        {
            var mat = Backlight.GetComponent<Renderer>().material;
            mat.color = c;
			Particles.StartParticles (c);
        }

        private void DisableBacklight()
        {
            SetBacklight(BacklightOffColor);
			Particles.StopParticles ();
        }

        /// <summary>
        /// Takes a float from 0-100 and sets the aperture that percent closed.
        /// </summary>
        /// <param name="percentClose"></param>
        private void SetApertureClose(float percentClose)
        {
            var meshRenderer = Shutter.GetComponent<SkinnedMeshRenderer>();
            meshRenderer.SetBlendShapeWeight(0, currentDoorSetting);
        }

        private void SetConnectorHidden(float percentHidden)
        {
            Connector.transform.localPosition = Vector3.Lerp(
                ConnectorExposed.localPosition,
                ConnectorHidden.localPosition,
                percentHidden / 100.0f);
        }

        private void StartOpenDoor()
        {
            if (doorAnimation != null)
            {
                StopCoroutine(doorAnimation);
            }

            doorAnimation = StartCoroutine(OpenDoor());

            PlayDoorSoundEffect(DoorSoundEffect.DoorOpen);
        }

        private IEnumerator OpenDoor()
        {
            while (currentDoorSetting > 0)
            {
                currentDoorSetting -= DoorOpenRate;

                if (EnableConnector && Connector != null)
                {
                    SetConnectorHidden(currentDoorSetting);
                }
                SetApertureClose(currentDoorSetting);

                yield return null;
            }
        }

        private void StartCloseDoor()
        {
            if (doorAnimation != null)
            {
                StopCoroutine(doorAnimation);
            }

            doorAnimation = StartCoroutine(CloseDoor());

            PlayDoorSoundEffect(DoorSoundEffect.DoorClosed);
        }

        private IEnumerator CloseDoor()
        {
            while (currentDoorSetting < 100.0f)
            {
                currentDoorSetting += DoorOpenRate;

                if (EnableConnector && Connector != null)
                {
                    SetConnectorHidden(currentDoorSetting);
                }
                SetApertureClose(currentDoorSetting);

                yield return null;
            }
        }
    }
}

