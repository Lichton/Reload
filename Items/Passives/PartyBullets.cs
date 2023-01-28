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
    class PartyBullets : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Party Bullets"; //Make Assault & Battery synergy with Battery Bullets!
            string resourceName = "Reload/Resources/Passives/PartyBullets";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PartyBullets>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Everybody Loves A Clown";
            string longDesc = "These joyful rounds bring hilarity and enjoyment to all they contact.\n\n" +
                "Small chance to inflict enemies with party fever, exploding them into confetti after a short time.\n\n" +
                "'What'dya call a bullet with a driver's license? A ROUND-ABOUT!' - Bumbles, a Clown-Kin";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.A;
            item.SetTag("bullet_modifier");
            item.PlaceItemInAmmonomiconAfterItemById(524);
        }

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.DoTheActions;
            base.Pickup(player);
        }

        public int i = 0;

        private void DoTheActions(Projectile arg1, float arg2)
        {
            if (UnityEngine.Random.Range(1, 9) == 1)
            {
                arg1.AdjustPlayerProjectileTint(ExtendedColours.pink, 1);
                arg1.statusEffectsToApply.Add(StaticStatusEffects.StandardPartyEffect);
            }
        }


        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.DoTheActions;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.PostProcessProjectile -= this.DoTheActions;
            }
            base.OnDestroy();
        }

        public AIActor CurrentTarget = null;
    }
}
