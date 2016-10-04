using UnityEngine;
using System.Collections;

namespace TowerGame {
	namespace Enemy {

    	public enum EnemyState {
    		Unitialized = 4,
    		InTransit = 1,
    		Dead = 2,
    		AtTarget = 3,
    		Default = 0
    	}

		public abstract class Enemy : AttackableEntity {
        	[HideInInspector]
        	public static string ENEMY_TAG = "enemy";

            public float UpdateInterval;
           
            public float MaxHealth;
            public float CurrentHealth;
        	
        	[HideInInspector]
        	public bool IsDead { 
        		get { 
        			return State == EnemyState.Dead; 
        		}
        	}

        	public float PrimaryAttackDamage;
        	public float PrimaryAttackRange;
        	
            protected EnemyState State;
            protected AttackableEntity Target;

        	public Enemy() {
        		CurrentHealth = MaxHealth;

        		State = EnemyState.Unitialized;
        	}

            public void Start()
            {
                this.transform.tag = ENEMY_TAG;
            }

            public void SetTarget(AttackableEntity target) {
        		NavMeshAgent agent = GetComponent<NavMeshAgent>();
        		agent.destination = target.transform.position; 
        		agent.Resume();

        		State = EnemyState.InTransit;
                Target = target;
        	}

        	public override bool ReceiveDamage(float amount) {
        		CurrentHealth -= amount;
        		HandleDamage ();

        		if (CurrentHealth < 0.0f) {
        			State = EnemyState.Dead;
        			HandleDeath ();

                    return true;
        		}

                return false;
        	}


        	public void Update() {
                StartCoroutine(InternalUpdate());
        	}

            private IEnumerator InternalUpdate() {
                while (State != EnemyState.Dead)
                {
                    switch (State)
                    {
                        case EnemyState.InTransit:
                            CheckInRangeOfTarget();
                            break;
                        case EnemyState.AtTarget:
                            AttackTarget();
                            break;
                        default:
                            break;
                    }

                    // Take a break in between updates.
                    yield return new WaitForSeconds(UpdateInterval);
                }
            }

        	private void CheckInRangeOfTarget() {
        		if ((this.transform.position - Target.transform.position).magnitude < PrimaryAttackRange) {
        			NavMeshAgent agent = GetComponent<NavMeshAgent>();
        			agent.Stop();

        			State = EnemyState.AtTarget;
        		}
        	}

        	protected virtual void AttackTarget() {
                Target.ReceiveDamage(PrimaryAttackDamage);
        	}

        	protected abstract void HandleDamage ();

        	protected abstract void HandleDeath ();
        }

    }
}
