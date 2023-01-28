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

namespace Reload
{
	class BulletBishop : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "BulletBishop";

		private static tk2dSpriteCollectionData BulletBishopCollection;


		public static List<int> spriteIds2 = new List<int>();

		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/bullet_pope_bosscard.png");

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
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(1000f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(1000f, null, false);
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

					}, "idlefront", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
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
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					35,
					36


					}, "attackfront", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
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


					}, "attackleft", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5.3f;
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


					}, "attackright", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5.3f;
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


					}, "attackback", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5.3f;
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

					enemy.aiActor.PreventBlackPhantom = false;

					var bs = prefab.GetComponent<BehaviorSpeculator>();
					BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
					bs.OverrideBehaviors = new List<OverrideBehaviorBase>
					{
						new BulletBishopBehavior
						{

						}
					};
					bs.OtherBehaviors = new List<BehaviorBase>
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
					bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
					bs.TickInterval = behaviorSpeculator.TickInterval;
					bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
					bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
					bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
					bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
					bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
					Game.Enemies.Add("rld:bullet_bishop", enemy.aiActor);
					var nur = enemy.aiActor;
					nur.EffectResistances = new ActorEffectResistance[]
					{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Poison
					},
					};

					//SpriteBuilder.AddSpriteToCollection("Reload/Resources/Enemies/Coallet/Idle/coallet_idle_006", SpriteBuilder.ammonomiconCollection);
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

					SpriteBuilder.AddSpriteToCollection("Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/ammonom_icon_pope", SpriteBuilder.ammonomiconCollection);
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
					ReloadModule.Strings.Enemies.Set("#BULLETBISHOPAMMONOMICONLONG", "This blessed bullet speaks with the voice of Kaliber herself. He preaches to the Gundead the word of Kaliber, and the gifts given to him are evident.");
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
			"Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/Teleport/bullet_pope_teleport_005" //69
		};


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

			public void Start()
			{
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.aiActor.HasBeenEngaged = false;
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);

				base.aiActor.healthHaver.OnDeath += (obj) =>
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_BISHOP, true);//Done
				};
			}
		}
	}
}