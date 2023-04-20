using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alexandria.ItemAPI;
using System.Reflection;
using FullInspector;
namespace Reload
{
    public class Prefabs
    {
        public static PrototypeDungeonRoom[] winchesterrooms;
        public static GenericRoomTable winchesterroomtable;
        public static GameObject WinchesterMinimapIcon;
        public static void InitCustomPrefabs()
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            winchesterroomtable = assetBundle.LoadAsset<GenericRoomTable>("winchesterroomtable");
            winchesterroomtable.includedRooms.elements = winchesterroomtable.includedRooms.elements.Shuffle();
            WinchesterMinimapIcon = assetBundle.LoadAsset<GameObject>("minimap_winchester_icon");

            List<PrototypeDungeonRoom> m_WinchesterRooms = new List<PrototypeDungeonRoom>();

            foreach (WeightedRoom wRoom in winchesterroomtable.includedRooms.elements) { m_WinchesterRooms.Add(wRoom.room); }

            if (m_WinchesterRooms.Count > 0)
            {
                foreach (PrototypeDungeonRoom winchesterRoom in m_WinchesterRooms)
                {
                    winchesterRoom.associatedMinimapIcon = WinchesterMinimapIcon;
                }
            }
            winchesterrooms = m_WinchesterRooms.ToArray();
        }
    }

}
