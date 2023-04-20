using Alexandria.NPCAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using Dungeonator;
using Alexandria.Misc;

namespace Reload
{
	public class ReeferShopController : CustomShopController
	{
		public override void DoSetup()
		{
			base.m_shopItems = new List<GameObject>();

			List<int> list = new List<int>();
			Func<GameObject, float, float> weightModifier = null;
			if (base.m_room == null) { GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(new IntVector2((int)base.gameObject.transform.position.x, (int)base.gameObject.transform.position.y)); ETGModConsole.Log("null room"); }



			bool flag = GameStatsManager.Instance.IsRainbowRun && AllowedToSpawnOnRainbowMode == false;
			for (int i = 0; i < base.spawnPositions.Length; i++)
			{
				if (flag == true)
				{
					base.m_shopItems.Add(null);
				}
				else if (this.currencyType == CustomShopItemController.ShopCurrencyType.META_CURRENCY && this.ExampleBlueprintPrefab != null)
				{
					if (this.FoyerMetaShopForcedTiers)
					{
						List<WeightedGameObject> compiledRawItems = this.shopItems.GetCompiledRawItems();
						int num = 0;
						bool flag2 = true;
						while (flag2)
						{
							for (int j = num; j < num + this.spawnPositions.Length; j++)
							{
								if (j >= compiledRawItems.Count)
								{
									flag2 = false;
									break;
								}
								GameObject gameObject = compiledRawItems[j].gameObject;
								PickupObject component = gameObject.GetComponent<PickupObject>();
								if (!component.encounterTrackable.PrerequisitesMet())
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								num += this.spawnPositions.Length;
							}
						}
						for (int k = num; k < num + this.spawnPositions.Length; k++)
						{
							if (k >= compiledRawItems.Count)
							{
								this.m_shopItems.Add(null);
								list.Add(1);
							}
							else
							{
								GameObject gameObject2 = compiledRawItems[k].gameObject;
								PickupObject component2 = gameObject2.GetComponent<PickupObject>();
								if (this.m_shopItems.Contains(gameObject2) || component2.encounterTrackable.PrerequisitesMet())
								{
									this.m_shopItems.Add(null);
									list.Add(1);
								}
								else
								{
									this.m_shopItems.Add(gameObject2);
									list.Add(Mathf.RoundToInt(compiledRawItems[k].weight));
								}
							}
						}
					}
					else
					{

						List<WeightedGameObject> compiledRawItems2 = this.shopItems.GetCompiledRawItems();
						GameObject gameObject3 = null;
						for (int l = 0; l < compiledRawItems2.Count; l++)
						{
							GameObject gameObject4 = compiledRawItems2[l].gameObject;
							PickupObject component3 = gameObject4.GetComponent<PickupObject>();
							if (component3 == null) { continue; }
							if (!this.m_shopItems.Contains(gameObject4))
							{
								if (!component3.encounterTrackable.PrerequisitesMet())
								{
									gameObject3 = gameObject4;
									list.Add(Mathf.RoundToInt(compiledRawItems2[l].weight));
									break;
								}
							}
						}
						this.m_shopItems.Add(gameObject3);
						if (gameObject3 == null)
						{
							list.Add(1);
						}
					}
				}
				else
				{
					GameObject gameObject5 = new GameObject();
					switch (this.poolType)
					{
						case (CustomShopController.ShopItemPoolType)ShopItemPoolType.DEFAULT:
							gameObject5 = this.shopItems.SelectByWeightWithoutDuplicates(this.m_shopItems, GameManager.Instance.IsSeeded);
							this.m_shopItems.Add(gameObject5);
							break;
						case (CustomShopController.ShopItemPoolType)ShopItemPoolType.DUPES:
							gameObject5 = this.shopItems.SelectByWeight(GameManager.Instance.IsSeeded);
							this.m_shopItems.Add(gameObject5);
							break;
						case (CustomShopController.ShopItemPoolType)ShopItemPoolType.DUPES_AND_NOEXCLUSION:
							gameObject5 = this.shopItems.SelectByWeightNoExclusions(GameManager.Instance.IsSeeded);
							this.m_shopItems.Add(gameObject5);
							break;

						default:
							gameObject5 = this.shopItems.SubshopSelectByWeightWithoutDuplicatesFullPrereqs(this.m_shopItems, weightModifier, 1, GameManager.Instance.IsSeeded);
							this.m_shopItems.Add(gameObject5);
							break;
					}
				}
			}


			m_itemControllers = new List<ShopItemController>();
			for (int m = 0; m < base.spawnPositions.Length; m++)
			{
				Transform transform = base.spawnPositions[m];
				if (!flag && !(base.m_shopItems[m] == null))
				{
					PickupObject component4 = base.m_shopItems[m].GetComponent<PickupObject>();
					if (!(component4 == null))
					{
						GameObject gameObject6 = new GameObject("Shop item " + m.ToString());
						Transform transform2 = gameObject6.transform;
						transform2.parent = transform;
						transform2.localPosition = Vector3.zero;
						EncounterTrackable component5 = component4.GetComponent<EncounterTrackable>();
						if (component5 != null)
						{
							GameManager.Instance.ExtantShopTrackableGuids.Add(component5.EncounterGuid);
						}
						CustomShopItemController shopItemController = gameObject6.AddComponent<CustomShopItemController>();

						this.AssignItemFacing(transform, shopItemController);
						if (base.m_room != null)
						{
							if (!base.m_room.IsRegistered(shopItemController))
							{
								base.m_room.RegisterInteractable(shopItemController);
							}
						}
						else
						{
							if (!RoomHandler.unassignedInteractableObjects.Contains(shopItemController))
							{
								RoomHandler.unassignedInteractableObjects.Add(shopItemController);
							}
						}


						if (this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META && this.ExampleBlueprintPrefab != null)
						{
							GameObject gameObject7 = UnityEngine.Object.Instantiate<GameObject>(this.ExampleBlueprintPrefab, new Vector3(150f, -50f, -100f), Quaternion.identity);
							ItemBlueprintItem component6 = gameObject7.GetComponent<ItemBlueprintItem>();
							EncounterTrackable component7 = gameObject7.GetComponent<EncounterTrackable>();
							component7.journalData.PrimaryDisplayName = component4.encounterTrackable.journalData.PrimaryDisplayName;
							component7.journalData.NotificationPanelDescription = component4.encounterTrackable.journalData.NotificationPanelDescription;
							component7.journalData.AmmonomiconFullEntry = component4.encounterTrackable.journalData.AmmonomiconFullEntry;
							component7.journalData.AmmonomiconSprite = component4.encounterTrackable.journalData.AmmonomiconSprite;
							component7.DoNotificationOnEncounter = false;
							component6.UsesCustomCost = true;
							component6.CustomCost = list[m];
							GungeonFlags saveFlagToSetOnAcquisition = GungeonFlags.NONE;
							for (int n = 0; n < component4.encounterTrackable.prerequisites.Length; n++)
							{
								if (component4.encounterTrackable.prerequisites[n].prerequisiteType == DungeonPrerequisite.PrerequisiteType.FLAG)
								{
									saveFlagToSetOnAcquisition = component4.encounterTrackable.prerequisites[n].saveFlagToCheck;
								}
							}
							component6.SaveFlagToSetOnAcquisition = saveFlagToSetOnAcquisition;
							component6.HologramIconSpriteName = component7.journalData.AmmonomiconSprite;
							shopItemController.CurrencyType = currencyType;
							shopItemController.OverridePrice = (int?)(20 * GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier).RoundToNearest(1));
							if (GameManager.Instance.SecondaryPlayer)
							{
								shopItemController.OverridePrice *= (int?)(GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier).RoundToNearest(1));

							}
							shopItemController.Initialize(component6, this);
							gameObject7.SetActive(false);
						}
						else
						{

							shopItemController.CurrencyType = currencyType;

							shopItemController.customCanBuy += CustomCanBuyMethod;
							shopItemController.customPrice += CustomPriceMethod;
							shopItemController.removeCurrency += RemoveCurrencyMethod;

							shopItemController.OnPurchase += OnPurchaseMethod;
							shopItemController.OnSteal += OnStealMethod;

							shopItemController.customPriceSprite = this.customPriceSprite;
							shopItemController.OverridePrice = 20;
							shopItemController.Initialize(component4, this);
						}

						m_itemControllers.Add(shopItemController);
					}
				}
			}
			bool flag3 = false;
			if (base.shopItemsGroup2 != null && base.spawnPositionsGroup2.Length > 0)
			{

				int count = base.m_shopItems.Count;
				for (int num2 = 0; num2 < base.spawnPositionsGroup2.Length; num2++)
				{
					if (flag)
					{
						base.m_shopItems.Add(null);
					}
					else
					{
						float num3 = base.spawnGroupTwoItem1Chance;
						if (num2 == 1)
						{
							num3 = base.spawnGroupTwoItem2Chance;
						}
						else if (num2 == 2)
						{
							num3 = base.spawnGroupTwoItem3Chance;
						}
						bool isSeeded = GameManager.Instance.IsSeeded;
						if (((!isSeeded) ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) < num3)
						{
							float replaceFirstRewardWithPickup = GameManager.Instance.RewardManager.CurrentRewardData.ReplaceFirstRewardWithPickup;
							if (!flag3 && ((!isSeeded) ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) < replaceFirstRewardWithPickup)
							{
								flag3 = true;
								GameObject gameObject5 = new GameObject();
								switch (this.poolType)
								{
									case (CustomShopController.ShopItemPoolType)ShopItemPoolType.DEFAULT:
										gameObject5 = this.shopItems.SelectByWeightWithoutDuplicates(this.m_shopItems, GameManager.Instance.IsSeeded);
										this.m_shopItems.Add(gameObject5);
										break;
									case (CustomShopController.ShopItemPoolType)ShopItemPoolType.DUPES:
										gameObject5 = this.shopItems.SelectByWeight(GameManager.Instance.IsSeeded);
										this.m_shopItems.Add(gameObject5);
										break;
									case (CustomShopController.ShopItemPoolType)ShopItemPoolType.DUPES_AND_NOEXCLUSION:
										gameObject5 = this.shopItems.SelectByWeightNoExclusions(GameManager.Instance.IsSeeded);
										this.m_shopItems.Add(gameObject5);
										break;
									default:
										gameObject5 = this.shopItems.SubshopSelectByWeightWithoutDuplicatesFullPrereqs(this.m_shopItems, weightModifier, 1, GameManager.Instance.IsSeeded);
										this.m_shopItems.Add(gameObject5);
										break;
								}
							}
							else if (!GameStatsManager.Instance.IsRainbowRun || AllowedToSpawnOnRainbowMode == true)
							{
								GameObject rewardObjectShopStyle2 = GameManager.Instance.RewardManager.GetRewardObjectShopStyle(GameManager.Instance.PrimaryPlayer, false, false, base.m_shopItems);
								base.m_shopItems.Add(rewardObjectShopStyle2);
							}
							else
							{
								base.m_shopItems.Add(null);
							}
						}
						else
						{
							base.m_shopItems.Add(null);
						}
					}
				}
				for (int num4 = 0; num4 < base.spawnPositionsGroup2.Length; num4++)
				{
					Transform transform3 = base.spawnPositionsGroup2[num4];
					if (!flag && !(base.m_shopItems[count + num4] == null))
					{
						PickupObject component8 = base.m_shopItems[count + num4].GetComponent<PickupObject>();
						if (!(component8 == null))
						{
							GameObject gameObject8 = new GameObject("Shop 2 item " + num4.ToString());
							Transform transform4 = gameObject8.transform;
							transform4.parent = transform3;
							transform4.localPosition = Vector3.zero;
							EncounterTrackable component9 = component8.GetComponent<EncounterTrackable>();
							if (component9 != null)
							{
								GameManager.Instance.ExtantShopTrackableGuids.Add(component9.EncounterGuid);
							}
							CustomShopItemController shopItemController2 = gameObject8.AddComponent<CustomShopItemController>();
							shopItemController2.OverridePrice = 20;
							this.AssignItemFacing(transform3, shopItemController2);
							if (!base.m_room.IsRegistered(shopItemController2))
							{
								base.m_room.RegisterInteractable(shopItemController2);
							}

							shopItemController2.Initialize(component8, this);

							m_itemControllers.Add(shopItemController2);
						}
					}
				}
			}
		}

		/*for (int num6 = 0; num6 < m_customShopItemControllers.Count; num6++)
		{

			m_customShopItemControllers[num6].CurrencyType = currencyType;
			m_customShopItemControllers[num6].CustomCanBuy += Balls.CustomCanBuy;
			m_customShopItemControllers[num6].CustomPrice += Balls.CustomPrice;
			m_customShopItemControllers[num6].RemoveCurrency += Balls.RemoveCurrency;
			m_customShopItemControllers[num6].customPriceSprite = this.customPriceSprite;
		}*/
		public GameObject shopNpc;
	}
}
