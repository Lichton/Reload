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
using MonoMod.RuntimeDetour;

namespace Reload
{
    class BrokenAlarmClock : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Broken Alarm Clock";//broken af
            string resourceName = "Reload/Resources/Passives/BrokenAlarmClock";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<BrokenAlarmClock>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Gunnedhog Day";
            string longDesc = "Always shows the time as 5:05 + 7h 45m, despite that being nonsense.\n\n" +
                "Resets the floor after you complete it if it was done in less then 10 minutes. Activating this effect decreases the time limit, and if it reaches 5 gives you a suprise. \n\n" +
                "'Sonny, you're a disgrace! Wait, put down the gun!' - Paradox's Grandfather";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.PlaceItemInAmmonomiconAfterItemById(279);

            Hook alarmHook = new Hook(
              typeof(GameManager).GetMethod("LoadCustomLevel", BindingFlags.Instance | BindingFlags.Public),
              typeof(BrokenAlarmClock).GetMethod("SaveFloor")
          );
        }

        public static void SaveFloor(Action<GameManager, string> orig, GameManager self, string DungeonName)
        {
            if (GameManager.Instance.GetLastLoadedLevelDefinition() != null)
            {
                floorGun = GameManager.Instance.GetLastLoadedLevelDefinition();
                timeParadox = 0;
            }
            orig(self, DungeonName);
        }

        public override void Pickup(PlayerController player)
        {
            FixFloorLoopPickup(player);
            player.OnNewFloorLoaded += this.FixFloorLoop;
            base.Pickup(player);
        }

        private void FixFloorLoopPickup(PlayerController player)
        {
            UIToolbox.TextBox(Color.white, TimeLimit / 60 + " Minute Limit", player.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
        }

        public override void Update()
        {
            base.Update();
            timeParadox += BraveTime.DeltaTime;
            if(timeParadox > 1 && doNotification)
            {
                if (Owner)
                {
                    UIToolbox.TextBox(Color.white, TimeLimit / 60 + " Minute Limit", Owner.gameObject, dfPivotPoint.TopCenter, new Vector3(0.7f, 1, 0), 3, 0.5f, 0.5f, 0.5f, 0);
                }
                doNotification = false;
            }
        }

        public bool doNotification = true;
        private void FixFloorLoop(PlayerController obj)
        {
           
            
            if (floorGun != null && timeParadox <= TimeLimit && timeParadox > 0)
            {
                TimeLimit -= 60;
                if (TimeLimit > 300)
                {
                    GameManager.Instance.LoadCustomLevel(floorGun.dungeonSceneName);
                    doNotification = true;
                }
                else
                {
                    Destroy(this);
                    IntVector2 bestRewardLocation = obj.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                    Chest syn_Chest = GameManager.Instance.RewardManager.Synergy_Chest;
                    syn_Chest.IsLocked = false;
                    Chest.Spawn(syn_Chest, bestRewardLocation);
                }
            }

            //fix bug with reloading game somehow, perhaps save this with saveapi??
        }
        float TimeLimit = 600;
        


        public override DebrisObject Drop(PlayerController player)
        {
            player.OnNewFloorLoaded -= this.FixFloorLoop;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.OnNewFloorLoaded -= this.FixFloorLoop;
            }
            base.OnDestroy();
        }

        public static GameLevelDefinition floorGun;
        private static float timeParadox = 0;
    }
}