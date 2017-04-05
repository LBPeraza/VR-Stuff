using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	public class InvincibilityPowerup : Powerup {

		public override PowerupType Type {
			get { return PowerupType.Invincibility; }
		}

		public override int MaxCount {
			get { return 5; }
		}

		public override float Duration {
			get { return 10.0f; }
		}

	}
}
