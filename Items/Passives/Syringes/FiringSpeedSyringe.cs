using UnityEngine;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Gungeon;
using Dungeonator;
using Alexandria;
using SaveAPI;
using System.Collections;
using System.Reflection;
using Alexandria.DungeonAPI;
using System;
using Alexandria.Misc;

namespace Reload
{
    class FiringSpeedSyringe : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Volley Syringe";
            string resourceName = "Reload/Resources/Passives/Syringes/FiringSpeedSyringe";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<FiringSpeedSyringe>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Volley Up?";
            string longDesc = "A syringe full of an odd-coloured liquid created by a strange 'merchant'.\n\n" +
                "Increases accuracy.\n\n" +
                "'Spice is the spice of life, kid!' - Reefer, Spice Addict";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.RemovePickupFromLootTables();
            VolleyUpID = item.PickupObjectId;
        }
        public static int VolleyUpID;

        public override void Pickup(PlayerController player)
        {
           

                OtherTools.ApplyStat(player, PlayerStats.StatType.RateOfFire, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
  
            base.Pickup(player);
            if (player != null)
            {
                
                player.RemovePassiveItem(VolleyUpID);
            }
        }



        public override DebrisObject Drop(PlayerController player)
        {
            return base.Drop(player);
        }



        public override void OnDestroy()
        {
            if (Owner)
            {
            }
            base.OnDestroy();
        }
    }
}
