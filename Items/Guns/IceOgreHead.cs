using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using System.Collections.Generic;
using static ItemAPI.Tools;

namespace Reload
{

    public class IceOgreHead : AdvancedGunBehavior
    {
        public bool AlternateFire = true;

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Ice Ogre Head", "ice_ogre_head");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:ice_ogre_head", "rld:ice_ogre_head");
            var obj = gun.gameObject.AddComponent<IceOgreHead>();
            obj.preventNormalFireAudio = true;
            obj.preventNormalReloadAudio = true;

            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Cold Shoulder");
            gun.SetLongDescription("Freezes enemies.\n\n" +
            "It still produces a bitterly cold breath. A favorite of the adventurer Frifle.\n\n" +
            "'Grgh, snowcone taste funny... me not feel so good.' - Ice Ogre");
           
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.  
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "ice_ogre_head_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 4);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            //fix this
            for (int i = 0; i < 6; i++)
            {
                gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(223) as Gun));
            }
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered;
            gun.reloadTime = 0.2f;
            gun.CanReloadNoMatterAmmo = false;
            gun.muzzleFlashEffects = null;
            gun.SetBaseMaxAmmo(700);
            gun.gunClass = GunClass.ICE;
            
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 4;
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            
            gun.barrelOffset.transform.localPosition = new Vector3(0.6f, 0.3f, 0f);
            gun.encounterTrackable.EncounterGuid = "iceogre";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.ammoCost = 1;
                mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = 0.1f;
                mod.angleVariance = 20f;
                mod.numberOfShotsInClip = -1;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 0.2f;
                projectile.baseData.speed *= 0.5f;
                projectile.shouldRotate = true;
                projectile.transform.parent = gun.barrelOffset;
                projectile.AppliesFreeze = true;
                projectile.FreezeApplyChance = 0.2f;
                EasyTrailMisc trail = projectile.gameObject.AddComponent<EasyTrailMisc>();
                trail.TrailPos = projectile.transform.position;
                trail.StartWidth = 0.3f;
                trail.EndWidth = 0;
                trail.LifeTime = 0.1f;
                trail.BaseColor = ExtendedColours.freezeBlue;
                trail.StartColor = ExtendedColours.freezeBlue;
                trail.EndColor = ExtendedColours.freezeBlue;
                projectile.freezeEffect = StaticStatusEffects.frostBulletsEffect;
                if (mod != gun.DefaultModule) { mod.ammoCost = 0; }
            }
            
            
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Icey Ammo", "Reload/Resources/Guns/Ammo/ice_full", "Reload/Resources/Guns/Ammo/ice_empty");
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            switch(UnityEngine.Random.Range(1, 9))
                {
                case 1:
                    projectile.AdditionalScaleMultiplier += 0.05f;
                break;
                case 2:
                    projectile.AdditionalScaleMultiplier += 0.1f;
                    break;
                case 3:
                    projectile.AdditionalScaleMultiplier += 0.15f;
                    break;
                case 4:
                    projectile.AdditionalScaleMultiplier += 0.2f;
                    break;
                case 5:
                    projectile.AdditionalScaleMultiplier -= 0.05f;
                    break;
                case 6:
                    projectile.AdditionalScaleMultiplier -= 0.1f;
                    break;
                case 7:
                    projectile.AdditionalScaleMultiplier -= 0.15f;
                    break;
                case 8:
                    projectile.AdditionalScaleMultiplier -= 0.2f;
                    break;
                
                }
            switch (UnityEngine.Random.Range(1, 9))
            {
                case 1:
                    projectile.baseData.speed += 0.2f;
                    break;
                case 2:
                    projectile.baseData.speed += 0.4f;
                    break;
                case 3:
                    projectile.baseData.speed += 0.6f;
                    break;
                case 4:
                    projectile.baseData.speed += 0.8f;
                    break;
                case 5:
                    projectile.baseData.speed -= 0.2f;
                    break;
                case 6:
                    projectile.baseData.speed -= 0.4f;
                    break;
                case 7:
                    projectile.baseData.speed -= 0.6f;
                    break;
                case 8:
                    projectile.baseData.speed -= 0.8f;
                    break;
                }
            }
        //This block of code allows us to change the reload sounds.

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            if(gun.GetComponent<tk2dSpriteAnimator>().CurrentClip == gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation) && gun.GetComponent<tk2dSpriteAnimator>().CurrentFrame == 0 && AlternateFire == true)
                {
                
                AkSoundEngine.PostEvent("Play_WPN_iceogre_shot_01", base.gameObject); 
                }
            
            else if (gun.GetComponent<tk2dSpriteAnimator>().CurrentClip == gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation) && gun.GetComponent<tk2dSpriteAnimator>().CurrentFrame == 4)
            {
                AkSoundEngine.PostEvent("Play_OBJ_cauldron_splash_01", base.gameObject);
            }

            AlternateFire = !AlternateFire;
        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            base.OnFinishAttack(player, gun);
            gun.Play(gun.idleAnimation);
        }

        //Now add the Tools class to your project.
        //All that's left now is sprite stuff. 
        //Your sprites should be organized, like how you see in the mod folder. 
        //Every gun requires that you have a .json to match the sprites or else the gun won't spawn at all
        //.Json determines the hand sprites for your character. You can make a gun two handed by having both "SecondaryHand" and "PrimaryHand" in the .json file, which can be edited through Notepad or Visual Studios
        //By default this gun is a one-handed weapon
        //If you need a basic two handed .json. Just use the jpxfrd2.json.
        //And finally, don't forget to add your Gun to your ETGModule class!
    }
}