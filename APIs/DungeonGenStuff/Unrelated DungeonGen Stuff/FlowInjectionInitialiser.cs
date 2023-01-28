using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using UnityEngine;
using Alexandria;
using SaveAPI;
using System.Text;
using Gungeon;
using Alexandria.DungeonAPI;
using static Alexandria.DungeonAPI.RoomFactory;
using System.Reflection;
using System.IO;
using Alexandria.ItemAPI;

namespace Reload
{
    public class FlowInjectionInitialiser : FlowDatabase
    {
		public static SharedInjectionData BaseSharedInjectionData;

		public static SharedInjectionData GungeonInjectionData;

		public static void InitializeFlows()
        {
			AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
			BaseSharedInjectionData = sharedAssets2.LoadAsset<SharedInjectionData>("Base Shared Injection Data");
			sharedAssets2 = null;
			AddBBRoom(false);
		}
		public static void AddBBRoom(bool refreshFlows = false)
		{
			AssetBundle shared_auto_001 = ResourceManager.LoadAssetBundle("shared_auto_001");

			GameObject iconPrefab =(shared_auto_001.LoadAsset("assets/data/prefabs/room icons/minimap_boss_icon.prefab") as GameObject);
			BulletBishopRoomPrefab = BuildFromResource("Reload/Resources/NPCs/Hostile/Bosses/BulletBishop/BishopRoom.room").room;
			BulletBishopRoomPrefab.associatedMinimapIcon = iconPrefab;
			BulletBishopRoom = new ProceduralFlowModifierData()
			{
				annotation = "Bullet Bishop Room",
				DEBUG_FORCE_SPAWN = false,
				OncePerRun = true,
				placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>() {
					ProceduralFlowModifierData.FlowModifierPlacementType.HUB_ADJACENT_CHAIN_START
				},
				roomTable = null,
				exactRoom = BulletBishopRoomPrefab,
				IsWarpWing = false,
				RequiresMasteryToken = false,
				chanceToLock = 0,
				selectionWeight = 1000,
				chanceToSpawn = 1,
				RequiredValidPlaceable = null,
				prerequisites = new DungeonPrerequisite[] {
					new DungeonPrerequisite
					{
						prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET,
						requiredTileset = GlobalDungeonData.ValidTilesets.CATHEDRALGEON,
						requireTileset = true,
						comparisonValue = 1f,
						encounteredObjectGuid = string.Empty,
						maxToCheck = TrackedMaximums.MOST_KEYS_HELD,
						requireDemoMode = false,
						requireCharacter = false,
						requiredCharacter = PlayableCharacters.Pilot,
						requireFlag = false,
						useSessionStatValue = false,
						encounteredRoom = null,
						requiredNumberOfEncounters = -1,
						saveFlagToCheck = GungeonFlags.TUTORIAL_COMPLETED,
						statToCheck = TrackedStats.GUNBERS_MUNCHED
					}
				},
				CanBeForcedSecret = false,
				RandomNodeChildMinDistanceFromEntrance = 0,
				exactSecondaryRoom = null,
				framedCombatNodes = 0,

			};
			BaseSharedInjectionData.InjectionData.Add(BulletBishopRoom);
		}
		public static PrototypeDungeonRoom BulletBishopRoomPrefab;
		public static ProceduralFlowModifierData BulletBishopRoom;
	}
}
