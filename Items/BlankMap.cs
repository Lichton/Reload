using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;
using Dungeonator;
using Alexandria;
using SaveAPI;
using System.Collections;
using System.Reflection;
using Alexandria.DungeonAPI;

namespace Reload
{
    class BlankMap : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Mini-Map";
            string resourceName = "Reload/Resources/MiniMap";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<BlankMap>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Heavily Undersized Display";
            string longDesc = "An impractically small map with barely enough space to display a small cluster of rooms.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rkm");

            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(137);
        }

        public bool chucklefuck = true;
        public override void Pickup(PlayerController player)
        {
            player.OnNewFloorLoaded += this.Fuc;
            if (chucklefuck == true)
            {
                chucklefuck = false;
                this.Fuc(player);
            }
            base.Pickup(player);
        }
        bool keeprevealing = true;
        bool connectPass = false;
        private void Fuc(PlayerController obj)
        {
            System.Random r = new System.Random();
            int index = r.Next(GameManager.Instance.Dungeon.data.rooms.Count);
            RoomHandler RandomRoom = GameManager.Instance.Dungeon.data.rooms[index];
            for (int x = 0; x < 250 && keeprevealing == true; x++)
            {

                if (IsValidRoom(RandomRoom))
                {
                    foreach (RoomHandler ConnectedRoom in RandomRoom.connectedRooms)
                    {
                        if (IsValidRoom(ConnectedRoom))
                        {
                            connectPass = true;
                        }
                    }
                    if (connectPass == true)
                    {
                        Minimap.Instance.RevealMinimapRoom(RandomRoom, true, false, RandomRoom == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                        RandomRoom.visibility = RoomHandler.VisibilityStatus.VISITED;
                        Minimap.Instance.RegenerateMapTilemap();
                        foreach (RoomHandler rommConnects in RandomRoom.connectedRooms)
                        {
                            if (rommConnects.IsSecretRoom != true)
                            {
                                Minimap.Instance.RevealMinimapRoom(rommConnects, true, false, rommConnects == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                                rommConnects.visibility = RoomHandler.VisibilityStatus.VISITED;
                                Minimap.Instance.RegenerateMapTilemap();
                            }
                        }
                        break;
                    }
                }
                r = new System.Random();
                index = r.Next(GameManager.Instance.Dungeon.data.rooms.Count);
                RandomRoom = GameManager.Instance.Dungeon.data.rooms[index];
                connectPass = false;
            }
        }


        public bool theRoom = true;

        public bool IsValidRoom(RoomHandler BoolChecker)
        {
            return BoolChecker != GameManager.Instance.PrimaryPlayer.CurrentRoom && BoolChecker.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.ENTRANCE && BoolChecker.RevealedOnMap != true;

        }



 



        public override DebrisObject Drop(PlayerController player)
        {


            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.OnNewFloorLoaded -= this.Fuc;
            }
            base.OnDestroy();
        }
    }
}