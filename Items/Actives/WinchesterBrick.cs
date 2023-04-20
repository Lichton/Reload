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
    class WinchesterBrick : PlayerItem
    {
        //TODO for todd
        //1. fix the camera bug when you tp to winchester (fixed)
        //2. fix oddity in winbrick sprite (fuxed) (fixed)
        //3. add way to leave - not done
        //4. fix stop on certain room because cant find area, and fix the player being teleportered in a softlock area without access to winchester (maybe get winchester interactable and put us below it) - fixed
        public static void Init()
        {
            string itemName = "Winchester Brick";
            string resourceName = "Reload/Resources/Actives/WinchesterBrick";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<WinchesterBrick>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 850f);
            string shortDesc = "Two Fools a Minute";
            string longDesc = "A brick from Winchester's game that has been sent out to advertise. His name is Larry!\n\n" +
                "Warps you to a Winchester challenge room. Use again to teleport back to where you were.\n\n" +
                "'Nice aim you've got there, shooter! I know a guy who'd be interested in a little target practice, whatdya say?' - Larry Woodplank";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "rld");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = PickupObject.ItemQuality.A;
            item.PlaceItemInAmmonomiconAfterItemById(289);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (!user) { return false; }
            if (user?.CurrentRoom?.area?.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS) { return false; }
            if ( user.IsInCombat | user.IsInMinecart | user.InExitCell) { return false; }
            if (user.CurrentRoom != null && user.CurrentRoom.IsSealed) { return false; }
            if (GameManager.Instance?.CurrentLevelOverrideState == GameManager.LevelOverrideState.RESOURCEFUL_RAT) { return false; }
            for (int k = 0; k < GameManager.Instance.AllPlayers.Length; k++)
            {
                for (int l = 0; l < GameManager.Instance.AllPlayers[k].inventory.AllGuns.Count; l++)
                {
                    if (GameManager.Instance.AllPlayers[k].inventory.AllGuns[l].name.StartsWith("ArtfulDodger"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override void DoEffect(PlayerController user)
        {
            var Room = TryBuildRoom();
            if (LastOwner && LastOwner == GameManager.Instance.PrimaryPlayer)
            {
                TeleportToRoom(user, Room, false);
            }
            else if (LastOwner && LastOwner == GameManager.Instance.SecondaryPlayer)
            {
                TeleportToRoom(user, Room, true);
            }
        }

        public override void AfterCooldownApplied(PlayerController user)
        {
            if (!teleportToWinchester)
            {
                this.remainingDamageCooldown = 0;
            }
        }

        public RoomHandler TryBuildRoom()
        {
            Dungeon dungeon2 = DungeonDatabase.GetOrLoadByName("Base_BulletHell");
            var SelectedPrototypeDungeonRoom = BraveUtility.RandomElement(Prefabs.winchesterrooms);
            var GlitchRoom = MiscUtilityCode.AddCustomRuntimeRoomWithTileSet(dungeon2, SelectedPrototypeDungeonRoom, false, false, true);
            return GlitchRoom;
        }
        public void TeleportToRoom(PlayerController targetPlayer, RoomHandler targetRoom, bool isSecondaryPlayer = false, Vector2? overridePosition = null)
        {
            if (teleportToWinchester)
            {
                RoomNormal = targetPlayer.CenterPosition;
                Vector2 OldPosition = (targetPlayer.transform.position - targetPlayer.CurrentRoom.area.basePosition.ToVector3());
                IntVector2 OldPositionIntVec2 = (targetPlayer.CenterPosition.ToIntVector2() - targetPlayer.CurrentRoom.area.basePosition);
                Vector2 NewPosition = Vector2.zero;

                foreach (TalkDoerLite npc in FindObjectsOfType<TalkDoerLite>())
                {
                    if (npc.GetAbsoluteParentRoom() == targetRoom)
                    {
                        NewPosition = (npc.transform.PositionVector2() - new Vector2(0, 0.5f));
                        break;
                    }
                }
                if (NewPosition != Vector2.zero)
                {
                    
                    teleportToWinchester = false;
                    AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", targetPlayer.gameObject);
                    ItemToolbox.DoConfetti(NewPosition);

                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !isSecondaryPlayer)
                    {
                        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(targetPlayer);
                        if (otherPlayer) { TeleportToRoom(otherPlayer, targetRoom, true); }
                    }
                    targetPlayer.DoVibration(Vibration.Time.Normal, Vibration.Strength.Medium);
                    if (NewPosition != Vector2.zero)
                    {
                        GameManager.Instance.StartCoroutine(HandleTeleportToRoom(targetPlayer, NewPosition));
                    }
                    targetPlayer.specRigidbody.Velocity = Vector2.zero;

                    targetPlayer.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
                    targetRoom.EnsureUpstreamLocksUnlocked();
                    
                }
            }
            else if(RoomNormal != null)
            {
                AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", targetPlayer.gameObject);
                ItemToolbox.DoConfetti(RoomNormal);

                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !isSecondaryPlayer)
                {
                    PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(targetPlayer);
                    if (otherPlayer) { TeleportToRoom(otherPlayer, targetRoom, true); }
                }

                targetPlayer.DoVibration(Vibration.Time.Normal, Vibration.Strength.Medium);
                targetPlayer.specRigidbody.Velocity = Vector2.zero;

                targetPlayer.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
                GameManager.Instance.StartCoroutine(HandleTeleportToRoom(targetPlayer, RoomNormal));
                teleportToWinchester = true;
            }
        }

            private IEnumerator HandleTeleportToRoom(PlayerController targetPlayer, Vector2 targetPoint)
            {
                
                CameraController cameraController = GameManager.Instance.MainCameraController;
                Vector2 offsetVector = (cameraController.transform.position - targetPlayer.transform.position);
                offsetVector -= cameraController.GetAimContribution();
                cameraController.SetManualControl(true, false);
                cameraController.OverridePosition = cameraController.transform.position;
                yield return new WaitForSeconds(0.1f);
                targetPlayer.transform.position = targetPoint;
                targetPlayer.specRigidbody.Reinitialize();
                targetPlayer.specRigidbody.RecheckTriggers = true;
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    cameraController.OverridePosition = cameraController.GetIdealCameraPosition();
                }
                else
                {
                    cameraController.OverridePosition = (targetPoint + offsetVector).ToVector3ZUp(0f);
                }
                targetPlayer.WarpFollowersToPlayer();
                targetPlayer.WarpCompanionsToPlayer(false);

                Pixelator.Instance.MarkOcclusionDirty();
                yield return null;
                cameraController.SetManualControl(false, true);
                yield return new WaitForSeconds(0.15f);
                targetPlayer.DoVibration(Vibration.Time.Normal, Vibration.Strength.Medium);
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(targetPlayer.specRigidbody, null, false);
                
                yield break;
            }
     
        public Vector2 RoomNormal;
        public bool teleportToWinchester = true;
    }


}
