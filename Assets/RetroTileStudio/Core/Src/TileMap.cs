using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTStudio;

namespace RTStudio
{
	public class TileMap : MonoBehaviour {

		// dimensions for the map
		public Vector2 mapDimensions = new Vector2 ( 20, 18 );	

		// dimensions of each tile used on the map
		public Vector2 tileDimensions = new Vector2 ( 8, 8 );

		// sets the color of the grid
		public Color gridColor = Color.green;

		// Is the option to transform the tilemap enabled
		public bool inspectorTransVisible = false;	

		// layers that make up the map
		public List<RTStudio.Layer> layers;

		// current selected tileset
		public TileSet tileSet;

		// selected tile index
		public int selectedTileIndex = 0;

		// selected layer index
		public int selectedLayerIndex = 0;

		[HideInInspector]
		public int tileSetIndex = -1; // tileset index used
		[HideInInspector]
		public int tileLastSetIndex; // tileset used cached previous when switching
		[HideInInspector]
		public int cameraToolIndex; // current camera being used fomapDimensionsr the tools
		[HideInInspector]
		public bool layerOptions = true; // toggle options foldout
        [HideInInspector]
		public bool showTilesMenu = true; // toggle tiles foldout
		[HideInInspector]
		public bool showAdvancedMenu = false; // toggle options foldout
		[HideInInspector]
		public bool showToolsMenu = false; // toggle foldout
        [HideInInspector]
		public bool showEditMenu = true;
		[HideInInspector]
		public float previewSize = 32;	   // preview size for tiles

		private Vector3 cachedTransform;
		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the position (x,y) given a specific index
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="index">Index.</param>
		/// ======================================================================================================================================================================
		public Vector2 GetPosition(int index)
		{
			float x = (int)(index % mapDimensions.x);
			float y = (int)(index / mapDimensions.x);

			x = this.transform.position.x + x * (tileDimensions.x/Settings.staticPixelsPerUnit) + (tileDimensions.x/Settings.staticPixelsPerUnit)/2.0f;
			y = this.transform.position.y + y * (tileDimensions.y/Settings.staticPixelsPerUnit) + (tileDimensions.y/Settings.staticPixelsPerUnit)/2.0f;

			return new Vector2(x , y);
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the object at point in world.
		/// </summary>
		/// <returns>The object at point in world.</returns>
		/// <param name="point">Point.</param>
		/// ======================================================================================================================================================================
		public GameObject GetObjectAtPointInWorld( Vector2 point )
		{
			int cursorX = (int)((point.x - transform.position.x) / (tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit));
			int cursorY = (int)((point.y - transform.position.y) / (tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit));

			GameObject current;
			for( int i = layers.Count-1; i >= 0; i--) 
			{
				current = GetObjectOnLayer( i, cursorX, cursorY );

				if ( current != null )
					return current;
			}

			return null;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the object on layer by name.
		/// </summary>
		/// <returns>The object on layer.</returns>
		/// <param name="layer">Layer name.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ======================================================================================================================================================================
		public GameObject GetObjectOnLayer( string layer, int x, int y )
		{
			foreach (RTStudio.Layer child in transform){
				if (child.name == layer )
				{
					return GetObjectOnLayer( child.sortOrder, x, y );
				}
			}
			return null;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the object on layer.
		/// </summary>
		/// <returns>The object on layer.</returns>
		/// <param name="layer">Layer.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ======================================================================================================================================================================
		public GameObject GetObjectOnLayer( int layer, int x, int y )
		{
			// early exit if out of bounds 
			if ( layer >= layers.Count || layer < 0 )
				return null;

			int index = GetIndexForXY( x, y );
			return layers[layer].GetObjectAtIndex( index );
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the index given x and y coordinates
		/// </summary>
		/// <returns>The index for X.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ======================================================================================================================================================================
		public int GetIndexForXY(int x, int y)
		{
			int tileIndex = (int)(x + (mapDimensions.x * y) );
			return tileIndex;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Lates the update.
		/// </summary>
		/// ======================================================================================================================================================================
		public virtual void LateUpdate()
		{
			// snap entire grid to pixel perfect position and cache. We dont want to do this every frame!!
			// this in theory pixel alignes everything. Admittadly this is not a 100% solution. It works most of the time.
			// The real solution need some tweeks to Unity itself :(
			if ( cachedTransform != transform.position )
			{
				float pixelUnit=(1.0f/(float)RTStudio.Settings.staticPixelsPerUnit);
				float scale=pixelUnit/RTStudio.Settings.bestFit;
				cachedTransform = transform.position;
				transform.position=new Vector3( Mathf.Round((transform.position.x)/scale)*scale, Mathf.Round((transform.position.y)/scale)*scale, transform.position.z);
			}	
		}


		/// ======================================================================================================================================================================
		/// <summary>
		/// Returns true if the point passed in is within the boounds of the tile map
		/// </summary>
		/// <returns><c>true</c>, if on map was pointed, <c>false</c> otherwise.</returns>
		/// <param name="p">P.</param>
		/// ======================================================================================================================================================================
		public bool PointOnMap(Vector2 p)
		{
			float l = transform.position.x;
			float r = l + mapDimensions.x * tileDimensions.x/Settings.staticPixelsPerUnit;
			float b = transform.position.y;
			float t = b + mapDimensions.y * tileDimensions.y/Settings.staticPixelsPerUnit;


			if ( p.x >= l && p.x <= r )
				if ( p.y >= b && p.y <= t )
					return true;

			return false;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the width in.
		/// </summary>
		/// <returns>The width in.</returns>
		/// ======================================================================================================================================================================
		public float GetWidthInPixels()
		{
			return mapDimensions.x*tileDimensions.x;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the height in.
		/// </summary>
		/// <returns>The width in.</returns>
		/// ======================================================================================================================================================================
		public float GetHeightInPixels()
		{
			return mapDimensions.y*tileDimensions.y;
		}


		/// ======================================================================================================================================================================
		/// <summary>
		/// middle point along the Y axis
		/// </summary>
		/// <returns>The middle bounds.</returns>
		/// ======================================================================================================================================================================
		public float GetMiddle()
		{
			float Y = mapDimensions.y/2.0f;		
			return gameObject.transform.position.y + Y;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// center point along the X axis
		/// </summary>
		/// <returns>The center bounds.</returns>
		/// ======================================================================================================================================================================
		public float GetCenter()
		{
			float X = mapDimensions.x/2.0f;		
			return gameObject.transform.position.x + X;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Left Bounds
		/// </summary>
		/// <returns>The left bounds.</returns>
		/// ======================================================================================================================================================================
		public float GetLeftBounds()
		{
			float X = (Settings.gameWidth/tileDimensions.x)/2.0f/RTStudio.Settings.gameScale;		
			return gameObject.transform.position.x + X;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Top Bounds
		/// </summary>
		/// <returns>The top bounds.</returns>
		/// ======================================================================================================================================================================
		public float GetTopBounds()
		{
			float Y = mapDimensions.y - (RTStudio.Settings.gameHeight/tileDimensions.y)/2.0f/RTStudio.Settings.gameScale;
			return gameObject.transform.position.y + Y;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Bottom Bounds
		/// </summary>
		/// <returns>The bottom bounds.</returns>
		/// ======================================================================================================================================================================
		public float GetBottomBounds()
		{
			float Y =  (RTStudio.Settings.gameHeight/tileDimensions.y)/2.0f/RTStudio.Settings.gameScale;
			return gameObject.transform.position.y + Y;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Left Bounds
		/// </summary>
		/// <returns>The left bounds.</returns>
		/// ======================================================================================================================================================================
		public float GetRightBounds()
		{
			float X = this.mapDimensions.x - (RTStudio.Settings.gameWidth/tileDimensions.x)/2.0f/RTStudio.Settings.gameScale;
			return gameObject.transform.position.x + X;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Helper functin to resize the map programatically
		/// </summary>
		/// <param name="v">V.</param>
		/// ======================================================================================================================================================================
		public void Resize( Vector2 v )
		{

			foreach( Layer layer in layers )
			{
				GameObject[] gos = new GameObject[ (int)(v.x * v.y) ];

				for( int i = 0; i < mapDimensions.x; i++) 
				{
					for( int j = 0; j < mapDimensions.y; j++) 
					{
							int tileIndex1 = (int)(i + mapDimensions.x * j);
							int tileIndex2 = (int)(i + v.x * j);

							if ( i < v.x && j < v.y )
							{
								if ( layer.tiles[ (int)(tileIndex1) ] != null && tileIndex2 < gos.Length )
									gos[ (int)(tileIndex2) ] = layer.tiles[ (int)(tileIndex1) ];
							}
							else
							{
								GameObject.DestroyImmediate( layer.tiles[ (int)(tileIndex1) ] );
							}

					}
				}

				layer.tiles = gos;
			}

			mapDimensions = v;
		}

	}
}
