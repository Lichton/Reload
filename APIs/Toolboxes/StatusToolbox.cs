using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Reload
{
    public class StaticStatusEffects
    {
        //---------------------------------------BASEGAME STATUS EFFECTS
        //Fires
        public static GameActorFireEffect hotLeadEffect = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect;
        public static GameActorFireEffect greenFireEffect = PickupObjectDatabase.GetById(706).GetComponent<Gun>().DefaultModule.projectiles[0].fireEffect;
        public static GameActorFireEffect SunlightBurn = PickupObjectDatabase.GetById(748).GetComponent<Gun>().DefaultModule.chargeProjectiles[0].Projectile.fireEffect;


        //Freezes
        public static GameActorFreezeEffect frostBulletsEffect = PickupObjectDatabase.GetById(278).GetComponent<BulletStatusEffectItem>().FreezeModifierEffect;
        public static GameActorFreezeEffect chaosBulletsFreeze = PickupObjectDatabase.GetById(569).GetComponent<ChaosBulletsItem>().FreezeModifierEffect;

        //Poisons
        public static GameActorHealthEffect irradiatedLeadEffect = PickupObjectDatabase.GetById(204).GetComponent<BulletStatusEffectItem>().HealthModifierEffect;

        //Charms
        public static GameActorCharmEffect charmingRoundsEffect = PickupObjectDatabase.GetById(527).GetComponent<BulletStatusEffectItem>().CharmModifierEffect;

        //Cheeses
        public static GameActorCheeseEffect elimentalerCheeseEffect = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect;

        //Speed Changes
        public static GameActorSpeedEffect tripleCrossbowSlowEffect = (PickupObjectDatabase.GetById(381) as Gun).DefaultModule.projectiles[0].speedEffect;

        public static GameActorPartyEffect StandardPartyEffect;




        public static void InitCustomEffects()
        {

        }


    }

    public class ApplyDirectStatusEffects //----------------------------------------------------------------------------------------------------------------------------
    {
        public static void ApplyDirectFreeze(GameActor target, float duration, float freezeAmount, float damageToDealOnUnfreeze, Color tintColour, Color deathTintColour, EffectResistanceType resistanceType, string identifier, bool tintsEnemy, bool tintsCorpse)
        {
            try
            {
                //ETGModConsole.Log("Attempted to apply direct freeze");
                GameActorFreezeEffect freezeModifierEffect = PickupObjectDatabase.GetById(278).GetComponent<BulletStatusEffectItem>().FreezeModifierEffect;
                GameActorFreezeEffect freezeToApply = new GameActorFreezeEffect
                {
                    duration = duration,
                    TintColor = tintColour,
                    DeathTintColor = deathTintColour,
                    effectIdentifier = identifier,
                    AppliesTint = tintsEnemy,
                    AppliesDeathTint = tintsCorpse,
                    resistanceType = resistanceType,
                    FreezeAmount = freezeAmount,
                    UnfreezeDamagePercent = damageToDealOnUnfreeze,
                    crystalNum = freezeModifierEffect.crystalNum,
                    crystalRot = freezeModifierEffect.crystalRot,
                    crystalVariation = freezeModifierEffect.crystalVariation,
                    FreezeCrystals = freezeModifierEffect.FreezeCrystals,
                    debrisAngleVariance = freezeModifierEffect.debrisAngleVariance,
                    debrisMaxForce = freezeModifierEffect.debrisMaxForce,
                    debrisMinForce = freezeModifierEffect.debrisMinForce,
                    OverheadVFX = freezeModifierEffect.OverheadVFX,
                    vfxExplosion = freezeModifierEffect.vfxExplosion,
                    stackMode = freezeModifierEffect.stackMode,
                    maxStackedDuration = freezeModifierEffect.maxStackedDuration,
                    AffectsEnemies = true,
                    AffectsPlayers = false,
                    AppliesOutlineTint = false,
                    OutlineTintColor = tintColour,
                    PlaysVFXOnActor = true,
                };
                target.ApplyEffect(freezeToApply, 1f, null);

            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        public static void ApplyDirectCheese(GameActor target, float duration, float cheeseAmount, float damageToDealOnUnfreeze, Color tintColour, Color deathTintColour, EffectResistanceType resistanceType, string identifier, bool tintsEnemy, bool tintsCorpse)
        {
            try
            {
                //ETGModConsole.Log("Attempted to apply direct freeze");
                GameActorCheeseEffect cheeseModifierEffect = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect;
                GameActorCheeseEffect cheeseToApply = new GameActorCheeseEffect
                {
                    duration = duration,
                    TintColor = tintColour,
                    DeathTintColor = deathTintColour,
                    effectIdentifier = identifier,
                    AppliesTint = tintsEnemy,
                    AppliesDeathTint = tintsCorpse,
                    resistanceType = resistanceType,
                    crystalNum = cheeseModifierEffect.crystalNum,
                    crystalRot = cheeseModifierEffect.crystalRot,
                    crystalVariation = cheeseModifierEffect.crystalVariation,
                    CheeseCrystals = cheeseModifierEffect.CheeseCrystals,
                    debrisAngleVariance = cheeseModifierEffect.debrisAngleVariance,
                    debrisMaxForce = cheeseModifierEffect.debrisMaxForce,
                    debrisMinForce = cheeseModifierEffect.debrisMinForce,
                    OverheadVFX = cheeseModifierEffect.OverheadVFX,
                    vfxExplosion = cheeseModifierEffect.vfxExplosion,
                    stackMode = cheeseModifierEffect.stackMode,
                    maxStackedDuration = cheeseModifierEffect.maxStackedDuration,
                    AffectsEnemies = true,
                    AffectsPlayers = false,
                    AppliesOutlineTint = false,
                    OutlineTintColor = tintColour,
                    PlaysVFXOnActor = true,
                    CheeseGoop = cheeseModifierEffect.CheeseGoop,
                    CheeseGoopRadius = cheeseModifierEffect.CheeseGoopRadius,
                    CheeseAmount = cheeseAmount
                };
                target.ApplyEffect(cheeseToApply, 1f, null);

            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }
        public static void ApplyDirectPoison(GameActor target, float duration, float dps, Color tintColour, Color deathTintColour, EffectResistanceType resistanceType, string identifier, bool tintsEnemy, bool tintsCorpse)
        {
            GameActorHealthEffect irradiatedLeadEffect = Gungeon.Game.Items["irradiated_lead"].GetComponent<BulletStatusEffectItem>().HealthModifierEffect;
            GameActorHealthEffect poisonToApply = new GameActorHealthEffect
            {
                duration = duration,
                DamagePerSecondToEnemies = dps,
                TintColor = tintColour,
                DeathTintColor = deathTintColour,
                effectIdentifier = identifier,
                AppliesTint = tintsEnemy,
                AppliesDeathTint = tintsCorpse,
                resistanceType = resistanceType,

                //Eh
                OverheadVFX = irradiatedLeadEffect.OverheadVFX,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                ignitesGoops = false,
                OutlineTintColor = tintColour,
                PlaysVFXOnActor = false,
            };
            if (target && target.aiActor && target.healthHaver && target.healthHaver.IsAlive)
            {
                target.ApplyEffect(poisonToApply, 1f, null);
            }
        }
        public static void ApplyDirectSlow(GameActor target, float duration, float speedMultiplier, Color tintColour, Color deathTintColour, EffectResistanceType resistanceType, string identifier, bool tintsEnemy, bool tintsCorpse)
        {
            Gun gun = ETGMod.Databases.Items["triple_crossbow"] as Gun;
            GameActorSpeedEffect gameActorSpeedEffect = gun.DefaultModule.projectiles[0].speedEffect;
            GameActorSpeedEffect speedToApply = new GameActorSpeedEffect
            {
                duration = duration,
                TintColor = tintColour,
                DeathTintColor = deathTintColour,
                effectIdentifier = identifier,
                AppliesTint = tintsEnemy,
                AppliesDeathTint = tintsCorpse,
                resistanceType = resistanceType,
                SpeedMultiplier = speedMultiplier,

                //Eh
                OverheadVFX = gameActorSpeedEffect.OverheadVFX,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                OutlineTintColor = tintColour,
                PlaysVFXOnActor = false,
            };
            if (target && target.aiActor && target.healthHaver && target.healthHaver.IsAlive)
            {
                target.ApplyEffect(speedToApply, 1f, null);
            }
        }
        public static void ApplyDirectFire(GameActor target, float duration, float dps, Color tintColour, Color deathTintColour, EffectResistanceType resistanceType, string identifier, bool tintsEnemy, bool tintsCorpse)
        {
            GameActorFireEffect fireToApply = new GameActorFireEffect
            {
                duration = duration,
                DamagePerSecondToEnemies = dps,
                TintColor = tintColour,
                DeathTintColor = deathTintColour,
                effectIdentifier = identifier,
                AppliesTint = tintsEnemy,
                AppliesDeathTint = tintsCorpse,
                resistanceType = resistanceType,

                //Eh
                OverheadVFX = StaticStatusEffects.hotLeadEffect.OverheadVFX,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                ignitesGoops = StaticStatusEffects.hotLeadEffect.ignitesGoops,
                OutlineTintColor = tintColour,
                PlaysVFXOnActor = StaticStatusEffects.hotLeadEffect.PlaysVFXOnActor,
            };
            if (target && target.aiActor && target.healthHaver && target.healthHaver.IsAlive)
            {
                target.ApplyEffect(fireToApply, 1f, null);
            }
        }
    }
    class StatusEffectHelper
    {
        public static GameActorCheeseEffect GenerateCheese(float length = 10f, float intensity = 50f)
        {
            GameActorCheeseEffect customCheese = new GameActorCheeseEffect
            {
                duration = length,
                TintColor = StaticStatusEffects.elimentalerCheeseEffect.TintColor,
                DeathTintColor = StaticStatusEffects.elimentalerCheeseEffect.DeathTintColor,
                effectIdentifier = "Cheese",
                AppliesTint = true,
                AppliesDeathTint = true,
                resistanceType = EffectResistanceType.None,
                CheeseAmount = intensity,

                //Eh
                OverheadVFX = StaticStatusEffects.elimentalerCheeseEffect.OverheadVFX,
                AffectsPlayers = StaticStatusEffects.elimentalerCheeseEffect.AffectsPlayers,
                AppliesOutlineTint = StaticStatusEffects.elimentalerCheeseEffect.AppliesOutlineTint,
                OutlineTintColor = StaticStatusEffects.elimentalerCheeseEffect.OutlineTintColor,
                PlaysVFXOnActor = StaticStatusEffects.elimentalerCheeseEffect.PlaysVFXOnActor,
                AffectsEnemies = StaticStatusEffects.elimentalerCheeseEffect.AffectsEnemies,
                debrisAngleVariance = StaticStatusEffects.elimentalerCheeseEffect.debrisAngleVariance,
                debrisMaxForce = StaticStatusEffects.elimentalerCheeseEffect.debrisMaxForce,
                debrisMinForce = StaticStatusEffects.elimentalerCheeseEffect.debrisMinForce,
                CheeseCrystals = StaticStatusEffects.elimentalerCheeseEffect.CheeseCrystals,
                CheeseGoop = StaticStatusEffects.elimentalerCheeseEffect.CheeseGoop,
                CheeseGoopRadius = StaticStatusEffects.elimentalerCheeseEffect.CheeseGoopRadius,
                crystalNum = StaticStatusEffects.elimentalerCheeseEffect.crystalNum,
                crystalRot = StaticStatusEffects.elimentalerCheeseEffect.crystalRot,
                crystalVariation = StaticStatusEffects.elimentalerCheeseEffect.crystalVariation,
                maxStackedDuration = StaticStatusEffects.elimentalerCheeseEffect.maxStackedDuration,
                stackMode = StaticStatusEffects.elimentalerCheeseEffect.stackMode,
                vfxExplosion = StaticStatusEffects.elimentalerCheeseEffect.vfxExplosion,
            };
            return customCheese;
        }
        public static GameActorHealthEffect GeneratePoison(float dps = 3, bool damagesEnemies = true, float duration = 4, bool affectsPlayers = true)
        {
            GameActorHealthEffect customPoison = new GameActorHealthEffect
            {
                duration = duration,
                TintColor = StaticStatusEffects.irradiatedLeadEffect.TintColor,
                DeathTintColor = StaticStatusEffects.irradiatedLeadEffect.DeathTintColor,
                effectIdentifier = "Poison",
                AppliesTint = true,
                AppliesDeathTint = true,
                resistanceType = EffectResistanceType.Poison,
                DamagePerSecondToEnemies = dps,
                ignitesGoops = false,

                //Eh
                OverheadVFX = StaticStatusEffects.irradiatedLeadEffect.OverheadVFX,
                AffectsEnemies = damagesEnemies,
                AffectsPlayers = StaticStatusEffects.irradiatedLeadEffect.AffectsPlayers,
                AppliesOutlineTint = StaticStatusEffects.irradiatedLeadEffect.AppliesOutlineTint,
                OutlineTintColor = StaticStatusEffects.irradiatedLeadEffect.OutlineTintColor,
                PlaysVFXOnActor = StaticStatusEffects.irradiatedLeadEffect.PlaysVFXOnActor,
            };
            return customPoison;
        }
        public static GameActorFireEffect GenerateFireEffect(float dps = 3, bool damagesEnemies = true, float duration = 4)
        {
            GameActorFireEffect customFire = new GameActorFireEffect
            {
                duration = duration,
                TintColor = StaticStatusEffects.hotLeadEffect.TintColor,
                DeathTintColor = StaticStatusEffects.hotLeadEffect.DeathTintColor,
                effectIdentifier = StaticStatusEffects.hotLeadEffect.effectIdentifier,
                AppliesTint = true,
                AppliesDeathTint = true,
                resistanceType = EffectResistanceType.Fire,
                DamagePerSecondToEnemies = dps,
                ignitesGoops = true,

                //Eh
                OverheadVFX = StaticStatusEffects.hotLeadEffect.OverheadVFX,
                AffectsEnemies = damagesEnemies,
                AffectsPlayers = StaticStatusEffects.hotLeadEffect.AffectsPlayers,
                AppliesOutlineTint = StaticStatusEffects.hotLeadEffect.AppliesOutlineTint,
                OutlineTintColor = StaticStatusEffects.hotLeadEffect.OutlineTintColor,
                PlaysVFXOnActor = StaticStatusEffects.hotLeadEffect.PlaysVFXOnActor,

                FlameVfx = StaticStatusEffects.hotLeadEffect.FlameVfx,
                flameBuffer = StaticStatusEffects.hotLeadEffect.flameBuffer,
                flameFpsVariation = StaticStatusEffects.hotLeadEffect.flameFpsVariation,
                flameMoveChance = StaticStatusEffects.hotLeadEffect.flameMoveChance,
                flameNumPerSquareUnit = StaticStatusEffects.hotLeadEffect.flameNumPerSquareUnit,
                maxStackedDuration = StaticStatusEffects.hotLeadEffect.maxStackedDuration,
                stackMode = StaticStatusEffects.hotLeadEffect.stackMode,
                IsGreenFire = StaticStatusEffects.hotLeadEffect.IsGreenFire,
            };
            return customFire;
        }
        public static GameActorCharmEffect GenerateCharmEffect(float duration)
        {
            GameActorCharmEffect charmEffect = new GameActorCharmEffect
            {
                duration = duration,
                TintColor = StaticStatusEffects.charmingRoundsEffect.TintColor,
                AppliesDeathTint = StaticStatusEffects.charmingRoundsEffect.AppliesDeathTint,
                AppliesTint = StaticStatusEffects.charmingRoundsEffect.AppliesTint,
                effectIdentifier = StaticStatusEffects.charmingRoundsEffect.effectIdentifier,
                DeathTintColor = StaticStatusEffects.charmingRoundsEffect.DeathTintColor,
                OverheadVFX = StaticStatusEffects.charmingRoundsEffect.OverheadVFX,
                AffectsEnemies = StaticStatusEffects.charmingRoundsEffect.AffectsEnemies,
                AppliesOutlineTint = StaticStatusEffects.charmingRoundsEffect.AppliesOutlineTint,
                AffectsPlayers = StaticStatusEffects.charmingRoundsEffect.AffectsPlayers,
                maxStackedDuration = StaticStatusEffects.charmingRoundsEffect.maxStackedDuration,
                OutlineTintColor = StaticStatusEffects.charmingRoundsEffect.OutlineTintColor,
                PlaysVFXOnActor = StaticStatusEffects.charmingRoundsEffect.PlaysVFXOnActor,
                resistanceType = StaticStatusEffects.charmingRoundsEffect.resistanceType,
                stackMode = StaticStatusEffects.charmingRoundsEffect.stackMode,
            };
            return charmEffect;
        }

        public static GameActorCharmEffect GeneratePermanentCharmEffect(bool applyTint = false, bool appliesVFX = true)
        {
            GameActorCharmEffect charmEffect = new GameActorCharmEffect
            {
                duration = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect.duration,
                TintColor = StaticStatusEffects.charmingRoundsEffect.TintColor,
                AppliesDeathTint = applyTint,
                AppliesTint = applyTint,
                effectIdentifier = StaticStatusEffects.charmingRoundsEffect.effectIdentifier,
                DeathTintColor = StaticStatusEffects.charmingRoundsEffect.DeathTintColor,
                OverheadVFX = StaticStatusEffects.charmingRoundsEffect.OverheadVFX,
                AffectsEnemies = StaticStatusEffects.charmingRoundsEffect.AffectsEnemies,
                AppliesOutlineTint = StaticStatusEffects.charmingRoundsEffect.AppliesOutlineTint,
                AffectsPlayers = StaticStatusEffects.charmingRoundsEffect.AffectsPlayers,
                maxStackedDuration = StaticStatusEffects.charmingRoundsEffect.maxStackedDuration,
                OutlineTintColor = StaticStatusEffects.charmingRoundsEffect.OutlineTintColor,
                PlaysVFXOnActor = appliesVFX,
                resistanceType = StaticStatusEffects.charmingRoundsEffect.resistanceType,
                stackMode = StaticStatusEffects.charmingRoundsEffect.stackMode,


            };
            return charmEffect;
        }
        public static GameActorPartyEffect GeneratePartyEffect(float duration)
        {
            GameActorPartyEffect commonParty = new GameActorPartyEffect
            {
                duration = duration,
                effectIdentifier = "party",
                resistanceType = EffectResistanceType.None,
                OverheadVFX = PartyStatusEffectSetup.PartyOverheadVFX,
                AffectsEnemies = true,
                AffectsPlayers = false,
                AppliesOutlineTint = false,
                PlaysVFXOnActor = false,
                AppliesTint = false,
                AppliesDeathTint = false,
            };
            return commonParty;
        }


    }
}