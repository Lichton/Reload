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
    class RangeSyringe : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Range Syringe";
            string resourceName = "Reload/Resources/Passives/Syringes/RangeSyringe";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RangeSyringe>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Range Up?";
            string longDesc = "A syringe full of an odd-coloured liquid created by a strange 'merchant'.\n\n" +
                "Increases range.\n\n" +
                "'Spice is the spice of life, kid!' - Reefer, Spice Addict";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.RemovePickupFromLootTables();
            RangeID = item.PickupObjectId;
        }
        public static int RangeID;

        public override void Pickup(PlayerController player)
        {
           

       
                OtherTools.ApplyStat(player, PlayerStats.StatType.RangeMultiplier, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
 
            base.Pickup(player);
            if (player != null)
            {
                player.RemovePassiveItem(RangeID);
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
