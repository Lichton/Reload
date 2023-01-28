using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Reload
{
    class PartyStatusEffectSetup
    {
        public static GameObject PartyOverheadVFX;

        public static void Init()
        {
            PartyOverheadVFX = VFXToolbox.CreateOverheadVFX(PartyVFXPaths, "PartyOverhead", 10, 0.125f);
    
            ///every +0.0625f offsets by one pixel in according axis
            GameActorPartyEffect StandParty = StatusEffectHelper.GeneratePartyEffect(5);
            StaticStatusEffects.StandardPartyEffect = StandParty;
        }

        public static List<string> PartyVFXPaths = new List<string>()
        {
            "Reload/Resources/Misc/StatusEffect/Party/partypopper1",
            "Reload/Resources/Misc/StatusEffect/Party/partypopper2",
            "Reload/Resources/Misc/StatusEffect/Party/partypopper3",
            "Reload/Resources/Misc/StatusEffect/Party/partypopper4",
            "Reload/Resources/Misc/StatusEffect/Party/partypopper5",
            "Reload/Resources/Misc/StatusEffect/Party/partypopper6",
            "Reload/Resources/Misc/StatusEffect/Party/partypopper7",
        };
    }
    public class GameActorPartyEffect : GameActorEffect
    {
        public GameActorPartyEffect()
        {
            this.AppliesTint = false;
            this.AppliesDeathTint = false;
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            
            AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", actor.gameObject);
            ItemToolbox.DoConfetti(actor.sprite.WorldBottomLeft);
            UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.ItemPoofVFX, actor.sprite.WorldCenter, Quaternion.identity);
            if (actor.healthHaver.IsBoss)
            {
                actor.healthHaver.ApplyDamage(100, Vector2.zero, "Partying");
            }
            else
            {
                actor.aiActor.EraseFromExistenceWithRewards();
            }
            base.OnEffectRemoved(actor, effectData);
        }

    }
}