using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTStudio
{
	[ExecuteInEditMode]
	public class Layer : MonoBehaviour {

		[HideInInspector]
		public bool isAParallaxLayer = false;
		[HideInInspector]
		public GameObject[] tiles;
		public float alpha = 1;
		public int sortOrder = 0;

		int lastSortOrder = 0;
		float lastAlpha;

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Raises the enable event.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void Awake()
		{
			// Register all my children as pixel objects
			/*if ( Application.isPlaying )
			{
				foreach ( Transform c in transform )
				{
					if ( c.GetComponent<PixelSnapper>() == null )
						c.gameObject.AddComponent<PixelSnapper>();
				}
			}*/
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Use this for initialization
		/// </summary>
		/// ====================================================================================================================================================================== 
		void Start () {
			lastSortOrder = -100;
			FixSortint();

			// make sure the user didnt manually add a parallax to the layer,
			isAParallaxLayer = GetComponent<RTStudio.Parallax>() != null;
		}
	
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void Update () 
		{
			FixAlphas();
			FixSortint();
			lastAlpha = alpha;
			lastSortOrder = sortOrder;

			// continuously check if user added a parallax layer in editor mode
			if ( !Application.isPlaying )
			{
				if ( isAParallaxLayer == false )
					isAParallaxLayer = GetComponent<RTStudio.Parallax>() != null? true : isAParallaxLayer;
			}
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Gets the tile object at location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ====================================================================================================================================================================== 
		
		public GameObject GetObjectAtIndex( int index )
		{
			if ( index >= tiles.Length || index < 0 )
				return null;
			else
				return tiles[index];
		}


		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Go through each SpriteRenderer in the layer and set the alpha to the correct value
		/// </summary>
		/// <param name="force">If set to <c>true</c> force.</param>
		/// ====================================================================================================================================================================== 
		public void FixAlphas(bool force = false)
		{
			// update only if necessary
			if ( lastAlpha == alpha && !force)
				return;

			SpriteRenderer[] sR = GetComponentsInChildren<SpriteRenderer>();
			foreach(SpriteRenderer s in sR )
			{
				s.color = new Color( s.color.r, s.color.g, s.color.b, alpha );
			}
		}

	
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Go through each SpriteRenderer in the layer and set the sorting to the correct value.
		/// </summary>
		/// <param name="force">If set to <c>true</c> force.</param>
		/// ====================================================================================================================================================================== 
		public void FixSortint(bool force = false)
		{
			// update only if necessary
			if ( lastSortOrder == sortOrder && !force)
				return;

			// fix any sorting issues
			SpriteRenderer[] srenderers = GetComponentsInChildren<SpriteRenderer>();
			foreach ( SpriteRenderer r in srenderers )
			{
				r.sortingOrder = sortOrder;
			}

		}

	}
}
