using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Alexandria.ItemAPI;
using UnityEngine;
using System.Collections;
using Alexandria.NPCAPI;

namespace Reload
{
    class Customhelpers
    {
        /// <summary>
        /// Runs when a MajorBreakable is destroyed.
        /// </summary>
        public static Action<MajorBreakable, Vector2> OnMajorBreakableBroken;

        public static void InitHooks()
        {
            new Hook(typeof(MajorBreakable).GetMethod("Break", BindingFlags.Instance | BindingFlags.Public), typeof(Customhelpers).GetMethod("MajorBreakableBreak", BindingFlags.Static | BindingFlags.Public));
        }
        public static void MajorBreakableBreak(Action<MajorBreakable, Vector2> orig, MajorBreakable self, Vector2 direction)
        {
            if (OnMajorBreakableBroken != null) OnMajorBreakableBroken(self, direction);
            orig(self, direction);
        }
    }
}
