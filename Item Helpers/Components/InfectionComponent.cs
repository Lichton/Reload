using System;
using UnityEngine;

namespace Reload
{
    // Token: 0x0200170B RID: 5899
    public class InfectionComponent : MonoBehaviour
	{
		// Token: 0x0600892E RID: 35118 RVA: 0x0037D278 File Offset: 0x0037B478

		// Token: 0x0600892F RID: 35119 RVA: 0x0037D2D8 File Offset: 0x0037B4D8
		public void Start()
		{
			this.preinfected = base.GetComponent<AIActor>();
            preinfected.specRigidbody.OnPreRigidbodyCollision += this.InfectOthers;
		}

		//fix infection not working if they die on first hit
        private void InfectOthers(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			if (myRigidbody != null)
			{
				if(otherRigidbody != null)
				{
					if (otherRigidbody.gameObject.GetComponent<AIActor>() != null)
					{
						AIActor enemy = otherRigidbody.aiActor;
						if (enemy.gameObject.GetComponent<ResurrectionComponent>() == null && enemy.gameObject.GetComponent<InfectionComponent>() == null)
						{
							enemy.gameObject.AddComponent<ResurrectionComponent>();
						}
					}
				}
			}
		}
        public AIActor preinfected;
     
	}
}