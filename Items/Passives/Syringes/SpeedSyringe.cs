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
    class SpeedSyringe : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Speed Syringe";
            string resourceName = "Reload/Resources/Passives/Syringes/SpeedSyringe";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SpeedSyringe>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Speed Up?";
            string longDesc = "A syringe full of an odd-coloured liquid created by a strange 'merchant'.\n\n" +
                "Increases speed.\n\n" +
                "'Spice is the spice of life, kid!' - Reefer, Spice Addict";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.RemovePickupFromLootTables();
            SpeedID = item.PickupObjectId;
        }
        public static int SpeedID;

        public override void Pickup(PlayerController player)
        {
           

           
                OtherTools.ApplyStat(player, PlayerStats.StatType.MovementSpeed, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            base.Pickup(player);
            if (player != null)
            {
                player.RemovePassiveItem(SpeedID);
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
