using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class AudioMix : MonoBehaviour
    {
        public static float BackgroundMusicVolume = 0.3f;
        public static float GeneralSoundEffectVolume = 0.75f;

        public static float VirusApproachingSoundEffectVolume = 1.0f;
        public static float VirusStrikesSoundEffectVolume = 1.0f;

        public static float PacketArrivesSoundEffectVolume = 0.75f;
        public static float PacketExpiresSoundEffectVolume = 1.0f;
        public static float PacketNearingExpirationSoundEffectVolume = 1.0f;

        public static float PortDoorOpensSoundEffectVolume = 1.0f;
        public static float PortDoorClosesSoundEffectVolume = 1.0f;

        public static float PortDoorOpensSoundEffectOffset = 0.2f;
        public static float PortDoorClosesSoundEffectOffset = 0.2f;

        public static float LinkDrawingSoundEffectVolume = 0.6f;
        public static float LinkCompletedSoundEffectVolume = 0.7f;
        public static float LinkSeveredSoundEffectVolume = 0.7f;
        public static float LinkDepletedSoundEffectVolume = 0.7f;

        public static AudioSource AddAudioSourceTo(GameObject parent)
        {
            var audioSource = parent.AddComponent<AudioSource>();
            audioSource.spatialize = true;
            audioSource.playOnAwake = false;

            return audioSource;
        }
    }
}
