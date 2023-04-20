using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Reload
{
    class WheelOfCommerce : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Wheel Of Commerce";
            string resourceName = "Reload/Resources/Actives/WheelOfCommerce";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<WheelOfCommerce>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "It Must Spin";
            string longDesc = "This artifact was created by Adgun Smith, the Gungeon’s finest robber baron. Its gold plate shines slightly brighter when you pick up casings. \n\n " +
                "Charges based on casings earned. Grants a reward when used.\n\n" +
                "'Why are the Lore Gunjurers decannonizing my tomes? And I by that I mean- WHY ARE THEY SHOOTING THEM OUT OF CANNONS?' - Adgun Smith";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 50f);
            item.consumable = false;
            item.quality = ItemQuality.EXCLUDED;
        }

        public override void Update()
        {
            if (this.m_pickedUp)
            {
                if (this.LastOwner == null)
                {
                    this.LastOwner = base.GetComponentInParent<PlayerController>();
                    
                }
                if (LastOwner)
                {
                    ETGModConsole.Log("testc 1");
                    if (LastOwner.carriedConsumables.Currency > CurrencyOldStored)
                    {
                        ETGModConsole.Log("testc 2");
                        this.ApplyAdditionalDamageCooldown(LastOwner.carriedConsumables.Currency - CurrencyOldStored);
                    }
                }

                CurrencyOldStored = this.LastOwner.carriedConsumables.Currency;
                if (this.IsCurrentlyActive)
                {
                    this.m_activeElapsed += BraveTime.DeltaTime * this.m_adjustedTimeScale;
                    if (!string.IsNullOrEmpty(this.OnActivatedSprite))
                    {
                        base.sprite.SetSprite(this.OnActivatedSprite);
                    }
                }
               
            }
            else
            {
                base.HandlePickupCurseParticles();
                if (!this.m_isBeingEyedByRat && Time.frameCount % 47 == 0 && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
                {
                    GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
                }
            }

            
        }

        int CurrencyOldStored;
        
        public override void DoEffect(PlayerController user)
        {
            
        }
    }
}