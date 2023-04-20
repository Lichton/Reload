using Dungeonator;
using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tk2dRuntime.TileMap;
using UnityEngine;

namespace Reload
{
    class MiscUtilityCode
    {
        public static RoomHandler AddCustomRuntimeRoom(Dungeon dungeon, IntVector2 dimensions, GameObject roomPrefab, IntVector2? roomWorldPositionOverride = null, Vector3? roomPrefabPositionOverride = null)
        {
            IntVector2 RoomPosition = new IntVector2(10, 10);
            if (roomWorldPositionOverride.HasValue) { RoomPosition = roomWorldPositionOverride.Value; }
            IntVector2 intVector = new IntVector2(dungeon.data.Width + RoomPosition.x, RoomPosition.y);
            int newWidth = dungeon.data.Width + RoomPosition.x + dimensions.x;
            int newHeight = Mathf.Max(dungeon.data.Height, dimensions.y + RoomPosition.y);
            CellData[][] array = BraveUtility.MultidimensionalArrayResize(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
            CellArea cellArea = new CellArea(intVector, dimensions, 0);
            cellArea.IsProceduralRoom = true;
            dungeon.data.cellData = array;
            dungeon.data.ClearCachedCellData();
            RoomHandler roomHandler = new RoomHandler(cellArea);
            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    IntVector2 p = new IntVector2(i, j) + intVector;
                    CellData cellData = new CellData(p, CellType.FLOOR);
                    cellData.parentArea = cellArea;
                    cellData.parentRoom = roomHandler;
                    cellData.nearestRoom = roomHandler;
                    array[p.x][p.y] = cellData;
                    roomHandler.RuntimeStampCellComplex(p.x, p.y, CellType.FLOOR, DiagonalWallType.NONE);

                }
            }
            dungeon.data.rooms.Add(roomHandler);
            if (roomPrefabPositionOverride.HasValue)
            {
                float X = roomPrefabPositionOverride.Value.x;
                float Y = roomPrefabPositionOverride.Value.x;
                UnityEngine.Object.Instantiate(roomPrefab, new Vector3(intVector.x + X, intVector.y + Y, 0f), Quaternion.identity);
            }
            else
            {
                UnityEngine.Object.Instantiate(roomPrefab, new Vector3(intVector.x, intVector.y, 0f), Quaternion.identity);
            }
            DeadlyDeadlyGoopManager.ReinitializeData();
            return roomHandler;
        }
        public static RoomHandler AddCustomRuntimeRoom(PrototypeDungeonRoom prototype, bool addRoomToMinimap = true, bool addTeleporter = true, bool isSecretRatExitRoom = false, Action<RoomHandler> postProcessCellData = null, DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.STANDARD, bool allowProceduralDecoration = true, bool allowProceduralLightFixtures = true, bool suppressExceptionMessages = false)
        {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            tk2dTileMap m_tilemap = dungeon.MainTilemap;

            if (m_tilemap == null)
            {
                ETGModConsole.Log("ERROR: TileMap object is null! Something seriously went wrong!");
                Debug.Log("ERROR: TileMap object is null! Something seriously went wrong!");
                return null;
            }

            /*TK2DDungeonAssembler assembler = new TK2DDungeonAssembler();
            assembler.Initialize(dungeon.tileIndices);*/
            TK2DDungeonAssembler assembler = ReflectionHelpers.ReflectGetField<TK2DDungeonAssembler>(typeof(Dungeon), "assembler", dungeon);

            IntVector2 basePosition = IntVector2.Zero;
            IntVector2 basePosition2 = new IntVector2(50, 50);
            int num = basePosition2.x;
            int num2 = basePosition2.y;
            IntVector2 intVector = new IntVector2(int.MaxValue, int.MaxValue);
            IntVector2 intVector2 = new IntVector2(int.MinValue, int.MinValue);
            intVector = IntVector2.Min(intVector, basePosition);
            intVector2 = IntVector2.Max(intVector2, basePosition + new IntVector2(prototype.Width, prototype.Height));
            IntVector2 a = intVector2 - intVector;
            IntVector2 b = IntVector2.Min(IntVector2.Zero, -1 * intVector);
            a += b;
            IntVector2 intVector3 = new IntVector2(dungeon.data.Width + num, num);
            int newWidth = dungeon.data.Width + num * 2 + a.x;
            int newHeight = Mathf.Max(dungeon.data.Height, a.y + num * 2);
            CellData[][] array = BraveUtility.MultidimensionalArrayResize(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
            dungeon.data.cellData = array;
            dungeon.data.ClearCachedCellData();
            IntVector2 d = new IntVector2(prototype.Width, prototype.Height);
            IntVector2 b2 = basePosition + b;
            IntVector2 intVector4 = intVector3 + b2;
            CellArea cellArea = new CellArea(intVector4, d, 0);
            cellArea.prototypeRoom = prototype;
            RoomHandler targetRoom = new RoomHandler(cellArea);
            for (int k = -num; k < d.x + num; k++)
            {
                for (int l = -num; l < d.y + num; l++)
                {
                    IntVector2 p = new IntVector2(k, l) + intVector4;
                    if ((k >= 0 && l >= 0 && k < d.x && l < d.y) || array[p.x][p.y] == null)
                    {
                        CellData cellData = new CellData(p, CellType.WALL);
                        cellData.positionInTilemap = cellData.positionInTilemap - intVector3 + new IntVector2(num2, num2);
                        cellData.parentArea = cellArea;
                        cellData.parentRoom = targetRoom;
                        cellData.nearestRoom = targetRoom;
                        cellData.distanceFromNearestRoom = 0f;
                        array[p.x][p.y] = cellData;
                    }
                }
            }
            dungeon.data.rooms.Add(targetRoom);
            try
            {
                targetRoom.WriteRoomData(dungeon.data);
            }
            catch (Exception)
            {
                if (!suppressExceptionMessages)
                {
                    ETGModConsole.Log("WARNING: Exception caused during WriteRoomData step on room: " + targetRoom.GetRoomName());
                }
            }
            try
            {
                dungeon.data.GenerateLightsForRoom(dungeon.decoSettings, targetRoom, GameObject.Find("_Lights").transform, lightStyle);
            }
            catch (Exception)
            {
                if (!suppressExceptionMessages)
                {
                    ETGModConsole.Log("WARNING: Exception caused during GeernateLightsForRoom step on room: " + targetRoom.GetRoomName());
                }
            }

            postProcessCellData?.Invoke(targetRoom);

            if (targetRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET) { targetRoom.BuildSecretRoomCover(); }
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
            tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
            string str = UnityEngine.Random.Range(10000, 99999).ToString();
            gameObject.name = "Glitch_" + "RuntimeTilemap_" + str;
            component.renderData.name = "Glitch_" + "RuntimeTilemap_" + str + " Render Data";
            component.Editor__SpriteCollection = dungeon.tileIndices.dungeonCollection;
            try
            {
                TK2DDungeonAssembler.RuntimeResizeTileMap(component, a.x + num2 * 2, a.y + num2 * 2, m_tilemap.partitionSizeX, m_tilemap.partitionSizeY);
                IntVector2 intVector5 = new IntVector2(prototype.Width, prototype.Height);
                IntVector2 b3 = basePosition + b;
                IntVector2 intVector6 = intVector3 + b3;
                for (int num4 = -num2; num4 < intVector5.x + num2; num4++)
                {
                    for (int num5 = -num2; num5 < intVector5.y + num2 + 2; num5++)
                    {
                        assembler.BuildTileIndicesForCell(dungeon, component, intVector6.x + num4, intVector6.y + num5);
                    }
                }
                RenderMeshBuilder.CurrentCellXOffset = intVector3.x - num2;
                RenderMeshBuilder.CurrentCellYOffset = intVector3.y - num2;
                component.ForceBuild();
                RenderMeshBuilder.CurrentCellXOffset = 0;
                RenderMeshBuilder.CurrentCellYOffset = 0;
                component.renderData.transform.position = new Vector3(intVector3.x - num2, intVector3.y - num2, intVector3.y - num2);
            }
            catch (Exception ex)
            {
                if (!suppressExceptionMessages)
                {
                    ETGModConsole.Log("WARNING: Exception occured during RuntimeResizeTileMap / RenderMeshBuilder steps!");
                    Debug.Log("WARNING: Exception occured during RuntimeResizeTileMap/RenderMeshBuilder steps!");
                    Debug.LogException(ex);
                }
            }
            targetRoom.OverrideTilemap = component;
            if (allowProceduralLightFixtures)
            {
                for (int num7 = 0; num7 < targetRoom.area.dimensions.x; num7++)
                {
                    for (int num8 = 0; num8 < targetRoom.area.dimensions.y + 2; num8++)
                    {
                        IntVector2 intVector7 = targetRoom.area.basePosition + new IntVector2(num7, num8);
                        if (dungeon.data.CheckInBoundsAndValid(intVector7))
                        {
                            CellData currentCell = dungeon.data[intVector7];
                            TK2DInteriorDecorator.PlaceLightDecorationForCell(dungeon, component, currentCell, intVector7);
                        }
                    }
                }
            }

            Pathfinder.Instance.InitializeRegion(dungeon.data, targetRoom.area.basePosition + new IntVector2(-3, -3), targetRoom.area.dimensions + new IntVector2(3, 3));

            if (prototype.usesProceduralDecoration && prototype.allowFloorDecoration && allowProceduralDecoration)
            {
                TK2DInteriorDecorator decorator = new TK2DInteriorDecorator(assembler);
                
                    decorator.HandleRoomDecoration(targetRoom, dungeon, m_tilemap);
                
            }

            targetRoom.PostGenerationCleanup();

            if (addRoomToMinimap)
            {
                targetRoom.visibility = RoomHandler.VisibilityStatus.VISITED;
                GameManager.Instance.StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(targetRoom, true, true, false));
                if (isSecretRatExitRoom) { targetRoom.visibility = RoomHandler.VisibilityStatus.OBSCURED; }
            }
            if (addTeleporter) { targetRoom.AddProceduralTeleporterToRoom(); }
            if (addRoomToMinimap) { Minimap.Instance.InitializeMinimap(dungeon.data); }
            DeadlyDeadlyGoopManager.ReinitializeData();
            return targetRoom;
        }

        public static IntVector2? GetRandomAvailableCellSmart(RoomHandler CurrentRoom, IntVector2 Clearence, bool relativeToRoom = false)
        {
            CellValidator cellValidator = delegate (IntVector2 c) {
                for (int X = 0; X < Clearence.x; X++)
                {
                    for (int Y = 0; Y < Clearence.y; Y++)
                    {
                        if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(c.x + X, c.y + Y) ||
                             GameManager.Instance.Dungeon.data[c.x + X, c.y + Y].type == CellType.PIT ||
                             GameManager.Instance.Dungeon.data[c.x + X, c.y + Y].isOccupied ||
                             GameManager.Instance.Dungeon.data[c.x + X, c.y + Y].type == CellType.WALL)
                        {
                            return false;
                        }
                    }
                }
                return true;
            };
            if (relativeToRoom)
            {
                return CurrentRoom.GetRandomAvailableCell(new IntVector2?(new IntVector2(Clearence.x, Clearence.y)), new CellTypes?(CellTypes.FLOOR), false, cellValidator) - CurrentRoom.area.basePosition;
            }
            else
            {
                return CurrentRoom.GetRandomAvailableCell(new IntVector2?(new IntVector2(Clearence.x, Clearence.y)), new CellTypes?(CellTypes.FLOOR), false, cellValidator);
            }
        }

        public static IntVector2? GetRandomAvailableCellForPlayer(Dungeon dungeon, RoomHandler currentRoom, bool relativeToRoom = false)
        {
            List<IntVector2> validCellsCached = new List<IntVector2>();
            for (int Width = -1; Width <= currentRoom.area.dimensions.x; Width++)
            {
                for (int height = -1; height <= currentRoom.area.dimensions.y; height++)
                {
                    int X = currentRoom.area.basePosition.x + Width;
                    int Y = currentRoom.area.basePosition.y + height;
                    if (!dungeon.data.isWall(X - 2, Y + 2) && !dungeon.data.isWall(X - 1, Y + 2) && !dungeon.data.isWall(X, Y + 2) && !dungeon.data.isWall(X + 1, Y + 2) && !dungeon.data.isWall(X + 2, Y + 2) &&
                        !dungeon.data.isWall(X - 2, Y + 1) && !dungeon.data.isWall(X - 1, Y + 1) && !dungeon.data.isWall(X, Y + 1) && !dungeon.data.isWall(X + 1, Y + 1) && !dungeon.data.isWall(X + 2, Y + 1) &&
                        !dungeon.data.isWall(X - 2, Y) && !dungeon.data.isWall(X - 1, Y) && !dungeon.data.isWall(X, Y) && !dungeon.data.isWall(X + 1, Y) && !dungeon.data.isWall(X + 2, Y) &&
                        !dungeon.data.isWall(X - 2, Y - 1) && !dungeon.data.isWall(X - 1, Y - 1) && !dungeon.data.isWall(X, Y - 1) && !dungeon.data.isWall(X + 1, Y - 1) && !dungeon.data.isWall(X + 2, Y - 1) &&
                        !dungeon.data.isWall(X - 2, Y - 2) && !dungeon.data.isWall(X - 1, Y - 2) && !dungeon.data.isWall(X, Y - 2) && !dungeon.data.isWall(X + 1, Y - 2) && !dungeon.data.isWall(X + 2, Y - 2) &&
                        !dungeon.data[X - 2, Y + 2].isOccupied && !dungeon.data[X - 1, Y + 2].isOccupied && !dungeon.data[X, Y + 2].isOccupied && !dungeon.data[X + 1, Y + 2].isOccupied && !dungeon.data[X + 2, Y + 2].isOccupied &&
                        !dungeon.data[X - 2, Y + 1].isOccupied && !dungeon.data[X - 1, Y + 1].isOccupied && !dungeon.data[X, Y + 1].isOccupied && !dungeon.data[X + 1, Y + 1].isOccupied && !dungeon.data[X + 2, Y + 1].isOccupied &&
                        !dungeon.data[X - 2, Y].isOccupied && !dungeon.data[X - 1, Y].isOccupied && !dungeon.data[X, Y].isOccupied && !dungeon.data[X + 1, Y].isOccupied && !dungeon.data[X + 2, Y].isOccupied &&
                        !dungeon.data[X - 2, Y - 1].isOccupied && !dungeon.data[X - 1, Y - 1].isOccupied && !dungeon.data[X, Y - 1].isOccupied && !dungeon.data[X + 1, Y - 1].isOccupied && !dungeon.data[X + 2, Y - 1].isOccupied &&
                        !dungeon.data[X - 2, Y - 2].isOccupied && !dungeon.data[X - 1, Y - 2].isOccupied && !dungeon.data[X, Y - 2].isOccupied && !dungeon.data[X + 1, Y - 2].isOccupied && !dungeon.data[X + 2, Y - 2].isOccupied &&
                        !dungeon.data.isPit(X - 2, Y + 2) && !dungeon.data.isPit(X - 1, Y + 2) && !dungeon.data.isPit(X, Y + 2) && !dungeon.data.isPit(X + 1, Y + 2) && !dungeon.data.isPit(X + 2, Y + 2) &&
                        !dungeon.data.isPit(X - 2, Y + 1) && !dungeon.data.isPit(X - 1, Y + 1) && !dungeon.data.isPit(X, Y + 1) && !dungeon.data.isPit(X + 1, Y + 1) && !dungeon.data.isPit(X + 2, Y + 1) &&
                        !dungeon.data.isPit(X - 2, Y) && !dungeon.data.isPit(X - 1, Y) && !dungeon.data.isPit(X, Y) && !dungeon.data.isPit(X + 1, Y) && !dungeon.data.isPit(X + 2, Y) &&
                        !dungeon.data.isPit(X - 2, Y - 1) && !dungeon.data.isPit(X - 1, Y - 1) && !dungeon.data.isPit(X, Y - 1) && !dungeon.data.isPit(X + 1, Y - 1) && !dungeon.data.isPit(X + 2, Y - 1) &&
                        !dungeon.data.isPit(X - 2, Y - 2) && !dungeon.data.isPit(X - 1, Y - 2) && !dungeon.data.isPit(X, Y - 2) && !dungeon.data.isPit(X + 1, Y - 2) && !dungeon.data.isPit(X + 2, Y - 2))
                    {
                        validCellsCached.Add(new IntVector2(X, Y));
                    }
                }
            }
            if (validCellsCached.Count > 0)
            {
                IntVector2 SelectedCell = BraveUtility.RandomElement(validCellsCached);
                IntVector2 RegisteredCell = (SelectedCell);
                validCellsCached.Remove(SelectedCell);
                if (relativeToRoom)
                {
                    return (SelectedCell - currentRoom.area.basePosition);
                }
                else
                {
                    return (SelectedCell);
                }
            }
            else
            {
                return null;
            }
        }

         public static DungeonPlaceable GenerateDungeonPlacable(GameObject ObjectPrefab = null, bool spawnsEnemy = false, bool useExternalPrefab = false, bool spawnsItem = false, string EnemyGUID = "479556d05c7c44f3b6abb3b2067fc778", int itemID = 307, Vector2? CustomOffset = null, bool itemHasDebrisObject = true, float spawnChance = 1f) {
            AssetBundle m_assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle m_assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            AssetBundle m_resourceBundle = ResourceManager.LoadAssetBundle("brave_resources_001");

            // Used with custom DungeonPlacable        
            GameObject ChestBrownTwoItems = m_assetBundle.LoadAsset<GameObject>("Chest_Wood_Two_Items");
            GameObject Chest_Silver = m_assetBundle.LoadAsset<GameObject>("chest_silver");
            GameObject Chest_Green = m_assetBundle.LoadAsset<GameObject>("chest_green");
            GameObject Chest_Synergy = m_assetBundle.LoadAsset<GameObject>("chest_synergy");
            GameObject Chest_Red = m_assetBundle.LoadAsset<GameObject>("chest_red");
            GameObject Chest_Black = m_assetBundle.LoadAsset<GameObject>("Chest_Black");
            GameObject Chest_Rainbow = m_assetBundle.LoadAsset<GameObject>("Chest_Rainbow");
            // GameObject Chest_Rat = m_assetBundle.LoadAsset<GameObject>("Chest_Rat");

            m_assetBundle = null;
            m_assetBundle2 = null;
            m_resourceBundle = null;

            DungeonPlaceableVariant BlueChestVariant = new DungeonPlaceableVariant();
            BlueChestVariant.percentChance = 0.35f;
            BlueChestVariant.unitOffset = new Vector2(1, 0.8f);
            BlueChestVariant.enemyPlaceableGuid = string.Empty;
            BlueChestVariant.pickupObjectPlaceableId = -1;
            BlueChestVariant.forceBlackPhantom = false;
            BlueChestVariant.addDebrisObject = false;
            BlueChestVariant.prerequisites = null;
            BlueChestVariant.materialRequirements = null;
            BlueChestVariant.nonDatabasePlaceable = Chest_Silver;

            DungeonPlaceableVariant BrownChestVariant = new DungeonPlaceableVariant();
            BrownChestVariant.percentChance = 0.28f;
            BrownChestVariant.unitOffset = new Vector2(1, 0.8f);
            BrownChestVariant.enemyPlaceableGuid = string.Empty;
            BrownChestVariant.pickupObjectPlaceableId = -1;
            BrownChestVariant.forceBlackPhantom = false;
            BrownChestVariant.addDebrisObject = false;
            BrownChestVariant.prerequisites = null;
            BrownChestVariant.materialRequirements = null;
            BrownChestVariant.nonDatabasePlaceable = ChestBrownTwoItems;

            DungeonPlaceableVariant GreenChestVariant = new DungeonPlaceableVariant();
            GreenChestVariant.percentChance = 0.25f;
            GreenChestVariant.unitOffset = new Vector2(1, 0.8f);
            GreenChestVariant.enemyPlaceableGuid = string.Empty;
            GreenChestVariant.pickupObjectPlaceableId = -1;
            GreenChestVariant.forceBlackPhantom = false;
            GreenChestVariant.addDebrisObject = false;
            GreenChestVariant.prerequisites = null;
            GreenChestVariant.materialRequirements = null;
            GreenChestVariant.nonDatabasePlaceable = Chest_Green;

            DungeonPlaceableVariant SynergyChestVariant = new DungeonPlaceableVariant();
            SynergyChestVariant.percentChance = 0.2f;
            SynergyChestVariant.unitOffset = new Vector2(1, 0.8f);
            SynergyChestVariant.enemyPlaceableGuid = string.Empty;
            SynergyChestVariant.pickupObjectPlaceableId = -1;
            SynergyChestVariant.forceBlackPhantom = false;
            SynergyChestVariant.addDebrisObject = false;
            SynergyChestVariant.prerequisites = null;
            SynergyChestVariant.materialRequirements = null;
            SynergyChestVariant.nonDatabasePlaceable = Chest_Synergy;

            DungeonPlaceableVariant RedChestVariant = new DungeonPlaceableVariant();
            RedChestVariant.percentChance = 0.15f;
            RedChestVariant.unitOffset = new Vector2(0.5f, 0.5f);
            RedChestVariant.enemyPlaceableGuid = string.Empty;
            RedChestVariant.pickupObjectPlaceableId = -1;
            RedChestVariant.forceBlackPhantom = false;
            RedChestVariant.addDebrisObject = false;
            RedChestVariant.prerequisites = null;
            RedChestVariant.materialRequirements = null;
            RedChestVariant.nonDatabasePlaceable = Chest_Red;

            DungeonPlaceableVariant BlackChestVariant = new DungeonPlaceableVariant();
            BlackChestVariant.percentChance = 0.1f;
            BlackChestVariant.unitOffset = new Vector2(0.5f, 0.5f);
            BlackChestVariant.enemyPlaceableGuid = string.Empty;
            BlackChestVariant.pickupObjectPlaceableId = -1;
            BlackChestVariant.forceBlackPhantom = false;
            BlackChestVariant.addDebrisObject = false;
            BlackChestVariant.prerequisites = null;
            BlackChestVariant.materialRequirements = null;
            BlackChestVariant.nonDatabasePlaceable = Chest_Black;

            DungeonPlaceableVariant RainbowChestVariant = new DungeonPlaceableVariant();
            RainbowChestVariant.percentChance = 0.005f;
            RainbowChestVariant.unitOffset = new Vector2(0.5f, 0.5f);
            RainbowChestVariant.enemyPlaceableGuid = string.Empty;
            RainbowChestVariant.pickupObjectPlaceableId = -1;
            RainbowChestVariant.forceBlackPhantom = false;
            RainbowChestVariant.addDebrisObject = false;
            RainbowChestVariant.prerequisites = null;
            RainbowChestVariant.materialRequirements = null;
            RainbowChestVariant.nonDatabasePlaceable = Chest_Rainbow;

            DungeonPlaceableVariant ItemVariant = new DungeonPlaceableVariant();
            ItemVariant.percentChance = spawnChance;
            if (CustomOffset.HasValue) {
                ItemVariant.unitOffset = CustomOffset.Value;
            } else {
                ItemVariant.unitOffset = Vector2.zero;
            }
            // ItemVariant.unitOffset = new Vector2(0.5f, 0.8f);
            ItemVariant.enemyPlaceableGuid = string.Empty;
            ItemVariant.pickupObjectPlaceableId = itemID;
            ItemVariant.forceBlackPhantom = false;
            if (itemHasDebrisObject) {
                ItemVariant.addDebrisObject = true;
            } else {
                ItemVariant.addDebrisObject = false;
            }            
            RainbowChestVariant.prerequisites = null;
            RainbowChestVariant.materialRequirements = null;

            List<DungeonPlaceableVariant> ChestTiers = new List<DungeonPlaceableVariant>();
            ChestTiers.Add(BrownChestVariant);
            ChestTiers.Add(BlueChestVariant);
            ChestTiers.Add(GreenChestVariant);
            ChestTiers.Add(SynergyChestVariant);
            ChestTiers.Add(RedChestVariant);
            ChestTiers.Add(BlackChestVariant);
            ChestTiers.Add(RainbowChestVariant);

            DungeonPlaceableVariant EnemyVariant = new DungeonPlaceableVariant();
            EnemyVariant.percentChance = spawnChance;
            EnemyVariant.unitOffset = Vector2.zero;
            EnemyVariant.enemyPlaceableGuid = EnemyGUID;
            EnemyVariant.pickupObjectPlaceableId = -1;
            EnemyVariant.forceBlackPhantom = false;
            EnemyVariant.addDebrisObject = false;
            EnemyVariant.prerequisites = null;
            EnemyVariant.materialRequirements = null;

            List<DungeonPlaceableVariant> EnemyTiers = new List<DungeonPlaceableVariant>();
            EnemyTiers.Add(EnemyVariant);

            List<DungeonPlaceableVariant> ItemTiers = new List<DungeonPlaceableVariant>();
            ItemTiers.Add(ItemVariant);

            DungeonPlaceable m_cachedCustomPlacable = ScriptableObject.CreateInstance<DungeonPlaceable>();
            m_cachedCustomPlacable.name = "CustomChestPlacable";
            if (spawnsEnemy | useExternalPrefab) {
                m_cachedCustomPlacable.width = 2;
                m_cachedCustomPlacable.height = 2;
            } else if (spawnsItem) {
                m_cachedCustomPlacable.width = 1;
                m_cachedCustomPlacable.height = 1;
            } else {
                m_cachedCustomPlacable.width = 4;
                m_cachedCustomPlacable.height = 1;
            }
            m_cachedCustomPlacable.roomSequential = false;
            m_cachedCustomPlacable.respectsEncounterableDifferentiator = true;
            m_cachedCustomPlacable.UsePrefabTransformOffset = false;
            m_cachedCustomPlacable.isPassable = true;
            if (spawnsItem) {
                m_cachedCustomPlacable.MarkSpawnedItemsAsRatIgnored = true;
            } else {
                m_cachedCustomPlacable.MarkSpawnedItemsAsRatIgnored = false;
            }
            
            m_cachedCustomPlacable.DebugThisPlaceable = false;
            if (useExternalPrefab && ObjectPrefab != null) {
                DungeonPlaceableVariant ExternalObjectVariant = new DungeonPlaceableVariant();
                ExternalObjectVariant.percentChance = spawnChance;
                if (CustomOffset.HasValue) {
                    ExternalObjectVariant.unitOffset = CustomOffset.Value;
                } else {
                    ExternalObjectVariant.unitOffset = Vector2.zero;
                }
                ExternalObjectVariant.enemyPlaceableGuid = string.Empty;
                ExternalObjectVariant.pickupObjectPlaceableId = -1;
                ExternalObjectVariant.forceBlackPhantom = false;
                ExternalObjectVariant.addDebrisObject = false;
                ExternalObjectVariant.nonDatabasePlaceable = ObjectPrefab;
                List<DungeonPlaceableVariant> ExternalObjectTiers = new List<DungeonPlaceableVariant>();
                ExternalObjectTiers.Add(ExternalObjectVariant);
                m_cachedCustomPlacable.variantTiers = ExternalObjectTiers;
            } else if (spawnsEnemy) {
                m_cachedCustomPlacable.variantTiers = EnemyTiers;
            } else if (spawnsItem) {
                m_cachedCustomPlacable.variantTiers = ItemTiers;
            } else {
                m_cachedCustomPlacable.variantTiers = ChestTiers;
            }
            return m_cachedCustomPlacable;
        }
        public static RoomHandler AddCustomRuntimeRoomWithTileSet(Dungeon dungeon2, PrototypeDungeonRoom prototype, bool addRoomToMinimap = true, bool addTeleporter = true, bool isSecretRatExitRoom = false, Action<RoomHandler> postProcessCellData = null, DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.STANDARD, bool allowProceduralDecoration = true, bool allowProceduralLightFixtures = true, bool RoomExploredOnMinimap = true, string RunTimeTileMapName = "Glitch")
        {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            tk2dTileMap m_tilemap = dungeon.MainTilemap;

            if (m_tilemap == null)
            {
                ETGModConsole.Log("ERROR: TileMap object is null! Something seriously went wrong!");
                Debug.Log("ERROR: TileMap object is null! Something seriously went wrong!");
                return null;
            }

            ExpandTK2DDungeonAssembler assembler = new ExpandTK2DDungeonAssembler();
            assembler.Initialize(dungeon2.tileIndices);

            IntVector2 basePosition = IntVector2.Zero;
            IntVector2 basePosition2 = new IntVector2(50, 50);
            int num = basePosition2.x;
            int num2 = basePosition2.y;
            IntVector2 intVector = new IntVector2(int.MaxValue, int.MaxValue);
            IntVector2 intVector2 = new IntVector2(int.MinValue, int.MinValue);
            intVector = IntVector2.Min(intVector, basePosition);
            intVector2 = IntVector2.Max(intVector2, basePosition + new IntVector2(prototype.Width, prototype.Height));
            IntVector2 a = intVector2 - intVector;
            IntVector2 b = IntVector2.Min(IntVector2.Zero, -1 * intVector);
            a += b;
            IntVector2 intVector3 = new IntVector2(dungeon.data.Width + num, num);
            int newWidth = dungeon.data.Width + num * 2 + a.x;
            int newHeight = Mathf.Max(dungeon.data.Height, a.y + num * 2);
            CellData[][] array = BraveUtility.MultidimensionalArrayResize(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
            dungeon.data.cellData = array;
            dungeon.data.ClearCachedCellData();
            IntVector2 d = new IntVector2(prototype.Width, prototype.Height);
            IntVector2 b2 = basePosition + b;
            IntVector2 intVector4 = intVector3 + b2;
            CellArea cellArea = new CellArea(intVector4, d, 0);
            cellArea.prototypeRoom = prototype;
            RoomHandler targetRoom = new RoomHandler(cellArea);
            for (int k = -num; k < d.x + num; k++)
            {
                for (int l = -num; l < d.y + num; l++)
                {
                    IntVector2 p = new IntVector2(k, l) + intVector4;
                    if ((k >= 0 && l >= 0 && k < d.x && l < d.y) || array[p.x][p.y] == null)
                    {
                        CellData cellData = new CellData(p, CellType.WALL);
                        cellData.positionInTilemap = cellData.positionInTilemap - intVector3 + new IntVector2(num2, num2);
                        cellData.parentArea = cellArea;
                        cellData.parentRoom = targetRoom;
                        cellData.nearestRoom = targetRoom;
                        cellData.distanceFromNearestRoom = 0f;
                        array[p.x][p.y] = cellData;
                    }
                }
            }
            dungeon.data.rooms.Add(targetRoom);
            try
            {
                targetRoom.WriteRoomData(dungeon.data);
            }
            catch (Exception)
            {
                ETGModConsole.Log("WARNING: Exception caused during WriteRoomData step on room: " + targetRoom.GetRoomName());
            }
            try
            {
                GenerateLightsForRoomFromOtherTileset(dungeon2.decoSettings, targetRoom, GameObject.Find("_Lights").transform, dungeon, dungeon2, lightStyle);
            }
            catch (Exception ex)
            {
                ETGModConsole.Log("WARNING: Exception caused during GenerateLightsForRoom step on room: " + targetRoom.GetRoomName());
                ETGModConsole.Log("WARNING: Trying fall back code..." + targetRoom.GetRoomName());
                Debug.LogException(ex);
                try
                {
                    dungeon.data.GenerateLightsForRoom(dungeon.decoSettings, targetRoom, GameObject.Find("_Lights").transform, lightStyle);
                }
                catch (Exception ex2)
                {
                    ETGModConsole.Log("WARNING: Exception caused during GenerateLightsForRoom step on room while attempting fall back code: " + targetRoom.GetRoomName());
                    Debug.LogException(ex2);
                }
            }
            postProcessCellData?.Invoke(targetRoom);

            if (targetRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET) { targetRoom.BuildSecretRoomCover(); }

           


            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
            tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
            string str = UnityEngine.Random.Range(10000, 99999).ToString();
            gameObject.name = RunTimeTileMapName + "_RuntimeTilemap_" + str;
            component.renderData.name = "Glitch_" + "RuntimeTilemap_" + str + " Render Data";
            component.Editor__SpriteCollection = dungeon2.tileIndices.dungeonCollection;
            try
            {
                ExpandTK2DDungeonAssembler.RuntimeResizeTileMap(component, a.x + num2 * 2, a.y + num2 * 2, m_tilemap.partitionSizeX, m_tilemap.partitionSizeY);
                IntVector2 intVector5 = new IntVector2(prototype.Width, prototype.Height);
                IntVector2 b3 = basePosition + b;
                IntVector2 intVector6 = intVector3 + b3;
                for (int num4 = -num2; num4 < intVector5.x + num2; num4++)
                {
                    for (int num5 = -num2; num5 < intVector5.y + num2 + 2; num5++)
                    {
                        assembler.BuildTileIndicesForCell(dungeon, dungeon2, component, intVector6.x + num4, intVector6.y + num5);
                    }
                }
                RenderMeshBuilder.CurrentCellXOffset = intVector3.x - num2;
                RenderMeshBuilder.CurrentCellYOffset = intVector3.y - num2;
                component.ForceBuild();
                RenderMeshBuilder.CurrentCellXOffset = 0;
                RenderMeshBuilder.CurrentCellYOffset = 0;
                component.renderData.transform.position = new Vector3(intVector3.x - num2, intVector3.y - num2, intVector3.y - num2);
            }
            catch (Exception ex)
            {
                ETGModConsole.Log("WARNING: Exception occured during RuntimeResizeTileMap / RenderMeshBuilder steps!");
                Debug.Log("WARNING: Exception occured during RuntimeResizeTileMap/RenderMeshBuilder steps!");
                Debug.LogException(ex);
                return null; // Return null to prevent lead key from using this room. In most cases the resulting room is not usable as walls did not generate and there is no collision.
            }
            targetRoom.OverrideTilemap = component;
            if (allowProceduralLightFixtures)
            {
                for (int num7 = 0; num7 < targetRoom.area.dimensions.x; num7++)
                {
                    for (int num8 = 0; num8 < targetRoom.area.dimensions.y + 2; num8++)
                    {
                        IntVector2 intVector7 = targetRoom.area.basePosition + new IntVector2(num7, num8);
                        if (dungeon.data.CheckInBoundsAndValid(intVector7))
                        {
                            CellData currentCell = dungeon.data[intVector7];
                            ExpandTK2DInteriorDecorator.PlaceLightDecorationForCell(dungeon, component, currentCell, intVector7);
                        }
                    }
                }
            }

            Pathfinder.Instance.InitializeRegion(dungeon.data, targetRoom.area.basePosition + new IntVector2(-3, -3), targetRoom.area.dimensions + new IntVector2(3, 3));

            if (prototype.usesProceduralDecoration && prototype.allowFloorDecoration && allowProceduralDecoration)
            {
                ExpandTK2DInteriorDecorator decorator = new ExpandTK2DInteriorDecorator(assembler);
                    decorator.HandleRoomDecoration(targetRoom, dungeon, dungeon2, m_tilemap);

                
            }

            targetRoom.PostGenerationCleanup();

            GameObject m_CollisionObject = new GameObject(component.renderData.name + "_CollisionObject");
            IntVector2 targetRoomPosition = targetRoom.area.basePosition;
            m_CollisionObject.transform.parent = targetRoom.hierarchyParent;
            IntVector2 targetRoomSize = targetRoom.area.dimensions;
            m_CollisionObject.transform.position = targetRoomPosition.ToVector3();

            for (int width = -2; width < targetRoomSize.x + 2; width++)
            {
                for (int height = -2; height < targetRoomSize.y + 2; height++)
                {
                    int X = targetRoomPosition.x + width;
                    int Y = targetRoomPosition.y + height;
                    if (dungeon.data.isWall(X, Y))
                    {
                        IntVector2 positionOffset = new IntVector2(width, height);
                        if (dungeon.data.isFaceWallLower(X, Y))
                        {
                            GenerateOrAddToRigidBody(m_CollisionObject, CollisionLayer.LowObstacle, PixelCollider.PixelColliderGeneration.Manual, dimensions: IntVector2.One, offset: positionOffset);
                        }
                        else
                        {
                            GenerateOrAddToRigidBody(m_CollisionObject, CollisionLayer.HighObstacle, PixelCollider.PixelColliderGeneration.Manual, dimensions: IntVector2.One, offset: positionOffset);
                        }
                    }
                }
            }

            if (!m_CollisionObject.GetComponent<SpeculativeRigidbody>()) { UnityEngine.Object.Destroy(m_CollisionObject); }

            

            HandleSpecificRoomAGDInjection(targetRoom, dungeon, dungeon2.tileIndices.tilesetId);

            if (addRoomToMinimap)
            {
                if (RoomExploredOnMinimap)
                {
                    targetRoom.visibility = RoomHandler.VisibilityStatus.VISITED;
                }
                else
                {
                    targetRoom.visibility = RoomHandler.VisibilityStatus.OBSCURED;
                }
                GameManager.Instance.StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(targetRoom, true, true, false));
                if (isSecretRatExitRoom && RoomExploredOnMinimap) { targetRoom.visibility = RoomHandler.VisibilityStatus.OBSCURED; }
            }
            if (addTeleporter) { targetRoom.AddProceduralTeleporterToRoom(); }
            if (addRoomToMinimap) { Minimap.Instance.InitializeMinimap(dungeon.data); }
            DeadlyDeadlyGoopManager.ReinitializeData();

            return targetRoom;
        }

        public static SpeculativeRigidbody GenerateOrAddToRigidBody(GameObject targetObject, CollisionLayer collisionLayer, PixelCollider.PixelColliderGeneration colliderGenerationMode = PixelCollider.PixelColliderGeneration.Tk2dPolygon, bool collideWithTileMap = false, bool CollideWithOthers = true, bool CanBeCarried = true, bool CanBePushed = false, bool RecheckTriggers = false, bool IsTrigger = false, bool replaceExistingColliders = false, bool UsesPixelsAsUnitSize = false, IntVector2? dimensions = null, IntVector2? offset = null)
        {
            SpeculativeRigidbody m_CachedRigidBody = GameObjectExtensions.GetOrAddComponent<SpeculativeRigidbody>(targetObject);
            m_CachedRigidBody.CollideWithOthers = CollideWithOthers;
            m_CachedRigidBody.CollideWithTileMap = collideWithTileMap;
            m_CachedRigidBody.Velocity = Vector2.zero;
            m_CachedRigidBody.MaxVelocity = Vector2.zero;
            m_CachedRigidBody.ForceAlwaysUpdate = false;
            m_CachedRigidBody.CanPush = false;
            m_CachedRigidBody.CanBePushed = CanBePushed;
            m_CachedRigidBody.PushSpeedModifier = 1f;
            m_CachedRigidBody.CanCarry = false;
            m_CachedRigidBody.CanBeCarried = CanBeCarried;
            m_CachedRigidBody.PreventPiercing = false;
            m_CachedRigidBody.SkipEmptyColliders = false;
            m_CachedRigidBody.RecheckTriggers = RecheckTriggers;
            m_CachedRigidBody.UpdateCollidersOnRotation = false;
            m_CachedRigidBody.UpdateCollidersOnScale = false;

            IntVector2 Offset = IntVector2.Zero;
            IntVector2 Dimensions = IntVector2.Zero;
            if (colliderGenerationMode != PixelCollider.PixelColliderGeneration.Tk2dPolygon)
            {
                if (dimensions.HasValue)
                {
                    Dimensions = dimensions.Value;
                    if (!UsesPixelsAsUnitSize)
                    {
                        Dimensions = (new IntVector2(Dimensions.x * 16, Dimensions.y * 16));
                    }
                }
                if (offset.HasValue)
                {
                    Offset = offset.Value;
                    if (!UsesPixelsAsUnitSize)
                    {
                        Offset = (new IntVector2(Offset.x * 16, Offset.y * 16));
                    }
                }
            }
            PixelCollider m_CachedCollider = new PixelCollider()
            {
                ColliderGenerationMode = colliderGenerationMode,
                CollisionLayer = collisionLayer,
                IsTrigger = IsTrigger,
                BagleUseFirstFrameOnly = (colliderGenerationMode == PixelCollider.PixelColliderGeneration.Tk2dPolygon),
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = Offset.x,
                ManualOffsetY = Offset.y,
                ManualWidth = Dimensions.x,
                ManualHeight = Dimensions.y,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0
            };

            if (replaceExistingColliders | m_CachedRigidBody.PixelColliders == null)
            {
                m_CachedRigidBody.PixelColliders = new List<PixelCollider> { m_CachedCollider };
            }
            else
            {
                m_CachedRigidBody.PixelColliders.Add(m_CachedCollider);
            }

            if (m_CachedRigidBody.sprite && colliderGenerationMode == PixelCollider.PixelColliderGeneration.Tk2dPolygon)
            {
                Bounds bounds = m_CachedRigidBody.sprite.GetBounds();
                m_CachedRigidBody.sprite.GetTrueCurrentSpriteDef().colliderVertices = new Vector3[] { bounds.center - bounds.extents, bounds.center + bounds.extents };
                // m_CachedRigidBody.ForceRegenerate();
                // m_CachedRigidBody.RegenerateCache();
            }

            return m_CachedRigidBody;
        }

        private static HashSet<IntVector2> GetCeilingTileSet(IntVector2 pos1, IntVector2 pos2, DungeonData.Direction facingDirection)
        {
            IntVector2 intVector;
            IntVector2 intVector2;
            if (facingDirection == DungeonData.Direction.NORTH)
            {
                intVector = pos1 + new IntVector2(-1, 0);
                intVector2 = pos2 + new IntVector2(1, 1);
            }
            else if (facingDirection == DungeonData.Direction.SOUTH)
            {
                intVector = pos1 + new IntVector2(-1, 2);
                intVector2 = pos2 + new IntVector2(1, 3);
            }
            else if (facingDirection == DungeonData.Direction.EAST)
            {
                intVector = pos1 + new IntVector2(-1, 0);
                intVector2 = pos2 + new IntVector2(0, 3);
            }
            else
            {
                if (facingDirection != DungeonData.Direction.WEST) { return null; }
                intVector = pos1 + new IntVector2(0, 0);
                intVector2 = pos2 + new IntVector2(1, 3);
            }
            HashSet<IntVector2> hashSet = new HashSet<IntVector2>();
            for (int i = intVector.x; i <= intVector2.x; i++)
            {
                for (int j = intVector.y; j <= intVector2.y; j++)
                {
                    IntVector2 item = new IntVector2(i, j);
                    hashSet.Add(item);
                }
            }
            return hashSet;
        }


        private static void HandleSpecificRoomAGDInjection(RoomHandler TargetRoom, Dungeon dungeon, GlobalDungeonData.ValidTilesets tilesetID, RunData runData = null, List<AGDEnemyReplacementTier> TierList = null)
        {
            List<AIActor> EnemyList1 = new List<AIActor>();
            // RunData runData = GameManager.Instance.RunData;
            if (runData == null) { runData = new RunData(); }
            if (TierList == null) { TierList = GameManager.Instance.EnemyReplacementTiers; }
            if (runData.AgdInjectionRunCounts == null || runData.AgdInjectionRunCounts.Length != TierList.Count)
            {
                runData.AgdInjectionRunCounts = new int[TierList.Count];
            }
            int[] agdInjectionRunCounts = runData.AgdInjectionRunCounts;
            for (int i = 0; i < TierList.Count; i++)
            {
                AGDEnemyReplacementTier agdenemyReplacementTier = TierList[i];
                int num = 0;
                if (agdenemyReplacementTier != null && agdenemyReplacementTier.TargetTileset == tilesetID && !agdenemyReplacementTier.ExcludeForPrereqs())
                {
                    if (agdenemyReplacementTier.MaxPerRun <= 0 || agdInjectionRunCounts[i] < agdenemyReplacementTier.MaxPerRun)
                    {
                        if (TargetRoom.EverHadEnemies && TargetRoom.IsStandardRoom && !agdenemyReplacementTier.ExcludeRoomForColumns(dungeon.data, TargetRoom) && !agdenemyReplacementTier.ExcludeRoom(TargetRoom))
                        {
                            TargetRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref EnemyList1);
                            if (!agdenemyReplacementTier.ExcludeRoomForEnemies(TargetRoom, EnemyList1))
                            {
                                for (int I = 0; I < EnemyList1.Count; I++)
                                {
                                    AIActor enemy = EnemyList1[I];
                                    if (enemy && (enemy.AdditionalSimpleItemDrops == null || enemy.AdditionalSimpleItemDrops.Count <= 0) && (!enemy.healthHaver || !enemy.healthHaver.IsBoss))
                                    {
                                        if (((agdenemyReplacementTier.TargetAllSignatureEnemies && enemy.IsSignatureEnemy) || (agdenemyReplacementTier.TargetAllNonSignatureEnemies && !enemy.IsSignatureEnemy) || (agdenemyReplacementTier.TargetGuids != null && agdenemyReplacementTier.TargetGuids.Contains(enemy.EnemyGuid))) && UnityEngine.Random.value < agdenemyReplacementTier.ChanceToReplace)
                                        {
                                            Vector2? vector = null;
                                            if (agdenemyReplacementTier.RemoveAllOtherEnemies)
                                            {
                                                vector = new Vector2?(TargetRoom.area.Center);
                                                for (int J = EnemyList1.Count - 1; J >= 0; J--)
                                                {
                                                    AIActor aiactor2 = EnemyList1[I];
                                                    if (aiactor2)
                                                    {
                                                        TargetRoom.DeregisterEnemy(aiactor2, true);
                                                        UnityEngine.Object.Destroy(aiactor2.gameObject);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (enemy.specRigidbody)
                                                {
                                                    enemy.specRigidbody.Initialize();
                                                    vector = new Vector2?(enemy.specRigidbody.UnitBottomLeft);
                                                }
                                                TargetRoom.DeregisterEnemy(enemy, true);
                                                UnityEngine.Object.Destroy(enemy.gameObject);
                                            }
                                            RoomHandler roomHandler2 = TargetRoom;
                                            string enemyGuid = BraveUtility.RandomElement(agdenemyReplacementTier.ReplacementGuids);
                                            Vector2? goalPosition = vector;
                                            roomHandler2.AddSpecificEnemyToRoomProcedurally(enemyGuid, false, goalPosition);
                                            num++;
                                            agdInjectionRunCounts[i]++;
                                            if ((agdenemyReplacementTier.MaxPerFloor > 0 && num >= agdenemyReplacementTier.MaxPerFloor) || (agdenemyReplacementTier.MaxPerRun > 0 && agdInjectionRunCounts[i] >= agdenemyReplacementTier.MaxPerRun) || agdenemyReplacementTier.RemoveAllOtherEnemies)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                if ((agdenemyReplacementTier.MaxPerFloor > 0 && num >= agdenemyReplacementTier.MaxPerFloor) || (agdenemyReplacementTier.MaxPerRun > 0 && agdInjectionRunCounts[i] >= agdenemyReplacementTier.MaxPerRun))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            List<AIActor> EnemyList2 = new List<AIActor>();
            float newPlayerEnemyCullFactor = GameStatsManager.Instance.NewPlayerEnemyCullFactor;
            if (newPlayerEnemyCullFactor > 0f && TargetRoom.EverHadEnemies && TargetRoom.IsStandardRoom && !TargetRoom.IsGunslingKingChallengeRoom)
            {
                if (TargetRoom.area.runtimePrototypeData != null && TargetRoom.area.runtimePrototypeData.roomEvents != null)
                {
                    bool DarkRoom = false;
                    for (int i = 0; i < TargetRoom.area.runtimePrototypeData.roomEvents.Count; i++)
                    {
                        if (TargetRoom.area.runtimePrototypeData.roomEvents[i].action == RoomEventTriggerAction.BECOME_TERRIFYING_AND_DARK)
                        {
                            DarkRoom = true;
                        }
                    }
                    if (DarkRoom) { return; }
                }
                TargetRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref EnemyList2);
                for (int j = 0; j < EnemyList2.Count; j++)
                {
                    AIActor TargetEnemy = EnemyList2[j];
                    if (TargetEnemy && (TargetEnemy.AdditionalSimpleItemDrops == null || TargetEnemy.AdditionalSimpleItemDrops.Count <= 0))
                    {
                        if (TargetEnemy.IsNormalEnemy && !TargetEnemy.IsHarmlessEnemy && TargetEnemy.IsWorthShootingAt && (!TargetEnemy.healthHaver || !TargetEnemy.healthHaver.IsBoss) && UnityEngine.Random.value < newPlayerEnemyCullFactor)
                        {
                            UnityEngine.Object.Destroy(TargetEnemy.gameObject);
                        }
                    }
                }
            }
        }

        public static void GenerateLightsForRoomFromOtherTileset(TilemapDecoSettings decoSettings, RoomHandler rh, Transform lightParent, Dungeon dungeon, Dungeon dungeon2, DungeonData.LightGenerationStyle style = DungeonData.LightGenerationStyle.STANDARD)
        {
            if (!dungeon2.roomMaterialDefinitions[rh.RoomVisualSubtype].useLighting) { return; }

            bool flag = decoSettings.lightCookies.Length > 0;
            List<Tuple<IntVector2, float>> list = new List<Tuple<IntVector2, float>>();
            bool flag2 = false;
            List<IntVector2> list2;
            int count;
            if (rh.area != null && !rh.area.IsProceduralRoom && !rh.area.prototypeRoom.usesProceduralLighting)
            {
                list2 = rh.GatherManualLightPositions();
                count = list2.Count;
            }
            else
            {
                flag2 = true;
                list2 = rh.GatherOptimalLightPositions(decoSettings);
                count = list2.Count;
                if (rh.area != null && rh.area.prototypeRoom != null) { PostprocessLightPositions(dungeon, list2, rh); }
            }
            if (rh.area.prototypeRoom != null)
            {
                for (int i = 0; i < rh.area.instanceUsedExits.Count; i++)
                {
                    RuntimeRoomExitData runtimeRoomExitData = rh.area.exitToLocalDataMap[rh.area.instanceUsedExits[i]];
                    RuntimeExitDefinition runtimeExitDefinition = rh.exitDefinitionsByExit[runtimeRoomExitData];
                    if (runtimeRoomExitData.TotalExitLength > 4 && !runtimeExitDefinition.containsLight)
                    {
                        IntVector2 first = (!runtimeRoomExitData.jointedExit) ? runtimeExitDefinition.GetLinearMidpoint(rh) : (runtimeRoomExitData.ExitOrigin - IntVector2.One);
                        list.Add(new Tuple<IntVector2, float>(first, 0.5f));
                        runtimeExitDefinition.containsLight = true;
                    }
                }
            }
            GlobalDungeonData.ValidTilesets tilesetId = dungeon2.tileIndices.tilesetId;
            float lightCullingPercentage = decoSettings.lightCullingPercentage;
            if (flag2 && lightCullingPercentage > 0f)
            {
                int num = Mathf.FloorToInt(list2.Count * lightCullingPercentage);
                int num2 = Mathf.FloorToInt(list.Count * lightCullingPercentage);
                if (num == 0 && num2 == 0 && list2.Count + list.Count > 4)
                {
                    num = 1;
                } while (num > 0 && list2.Count > 0)
                {
                    list2.RemoveAt(UnityEngine.Random.Range(0, list2.Count));
                    num--;
                } while (num2 > 0 && list.Count > 0)
                {
                    list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
                    num2--;
                }
            }
            int count2 = list2.Count;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && (tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON || tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) && (flag2 || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.CONNECTOR))
            {
                list2.AddRange(rh.GatherPitLighting(decoSettings, list2));
            }
            for (int j = 0; j < list2.Count + list.Count; j++)
            {
                IntVector2 a = IntVector2.NegOne;
                float num3 = 1f;
                bool flag3 = false;
                if (j < list2.Count && j >= count2)
                {
                    flag3 = true;
                    num3 = 0.6f;
                }
                if (j < list2.Count)
                {
                    a = rh.area.basePosition + list2[j];
                }
                else
                {
                    a = rh.area.basePosition + list[j - list2.Count].First;
                    num3 = list[j - list2.Count].Second;
                }
                bool flag4 = false;
                if (flag && flag2 && a == rh.GetCenterCell()) { flag4 = true; }
                IntVector2 intVector = a + IntVector2.Up;
                bool flag5 = j >= count;
                bool flag6 = false;
                Vector3 b = Vector3.zero;
                if (dungeon.data[a + IntVector2.Up].type == CellType.WALL)
                {
                    dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.NORTH;
                    b = Vector3.down;
                }
                else if (dungeon.data[a + IntVector2.Right].type == CellType.WALL)
                {
                    dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.EAST;
                }
                else if (dungeon.data[a + IntVector2.Left].type == CellType.WALL)
                {
                    dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.WEST;
                }
                else if (dungeon.data[a + IntVector2.Down].type == CellType.WALL)
                {
                    flag6 = true;
                    dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.SOUTH;
                }
                else
                {
                    dungeon.data[intVector].cellVisualData.lightDirection = (DungeonData.Direction)(-1);
                }
                int num4 = rh.RoomVisualSubtype;
                float num5 = 0f;
                if (rh.area.prototypeRoom != null)
                {
                    PrototypeDungeonRoomCellData prototypeDungeonRoomCellData = (j >= list2.Count) ? rh.area.prototypeRoom.ForceGetCellDataAtPoint(list[j - list2.Count].First.x, list[j - list2.Count].First.y) : rh.area.prototypeRoom.ForceGetCellDataAtPoint(list2[j].x, list2[j].y);
                    if (prototypeDungeonRoomCellData != null && prototypeDungeonRoomCellData.containsManuallyPlacedLight)
                    {
                        num4 = prototypeDungeonRoomCellData.lightStampIndex;
                        num5 = prototypeDungeonRoomCellData.lightPixelsOffsetY / 16f;
                    }
                }
                if (num4 < 0 || num4 >= dungeon2.roomMaterialDefinitions.Length) { num4 = 0; }
                DungeonMaterial dungeonMaterial = dungeon2.roomMaterialDefinitions[num4];
                int num6 = -1;
                GameObject original;
                if (style == DungeonData.LightGenerationStyle.FORCE_COLOR || style == DungeonData.LightGenerationStyle.RAT_HALLWAY)
                {
                    num6 = 0;
                    original = dungeonMaterial.lightPrefabs.elements[0].gameObject;
                }
                else
                {
                    original = dungeonMaterial.lightPrefabs.SelectByWeight(out num6, false);
                }
                if ((!dungeonMaterial.facewallLightStamps[num6].CanBeTopWallLight && flag6) || (!dungeonMaterial.facewallLightStamps[num6].CanBeCenterLight && flag5))
                {
                    if (num6 >= dungeonMaterial.facewallLightStamps.Count) { num6 = 0; }
                    num6 = dungeonMaterial.facewallLightStamps[num6].FallbackIndex;
                    original = dungeonMaterial.lightPrefabs.elements[num6].gameObject;
                }
                GameObject gameObject = UnityEngine.Object.Instantiate(original, intVector.ToVector3(0f), Quaternion.identity);
                gameObject.transform.parent = lightParent;
                gameObject.transform.position = intVector.ToCenterVector3(intVector.y + decoSettings.lightHeight) + new Vector3(0f, num5, 0f) + b;
                ShadowSystem componentInChildren = gameObject.GetComponentInChildren<ShadowSystem>();
                Light componentInChildren2 = gameObject.GetComponentInChildren<Light>();
                if (componentInChildren2 != null) { componentInChildren2.intensity *= num3; }
                if (style == DungeonData.LightGenerationStyle.FORCE_COLOR || style == DungeonData.LightGenerationStyle.RAT_HALLWAY)
                {
                    SceneLightManager component = gameObject.GetComponent<SceneLightManager>();
                    if (component)
                    {
                        Color[] validColors = new Color[] { component.validColors[0] };
                        component.validColors = validColors;
                    }
                }
                if (flag3 && componentInChildren != null)
                {
                    if (componentInChildren2)
                    {
                        componentInChildren2.range += (dungeon2.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON) ? 3 : 5;
                    }
                    componentInChildren.ignoreCustomFloorLight = true;
                }
                if (flag4 && flag && componentInChildren != null)
                {
                    componentInChildren.uLightCookie = decoSettings.GetRandomLightCookie();
                    componentInChildren.uLightCookieAngle = UnityEngine.Random.Range(0f, 6.28f);
                    componentInChildren2.intensity *= 1.5f;
                }
                if (dungeon.data[intVector].cellVisualData.lightDirection == DungeonData.Direction.NORTH)
                {
                    bool flag7 = true;
                    for (int k = -2; k < 3; k++)
                    {
                        if (dungeon.data[intVector + IntVector2.Right * k].type == CellType.FLOOR)
                        {
                            flag7 = false;
                            break;
                        }
                    }
                    if (flag7 && componentInChildren)
                    {
                        GameObject original2 = (GameObject)BraveResources.Load("Global VFX/Wall_Light_Cookie", ".prefab");
                        GameObject gameObject2 = UnityEngine.Object.Instantiate(original2);
                        Transform transform = gameObject2.transform;
                        transform.parent = gameObject.transform;
                        transform.localPosition = Vector3.zero;
                        componentInChildren.PersonalCookies.Add(gameObject2.GetComponent<Renderer>());
                    }
                }
                CellData cellData = dungeon.data[intVector + new IntVector2(0, Mathf.RoundToInt(num5))];
                if (dungeon2.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
                {
                    dungeon.data[cellData.position + IntVector2.Down].cellVisualData.containsObjectSpaceStamp = true;
                }
                BraveUtility.DrawDebugSquare(cellData.position.ToVector2(), Color.magenta, 1000f);
                cellData.cellVisualData.containsLight = true;
                cellData.cellVisualData.lightObject = gameObject;
                LightStampData facewallLightStampData = dungeonMaterial.facewallLightStamps[num6];
                LightStampData sidewallLightStampData = dungeonMaterial.sidewallLightStamps[num6];
                cellData.cellVisualData.facewallLightStampData = facewallLightStampData;
                cellData.cellVisualData.sidewallLightStampData = sidewallLightStampData;
            }
        }
        public static void PostprocessLightPositions(Dungeon dungeon, List<IntVector2> positions, RoomHandler room)
        {
            CheckCellNeedsAdditionalLight(positions, room, dungeon.data[room.GetCenterCell()]);
            for (int i = 0; i < room.Cells.Count; i++)
            {
                CellData currentCell = dungeon.data[room.Cells[i]];
                CheckCellNeedsAdditionalLight(positions, room, currentCell);
            }
        }

        public static bool CheckCellNeedsAdditionalLight(List<IntVector2> positions, RoomHandler room, CellData currentCell)
        {
            int num = (!room.area.IsProceduralRoom) ? 10 : 20;
            if (currentCell.isExitCell) { return false; }
            if (currentCell.type == CellType.WALL) { return false; }
            bool flag = true;
            for (int i = 0; i < positions.Count; i++)
            {
                int num2 = IntVector2.ManhattanDistance(positions[i] + room.area.basePosition, currentCell.position);
                if (num2 <= num)
                {
                    flag = false;
                    break;
                }
            }
            if (flag) { positions.Add(currentCell.position - room.area.basePosition); }
            return flag;
        }
    }



        public class RoomBuilder
        {

           
            public static void GenerateRoomLayoutFromTexture(PrototypeDungeonRoom room, Texture2D sourceTexture, PrototypeRoomPitEntry.PitBorderType PitBorderType = PrototypeRoomPitEntry.PitBorderType.FLAT, CoreDamageTypes DamageCellsType = CoreDamageTypes.None)
            {
                float DamageToPlayersPerTick = 0;
                float DamageToEnemiesPerTick = 0;
                float TickFrequency = 0;
                bool RespectsFlying = true;
                bool DamageCellsArePoison = false;

                if (DamageCellsType == CoreDamageTypes.Fire)
                {
                    DamageToPlayersPerTick = 0.5f;
                    TickFrequency = 1;
                }
                else if (DamageCellsType == CoreDamageTypes.Poison)
                {
                    DamageCellsArePoison = true;
                    DamageToPlayersPerTick = 0.5f;
                    TickFrequency = 1;
                }

                if (sourceTexture == null)
                {
                    ETGModConsole.Log("[ExpandTheGungeon] GenerateRoomFromImage: Error! Requested Texture Resource is Null!");
                    return;
                }

                Color WhitePixel = new Color32(255, 255, 255, 255); // Wall Cell
                Color PinkPixel = new Color32(255, 0, 255, 255); // Diagonal Wall Cell (North East)
                Color YellowPixel = new Color32(255, 255, 0, 255); // Diagonal Wall Cell (North West)
                Color HalfPinkPixel = new Color32(127, 0, 127, 255); // Diagonal Wall Cell (South East)
                Color HalfYellowPixel = new Color32(127, 127, 0, 255); // Diagonal Wall Cell (South West)

                Color BluePixel = new Color32(0, 0, 255, 255); // Floor Cell

                Color BlueHalfGreenPixel = new Color32(0, 127, 255, 255); // Floor Cell (Ice Override)
                Color HalfBluePixel = new Color32(0, 0, 127, 255); // Floor Cell (Water Override)
                Color HalfRedPixel = new Color32(0, 0, 127, 255); // Floor Cell (Carpet Override)
                Color GreenHalfRBPixel = new Color32(127, 255, 127, 255); // Floor Cell (Grass Override)
                Color HalfWhitePixel = new Color32(127, 127, 127, 255); // Floor Cell (Bone Override)
                Color OrangePixel = new Color32(255, 127, 0, 255); // Floor Cell (Flesh Override)
                Color RedHalfGBPixel = new Color32(255, 127, 127, 255); // Floor Cell (ThickGoop Override)

                Color GreenPixel = new Color32(0, 255, 0, 255); // Damage Floor Cell

                Color RedPixel = new Color32(255, 0, 0, 255); // Pit Cell

                int width = room.Width;
                int height = room.Height;
                int ArrayLength = (width * height);

                if (sourceTexture.GetPixels32().Length != ArrayLength)
                {
                    ETGModConsole.Log("[ExpandTheGungeon] GenerateRoomFromImage: Error! Image resolution doesn't match size of room!");
                    return;
                }

                room.FullCellData = new PrototypeDungeonRoomCellData[ArrayLength];
                List<Vector2> m_Pits = new List<Vector2>();

                for (int X = 0; X < width; X++)
                {
                    for (int Y = 0; Y < height; Y++)
                    {
                        int ArrayPosition = (Y * width + X);
                        Color? m_Pixel = sourceTexture.GetPixel(X, Y);
                        CellType cellType = CellType.FLOOR;
                        DiagonalWallType diagonalWallType = DiagonalWallType.NONE;
                        CellVisualData.CellFloorType OverrideFloorType = CellVisualData.CellFloorType.Stone;
                        bool isDamageCell = false;
                        bool cellDamagesPlayer = false;
                        if (m_Pixel.HasValue)
                        {
                            if (m_Pixel.Value == WhitePixel | m_Pixel.Value == PinkPixel |
                                m_Pixel.Value == YellowPixel | m_Pixel.Value == HalfPinkPixel |
                                m_Pixel.Value == HalfYellowPixel)
                            {
                                cellType = CellType.WALL;
                                if (m_Pixel.Value == PinkPixel)
                                {
                                    diagonalWallType = DiagonalWallType.NORTHEAST;
                                }
                                else if (m_Pixel.Value == YellowPixel)
                                {
                                    diagonalWallType = DiagonalWallType.NORTHWEST;
                                }
                                else if (m_Pixel.Value == HalfPinkPixel)
                                {
                                    diagonalWallType = DiagonalWallType.SOUTHEAST;
                                }
                                else if (m_Pixel.Value == HalfYellowPixel)
                                {
                                    diagonalWallType = DiagonalWallType.SOUTHWEST;
                                }
                            }
                            else if (m_Pixel.Value == RedPixel)
                            {
                                cellType = CellType.PIT;
                                m_Pits.Add(new Vector2(X, Y));
                            }
                            else if (m_Pixel.Value == BluePixel | m_Pixel.Value == GreenPixel |
                              m_Pixel.Value == BlueHalfGreenPixel | m_Pixel.Value == HalfBluePixel |
                              m_Pixel.Value == HalfRedPixel | m_Pixel.Value == GreenHalfRBPixel |
                              m_Pixel.Value == HalfWhitePixel | m_Pixel.Value == OrangePixel |
                              m_Pixel.Value == RedHalfGBPixel)
                            {
                                cellType = CellType.FLOOR;
                                if (m_Pixel.Value == GreenPixel)
                                {
                                    isDamageCell = true;
                                    if (DamageCellsType == CoreDamageTypes.Ice)
                                    {
                                        cellDamagesPlayer = false;
                                    }
                                    else
                                    {
                                        cellDamagesPlayer = true;
                                    }
                                }
                                else if (m_Pixel.Value == BlueHalfGreenPixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.Ice;
                                }
                                else if (m_Pixel.Value == HalfBluePixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.Water;
                                }
                                else if (m_Pixel.Value == HalfRedPixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.Carpet;
                                }
                                else if (m_Pixel.Value == GreenHalfRBPixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.Grass;
                                }
                                else if (m_Pixel.Value == HalfWhitePixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.Bone;
                                }
                                else if (m_Pixel.Value == OrangePixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.Flesh;
                                }
                                else if (m_Pixel.Value == RedHalfGBPixel)
                                {
                                    OverrideFloorType = CellVisualData.CellFloorType.ThickGoop;
                                }
                            }
                            else
                            {
                                cellType = CellType.FLOOR;
                            }
                        }
                        else
                        {
                            cellType = CellType.FLOOR;
                        }
                        if (DamageCellsType != CoreDamageTypes.None && isDamageCell)
                        {
                            room.FullCellData[ArrayPosition] = GenerateCellData(cellType, diagonalWallType, cellDamagesPlayer, DamageCellsArePoison, DamageCellsType, DamageToPlayersPerTick, DamageToEnemiesPerTick, TickFrequency, RespectsFlying);
                        }
                        else
                        {
                            room.FullCellData[ArrayPosition] = GenerateCellData(cellType, diagonalWallType, OverrideFloorType: OverrideFloorType);
                        }
                    }
                }

                if (m_Pits.Count > 0)
                {
                    room.pits = new List<PrototypeRoomPitEntry>() {
                    new PrototypeRoomPitEntry(m_Pits) {
                        containedCells = m_Pits,
                        borderType = PitBorderType
                    }
                };
                }
                room.OnBeforeSerialize();
                room.OnAfterDeserialize();
                room.UpdatePrecalculatedData();
            }

            public static PrototypeDungeonRoom GenerateRoomPrefabFromTexture2D(Texture2D sourceTexture, PrototypeDungeonRoom.RoomCategory roomCategory = PrototypeDungeonRoom.RoomCategory.NORMAL, PrototypeRoomPitEntry.PitBorderType PitBorderType = PrototypeRoomPitEntry.PitBorderType.FLAT, CoreDamageTypes DamageCellsType = CoreDamageTypes.None)
            {
                PrototypeDungeonRoom m_NewRoomPrefab = ScriptableObject.CreateInstance<PrototypeDungeonRoom>();
                m_NewRoomPrefab.name = "Expand Corrupted Room";
                m_NewRoomPrefab.QAID = "FF" + UnityEngine.Random.Range(1000, 9999);
                m_NewRoomPrefab.GUID = System.Guid.NewGuid().ToString();
                m_NewRoomPrefab.PreventMirroring = false;
                m_NewRoomPrefab.category = roomCategory;
                m_NewRoomPrefab.subCategoryBoss = PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS;
                m_NewRoomPrefab.subCategoryNormal = PrototypeDungeonRoom.RoomNormalSubCategory.COMBAT;
                m_NewRoomPrefab.subCategorySpecial = PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP;
                m_NewRoomPrefab.subCategorySecret = PrototypeDungeonRoom.RoomSecretSubCategory.UNSPECIFIED_SECRET;
                m_NewRoomPrefab.exitData = new PrototypeRoomExitData() { exits = new List<PrototypeRoomExit>() };
                m_NewRoomPrefab.pits = new List<PrototypeRoomPitEntry>();
                m_NewRoomPrefab.placedObjects = new List<PrototypePlacedObjectData>();
                m_NewRoomPrefab.placedObjectPositions = new List<Vector2>();
                m_NewRoomPrefab.eventTriggerAreas = new List<PrototypeEventTriggerArea>();
                m_NewRoomPrefab.roomEvents = new List<RoomEventDefinition>() {
                new RoomEventDefinition(RoomEventTriggerCondition.ON_ENTER_WITH_ENEMIES, RoomEventTriggerAction.SEAL_ROOM),
                new RoomEventDefinition(RoomEventTriggerCondition.ON_ENEMIES_CLEARED, RoomEventTriggerAction.UNSEAL_ROOM),
            };
                m_NewRoomPrefab.overriddenTilesets = 0;
                m_NewRoomPrefab.prerequisites = new List<DungeonPrerequisite>();
                m_NewRoomPrefab.InvalidInCoop = false;
                m_NewRoomPrefab.cullProceduralDecorationOnWeakPlatforms = false;
                m_NewRoomPrefab.preventAddedDecoLayering = false;
                m_NewRoomPrefab.precludeAllTilemapDrawing = false;
                m_NewRoomPrefab.drawPrecludedCeilingTiles = false;
                m_NewRoomPrefab.preventBorders = false;
                m_NewRoomPrefab.preventFacewallAO = false;
                m_NewRoomPrefab.usesCustomAmbientLight = false;
                m_NewRoomPrefab.customAmbientLight = Color.white;
                m_NewRoomPrefab.ForceAllowDuplicates = false;
                m_NewRoomPrefab.injectionFlags = new RuntimeInjectionFlags() { CastleFireplace = false, ShopAnnexed = false };
                m_NewRoomPrefab.IsLostWoodsRoom = false;
                m_NewRoomPrefab.UseCustomMusic = false;
                m_NewRoomPrefab.UseCustomMusicState = false;
                m_NewRoomPrefab.CustomMusicEvent = string.Empty;
                m_NewRoomPrefab.UseCustomMusicSwitch = false;
                m_NewRoomPrefab.CustomMusicSwitch = string.Empty;
                m_NewRoomPrefab.overrideRoomVisualTypeForSecretRooms = false;
                m_NewRoomPrefab.rewardChestSpawnPosition = new IntVector2(6, 14);
                m_NewRoomPrefab.Width = sourceTexture.width;
                m_NewRoomPrefab.Height = sourceTexture.height;
                m_NewRoomPrefab.additionalObjectLayers = new List<PrototypeRoomObjectLayer>(0);
                GenerateRoomLayoutFromTexture(m_NewRoomPrefab, sourceTexture, PitBorderType, DamageCellsType);
                return m_NewRoomPrefab;
            }

            public static void GenerateBasicRoomLayout(PrototypeDungeonRoom room, CellType DefaultCellType = CellType.FLOOR, PrototypeRoomPitEntry.PitBorderType pitBorderType = PrototypeRoomPitEntry.PitBorderType.FLAT)
            {
                int width = room.Width;
                int height = room.Height;
                int ArrayLength = (width * height);

                room.FullCellData = new PrototypeDungeonRoomCellData[ArrayLength];
                List<Vector2> m_Pits = new List<Vector2>();

                for (int X = 0; X < width; X++)
                {
                    for (int Y = 0; Y < height; Y++)
                    {
                        int ArrayPosition = (Y * width + X);
                        room.FullCellData[ArrayPosition] = GenerateCellData(DefaultCellType);
                        if (DefaultCellType == CellType.PIT) { m_Pits.Add(new Vector2(X, Y)); }
                    }
                }

                if (m_Pits.Count > 0)
                {
                    room.pits = new List<PrototypeRoomPitEntry>() {
                    new PrototypeRoomPitEntry(m_Pits) {
                        containedCells = m_Pits,
                        borderType = pitBorderType
                    }
                };
                }

                room.OnBeforeSerialize();
                room.UpdatePrecalculatedData();
            }

            public static PrototypeDungeonRoomCellData GenerateCellData(CellType cellType, DiagonalWallType diagnalWallType = DiagonalWallType.NONE, bool DoesDamage = false, bool IsPoison = false, CoreDamageTypes DamageType = CoreDamageTypes.None, float DamageToPlayersPerTick = 0, float DamageToEnemiesPerTick = 0, float TickFrequency = 0, bool RespectsFlying = true, CellVisualData.CellFloorType OverrideFloorType = CellVisualData.CellFloorType.Stone)
            {
                PrototypeDungeonRoomCellData m_NewCellData = new PrototypeDungeonRoomCellData(string.Empty, cellType)
                {
                    state = cellType,
                    diagonalWallType = diagnalWallType,
                    breakable = false,
                    str = string.Empty,
                    conditionalOnParentExit = false,
                    conditionalCellIsPit = false,
                    parentExitIndex = -1,
                    containsManuallyPlacedLight = false,
                    lightPixelsOffsetY = 0,
                    lightStampIndex = 0,
                    doesDamage = DoesDamage,
                    damageDefinition = new CellDamageDefinition()
                    {
                        damageTypes = DamageType,
                        damageToPlayersPerTick = DamageToPlayersPerTick,
                        damageToEnemiesPerTick = DamageToEnemiesPerTick,
                        tickFrequency = TickFrequency,
                        respectsFlying = RespectsFlying,
                        isPoison = IsPoison
                    },
                    appearance = new PrototypeDungeonRoomCellAppearance()
                    {
                        overrideDungeonMaterialIndex = -1,
                        IsPhantomCarpet = false,
                        ForceDisallowGoop = false,
                        OverrideFloorType = OverrideFloorType,
                        globalOverrideIndices = new PrototypeIndexOverrideData() { indices = new List<int>(0) }
                    },
                    ForceTileNonDecorated = false,
                    additionalPlacedObjectIndices = new List<int>() { -1 },
                    placedObjectRUBELIndex = -1
                };

                if (DamageType == CoreDamageTypes.Poison)
                {
                    m_NewCellData.ForceTileNonDecorated = true;
                    m_NewCellData.appearance.OverrideFloorType = CellVisualData.CellFloorType.Stone;
                    m_NewCellData.damageDefinition.damageTypes = CoreDamageTypes.Poison;
                }
                else if (DamageType == CoreDamageTypes.Fire)
                {
                    m_NewCellData.ForceTileNonDecorated = true;
                    m_NewCellData.appearance.OverrideFloorType = CellVisualData.CellFloorType.Stone;
                    m_NewCellData.damageDefinition.damageTypes = CoreDamageTypes.Fire;
                }

                return m_NewCellData;
            }

            public static void AddExitToRoom(PrototypeDungeonRoom room, Vector2 ExitLocation, DungeonData.Direction ExitDirection, PrototypeRoomExit.ExitType ExitType = PrototypeRoomExit.ExitType.NO_RESTRICTION, PrototypeRoomExit.ExitGroup ExitGroup = PrototypeRoomExit.ExitGroup.A, bool ContainsDoor = true, int ExitLength = 3, int exitSize = 2, DungeonPlaceable overrideDoorObject = null)
            {
                if (room == null) { return; }
                if (room.exitData == null)
                {
                    room.exitData = new PrototypeRoomExitData();
                    room.exitData.exits = new List<PrototypeRoomExit>();
                }
                if (room.exitData.exits == null) { room.exitData.exits = new List<PrototypeRoomExit>(); }
                PrototypeRoomExit m_NewExit = new PrototypeRoomExit(ExitDirection, ExitLocation)
                {
                    exitDirection = ExitDirection,
                    exitType = ExitType,
                    exitGroup = ExitGroup,
                    containsDoor = ContainsDoor,
                    exitLength = ExitLength,
                    containedCells = new List<Vector2>(),
                };

                if (ExitDirection == DungeonData.Direction.WEST | ExitDirection == DungeonData.Direction.EAST)
                {
                    if (exitSize > 2)
                    {
                        m_NewExit.containedCells.Add(ExitLocation);
                        m_NewExit.containedCells.Add(ExitLocation + new Vector2(0, 1));
                        for (int i = 2; i < exitSize; i++)
                        {
                            m_NewExit.containedCells.Add(ExitLocation + new Vector2(0, i));
                        }
                    }
                    else
                    {
                        m_NewExit.containedCells.Add(ExitLocation);
                        m_NewExit.containedCells.Add(ExitLocation + new Vector2(0, 1));
                    }
                }
                else
                {
                    if (exitSize > 2)
                    {
                        m_NewExit.containedCells.Add(ExitLocation);
                        m_NewExit.containedCells.Add(ExitLocation + new Vector2(1, 0));
                        for (int i = 2; i < exitSize; i++)
                        {
                            m_NewExit.containedCells.Add(ExitLocation + new Vector2(i, 0));
                        }
                    }
                    else
                    {
                        m_NewExit.containedCells.Add(ExitLocation);
                        m_NewExit.containedCells.Add(ExitLocation + new Vector2(1, 0));
                    }
                }

                if (overrideDoorObject) { m_NewExit.specifiedDoor = overrideDoorObject; }

                room.exitData.exits.Add(m_NewExit);
            }

            public static void AddObjectToRoom(PrototypeDungeonRoom room, Vector2 position, DungeonPlaceable PlacableContents = null, DungeonPlaceableBehaviour NonEnemyBehaviour = null, string EnemyBehaviourGuid = null, float SpawnChance = 1f, int xOffset = 0, int yOffset = 0, int layer = 0, int PathID = -1, int PathStartNode = 0)
            {
                if (room == null) { return; }
                if (room.placedObjects == null) { room.placedObjects = new List<PrototypePlacedObjectData>(); }
                if (room.placedObjectPositions == null) { room.placedObjectPositions = new List<Vector2>(); }

                PrototypePlacedObjectData m_NewObjectData = new PrototypePlacedObjectData()
                {
                    placeableContents = null,
                    nonenemyBehaviour = null,
                    spawnChance = SpawnChance,
                    unspecifiedContents = null,
                    enemyBehaviourGuid = string.Empty,
                    contentsBasePosition = position,
                    layer = layer,
                    xMPxOffset = xOffset,
                    yMPxOffset = yOffset,
                    fieldData = new List<PrototypePlacedObjectFieldData>(0),
                    instancePrerequisites = new DungeonPrerequisite[0],
                    linkedTriggerAreaIDs = new List<int>(0),
                    assignedPathIDx = PathID,
                    assignedPathStartNode = PathStartNode
                };

                if (PlacableContents != null)
                {
                    m_NewObjectData.placeableContents = PlacableContents;
                }
                else if (NonEnemyBehaviour != null)
                {
                    m_NewObjectData.nonenemyBehaviour = NonEnemyBehaviour;
                }
                else if (EnemyBehaviourGuid != null)
                {
                    m_NewObjectData.enemyBehaviourGuid = EnemyBehaviourGuid;
                }
                else
                {
                    // All possible object fields were left null? Do nothing and return if this is the case.
                    return;
                }

                room.placedObjects.Add(m_NewObjectData);
                room.placedObjectPositions.Add(position);
                return;
            }

            public static void AddObjectToRoom(PrototypeDungeonRoom room, Vector2 position, GameObject PlacableObject, int xOffset = 0, int yOffset = 0, int layer = 0, float SpawnChance = 1f, int PathID = -1, int PathStartNode = 0)
            {
                if (room == null) { return; }
                if (room.placedObjects == null) { room.placedObjects = new List<PrototypePlacedObjectData>(); }
                if (room.placedObjectPositions == null) { room.placedObjectPositions = new List<Vector2>(); }

                PrototypePlacedObjectData m_NewObjectData = new PrototypePlacedObjectData()
                {
                    placeableContents = MiscUtilityCode.GenerateDungeonPlacable(PlacableObject, useExternalPrefab: true),
                    nonenemyBehaviour = null,
                    spawnChance = SpawnChance,
                    unspecifiedContents = null,
                    enemyBehaviourGuid = string.Empty,
                    contentsBasePosition = position,
                    layer = layer,
                    xMPxOffset = xOffset,
                    yMPxOffset = yOffset,
                    fieldData = new List<PrototypePlacedObjectFieldData>(0),
                    instancePrerequisites = new DungeonPrerequisite[0],
                    linkedTriggerAreaIDs = new List<int>(0),
                    assignedPathIDx = PathID,
                    assignedPathStartNode = PathStartNode
                };

                room.placedObjects.Add(m_NewObjectData);
                room.placedObjectPositions.Add(position);
                return;
            }

        public static Texture2D DumpRoomAreaToTexture2D(RoomHandler room)
        {
            int width = room.area.dimensions.x;
            int height = room.area.dimensions.y;
            IntVector2 basePosition = room.area.basePosition;

            Texture2D m_NewImage = new Texture2D(width, height, TextureFormat.RGBA32, false);
            if (!string.IsNullOrEmpty(room.GetRoomName())) { m_NewImage.name = room.GetRoomName(); }

            Color WhitePixel = new Color32(255, 255, 255, 255); // Wall Cell
            Color PinkPixel = new Color32(255, 0, 255, 255); // Diagonal Wall Cell (North East)
            Color YellowPixel = new Color32(255, 255, 0, 255); // Diagonal Wall Cell (North West)
            Color HalfPinkPixel = new Color32(127, 0, 127, 255); // Diagonal Wall Cell (South East)
            Color HalfYellowPixel = new Color32(127, 127, 0, 255); // Diagonal Wall Cell (South West)

            Color BluePixel = new Color32(0, 0, 255, 255); // Floor Cell

            Color BlueHalfGreenPixel = new Color32(0, 127, 255, 255); // Floor Cell (Ice Override)
            Color HalfBluePixel = new Color32(0, 0, 127, 255); // Floor Cell (Water Override)
            Color HalfRedPixel = new Color32(0, 0, 127, 255); // Floor Cell (Carpet Override)
            Color GreenHalfRBPixel = new Color32(127, 255, 127, 255); // Floor Cell (Grass Override)
            Color HalfWhitePixel = new Color32(127, 127, 127, 255); // Floor Cell (Bone Override)
            Color OrangePixel = new Color32(255, 127, 0, 255); // Floor Cell (Flesh Override)
            Color RedHalfGBPixel = new Color32(255, 127, 127, 255); // Floor Cell (ThickGoop Override)

            Color GreenPixel = new Color32(0, 255, 0, 255); // Damage Floor Cell

            Color RedPixel = new Color32(255, 0, 0, 255); // Pit Cell

            Color BlackPixel = new Color32(0, 0, 0, 255); // NULL Cell

            for (int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    IntVector2 m_CellPosition = (new IntVector2(X, Y) + basePosition);
                    if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(m_CellPosition.x, m_CellPosition.y))
                    {
                        CellType? cellData = GameManager.Instance.Dungeon.data[m_CellPosition].type;
                        CellData localDungeonData = GameManager.Instance.Dungeon.data[m_CellPosition];
                        bool DamageCell = false;
                        DiagonalWallType diagonalWallType = DiagonalWallType.NONE;
                        if (localDungeonData != null)
                        {
                            DamageCell = localDungeonData.doesDamage;
                            diagonalWallType = localDungeonData.diagonalWallType;
                        }
                        if (localDungeonData == null | !cellData.HasValue)
                        {
                            m_NewImage.SetPixel(X, Y, BlackPixel);
                        }
                        else if (cellData.Value == CellType.FLOOR)
                        {
                            if (DamageCell)
                            {
                                m_NewImage.SetPixel(X, Y, GreenPixel);
                            }
                            else
                            {
                                CellVisualData.CellFloorType overrideFloorType = localDungeonData.cellVisualData.floorType;
                                if (overrideFloorType == CellVisualData.CellFloorType.Stone)
                                {
                                    m_NewImage.SetPixel(X, Y, BluePixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.Ice)
                                {
                                    m_NewImage.SetPixel(X, Y, BlueHalfGreenPixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.Water)
                                {
                                    m_NewImage.SetPixel(X, Y, HalfBluePixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.Carpet)
                                {
                                    m_NewImage.SetPixel(X, Y, HalfRedPixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.Grass)
                                {
                                    m_NewImage.SetPixel(X, Y, GreenHalfRBPixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.Bone)
                                {
                                    m_NewImage.SetPixel(X, Y, HalfWhitePixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.Flesh)
                                {
                                    m_NewImage.SetPixel(X, Y, OrangePixel);
                                }
                                else if (overrideFloorType == CellVisualData.CellFloorType.ThickGoop)
                                {
                                    m_NewImage.SetPixel(X, Y, RedHalfGBPixel);
                                }
                                else
                                {
                                    m_NewImage.SetPixel(X, Y, BluePixel);
                                }
                            }
                        }
                        else if (cellData.Value == CellType.WALL)
                        {
                            if (diagonalWallType == DiagonalWallType.NORTHEAST)
                            {
                                m_NewImage.SetPixel(X, Y, PinkPixel);
                            }
                            else if (diagonalWallType == DiagonalWallType.NORTHWEST)
                            {
                                m_NewImage.SetPixel(X, Y, YellowPixel);
                            }
                            else if (diagonalWallType == DiagonalWallType.SOUTHEAST)
                            {
                                m_NewImage.SetPixel(X, Y, HalfPinkPixel);
                            }
                            else if (diagonalWallType == DiagonalWallType.SOUTHWEST)
                            {
                                m_NewImage.SetPixel(X, Y, HalfYellowPixel);
                            }
                            else
                            {
                                m_NewImage.SetPixel(X, Y, WhitePixel);
                            }
                        }
                        else if (cellData.Value == CellType.PIT)
                        {
                            m_NewImage.SetPixel(X, Y, RedPixel);
                        }
                    }
                    else
                    {
                        m_NewImage.SetPixel(X, Y, BlackPixel);
                    }
                }
            }

            m_NewImage.Apply();

            return m_NewImage;
        }
    }


    }
