using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum WaveLevelControllerState
    {
        Unset,
        InWave,
        InWavePrefix,
        WaitingForWaveClear,
        InWaveSuffix,
        Finished
    }

    public class WaveLevelController : LevelController
    {
        [HideInInspector]
        public LevelControllerConfig LevelControllerConfig;
        [HideInInspector]
        public IEnumerator WaveEnumerator;
        public WaveConfig CurrentWave;
        [HideInInspector]
        public IEnumerator PacketConfigEnumerator;
        public PacketConfig CurrentPacket;

        [HideInInspector]
        public float LastSpawnTime;
        [HideInInspector]
        public float EnteredPeriodTime;
        public float WaveTime;
        public int NumActivePackets;

        public WaveLevelControllerState State;

        public delegate void OnWaveCleared(int currentWave);
        public event OnWaveCleared WaveCleared;

        private bool alertedGameManagerToGameCleared = false;
        private int currentWaveNumber = 1;

        public virtual void LoadLevelConfig(LevelControllerConfig config)
        {
            LevelControllerConfig = config;

            WaveEnumerator = config.Waves.GetEnumerator();
            WaveEnumerator.MoveNext();
            CurrentWave = (WaveConfig) WaveEnumerator.Current;

            PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();

            LastSpawnTime = GameManager.GetInstance().GameTime();
            NumActivePackets = 0;
            WaveTime = 0.0f;
            EnteredPeriodTime = LastSpawnTime;

            State = WaveLevelControllerState.InWavePrefix;
        }

        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            LoadLevelConfig(manager.LevelParameters.LevelControllerConfig);
            currentWaveNumber = 1;

            // Find a wave display(s) and initialize it.
            foreach (var waveDisplay in GameObject.FindObjectsOfType<WaveDisplay>())
            {
                waveDisplay.Initialize(this);
            }
        }

        protected virtual void OnPacketDestroyed(Packet p)
        {
            NumActivePackets--;
        }

        private void TryAdvancePacket()
        {
            // Advance iterator.
            if (!PacketConfigEnumerator.MoveNext())
            {
                // Reached end of wave.
                State = WaveLevelControllerState.WaitingForWaveClear;
                EnteredPeriodTime = GameManager.GetInstance().GameTime();
            }
            else
            {
                CurrentPacket = (PacketConfig)PacketConfigEnumerator.Current;
            }
        }

        public override void Update()
        {
            base.Update();

            if (!GameManager.GetInstance().IsPaused)
            {
                float currentTime = GameManager.GetInstance().GameTime();

                switch (State)
                {
                    case WaveLevelControllerState.InWavePrefix:
                        if (currentTime - EnteredPeriodTime > CurrentWave.MarginBefore)
                        {
                            State = WaveLevelControllerState.InWave;
                            EnteredPeriodTime = currentTime;

                            TryAdvancePacket();
                        }
                        break;
                    case WaveLevelControllerState.InWave:
                        WaveTime = currentTime - EnteredPeriodTime; 
                        if (WaveTime > CurrentPacket.Offset)
                        {
                            // Time to spawn this packet.
                            PacketSource source = null;
                            if (CurrentPacket.Source != null && CurrentPacket.Source != "")
                            {
                                // Config specifies a source port.
                                var desired = (PacketSource)GameUtils.AddressToSource[CurrentPacket.Source];
                                if (!desired.IsFull())
                                {
                                    source = desired;
                                }
                            }
                            
                            if (source == null)
                            {
                                // No source port specified -- randomly find one.
                                source = GetRandomSource();
                            }

                            if (source != null)
                            {
                                PacketSink sink = (PacketSink)GameUtils.AddressToSink[CurrentPacket.Destination];
								if (sink == null) {
									sink = GetRandomSink ();
								}

                                NumActivePackets++;

                                var packet = PacketFactory.CreateLoadedPacket(source, sink, CurrentPacket.Type);
                                if (CurrentPacket.Size > 0)
                                {
                                    packet.Payload.Size = CurrentPacket.Size;
                                }
                                packet.Destroyed += OnPacketDestroyed;

                                TryAdvancePacket();
                            }
                        }
                        break;
                    case WaveLevelControllerState.WaitingForWaveClear:
                        if (NumActivePackets == 0)
                        {
                            // Wave is cleared!
                            State = WaveLevelControllerState.InWaveSuffix;
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                    case WaveLevelControllerState.InWaveSuffix:
                        if (currentTime - EnteredPeriodTime > CurrentWave.MarginAfter)
                        {
                            if (WaveEnumerator.MoveNext())
                            {
                                // Next wave.
                                CurrentWave = (WaveConfig) WaveEnumerator.Current;
                                PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();

                                State = WaveLevelControllerState.InWavePrefix;

                                currentWaveNumber++;
                                if (WaveCleared != null)
                                {
                                    WaveCleared.Invoke(currentWaveNumber);
                                }
                            }
                            else
                            {
                                // Ran out of waves.
                                State = WaveLevelControllerState.Finished;
                            }
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                    case WaveLevelControllerState.Finished:
                        if (!alertedGameManagerToGameCleared)
                        {
                            alertedGameManagerToGameCleared = true;
                            GameManager.LevelCleared();
                        }
                        break;
                }
            }
        }

    }
}

