using SaveAPI;
using System.Collections;
using System.Reflection;
using Alexandria.DungeonAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alexandria.ItemAPI;

namespace Reload
{
    class TaxidermyKit : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Taxidermy Kit";
            string resourceName = "Reload/Resources/Actives/TaxidermyKit";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TaxidermyKit>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 400f);
            string shortDesc = "Remedy The Death Of Thee";
            string longDesc = "A kit full of used needles and stuffing, crafted from the corpses of bullet-kin.\n\n" +
                "Turns enemy corpses into weak mannequin companions.\n\n" +
                "'Why yes, it is rather hard to taxidermy bullet-kin. Did the solid metal not clue you in?' - Dr. Stich";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            item.PlaceItemInAmmonomiconAfterItemById(403);
            item.quality = PickupObject.ItemQuality.D;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            for (int i = 0; i < StaticReferenceManager.AllCorpses.Count; i++)
            {
                GameObject gameObject = StaticReferenceManager.AllCorpses[i];
                if (gameObject && gameObject.GetComponent<tk2dBaseSprite>() && gameObject.transform.position.GetAbsoluteRoom() == user.CurrentRoom)
                {
                    return true;

                }
                continue;
            }
            return false;
        }

        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_ENM_wizard_summon_01", base.gameObject);
            for (int i = 0; i < StaticReferenceManager.AllCorpses.Count; i++)
            {
                GameObject gameObject = StaticReferenceManager.AllCorpses[i];
                if (gameObject && gameObject.GetComponent<tk2dBaseSprite>() && gameObject.transform.position.GetAbsoluteRoom() == user.CurrentRoom)
                {
                    UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.SmallMagicPuffVFX, gameObject.GetComponent<tk2dBaseSprite>().WorldCenter, Quaternion.identity);
                    Spawndummy(gameObject.GetComponent<tk2dBaseSprite>().WorldCenter);
                }
            }
        }

        public void Spawndummy(Vector2 vector)
        {

            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(Manny.guid);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
            CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
            orAddComponent.Initialize(LastOwner);
            if (orAddComponent.specRigidbody)
            {
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
            }
           
        }


    }
}
