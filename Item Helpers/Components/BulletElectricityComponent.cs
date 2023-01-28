/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria;
using UnityEngine;

namespace Reload
{
    public class BulletElectricitySynergyComponent : MonoBehaviour
	{
		// Token: 0x0600892E RID: 35118 RVA: 0x0037D278 File Offset: 0x0037B478

		// Token: 0x0600892F RID: 35119 RVA: 0x0037D2D8 File Offset: 0x0037B4D8
		public void Start()
		{
			this.m_proj = base.GetComponent<Projectile>();
			this.targetAtTimeOfStart = AssaultBullets.CurrentTarget;
			MakeChainWithTarget();
			this.m_proj.OnPostUpdate += this.HandleChain;
			
		}

        private void MakeChainWithTarget()
        {
            throw new NotImplementedException();
        }

        private void HandleChain(Projectile obj)
        {
            
        }

        public Projectile m_proj;
		public AIActor targetAtTimeOfStart;

	}
}
*/