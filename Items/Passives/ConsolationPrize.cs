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
    class ConsolationPrize: PassiveItem
    {

        public static void Init()
        {
            string itemName = "Consolation Prize"; //Make Assault & Battery synergy with Battery Bullets!
            string resourceName = "Reload/Resources/Passives/ConsolationPrize";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ConsolationPrize>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Gunner Up";
            string longDesc = "A second place medal for an unknown competition. Its powers of pity reward your failures a little.\n\n" +
                "Grants an extra pickup on defeating a boss or a subboss when you've taken damage during the fight.\n\n" +
                "'And so I dub thee: NOT THE WINNER!' - Lord Janet";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(326);
        }

        public override void Pickup(PlayerController player)
        {
            Alexandria.Misc.CustomActions.OnBossKilled += this.DropPrize;
            base.Pickup(player);
        }

        private void DropPrize(HealthHaver arg1, bool arg2)
        {
            if(arg2 == false && Owner)
            {
                 if(Owner.CurrentRoom.PlayerHasTakenDamageInThisRoom)
                {
                   IntVector2 Pos = Owner.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                    int listItem = BraveUtility.RandomElement(list);
                    var debObj = LootEngine.SpawnItem(PickupObjectDatabase.GetById(listItem).gameObject, Pos.ToVector2(), Vector2.zero, 0);
                    AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", debObj.gameObject);
                    ItemToolbox.DoConfetti(Pos.ToVector2());
                    Instantiate<GameObject>(EasyVFXDatabase.ItemPoofVFX, Pos.ToVector2(), Quaternion.identity);
                }
            }
        }


        public static List<int> list = new List<int>()
        {
            78, //Ammo
            600, //Spread Ammo
            565, //Glass Guon Stone
            120, //Armor
            224, //Blank
            67, //Key
            73, //Half Heart
            83, //Heart
        };


        public override DebrisObject Drop(PlayerController player)
        {
            Alexandria.Misc.CustomActions.OnBossKilled -= this.DropPrize;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Alexandria.Misc.CustomActions.OnBossKilled -= this.DropPrize;
            }
            base.OnDestroy();
        }

        public AIActor CurrentTarget = null;
    }
}
