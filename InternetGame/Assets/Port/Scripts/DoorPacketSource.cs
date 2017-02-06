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

        public Color BacklightOffColor = Color.black;

        public AudioSource DoorSounds;
        public AudioClip DoorOpenedSoundEffect;
        public AudioClip DoorClosedSoundEffect;

        public bool IsOpen = false;

        private float currentDoorSetting;

        Coroutine doorAnimation;

        public override void InitializeAudio()
        {
            base.InitializeAudio();

            DoorSounds = AudioMix.AddAudioSourceTo(this.gameObject);

            DoorOpenedSoundEffect = Resources.Load<AudioClip>("door_opened");
            DoorClosedSoundEffect = Resources.Load<AudioClip>("door_closed");
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

            if (!HasUnfinishedLink())
            {
                SetBacklight(p.Color);
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

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (cause == SeverCause.UnfinishedLink)
            {
                // When the player doesnt finish a link, we need to stop flashing.
                DisableBacklight();
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

        private void SetBacklight(Color c)
        {
            var mat = Backlight.GetComponent<Renderer>().material;
            mat.color = c;
            mat.SetColor("_EmissionColor", c);
        }

        private void DisableBacklight()
        {
            SetBacklight(BacklightOffColor);
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

