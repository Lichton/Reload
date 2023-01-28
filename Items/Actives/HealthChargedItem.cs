using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Reload
{
    class HealthChargedItem : PlayerItem
    {
        public static void Init()
        {
            string itemName = "ffff";
            string resourceName = "Reload/Resources/Actives/BoxOpen";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<HealthChargedItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = ".. s 2";
            string longDesc = "fff x d";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 500f);
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
        public override bool CanBeUsed(PlayerController user)
        {
            return true;
        }
        public override void DoEffect(PlayerController user)
        {
            
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }


    }
}