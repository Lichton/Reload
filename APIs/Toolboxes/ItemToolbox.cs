using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Reload
{
    public static class ItemToolbox
    {
        public static void PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
        {
            item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
        }

        private static string[] m_confettiNames = new string[] {
                "Global VFX/Confetti_Blue_001",
                "Global VFX/Confetti_Yellow_001",
                "Global VFX/Confetti_Green_001"
            };
        public static void DoConfetti(Vector2 startPosition)
        {
            for (int i = 0; i < 16; i++)
            {
                GameObject ConfettiObject = (GameObject)ResourceCache.Acquire(BraveUtility.RandomElement(m_confettiNames));
                if (ConfettiObject)
                {
                    WaftingDebrisObject component = GameObject.Instantiate(ConfettiObject).GetComponent<WaftingDebrisObject>();
                    if (component)
                    {
                        component.sprite.PlaceAtPositionByAnchor(startPosition.ToVector3ZUp(0f) + new Vector3(0.5f, 0.5f, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
                        Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                        insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                        component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0f);
                    }
                }
            }
        }
        public static void SpawnZombieSpent(AIActor newinfected, bool IsOriginal)
		{
			if (newinfected != null && !newinfected.healthHaver.IsBoss && newinfected.IsNormalEnemy)
			{
				{

					IntVector2 spawnLoc = newinfected.CenterPosition.ToIntVector2();
					AIActor zombie = AIActor.Spawn(spentToBecome, spawnLoc, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(spawnLoc), true, AIActor.AwakenAnimationType.Default, true);
					if (zombie.GetComponent<SpawnEnemyOnDeath>()) UnityEngine.Object.Destroy(zombie.GetComponent<SpawnEnemyOnDeath>());
					if (newinfected.IsBlackPhantom)
					{
						zombie.BecomeBlackPhantom();
					}
                    CompanionController orAddComponent = zombie.gameObject.GetOrAddComponent<CompanionController>();
                    orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                    orAddComponent.Initialize(GameManager.Instance.PrimaryPlayer);
                    zombie.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                    
					zombie.OverrideHitEnemies = true;
					zombie.CollisionDamage = 2f;
					zombie.CollisionDamageTypes |= CoreDamageTypes.Electric;
					zombie.IsHarmlessEnemy = true;
					zombie.IgnoreForRoomClear = true;
					zombie.healthHaver.maximumHealth *= 5f;
                    zombie.RegisterOverrideColor(ExtendedColours.poisonGreen, "Zombie");
                    zombie.gameObject.AddComponent<InfectionComponent>();
					zombie.gameObject.AddComponent<KillOnRoomClear>();
					zombie.specRigidbody.OnPreRigidbodyCollision += CollisionExceptionHandler;
                    zombie.knockbackDoer.weight *= 10;
                    zombie.knockbackDoer.knockbackMultiplier *= 0.5f;
				}
			}
		}

        private static void CollisionExceptionHandler(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if((myRigidbody.gameObject.GetComponent<InfectionComponent>() != null) && (otherRigidbody.gameObject.GetComponent<InfectionComponent>() != null))
            {
				PhysicsEngine.SkipCollision = true;
			}
        }
        public static void ApplyFear(this AIActor enemy, PlayerController player, float fearLength, float fearStartDistance, float fearStopDistance)
        {
            EnemyFeared inflictedFear = enemy.gameObject.AddComponent<EnemyFeared>();
            inflictedFear.player = player;
            inflictedFear.fearLength = fearLength;
            inflictedFear.fearStartDistance = fearStartDistance;
            inflictedFear.fearStopDistance = fearStopDistance;
        }
        public static string guid = "249db525a9464e5282d02162c88e0357";
		public static AIActor spentToBecome = EnemyDatabase.GetOrLoadByGuid(guid);

	}

    public class EnemyFeared : BraveBehaviour
    {
        private void Start()
        {
            this.enemy = base.aiActor;
            if (this.player != null && this.enemy != null)
            {
                StartCoroutine("HandleFear");
            }
        }
        private IEnumerator HandleFear()
        {
            if (this.enemy.behaviorSpeculator != null)
            {
                //ETGModConsole.Log("Fear Triggered");
                FleePlayerData fleedata = new FleePlayerData
                {
                    StartDistance = this.fearStartDistance,
                    StopDistance = this.fearStopDistance,
                    Player = this.player,
                };
                this.enemy.behaviorSpeculator.FleePlayerData = fleedata;

                yield return new WaitForSeconds(fearLength);

                this.enemy.behaviorSpeculator.FleePlayerData = null;
            }
        }

        

        public PlayerController player;
        private AIActor enemy;
        public float fearLength;
        public float fearStartDistance;
        public float fearStopDistance;
    }
}
