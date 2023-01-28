/*using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;

namespace Reload
{
    class StatSyringeInteractable : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Syringe";
            string resourcePath = "Reload/Resources/Pickups/Syringe.png";
            GameObject gameObject = new GameObject(name);
            StatSyringeInteractable item = gameObject.AddComponent<StatSyringeInteractable>();

            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Stat Up";
            string longDesc = "Some side effects may occur.";
            item.SetupItem(shortDesc, longDesc, "rld");
            StatSyringeInteractable.BaseSyringeID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;

        }
        public static int BaseSyringeID;


        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            m_hasBeenPickedUp = true;
            player.BloopItemAboveHead(base.sprite, "");
            AkSoundEngine.PostEvent("Play_ENM_critter_poof_01", base.gameObject);
            string SideEffectName = "NO SIDE EFFECTS!";
            OtherTools.Notify("Injected", SideEffectName, "Reload/Resources/Pickups/Syringe");
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public string CheckIfMultiple(string PickupString, float valueOfPickup)
        {
            string isMult = valueOfPickup >= 1 ? "s, " : ", ";
            return PickupString + isMult;
        }
        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        protected void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

        public void Interact(PlayerController interactor)
        {
       
                if (RoomHandler.unassignedInteractableObjects.Contains(this))
                {
                    RoomHandler.unassignedInteractableObjects.Remove(this);
                }
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
                this.Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}
*/