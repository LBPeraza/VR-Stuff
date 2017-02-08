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

		public DoorPacketSourceParticles Particles;

        public Color BacklightOffColor = Color.black;

        public AudioSource DoorSounds;
        public AudioClip DoorOpenedSoundEffect;
        public AudioClip DoorClosedSoundEffect;

        public Light WarningLight;
        public float WarningLightMaxIntensity = 5.0f;
        public Color WarningLightColor = Color.red;
        public GameObject WarningBulb;

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

        public override void Initialize()
        {
            base.Initialize();

            currentDoorSetting = 100.0f; // Completely closed.
            SetApertureClose(currentDoorSetting);

            DisableBacklight();
        }

        protected override void OnNewPacketOnDeck(Packet p)
        {
            base.OnNewPacketOnDeck(p);

            p.OnSaved += OnPacketSaved;

            if (!HasUnfinishedLink())
            {
                SetBacklight(p.Color);
            }
        }

        public override void OnPacketWarning(Packet p)
        {
            base.OnPacketWarning(p);

            // SetFlashing(true);
        }

        public override void OnPacketHasExpired(Packet p)
        {
            base.OnPacketHasExpired(p);

            // SetFlashing(false);
        }

        public void OnPacketSaved(Packet p)
        {
            // SetFlashing(false);
        }

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (cause == SeverCause.UnfinishedLink)
            {
                // When the player doesnt finish a link, we need to stop flashing, or
                // start flashing the next packet's color.
                if (!HasUnfinishedLink())
                {
                    SetBacklight(Peek().Color);
                } else
                {
                    DisableBacklight();
                }
            }
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
               SetBacklight((Color)PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!IsOpen)
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

                    IsFlashing = false;
                }
            }
        }

        private IEnumerator Flash(Color c)
        {
            while (true)
            {
                float t = Mathf.PingPong(Time.fixedTime, 1.0f);
                Color toSet = Color.Lerp(Color.black, c, t);
                float intensity = t * WarningLightMaxIntensity;

                WarningLight.intensity = intensity;
                WarningBulb.GetComponent<Renderer>().material.SetColor("_EmissionColor", toSet);

                yield return null;
            }
        }

        private void SetBacklight(Color c)
        {
            var mat = Backlight.GetComponent<Renderer>().material;
            mat.color = c;
            mat.SetColor("_EmissionColor", c);
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

                SetApertureClose(currentDoorSetting);

                yield return null;
            }
        }
    }
}

