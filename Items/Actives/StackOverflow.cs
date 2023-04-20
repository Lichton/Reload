using SaveAPI;
using System.Collections;
using System.Reflection;
using Alexandria.DungeonAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Reload
{
    class StackOverflow : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Item Pooler";
            string resourceName = "Reload/Resources/Passives/ActivePooler";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<StackOverflow>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 1f);
            string shortDesc = "Treasure Room";
            string longDesc = "Invented by a mad scientist bent on evening out his terrible luck with everyone elses' loot.\n\n" +
                "Interferes with the magicks that determine the contents of chests, forcing them to give you lunch instead.\n\n" +
                "'If everyone has terrible luck... NO ONE WILL BE UNLUCKY!' - Sinister";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.S;
            item.PlaceItemInAmmonomiconAfterItemById(573);
            
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return CheckBreakfasted;
        }

        public override void DoEffect(PlayerController user)
        {
            Alexandria.Misc.CustomActions.OnChestPreOpen += this.MakeChestLunch;
            AkSoundEngine.PostEvent("Play_OBJ_chestwarp_use_01", base.gameObject);
            CheckBreakfasted = false;

        }

        private void MakeChestLunch(Chest arg1, PlayerController arg2)
        {
            PickupObject breakfast = PickupObjectDatabase.GetById(422);
            List<PickupObject> lister = new List<PickupObject>();
            lister.Add(breakfast);
            arg1.contents = lister;
            CheckBreakfasted = true;
            Alexandria.Misc.CustomActions.OnChestPreOpen -= this.MakeChestLunch;
        }

        public static bool CheckBreakfasted = true;
    }
}
