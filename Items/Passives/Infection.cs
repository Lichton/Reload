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
    class Infection : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Spentfection"; //Spentfection
            string resourceName = "Reload/Resources/Passives/Outbreak";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Infection>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "On A Romeroll";
            string longDesc = "A bioweapon sourced from black market gun dealers trying to pawn it off to the Hegemony of Man. They brought a live example, and things escalated from there.\n\n" +
            "Turns the first enemy killed in a room into a friendly spent, and compels it to spread the infection, reviving enemies killed as another of its kind.\n\n" +
            "'I make viruses, bacteria, and biological horrors for my 9 to 5. You'll have to be more specific.' - Alec 'Mercenary' Black, Self-Proclaimed Biologist";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.B;
            item.PlaceItemInAmmonomiconAfterItemById(159);
        }

        public override void Pickup(PlayerController player)
        {
            player.OnEnteredCombat += this.ResetFirstEnemySpent;
            player.OnAnyEnemyReceivedDamage += this.ConditionalSpawnZombie;
            base.Pickup(player);

        }


        private void ConditionalSpawnZombie(float damage, bool fatal, HealthHaver enemyHealth)
        {
            if (FirstEnemyRevived == false && fatal == true && Owner)
            {
                ItemToolbox.SpawnZombieSpent(enemyHealth.aiActor, true);
                FirstEnemyRevived = true;
            }
        }
        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.OnEnteredCombat -= this.ResetFirstEnemySpent;
            player.OnAnyEnemyReceivedDamage -= this.ConditionalSpawnZombie;
            return debrisObject;
        }

        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.OnEnteredCombat -= this.ResetFirstEnemySpent;
                Owner.OnAnyEnemyReceivedDamage -= this.ConditionalSpawnZombie;
            }
            base.OnDestroy();
        }

        private void ResetFirstEnemySpent()
        {
            FirstEnemyRevived = false;
        }

        public bool FirstEnemyRevived = false;
    }
}
