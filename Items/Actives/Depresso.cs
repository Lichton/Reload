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
    class Depresso : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Depresso"; 
            string resourceName = "Reload/Resources/Passives/Depresso";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Depresso>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Life's Bean Better";
            string longDesc = "Made from the tears of Bullet Kin rejected by the Trigger Twins Troops Team.\n\n" +
                "Standing in water slows enemies, and using the item splashes water in a line.\n\n" +
                "'It is well known in my field that many species are effected by perception of colors. But how could this apply to warfare, you ask?' - Rane Bo, Weapons Expert & Psychologist";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.PlaceItemInAmmonomiconAfterItemById(427);
        }

    }
}
