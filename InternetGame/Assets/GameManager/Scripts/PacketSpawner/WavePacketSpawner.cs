using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum WavePacketSpawnerState
    {
        Unset,
        InWave,
        InWavePrefix,
        WaitingForWaveClear,
        InWaveSuffix,
        Finished
    }

    public class WavePacketSpawner : PacketSpawner
    {
        [HideInInspector]
        public PacketSpawnerConfig PacketSpawnerConfig;
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

        public WavePacketSpawnerState State;

        public delegate void OnWaveCleared(int currentWave);
        public event OnWaveCleared WaveCleared;

        private bool alertedGameManagerToGameCleared = false;
        private int currentWaveNumber = 1;

        public virtual void LoadLevelConfig(PacketSpawnerConfig config)
        {
            PacketSpawnerConfig = config;

            WaveEnumerator = config.Waves.GetEnumerator();
            WaveEnumerator.MoveNext();
            CurrentWave = (WaveConfig) WaveEnumerator.Current;

            PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();

            LastSpawnTime = GameManager.GetInstance().GameTime();
            NumActivePackets = 0;
            WaveTime = 0.0f;
            EnteredPeriodTime = LastSpawnTime;

            State = WavePacketSpawnerState.InWavePrefix;
        }

        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            LoadLevelConfig(manager.LevelParameters.PacketSpawnerConfig);
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
                State = WavePacketSpawnerState.WaitingForWaveClear;
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
                    case WavePacketSpawnerState.InWavePrefix:
                        if (currentTime - EnteredPeriodTime > CurrentWave.MarginBefore)
                        {
                            State = WavePacketSpawnerState.InWave;
                            EnteredPeriodTime = currentTime;

                            TryAdvancePacket();
                        }
                        break;
                    case WavePacketSpawnerState.InWave:
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
                    case WavePacketSpawnerState.WaitingForWaveClear:
                        if (NumActivePackets == 0)
                        {
                            // Wave is cleared!
                            State = WavePacketSpawnerState.InWaveSuffix;
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                    case WavePacketSpawnerState.InWaveSuffix:
                        if (currentTime - EnteredPeriodTime > CurrentWave.MarginAfter)
                        {
                            if (WaveEnumerator.MoveNext())
                            {
                                // Next wave.
                                CurrentWave = (WaveConfig) WaveEnumerator.Current;
                                PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();

                                State = WavePacketSpawnerState.InWavePrefix;

                                currentWaveNumber++;
                                if (WaveCleared != null)
                                {
                                    WaveCleared.Invoke(currentWaveNumber);
                                }
                            }
                            else
                            {
                                // Ran out of waves.
                                State = WavePacketSpawnerState.Finished;
                            }
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                    case WavePacketSpawnerState.Finished:
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

