using System;
using FullInspector;
using UnityEngine;

namespace Reload
{
	public class BulletBishopBehavior : OverrideBehaviorBase
	{

		public override void Start()
		{
			base.Start();
		}


		public override void Upkeep()
		{
			base.Upkeep();
		}

		// Token: 0x06004AA4 RID: 19108 RVA: 0x001911E0 File Offset: 0x0018F3E0
		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			base.ContinuousUpdate();
            if (IsOnCooldown(m_aiActor))
            {
                cooldown++;
            }
            else if(!IsPlayingAttackAnimation(m_aiAnimator) && !IsPlayingTeleportAnimation(m_aiAnimator) && CooldownComplete(m_aiActor))
            {
                DoAnimConditional(m_aiAnimator);
            }
            if (IsReadyForTeleport(m_aiAnimator, m_aiActor))
            {
				Teleport();
            }
            else if(IsReadyForAttack(m_aiAnimator, m_aiActor))
			{
                ChooseAttack();
            }
			return ContinuousBehaviorResult.Continue;
		}

        private void DoAnimConditional(AIAnimator m_aiAnimator)
        {
            if (bishopCurrentState == bishopStates.BISHOP_GONNATELEPORT)
            {
                m_aiActor.aiAnimator.PlayClip("teleport", 0f);
                bishopCurrentState = bishopStates.BISHOP_DOTELEPORT;
            }
            else if(bishopCurrentState == bishopStates.BISHOP_GONNAATTACK)
            {
                ChooseAttackAnim();
            }
        }

        private void ChooseAttackAnim()
        {
            bishopCurrentState = bishopStates.BISHOP_DOATTACK;
        }

        private void ChooseAttack()
        {
            throw new NotImplementedException();
        }
        private bool IsOnCooldown(AIActor actor)
        {
            if (bishopCurrentState == bishopStates.BISHOP_DIDATTACK || bishopCurrentState == bishopStates.BISHOP_DIDTELEPORT)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
            private bool IsReadyForAttack(AIAnimator aiAnimator, AIActor aiActor)
        {
           if(!IsPlayingAttackAnimation(aiAnimator) && bishopCurrentState == bishopStates.BISHOP_DOATTACK)
            {
				return true;
            }
            else
            {
				return false;
            }
        }

        private bool CooldownComplete(AIActor aiActor)
        {
            if (cooldown >= cooldown_max && !IsOnCooldown(aiActor))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPlayingAttackAnimation(AIAnimator aiAnimator)
        {
            if(m_aiActor.aiAnimator.IsPlaying("attackfront") || m_aiActor.aiAnimator.IsPlaying("attackleft") || m_aiActor.aiAnimator.IsPlaying("attackright") || m_aiActor.aiAnimator.IsPlaying("attackback"))
            {
                    return true;
            }
            else
            {
				return false;
            }
        }

        private bool IsReadyForTeleport(AIAnimator aiAnimator, AIActor aiActor)
        {
            if (!IsPlayingTeleportAnimation(aiAnimator) && bishopCurrentState == bishopStates.BISHOP_DOTELEPORT)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPlayingTeleportAnimation(AIAnimator aiAnimator)
        {
            if (m_aiActor.aiAnimator.IsPlaying("teleport"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Teleport()
        {
            UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, m_aiActor.sprite.WorldBottomCenter, Quaternion.identity);
		    bishopCurrentState = bishopStates.BISHOP_DIDTELEPORT;
            int whichTeleportPad = UnityEngine.Random.Range(1, 5);
            switch(whichTeleportPad)
            {
                case 1:
                    if(UnityEngine.GameObject.Find("bulletpopecarpettop") != null)
                    {
                        var Carpet = UnityEngine.GameObject.Find("bulletpopecarpettop");





                        UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, m_aiActor.sprite.WorldBottomCenter, Quaternion.identity);
                        m_aiActor.aiAnimator.PlayClip("idlefront", 0f);
                    }
                    break;
                case 2:
                    if (UnityEngine.GameObject.Find("bulletpopecarpetleft") != null)
                    {
                        var Carpet = UnityEngine.GameObject.Find("bulletpopecarpetleft");






                        UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, m_aiActor.sprite.WorldBottomCenter, Quaternion.identity);
                        m_aiActor.aiAnimator.PlayClip("idleleft", 0f);
                    }
                    break;
                case 3:
                    if (UnityEngine.GameObject.Find("bulletpopecarpetright") != null)
                    {
                        var Carpet = UnityEngine.GameObject.Find("bulletpopecarpetright");




                        UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, m_aiActor.sprite.WorldBottomCenter, Quaternion.identity);
                        m_aiActor.aiAnimator.PlayClip("idleright", 0f);
                    }
                    break;
                case 4:
                    if (UnityEngine.GameObject.Find("bulletpopecarpetback") != null)
                    {
                        var Carpet = UnityEngine.GameObject.Find("bulletpopecarpetback");
                        




                        UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.IncensePoof, m_aiActor.sprite.WorldBottomCenter, Quaternion.identity);
                        m_aiActor.aiAnimator.PlayClip("idleback", 0f);
                    }
                    break;
            }
        }

       

		public AIActor bishop;
		public bishopStates bishopCurrentState = bishopStates.BISHOP_GONNATELEPORT;
        public int cooldown = 0;
        private int cooldown_max = 240;

        public enum bishopStates
        {
			BISHOP_GONNATELEPORT,
			BISHOP_GONNAATTACK,
            BISHOP_DOATTACK,
            BISHOP_DOTELEPORT,
			BISHOP_DIDTELEPORT,
			BISHOP_DIDATTACK
        }
	}
}