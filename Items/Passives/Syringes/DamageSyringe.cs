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
    class DamageSyringe : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Damage Syringe";
            string resourceName = "Reload/Resources/Passives/Syringes/DamageSyringe";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<DamageSyringe>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Damage Up?";
            string longDesc = "A syringe full of an odd-coloured liquid created by a strange 'merchant'.\n\n" +
                "Increases damage.\n\n" +
                "'Spice is the spice of life, kid!' - Reefer, Spice Addict";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(259);
            item.RemovePickupFromLootTables();
            DamageUpID = item.PickupObjectId;
        }
        public static int DamageUpID;

        public override void Pickup(PlayerController player)
        {
           
           
        
                OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            base.Pickup(player);
            if (player != null)
            {
                player.RemovePassiveItem(DamageUpID);
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
