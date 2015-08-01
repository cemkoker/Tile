
#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace RTStudio.Utils
{
	public class TileMapUtils
	{

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws the grid based on the user settings in inspector.
		/// </summary>
		/// ======================================================================================================================================================================
		public static void DrawGrid(RTStudio.TileMap targetTileMap)
		{
			
			Handles.color = targetTileMap.gridColor;
			
			float horizontal = targetTileMap.mapDimensions.x*2;
			float vertical = targetTileMap.mapDimensions.y*2;
			
			// Determine length of line to draw
			float v_distance = vertical * (targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit)*0.5f;
			float h_distance = horizontal * (targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit)*0.5f;
			
			// Draw full grid
			for (int x = 0; x <= horizontal; ++x)
			{
				if ( x % 2 == 0 ) Handles.color = new Color ( targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, targetTileMap.gridColor.a / 2.0f);
				else Handles.color = new Color ( targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, targetTileMap.gridColor.a / 8.0f);
				
				Vector2 p1 = new Vector2(x * (targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit)*0.5f, 0) + (Vector2)targetTileMap.transform.position;
				Vector2 p2 = new Vector2(x * (targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit)*0.5f, v_distance) + (Vector2)targetTileMap.transform.position;
				Handles.DrawLine(p1, p2);
			}
			
			for (int y = 0; y <= vertical; ++y)
			{
				if ( y % 2 == 0 ) Handles.color = new Color ( targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, targetTileMap.gridColor.a  / 2.0f);
				else Handles.color = new Color ( targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, targetTileMap.gridColor.a / 8.0f);
				
				Vector2 p1 = new Vector2(0, y * (targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit)*0.5f) + (Vector2)targetTileMap.transform.position;
				Vector2 p2 = new Vector2(h_distance, y * (targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit)*0.5f) + (Vector2)targetTileMap.transform.position;
				Handles.DrawLine(p1, p2);
			}

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws the info for the tilemap, cursor location, selected tile and selected layer in the scene view.
		/// </summary>
		/// ====================================================================================================================================================================== 
		public static void DrawInfo(RTStudio.TileMap targetTileMap, int cursorX, int cursorY)
		{
			if (Camera.current == null)
			{
				return;
			}
			
			
			GUIStyle style = new GUIStyle( GUI.skin.box );
			style.normal.textColor = new Color( 0.9f,0.1f,0.1f); 
			style.alignment = TextAnchor.MiddleLeft;
			
			string infoString = "";
			string tileInfo = " | Tile: None";
			
			if ( Camera.current.pixelWidth <= 270 )
				return;
			
			if ( targetTileMap == null  )
				infoString = " Please Specify A Tilemap";
			else if (targetTileMap.tileSet == null )
				infoString = " No Tileset Specified!";
			else if ( targetTileMap.layers == null || targetTileMap.layers.Count == 0 )
			{
				infoString = " No layers have been created! Create a layer before adding tiles";
			}
			else
			{
				if (targetTileMap.tileSet.prefabs.Count <= targetTileMap.selectedTileIndex )
				{
					targetTileMap.selectedTileIndex = targetTileMap.tileSet.prefabs.Count-1;
					return;
				}
				
				if ( targetTileMap.selectedTileIndex < 0 ||  targetTileMap.selectedLayerIndex >= targetTileMap.layers.Count )
					return;
				
				if ( targetTileMap.tileSet.prefabs != null && targetTileMap.tileSet.prefabs [targetTileMap.selectedTileIndex] != null )
				{
					tileInfo =" | Tile: " + targetTileMap.tileSet.prefabs [targetTileMap.selectedTileIndex].name; 
				}
				
				style.normal.textColor = new Color( 1f,1f,1f,0.5f);
				
				if ( targetTileMap.layers == null || targetTileMap.layers.Count <= 0 )
					infoString = " Current Layer: None | " + tileInfo + " | Grid: ("+cursorX+" ,"+cursorY+")";
				else
				{

					if ( targetTileMap.selectedTileIndex >= 0 )
						infoString = " Current Layer: "+ targetTileMap.layers[targetTileMap.selectedLayerIndex].name + tileInfo + " | Grid: ("+cursorX+" ,"+cursorY+")";



				}
			}
			
			Handles.BeginGUI();
			float height = Camera.current.pixelHeight;
			float boxWidth = Camera.current.pixelWidth;
			float boxHeight = 20;
			Rect rect1 = new Rect(0, height - boxHeight, boxWidth, boxHeight);

			
			GUI.Box(rect1, infoString, style);
			Handles.EndGUI();
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws a rect.
		/// </summary>
		/// <param name="targetTileMap">Target tile map.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="sizeX">Size x.</param>
		/// <param name="sizeZ">Size z.</param>
		/// <param name="outline">Outline color</param>
		/// <param name="fill">Fill color</param>
		/// ======================================================================================================================================================================

		public static void DrawRect(RTStudio.TileMap targetTileMap, int x, int y, int sizeX, int sizeZ, Color outline, Color fill)
		{
			Handles.color = Color.white;
			
			var min = new Vector3( (x * targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit) + targetTileMap.transform.position.x,
			                      (y * targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit)  + targetTileMap.transform.position.y,
			                      0 );
			var max = min + new Vector3(sizeX * targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit,
			                            sizeZ * targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit,
			                            0);
			
			Vector3[] rect = new Vector3[4];
			rect[0].Set(min.x,min.y,0);
			rect[1].Set(max.x,min.y,0);
			rect[2].Set(max.x,max.y,0);
			rect[3].Set(min.x,max.y,0);
			Handles.DrawSolidRectangleWithOutline(rect, fill, outline);
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws the mouse selection reticule.
		/// </summary>
		/// <param name="events">Events.</param>
		/// ======================================================================================================================================================================

		public static void DrawMouse (RTStudio.TileMap targetTileMap, int mx, int my )
		{
				
				Rect cursor = new Rect
					(
						targetTileMap.transform.position.x + ( (targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit) * mx ),
						targetTileMap.transform.position.y + ( (targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit) * my ),
						targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit,
						targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit
						);
				
				Color save = Handles.color;
				Handles.color = new Color(targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, 0.65f);
				GUI.color = new Color(targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, 0.65f);
				
				Handles.Label(cursor.position, "("+mx+","+my+")");
				Handles.DrawSolidRectangleWithOutline( RTStudio.Utils.Utils.GetV3FromRect(cursor), 
				                                      new Color(targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, 0.05f) ,
				                                      new Color(targetTileMap.gridColor.r, targetTileMap.gridColor.g, targetTileMap.gridColor.b, 0.25f) );
				Handles.color = save;
				GUI.color = save;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Fixs the tile map.
		/// </summary>
		/// ======================================================================================================================================================================

		public static void FixTileMap( RTStudio.TileMap targetTileMap )
		{
			if ( targetTileMap.layers == null || targetTileMap == null )
				return;
			
			Undo.RecordObject(targetTileMap, null);
			
			foreach ( RTStudio.Layer layer in targetTileMap.layers )
			{
				//Force the layer to 0 0 0
				layer.transform.position = Vector3.zero;
				
				for( int i = 0; i < targetTileMap.mapDimensions.x; i++) 
				{
					for( int j = 0; j < targetTileMap.mapDimensions.y; j++) 
					{
						
						int tileIndex = (int)(i + targetTileMap.mapDimensions.x * j);
						if ( tileIndex < layer.tiles.Length && layer.tiles[ (int)(tileIndex) ] != null )
						{
							Undo.RecordObject(layer.tiles[ (int)(tileIndex) ], null);
							layer.tiles[ (int)(tileIndex) ].transform.localPosition = targetTileMap.GetPosition(tileIndex);
						}
					}
				}
			}
			
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Set the layers to the correct sorting value.
		/// </summary>
		/// <param name="targetTileMap">Target tile map.</param>
		/// ======================================================================================================================================================================

		public static void SortLayers( RTStudio.TileMap targetTileMap )
		{
			if (Application.isPlaying || targetTileMap.layers == null || targetTileMap.layers.Count == 0)
				return;
			
			int i = 0;
			foreach ( RTStudio.Layer l in targetTileMap.layers )
			{
				l.sortOrder = i;
				i++;
			}
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Creates the tile set with path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="useActive">If set to <c>true</c> use active.</param>
		/// ======================================================================================================================================================================

		public static void CreateTileSetWithPath( string path, bool useActive = true )
		{
			var asset = ScriptableObject.CreateInstance<RTStudio.TileSet>();
			var assetPathAndName = path;
			if (string.IsNullOrEmpty(path))
			{
				path = "Assets";
				path += "/RTSTileMap.asset";
				assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
			}
			else if (Path.GetExtension(path) == "" && Selection.activeObject && useActive )
			{
				Debug.Log("Using selection");
				path += "/RTSTileMap.asset";
				assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
			}

			AssetDatabase.CreateAsset(asset, assetPathAndName);
			AssetDatabase.SaveAssets();
			
			if ( useActive )
			{
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = asset;
			}
		
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Creates the tile map.
		/// </summary>
		/// ======================================================================================================================================================================
		[MenuItem("GameObject/Create Other/RTStudio/TileMap")]
		static void CreateTileMap()
		{		
			var obj = new GameObject("RTStudioTileMap");
			obj.AddComponent<RTStudio.TileMap>();
		}

	}

}
#endif