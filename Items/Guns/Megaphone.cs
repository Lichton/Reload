using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using Dungeonator;
using System.Collections.Generic;

namespace Reload
{

    public class Megaphone : AdvancedGunBehavior
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Megaphone", "megaphone");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:megaphone", "rld:megaphone");
            var obj = gun.gameObject.AddComponent<Megaphone>();
            obj.preventNormalFireAudio = true;
            obj.overrideNormalFireAudio = "Play_WPN_grenade_blast_01";
            obj.preventNormalReloadAudio = true;
            obj.overrideNormalReloadAudio = "Play_WPN_screechgun_reload_01";
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Shoot Your Voice");
            gun.SetLongDescription("A megaphone that has been modified to produce ear-piercing sound blasts, aside from the normal voice-amplification. \n\n" +
            "Shoots invisible blasts that expand into a soundwave that stuns enemies.\n\n" +
            "'WHAT'S THAT? I CAN'T HEAR YOU THROUGH MY EARPLUGS!' - Parselo, known widely as the Decibel Killer, a serial killer famous for stealing Gundead Military technology to blare his speeches at lethal volume.");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "megaphone_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(17) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered;
            gun.reloadTime = 1f;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 5;
            gun.muzzleFlashEffects = null;
            gun.SetBaseMaxAmmo(100);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.A;
            gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.5f, 0f);
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            projectile.baseData.damage = 8f;
            projectile.baseData.speed *= 2f;
            projectile.transform.parent = gun.barrelOffset;
            
            projectile.StunApplyChance = 0.2f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Microphone Ammo", "Reload/Resources/Guns/Ammo/megaphone_full", "Reload/Resources/Guns/Ammo/megaphone_empty");
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            base.OnReloadPressed(player, gun, bSOMETHING);
            if (gun.ClipShotsRemaining < gun.ClipCapacity && !gun.IsReloading)
            {
                AkSoundEngine.PostEvent("Play_WPN_screechgun_reload_01", base.gameObject);

            }
            if (player.PlayerHasActiveSynergy("Phoney") && gun.ClipShotsRemaining <= 0 && !gun.IsReloading)
            {
                StunwavePlayer(player);
            }

        }

        private void StunwavePlayer(PlayerController player)
        {
            if (player && player.specRigidbody)
            {
                Timers.Add(1.5f - BraveTime.DeltaTime);
                preVWaveDists.Add(0f);
                Vector2 center = player.specRigidbody.UnitCenter;
                Exploder.DoDistortionWave(center, 0.5f, 0.04f, 5f, 1.5f);
                centers.Add(center);
            }
        }

        public override void PostProcessProjectile(Projectile projectile)
        {

            projectile.OnDestruction += this.Stunwave;
        }

        private void Stunwave(Projectile obj)
        {
            if (obj && obj.specRigidbody)
            {
                Timers.Add(1.5f - BraveTime.DeltaTime);
                preVWaveDists.Add(0f);
                Vector2 center = obj.specRigidbody.UnitCenter;
                Exploder.DoDistortionWave(center, 0.5f, 0.04f, 5f, 1.5f);
                centers.Add(center);
            }
        }

        protected override void Update()
        {
            if (gun.CurrentOwner && gun.GunPlayerOwner())
            {
                List<AIActor> activeEnemies = gun.GunPlayerOwner().CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

                if (activeEnemies != null)
                {
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        AIActor aiactor = activeEnemies[i];
                        for (int o = 0; o < Timers.Count; o++)
                        {
                            if (Timers[o] >= 0)
                            {
                                Timers[o] -= BraveTime.DeltaTime;

                                num = BraveMathCollege.LinearToSmoothStepInterpolate(0f, 5f, 1f - Timers[o] / 1.5f);
                                Vector2 unitCenter = aiactor.specRigidbody.GetUnitCenter(ColliderType.HitBox);

                                float num2 = Vector2.Distance(unitCenter, centers[o]);
                                if (num2 >= preVWaveDists[o] - 0.25f && num2 <= num + 0.25f)
                                {
                                    aiactor.behaviorSpeculator.Stun(2f);
                                }
                            }
                            else
                            {
                                Timers.Remove(Timers[o]);
                                centers.Remove(centers[o]);
                                preVWaveDists.Remove(preVWaveDists[o]);
                            }
                        }
                    }
                    for (int i = 0; i < preVWaveDists.Count; i++)
                    {
                        preVWaveDists[i] = num;
                    }

                }
            }
        }

        public List<float> Timers;
        public List<Vector2> centers;
        public List<float> preVWaveDists;
        public float num;
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