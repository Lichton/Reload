using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Gungeon;

namespace Reload
{
    class SynergyList
    {
        public static void MakeSynergies()
        {
            #region Synergies
            //CustomSynergies.Add("Shock & Ow", new List<string>() { "rld:assault_bullets", "battery_bullets" });
            CustomSynergies.Add("Phoney", new List<string>() {"rld:megaphone", "big_boy"});
            //CustomSynergies.Add("A Song Of Ice & Fire", new List<string>() { "rld:ice_ogre_head", "demon_head" });
            //CustomSynergies.Add("We Need Backup!", new List<string>() { "rld:gungeon_core", "nn:reinforcement_radio" });
            #endregion


        }

        public static void MakeCrossModSynergies()
        {
            #region ModdedSynergies
            if (Game.Items.ContainsID("nn:reinforcement_radio")) 
            {
                CustomSynergies.Add("AIActor.Spawn();", new List<string>() { "rld:gungeon_core", "nn:reinforcement_radio" });
            }
            #endregion
        }

        public static void MakeGunsCooler()
        {
            #region Dual Wielding
            //AddDualWield(IceOgreHead.IceID, 60, "A Song Of Ice & Fire");
            #endregion
        }

        public static void AddDualWield(int gun1, int gun2, string synergy)
        {
            AdvancedDualWieldSynergyProcessor gun1DUAL = (PickupObjectDatabase.GetById(gun1) as Gun).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            gun1DUAL.PartnerGunID = gun2;
            gun1DUAL.SynergyNameToCheck = synergy;
            AdvancedDualWieldSynergyProcessor gun2DUAL = (PickupObjectDatabase.GetById(gun2) as Gun).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            gun2DUAL.PartnerGunID = gun1;
            gun2DUAL.SynergyNameToCheck = synergy;
        }
    }
}