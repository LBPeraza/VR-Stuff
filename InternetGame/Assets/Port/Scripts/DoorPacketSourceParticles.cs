using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	
	public class DoorPacketSourceParticles : MonoBehaviour {

		public List<ParticleSystem> ParticleSystems;

		private bool isStopped;

		public bool IsStopped {
			get { return isStopped; }
		}

		public void Initialize() {
			isStopped = true;
			StopParticles ();
		}

		void SetColorNoAlpha(ParticleSystem p, Color c) {
			Color current = p.main.startColor.color;
			c.a = current.a;
			p.startColor = c;
		}

		public void StartParticles(Color particleColor) {
			foreach (ParticleSystem p in ParticleSystems) {
				if (isStopped) {
					p.Play ();
				}
				SetColorNoAlpha (p, particleColor);
			}
			isStopped = false;
		}

		public void StopParticles() {
			isStopped = true;
			foreach (ParticleSystem p in ParticleSystems) {
				p.Stop ();
			}
		}

	}
}