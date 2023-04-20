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
    class SyringeBelt : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Syringe Belt"; //Make Assault & Battery synergy with Battery Bullets!
            string resourceName = "Reload/Resources/Passives/SyringeBelt";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SyringeBelt>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Only Stings A Little";
            string longDesc = "A belt full of syringes of various types, enchanted to only be drawn in the presence of other items.\n\n" +
                "Gives a 50% chance for chests to contain stat-increasing syringes.\n\n" +
                "'Kid, did you just steal my belt? Ah, what the bullet hell. Keep it.' - Reefer, Drug Dealer";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.C;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.REEFER_PURCHASE, true);
        }

        public override void Pickup(PlayerController player)
        {
            Alexandria.Misc.CustomActions.OnChestPreOpen += this.SpawnSyringeChance;
            base.Pickup(player);
        }

        private void SpawnSyringeChance(Chest arg1, PlayerController arg2)
        {
            if (UnityEngine.Random.Range(1, 3) == 2)
            { 
                arg1.PredictContents(arg2);
                int listItem = BraveUtility.RandomElement(list);
                PickupObject breakfast = PickupObjectDatabase.GetById(listItem);
                arg1.contents.Add(breakfast);
            }
           
            
        }

        public List<int> list =
        new List<int>
        {
            DamageSyringe.DamageUpID,
        CursedSyringe.CursedUpID, 
            CoolnessSyringe.CoolnessID, 
            BulletVelocitySyringe.VelocityUpID, 
            AccuracySyringe.AccuracyUpID, 
            FiringSpeedSyringe.VolleyUpID,
            HealthSyringe.HealthID, 
            RangeSyringe.RangeID, 
            ReloadSyringe.ReloadUpID,
            SpeedSyringe.SpeedID,
        };

        public override DebrisObject Drop(PlayerController player)
        {
            Alexandria.Misc.CustomActions.OnChestPreOpen -= this.SpawnSyringeChance;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Alexandria.Misc.CustomActions.OnChestPreOpen -= this.SpawnSyringeChance;
            }
            base.OnDestroy();
        }

        public AIActor CurrentTarget = null;
    }
}
