using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SaveAPI;
using UnityEngine;
using GungeonAPI;
using Alexandria;
using HutongGames.PlayMaker;
using Alexandria.NPCAPI;
using Dungeonator;

namespace Reload
{
	// Token: 0x02000158 RID: 344
	public class RestockMachine : SimpleInteractable, IPlayerInteractable
	{
		// Token: 0x06000AFB RID: 2811 RVA: 0x00066254 File Offset: 0x00064454
		public void Start()
		{
			shop = base.transform.parent.GetComponent<ReeferShopController>();
			base.gameObject.transform.SetParent(shop.transform);
			base.gameObject.transform.localPosition = new Vector2(2.2f, -0.5f);

			tk2dSprite fatrd = base.gameObject.GetComponent<tk2dSprite>();
			this.m_isToggled = false;
			SpeculativeRigidbody rigidbody = Alexandria.NPCAPI.ShopAPI.GenerateOrAddToRigidBody(base.gameObject, CollisionLayer.HighObstacle, PixelCollider.PixelColliderGeneration.Manual, true, true, true, false, false, false, false, true, new IntVector2(11, 17));
			tk2dSpriteAnimationClip idle = base.gameObject.AddAnimation("idle", "Reload/Resources/NPCs/Allied/Reefer/RestockMachine/Idle", 4, NPCBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.None, DirectionalAnimation.FlipType.None);
			tk2dSpriteAnimationClip fire = base.gameObject.AddAnimation("fire", "Reload/Resources/NPCs/Allied/Reefer/RestockMachine/Fire", 10, NPCBuilder.AnimationType.Hit, DirectionalAnimation.DirectionType.None, DirectionalAnimation.FlipType.None);
			fire.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
			base.spriteAnimator.DefaultClipId = base.spriteAnimator.GetClipIdByName("idle");
			base.spriteAnimator.playAutomatically = true;
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
			this.m_canUse = true;
		}
		public void Interact(PlayerController interactor)
		{
			float price = (10 * interactor.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier)).RoundToNearest(1);
			if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.SecondaryPlayer)
			{
				price *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
				price.RoundToNearest(1);
			}
			if (base.spriteAnimator.IsPlaying("fire"))
			{
				return;
			}
			else if (interactor.carriedConsumables.Currency < price || shop == null)
			{
				AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", base.gameObject);
				FailedRestockPrice();
				return;
			}
			if (interactor.carriedConsumables.Currency >= price && shop.m_itemControllers != null && shop.m_itemControllers.Count > 0)
			{
				foreach (ShopItemController shopItemmm in shop.m_itemControllers)
				{
                    if (shop.m_room.interactableObjects.Contains(shopItemmm))
                    {
						AngryBookGreenClover = true;
                    }

				}
				if (AngryBookGreenClover == true)
				{
					base.spriteAnimator.Play("fire");
					AngryBookGreenClover = false;
					AkSoundEngine.PostEvent("Play_OBJ_item_purchase_01", base.gameObject);
					interactor.carriedConsumables.Currency -= (int)price;

					foreach (ShopItemController shop2 in shop.m_itemControllers)
					{

						CustomShopItemController shopItem = (CustomShopItemController)shop2;
						if (shopItem.item != null && shop.m_room.interactableObjects.Contains(shopItem))
						{
							var pos = shopItem.transform.position;
							Transform transform3 = shopItem.transform.parent;
							//make new item at pos
							

							List<WeightedGameObject> newList = shop.shopItems.GetCompiledRawItems();
							int index = random.Next(shop.shopItems.GetCompiledRawItems().Count);
							PickupObject component8 = newList[index].gameObject.GetComponent<PickupObject>();
							for(i=1; i <= 100; i++)
                            {
								if(component8 != shopItem.item)
                                {
									break;
                                }
                                else
                                {
									index = random.Next(shop.shopItems.GetCompiledRawItems().Count);
								}
                            }
							if (component8 != null)
							{
								i++;
								GameObject gameObject8 = new GameObject("Shop restock item " + i.ToString());
								Transform transform4 = gameObject8.transform;
								transform4.parent = transform3;
								transform4.localPosition = Vector3.zero;
								EncounterTrackable component9 = component8.GetComponent<EncounterTrackable>();
								if (component9 != null)
								{
									GameManager.Instance.ExtantShopTrackableGuids.Add(component9.EncounterGuid);
								}
								CustomShopItemController shopItemController2 = gameObject8.AddComponent<CustomShopItemController>();
								shop.AssignItemFacing(transform4, shopItemController2);
								if (!shop.m_room.IsRegistered(shopItemController2))
								{
									shop.m_room.RegisterInteractable(shopItemController2);
								}
								shopItemController2.CurrencyType = Alexandria.NPCAPI.CustomShopItemController.ShopCurrencyType.COINS;
								shopItemController2.OverridePrice = (int?)(20 * interactor.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier).RoundToNearest(1));
								if (GameManager.Instance.SecondaryPlayer)
								{
									shopItemController2.OverridePrice *= (int?)(GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier).RoundToNearest(1));

								}
								shopItemController2.Initialize(component8, shop);
								restockedList.Add(shopItemController2);
								itemList.Add(shopItem);
							}

						}

					}
				}
                else
                {
					AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", base.gameObject);
					FailedRestockNone();
					return;
				}
				if (itemList != null)
				{
					for (int item = 0; item < itemList.Count; item++)
					{
						shop.m_itemControllers.Remove(itemList[item]);
						shop.PurchaseItem(itemList[item], false, false);
					}
					itemList.Clear();
				}

				if (restockedList != null)
				{
					for (int item = 0; item < restockedList.Count; item++)
					{
						shop.m_itemControllers.Add(restockedList[item]);
					}
					restockedList.Clear();
				}
			}
		}
		bool AngryBookGreenClover = false;

		public int SafeCheckedItemList(List<ShopItemController> lister)
        {
			if(lister != null)
            {
				return lister.Count;
            }
            else
            {
				return 0;
            }
        }
		static System.Random random = new System.Random();

		List<CustomShopItemController> itemList = new List<CustomShopItemController>();

		List<CustomShopItemController> restockedList = new List<CustomShopItemController>();

		int i = 1;

        public void FailedRestockPrice()
        {
			if (shop != null)
			{
			
			}
		}

		public void FailedRestockNone()
		{
			if (shop != null)
			{
				TextBoxManager.ShowTextBox(shop.shopNpc.transform.position + new Vector3(0f, 2.5f), shop.shopNpc.transform, 5f, StringTableManager.GetString("#REEFER_RESTOCKDENY_TALK"), "shopkeep", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
			}
		}

		public float GetDistanceToPoint(Vector2 point)
		{
			return Vector2.Distance(base.specRigidbody.UnitBottomCenter, point);
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}
		public void OnEnteredRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
			Vector3 offset = new Vector3(base.sprite.GetBounds().max.x + 0.1875f, base.sprite.GetBounds().min.y, 0f);
			float price = (10 * interactor.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier)).RoundToNearest(1);
			if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.SecondaryPlayer)
			{
				price *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
				price.RoundToNearest(1);
			}
			string text = "Restock? "+ price + "[sprite \"ui_coin\"]";	
			GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(base.transform, offset, text);
			dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
			componentInChildren.ColorizeSymbols = false;
			componentInChildren.ProcessMarkup = true;


		}

		public ReeferShopController shop;
		public void OnExitRange(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0.05f, SpriteOutlineManager.OutlineType.NORMAL);
			GameUIRoot.Instance.DeregisterDefaultLabel(base.transform);
		}

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;



		}
	}
}
