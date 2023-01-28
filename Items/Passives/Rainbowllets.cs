using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Gungeon;
using Dungeonator;
using Alexandria;
using SaveAPI;
using System.Collections;
using System.Reflection;
using Alexandria.DungeonAPI;
using System;
using System.Collections.Generic;

namespace Reload
{
    class Rainbowllets : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Rainbowllets"; //Make Assault & Battery synergy with Battery Bullets!
            string resourceName = "Reload/Resources/Passives/Rainbowllets";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Rainbowllets>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Sunshine & Lollipops";
            string longDesc = "Contrary to popular belief, these bullets are not actually rainbow. Holographic technology overlays an image over the bullets, selecting a colour before being fired.\n\n" +
                "Bullets are tinted a randomly selected colour, with a chance to apply a corresponding effect.\n\n" +
                "'It is well known in my field that many species are effected by perception of colors. But how could this apply to warfare, you ask?' - Rane Bo, Weapons Expert & Psychologist";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.S;
            item.SetTag("bullet_modifier");
            item.PlaceItemInAmmonomiconAfterItemById(524);
        }

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.SelectColourBuff;
            player.PostProcessBeam += this.SelectColourBuffBeam;
            base.Pickup(player);
        }

        private void SelectColourBuffBeam(BeamController obj)
        {
            int color = UnityEngine.Random.Range(1, 7);
            DoColourizeBeam(obj, color);
        }

        private void DoColourizeBeam(BeamController beam, int color)
        {
            switch (color)
            {
                case 1:
                    beam.AdjustPlayerBeamTint(Color.red, 1);
                    beam.projectile.OnHitEnemy += this.BleedApplyChance;
                    break;
                case 2:
                    beam.AdjustPlayerBeamTint(ExtendedColours.orange, 1);
                    beam.projectile.OnHitEnemy += this.FireApplyChance;
                    break;
                case 3:
                    beam.AdjustPlayerBeamTint(Color.yellow, 1);
                    beam.projectile.OnHitEnemy += this.CheeseApplyChance;
                    break;
                case 4:
                    beam.AdjustPlayerBeamTint(Color.green, 1);
                    beam.projectile.OnHitEnemy += this.PoisonApplyChance;
                    break;
                case 5:
                    beam.AdjustPlayerBeamTint(Color.blue, 1);
                    beam.projectile.OnHitEnemy += this.FreezeApplyChance;
                    break;
                case 6:
                    beam.AdjustPlayerBeamTint(ExtendedColours.purple, 1);
                    beam.projectile.OnHitEnemy += this.FearApplyChanced;
                    break;
            }
        }

        public void DoColourize(Projectile arg1, int integer)
        {
            switch(integer)
            {
                case 1:
                    arg1.HasDefaultTint = true;
                    arg1.DefaultTintColor = Color.red;
                    arg1.OnHitEnemy += this.FireApplyChance;
                    break;
                case 2:
                    arg1.HasDefaultTint = true;
                    arg1.DefaultTintColor = ExtendedColours.orange;
                    arg1.OnHitEnemy += this.FireApplyChance;
                    break;
                case 3:
                    arg1.HasDefaultTint = true;
                    arg1.DefaultTintColor = Color.yellow;
                    arg1.OnHitEnemy += this.CheeseApplyChance;
                    break;
                case 4:
                    arg1.HasDefaultTint = true;
                    arg1.DefaultTintColor = Color.green;
                    arg1.OnHitEnemy += this.PoisonApplyChance;
                    break;
                case 5:
                    arg1.HasDefaultTint = true;
                    arg1.DefaultTintColor = Color.blue;
                    arg1.OnHitEnemy += this.FreezeApplyChance;
                    break;
                case 6:
                    arg1.HasDefaultTint = true;
                    arg1.DefaultTintColor = ExtendedColours.purple;
                    arg1.OnHitEnemy += this.FearApplyChanced;
                    break;
            }
        }
        private void BleedApplyChance(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (UnityEngine.Random.Range(1, 6) == 1)
            {
 
            }
        }
        private void CheeseApplyChance(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (UnityEngine.Random.Range(1, 6) == 1)
            {
               
                    ApplyDirectStatusEffects.ApplyDirectCheese(arg2.aiActor, 5f, 50f, 0f, Color.yellow, Color.yellow, EffectResistanceType.None, "rainbowCheese", true, true);

            }
        }
        private void FireApplyChance(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (UnityEngine.Random.Range(1, 6) == 1)
            {
                
                        ApplyDirectStatusEffects.ApplyDirectFire(arg2.aiActor, 5f, 3f, ExtendedColours.orange, ExtendedColours.orange, EffectResistanceType.Fire, "rainbowFire", true, true);

            }
        }
        private void PoisonApplyChance(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (UnityEngine.Random.Range(1, 6) == 1)
            {
            
                        ApplyDirectStatusEffects.ApplyDirectPoison(arg2.aiActor, 5f, 3f, Color.green, Color.green, EffectResistanceType.Poison, "rainbowPoison", true, true);

            }
        }

        private void FreezeApplyChance(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (UnityEngine.Random.Range(1, 6) == 1)
            {
           
                        ApplyDirectStatusEffects.ApplyDirectFreeze(arg2.aiActor, 5f, 50f, 0f, Color.blue, Color.blue, EffectResistanceType.Freeze, "rainbowFreeze", true, true);

            }
        }

        private void FearApplyChanced(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {

            if (UnityEngine.Random.Range(1, 6) == 1)
            {

                ApplyDirectStatusEffects.ApplyDirectSlow(arg2.aiActor, 5f, 3f, ExtendedColours.purple, ExtendedColours.purple, EffectResistanceType.Fire, "rainbowSlow", true, true);
            }
        }

        private void SelectColourBuff(Projectile arg1, float arg2)
        {
            int color = UnityEngine.Random.Range(1, 7);
            DoColourize(arg1, color);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.SelectColourBuff;
            player.PostProcessBeam -= this.SelectColourBuffBeam;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.PostProcessProjectile -= this.SelectColourBuff;
                Owner.PostProcessBeam -= this.SelectColourBuffBeam;
            }
            base.OnDestroy();
        }

        public AIActor CurrentTarget = null;
    }
}
