using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTStudio
{
	public class TileSet : ScriptableObject {

		public List<Transform> prefabs= new List<Transform>();	// Prefabs that represent the tileset

		[HideInInspector]
		public List<Texture2D> previews= new List<Texture2D>();	// Cached previews for the inspector
	}
}
