using Alexandria.DungeonAPI;
using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using BepInEx;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Ionic.Zip;
using System.IO;
using System.Reflection;
using UnityEngine.Collections;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Dungeonator;
using Brave.BulletScript;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using GungeonAPI;

namespace Reload
{
    
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class ReloadModule : BaseUnityPlugin
    {
        public static ReloadModule instance;
        public static string FilePathAudio;
        public const string GUID = "roundking.etg.reload";
        public const string NAME = "R3L0AD";
        public const string VERSION = "1.0";
        public const string TEXT_COLOR = "#00FFFF";
        public static string FilePath;
        public static AdvancedStringDB Strings;
        public static BaseShopController reeferShop;
        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {     
            Log($"{NAME} has activated, be aware.", TEXT_COLOR);
            FilePath = this.FolderPath() + "/rooms";
            //Sounds

            //Tools and Toolboxes
            Customhelpers.InitHooks();
            ItemAPI.EnemyBuilder.Init();
            ItemAPI.BossBuilder.Init();

            ReloadModule.Strings = new AdvancedStringDB();
            CustomClipAmmoTypeToolbox.Init();
            SaveAPIManager.Setup("rld");
            EasyVFXDatabase.Init();
            VFXToolbox.InitVFX();
            //Status Effects
            PartyStatusEffectSetup.Init();

            //Passives
            MiniMap.Init();
            AssaultBullets.Init();
            Infection.Init();
            Rainbowllets.Init();
            PartyBullets.Init();
            GreyJumpsuit.Init();
            //Syringes
            AccuracySyringe.Init();
            BulletVelocitySyringe.Init();
            CoolnessSyringe.Init();
            CursedSyringe.Init();
            DamageSyringe.Init();
            FiringSpeedSyringe.Init();
            HealthSyringe.Init();
            RangeSyringe.Init();
            ReloadSyringe.Init();
            SpeedSyringe.Init();
            //Actives
            //MunitionsMixer.Init();
            //HealthChargedItem.Init();
            DungeonCore.Init();
            //Guns
            Megaphone.Add();
            IceOgreHead.Add();

            //Synergy
            //SynergyList.MakeSynergies();

            //NPCs
            Reefer.Init();
            //KnightKin.Init();

           

            //BulletBishop.Init();
           //BulletBishopCarpetTop.Add();
            //BulletBishopCarpetLeft.Add();
           //BulletBishopCarpetRight.Add();
           //BulletBishopCarpetBack.Add();
            //FlowInjectionInitialiser.InitializeFlows();
        }

        public static void Log(string text, string color="FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
    }
}
