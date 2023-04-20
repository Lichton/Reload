using System.Collections.Generic;
using Gungeon;
using UnityEngine;
using System.Collections;
using Dungeonator;
using SaveAPI;
using static DirectionalAnimation;
using ItemAPI;
using GungeonAPI;
using Alexandria;
using Alexandria.EnemyAPI;
using Reload;
using Brave.BulletScript;

namespace Reload
{
	class BulletBishop : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "BulletBishop";

		private static tk2dSpriteCollectionData BulletBishopCollection;


		public static List<int> spriteIds2 = new List<int>();

		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/bullet_pope_bosscard.png");
		public static GameObject shootpoint;
		public static GameObject shootpointd;
		public static void Init()
		{
			BulletBishop.BuildPrefab();
		}
		public static void BuildPrefab()
		{
			bool flag = prefab != null || Alexandria.EnemyAPI.EnemyBuilder.Dictionary.ContainsKey(guid);

			bool flag2 = flag;
			if (!flag2)
			{
				prefab = Alexandria.EnemyAPI.BossBuilder.BuildPrefab("BulletBishop", guid, "Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleFront/bullet_pope_idle_front_001.png", new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();

				AIAnimator aiAnimator = enemy.aiAnimator;

				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 1.75f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = true;
				enemy.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("c00390483f394a849c36143eb878998f").ShadowObject;
				enemy.aiActor.IgnoreForRoomClear = false;
				//enemy.aiActor.aiAnimator.HitReactChance = 0.1f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(650f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(650f, null, false);
				//-----------------------------------------------------------------------------------------------------

				//-----------------------------------------------------------------------------------------------------
				bool flag3 = BulletBishopCollection == null;
				if (flag3)
				{
					BulletBishopCollection = SpriteBuilder.ConstructCollection(prefab, "BulletPreacherCollection");
					UnityEngine.Object.DontDestroyOnLoad(BulletBishopCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], BulletBishopCollection);
					}
					//--------
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{

					0,
					1

					}, "idlefront", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{

					2,
					3

					}, "idleleft", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{

					4,
					5

					}, "idleright", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{

					6,
					7

					}, "idleback", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
					94,
					95,
					96,
					97,
					98,
					99,
					//loop
					100,
					101,
					102,
					103,
					//loop
					//loop
					100,
					101,
					102,
					103,
					//loop
					//loop
					100,
					101,
					102,
					103,
					//loop
					//loop
					100,
					101,
					102,
					103,
					//loop
					104,
					105,
					106,
					107,
					108


					}, "attackfront", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
					37,
					38,
					39,
					40,
					41,
					42,
					43,
					45


					}, "attackleft", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5.3f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
					46,
					47,
					48,
					49,
					50,
					51,
					52,
					53,
					54


					}, "attackright", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5.3f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
55,
					56,
					57,
					58,
					59,
					60,
					61,
					62,
					63


					}, "attackback", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5.3f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
64,
					65,
					66,
					67,
					68
					}, "teleport", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
8,
					9,
					10,
					11,
					12,
					13,
					14,
					15,
					16,
					17,
					18,
					19,
					20,
					21
					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
22,
					23,
					24,
					25,
					26,
					27
					}, "die", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
					69,
					70,
					71,
					72,
					73,
					74,
					}, "summon", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
					82,
					83,
					84,
					85,
					86,
					87,
					88,
					89,
					90,
					91,
					92,
					93
					}, "glow", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, BulletBishopCollection, new List<int>
					{
					79,
					80,
					81,
					}, "damaged", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;


					//--------

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
						ManualOffsetY = 10,
						ManualWidth = 34,
						ManualHeight = 40,
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
						ManualOffsetY = 10,
						ManualWidth = 34,
						ManualHeight = 40,
						ManualDiameter = 0,
						ManualLeftX = 0,
						ManualLeftY = 0,
						ManualRightX = 0,
						ManualRightY = 0
					});
					enemy.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
					shootpoint = new GameObject("attach");
					shootpoint.transform.parent = enemy.transform;
					shootpoint.transform.position = enemy.sprite.WorldCenter - new Vector2(0.25f, -0.25f);
					GameObject m_CachedGunAttachPoint = enemy.transform.Find("attach").gameObject;
					m_CachedGunAttachPoint = new GameObject("attach");
					m_CachedGunAttachPoint.transform.parent = enemy.transform;
					m_CachedGunAttachPoint.transform.position = new Vector2(enemy.sprite.WorldBottomRight.x - 0.80f, enemy.sprite.WorldBottomRight.y + 0.80f);
					GameObject m_CachedGunAttachPoint2 = enemy.transform.Find("attach").gameObject;

					enemy.aiActor.PreventBlackPhantom = false;
					var bs = prefab.GetComponent<BehaviorSpeculator>();
					DirectionalAnimation Hurray = new DirectionalAnimation
					{
						Type = DirectionalAnimation.DirectionType.Single,
						Prefix = "attackfront",
						AnimNames = new string[1],
						Flipped = new DirectionalAnimation.FlipType[1]
					};
					aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "attackfront",
						anim = Hurray
					}
				};
					BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;

					aiAnimator.IdleAnimation = new DirectionalAnimation
					{
						Type = DirectionalAnimation.DirectionType.Single,
						Prefix = "idlefront",
						AnimNames = new string[1],
						Flipped = new DirectionalAnimation.FlipType[1]
					};

					/*aiAnimator.HitAnimation = new DirectionalAnimation
					{
						Type = DirectionalAnimation.DirectionType.Single,
						Prefix = "damaged",
						AnimNames = new string[1],
						Flipped = new DirectionalAnimation.FlipType[1]
					};*/


					bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
					bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
					bs.TargetBehaviors = new List<TargetBehaviorBase>
					{
					new TargetPlayerBehavior
					{
						Radius = 45f,
						LineOfSight = true,
						ObjectPermanence = true,
						SearchInterval = 0.25f,
						PauseOnTargetSwitch = false,
						PauseTime = 0.25f
					}

					};

					bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
				{

					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
												Behavior = new ShootBehavior{
						ShootPoint = m_CachedGunAttachPoint,
						ReaimOnFire = true,
						BulletScript = new CustomBulletScriptSelector(typeof(ShotgunExecutionerChain1)),
						LeadAmount = 0f,
						AttackCooldown = 1.25f,
						FireAnimation = "attackfront",
						RequiresLineOfSight = false,
						StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true
						},
						NickName = "DirectionalCross"
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint =  m_CachedGunAttachPoint,
						BulletScript = new CustomBulletScriptSelector(typeof(SummonScript)),
						LeadAmount = 0f,
						AttackCooldown = 1.25f,
						PostFireAnimation = "summon",
						RequiresLineOfSight = false,
						StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true
						},
						NickName = "Summonus Fuckyouius"
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,

						Behavior = new ShootBehavior{
						ShootPoint =  shootpoint,
						ReaimOnFire = true,
						BulletScript = new CustomBulletScriptSelector(typeof(CrossBullet)),
						LeadAmount = 0f,
						AttackCooldown = 1.25f,
						PostFireAnimation = "glow",
						RequiresLineOfSight = false,
						StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true
						},
						NickName = "Bishop, stop whacking people with that bible!"
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 1f,
					Behavior = new TeleportBehavior()
				{

					AttackableDuringAnimation = true,
					
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					StayOnScreen = true,
					AvoidWalls = true,
					MinWallDistance = 3f,
					GoneTime = 1f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 4f,
					MaxDistanceFromPlayer = -1f,
					teleportOutAnim = "teleport",
			        
					teleportInBulletScript = new CustomBulletScriptSelector(typeof(DiagonalScript)),
					teleportOutBulletScript = new CustomBulletScriptSelector(typeof(DiagonalScript)),
					AttackCooldown = 1.25f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
				},
					NickName = "Incenseivized"
					},
				};
					bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
					bs.TickInterval = behaviorSpeculator.TickInterval;
					bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
					bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
					bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
					bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
					bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
					
					Game.Enemies.Add("rld:bullet_bishop", enemy.aiActor);
					var nur = enemy.aiActor;
					

					
					if (enemy.GetComponent<EncounterTrackable>() != null)
					{
						UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
					}
					GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
					prefab.AddComponent<BulletBishop>();

					miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
					miniBossIntroDoer.initialDelay = 0.15f;
					miniBossIntroDoer.cameraMoveSpeed = 14;
					miniBossIntroDoer.specifyIntroAiAnimator = null;
					miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Beholster";
					//miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
					miniBossIntroDoer.PreventBossMusic = false;
					miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
					miniBossIntroDoer.preIntroAnim = string.Empty;
					miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
					miniBossIntroDoer.introAnim = "intro";
					miniBossIntroDoer.introDirectionalAnim = string.Empty;
					miniBossIntroDoer.continueAnimDuringOutro = false;
					miniBossIntroDoer.cameraFocus = null;
					miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
					miniBossIntroDoer.restrictPlayerMotionToRoom = false;
					miniBossIntroDoer.fusebombLock = false;
					miniBossIntroDoer.AdditionalHeightOffset = 0;
					ReloadModule.Strings.Enemies.Set("#BULLETBISHOP_NAME", "BULLET BISHOP");
					ReloadModule.Strings.Enemies.Set("#BULLETBISHOP_NAME_SMALL", "Bullet Bishop");

					ReloadModule.Strings.Enemies.Set("EX_CATHEDRA", "EX CATHEDRA");
					ReloadModule.Strings.Enemies.Set("#QUOTE", "");
					enemy.aiActor.OverrideDisplayName = "#BULLETBISHOP_NAME_SMALL";

					miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
					{
						bossNameString = "#BULLETBISHOP_NAME",
						bossSubtitleString = "EX_CATHEDRA",
						bossQuoteString = "#QUOTE",
						bossSpritePxOffset = IntVector2.Zero,
						topLeftTextPxOffset = IntVector2.Zero,
						bottomRightTextPxOffset = IntVector2.Zero,
						bgColor = Color.blue
					};
					if (BossCardTexture)
					{
						miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
						miniBossIntroDoer.SkipBossCard = false;
						enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
					}
					else
					{
						miniBossIntroDoer.SkipBossCard = true;
						enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
					}

					SpriteBuilder.AddSpriteToCollection("Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleFront/bullet_pope_idle_front_001", SpriteBuilder.ammonomiconCollection);
					if (enemy.GetComponent<EncounterTrackable>() != null)
					{
						UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
					}
					enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
					enemy.encounterTrackable.journalData = new JournalEntry();
					enemy.encounterTrackable.EncounterGuid = "rld:bullet_bishop";
					enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
					enemy.encounterTrackable.journalData.SuppressKnownState = false;
					enemy.encounterTrackable.journalData.IsEnemy = true;
					enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
					enemy.encounterTrackable.ProxyEncounterGuid = "";
					enemy.encounterTrackable.journalData.AmmonomiconSprite = "Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleFront/bullet_pope_idle_front_001";
					enemy.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Reload\\Resources\\NPCs\\Hostile\\Bosses\\BulletBishop\\bullet_bishop_ammonom.png");
					ReloadModule.Strings.Enemies.Set("#BULLETBISHOPAMMONOMICON", "Bullet Bishop");
					ReloadModule.Strings.Enemies.Set("#BULLETBISHOPAMMONOMICONSHORT", "Ex Cathedra");
					ReloadModule.Strings.Enemies.Set("#BULLETBISHOPAMMONOMICONLONG", "This blessed bullet speaks with the voice of Kaliber herself. He preaches to the Gundead the words of the Cult of The Gundead, and the gifts given to him are evident.");
					enemy.encounterTrackable.journalData.PrimaryDisplayName = "#BULLETBISHOPAMMONOMICON";
					enemy.encounterTrackable.journalData.NotificationPanelDescription = "#BULLETBISHOPAMMONOMICONSHORT";
					enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#BULLETBISHOPAMMONOMICONLONG";
                    EnemyToolbox.AddEnemyToDatabase(enemy.gameObject, "rld:bullet_bishop");
					EnemyDatabase.GetEntry("rld:bullet_bishop").ForcedPositionInAmmonomicon = 1000;
					EnemyDatabase.GetEntry("rld:bullet_bishop").isInBossTab = false;
					EnemyDatabase.GetEntry("rld:bullet_bishop").isNormalEnemy = true;

					miniBossIntroDoer.SkipFinalizeAnimation = true;
					miniBossIntroDoer.RegenerateCache();
					//==================
					//Important for not breaking basegame stuff!
					StaticReferenceManager.AllHealthHavers.Remove(enemy.aiActor.healthHaver);
					//==================

				}
			}
		}

		private static string[] spritePaths = new string[]
		{
			
			//idles
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleFront/bullet_pope_idle_front_001", //1
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleFront/bullet_pope_idle_front_002", //2

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleLeft/bullet_pope_idle_left_001", //3
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleLeft/bullet_pope_idle_left_002", //4

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleRight/bullet_pope_idle_right_001", //5
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleRight/bullet_pope_idle_right_002", //6

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleBack/bullet_pope_idle_back_001", //7
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/IdleBack/bullet_pope_idle_back_002", //8
			//intro

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_001", //9
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_002", //10
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_003", //11
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_004", //12
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_005", //13
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_006", //14
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_007", //15
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_008", //16
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_009", //17
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_010", //18
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_011", //19
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_012", //20
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_013", //21
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Intro/bullet_pope_appear_014", //22
			//death
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Die/bullet_pope_die_001", //23
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Die/bullet_pope_die_002", //24
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Die/bullet_pope_die_003", //25
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Die/bullet_pope_die_004", //26
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Die/bullet_pope_die_005", //27
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Die/bullet_pope_die_006", //28
			//attack
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_001", //29
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_002", //30
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_003", //31
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_004", //32
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_005", //33
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_006", //34
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_007", //35
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_008", //36
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_front_009", //37

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_001", //38
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_002", //39
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_003", //40
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_004", //41
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_005", //42
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_006", //43
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_007", //44
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_008", //45
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_left_009", //46

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_001", //47
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_002", //48
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_003", //49
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_004", //50
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_005", //51
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_006", //52
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_007", //53
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_008", //54
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_right_009", //55

			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_001", //56
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_002", //57
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_003", //58
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_004", //59
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_005", //60
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_006", //61
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_007", //62
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_008", //63
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Attack/bullet_pope_attack_back_009", //64
			//teleport
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Teleport/bullet_pope_teleport_001", //65
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Teleport/bullet_pope_teleport_002", //66
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Teleport/bullet_pope_teleport_003", //67
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Teleport/bullet_pope_teleport_004", //68
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Teleport/bullet_pope_teleport_005", //69
			//summon
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Summon/bullet_pope_summon_001", //69
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Summon/bullet_pope_summon_002", //70
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Summon/bullet_pope_summon_003", //71
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Summon/bullet_pope_summon_004", //72
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Summon/bullet_pope_summon_005", //73
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Summon/bullet_pope_summon_006", //74
			//glow
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow/bullet_pope_glow_001", //75
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow/bullet_pope_glow_002", //76
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow/bullet_pope_glow_003", //77
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow/bullet_pope_glow_004", //78
			//damaged
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Damaged/bullet_pope_damage_001", //79
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Damaged/bullet_pope_damage_002", //80
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Damaged/bullet_pope_damage_003", //81
			//glow2
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_001", //82
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_002", //83
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_003", //84
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_004", //85
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_005", //86
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_006", //87
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_007", //88
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_008", //89
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_009", //90
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_010", //91
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_011", //92
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Glow2/bullet_pope_glow_012", //93
			//attackLoop
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_001", //94
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_002", //95
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_003", //96
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_004", //97
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_005", //98
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_006", //99
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_007", //100
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_008", //101
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_009", //102
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_010", //103
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_011", //104
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_012", //105
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_013", //106
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_014", //107
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Loop/bullet_pope_attack_front_015", //108

		};

		public class SummonScript : Script // This BulletScript is just a modified version of the script BulletManShroomed, which you can find with dnSpy.
		{
			public override IEnumerator Top()
			{
				IntVector2 meceea = new IntVector2(GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom().GetRandomVisibleClearSpot(2, 2).x, GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom().GetRandomVisibleClearSpot(2, 2).y);
				AkSoundEngine.PostEvent("Play_ENM_wizardred_shoot_01", this.BulletBank.aiActor.gameObject); 
				 var Enemy = EnemyDatabase.GetOrLoadByGuid("8bb5578fba374e8aae8e10b754e61d62");
				AIActor actor = AIActor.Spawn(Enemy.aiActor, meceea, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Spawn, true);
				actor.HandleReinforcementFallIntoRoom(0f);
				yield break;
			}

		}

		public class DiagonalScript2 : Script
		{
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
				AkSoundEngine.PostEvent("Play_ENM_wizardred_vanish_01", this.BulletBank.aiActor.gameObject);
				float aimDirection = base.GetAimDirection((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 11f);
				for (int i = 0; i < 16; i++)
				{
					base.Fire(new Direction(aimDirection, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(11f, SpeedType.Absolute), new BashelliskSnakeBullets1.SnakeBullet(i * 3));
					base.Fire(new Direction(aimDirection + 25, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(13f, SpeedType.Absolute), new BashelliskSnakeBullets1.SnakeBullet(i * 4));
					base.Fire(new Direction(aimDirection - 25,  Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(13f, SpeedType.Absolute), new BashelliskSnakeBullets1.SnakeBullet(i * 4));
					
				}
				return null;
			}

			// Token: 0x0400008D RID: 141
			private const int NumBullets = 8;

			// Token: 0x0400008E RID: 142
			private const int BulletSpeed = 11;

			// Token: 0x0400008F RID: 143
			private const float SnakeMagnitude = 0.6f;

			// Token: 0x04000090 RID: 144
			private const float SnakePeriod = 3f;

			// Token: 0x02000026 RID: 38
			public class SnakeBullet : Bullet
			{
				// Token: 0x0600008B RID: 139 RVA: 0x00003EE1 File Offset: 0x000020E1
				public SnakeBullet(int delay) : base("snakeBullet", false, false, false)
				{
					this.delay = delay;
				}

				// Token: 0x0600008C RID: 140 RVA: 0x00003EF8 File Offset: 0x000020F8
				public override IEnumerator Top()
				{
					this.ManualControl = true;
					yield return this.Wait(this.delay);
					Vector2 truePosition = this.Position;
					for (int i = 0; i < 360; i++)
					{
						float offsetMagnitude = Mathf.SmoothStep(-0.6f, 0.6f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						this.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				// Token: 0x04000091 RID: 145
				private int delay;
			}
		}

		public class ShotgunExecutionerChain1 : Script
		{
			// Token: 0x06000BC1 RID: 3009 RVA: 0x000397DC File Offset: 0x000379DC
			public override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("b1770e0f1c744d9d887cc16122882b4f").bulletBank.GetBullet("chain"));
				}
				base.EndOnBlank = true;
				yield return base.Wait(40);
				ShotgunExecutionerChain1.HandBullet handBullet = null;
				for (int i = 0; i < 5; i++)
				{
					handBullet = this.FireVolley(i, (float)(20 + i * 5));
					if (i < 4)
					{
						yield return base.Wait(30);
					}
				}
				while (!handBullet.IsEnded && !handBullet.HasStopped)
				{
					yield return base.Wait(1);
				}
				yield return base.Wait(50);
				yield break;
			}

			// Token: 0x06000BC2 RID: 3010 RVA: 0x000397F8 File Offset: 0x000379F8
			private ShotgunExecutionerChain1.HandBullet FireVolley(int volleyIndex, float speed)
			{
				ShotgunExecutionerChain1.HandBullet handBullet = new ShotgunExecutionerChain1.HandBullet(this);
				base.Fire(new Direction(base.AimDirection, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), handBullet);
				for (int i = 0; i < 20; i++)
				{
					base.Fire(new Direction(base.AimDirection, Brave.BulletScript.DirectionType.Absolute, -1f), new ShotgunExecutionerChain1.ArmBullet(this, handBullet, i));
				}
				return handBullet;
			}

			// Token: 0x04000C92 RID: 3218
			private const int NumArmBullets = 20;

			// Token: 0x04000C93 RID: 3219
			private const int NumVolley = 3;

			// Token: 0x04000C94 RID: 3220
			private const int FramesBetweenVolleys = 30;

			// Token: 0x020002F8 RID: 760
			private class ArmBullet : Bullet
			{
				// Token: 0x06000BC3 RID: 3011 RVA: 0x00039860 File Offset: 0x00037A60
				public ArmBullet(ShotgunExecutionerChain1 parentScript, ShotgunExecutionerChain1.HandBullet handBullet, int index) : base("chain", false, false, false)
				{
					this.m_parentScript = parentScript;
					this.m_handBullet = handBullet;
					this.m_index = index;
				}

				// Token: 0x06000BC4 RID: 3012 RVA: 0x00039888 File Offset: 0x00037A88
				public override IEnumerator Top()
				{
					base.ManualControl = true;
					while (!this.m_parentScript.IsEnded && !this.m_handBullet.IsEnded && !this.m_handBullet.HasStopped && base.BulletBank)
					{
						base.Position = Vector2.Lerp(this.m_parentScript.Position, this.m_handBullet.Position, (float)this.m_index / 20f);
						yield return base.Wait(1);
					}
					if (this.m_parentScript.IsEnded)
					{
						base.Vanish(false);
						yield break;
					}
					int delay = 10 - this.m_index - 5;
					if (delay > 0)
					{
						yield return base.Wait(delay);
					}
					float currentOffset = 0f;
					Vector2 truePosition = base.Position;
					int halfWiggleTime = 10;
					for (int i = 0; i < 30; i++)
					{
						if (i == 0 && delay < 0)
						{
							i = -delay;
						}
						float magnitude = 0.4f;
						magnitude = Mathf.Min(magnitude, Mathf.Lerp(0.2f, 0.4f, (float)this.m_index / 8f));
						magnitude = Mathf.Min(magnitude, Mathf.Lerp(0.2f, 0.4f, (float)(20 - this.m_index - 1) / 3f));
						magnitude = Mathf.Lerp(magnitude, 0f, (float)i / (float)halfWiggleTime - 2f);
						currentOffset = Mathf.SmoothStep(-magnitude, magnitude, Mathf.PingPong(0.5f + (float)i / (float)halfWiggleTime, 1f));
						base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, currentOffset);
						yield return base.Wait(1);
					}
					yield return base.Wait(this.m_index + 1 + 30);
					base.Vanish(false);
					yield break;
				}

				// Token: 0x04000C95 RID: 3221
				public const int BulletDelay = 60;

				// Token: 0x04000C96 RID: 3222
				private const float WiggleMagnitude = 0.4f;

				// Token: 0x04000C97 RID: 3223
				public const int WiggleTime = 30;

				// Token: 0x04000C98 RID: 3224
				private const int NumBulletsToPreShake = 5;

				// Token: 0x04000C99 RID: 3225
				private ShotgunExecutionerChain1 m_parentScript;

				// Token: 0x04000C9A RID: 3226
				private ShotgunExecutionerChain1 shotgunExecutionerChain1;

				// Token: 0x04000C9B RID: 3227
				private ShotgunExecutionerChain1.HandBullet m_handBullet;

				// Token: 0x04000C9C RID: 3228
				private int m_index;
			}

			// Token: 0x020002FA RID: 762
			private class HandBullet : Bullet
			{
				// Token: 0x06000BCB RID: 3019 RVA: 0x00039C4C File Offset: 0x00037E4C
				public HandBullet(ShotgunExecutionerChain1 parentScript) : base("chain", false, false, false)
				{
					this.m_parentScript = parentScript;
				}

				// Token: 0x170002C0 RID: 704
				// (get) Token: 0x06000BCC RID: 3020 RVA: 0x00039C64 File Offset: 0x00037E64
				// (set) Token: 0x06000BCD RID: 3021 RVA: 0x00039C6C File Offset: 0x00037E6C
				public bool HasStopped { get; set; }

				// Token: 0x06000BCE RID: 3022 RVA: 0x00039C78 File Offset: 0x00037E78
				public override IEnumerator Top()
				{
					this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
					this.Projectile.BulletScriptSettings.surviveTileCollisions = true;
					SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
					specRigidbody.OnCollision += this.OnCollision;
					while (!this.m_parentScript.IsEnded && !this.HasStopped)
					{
						yield return base.Wait(1);
					}
					if (this.m_parentScript.IsEnded)
					{
						base.Vanish(false);
						yield break;
					}
					yield return base.Wait(111);
					base.Vanish(false);
					yield break;
				}

					
				// Token: 0x06000BCF RID: 3023 RVA: 0x00039C94 File Offset: 0x00037E94
				private void OnCollision(CollisionData collision)
				{
					bool flag = collision.collisionType == CollisionData.CollisionType.TileMap;
					SpeculativeRigidbody otherRigidbody = collision.OtherRigidbody;
					if (otherRigidbody)
					{
						flag = (otherRigidbody.majorBreakable || otherRigidbody.PreventPiercing || (!otherRigidbody.gameActor && !otherRigidbody.minorBreakable));
					}
					if (flag)
					{
						base.Position = collision.MyRigidbody.UnitCenter + PhysicsEngine.PixelToUnit(collision.NewPixelsToMove);
						this.Speed = 0f;
						this.HasStopped = true;
						PhysicsEngine.PostSliceVelocity = new Vector2?(new Vector2(0f, 0f));
						SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
						specRigidbody.OnCollision -= OnCollision;
					}
					else
					{
						PhysicsEngine.PostSliceVelocity = new Vector2?(collision.MyRigidbody.Velocity);
					}
				}

				// Token: 0x06000BD0 RID: 3024 RVA: 0x00039DA0 File Offset: 0x00037FA0
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (this.Projectile)
					{
						SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
						specRigidbody.OnCollision -= OnCollision;
					}
					this.HasStopped = true;
				}

				// Token: 0x04000CA8 RID: 3240
				private ShotgunExecutionerChain1 m_parentScript;
			}
		}
		public class DiagonalScript : Script
		{
			public override IEnumerator Top()
			{
				Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, base.BulletBank.aiActor.sprite.WorldCenter + new Vector2(0f, 0.5f), Quaternion.identity);
				Instantiate<GameObject>(EasyVFXDatabase.ItemPoofVFX, base.BulletBank.aiActor.sprite.WorldCenter, Quaternion.identity);
				AkSoundEngine.PostEvent("Play_ENM_wizardred_vanish_01", this.BulletBank.aiActor.gameObject);
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
				float aimDirection = base.GetAimDirection((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 11f);
				for (int i = 0; i < 16; i++)
				{
					base.Fire(new Direction(aimDirection, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(11f, SpeedType.Absolute), new BashelliskSnakeBullets1.SnakeBullet(i * 3));
					
				}
				return null;
			}

			// Token: 0x0400008D RID: 141
			private const int NumBullets = 8;

			// Token: 0x0400008E RID: 142
			private const int BulletSpeed = 11;

			// Token: 0x0400008F RID: 143
			private const float SnakeMagnitude = 0.6f;

			// Token: 0x04000090 RID: 144
			private const float SnakePeriod = 3f;

			// Token: 0x02000026 RID: 38
			public class SnakeBullet : Bullet
			{
				// Token: 0x0600008B RID: 139 RVA: 0x00003EE1 File Offset: 0x000020E1
				public SnakeBullet(int delay) : base("snakeBullet", false, false, false)
				{
					this.delay = delay;
				}

				// Token: 0x0600008C RID: 140 RVA: 0x00003EF8 File Offset: 0x000020F8
				public override IEnumerator Top()
				{
					this.ManualControl = true;
					yield return this.Wait(this.delay);
					Vector2 truePosition = this.Position;
					for (int i = 0; i < 360; i++)
					{
						float offsetMagnitude = Mathf.SmoothStep(-0.6f, 0.6f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						this.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				// Token: 0x04000091 RID: 145
				private int delay;
			}
		}
		public class TeleportOutwards: Script // This BulletScript is just a modified version of the script BulletManShroomed, which you can find with dnSpy.
		{
			public override IEnumerator Top()
			{
				Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, base.BulletBank.aiActor.sprite.WorldCenter, Quaternion.identity);

				yield break;
			}

		}
		public class TeleportBackIn: Script // This BulletScript is just a modified version of the script BulletManShroomed, which you can find with dnSpy.
		{
			public override IEnumerator Top()
			{
				Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, base.BulletBank.aiActor.sprite.WorldCenter, Quaternion.identity);
				yield break;
			}

		}
		public class CrossBullet : Script
		{
			private void FireSpinningLine(Vector2 start, Vector2 end, int numBullets)
			{
				start *= 0.5f;
				end *= 0.5f;
				float direction = (this.BulletManager.PlayerPosition() - base.Position).ToAngle();
				for (int i = 0; i < numBullets; i++)
				{
					Vector2 b = Vector2.Lerp(start, end, (float)i / ((float)numBullets - 1f));
					base.Fire(new Direction(direction, Brave.BulletScript.DirectionType.Absolute, -1f), new BulletCardinalHat1.SpinningBullet(base.Position, base.Position + b));

				}
			}
			public override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("8bb5578fba374e8aae8e10b754e61d62").bulletBank.GetBullet("hat"));
				}

				this.FireSpinningLine(new Vector2(-2f, 0f), new Vector2(2f, 0f), 8);
				this.FireSpinningLine(new Vector2(0f, -2f), new Vector2(0f, 4f), 8);
				yield return this.Wait(60);
				yield break;
			}
		}
		public class EnemyBehavior : BraveBehaviour
		{
			private RoomHandler m_StartRoom;
			public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (!base.aiActor.HasBeenEngaged)
				{
					CheckPlayerRoom();
				}
			}
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && GameManager.Instance.PrimaryPlayer.IsInCombat == true)
				{

					GameManager.Instance.StartCoroutine(LateEngage());
				}
				else
				{
					base.aiActor.HasBeenEngaged = false;
				}
			}
			private IEnumerator LateEngage()
			{
				yield return new WaitForSeconds(0.5f);
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					base.aiActor.HasBeenEngaged = true;
				}
				yield break;
			}
			public static void DisableSuperTinting(AIActor actor)
			{
				Material mat = actor.sprite.renderer.material;
				mat.mainTexture = actor.sprite.renderer.material.mainTexture;
				mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				mat.DisableKeyword("BRIGHTNESS_CLAMP_OFF");
			}
			public void Start()
			{
				DisableSuperTinting(base.aiActor);
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.aiActor.HasBeenEngaged = false;
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);

				base.aiActor.healthHaver.OnDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_VO_kali_death_01", base.aiActor.gameObject);
					
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_BISHOP, true);//Done
					var locationToSpawn = aiActor.GetAbsoluteParentRoom().GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
					Chest spawnedChest = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(locationToSpawn);
					spawnedChest.RegisterChestOnMinimap(spawnedChest.GetAbsoluteParentRoom());
					spawnedChest.ForceUnlock();
				};
			}

			
		}
	}
}