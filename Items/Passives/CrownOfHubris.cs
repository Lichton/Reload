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
using System.Collections;
using System.Collections.Generic;

namespace Reload
{
    class CrownOfHubris : PassiveItem
    {
        //FixForChallengeModifier
        public static void Init()
        {
            string itemName = "Crown of Hubris";
            string resourceName = "Reload/Resources/Passives/CrownOfHubris";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CrownOfHubris>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "The Pigeon King";
            string longDesc = "A golden crown forged by the blacksmith for the King in Tallow.\n\n" +
                "Blesses the wearer with a great hubris, increasing their damage yet forcing them to leave one foe alive till the end of combat. In challenge mode, however, it does not punish the wearer.\n\n" +
                "'Bwak! Land-bound beasts so pitiful, look up at my form with gazes skyward and learn honour. Bwak!' - The King In Tallow";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.D;
            item.AddPassiveStatModifier(PlayerStats.StatType.Damage, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
        }

        public override void Pickup(PlayerController player)
        {
            player.OnEnteredCombat += this.HandleKing;
            base.Pickup(player);
        }


       
    
     

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat -= this.HandleKing;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Owner.OnEnteredCombat -= this.HandleKing;
            }
            base.OnDestroy();
        }
        private bool IsValidEnemy(AIActor testEnemy)
        {
            return testEnemy && !testEnemy.IsHarmlessEnemy && (testEnemy.healthHaver || !testEnemy.healthHaver.PreventAllDamage) && (!testEnemy.GetComponent<ExplodeOnDeath>() || testEnemy.healthHaver.IsBoss);
        }

        public override void Update()
        {
            base.Update();
            if(actor != null)
            {
                if(actor.GetAbsoluteParentRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear).Count == 1)
                {
                    actor.healthHaver.PreventAllDamage = false;
                    this.m_instanceVFX.GetComponent<tk2dSprite>().SetSprite("lastmanstanding_check_001");
                }
            }
        }
        private void HandleKing()
        {
            if (!ChallengeManager.CHALLENGE_MODE_ACTIVE)
            {

                if (Owner && Owner.CurrentRoom != null && Owner.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    GameObject ChallengeManagerReference = LoadHelper.LoadAssetFromAnywhere<GameObject>("_ChallengeManager");
                    ChallengeModifier LastBulletStandingX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[0].challenge as BestForLastChallengeModifier);
                    List<AIActor> m_activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                    int num = UnityEngine.Random.Range(0, m_activeEnemies.Count);

                    for (int i = 0; i < Owner.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear); i++)
                    {
                        if (i == num)
                        {
                            if (this.IsValidEnemy(m_activeEnemies[i]))
                            {
                                Vector2 v = (!m_activeEnemies[i].sprite) ? Vector2.up : (Vector2.up * (m_activeEnemies[i].sprite.WorldTopCenter.y - m_activeEnemies[i].sprite.WorldBottomCenter.y));
                                GameObject gameObject = m_activeEnemies[i].PlayEffectOnActor(EasyVFXDatabase.LastBulletStandingX, v, true, false, false);
                                Bounds bounds = m_activeEnemies[i].sprite.GetBounds();
                                Vector3 position = m_activeEnemies[i].transform.position + new Vector3((bounds.max.x + bounds.min.x) / 2f, bounds.max.y, 0f).Quantize(0.0625f);
                                position.y = m_activeEnemies[i].transform.position.y + m_activeEnemies[i].sprite.GetUntrimmedBounds().max.y;
                                position.x -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.x;
                                position.y -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.y;
                                gameObject.transform.position = position;
                                m_instanceVFX = gameObject;
                                m_activeEnemies[i].healthHaver.PreventAllDamage = true;
                                actor = m_activeEnemies[i];
                            }
                        }
                        else
                        {
                            num++;
                        }
                    }
                }
            }
        }
            
        
        public AIActor actor;
        public GameObject m_instanceVFX;
    }
}
