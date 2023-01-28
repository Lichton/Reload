using Alexandria.DungeonAPI;
using Alexandria.ItemAPI;
using Alexandria.NPCAPI;
using Dungeonator;
using GungeonAPI;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Alexandria.NPCAPI.CustomShopController;

namespace Reload
{
    public class SpawnObjectManager : MonoBehaviour //----------------------------------------------------------------------------------------------------------------------------
    {
        public static void SpawnObject(GameObject thingToSpawn, Vector3 convertedVector, GameObject SpawnVFX, bool correctForWalls = false)
        {
            Vector2 Vector2Position = convertedVector;

            GameObject newObject = Instantiate(thingToSpawn, convertedVector, Quaternion.identity);

            SpeculativeRigidbody ObjectSpecRigidBody = newObject.GetComponentInChildren<SpeculativeRigidbody>();
            Component[] componentsInChildren = newObject.GetComponentsInChildren(typeof(IPlayerInteractable));
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                IPlayerInteractable interactable = componentsInChildren[i] as IPlayerInteractable;
                if (interactable != null)
                {
                    newObject.transform.position.GetAbsoluteRoom().RegisterInteractable(interactable);
                }
            }
            Component[] componentsInChildren2 = newObject.GetComponentsInChildren(typeof(IPlaceConfigurable));
            for (int i = 0; i < componentsInChildren2.Length; i++)
            {
                IPlaceConfigurable placeConfigurable = componentsInChildren2[i] as IPlaceConfigurable;
                if (placeConfigurable != null)
                {
                    placeConfigurable.ConfigureOnPlacement(GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Position.ToIntVector2()));
                }
            }
            /* FlippableCover component7 = newObject.GetComponentInChildren<FlippableCover>();
             component7.transform.position.XY().GetAbsoluteRoom().RegisterInteractable(component7);
             component7.ConfigureOnPlacement(component7.transform.position.XY().GetAbsoluteRoom());*/

            ObjectSpecRigidBody.Initialize();
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(ObjectSpecRigidBody, null, false);

            if (SpawnVFX != null)
            {
                UnityEngine.Object.Instantiate<GameObject>(SpawnVFX, ObjectSpecRigidBody.sprite.WorldCenter, Quaternion.identity);
            }
            if (correctForWalls) CorrectForWalls(newObject);
        }
        private static void CorrectForWalls(GameObject portal)
        {
            SpeculativeRigidbody rigidbody = portal.GetComponent<SpeculativeRigidbody>();
            if (rigidbody)
            {
                bool flag = PhysicsEngine.Instance.OverlapCast(rigidbody, null, true, false, null, null, false, null, null, new SpeculativeRigidbody[0]);
                if (flag)
                {
                    Vector2 vector = portal.transform.position.XY();
                    IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
                    int num = 0;
                    int num2 = 1;
                    for (; ; )
                    {
                        for (int i = 0; i < cardinalsAndOrdinals.Length; i++)
                        {
                            portal.transform.position = vector + PhysicsEngine.PixelToUnit(cardinalsAndOrdinals[i] * num2);
                            rigidbody.Reinitialize();
                            if (!PhysicsEngine.Instance.OverlapCast(rigidbody, null, true, false, null, null, false, null, null, new SpeculativeRigidbody[0]))
                            {
                                return;
                            }
                        }
                        num2++;
                        num++;
                        if (num > 200)
                        {
                            goto Block_4;
                        }
                    }
                //return;
                Block_4:
                    UnityEngine.Debug.LogError("FREEZE AVERTED!  TELL RUBEL!  (you're welcome) 147");
                    return;
                }
            }
        }
    }
    public static class DungeonToolbox
    {
        public static void RegisterShopRoomWeighted(GameObject shop, PrototypeDungeonRoom protoroom, Vector2 vector, float weight = 1)
        {
            protoroom.category = PrototypeDungeonRoom.RoomCategory.NORMAL;
            DungeonPrerequisite[] array = shop.GetComponent<CustomShopController>()?.prerequisites != null ? shop.GetComponent<CustomShopController>().prerequisites : new DungeonPrerequisite[0];
            //Vector2 vector = new Vector2((float)(protoroom.Width / 2) + offset.x, (float)(protoroom.Height / 2) + offset.y);
            protoroom.placedObjectPositions.Add(vector);
            protoroom.placedObjects.Add(new PrototypePlacedObjectData
            {
                contentsBasePosition = vector,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = array,
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = new DungeonPlaceable
                {
                    width = 2,
                    height = 2,
                    respectsEncounterableDifferentiator = true,
                    variantTiers = new List<DungeonPlaceableVariant>
                    {
                        new DungeonPlaceableVariant
                        {
                            percentChance = 1f,
                            nonDatabasePlaceable = shop,
                            prerequisites = array,
                            materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
                        }
                    }
                }
            });
            RoomFactory.RoomData roomData = new RoomFactory.RoomData
            {
                room = protoroom,
                isSpecialRoom = true,

                category = "SPECIAL",
                specialSubCategory = "WEIRD_SHOP",
                weight = weight
            };
            RoomFactory.rooms.Add(shop.name, roomData);
            DungeonHandler.Register(roomData);
        }

		public static void Initialize(PickupObject i, CustomShopController parent, CustomShopItemController shoppy)
		{
			shoppy.m_baseParentShop = parent;
			InitializeInternal(i, shoppy);
			if (parent.baseShopType != BaseShopController.AdditionalShopType.NONE)
			{
				shoppy.sprite.depthUsesTrimmedBounds = true;
				shoppy.sprite.HeightOffGround = -1.25f;
				shoppy.sprite.UpdateZDepth();
			}
		}

		public static void InitializeInternal(PickupObject i, CustomShopItemController shoppy)
		{
			shoppy.item = i;
			if (shoppy.item && shoppy.item.encounterTrackable)
			{
				GameStatsManager.Instance.SingleIncrementDifferentiator(shoppy.item.encounterTrackable);
			}

			shoppy.CurrentPrice = shoppy.item.PurchasePrice;

			shoppy.gameObject.AddComponent<tk2dSprite>();
			tk2dSprite tk2dSprite = i.GetComponent<tk2dSprite>();
			if (tk2dSprite == null)
			{
				tk2dSprite = i.GetComponentInChildren<tk2dSprite>();
			}
			shoppy.sprite.SetSprite(tk2dSprite.Collection, tk2dSprite.spriteId);
			shoppy.sprite.IsPerpendicular = true;
			if (shoppy.UseOmnidirectionalItemFacing)
			{
				shoppy.sprite.IsPerpendicular = false;
			}
			shoppy.sprite.HeightOffGround = 1f;
			shoppy.UseOmnidirectionalItemFacing = true;
			shoppy.sprite.PlaceAtPositionByAnchor(shoppy.transform.parent.position, tk2dBaseSprite.Anchor.MiddleCenter);
			shoppy.sprite.transform.position = shoppy.sprite.transform.position.Quantize(0.0625f);
			DepthLookupManager.ProcessRenderer(shoppy.sprite.renderer);
			tk2dSprite componentInParent = shoppy.transform.parent.gameObject.GetComponentInParent<tk2dSprite>();
			if (componentInParent != null)
			{
				componentInParent.AttachRenderer(shoppy.sprite);
			}
			SpriteOutlineManager.AddOutlineToSprite(shoppy.sprite, Color.black, 0.1f, 0.05f, SpriteOutlineManager.OutlineType.NORMAL);
			GameObject gameObject = null;
			if (shoppy.m_parentShop != null && shoppy.m_parentShop.shopItemShadowPrefab != null)
			{
				gameObject = shoppy.m_parentShop.shopItemShadowPrefab;
			}
			if (shoppy.m_baseParentShop != null && shoppy.m_baseParentShop.shopItemShadowPrefab != null)
			{
				gameObject = shoppy.m_baseParentShop.shopItemShadowPrefab;
			}
			if (gameObject != null)
			{
				if (!shoppy.m_shadowObject)
				{
					shoppy.m_shadowObject = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				}
				tk2dBaseSprite component = shoppy.m_shadowObject.GetComponent<tk2dBaseSprite>();
				component.PlaceAtPositionByAnchor(shoppy.sprite.WorldBottomCenter, tk2dBaseSprite.Anchor.MiddleCenter);
				component.transform.position = component.transform.position.Quantize(0.0625f);
				shoppy.sprite.AttachRenderer(component);
				component.transform.parent = shoppy.sprite.transform;
				component.HeightOffGround = -0.5f;
			}
			shoppy.sprite.UpdateZDepth();
			SpeculativeRigidbody orAddComponent = shoppy.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
			orAddComponent.PixelColliders = new List<PixelCollider>();
			PixelCollider pixelCollider = new PixelCollider
			{
				ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle,
				CollisionLayer = CollisionLayer.HighObstacle,
				ManualDiameter = 14
			};
			Vector2 vector = shoppy.sprite.WorldCenter - shoppy.transform.position.XY();
			pixelCollider.ManualOffsetX = PhysicsEngine.UnitToPixel(vector.x) - 7;
			pixelCollider.ManualOffsetY = PhysicsEngine.UnitToPixel(vector.y) - 7;
			orAddComponent.PixelColliders.Add(pixelCollider);
			orAddComponent.Initialize();
			orAddComponent.OnPreRigidbodyCollision = null;
			SpeculativeRigidbody speculativeRigidbody = orAddComponent;
			speculativeRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(speculativeRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(shoppy.ItemOnPreRigidbodyCollision));
			shoppy.RegenerateCache();
		}
	}

    class EasyGoopDefinitions
    {
        //Basegame Goops
        public static GoopDefinition FireDef;
        public static GoopDefinition OilDef;
        public static GoopDefinition PoisonDef;
        public static GoopDefinition BlobulonGoopDef;
        public static GoopDefinition WebGoop;
        public static GoopDefinition WaterGoop;
        public static GoopDefinition NapalmGoop;
        public static GoopDefinition NapalmGoopQuickIgnite;
        public static GoopDefinition CharmGoopDef = PickupObjectDatabase.GetById(310)?.GetComponent<WingsItem>()?.RollGoop;
        public static GoopDefinition GreenFireDef = (PickupObjectDatabase.GetById(698) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
        public static GoopDefinition CheeseDef = (PickupObjectDatabase.GetById(808) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
        public static GoopDefinition noteblood = PickupObjectDatabase.GetById(272)?.GetComponent<IronCoinItem>()?.BloodDefinition;
        public static GoopDefinition MimicSpit = EnemyDatabase.GetOrLoadByGuid("479556d05c7c44f3b6abb3b2067fc778").GetComponent<GoopDoer>().goopDefinition;
        public static GoopDefinition BulletKingWine = EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("goblet").BulletObject.GetComponent<GoopDoer>().goopDefinition;


        public static void DefineDefaultGoops()
        {
            //Sets up the goops that have to be extracted from asset bundles
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            EasyGoopDefinitions.goopDefs = new List<GoopDefinition>();
            foreach (string text in EasyGoopDefinitions.goops)
            {
                GoopDefinition goopDefinition;
                try
                {
                    GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
                    goopDefinition = gameObject.GetComponent<GoopDefinition>();
                }
                catch
                {
                    goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
                }
                goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
                EasyGoopDefinitions.goopDefs.Add(goopDefinition);
            }
            List<GoopDefinition> list = EasyGoopDefinitions.goopDefs;

            //Define the asset bundle goops
            FireDef = EasyGoopDefinitions.goopDefs[0];
            OilDef = EasyGoopDefinitions.goopDefs[1];
            PoisonDef = EasyGoopDefinitions.goopDefs[2];
            BlobulonGoopDef = EasyGoopDefinitions.goopDefs[3];
            WebGoop = EasyGoopDefinitions.goopDefs[4];
            WaterGoop = EasyGoopDefinitions.goopDefs[5];
            NapalmGoop = EasyGoopDefinitions.goopDefs[6];
            NapalmGoopQuickIgnite = EasyGoopDefinitions.goopDefs[7];

        }
       
        private static string[] goops = new string[]
        {
            "assets/data/goops/napalmgoopthatworks.asset",
            "assets/data/goops/oil goop.asset",
            "assets/data/goops/poison goop.asset",
            "assets/data/goops/blobulongoop.asset",
            "assets/data/goops/phasewebgoop.asset",
            "assets/data/goops/water goop.asset",
            "assets/data/goops/napalm goop.asset",
            "assets/data/goops/napalmgoopquickignite.asset",
        };
        private static List<GoopDefinition> goopDefs;
    }

        public class EasyPlaceableObjects
        {
            public static GameObject CoffinVert = LoadHelper.LoadAssetFromAnywhere<GameObject>("Coffin_Vertical");
            public static GameObject CoffinHoriz = LoadHelper.LoadAssetFromAnywhere<GameObject>("Coffin_Horizontal");
            public static GameObject Brazier = LoadHelper.LoadAssetFromAnywhere<DungeonPlaceable>("brazier").variantTiers[0].GetOrLoadPlaceableObject;
            public static GameObject CursedPot = LoadHelper.LoadAssetFromAnywhere<DungeonPlaceable>("Curse Pot").variantTiers[0].GetOrLoadPlaceableObject;
            public static GameObject GenericBarrel = LoadHelper.LoadAssetFromAnywhere<DungeonPlaceable>("Barrel_collection").variantTiers[0].nonDatabasePlaceable;
            public static DungeonPlaceable GenericBarrelDungeonPlaceable = LoadHelper.LoadAssetFromAnywhere<DungeonPlaceable>("Barrel_collection");
            public static GameObject PoisonBarrel = LoadHelper.LoadAssetFromAnywhere<GameObject>("Yellow Drum");
            public static GameObject MetalExplosiveBarrel = LoadHelper.LoadAssetFromAnywhere<GameObject>("Red Drum");
            public static GameObject ExplosiveBarrel = LoadHelper.LoadAssetFromAnywhere<GameObject>("Red Barrel");
            public static GameObject WaterBarrel = LoadHelper.LoadAssetFromAnywhere<GameObject>("Blue Drum");
            public static GameObject OilBarrel = LoadHelper.LoadAssetFromAnywhere<GameObject>("Purple Drum");
            public static GameObject IceBomb = LoadHelper.LoadAssetFromAnywhere<GameObject>("Ice Cube Bomb");
            public static GameObject TableHorizontal = LoadHelper.LoadAssetFromAnywhere<GameObject>("Table_Horizontal");
            public static GameObject TableVertical = LoadHelper.LoadAssetFromAnywhere<GameObject>("Table_Vertical");
            public static GameObject TableHorizontalStone = LoadHelper.LoadAssetFromAnywhere<GameObject>("Table_Horizontal_Stone");
            public static GameObject TableVerticalStone = LoadHelper.LoadAssetFromAnywhere<GameObject>("Table_Vertical_Stone");
            public static GameObject SpikeTrap = LoadHelper.LoadAssetFromAnywhere<GameObject>("trap_spike_gungeon_2x2");
            public static GameObject FlameTrap = LoadHelper.LoadAssetFromAnywhere<GameObject>("trap_flame_poofy_gungeon_1x1");
            public static GameObject HangingPot = LoadHelper.LoadAssetFromAnywhere<GameObject>("Hanging_Pot");
            public static GameObject DeadBlow = LoadHelper.LoadAssetFromAnywhere<GameObject>("Forge_Hammer");
            public static GameObject ChestTruth = LoadHelper.LoadAssetFromAnywhere<GameObject>("TruthChest");
            public static GameObject ChestRat = LoadHelper.LoadAssetFromAnywhere<GameObject>("Chest_Rat");
            public static GameObject Mirror = LoadHelper.LoadAssetFromAnywhere<GameObject>("Shrine_Mirror");
            public static GameObject FoldingTable = PickupObjectDatabase.GetById(644).GetComponent<FoldingTableItem>().TableToSpawn.gameObject;
            public static GameObject BabyDragunNPC = LoadHelper.LoadAssetFromAnywhere<GameObject>("BabyDragunJail");
        }
}