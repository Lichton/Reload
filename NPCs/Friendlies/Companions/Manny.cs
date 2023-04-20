using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Reload
{
    class Manny
    {
        public static void Init()
        {
            BuildPrefab();
        }
        public static GameObject prefab;
        public static readonly string guid = "manny_taxidermy03143151351677777";
        public static void BuildPrefab()
        {
            bool flag = Manny.prefab != null || CompanionBuilder.companionDictionary.ContainsKey(Manny.guid);
            if (!flag)
            {
                Manny.prefab = CompanionBuilder.BuildPrefab("Manny", Manny.guid, "Reload/Resources/NPCs/Allied/Manny/IdleLeft/manny_idle_left_001", new IntVector2(0, 0), new IntVector2(16, 19));
                var companionController = Manny.prefab.AddComponent<MannyBehavior>();
                companionController.aiActor.MovementSpeed = 5f;
                Manny.prefab.AddAnimation("idle_right", "Reload/Resources/NPCs/Allied/Manny/IdleRight/manny_idle_right", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                Manny.prefab.AddAnimation("idle_left", "Reload/Resources/NPCs/Allied/Manny/IdleLeft/manny_idle_left", 4, CompanionBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                Manny.prefab.AddAnimation("run_right", "Reload/Resources/NPCs/Allied/Manny/WalkRight/manny_walk_right", 10, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                Manny.prefab.AddAnimation("run_left", "Reload/Resources/NPCs/Allied/Manny/WalkLeft/manny_walk_left", 10, CompanionBuilder.AnimationType.Move, DirectionalAnimation.DirectionType.TwoWayHorizontal, DirectionalAnimation.FlipType.None);
                companionController.CanInterceptBullets = true;
                companionController.aiActor.healthHaver.PreventAllDamage = false;
                companionController.aiActor.CollisionDamage = 0f;
                companionController.aiActor.specRigidbody.CollideWithOthers = true;
                companionController.aiActor.specRigidbody.CollideWithTileMap = false;
                companionController.healthHaver.healthIsNumberOfHits = true;
                companionController.healthHaver.SetHealthMaximum(5f);
                companionController.healthHaver.ForceSetCurrentHealth(5f);
                BehaviorSpeculator component = Manny.prefab.GetComponent<BehaviorSpeculator>();
                companionController.aiAnimator.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.PlayerHitBox,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 0,
                    ManualOffsetY = 0,
                    ManualWidth = 16,
                    ManualHeight = 19,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0
                });
                companionController.aiAnimator.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 0,
                    ManualOffsetY = 0,
                    ManualWidth = 16,
                    ManualHeight = 19,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0
                });
               
                component.MovementBehaviors.Add(new CompanionFollowPlayerBehavior
                {
                    IdleAnimations = new string[]
                     {
                        "idle"
                     },
                    CatchUpRadius = 6,
                    CatchUpMaxSpeed = 10,
                    CatchUpAccelTime = 1,
                    CatchUpSpeed = 7,
                });
            }
        }
        public class MannyBehavior : CompanionController
        {
            private void Start()
            {
                this.Owner = this.m_owner;
                base.healthHaver.OnPreDeath += this.Poof;
            }

            private void Poof(Vector2 obj)
            {
                UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.ItemPoofVFX, base.specRigidbody.UnitCenter, Quaternion.identity);
                base.healthHaver.OnPreDeath -= this.Poof;
            }

            public PlayerController Owner;
        }
        
    }
}