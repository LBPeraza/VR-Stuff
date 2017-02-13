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
        public GameObject Backlight;
        public GameObject Shutter;
        public float DoorOpenRate;
        public bool EnableConnector = true;

		public DoorPacketSourceParticles Particles;

        public Transform ConnectorHidden;
        public Transform ConnectorExposed;

        public Color BacklightOffColor = Color.black;

        public AudioSource DoorSounds;
        public AudioClip DoorOpenedSoundEffect;
        public AudioClip DoorClosedSoundEffect;

        public Light WarningLight;
        public float WarningLightMaxIntensity = 5.0f;
        public float WarningLightMinIntensity = 1.0f;
        public Color WarningLightColor = Color.red;
        public GameObject WarningBulb;
        public float FlashRate = 2.0f;

        public bool IsOpen = false;
        public bool IsFlashing = false;

        private float currentDoorSetting;

        Coroutine doorAnimation;
        Coroutine flashingAnimation;

        public override void InitializeAudio()
        {
            base.InitializeAudio();

            DoorSounds = AudioMix.AddAudioSourceTo(this.gameObject);

            DoorOpenedSoundEffect = Resources.Load<AudioClip>("door_opened");
            DoorClosedSoundEffect = Resources.Load<AudioClip>("door_closed");

			Particles.Initialize ();
        }

        private void InstantiateConnector()
        {
            Connector = ConnectorFactory.CreateDefaultConnector(this, transform);
            Connector.transform.localPosition = ConnectorHidden.localPosition;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (EnableConnector)
            {
                InstantiateConnector();
            }

            currentDoorSetting = 100.0f; // Completely closed.
            SetApertureClose(currentDoorSetting);

            if (WarningBulb != null)
            {
                Material copy = new Material(WarningBulb.GetComponent<Renderer>().material);
                WarningBulb.GetComponent<Renderer>().material = copy;
            }

            DisableBacklight();
        }

        protected override void OnNewPacketOnDeck(Packet p)
        {
            base.OnNewPacketOnDeck(p);

            p.OnSaved += OnPacketSaved;

            if (!HasUnfinishedLink())
            {
                if (EnableConnector)
                {
                    InstantiateConnector();
                }

                SetBacklight(p.Color);
            }
        }

        public override void OnPacketWarning(Packet p)
        {
            base.OnPacketWarning(p);

            SetFlashing(true);
        }

        public override void OnPacketHasExpired(Packet p)
        {
            base.OnPacketHasExpired(p);

            if (IsEmpty())
            {
                DisableBacklight();
            } 
            SetFlashing(false);
        }

        public void OnPacketSaved(Packet p)
        {
            SetFlashing(false);
        }

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (cause == SeverCause.UnfinishedLink)
            {
                // When the player doesnt finish a link, we need to stop flashing, or
                // start flashing the next packet's color.
                if (!HasUnfinishedLink() && !IsEmpty())
                {
                    if (EnableConnector)
                    {
                        InstantiateConnector();
                    }
                    SetBacklight(Peek().Color);
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

        protected override void OnTransmissionStarted(Link l, Packet p)
        {
            base.OnTransmissionStarted(l, p);

            if (IsEmpty())
            {
                DisableBacklight();
            }
            else
            {
                if (EnableConnector)
                {
                    InstantiateConnector();
                }
                SetBacklight((Color)PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Don't open if the player is drawing a link.
                Cursor cursor = other.transform.parent.GetComponent<Cursor>();
                bool cursorIsDrawingLink = LinkController.GetInstance().State == LinkControllerState.DrawingLink;
                if (!cursorIsDrawingLink && !IsOpen)
                {
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

        private void SetFlashing(bool flashing)
        {
            if (flashing != IsFlashing)
            {
                if (flashing)
                {
                    flashingAnimation = StartCoroutine(Flash(WarningLightColor));

                    IsFlashing = true;
                }
                else
                {
                    StopCoroutine(flashingAnimation);
                    SetBulb(Color.black, 0.0f);

                    IsFlashing = false;
                }
            }
        }
        
        private void SetBulb(Color c, float intensity)
        {
            WarningLight.intensity = intensity;
            WarningBulb.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
        }

        private IEnumerator Flash(Color c)
        {
            while (true)
            {
                float t = 0.5f * Mathf.Cos(Time.fixedTime * FlashRate) + 0.5f;
                Color toSet = Color.Lerp(Color.black, c, t);
                float intensity = WarningLightMinIntensity + (t * (WarningLightMaxIntensity - WarningLightMinIntensity));

                SetBulb(toSet, intensity);

                yield return null;
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

