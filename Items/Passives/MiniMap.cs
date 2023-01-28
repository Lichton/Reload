using System;
using System.Collections.Generic;
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

namespace Reload
{
    class MiniMap : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Mini-Map";
            string resourceName = "Reload/Resources/Passives/MiniMap";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MiniMap>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Heavily Undersized Display";
            string longDesc = "An impractically small map with barely enough space to display a small cluster of rooms.\n\n" +
                "Displays a random room and all rooms connected to it on the map when you enter a floor.\n\n" +
                "'It's a MINI-MAP for a reason, I don't want it blocking my whole line of sight! The regular maps are made for bludgeoning!' - Map Knight, while beating the interviewer with a rolled-up map.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(137);

            Game.Items.Rename("rld:mini-map", "rld:mini_map");
        }
        bool first_pickup = true;
        public override void Pickup(PlayerController player)
        {
            player.OnNewFloorLoaded += GenerateCluster;
            base.Pickup(player);
            if (first_pickup)
            {
                GenerateCluster(player);
                first_pickup = false;
            }
        }
        private void GenerateCluster(PlayerController obj)
        {
            for (int x = 0; x < 250; x++)
            {
                RoomHandler RandomRoom = GenerateRandomRoom();
                if (IsValidRoom(RandomRoom) && !(RandomRoom == obj.CurrentRoom))
                {
                    RevealRoom(RandomRoom);
                    foreach (RoomHandler ConnectedRoom in RandomRoom.connectedRooms)
                    {
                        if (IsValidRoomConnected(ConnectedRoom))
                        {
                            RevealRoom(ConnectedRoom);
                            break;
                        }
                    }
                }
               
            }
        }
        public void RevealRoom(RoomHandler roomToReveal)
        {
            Minimap.Instance.RevealMinimapRoom(roomToReveal, true, true, false);
            Minimap.Instance.RegenerateMapTilemap();
        }

        public RoomHandler GenerateRandomRoom()
        {
            System.Random r = new System.Random();
            int index = r.Next(GameManager.Instance.Dungeon.data.rooms.Count);
            RoomHandler SelectedRoom = GameManager.Instance.Dungeon.data.rooms[index];
            return SelectedRoom;
        }
        public bool IsValidRoom(RoomHandler BoolChecker)
        {
            return BoolChecker != GameManager.Instance.PrimaryPlayer.CurrentRoom && BoolChecker.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.ENTRANCE && BoolChecker.RevealedOnMap != true && BoolChecker.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.SPECIAL && BoolChecker.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.SECRET && BoolChecker.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.REWARD;
        }
        public bool IsValidRoomConnected(RoomHandler BoolChecker)
        {
            return BoolChecker.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.SECRET;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.OnNewFloorLoaded -= GenerateCluster;
            return debrisObject;
        }

        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.OnNewFloorLoaded -= GenerateCluster;
            }
            base.OnDestroy();
        }

    }
}