using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Reload
{
    class EnemyToolbox
    {
        public static void AddEnemyToDatabase(GameObject EnemyPrefab, string EnemyGUID)
        {
            EnemyDatabaseEntry item = new EnemyDatabaseEntry
            {
                myGuid = EnemyGUID,
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = true,
                path = EnemyGUID,
                isInBossTab = false,
                encounterGuid = EnemyGUID
            };
            EnemyDatabase.Instance.Entries.Add(item);
            EncounterDatabaseEntry encounterDatabaseEntry = new EncounterDatabaseEntry(EnemyPrefab.GetComponent<AIActor>().encounterTrackable)
            {
                path = EnemyGUID,
                myGuid = EnemyPrefab.GetComponent<AIActor>().encounterTrackable.EncounterGuid
            };
            EncounterDatabase.Instance.Entries.Add(encounterDatabaseEntry);
        }
    }
    class CompanionisedEnemyUtility
    {
        public static void InitHooks()
        {
            DisplaceHook = new Hook(
    typeof(DisplaceBehavior).GetMethod("SpawnImage", BindingFlags.Instance | BindingFlags.NonPublic),
    typeof(CompanionisedEnemyUtility).GetMethod("DisplacedImageSpawnHook", BindingFlags.Instance | BindingFlags.NonPublic),
    typeof(DisplaceBehavior));

        }
        public static Hook DisplaceHook;

        public static AIActor SpawnCompanionisedEnemy(PlayerController owner, string enemyGuid, IntVector2 position, bool doTint, Color enemyTint, int baseDMG, int jammedDMGMult, bool shouldBeJammed, bool DoCore)
        {
            var enemyToSpawn = EnemyDatabase.GetOrLoadByGuid(enemyGuid);
            AIActor TargetActor = AIActor.Spawn(enemyToSpawn, position, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(position), true, AIActor.AwakenAnimationType.Default, true);
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(TargetActor.specRigidbody, null, false);

            CompanionController orAddComponent = TargetActor.gameObject.GetOrAddComponent<CompanionController>();
            orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
            orAddComponent.Initialize(owner);

            if (shouldBeJammed == true)
            {
                TargetActor.BecomeBlackPhantom();
            }
            
            TargetActor.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
            
            TargetActor.gameObject.AddComponent<KillOnRoomClear>();
            if (DoCore)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(VFXToolbox.FriendlyVFX);
                tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                gameObject.transform.parent = TargetActor.transform;
                if (TargetActor.healthHaver.IsBoss)
                {
                    gameObject.transform.position = TargetActor.specRigidbody.HitboxPixelCollider.UnitTopCenter;
                }
                else
                {
                    Bounds bounds = TargetActor.sprite.GetBounds();
                    Vector3 vector = TargetActor.transform.position + new Vector3((bounds.max.x + bounds.min.x) / 2f, bounds.max.y, 0f).Quantize(0.0625f);
                    gameObject.transform.position = TargetActor.sprite.WorldCenter.ToVector3ZUp(0f).WithY(vector.y);
                }
                component.HeightOffGround = 0.5f;
                TargetActor.sprite.AttachRenderer(component);
            }
            ContinualKillOnRoomClear contKill = TargetActor.gameObject.AddComponent<ContinualKillOnRoomClear>();
            if (TargetActor.HasTag("multiple_phase_enemy"))
            {
                contKill.forceExplode = true;
                contKill.eraseInsteadOfKill = true;
            }
            CompanionisedEnemyBulletModifiers companionisedBullets = TargetActor.gameObject.GetOrAddComponent<CompanionisedEnemyBulletModifiers>();
            companionisedBullets.jammedDamageMultiplier = 2f;
            companionisedBullets.baseBulletDamage = 10f;
            companionisedBullets.scaleSpeed = true;
            companionisedBullets.scaleDamage = true;
            companionisedBullets.scaleSize = false;
            companionisedBullets.doPostProcess = false;
            TargetActor.IsHarmlessEnemy = true;
            if (doTint) TargetActor.RegisterOverrideColor(enemyTint, "CompanionisedEnemyTint");
            TargetActor.IgnoreForRoomClear = true;
            

            return TargetActor;
        }

        public static void MakeSeeThrough(Projectile obj)
        {
            Material material = new Material(ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit"));
            material.SetTexture("_MainTex", obj.sprite.renderer.material.mainTexture);
            material.SetTexture("_MaskTex", obj.sprite.renderer.material.mainTexture);
            material.SetFloat("_Fade", 0.4f);
            obj.sprite.renderer.material = material;
        }
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
            this.MakeSeeThrough = true;
        }
        public void Start()
        {
            enemy = base.aiActor;
            AIBulletBank bulletBank2 = enemy.bulletBank;
            foreach (AIBulletBank.Entry bullet in bulletBank2.Bullets)
            {
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
                    proj.baseData.damage = baseBulletDamage;
                    if(MakeSeeThrough)
                    {
                        CompanionisedEnemyUtility.MakeSeeThrough(proj);
                    }
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
        public bool MakeSeeThrough;
        public Color TintColor;

    }
    public class ContinualKillOnRoomClear : MonoBehaviour
    {
        public ContinualKillOnRoomClear()
        {
            this.lengthOfNonCombatSurvival = 1f;
        }
        private void Start()
        {
            if (base.GetComponent<AIActor>()) this.self = base.GetComponent<AIActor>();
        }
        private void Update()
        {
            if (!GameManager.Instance.PrimaryPlayer.IsInCombat)
            {
                if (!ithasBegun)
                {
                    ithasBegun = true;
                    GameManager.Instance.StartCoroutine(Suicide());
                }
            }
        }
        private IEnumerator Suicide()
        {
            yield return new WaitForSeconds(lengthOfNonCombatSurvival);
            if (self & self.healthHaver && self.healthHaver.IsAlive)
            {
                ithasBegun = true;
                if (forceExplode && self.specRigidbody) Exploder.DoDefaultExplosion(self.specRigidbody.UnitCenter, Vector2.zero);
                if (eraseInsteadOfKill)
                {
                    self.EraseFromExistenceWithRewards();
                }
                else
                {
                    if (self.healthHaver)
                    {
                        self.healthHaver.ApplyDamage(float.MaxValue, Vector2.zero, "Erasure");
                    }
                }
            }
            yield break;
        }
        private bool ithasBegun = false;
        private AIActor self;
        public float lengthOfNonCombatSurvival;
        public bool eraseInsteadOfKill;
        public bool forceExplode;
    }


}