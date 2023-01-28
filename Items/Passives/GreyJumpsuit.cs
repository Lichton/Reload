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
using System.Collections.Generic;
using MonoMod.RuntimeDetour;

namespace Reload
{
    class GreyJumpsuit: PassiveItem
    {

        public static void Init()
        {
            string itemName = "Grey Jumpsuit";
            string resourceName = "Reload/Resources/Passives/GreyJumpsuit";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GreyJumpsuit>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Greytide Worldwide";
            string longDesc = "The clothes of an infamous gungeoneer widely believed to have been damned to Bullet Hell for destroying numerous sections of the Gungeon.\n\n" +
                "Destroying furniture and decoratives increases your damage for five seconds, with stacking effects.\n\n" +
                "'Care to explain?' - The Lich";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");

            item.quality = PickupObject.ItemQuality.A;
            item.PlaceItemInAmmonomiconAfterItemById(524);
            GreySuitID = item.PickupObjectId;


            
        }
        static int GreySuitID;

        

        public void JumpsuitModifier(MinorBreakable obj)
        {
            Upgrade(1.025f);
        }



        private static IEnumerator Removestat(StatModifier statModifier, PlayerController player)
        {
            yield return new WaitForSeconds(5);
            player.ownerlessStatModifiers.Remove(statModifier);
            player.stats.RecalculateStats(player, false, false);
            yield break;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            Alexandria.Misc.CustomActions.OnMinorBreakableShattered += JumpsuitModifier;

            Customhelpers.OnMajorBreakableBroken += this.JumpsuitTableMod;
        }

        private void JumpsuitTableMod(MajorBreakable obj, Vector2 direction)
        {
           if(obj.gameObject.name.Contains("Table_Vertical") || obj.gameObject.name.Contains("Table_Horizontal") || obj.gameObject.name.Contains("Table_Vertical_Stone")|| obj.gameObject.name.Contains("Table_Horizontal_Stone") || obj.gameObject.name.Contains("Coffin_Horizontal") || obj.gameObject.name.Contains("Folding_Table_Horizontal") || obj.gameObject.name.Contains("Folding_Table_Horizontal"))
            {
                Upgrade(1.2f);
            }
            
        }

        private void Upgrade(float amounter)
        {
            if (GameManager.Instance.PrimaryPlayer)
            {
                if (GameManager.Instance.PrimaryPlayer.HasPickupID(GreySuitID))
                {
                    PlayerController playerGuy = GameManager.Instance.PrimaryPlayer;
                    StatModifier statModifier = new StatModifier
                    {
                        amount = amounter,
                        statToBoost = PlayerStats.StatType.Damage,
                        modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
                    };
                    playerGuy.ownerlessStatModifiers.Add(statModifier);
                    playerGuy.stats.RecalculateStats(playerGuy, false, false);
                    GameManager.Instance.StartCoroutine(Removestat(statModifier, playerGuy));
                }
            }
            else if (GameManager.Instance.SecondaryPlayer)

            {
                if (GameManager.Instance.SecondaryPlayer)
                {
                    if (GameManager.Instance.SecondaryPlayer.HasPickupID(GreySuitID))
                    {
                        PlayerController playerGuy = GameManager.Instance.SecondaryPlayer;
                        StatModifier statModifier = new StatModifier
                        {
                            amount = 1.025f,
                            statToBoost = PlayerStats.StatType.Damage,
                            modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
                        };
                        playerGuy.ownerlessStatModifiers.Add(statModifier);
                        playerGuy.stats.RecalculateStats(playerGuy, false, false);
                        GameManager.Instance.StartCoroutine(Removestat(statModifier, playerGuy));
                    }
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            Alexandria.Misc.CustomActions.OnMinorBreakableShattered -= JumpsuitModifier;
            Customhelpers.OnMajorBreakableBroken -= this.JumpsuitTableMod;
            return base.Drop(player);
            
        }


        public override void OnDestroy()
        {
            if (Owner)
            {
                Alexandria.Misc.CustomActions.OnMinorBreakableShattered -= JumpsuitModifier;
                Customhelpers.OnMajorBreakableBroken -= this.JumpsuitTableMod;
            }
            base.OnDestroy();
        }
    }
}
