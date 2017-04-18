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

	public enum PowerupSpawnState
	{
		Unset,
		InWave,
		WaitingForNextWave
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
		public IEnumerator PowerupConfigEnumerator;
        public PacketConfig CurrentPacket;
		public PowerupConfig CurrentPowerup;

        [HideInInspector]
        public float LastPacketSpawnTime;
		[HideInInspector]
		public float LastPowerupSpawnTime;
        [HideInInspector]
        public float PacketEnteredPeriodTime;
		[HideInInspector]
		public float PowerupEnteredPeriodTime;
        public float WaveTime;
        public int NumActivePackets;

        public WaveLevelControllerState WaveState;
		public PowerupSpawnState PowerupState;

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
			PowerupConfigEnumerator = CurrentWave.Powerups.GetEnumerator();

            LastPacketSpawnTime = GameManager.GetInstance().GameTime();
            NumActivePackets = 0;
            WaveTime = 0.0f;
            PacketEnteredPeriodTime = LastPacketSpawnTime;

            WaveState = WaveLevelControllerState.InWavePrefix;
			PowerupState = PowerupSpawnState.WaitingForNextWave;
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
                WaveState = WaveLevelControllerState.WaitingForWaveClear;
                PacketEnteredPeriodTime = GameManager.GetInstance().GameTime();
            }
            else
            {
                CurrentPacket = (PacketConfig)PacketConfigEnumerator.Current;
            }
        }

		private void TrySpawnPowerup(float currentTime) {
			switch (PowerupState) {
			case PowerupSpawnState.InWave:
				float timeDiff = currentTime - PowerupEnteredPeriodTime;
				if (timeDiff > CurrentPowerup.Offset) {

					PowerupType type = CurrentPowerup.PowerupType;
					if (type == PowerupType.Unset) {
						type = GetRandomPowerupType ();
					}
					PowerupFactory.CreatePowerup (CurrentPowerup.PowerupType, CurrentPowerup.Location);

					TryAdvancePowerup();

				}
				break;
			case PowerupSpawnState.WaitingForNextWave:
				break;
			default:
				break;
			}
		}

		private void TryAdvancePowerup()
		{
			if (!PowerupConfigEnumerator.MoveNext ()) {
				Debug.Log (1);
				PowerupState = PowerupSpawnState.WaitingForNextWave;
				PowerupEnteredPeriodTime = GameManager.GetInstance ().GameTime ();
			} else {
				Debug.Log (2);
				CurrentPowerup = (PowerupConfig)PowerupConfigEnumerator.Current;
			}
		}

        public override void Update()
        {
            base.Update();

            if (!GameManager.GetInstance().IsPaused)
            {
				float currentTime = GameManager.GetInstance().GameTime();

                switch (WaveState)
                {
                    case WaveLevelControllerState.InWavePrefix:
                        if (currentTime - PacketEnteredPeriodTime > CurrentWave.MarginBefore)
                        {
                            WaveState = WaveLevelControllerState.InWave;
							PacketEnteredPeriodTime = currentTime;

							PowerupState = PowerupSpawnState.InWave;
							PowerupEnteredPeriodTime = currentTime;

                            TryAdvancePacket();
							TryAdvancePowerup ();
                        }
                        break;
					case WaveLevelControllerState.InWave:
						TrySpawnPowerup (currentTime);
                        WaveTime = currentTime - PacketEnteredPeriodTime; 
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
                            WaveState = WaveLevelControllerState.InWaveSuffix;
                            PacketEnteredPeriodTime = currentTime;
                        }
                        break;
                    case WaveLevelControllerState.InWaveSuffix:
                        if (currentTime - PacketEnteredPeriodTime > CurrentWave.MarginAfter)
                        {
                            if (WaveEnumerator.MoveNext())
                            {
                                // Next wave.
                                CurrentWave = (WaveConfig) WaveEnumerator.Current;
                                PacketConfigEnumerator = CurrentWave.Packets.GetEnumerator();
								PowerupConfigEnumerator = CurrentWave.Powerups.GetEnumerator ();

								WaveState = WaveLevelControllerState.InWavePrefix;
								PowerupState = PowerupSpawnState.WaitingForNextWave;

                                currentWaveNumber++;
                                if (WaveCleared != null)
                                {
                                    WaveCleared.Invoke(currentWaveNumber);
                                }
                            }
                            else
                            {
                                // Ran out of waves.
                                WaveState = WaveLevelControllerState.Finished;
                            }
                            PacketEnteredPeriodTime = currentTime;
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

