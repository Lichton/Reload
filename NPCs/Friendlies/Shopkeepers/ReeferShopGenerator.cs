using Dungeonator;
using Alexandria.DungeonAPI;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Alexandria.NPCAPI.CustomShopController;
using static Alexandria.NPCAPI.ShopAPI;
using static Alexandria.NPCAPI.NpcTools;
using Alexandria.NPCAPI;

namespace Reload
{
    public static class ReeferShopGenerator
    {
        public static GameObject SetUpShop(string name, string prefix, List<string> idleSpritePaths, int idleFps, List<string> talkSpritePaths, int talkFps, GenericLootTable lootTable, CustomShopItemController.ShopCurrencyType currency, string runBasedMultilineGenericStringKey,
          string runBasedMultilineStopperStringKey, string purchaseItemStringKey, string purchaseItemFailedStringKey, string introStringKey, string attackedStringKey, string stolenFromStringKey, Vector3 talkPointOffset, Vector3 npcPosition, VoiceBoxes voiceBox = VoiceBoxes.OLD_MAN, Vector3[] itemPositions = null, float costModifier = 1, bool giveStatsOnPurchase = false,
          StatModifier[] statsToGiveOnPurchase = null, Func<CustomShopController, PlayerController, int, bool> CustomCanBuy = null, Func<CustomShopController, PlayerController, int, int> CustomRemoveCurrency = null, Func<CustomShopController, CustomShopItemController, PickupObject, int> CustomPrice = null,
          Func<PlayerController, PickupObject, int, bool> OnPurchase = null, Func<PlayerController, PickupObject, int, bool> OnSteal = null, string currencyIconPath = "", string currencyName = "", bool canBeRobbed = true, bool hasCarpet = false, string carpetSpritePath = "",
          Vector2? CarpetOffset = null, bool hasMinimapIcon = false, string minimapIconSpritePath = "", bool addToMainNpcPool = false, float percentChanceForMainPool = 0.1f, DungeonPrerequisite[] prerequisites = null, float fortunesFavorRadius = 2,
          ShopItemPoolType poolType = ShopItemPoolType.DEFAULT, bool RainbowModeImmunity = false, IntVector2? hitboxSize = null, IntVector2? hitboxOffset = null)
        {

            try
            {

                if (prerequisites == null)
                {
                    prerequisites = new DungeonPrerequisite[0];
                }
                //bool isBreachShop = false;
                Vector3 breachPos = Vector3.zero;

                var shared_auto_001 = ResourceManager.LoadAssetBundle("shared_auto_001");
                var shared_auto_002 = ResourceManager.LoadAssetBundle("shared_auto_002");
                var SpeechPoint = new GameObject("SpeechPoint");
                SpeechPoint.transform.position = talkPointOffset;



                var npcObj = SpriteBuilder.SpriteFromResource(idleSpritePaths[0], new GameObject(prefix + ":" + name), Assembly.GetCallingAssembly());

                FakePrefab.MarkAsFakePrefab(npcObj);
                UnityEngine.Object.DontDestroyOnLoad(npcObj);
                npcObj.SetActive(false);

                npcObj.layer = 22;

                var collection = npcObj.GetComponent<tk2dSprite>().Collection;
                SpeechPoint.transform.parent = npcObj.transform;

                FakePrefab.MarkAsFakePrefab(SpeechPoint);
                UnityEngine.Object.DontDestroyOnLoad(SpeechPoint);
                SpeechPoint.SetActive(true);


                var idleIdsList = new List<int>();
                var talkIdsList = new List<int>();

                foreach (string sprite in idleSpritePaths)
                {
                    idleIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection, Assembly.GetCallingAssembly()));
                }

                foreach (string sprite in talkSpritePaths)
                {
                    talkIdsList.Add(SpriteBuilder.AddSpriteToCollection(sprite, collection, Assembly.GetCallingAssembly()));
                }

                tk2dSpriteAnimator spriteAnimator = npcObj.AddComponent<tk2dSpriteAnimator>();

                SpriteBuilder.AddAnimation(spriteAnimator, collection, idleIdsList, "idle", tk2dSpriteAnimationClip.WrapMode.Loop, idleFps);
                SpriteBuilder.AddAnimation(spriteAnimator, collection, talkIdsList, "talk", tk2dSpriteAnimationClip.WrapMode.Loop, talkFps);


                if (hitboxSize == null) hitboxSize = new IntVector2(20, 40);
                if (hitboxOffset == null) new IntVector2(3, -2);

                SpeculativeRigidbody rigidbody = GenerateOrAddToRigidBody(npcObj, CollisionLayer.LowObstacle, PixelCollider.PixelColliderGeneration.Manual, true, true, true, false, false, false, false, true, hitboxSize, hitboxOffset);
                rigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker));

                //SpeculativeRigidbody rigidbody = GenerateOrAddToRigidBody(npcObj, CollisionLayer.BulletBlocker, PixelCollider.PixelColliderGeneration.Manual, true, true, true, false, false, false, false, true, new IntVector2(20, 18), new IntVector2(5, 0));

                TalkDoerLite talkDoer = npcObj.AddComponent<TalkDoerLite>();

                talkDoer.placeableWidth = 4;
                talkDoer.placeableHeight = 3;
                talkDoer.difficulty = 0;
                talkDoer.isPassable = true;
                talkDoer.usesOverrideInteractionRegion = false;
                talkDoer.overrideRegionOffset = Vector2.zero;
                talkDoer.overrideRegionDimensions = Vector2.zero;
                talkDoer.overrideInteractionRadius = -1;
                talkDoer.PreventInteraction = false;
                talkDoer.AllowPlayerToPassEventually = true;
                talkDoer.speakPoint = SpeechPoint.transform;
                talkDoer.SpeaksGleepGlorpenese = false;
                talkDoer.audioCharacterSpeechTag = ReturnVoiceBox(voiceBox);
                talkDoer.playerApproachRadius = 5;
                talkDoer.conversationBreakRadius = 5;
                talkDoer.echo1 = null;
                talkDoer.echo2 = null;
                talkDoer.PreventCoopInteraction = false;
                talkDoer.IsPaletteSwapped = false;
                talkDoer.PaletteTexture = null;
                talkDoer.OutlineDepth = 0.5f;
                talkDoer.OutlineLuminanceCutoff = 0.05f;
                talkDoer.MovementSpeed = 3;
                talkDoer.PathableTiles = CellTypes.FLOOR;


                UltraFortunesFavor dreamLuck = npcObj.AddComponent<UltraFortunesFavor>();

                dreamLuck.goopRadius = fortunesFavorRadius;
                dreamLuck.beamRadius = fortunesFavorRadius;
                dreamLuck.bulletRadius = fortunesFavorRadius;
                dreamLuck.bulletSpeedModifier = 0.8f;

                dreamLuck.vfxOffset = 0.625f;
                dreamLuck.sparkOctantVFX = shared_auto_001.LoadAsset<GameObject>("FortuneFavor_VFX_Spark");


                AIAnimator aIAnimator = GenerateBlankAIAnimator(npcObj);
                aIAnimator.spriteAnimator = spriteAnimator;
                aIAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.Single,
                    Prefix = "idle",
                    AnimNames = new string[]
                    {
                        ""
                    },
                    Flipped = new DirectionalAnimation.FlipType[]
                    {
                        DirectionalAnimation.FlipType.None
                    }

                };

                aIAnimator.TalkAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.Single,
                    Prefix = "talk",
                    AnimNames = new string[]
                    {
                        ""
                    },
                    Flipped = new DirectionalAnimation.FlipType[]
                    {
                        DirectionalAnimation.FlipType.None
                    }
                };



                var basenpc = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<GameObject>("Merchant_Key").transform.Find("NPC_Key").gameObject;

                PlayMakerFSM iHaveNoFuckingClueWhatThisIs = npcObj.AddComponent<PlayMakerFSM>();

                UnityEngine.JsonUtility.FromJsonOverwrite(UnityEngine.JsonUtility.ToJson(basenpc.GetComponent<PlayMakerFSM>()), iHaveNoFuckingClueWhatThisIs);

                FieldInfo fsmStringParams = typeof(ActionData).GetField("fsmStringParams", BindingFlags.NonPublic | BindingFlags.Instance);

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[1].ActionData) as List<FsmString>)[0].Value = runBasedMultilineGenericStringKey;
                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[1].ActionData) as List<FsmString>)[1].Value = runBasedMultilineStopperStringKey;

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[4].ActionData) as List<FsmString>)[0].Value = purchaseItemStringKey;

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[5].ActionData) as List<FsmString>)[0].Value = purchaseItemFailedStringKey;

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[7].ActionData) as List<FsmString>)[0].Value = introStringKey;

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[8].ActionData) as List<FsmString>)[0].Value = attackedStringKey;

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[9].ActionData) as List<FsmString>)[0].Value = stolenFromStringKey;
                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[9].ActionData) as List<FsmString>)[1].Value = stolenFromStringKey;

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[10].ActionData) as List<FsmString>)[0].Value = "#SHOP_GENERIC_NO_SALE_LABEL";

                (fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[12].ActionData) as List<FsmString>)[0].Value = "#COOP_REBUKE";

                /*
                foreach (FsmString fuck in fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[9].ActionData) as List<FsmString>)
                {
                    ETGModConsole.Log(fuck.Value);
                }
                */

                npcObj.name = prefix + ":" + name;

                var posList = new List<Transform>();
                for (int i = 0; i < itemPositions.Length; i++)
                {

                    var ItemPoint = new GameObject("ItemPoint" + i);
                    ItemPoint.transform.position = itemPositions[i];
                    FakePrefab.MarkAsFakePrefab(ItemPoint);
                    UnityEngine.Object.DontDestroyOnLoad(ItemPoint);
                    ItemPoint.SetActive(true);
                    posList.Add(ItemPoint.transform);
                }

                var ItemPoint1 = new GameObject("ItemPoint1");
                ItemPoint1.transform.position = new Vector3(1.125f, 2.125f, 1);
                FakePrefab.MarkAsFakePrefab(ItemPoint1);
                UnityEngine.Object.DontDestroyOnLoad(ItemPoint1);
                ItemPoint1.SetActive(true);
                var ItemPoint2 = new GameObject("ItemPoint2");
                ItemPoint2.transform.position = new Vector3(2.625f, 1f, 1);
                FakePrefab.MarkAsFakePrefab(ItemPoint2);
                UnityEngine.Object.DontDestroyOnLoad(ItemPoint2);
                ItemPoint2.SetActive(true);
                var ItemPoint3 = new GameObject("ItemPoint3");
                ItemPoint3.transform.position = new Vector3(4.125f, 2.125f, 1);
                FakePrefab.MarkAsFakePrefab(ItemPoint3);
                UnityEngine.Object.DontDestroyOnLoad(ItemPoint3);
                ItemPoint3.SetActive(true);


                var shopObj = new GameObject(prefix + ":" + name + "_Shop").AddComponent<ReeferShopController>();
                shopObj.AllowedToSpawnOnRainbowMode = RainbowModeImmunity;
                FakePrefab.MarkAsFakePrefab(shopObj.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(shopObj.gameObject);

                shopObj.gameObject.SetActive(false);

                shopObj.currencyType = (Alexandria.NPCAPI.CustomShopItemController.ShopCurrencyType)currency;

                shopObj.ActionAndFuncSetUp(CustomCanBuy, CustomRemoveCurrency, CustomPrice, OnPurchase, OnSteal);

                if (currency == CustomShopItemController.ShopCurrencyType.CUSTOM)
                {
                    if (!string.IsNullOrEmpty(currencyIconPath))
                    {
                        shopObj.customPriceSprite = AddCustomCurrencyType(currencyIconPath, $"{prefix}:{currencyName}", Assembly.GetCallingAssembly());
                    }
                    else
                    {
                        shopObj.customPriceSprite = currencyName;
                    }
                }



                //GungeonAPI.ToolsGAPI.AddNewItemToAtlas()

                shopObj.canBeRobbed = canBeRobbed;

                shopObj.placeableHeight = 5;
                shopObj.placeableWidth = 5;
                shopObj.difficulty = 0;
                shopObj.isPassable = true;
                shopObj.baseShopType = BaseShopController.AdditionalShopType.TRUCK;//shopType;

                shopObj.FoyerMetaShopForcedTiers = false;
                shopObj.IsBeetleMerchant = false;
                shopObj.ExampleBlueprintPrefab = null;
                shopObj.poolType = poolType;
                shopObj.shopItems = lootTable;
                shopObj.spawnPositions = posList.ToArray();//{ ItemPoint1.transform, ItemPoint2.transform, ItemPoint3.transform };

                foreach (var pos in shopObj.spawnPositions)
                {
                    pos.parent = shopObj.gameObject.transform;
                }

                shopObj.shopItemsGroup2 = null;
                shopObj.spawnPositionsGroup2 = null;
                shopObj.spawnGroupTwoItem1Chance = 0.5f;
                shopObj.spawnGroupTwoItem2Chance = 0.5f;
                shopObj.spawnGroupTwoItem3Chance = 0.5f;
                shopObj.shopkeepFSM = npcObj.GetComponent<PlayMakerFSM>();
                shopObj.shopItemShadowPrefab = shared_auto_001.LoadAsset<GameObject>("Merchant_Key").GetComponent<BaseShopController>().shopItemShadowPrefab;

                shopObj.prerequisites = prerequisites;
                //shopObj.shopItemShadowPrefab = 

                shopObj.cat = null;


                if (hasMinimapIcon)
                {
                    if (!string.IsNullOrEmpty(minimapIconSpritePath))
                    {
                        shopObj.OptionalMinimapIcon = SpriteBuilder.SpriteFromResource(minimapIconSpritePath, null, Assembly.GetCallingAssembly());
                        UnityEngine.Object.DontDestroyOnLoad(shopObj.OptionalMinimapIcon);
                        FakePrefab.MarkAsFakePrefab(shopObj.OptionalMinimapIcon);
                    }
                    else
                    {
                        shopObj.OptionalMinimapIcon = ResourceCache.Acquire("Global Prefabs/Minimap_NPC_Icon") as GameObject;
                    }
                }

                shopObj.ShopCostModifier = costModifier;
                shopObj.FlagToSetOnEncounter = GungeonFlags.NONE;
                shopObj.giveStatsOnPurchase = giveStatsOnPurchase;
                shopObj.statsToGive = statsToGiveOnPurchase;

                //shopObj.

                /*if (isBreachShop)
                {
                    shopObj.gameObject.AddComponent<BreachShopComp>().offset = breachPos;
                    BreachShopTools.registeredShops.Add(prefix + ":" + name, shopObj.gameObject);
                    shopObj.FoyerMetaShopForcedTiers = true;
                    var exampleBlueprintObj = SpriteBuilder.SpriteFromResource(carpetSpritePath, new GameObject(prefix + ":" + name + "_ExampleBlueprintPrefab"));
                    exampleBlueprintObj.GetComponent<tk2dSprite>().SortingOrder = 2;
                    FakePrefab.MarkAsFakePrefab(exampleBlueprintObj);
                    UnityEngine.Object.DontDestroyOnLoad(exampleBlueprintObj);
                    exampleBlueprintObj.SetActive(false);
                    //var item = exampleBlueprintObj.AddComponent<ItemBlueprintItem>();
                    //item.quality = PickupObject.ItemQuality.SPECIAL;
                    //item.PickupObjectId = 99999999;
                    
                    shopObj.ExampleBlueprintPrefab = shared_auto_001.LoadAsset<GameObject>("NPC_Beetle_Merchant_Foyer").GetComponent<BaseShopController>().ExampleBlueprintPrefab;
                }*/

                npcObj.transform.parent = shopObj.gameObject.transform;
                npcObj.transform.position = npcPosition;//new Vector3(1.9375f, 3.4375f, 5.9375f) + npcPositionOffset;
                RMShop(shopObj, npcObj);



                if (hasCarpet)
                {
                    var carpetObj = SpriteBuilder.SpriteFromResource(carpetSpritePath, new GameObject(prefix + ":" + name + "_Carpet"), Assembly.GetCallingAssembly());
                    carpetObj.GetComponent<tk2dSprite>().SortingOrder = 5;
                    FakePrefab.MarkAsFakePrefab(carpetObj);
                    UnityEngine.Object.DontDestroyOnLoad(carpetObj);
                    carpetObj.SetActive(true);

                    if (CarpetOffset == null) CarpetOffset = Vector2.zero;

                    carpetObj.transform.position = new Vector3(CarpetOffset.Value.x, CarpetOffset.Value.y, 1.7f);
                    carpetObj.transform.parent = shopObj.gameObject.transform;
                    carpetObj.layer = 20;
                }
                npcObj.SetActive(true);

                if (addToMainNpcPool)
                {
                    shared_auto_002.LoadAsset<DungeonPlaceable>("shopannex_contents_01").variantTiers.Add(new DungeonPlaceableVariant
                    {
                        percentChance = percentChanceForMainPool,
                        unitOffset = new Vector2(-0.5f, -1.25f),
                        nonDatabasePlaceable = shopObj.gameObject,
                        enemyPlaceableGuid = "",
                        pickupObjectPlaceableId = -1,
                        forceBlackPhantom = false,
                        addDebrisObject = false,
                        prerequisites = prerequisites, //shit for unlocks gose here sooner or later
                        materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0],

                    });
                }

                ShopAPI.builtShops.Add(prefix + ":" + name, shopObj.gameObject);
                return shopObj.gameObject;
            }
            catch (Exception message)
            {
                ETGModConsole.Log(message.ToString());
                return null;
            }
        }

        public static void RMShop(Alexandria.NPCAPI.CustomShopController shopObj, GameObject npcObj)
        {
            var carpetObj = SpriteBuilder.SpriteFromResource("Reload/Resources/NPCs/Allied/Reefer/RestockMachine/Idle/restock_idle_001");
            carpetObj.name = "Restock" + integer;
            carpetObj.SetActive(true);
            carpetObj.layer = 20;
            carpetObj.transform.parent = shopObj.transform;
            var restock = carpetObj.AddComponent<RestockMachine>();
            restock.Start();
            integer++;
        }

        public static int integer = 1;
    }
}
