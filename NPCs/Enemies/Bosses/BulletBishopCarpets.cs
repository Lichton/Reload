using System.Collections.Generic;
using UnityEngine;
using Alexandria.DungeonAPI;
using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using BepInEx;
using SaveAPI;
using System;
using System.Linq;
using System.Text;
using GungeonAPI;
using System.Reflection;

namespace Reload
{
	
	public static class BulletBishopCarpetTop
	{
		public static void Add()
		{
			var carpetObj = new GameObject();
			carpetObj.name = "bulletpopecarpettop";
			FakePrefab.MarkAsFakePrefab(carpetObj);
			UnityEngine.Object.DontDestroyOnLoad(carpetObj);
			carpetObj.SetActive(true);
			carpetObj.layer = 20;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("bulletpopecarpettop", carpetObj);
		}
	}

	public static class BulletBishopCarpetLeft
	{
		public static void Add()
		{
			var carpetObj = new GameObject();
			carpetObj.name = "bulletpopecarpetleft";
			FakePrefab.MarkAsFakePrefab(carpetObj);
			UnityEngine.Object.DontDestroyOnLoad(carpetObj);
			carpetObj.SetActive(true);
			carpetObj.layer = 20;
			StaticReferences.customObjects.Add("bulletpopecarpetleft", carpetObj);
		}
	}

	public static class BulletBishopCarpetRight
	{
		public static void Add()
		{
			var carpetObj = new GameObject();
			carpetObj.name = "bulletpopecarpetright";
			FakePrefab.MarkAsFakePrefab(carpetObj);
			UnityEngine.Object.DontDestroyOnLoad(carpetObj);
			carpetObj.SetActive(true);
			carpetObj.layer = 20;
			Alexandria.DungeonAPI.StaticReferences.customObjects.Add("bulletpopecarpetright", carpetObj);
		}
	}

	public static class BulletBishopCarpetBack
	{
		public static void Add()
		{
			var carpetObj = new GameObject();
			carpetObj.name = "bulletpopecarpetback";
			FakePrefab.MarkAsFakePrefab(carpetObj);
			UnityEngine.Object.DontDestroyOnLoad(carpetObj);
			carpetObj.SetActive(true);
			carpetObj.layer = 20;
			Alexandria.DungeonAPI.StaticReferences.customObjects.Add("bulletpopecarpetback", carpetObj);
		}
	}
}