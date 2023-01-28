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
    class ReloadSyringe : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Reload Syringe";
            string resourceName = "Reload/Resources/Passives/Syringes/ReloadSyringe";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ReloadSyringe>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Reload Up?";
            string longDesc = "A syringe full of an odd-coloured liquid created by a strange 'merchant'.\n\n" +
                "Increases reload speed.\n\n" +
                "'Spice is the spice of life, kid!' - Reefer, Spice Addict";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.RemovePickupFromLootTables();
            ReloadUpID = item.PickupObjectId;
        }
        public static int ReloadUpID;

        public override void Pickup(PlayerController player)
        {
           

          
                OtherTools.ApplyStat(player, PlayerStats.StatType.ReloadSpeed, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
      
            base.Pickup(player);
            if (player != null)
            {
                player.RemovePassiveItem(ReloadUpID);
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
