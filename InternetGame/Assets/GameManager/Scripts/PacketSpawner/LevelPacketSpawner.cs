using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public struct LevelPacketSpawnerConfig
    {
        public WaveConfig[] Waves;       
    }

    public struct WaveConfig
    {
        public float MarginBefore;
        public float MarginAfter;

        public PacketConfig[] Packets;
    }

    public struct PacketConfig
    {
        public string Destination;
        public PacketPayloadType Type;
        public int Size;

        public string Source;

        public float Offset;
    }

    public enum LevelPacketSpawnerState
    {
        Unset,
        InWave,
        InWavePrefix,
        WaitingForWaveClear,
        InWaveSuffix,
        Finished
    }

    public class LevelPacketSpawner : PacketSpawner
    {
        [HideInInspector]
        public LevelPacketSpawnerConfig PacketSpawnerConfig;
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

        public LevelPacketSpawnerState State;

        public virtual void LoadLevelConfig(LevelPacketSpawnerConfig config)
        {
            PacketSpawnerConfig = config;

            WaveEnumerator = config.Waves.GetEnumerator();
            WaveEnumerator.MoveNext();
            CurrentWave = (WaveConfig) WaveEnumerator.Current;

            PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();
            PacketConfigEnumerator.MoveNext();
            CurrentPacket = (PacketConfig) PacketConfigEnumerator.Current;

            LastSpawnTime = GameManager.GetInstance().GameTime();
            NumActivePackets = 0;
            WaveTime = 0.0f;
            EnteredPeriodTime = LastSpawnTime;

            State = LevelPacketSpawnerState.InWavePrefix;
        }

        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            LoadLevelConfig(manager.LevelParameters.PacketSpawnConfig);
        }

        protected virtual void OnPacketDestroyed(Packet p)
        {
            NumActivePackets--;
        }

        public override void Update()
        {
            base.Update();

            if (!GameManager.GetInstance().IsPaused)
            {
                float currentTime = GameManager.GetInstance().GameTime();

                switch (State)
                {
                    case LevelPacketSpawnerState.InWavePrefix:
                        if (currentTime - EnteredPeriodTime > CurrentWave.MarginBefore)
                        {
                            State = LevelPacketSpawnerState.InWave;
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                    case LevelPacketSpawnerState.InWave:
                        WaveTime = currentTime - EnteredPeriodTime; 
                        if (WaveTime > CurrentPacket.Offset)
                        {
                            // Time to spawn this packet.
                            PacketSource source;
                            if (CurrentPacket.Source != null)
                            {
                                // Config specifies a source port.
                                source = (PacketSource)GameUtils.AddressToSource[CurrentPacket.Source];
                            }
                            else
                            {
                                // No source port specified -- randomly find one.
                                source = GetRandomSource();

                                if (source != null)
                                {
                                    PacketSink sink = (PacketSink)GameUtils.AddressToSink[CurrentPacket.Destination];

                                    NumActivePackets++;

                                    var packet = PacketFactory.CreateLoadedPacket(source, sink, CurrentPacket.Type);
                                    packet.Destroyed += OnPacketDestroyed;

                                    // Advance iterator.
                                    if (!PacketConfigEnumerator.MoveNext())
                                    {
                                        // Reached end of wave.
                                        State = LevelPacketSpawnerState.WaitingForWaveClear;
                                        EnteredPeriodTime = currentTime;
                                    }
                                    else
                                    {
                                        CurrentPacket = (PacketConfig) PacketConfigEnumerator.Current;
                                    }
                                }
                            }
                        }
                        break;
                    case LevelPacketSpawnerState.WaitingForWaveClear:
                        if (NumActivePackets == 0)
                        {
                            // Wave is cleared!
                            State = LevelPacketSpawnerState.InWaveSuffix;
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                    case LevelPacketSpawnerState.InWaveSuffix:
                        if (currentTime - EnteredPeriodTime > CurrentWave.MarginAfter)
                        {
                            if (WaveEnumerator.MoveNext())
                            {
                                // Next wave.
                                CurrentWave = (WaveConfig) WaveEnumerator.Current;
                                PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();
                                PacketConfigEnumerator.MoveNext();
                                CurrentPacket = (PacketConfig) PacketConfigEnumerator.Current;
                                State = LevelPacketSpawnerState.InWavePrefix;
                            }
                            else
                            {
                                // Ran out of waves.
                                State = LevelPacketSpawnerState.Finished;
                            }
                            EnteredPeriodTime = currentTime;
                        }
                        break;
                }
            }
        }

    }
}

