using Gungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Reload
{
    public static class SyringeHelper
    {
        public static void DoSideEffect(PlayerController player, TypeOfSyringe type)
        {
            int integer = UnityEngine.Random.Range(1, 11);
            switch (integer)
            {
                case 1:
               
                    UIToolbox.TextBox(Color.red, "ROID RAGE!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    player.forceFire = true;
                    GameManager.Instance.StartCoroutine(fixForcedFire(player));
                    break;
                case 2:
               
                    Pixelator.Instance.AdditionalCoreStackRenderPass = new Material(ShaderCache.Acquire("Brave/Internal/RainbowChestShader"));
                    Pixelator.Instance.AdditionalCoreStackRenderPass.SetFloat("_AllColorsToggle", 1f);
                    player.OnNewFloorLoaded += Unrainbow; 
                    UIToolbox.TextBox(Color.red, "HALLUCINOGEN!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 3:
                  
                    UIToolbox.TextBox(Color.red, "ADDICTION!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 4:
                  
                    UIToolbox.TextBox(Color.red, "BAD BATCH!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 5:
                
                    player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Overdose");
                    UIToolbox.TextBox(Color.red, "OVERDOSE!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 6:
              
                    DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef);
                    goop.TimedAddGoopCircle(player.CenterPosition, 5f, 0.5f);
                    UIToolbox.TextBox(Color.red, "POISONED!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 7:
                

                    UIToolbox.TextBox(Color.red, "WOOZY!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 8:
                  
                    UIToolbox.TextBox(Color.red, "SHAKY HANDS!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 9:
                  
                    UIToolbox.TextBox(Color.red, "ADVERSE REACTION!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    break;
                case 10:
            
                    UIToolbox.TextBox(Color.red, "WRONG DOSAGE!", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                    switch (type)
                    {
                        case TypeOfSyringe.DAMAGEUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                        case TypeOfSyringe.CURSED:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);
                            break;
                        case TypeOfSyringe.SPEEDUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.MovementSpeed, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                        case TypeOfSyringe.RELOADUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.ReloadSpeed, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                        case TypeOfSyringe.ACCURATEUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.Accuracy, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                        case TypeOfSyringe.COOLNESSUP:
                            if (player.stats.GetStatValue(PlayerStats.StatType.Coolness) > 0)
                            {
                                OtherTools.ApplyStat(player, PlayerStats.StatType.Coolness, -1f, StatModifier.ModifyMethod.ADDITIVE);
                            }
                            break;
                        case TypeOfSyringe.HEALTHUP:
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Wrong Dosage", CoreDamageTypes.None, DamageCategory.Unstoppable, true);
                            break;
                        case TypeOfSyringe.RANGEUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.RangeMultiplier, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                        case TypeOfSyringe.BULLETVELOCITYUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.ProjectileSpeed, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                        case TypeOfSyringe.FIRINGSPEEDUP:
                            OtherTools.ApplyStat(player, PlayerStats.StatType.RateOfFire, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                            break;
                    }
                    break;
            }
        }

        private static void Unrainbow(PlayerController obj)
        {
            Pixelator.Instance.AdditionalCoreStackRenderPass = null;
            obj.OnNewFloorLoaded -= Unrainbow;
        }

        public enum TypeOfSyringe
        {
            DAMAGEUP,
            CURSED,
            SPEEDUP,
            RELOADUP,
            ACCURATEUP,
            COOLNESSUP,
            HEALTHUP,
            RANGEUP,
            BULLETVELOCITYUP,
            FIRINGSPEEDUP
        }

        private static IEnumerator fixForcedFire(PlayerController player)
        {
            yield return new WaitForSeconds(5);
            player.forceFire = false;
            yield break;
        }
    }
}
