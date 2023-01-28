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
    class CoolnessSyringe : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Coolness Syringe";
            string resourceName = "Reload/Resources/Passives/Syringes/CoolnessSyringe";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CoolnessSyringe>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Coolness Up?";
            string longDesc = "A syringe full of an odd-coloured liquid created by a strange 'merchant'.\n\n" +
                "Increases coolness.\n\n" +
                "'Spice is the spice of life, kid!' - Reefer, Spice Addict";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.RemovePickupFromLootTables();
            CoolnessID = item.PickupObjectId;
        }
        public static int CoolnessID;

        public override void Pickup(PlayerController player)
        {
           

           
                OtherTools.ApplyStat(player, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);

            base.Pickup(player);
            if (player != null)
            {
                player.RemovePassiveItem(CoolnessID);
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
