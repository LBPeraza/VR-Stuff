using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public struct SoundtrackMetadata
    {
        public int BeginLowBand;
        public int EndLowBand;
        public float MaxIntensity;
        public float IntensityDropoff;
        public float IntensityThreshold;
    }

    public class SoundtrackUtility
    {
        public static SoundtrackMetadata GetMetadata(AudioClip soundtrack)
        {
            SoundtrackMetadata metadata;
            metadata.IntensityDropoff = 0.005f;
            metadata.IntensityThreshold = 0.5f;

            switch (soundtrack.name)
            {
                case "deep_dream_machine":
                    metadata.BeginLowBand = 0;
                    metadata.EndLowBand = 200;
                    metadata.MaxIntensity = 0.08f;
                    metadata.IntensityDropoff = 0.02f;
                    break;
                case "ethereal_colosseum":
                    metadata.BeginLowBand = 0;
                    metadata.EndLowBand = 100;
                    metadata.MaxIntensity = 0.2f;
                    break;
                default:
                    metadata.BeginLowBand = 0;
                    metadata.EndLowBand = 100;
                    metadata.MaxIntensity = 0.2f;
                    break;
            }

            return metadata;
        }
    }
}
