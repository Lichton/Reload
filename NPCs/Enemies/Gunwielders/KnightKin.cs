using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using static DirectionalAnimation;
namespace Reload
{
	public class KnightKin : AIActor
	{
		public static GameObject prefab;
		//Always make sure to give your enemy a unique guid. This is essentially the id of your enemy and is integral for many parts of EnemyAPI
		public static readonly string guid = "knightkin";
		public static void Init()
		{
			//As always don't forget to initalize your enemy. 
			KnightKin.BuildPrefab();
		}
		public static void BuildPrefab()
		{

			if (prefab == null || !ItemAPI.EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				//Sets up the prefab of the enemy. The spritepath, "FrostAndGunfireItems/Resources/Milton/Idle/milton_idle_001", determines the setup sprite for your enemy. vvvv This bool right here determines whether or not an enemy has an AiShooter or not. AIShooters are necessary if you want your enemy to hold a gun for example. An example of this can be seen in here.
				prefab = ItemAPI.EnemyBuilder.BuildPrefab("knightKin", guid, "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/IdleLeft/bullet_armor_idle_left_001", new IntVector2(0, 0), new IntVector2(0, 0), true);
				//This line extends a BraveBehavior called EnemyBehavior, this is a generic behavior I use for setting up things that can't be setup in BuildPrefab.
				var enemy = prefab.AddComponent<EnemyBehavior>();
				//Here you can setup various things like movement speed, weight, and health. There's a lot you can do with the AiActor parameter so feel free to experiment.
				//For Humphrey let's make him a flying enemy
				enemy.aiActor.PreventFallingInPitsEver = true;
				enemy.aiActor.MovementSpeed = 2f;
				enemy.aiActor.knockbackDoer.weight = 100;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(30f);
				enemy.aiActor.healthHaver.SetHealthMaximum(45f, null, false);
				//This is where you setup your animations. Most animations need specific frame names to be recognized like idle or die. 
				//The AddAnimation lines gets sprites from the folder specified in second phrase of the this line. At the very least you need an animation that contains the word idle for the idle animations for example.
				//AnimationType determines what kind of animation your making. In Gungeon there are 7 different Animation Types: Move, Idle, Fidget, Flight, Hit, Talk, Other. For a majority of these animations, these play automatically, however specific animations need to be told when to play such as Attack.
				//DirectionType determines the amount of ways an animation can face. You'll have to change your animation names to correspond with the DirectionType. For example if you want an animation to face eight ways you'll have to name your animations something like ""attack_south_west", "attack_north_east",  "attack_east", "attack_south_east",  "attack_north",  "attack_south", "attack_west", "attack_north_west" and change DirectionType to  DirectionType.EightWayOrdinal.
				//I suggest looking at the sprites of base game enemies to determine the names for the different directions.
				prefab.AddAnimation("idle_right", "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/IdleRight", fps: 7, AnimationType.Idle, DirectionType.TwoWayHorizontal);
				prefab.AddAnimation("idle_left", "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/IdleLeft", fps: 7, AnimationType.Idle, DirectionType.TwoWayHorizontal);
				prefab.AddAnimation("run_left", "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/MoveLeft", fps: 8, AnimationType.Move, DirectionType.TwoWayHorizontal);
				prefab.AddAnimation("run_right", "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/MoveRight", fps: 8, AnimationType.Move, DirectionType.TwoWayHorizontal);
				prefab.AddAnimation("die_left", "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/DieLeft", fps: 8, AnimationType.Move, DirectionType.TwoWayHorizontal, tk2dSpriteAnimationClip.WrapMode.Once, assignAnimation: false);
				prefab.AddAnimation("die_right", "Reload/Resources/NPCs/Hostile/Enemies/KnightKin/DieRight", fps: 8, AnimationType.Move, DirectionType.TwoWayHorizontal, tk2dSpriteAnimationClip.WrapMode.Once, assignAnimation: false);
				//Here we create a new DirectionalAnimation for our enemy to pull from. 
				//Make sure the AnimNames correspong to the AddAnimation names.		
				DirectionalAnimation die = new DirectionalAnimation()
				{
					AnimNames = new string[] { "die_right", "die_left" },
					Flipped = new FlipType[] { FlipType.None, FlipType.None },
					Type = DirectionType.TwoWayHorizontal,
					Prefix = string.Empty
					
				};
				enemy.aiAnimator.AssignDirectionalAnimation("die", die, AnimationType.Other);
				//This is where we get into the meat and potatoes of our enemy. This is where all the behaviors of our enemy are made.

				//this line adds a BehaviorSpeculator to our enemy which is the base for adding behaviors on to. 
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				//We're also going to duplicate the bulletbank and aishooter from the Bullet Kin.
				//The "762" represents the the gun id we want our enemy to hold. For a list of all gun ids, look at the items file on this github.
				//For now let's give him the finished gun.
				GameObject position = enemy.transform.Find("GunAttachPoint").gameObject;
				AIActor SourceEnemy = EnemyDatabase.GetOrLoadByGuid("1a78cfb776f54641b832e92c44021cf2");
				ItemAPI.EnemyBuilder.DuplicateAIShooterAndAIBulletBank(prefab, SourceEnemy.aiShooter, SourceEnemy.GetComponent<AIBulletBank>(), 417, position.transform);
				enemy.aiActor.healthHaver.SetHealthMaximum(30f, null, false);
				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider

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
					ManualHeight = 25,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0
				});
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyHitBox,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 18,
					ManualHeight = 27,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,

				});
				//Here we will add some basic behaviors such as TargetPlayerBehavior and SeekTargetBehavior.
				//You can change many things in these behaviors so feel free to go nuts.
				bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
			};
				//Now this is one of the most important behaviors because it allows our enemy to shoot.
				//If we want our enemy to shoot out of a gun, use ShootGunBehavior			
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootGunBehavior() {
					GroupCooldownVariance = 0f,
					LineOfSight = false,
					//With weapon type you can determine if you want your enemy to fire the gun directly or to use a bullet script. Let's use a Bullet Script for now. For the rest of code, let's modify it to be close to the Finished Gun.
					WeaponType = WeaponType.BulletScript,
					OverrideBulletName = null,
					BulletScript = new Alexandria.EnemyAPI.CustomBulletScriptSelector(typeof(KnightScript)),
					FixTargetDuringAttack = true,
					StopDuringAttack = true,
					LeadAmount = 0,
					LeadChance = 1,
					RespectReload = true,
					MagazineCapacity = 1,
					ReloadSpeed = 5f,
					EmptiesClip = true,
					SuppressReloadAnim = false,
					TimeBetweenShots = -1,
					PreventTargetSwitching = true,
					OverrideAnimation = null,
					OverrideDirectionalAnimation = null,
					HideGun = false,
					UseLaserSight = false,
					UseGreenLaser = false,
					PreFireLaserTime = -1,
					AimAtFacingDirectionWhenSafe = false,
					Cooldown = 1f,
					CooldownVariance = 0,
					AttackCooldown = 0,
					GlobalCooldown = 0,
					InitialCooldown = 0,
					InitialCooldownVariance = 0,
					GroupName = null,
					GroupCooldown = 0,
					MinRange = 0,
					Range = 5,
					MinWallDistance = 0,
					MaxEnemiesInRoom = 0,
					MinHealthThreshold = 0,
					MaxHealthThreshold = 1,
					HealthThresholds = new float[0],
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					IsBlackPhantom = false,
					resetCooldownOnDamage = null,
					RequiresLineOfSight = true,
					MaxUsages = 0,
				}
			};
				bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new SeekTargetBehavior
				{
					StopWhenInRange = false,
					CustomRange = 5f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.2f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f
				}
			};
				//Adds the enemy to MTG spawn pool and spawn command
				Game.Enemies.Add("rld:knight_kin", enemy.aiActor);
			}
		}
		public class EnemyBehavior : BraveBehaviour
		{
			//This determines that the enemy is active when a player is in the room
			private RoomHandler m_StartRoom;
			private void Update()
			{
				if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }
			}
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom) { base.aiActor.HasBeenEngaged = true; }
			}
			private void Start()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				//This line determines what happens when an enemy dies. For now it's something simple like playing a death sound.
				//A full list of all the sounds can be found in the SFX.txt document that comes with this github.
				base.aiActor.healthHaver.OnPreDeath += (obj) => { AkSoundEngine.PostEvent("Play_VO_kali_death_01", base.aiActor.gameObject); };
			}
		}
		public class KnightScript : Script
		{
			//Here we are going to create a basic bullet script.
			public override IEnumerator Top()
			{
				//In this line we're going to take the fiery bullet from Muzzle Flare and it to our enemy.
				//In this case, the giery bullet is the default bullet of the Muzzle Flare.
				//Try looking up the bullet scripts of other enemies in DnSpy to try to find thier bullet names.
				//The Guid of all enemies can be found in the "enemies" file of the github.
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				}
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim - 20, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim + 20, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim - 40, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim + 40, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim - 60, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim + 60, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim - 80, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim + 80, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim - 100, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim + 100, -1f), new Speed(8f, SpeedType.Absolute), new Bullet()); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
				yield return this.Wait(6);//Here the script will wait 6 frames before firing another shot.
				yield break;
			}
		}
		public class BasicBullet : Bullet
		{
			// Token: 0x06000A91 RID: 2705 RVA: 0x00030B38 File Offset: 0x0002ED38
			public BasicBullet() : base("default", false, false, false)
			{
			}

		}
	}
}
