using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using tk2dRuntime.TileMap;
using UnityEngine;
using FullInspector;
using Gungeon;

//using DirectionType = DirectionalAnimation.DirectionType;
using System.Collections;

using Brave.BulletScript;
using GungeonAPI;

using System.Text;
using System.IO;
using System.Reflection;
using SaveAPI;

using MonoMod.RuntimeDetour;
using Alexandria.ItemAPI;

namespace Reload
{


    public static class OtherTools
    {
        
        public static GameObject MakeLine(string spritePath, Vector2 colliderDimensions, Vector2 colliderOffsets, List<string> beamAnimationPaths = null, int beamFPS = -1)
        {
            try
            {

                GameObject line = new GameObject("line");

                float convertedColliderX = colliderDimensions.x / 16f;
                float convertedColliderY = colliderDimensions.y / 16f;
                float convertedOffsetX = colliderOffsets.x / 16f;
                float convertedOffsetY = colliderOffsets.y / 16f;

                int spriteID = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
                tk2dTiledSprite tiledSprite = line.GetOrAddComponent<tk2dTiledSprite>();

                tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.colliderVertices = new Vector3[]{
                    new Vector3(convertedOffsetX, convertedOffsetY, 0f),
                    new Vector3(convertedColliderX, convertedColliderY, 0f)
                };

                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);
                /*
                tk2dSpriteAnimator animator = line.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteAnimation animation = line.GetOrAddComponent<tk2dSpriteAnimation>();
                animation.clips = new tk2dSpriteAnimationClip[0];
                animator.Library = animation;
                animator.sprite.SetSprite(spriteID); 
                if (beamAnimationPaths != null)
                {
                    tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip() { name = "line_idle_thing", frames = new tk2dSpriteAnimationFrame[0], fps = beamFPS };
                    List<string> spritePaths = beamAnimationPaths;
                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
                    foreach (string path in spritePaths)
                    {
                        tk2dSpriteCollectionData collection = ETGMod.Databases.Items.ProjectileCollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(path, collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);
                        frameDef.colliderVertices = def.colliderVertices;
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }
                    clip.frames = frames.ToArray();
                    animation.clips = animation.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
                }
                */
                return line;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
                return null;
            }
        }


        public static bool RandomizerChanceBased(int minToCheck, int max)
        {
            int integer = UnityEngine.Random.Range(minToCheck, max);
            bool check = integer == minToCheck;
            return check;
        }

       
        public static Texture2D Rotated(this Texture2D texture, bool clockwise = false)
        {
            Color32[] original = texture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];
            int w = texture.width;
            int h = texture.height;

            int iRotated, iOriginal;

            for (int j = 0; j < h; ++j)
            {
                for (int i = 0; i < w; ++i)
                {
                    iRotated = (i + 1) * h - j - 1;
                    iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                    rotated[iRotated] = original[iOriginal];
                }
            }

            Texture2D rotatedTexture = new Texture2D(h, w);
            rotatedTexture.SetPixels32(rotated);
            rotatedTexture.Apply();
            return rotatedTexture;
        }

        public static Texture2D Flipped(this Texture2D texture, bool horizontal = true)
        {
            int w = texture.width;
            int h = texture.height;

            Texture2D output = new Texture2D(w, h);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    output.SetPixel(i, j, texture.GetPixel(w - i - 1, j));
                }
            }
            output.Apply();
            return output;
        }
        public static Texture GetReadable(this Texture texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                    texture.width,
                    texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D output = new Texture2D(texture.width, texture.height);
            output.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            output.Apply();
            RenderTexture.active = previous;

            return output;
        }


        public static VFXPool CreateMuzzleflash(string name, List<string> spriteNames, int fps, List<IntVector2> spriteSizes, List<tk2dBaseSprite.Anchor> anchors, List<Vector2> manualOffsets, bool orphaned, bool attached, bool persistsOnDeath,
            bool usesZHeight, float zHeight, VFXAlignment alignment, bool destructible, List<float> emissivePowers, List<Color> emissiveColors)
        {
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            GameObject obj = new GameObject(name);
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSprite sprite = obj.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.fps = fps;
            clip.frames = new tk2dSpriteAnimationFrame[0];
            for (int i = 0; i < spriteNames.Count; i++)
            {
                string spriteName = spriteNames[i];
                IntVector2 spriteSize = spriteSizes[i];
                tk2dBaseSprite.Anchor anchor = anchors[i];
                Vector2 manualOffset = manualOffsets[i];
                float emissivePower = emissivePowers[i];
                Color emissiveColor = emissiveColors[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = OtherTools.VFXCollection.GetSpriteIdByName(spriteName);
                tk2dSpriteDefinition def = OtherTools.SetupDefinitionForShellSprite(spriteName, frame.spriteId, spriteSize.x, spriteSize.y);
                def.ConstructOffsetsFromAnchor(anchor, def.position3);
                def.MakeOffset(manualOffset);
                def.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                def.material.SetFloat("_EmissiveColorPower", emissivePower);
                def.material.SetColor("_EmissiveColor", emissiveColor);
                def.materialInst.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                def.materialInst.SetFloat("_EmissiveColorPower", emissivePower);
                def.materialInst.SetColor("_EmissiveColor", emissiveColor);
                frame.spriteCollection = OtherTools.VFXCollection;
                clip.frames = clip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            sprite.renderer.material.SetFloat("_EmissiveColorPower", emissivePowers[0]);
            sprite.renderer.material.SetColor("_EmissiveColor", emissiveColors[0]);
            clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            clip.name = "start";
            animator.spriteAnimator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
            animator.spriteAnimator.Library.enabled = true;
            SpriteAnimatorKiller kill = animator.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill.fadeTime = -1f;
            kill.animator = animator;
            kill.delayDestructionTime = -1f;
            vfObj.orphaned = orphaned;
            vfObj.attached = attached;
            vfObj.persistsOnDeath = persistsOnDeath;
            vfObj.usesZHeight = usesZHeight;
            vfObj.zHeight = zHeight;
            vfObj.alignment = alignment;
            vfObj.destructible = destructible;
            vfObj.effect = obj;
            complex.effects = new VFXObject[] { vfObj };
            pool.effects = new VFXComplex[] { complex };
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("start");
            return pool;
        }
        public static tk2dSpriteCollectionData VFXCollection
        {
            get
            {
                return (PickupObjectDatabase.GetById(95) as Gun).clipObject.GetComponent<tk2dBaseSprite>().Collection;
            }
        }
        public static tk2dSpriteDefinition SetupDefinitionForShellSprite(string name, int id, int pixelWidth, int pixelHeight, tk2dSpriteDefinition overrideToCopyFrom = null)
        {
            float thing = 14;
            float trueWidth = (float)pixelWidth / thing;
            float trueHeight = (float)pixelHeight / thing;
            tk2dSpriteDefinition def = overrideToCopyFrom ?? OtherTools.VFXCollection.inst.spriteDefinitions[(PickupObjectDatabase.GetById(202) as Gun).shellCasing.GetComponent<tk2dBaseSprite>().spriteId].CopyDefinitionFrom();
            def.boundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.boundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.untrimmedBoundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.untrimmedBoundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.position0 = new Vector3(0f, 0f, 0f);
            def.position1 = new Vector3(0f + trueWidth, 0f, 0f);
            def.position2 = new Vector3(0f, 0f + trueHeight, 0f);
            def.position3 = new Vector3(0f + trueWidth, 0f + trueHeight, 0f);
            def.name = name;
            OtherTools.VFXCollection.spriteDefinitions[id] = def;
            return def;
        }
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        public static FieldInfo ProjectileHealthHaverHitCountInfo = typeof(Projectile).GetField("m_healthHaverHitCount", AnyBindingFlags);
        public static MethodInfo ProjectileHandleDelayedDamageInfo = typeof(Projectile).GetMethod("HandleDelayedDamage", AnyBindingFlags);

        public static BindingFlags AnyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        //==========================================================================================================================================================
        public static bool verbose = true;
        private static string defaultLog = Path.Combine(ETGMod.ResourcesDirectory, "PSOG.txt");
        public static string modID = "PSOG";


        public static List<string> PastNames = new List<string>
        {
            "fs_convict",
            "fs_pilot",
            "fs_bullet",
            "fs_soldier",
            "fs_guide",
            "fs_robot"
        };

        public static List<string> BossBlackList = new List<string>
        {
            "39de9bd6a863451a97906d949c103538",
            "fa6a7ac20a0e4083a4c2ee0d86f8bbf7",
            "47bdfec22e8e4568a619130a267eab5b",
            "ea40fcc863d34b0088f490f4e57f8913",
            "c00390483f394a849c36143eb878998f",
            "ec6b674e0acd4553b47ee94493d66422",
            "ffca09398635467da3b1f4a54bcfda80",
            "1b5810fafbec445d89921a4efb4e42b7",
            "4b992de5b4274168a8878ef9bf7ea36b",
            "c367f00240a64d5d9f3c26484dc35833",
            "da797878d215453abba824ff902e21b4",
            "5729c8b5ffa7415bb3d01205663a33ef",
            "fa76c8cfdf1c4a88b55173666b4bc7fb",
            "8b0dd96e2fe74ec7bebc1bc689c0008a",
            "5e0af7f7d9de4755a68d2fd3bbc15df4",
            "9189f46c47564ed588b9108965f975c9",
            "6868795625bd46f3ae3e4377adce288b",
            "4d164ba3f62648809a4a82c90fc22cae",
            "6c43fddfd401456c916089fdd1c99b1c",
            "3f11bbbc439c4086a180eb0fb9990cb4",
            "f3b04a067a65492f8b279130323b41f0",
            "41ee1c8538e8474a82a74c4aff99c712",
            "465da2bb086a4a88a803f79fe3a27677",
            "05b8afe0b6cc4fffa9dc6036fa24c8ec",
            "cd88c3ce60c442e9aa5b3904d31652bc",
            "68a238ed6a82467ea85474c595c49c6e",
            "7c5d5f09911e49b78ae644d2b50ff3bf",
            "76bc43539fc24648bff4568c75c686d1",
            "0ff278534abb4fbaaa65d3f638003648",
            "6ad1cafc268f4214a101dca7af61bc91",
            "14ea47ff46b54bb4a98f91ffcffb656d",
            "shellrax",
            "Bullet_Banker",
            "Fungannon",
            "Ophanaim",
            "annihichamber"

        };
        public static void Init()
        {
            if (File.Exists(defaultLog)) File.Delete(defaultLog);
        }
        public static bool OwnerHasSynergy(this Gun gun, string synergyName)
        {
            return gun.CurrentOwner is PlayerController && (gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy(synergyName);
        }

        public static void PrintNoID<T>(T obj, string color = "FFFFFF", bool force = false)
        {
            if (verbose || force)
            {
                string[] lines = obj.ToString().Split('\n');
                foreach (var line in lines)
                    LogToConsole($"<color=#{color}> {line}</color>");
            }

            Log(obj.ToString());
        }
        public static void LogToConsole(string message)
        {
            message.Replace("\t", "    ");
            ETGModConsole.Log(message);
        }
        public static void Log<T>(T obj)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(ETGMod.ResourcesDirectory, defaultLog), true))
            {
                writer.WriteLine(obj.ToString());
            }
        }

        public static void Notify(string header, string text, string spriteID, UINotificationController.NotificationColor color = UINotificationController.NotificationColor.SILVER, bool forceSingleLine = false)
        {
            tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
            int spriteIdByName = encounterIconCollection.GetSpriteIdByName(spriteID);
            GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, encounterIconCollection, spriteIdByName, color, false, forceSingleLine);
        }
        public static void ApplyStat(PlayerController player, PlayerStats.StatType statType, float amountToApply, StatModifier.ModifyMethod modifyMethod)
        {
            player.stats.RecalculateStats(player, false, false);
            StatModifier statModifier = new StatModifier()
            {
                statToBoost = statType,
                amount = amountToApply,
                modifyType = modifyMethod
            };
            player.ownerlessStatModifiers.Add(statModifier);
            player.stats.RecalculateStats(player, false, false);
        }
        public static bool Randomizer(float value)
        {
            return UnityEngine.Random.value > value;
        }


        public static void AnimateProjectile(this Projectile proj, List<string> names, int fps, bool loops, List<IntVector2> pixelSizes, List<bool> lighteneds, List<tk2dBaseSprite.Anchor> anchors, List<bool> anchorsChangeColliders,
            List<bool> fixesScales, List<Vector3?> manualOffsets, List<IntVector2?> overrideColliderPixelSizes, List<IntVector2?> overrideColliderOffsets, List<Projectile> overrideProjectilesToCopyFrom, string clipName = "idle")
        {
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = clipName;
            clip.fps = fps;
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                IntVector2 pixelSize = pixelSizes[i];
                IntVector2? overrideColliderPixelSize = overrideColliderPixelSizes[i];
                IntVector2? overrideColliderOffset = overrideColliderOffsets[i];
                Vector3? manualOffset = manualOffsets[i];
                bool anchorChangesCollider = anchorsChangeColliders[i];
                bool fixesScale = fixesScales[i];
                if (!manualOffset.HasValue)
                {
                    manualOffset = new Vector2?(Vector2.zero);
                }
                tk2dBaseSprite.Anchor anchor = anchors[i];
                bool lightened = lighteneds[i];
                Projectile overrideProjectileToCopyFrom = overrideProjectilesToCopyFrom[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
                frame.spriteCollection = ETGMod.Databases.Items.ProjectileCollection;
                frames.Add(frame);
                int? overrideColliderPixelWidth = null;
                int? overrideColliderPixelHeight = null;
                if (overrideColliderPixelSize.HasValue)
                {
                    overrideColliderPixelWidth = overrideColliderPixelSize.Value.x;
                    overrideColliderPixelHeight = overrideColliderPixelSize.Value.y;
                }
                int? overrideColliderOffsetX = null;
                int? overrideColliderOffsetY = null;
                if (overrideColliderOffset.HasValue)
                {
                    overrideColliderOffsetX = overrideColliderOffset.Value.x;
                    overrideColliderOffsetY = overrideColliderOffset.Value.y;
                }
                tk2dSpriteDefinition def = GunTools.SetupDefinitionForProjectileSprite(name, frame.spriteId, pixelSize.x, pixelSize.y, lightened, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX, overrideColliderOffsetY,
                    overrideProjectileToCopyFrom);
                def.ConstructOffsetsFromAnchor(anchor, def.position3, fixesScale, anchorChangesCollider);
                def.position0 += manualOffset.Value;
                def.position1 += manualOffset.Value;
                def.position2 += manualOffset.Value;
                def.position3 += manualOffset.Value;
                if (i == 0)
                {
                    proj.GetAnySprite().SetSprite(frame.spriteCollection, frame.spriteId);
                }
            }
            clip.wrapMode = loops ? tk2dSpriteAnimationClip.WrapMode.Loop : tk2dSpriteAnimationClip.WrapMode.Once;
            clip.frames = frames.ToArray();
            if (proj.sprite.spriteAnimator == null)
            {
                proj.sprite.spriteAnimator = proj.sprite.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            proj.sprite.spriteAnimator.playAutomatically = true;
            bool flag = proj.sprite.spriteAnimator.Library == null;
            if (flag)
            {
                proj.sprite.spriteAnimator.Library = proj.sprite.spriteAnimator.gameObject.AddComponent<tk2dSpriteAnimation>();
                proj.sprite.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[0];
                proj.sprite.spriteAnimator.Library.enabled = true;
            }
            proj.sprite.spriteAnimator.Library.clips = proj.sprite.spriteAnimator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
            proj.sprite.spriteAnimator.DefaultClipId = proj.sprite.spriteAnimator.Library.GetClipIdByName(clipName);
            proj.sprite.spriteAnimator.deferNextStartClip = false;
        }
        public static AIActor SetupAIActorDummy(string name, IntVector2 colliderOffset, IntVector2 colliderDimensions)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            SpeculativeRigidbody speculativeRigidbody = gameObject.AddComponent<tk2dSprite>().SetUpSpeculativeRigidbody(colliderOffset, colliderDimensions);
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            pixelCollider.ManualWidth = colliderDimensions.x;
            pixelCollider.ManualHeight = colliderDimensions.y;
            pixelCollider.ManualOffsetX = colliderOffset.x;
            pixelCollider.ManualOffsetY = colliderOffset.y;
            speculativeRigidbody.PixelColliders.Add(pixelCollider);
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            AIActor result = gameObject.AddComponent<AIActor>();
            HealthHaver healthHaver = gameObject.AddComponent<HealthHaver>();
            healthHaver.SetHealthMaximum(10f, null, false);
            healthHaver.ForceSetCurrentHealth(10f);
            BehaviorSpeculator behaviorSpeculator = gameObject.AddComponent<BehaviorSpeculator>();
            ((ISerializedObject)behaviorSpeculator).SerializedObjectReferences = new List<UnityEngine.Object>(0);
            ((ISerializedObject)behaviorSpeculator).SerializedStateKeys = new List<string>
            {
                "OverrideBehaviors",
                "OtherBehaviors",
                "TargetBehaviors",
                "AttackBehaviors",
                "MovementBehaviors"
            };
            ((ISerializedObject)behaviorSpeculator).SerializedStateValues = new List<string>
            {
                "",
                "",
                "",
                "",
                ""
            };
            return result;
        }

        public static void DisableSuperTinting(AIActor actor)
        {
            Material mat = actor.sprite.renderer.material;
            mat.mainTexture = actor.sprite.renderer.material.mainTexture;
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.DisableKeyword("BRIGHTNESS_CLAMP_OFF");
        }
        public static void AddPassiveStatModifier(this Gun gun, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod modifyMethod)
        {
            gun.passiveStatModifiers = gun.passiveStatModifiers.Concat(new StatModifier[]
            {
                new StatModifier
                {
                    statToBoost = statType,
                    amount = amount,
                    modifyType = modifyMethod
                }
            }).ToArray<StatModifier>();
        }
        public class CompanionisedEnemyBulletModifiers : BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public CompanionisedEnemyBulletModifiers()
            {
                this.scaleDamage = false;
                this.scaleSize = false;
                this.scaleSpeed = false;
                this.doPostProcess = false;
                this.baseBulletDamage = 10f;
                this.TintBullets = false;
                this.TintColor = Color.grey;
                this.jammedDamageMultiplier = 2f;
            }
            public void Start()
            {
                enemy = base.aiActor;
                AIBulletBank bulletBank2 = enemy.bulletBank;
                foreach (AIBulletBank.Entry bullet in bulletBank2.Bullets)
                {
                    SpawnManager.PoolManager.Remove(bullet.BulletObject.transform);
                    bullet.BulletObject.GetComponent<Projectile>().BulletScriptSettings.preventPooling = true;
                }
                if (enemy.aiShooter != null)
                {
                    AIShooter aiShooter = enemy.aiShooter;
                    aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
                }

                if (enemy.bulletBank != null)
                {
                    AIBulletBank bulletBank = enemy.bulletBank;
                    bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
                }
            }

            private void PostProcessSpawnedEnemyProjectiles(Projectile proj)
            {
                if (TintBullets) { proj.AdjustPlayerProjectileTint(this.TintColor, 1); }
                if (enemy != null)
                {
                    if (enemy.aiActor != null)
                    {
                        SpawnManager.PoolManager.Remove(proj.transform);
                        proj.baseData.damage = baseBulletDamage;
                        if (enemyOwner != null)
                        {
                            //ETGModConsole.Log("Companionise: enemyOwner is not null");
                            if (scaleDamage) proj.baseData.damage *= enemyOwner.stats.GetStatValue(PlayerStats.StatType.Damage);
                            if (scaleSize)
                            {
                                proj.RuntimeUpdateScale(enemyOwner.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
                            }
                            if (scaleSpeed)
                            {
                                proj.baseData.speed *= enemyOwner.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                                proj.UpdateSpeed();
                            }
                            //ETGModConsole.Log("Damage: " + proj.baseData.damage);
                            if (doPostProcess) enemyOwner.DoPostProcessProjectile(proj);
                        }
                        if (enemy.aiActor.IsBlackPhantom) { proj.baseData.damage = baseBulletDamage * jammedDamageMultiplier; }
                    }
                }
                else { ETGModConsole.Log("Shooter is NULL"); }
            }

            private AIActor enemy;
            public PlayerController enemyOwner;
            public bool scaleDamage;
            public bool scaleSize;
            public bool scaleSpeed;
            public bool doPostProcess;
            public float baseBulletDamage;
            public float jammedDamageMultiplier;
            public bool TintBullets;
            public Color TintColor;

        }
        public static AIBulletBank CopyAIBulletBank(AIBulletBank bank)
        {
            UnityEngine.GameObject obj = new UnityEngine.GameObject();
            AIBulletBank newBank = obj.GetOrAddComponent<AIBulletBank>();
            newBank.Bullets = bank.Bullets;
            newBank.FixedPlayerPosition = bank.FixedPlayerPosition;
            newBank.OnProjectileCreated = bank.OnProjectileCreated;
            newBank.OverrideGun = bank.OverrideGun;
            newBank.rampTime = bank.rampTime;
            newBank.OnProjectileCreatedWithSource = bank.OnProjectileCreatedWithSource;
            newBank.rampBullets = bank.rampBullets;
            newBank.transforms = bank.transforms;
            newBank.useDefaultBulletIfMissing = bank.useDefaultBulletIfMissing;
            newBank.rampStartHeight = bank.rampStartHeight;
            newBank.SpecificRigidbodyException = bank.SpecificRigidbodyException;
            newBank.PlayShells = bank.PlayShells;
            newBank.PlayAudio = bank.PlayAudio;
            newBank.PlayVfx = bank.PlayVfx;
            newBank.CollidesWithEnemies = bank.CollidesWithEnemies;
            newBank.FixedPlayerRigidbodyLastPosition = bank.FixedPlayerRigidbodyLastPosition;
            newBank.ActorName = bank.ActorName;
            newBank.TimeScale = bank.TimeScale;
            newBank.SuppressPlayerVelocityAveraging = bank.SuppressPlayerVelocityAveraging;
            newBank.FixedPlayerRigidbody = bank.FixedPlayerRigidbody;
            return newBank;
        }
        public class EasyTrailComponent : BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public EasyTrailComponent()
            {
                //=====
                this.TrailPos = new Vector3(0, 0, 0);
                //======
                this.BaseColor = Color.red;
                this.StartColor = Color.red;
                this.EndColor = Color.white;
                //======
                this.LifeTime = 1f;
                //======
                this.StartWidth = 1;
                this.EndWidth = 0;

            }
            /// <summary>
            /// Lets you add a trail to your projectile.    
            /// </summary>
            /// <param name="TrailPos">Where the trail attaches its center-point to. You can input a custom Vector3 but its best to use the base preset. (Namely"projectile.transform.position;").</param>
            /// <param name="BaseColor">The Base Color of your trail.</param>
            /// <param name="StartColor">The Starting color of your trail.</param>
            /// <param name="EndColor">The End color of your trail. Having it different to the StartColor will make it transition from the Starting/Base Color to its End Color during its lifetime.</param>
            /// <param name="LifeTime">How long your trail lives for.</param>
            /// <param name="StartWidth">The Starting Width of your Trail.</param>
            /// <param name="EndWidth">The Ending Width of your Trail. Not sure why youd want it to be something other than 0, but the options there.</param>
            public void Start()
            {
                obj = base.gameObject;
                {
                    TrailRenderer tr;
                    var tro = obj.AddChild("trail object");
                    tro.transform.position = obj.transform.position;
                    tro.transform.localPosition = TrailPos;

                    tr = tro.AddComponent<TrailRenderer>();
                    tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    tr.receiveShadows = false;
                    var mat = material ?? new Material(Shader.Find("Sprites/Default"));
                    tr.material = mat;
                    tr.minVertexDistance = 0.1f;
                    //======
                    mat.SetColor("_Color", BaseColor);
                    tr.startColor = StartColor;
                    tr.endColor = EndColor;
                    //======
                    tr.time = LifeTime;
                    //======
                    tr.startWidth = StartWidth;
                    tr.endWidth = EndWidth;
                    tr.autodestruct = false;
                    trail = tr;
                }
            }

            public TrailRenderer ReturnTrailRenderer()
            {
                return trail;
            }

            private GameObject obj;

            public Vector2 TrailPos;
            public Color BaseColor;
            public Color StartColor;
            public Color EndColor;
            public float LifeTime;
            public float StartWidth;
            public float EndWidth;
            public Material material;
            public TrailRenderer trail;


        }


        public class EasyTrailOnEnemy : BraveBehaviour //----------------------------------------------------------------------------------------------
        {
            public EasyTrailOnEnemy()
            {
                //=====
                this.TrailPos = new Vector3(0, 0, 0);
                //======
                this.BaseColor = Color.red;
                this.StartColor = Color.red;
                this.EndColor = Color.white;
                //======
                this.LifeTime = 1f;
                //======
                this.StartWidth = 1;
                this.EndWidth = 0;
                this.castingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            }


            public void SetMode(UnityEngine.Rendering.ShadowCastingMode mode)
            {
                this.castingMode = mode;
            }
            /// <summary>
            /// Lets you add a trail to your projectile.    
            /// </summary>
            /// <param name="TrailPos">Where the trail attaches its center-point to. You can input a custom Vector3 but its best to use the base preset. (Namely"projectile.transform.position;").</param>
            /// <param name="BaseColor">The Base Color of your trail.</param>
            /// <param name="StartColor">The Starting color of your trail.</param>
            /// <param name="EndColor">The End color of your trail. Having it different to the StartColor will make it transition from the Starting/Base Color to its End Color during its lifetime.</param>
            /// <param name="LifeTime">How long your trail lives for.</param>
            /// <param name="StartWidth">The Starting Width of your Trail.</param>
            /// <param name="EndWidth">The Ending Width of your Trail. Not sure why youd want it to be something other than 0, but the options there.</param>
            public void Start()
            {
                AI = base.aiActor;
                {
                    TrailRenderer tr;
                    var tro = base.aiActor.gameObject.AddChild("trail object");
                    tro.transform.position = base.aiActor.transform.position;
                    tro.transform.localPosition = TrailPos;

                    tr = tro.AddComponent<TrailRenderer>();
                    tr.shadowCastingMode = castingMode;
                    tr.receiveShadows = false;
                    var mat = new Material(Shader.Find("Sprites/Default"));
                    mat.mainTexture = _gradTexture;
                    tr.material = mat;
                    tr.minVertexDistance = 0.1f;
                    //======
                    mat.SetColor("_Color", BaseColor);
                    tr.startColor = StartColor;
                    tr.endColor = EndColor;
                    //======
                    tr.time = LifeTime;
                    //======
                    tr.startWidth = StartWidth;
                    tr.endWidth = EndWidth;
                    tr.enabled = true;
                }

            }

            public Texture _gradTexture;
            private AIActor AI;

            public Vector2 TrailPos;
            public Color BaseColor;
            public Color StartColor;
            public Color EndColor;
            public float LifeTime;
            public float StartWidth;
            public float EndWidth;
            public UnityEngine.Rendering.ShadowCastingMode castingMode;

        }

    }
}