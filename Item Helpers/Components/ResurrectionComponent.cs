using System;
using UnityEngine;

namespace Reload
{
	// Token: 0x0200170B RID: 5899
	public class ResurrectionComponent : MonoBehaviour
	{
		// Token: 0x0600892E RID: 35118 RVA: 0x0037D278 File Offset: 0x0037B478

		// Token: 0x0600892F RID: 35119 RVA: 0x0037D2D8 File Offset: 0x0037B4D8
		public void Start()
		{
			this.preinfected = base.GetComponent<AIActor>();
			preinfected.healthHaver.OnPreDeath += this.CorpseRevive;
			

		}

		

        private void CorpseRevive(Vector2 obj)
        {
			ItemToolbox.SpawnZombieSpent(preinfected, false);
		}
		public AIActor preinfected;

	}

}