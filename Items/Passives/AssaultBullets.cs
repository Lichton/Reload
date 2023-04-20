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
using System;

namespace Reload
{
    class AssaultBullets : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Assault Bullets"; 
            string resourceName = "Reload/Resources/Passives/AssaultBullets";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AssaultBullets>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "It Never Changes";
            string longDesc = "A distant cousin of the Robot's Battery Bullets.\n\n" +
                "Made by the Treadnaught's Coalition of Arms, these bullets redirect midair to hit your current target.\n\n" +
                "'You LINE UP every bullet in your clip and DEMAND they give you 20, do you hear me?!?' - Gun Tzu, Art of War";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.B;
            item.SetTag("bullet_modifier");
            item.AddToSubShop(ItemBuilder.ShopType.Trorc, 1);
            item.PlaceItemInAmmonomiconAfterItemById(524);
        }

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.DoTheActions;
            base.Pickup(player);
        }

        public int i = 0;
        //Study beam direct code from turret dudes

        private void DoTheActions(Projectile arg1, float arg2)
        {
            
            arg1.OnHitEnemy += this.SelectTarget;
            StartCoroutine(DelayRedirectBullet(arg1));
        }
        private void SelectTarget(Projectile Projectile, SpeculativeRigidbody target, bool unneeded)
        {
            if (target.healthHaver)
            {
                if ((target.healthHaver != CurrentTarget) && target.healthHaver.IsVulnerable == true && target.healthHaver.CanCurrentlyBeKilled)
                {
                    CurrentTarget = target.healthHaver;
                }
            }
        }
        public IEnumerator DelayRedirectBullet(Projectile proj)
        {
            yield return new WaitForSeconds(0.2f);
          
            if (CurrentTarget != null && proj != null)
            {
                //Fix
                var dirVec = CurrentTarget.specRigidbody.sprite.WorldCenter - proj.transform.position.XY();
                proj.SendInDirection(dirVec, false, true);
               // if(Owner.PlayerHasActiveSynergy("Shock & Ow"))
               // {
                 //   proj.gameObject.AddComponent<BulletElectricitySynergyComponent>();
               // }
            }
            yield break;
        }
      
        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.DoTheActions;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.PostProcessProjectile -= this.DoTheActions;
            }
            base.OnDestroy();
        }

        public HealthHaver CurrentTarget = null;
    }
}
