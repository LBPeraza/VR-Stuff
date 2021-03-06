using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	public class TimeFreezePowerup : Powerup {

		public override PowerupType Type {
			get { return PowerupType.TimeFreeze; }
		}

		public override float Duration {
			get { return 10.0f; }
		}

		public override int MaxCount {
			get { return 5; }
		}

		public override Color PowerupColor {
			get { return Color.cyan; }
		}

	}
}
