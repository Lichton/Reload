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

namespace Reload
{
    class MunitionsMixer : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Munitions Mixer"; //Make Assault & Battery synergy with Battery Bullets!
            string resourceName = "Reload/Resources/Passives/Depresso";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MunitionsMixer>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Spread And Green";
            string longDesc = "Gives both types of ammo the effects of the other type.\n\n" +
                "Spread ammo fills your current gun to full capacity, and regular ammo fills all of your guns partially.\n\n" +
                "'Double double fire and fight, ammo fill and bullets fright!' - The Three Witches";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.A;

            item.PlaceItemInAmmonomiconAfterItemById(524);
        }

        public override void Pickup(PlayerController player)
        {
          
            base.Pickup(player);
        }

        public int i = 0;

    


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

        public AIActor CurrentTarget = null;
    }
}
