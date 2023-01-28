using System.Collections;
using System.Reflection;
using Alexandria.DungeonAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alexandria.ItemAPI;
using static Alexandria.EnemyAPI.EnemyBuilder;
using Alexandria;

namespace Reload
{
    class DungeonCore : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Gungeon Core";
            string resourceName = "Reload/Resources/Passives/GungeonCore";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<DungeonCore>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "A Gungeon Is You";
            string longDesc = "Forged by the Blacksmith and imbued with a soul by the Lich, it is a vile creature that controls the environment to kill gungeoneers.\n\n" +
                "Summons three random weak monsters to fight for you.\n\n" +
                "'Everyone knows you shoot the chests before you open them, EVERYONE!' - Gungeoneer's Guide";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 375f);
            item.quality = PickupObject.ItemQuality.B;
        }
     
        public override void DoEffect(PlayerController user)
        {
            if (LastOwner)
            {

                System.Random random = new System.Random();

                for (int i = 0; i < 3; i++)
                {
                    int index = random.Next(enemyList.Count);
                    IntVector2? bestRewardLocation = LastOwner.CurrentRoom.GetRandomVisibleClearSpot(2, 2);
                    AIActor spawnedDude = CompanionisedEnemyUtility.SpawnCompanionisedEnemy(user, enemyList[index], (IntVector2)bestRewardLocation, false, Color.red, 5, 2, false, true);
                    
                    spawnedDude.HandleReinforcementFallIntoRoom(0f);

                
                }
            }

        }


        /* private Transform CreateEmptySprite(AIActor target)
         {
          GameObject gameObject = new GameObject("suck image");
          gameObject.layer = 1;
          tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
          gameObject.transform.parent = SpawnManager.Instance.VFX;
          tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
          tk2dSprite.transform.position = target.sprite.transform.position;
          GameObject gameObject2 = new GameObject("image parent");
          gameObject2.transform.position = tk2dSprite.WorldCenter;
         tk2dSprite.transform.parent = gameObject2.transform;
           if (target.optionalPalette != null)
           {
              tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }*/



        public override bool CanBeUsed(PlayerController user)
        {
            
            if(LastOwner)
            {
                if (LastOwner.IsInCombat)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        public List<string> enemyList = new List<string>
        {
            "db35531e66ce41cbb81d507a34366dfe",
            "6b7ef9e5d05b4f96b04f05ef4a0d1b18",
            "128db2f0781141bcb505d8f00f9e4d47",
            "8bb5578fba374e8aae8e10b754e61d62",
            "4d37ce3d666b4ddda8039929225b7ede",
            "88b6b6a93d4b4234a67844ef4728382c",
            "01972dee89fc4404a5c408d50007dad5",
            "05891b158cd542b1a5f3df30fb67a7ff",
            "0239c0680f9f467dbe5c4aab7dd1eca6",
            "df7fb62405dc4697b7721862c7b6b3cd",
            "206405acad4d4c33aac6717d184dc8d4",
            "b54d89f9e802455cbb2b8a96a31e8259",
            "2752019b770f473193b08b4005dc781f",
            "336190e29e8a4f75ab7486595b700d4a",
            "c0ff3744760c4a2eb0bb52ac162056e6",
            "95ec774b5a75467a9ab05fa230c0c143",
            "2feb50a6a40f4f50982e89fd276f6f15",
            "6f22935656c54ccfb89fca30ad663a64",
            "98fdf153a4dd4d51bf0bafe43f3c77ff",
            "3cadf10c489b461f9fb8814abc1a09c1",
            "31a3ea0c54a745e182e22ea54844a82d",
            "f905765488874846b7ff257ff81d6d0c",
            "e5cffcfabfae489da61062ea20539887",
            "a400523e535f41ac80a43ff6b06dc0bf",
            "70216cae6c1346309d86d4a0b4603045"
        };

    }
}
