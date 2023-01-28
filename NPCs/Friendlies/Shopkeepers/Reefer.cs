using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GungeonAPI;
using System.Reflection;
using Alexandria.NPCAPI;
using Alexandria.DungeonAPI;
using Alexandria.Misc;

namespace Reload
{
    class Reefer
    {
        public static void AddComplex(StringDBTable stringdb, string key, string value)
        {
            StringTableManager.ComplexStringCollection stringCollection = null;
            if (!stringdb.ContainsKey(key))
            {
                stringCollection = new StringTableManager.ComplexStringCollection();
                stringCollection.AddString(value, 1f);
                stringdb[key] = stringCollection;
            }
            else
            {
                stringCollection = (StringTableManager.ComplexStringCollection)stringdb[key];
                stringCollection.AddString(value, 1f);
                stringdb[key] = stringCollection;
            }


        }
        public static void Init()
        {
            List<string> npcIdleSprites = new List<string> { "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_001", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_002", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_003", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_004", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_005", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_006", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_007", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_008", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_009", "Reload/Resources/NPCs/Allied/Reefer/Idle/reefer_idle_010" };

            List<string> npcTalkSprites = new List<string> { "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_001", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_002", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_003", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_004", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_005", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_006", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_007", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_008", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_009", "Reload/Resources/NPCs/Allied/Reefer/Talk/reefer_talk_010"};



            var reeferDrugs = Alexandria.Misc.LootUtility.CreateLootTable();
            reeferDrugs.AddItemToPool(DamageSyringe.DamageUpID); 
            reeferDrugs.AddItemToPool(CursedSyringe.CursedUpID); 
            reeferDrugs.AddItemToPool(CoolnessSyringe.CoolnessID); 
            reeferDrugs.AddItemToPool(BulletVelocitySyringe.VelocityUpID); 
            reeferDrugs.AddItemToPool(AccuracySyringe.AccuracyUpID); 
            reeferDrugs.AddItemToPool(FiringSpeedSyringe.VolleyUpID);
            reeferDrugs.AddItemToPool(HealthSyringe.HealthID); 
            reeferDrugs.AddItemToPool(RangeSyringe.RangeID); 
            reeferDrugs.AddItemToPool(ReloadSyringe.ReloadUpID);
            reeferDrugs.AddItemToPool(SpeedSyringe.SpeedID);
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_GENERIC_TALK", "My stuff's top shelf, kid. Promise.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_GENERIC_TALK", "Kyle, pass me the- oh, it's you.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_GENERIC_TALK", "Heh, who told you my stuff has side effects? The nerve of some people.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_GENERIC_TALK", "Blah blah blah... Wait, did I say that out loud?");

            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOPPER_TALK", "Shut it, I'm trying to light a ciggie.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOPPER_TALK", "I don't feel like blabbering, kid.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOPPER_TALK", "Heh heh heh... heh.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOPPER_TALK", "Did'ya say something, kid? Heh.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOPPER_TALK", "I'm your dealer, not your therapist.");

            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_SHOP_PURCHASED", "Thanks for the casings, kid! Heh heh...");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_SHOP_PURCHASED", "Praying for ya, kid. Hope you're lucky.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_SHOP_PURCHASED", "Enjoy the rush, kid.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_SHOP_PURCHASED", "Heh, been wanting to get that one out of my shop.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_SHOP_PURCHASED", "Can't believe you've got a taste for that junk.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_SHOP_PURCHASED", "Enjoy, that one's 'strawberry' flavoured.");



            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_NO_PURCHASE", "I used to give freebies, but look where that got me.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_NO_PURCHASE", "I can tell you're sober enough to pay.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_NO_PURCHASE", "I aint some sort of half-rate dealer, pay or get out.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_NO_PURCHASE", "I don't give discounts, kid. Tough luck.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_NO_PURCHASE", "Heh, you're kidding, right?");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_NO_PURCHASE", "You're short some casings there, kid.");

            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_INTRO", "Hey kid, want some stats? Money back guaranteed.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_INTRO", "Hey, kid. Wanna buy some stats? Satisfaction guaranteed.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_INTRO", "I've gots lots of stuff for all sorts of experiences, kid. Take your pick.");

            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_ATTACKED_TALK", "Right in the feels, kid.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_ATTACKED_TALK", "Sorry, kid. I'm not ready to die.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_ATTACKED_TALK", "AGH! Heh, just kidding.");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_ATTACKED_TALK", "Hold on a minute, I think a syringe just pricked me.");

            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOLEN_TALK", "Huh? Whuzzat?");
            AddComplex(ETGMod.Databases.Strings.Core, "#REEFER_STOLEN_TALK", "Must've been a hallucination.");

            Reload.ReloadModule.reeferShop = ReeferShopGenerator.SetUpShop(
                          "reefer",
                          "rld",
                          npcIdleSprites,
                          8,
                          npcTalkSprites,
                          8,
                          reeferDrugs,
                          CustomShopItemController.ShopCurrencyType.COINS,
                          "#REEFER_GENERIC_TALK",
                          "#REEFER_STOPPER_TALK",
                          "#REEFER_SHOP_PURCHASED",
                          "#REEFER_NO_PURCHASE",
                          "#REEFER_INTRO",
                          "#REEFER_ATTACKED_TALK",
                          "#REEFER_STOLEN_TALK",
                           new Vector3(0f, 2.5f, 0),
                          new Vector3(1.9375f, 3.4375f, 5.9375f),
                          ShopAPI.VoiceBoxes.MANLY,
                          shopPos6,
                          1f,
                          false,
                          null,
                          null,
                          null,
                          null,
                          null,
                          null,
                          null,
                          null,
                          true,
                          true,
                          "Reload/Resources/NPCs/Allied/Reefer/reefer_carpet_001",
                          null,
                          true,
                          "Reload/Resources/NPCs/Allied/Reefer/reefer_mapicon",
                          true,
                          0.1f,
                          null,
                          2,
                          CustomShopController.ShopItemPoolType.DEFAULT,
                          true
                          //Minor visual bug when added to shop pools, find way to make carpet load on a lower level

                          ).GetComponent<CustomShopController>();

            PrototypeDungeonRoom Mod_Shop_Room = RoomFactory.BuildFromResource("Reload/Resources/Rooms/Contraband.room").room;
            Alexandria.NPCAPI.ShopAPI.RegisterShopRoom(Reload.ReloadModule.reeferShop.gameObject, Mod_Shop_Room, new UnityEngine.Vector2(7.5f, 7f));
        }



    public static Vector3[] shopPos6 = new Vector3[]
        {
           new Vector3(1.125f,  1.8f, 1),
            new Vector3(2.625f, 1.8f, 1),
           new Vector3(4.125f,  1.8f, 1)
        };
    }


}