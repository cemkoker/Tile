using System.Reflection;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace RTStudio
{

public class EditorCoroutine
{
	public static EditorCoroutine start( IEnumerator _routine )
	{
		EditorCoroutine coroutine = new EditorCoroutine(_routine);
		coroutine.start();
		return coroutine;
	}
	
	readonly IEnumerator routine;
	EditorCoroutine( IEnumerator _routine )
	{
		routine = _routine;
	}
	
	void start()
	{
		//Debug.Log("start");
		EditorApplication.update += update;
	}
	public void stop()
	{
		//Debug.Log("stop");
		EditorApplication.update -= update;
	}
	
	void update()
	{
		/* NOTE: no need to try/catch MoveNext,
		 * if an IEnumerator throws its next iteration returns false.
		 * Also, Unity probably catches when calling EditorApplication.update.
		 */
    
	    //Debug.Log("update");
	    if (!routine.MoveNext())
	    {
	        stop();
	    }
	}
}

[CustomEditor(typeof(RTStudio.TileMap))]
	public class TileMapEditor : Editor {

		// Structure Used For Right Click Menu on tiles
		public struct subMenuOptions
		{
			public int intId;				// unique id
			public string optionId;			// id of option selected
			public Transform tilePrefab;	// tile prefab
		};

		// states of tile editor
		public enum State { Hover, PaintSelectTool, CopySelectTool }

		// Current State
		public State state = State.Hover;

		// variables used for courser and cursor clicked locations. Perhaps i should switch this to vectors?
		private int cursorX;
		private int cursorY;
		private int cursorClickX;
		private int cursorClickZ;

		// list containing all the RetroTileStudio Tilemaps
		// this is scanned and cachd in this variables
		private List<Object> listOfTileAssets;

		// list containing all the cameras in the scene
		// this is scanned and cachd in this variables
		private List<Camera> listOfCameras;

		// flag a delete request
		private bool altClicking;

		// flag alt button down
		private bool altisDown;

		// vector for mouse location
		private Vector2 mouse;
	    
		// cache for icon texture
		private Texture iconLayerTextureimage;

		// cache for icon selected
		private Texture iconSelectedTextureimage;

		// tile map selected
		private RTStudio.TileMap targetTileMap;

		// map dimensions set by user in inspector before update
		private Vector2 newTileMapDimensions;
		// tile dimensions set by user in inspector before update
		private Vector2 newTilesDimensions;

		// List used for layers, this is a Reroder layer
		private ReorderableList layers;

		// serialized ojbect for target
		private SerializedObject targetSerializedObject;	

		// this is used to track the scroll bar location of the tile set
		private Vector2 tileScrollPosition;

		// this is the selected index of the tile in the tileset
		private int selectedTileSet;
				
		// last tool being used by the user is cached in there
		private Tool LastTool = Tool.None;

		// flags if a new layer was created this frame
		private bool justCreatedLayer = false;

		// id of the last layer created
		private int lastLayerCreated = 0;

		// cache for the logo for Retro
		private Texture2D logoOriginal;

		// paint color 
		private Color paintColor = new Color(0, 1, 0, 0.85f);
		private Color paintColorFill = new Color(0, 1, 0, 0.2f);

//		private Color selectColor = new Color(0, 1, 1, 0.85f);
		private Color selectColorFill = new Color(0, 1, 1, 0.2f);

//		private Color pasteColor = new Color(0.5f, 1, 0.7f, 0.85f);
		private Color pasteColorFill = new Color(0.5f, 0.7f, 1, 0.2f);
        
		// delete color 
		//private Color deleteColor = new Color(1, 0, 0, 0.85f);
		private Color deleteColorFill = new Color(1, 0, 0, 0.2f);


		//Constants used for drawing the tiles
		private const float buffer = 50;
		private const float padding = 4;
		private const float heightOfTileBox = 200;

		// used to calculate rows for Tiles
		private int numRows = 0;

		// cache for camera alignment textures
		private Texture LeftAlighimage;
		private Texture CenterAlighimage;
		private Texture RightAlighimage;
		private Texture TopAlighimage;
		private Texture MiddleAlighimage;
		private Texture BottomAlighimage;

		// cache for tool textures
		private Texture paintBrushimage;
		private Texture paintBrushSelectedimage;
		private Texture selectBrushSelectedimage;
		private Texture selectBrushimage;
		private Texture rotateBrushSelectedimage;
		private Texture rotateBrushimage;
		private Texture flipBrushXSelectedimage;
		private Texture flipBrushXimage;
      
		// copy cache
		GameObject[] copyTiles;
		int copySizeX;
		int copySizeY;
        enum BRUSH
		{
			PAINT,
			SELECT,
			ROTATION,
			FLIP,
		};

		private static BRUSH SelectedBrush;

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		public void OnEnable()
		{
			LastTool = Tools.current;
			initialize();
		}
		/// ======================================================================================================================================================================
		/// <summary>
		/// Raises the disable event.
		/// </summary>
		/// ======================================================================================================================================================================
		void OnDisable()
		{
			Tools.current = LastTool;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Initialization this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		private void initialize()
		{
			// get all the Retro Tilemaps
			listOfTileAssets = RTStudio.Utils.Utils.GetAssetsOfType(typeof(RTStudio.TileSet), "asset");

			// get all the cameras
			listOfCameras = new List<Camera>( Camera.allCameras );

			// set target tilemap to the target
			targetTileMap = (RTStudio.TileMap) target;

			// create a nfew serialized object
			targetSerializedObject = new SerializedObject(target);
			targetSerializedObject.Update();

			// set the map dimensions and tile dimensions
			newTileMapDimensions = targetTileMap.mapDimensions;
			newTilesDimensions = targetTileMap.tileDimensions;

			// set up layers and callbacks as necessary
			layers = new ReorderableList(targetSerializedObject, targetSerializedObject.FindProperty("layers"), true, true, true, true);
			layers.drawHeaderCallback = LayerHeaderDelegate;		
			layers.drawElementCallback = LayerDrawLDelegate;
			layers.onRemoveCallback = LayerRemoveDelegate;
			layers.onAddCallback = LayerAddDelegate;
			layers.onReorderCallback = LayerReorderDelegate;
			layers.onSelectCallback = LayerSelectedDelegate;
			layers.onChangedCallback = LayerChangedDelegate;

			// set to first tile set if exists
			if ( listOfTileAssets.Count > 0 && targetTileMap.tileSetIndex == -1)
			{
				targetTileMap.tileSetIndex = 0;
			}

			SelectedBrush = BRUSH.PAINT;


			// select the tileset
			if ( listOfTileAssets.Count > 0 && targetTileMap.tileSetIndex < listOfTileAssets.Count)
				targetTileMap.tileSet = (RTStudio.TileSet)listOfTileAssets[targetTileMap.tileSetIndex];

			// callback when an undo/redo is perfomed
			Undo.undoRedoPerformed += UndoCallback;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Raises the scene GU event.
		/// </summary>
		/// ======================================================================================================================================================================
		void OnSceneGUI()
		{
            
			Event events = Event.current;

			if ( justCreatedLayer )
			{
				justCreatedLayer = false;
				this.targetTileMap.selectedLayerIndex = lastLayerCreated;
			}

	
			RTStudio.Utils.TileMapUtils.DrawGrid( targetTileMap );
			RTStudio.Utils.TileMapUtils.DrawInfo( targetTileMap, cursorX, cursorY);

            if (!targetTileMap.inspectorTransVisible)
				state = UpdateState(events);

			if ( !targetTileMap.inspectorTransVisible && Tools.current != Tool.None )
			{
				LastTool = Tools.current;
				targetTileMap.GetComponent<Transform>().hideFlags = HideFlags.HideInInspector;
				Tools.current = Tool.None;
			}
			else if ( targetTileMap.inspectorTransVisible )
			{
				if (  Tools.current == Tool.None )
					Tools.current = LastTool;
                
                targetTileMap.GetComponent<Transform>().hideFlags = HideFlags.None;
            }

			// Fix parallax preview movements
			//ZeroOutLayerPosition();
            
			SceneView.RepaintAll();
        }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Callback when undo and redo is called
		/// </summary>
		/// ======================================================================================================================================================================
		void UndoCallback()
		{
			Repaint();
			RTStudio.Utils.TileMapUtils.FixTileMap(targetTileMap);
		}

		/// ======================================================================================================================================================================
        /// <summary>
		/// Shorts the cut key switcher.
		/// </summary>
        /// <param name="events">Events.</param>
		/// ======================================================================================================================================================================
        void ShortCutKeySwitcher(Event events)
		{
			if ( events.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R )
			{
				SelectedBrush = BRUSH.ROTATION;
				ZeroOutLayerPosition();
			}
			
			if ( events.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q )
			{
				SelectedBrush = BRUSH.PAINT;
				ZeroOutLayerPosition();
			}
			
			if ( events.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E )
			{
				SelectedBrush = BRUSH.FLIP;
				ZeroOutLayerPosition();
			}
			
			if ( events.type == EventType.KeyDown && Event.current.keyCode == KeyCode.W )
            {
                SelectedBrush = BRUSH.SELECT;
				ZeroOutLayerPosition();
            }
        }
        
        /// ======================================================================================================================================================================
        /// <summary>
		/// Updates the state.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="events">Events.</param>
		/// ======================================================================================================================================================================
		public State UpdateState ( Event events )
		{
			ShortCutKeySwitcher(events);

			if (Tools.current == Tool.View || Event.current.alt || (events.isMouse && events.button > 1) || Camera.current == null || events.type == EventType.ScrollWheel)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				return State.Hover;
			}
            
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
  
			mouse = events.mousePosition;
			mouse = Camera.current.ScreenToWorldPoint(new Vector2(mouse.x, Camera.current.pixelHeight - mouse.y));

			cursorX = (int)((mouse.x - targetTileMap.transform.position.x) / (targetTileMap.tileDimensions.x/RTStudio.Settings.staticPixelsPerUnit));
			cursorY = (int)((mouse.y - targetTileMap.transform.position.y) / (targetTileMap.tileDimensions.y/RTStudio.Settings.staticPixelsPerUnit));

			if ( targetTileMap.PointOnMap(mouse) )
            {
                RTStudio.Utils.TileMapUtils.DrawMouse(targetTileMap, cursorX, cursorY);
			}


			switch (state)
			{
				//Hovering
				case State.Hover:
					return UpdateHoverState( events );

				//Placing
				case State.CopySelectTool:
					return CopyPaintSelectState( events );

				//Placing
				case State.PaintSelectTool:
					return UpdatePaintSelectState( events );
			}
			return state;

		}

		
		/// ======================================================================================================================================================================
		/// <summary>
		/// Updates Selection state.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="events">Events.</param>
		/// ======================================================================================================================================================================
		
		State CopyPaintSelectState( Event events )
		{
			//Get the drag selection
			var x = Mathf.Min(cursorX, cursorClickX);
			var y = Mathf.Min(cursorY, cursorClickZ);
			var sizeX = Mathf.Abs(cursorX - cursorClickX) + 1;
			var sizeY = Mathf.Abs(cursorY - cursorClickZ) + 1;
			
			if ( targetTileMap == null || targetTileMap.tileSet == null )
			{
				HandleUtility.Repaint();
				return state; 
			}

			//Draw the drag selection
			RTStudio.Utils.TileMapUtils.DrawRect(targetTileMap, x, y, sizeX, sizeY, altClicking ? pasteColorFill  : selectColorFill, altClicking ? pasteColorFill : selectColorFill );
			// copy resets position to zero in case of parallaxing
			HandleUtility.Repaint();

			//Finish the drag
			if (events.type == EventType.MouseUp && events.button < 2)
			{
				if (altClicking)
				{
					if (events.button > 0)
					{
						EditorCoroutine.start( PasteCopyRect(x, y));
					}
				}
				else if (events.button == 0)
				{
					ZeroOutLayerPosition();
                    
                    if ( targetTileMap.selectedTileIndex < 0 )
					{
						targetTileMap.selectedTileIndex = 0;
					}
					
					if (targetTileMap.tileSet.prefabs.Count > targetTileMap.selectedTileIndex && targetTileMap.tileSet.prefabs.Count > 0)
					{	
						EditorCoroutine.start( CopyRect(x, y, sizeX, sizeY));
						//EditorCoroutine.start( SetPaintRect(x, y, sizeX, sizeY, targetTileMap.tileSet.prefabs[targetTileMap.selectedTileIndex]) );
					}
				}

				return State.Hover;
			}
			else if ( events.type == EventType.MouseDrag )
			{
				events.Use();
			}



			return state;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Updates Selection state.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="events">Events.</param>
		/// ======================================================================================================================================================================


		State UpdatePaintSelectState( Event events )
		{
			
			//Get the drag selection
			var x = Mathf.Min(cursorX, cursorClickX);
			var y = Mathf.Min(cursorY, cursorClickZ);
			var sizeX = Mathf.Abs(cursorX - cursorClickX) + 1;
			var sizeY = Mathf.Abs(cursorY - cursorClickZ) + 1;
			
			if ( targetTileMap == null || targetTileMap.tileSet == null )
			{
				HandleUtility.Repaint();
				return state; 
			}



			//Draw the drag selection
			RTStudio.Utils.TileMapUtils.DrawRect(targetTileMap, x, y, sizeX, sizeY, altClicking ? deleteColorFill  : paintColor, altClicking ? deleteColorFill : paintColorFill );
			HandleUtility.Repaint();
			//Finish the drag
			if (events.type == EventType.MouseUp && events.button < 2)
			{
				if (altClicking)
				{
					ZeroOutLayerPosition();

					if (events.button > 0)
					{
						EditorCoroutine.start( SetPaintRect(x, y, sizeX, sizeY, null));
					}
				}
				else if (events.button == 0)
				{
					// paint resets position to zero in caase of parallaxing
					ZeroOutLayerPosition();

					if ( targetTileMap.selectedTileIndex < 0 )
					{
						targetTileMap.selectedTileIndex = 0;
					}

					if (targetTileMap.tileSet.prefabs.Count > targetTileMap.selectedTileIndex && targetTileMap.tileSet.prefabs.Count > 0)
					{
						EditorCoroutine.start( SetPaintRect(x, y, sizeX, sizeY, targetTileMap.tileSet.prefabs[targetTileMap.selectedTileIndex]) );
					}
				}
				
				return State.Hover;
			}
			else if ( events.type == EventType.MouseDrag )
			{
				events.Use();
			}

			return state;
		}
		
		/// ======================================================================================================================================================================
		/// <summary>
		/// Updates hover state.
		/// </summary>
		/// <returns>The new state.</returns>
		/// <param name="events">Events.</param>
		/// ======================================================================================================================================================================
		
		State UpdateHoverState( Event events )
		{

			if ( targetTileMap == null || targetTileMap.tileSet == null )
			{
				HandleUtility.Repaint();
				return state; 
			}

            if (events.type == EventType.MouseDown && events.button < 2 && targetTileMap.PointOnMap( mouse ) )
			{
				cursorClickX = cursorX;
				cursorClickZ = cursorY;
                altClicking = events.button > 0;

                switch ( SelectedBrush )
				{
					case BRUSH.ROTATION:
						if ( events.button > 0 )
							SetRotateCC( cursorX, cursorY);
                      	else
                      		SetRotate( cursorX, cursorY);
					break;
					case BRUSH.FLIP:
						if ( events.button > 0 )
							SetFlip( cursorX, cursorY,false);
                        else
							SetFlip( cursorX, cursorY);
                        
                        break;
					case BRUSH.SELECT:
						HandleUtility.Repaint();
					return State.CopySelectTool;

					case BRUSH.PAINT:
						HandleUtility.Repaint();
						return State.PaintSelectTool;
                }
			}
            
            return state;
        }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Flip a object tile
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="flipX">If set to <c>true</c> flip x.</param>
		/// ======================================================================================================================================================================
        
		void SetFlip(int x, int y, bool flipX = true )
		{
	
			int tileIndex = (int)(x + (targetTileMap.mapDimensions.x * y) );
			
			if ( targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex] == null )
			{
				return;
			}

			Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform, "Undo Flip");
			
			Vector3 v = targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform.localScale;
			if ( flipX )
			{
				targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform.localScale = new Vector3 ( -1f*v.x, v.y, v.z) ;
			}
			else
				targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform.localScale = new Vector3 ( v.x, -1f*v.y, v.z) ;
 
        }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Sets the rotate.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
        /// ======================================================================================================================================================================
		void SetRotate(int x, int y)
        {		
		
			int tileIndex = (int)(x + (targetTileMap.mapDimensions.x * y) );
            
			if ( targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex] == null )
			{
				return;
			}

			Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform, "Undo Rotate");
			targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform.Rotate(0,0,-90f);
		

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Rotate Counter Clockwise.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ======================================================================================================================================================================
		void SetRotateCC(int x, int y)
		{		
			int tileIndex = (int)(x + (targetTileMap.mapDimensions.x * y) );
			
			if ( targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex] == null )
			{
				return;
			}

			Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform, "Undo Rotate");
            targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].transform.Rotate(0,0,90f);


        }
        
		/// ======================================================================================================================================================================
		/// <summary>
		/// Sets the copy rect.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="sizeX">Size x.</param>
		/// <param name="sizeY">Size y.</param>
		/// <param name="prefab">Prefab.</param>
		/// ======================================================================================================================================================================
		IEnumerator PasteCopyRect(int x, int y)
		{

			Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex], null); 

			int breakPoint = 0;
			int breakIncrement = 500;
		
			for (int xx = 0; xx < copySizeX; xx++)
			{
				for (int yy = 0; yy <copySizeY; yy++)
				{
					int tileIndex = (int)(xx + (copySizeX * (copySizeY-1-yy) ) );
					GameObject prefab = copyTiles[tileIndex];

					if ( prefab != null )
					{
						SetTile(x + xx, y - yy, prefab.transform, false);
					}

					if ( breakPoint >= breakIncrement )
					{
						breakPoint =0;
						yield return null;
					}
					breakPoint++;
			
				}
			
			}
			
			if ( targetTileMap.layers.Count > targetTileMap.selectedLayerIndex && targetTileMap.layers.Count != 0 )
			{
				targetTileMap.layers[targetTileMap.selectedLayerIndex].FixSortint(true);
				targetTileMap.layers[targetTileMap.selectedLayerIndex].FixAlphas(true);
			}

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Uses the Copy Rect.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="sizeX">Size x.</param>
		/// <param name="sizeY">Size y.</param>
		/// <param name="prefab">Prefab.</param>
		/// ======================================================================================================================================================================
		IEnumerator CopyRect(int x, int y, int sizeX, int sizeY)
		{
			int breakPoint = 0;
			int breakIncrement = 500;

			copySizeX = sizeX;
			copySizeY = sizeY;
			copyTiles = new GameObject[sizeX*sizeY];
					
			for (int xx = 0; xx < copySizeX; xx++)
			{
				for (int yy = 0; yy <copySizeY; yy++)
				{
					int tileIndex = (int)(xx + (copySizeX * yy) );
					copyTiles[tileIndex] = GetTile( x + xx, y + yy);
									
					if ( breakPoint >= breakIncrement )
					{
						breakPoint =0;
						yield return null;
					}
					breakPoint++;
				}

			}

			yield return null;			
		}

        /// ======================================================================================================================================================================
        /// <summary>
		/// Sets the rect.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="sizeX">Size x.</param>
		/// <param name="sizeY">Size y.</param>
		/// <param name="prefab">Prefab.</param>
		/// ======================================================================================================================================================================
		IEnumerator SetPaintRect(int x, int y, int sizeX, int sizeY, Transform prefab)
		{
            
            int breakPoint = 0;
			int breakIncrement = 500;

		
			int count = 0;
            for (int xx = 0; xx < sizeX; xx++)
			{
				for (int yy = 0; yy <sizeY; yy++)
				{

                    SetTile(x + xx, y + yy, prefab);
					count++;
					if ( breakPoint >= breakIncrement )
					{
						breakPoint =0;
						yield return null;
					}
					breakPoint++;
				}
			}

			if ( targetTileMap.layers == null )
				return false;

			if ( targetTileMap.layers.Count > targetTileMap.selectedLayerIndex && targetTileMap.layers.Count != 0 )
			{
				targetTileMap.layers[targetTileMap.selectedLayerIndex].FixSortint(true);
				targetTileMap.layers[targetTileMap.selectedLayerIndex].FixAlphas(true);
			}

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Get the tile on the current layer at X, Y.
		/// </summary>
		/// <returns><c>true</c>, if tile was set, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ======================================================================================================================================================================
		
		GameObject GetTile(int x, int y)
		{
			if ( x < 0 || y < 0 || x >= targetTileMap.mapDimensions.x || y >= targetTileMap.mapDimensions.y )
			{
				return null;
			}

			int tileIndex = (int)(x + (targetTileMap.mapDimensions.x * y) );
            
            if (tileIndex >= 0)
			{
				return targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex];
			}

			return null;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Sets the tile.
		/// </summary>
		/// <returns><c>true</c>, if tile was set, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="prefab">Prefab.</param>
		/// ======================================================================================================================================================================
        
		bool SetTile(int x, int y, Transform prefab, bool isPrefab = true )
		{
			if ( targetTileMap.layers == null || targetTileMap.layers.Count == 0 )
				return false;

			if ( x < 0 || y < 0 || x >= targetTileMap.mapDimensions.x || y >= targetTileMap.mapDimensions.y )
				return false;

			int tileIndex = (int)(x + (targetTileMap.mapDimensions.x * y) );

			Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex], null);             
            
			if (tileIndex >= 0)
			{
				return UpdatePrefab( tileIndex, prefab, isPrefab);
			}
			return false;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Updates the prefab.
		/// </summary>
		/// <returns><c>true</c>, if prefab was updated, <c>false</c> otherwise.</returns>
		/// <param name="tileIndex">Tile index.</param>
		/// <param name="prefab">Prefab.</param>
		/// ======================================================================================================================================================================
        
		bool UpdatePrefab( int tileIndex, Transform prefab, bool isPrefab = true )
		{
			if ( targetTileMap.layers == null || targetTileMap.layers.Count <= 0)
				return false;

			if ( prefab != null )
			{
				if ( targetTileMap.selectedLayerIndex < 0 || 
				    targetTileMap.selectedLayerIndex >= targetTileMap.layers.Count ||
				    targetTileMap.layers[targetTileMap.selectedLayerIndex] == null )
				{
					targetTileMap.selectedLayerIndex = targetTileMap.layers.Count -1;

					if ( targetTileMap.selectedLayerIndex < 0 )
						targetTileMap.selectedLayerIndex = 0;
					return false;
				}

				if ( targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex] != null )
				{

					if ( targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].name == prefab.name )
					{
						return false;
					}
//					Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex], null);                   
//                    Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex], null); 
					GameObject g = targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].gameObject;
					EditorApplication.delayCall += () =>Undo.DestroyObjectImmediate(g);                   
                }

				Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex], null); 
                
				Transform instance = null;
				if ( isPrefab )
					instance = (Transform)PrefabUtility.InstantiatePrefab( prefab );	
				else 
				{
					// get prefab first then copy over rotation and scale unique changes
					instance = (Transform)PrefabUtility.InstantiatePrefab( PrefabUtility.GetPrefabParent( prefab ) );	
					instance.localScale = prefab.lossyScale;
					instance.localRotation = prefab.localRotation;
				}
				if ( instance.GetComponent<PixelSnapper>() == null )
					instance.gameObject.AddComponent<PixelSnapper>();

				instance.parent = targetTileMap.layers[targetTileMap.selectedLayerIndex].transform;
				instance.localPosition = targetTileMap.GetPosition(tileIndex);
                
                targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex] = instance.gameObject;
				Undo.RegisterCreatedObjectUndo(instance.gameObject, "Create Tile Object");
  				return true;
			}
			else
			{
				if ( targetTileMap.selectedLayerIndex < 0 || 
				    targetTileMap.selectedLayerIndex >= targetTileMap.layers.Count ||
				    targetTileMap.layers[targetTileMap.selectedLayerIndex] == null 
				   )
				{
					targetTileMap.selectedLayerIndex = targetTileMap.layers.Count -1;

					if ( targetTileMap.selectedLayerIndex < 0 )
						targetTileMap.selectedLayerIndex = 0;

					return false;
				}

				if ( targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex] != null )
				{
//					Undo.RecordObject(targetTileMap.layers[targetTileMap.selectedLayerIndex], null); 
					GameObject g = targetTileMap.layers[targetTileMap.selectedLayerIndex].tiles[tileIndex].gameObject;
					EditorApplication.delayCall += () =>Undo.DestroyObjectImmediate(g);
                }
			}
			return false;
		}

		/// ====================================================================================================================================================================== 
        /// <summary>
		/// Handles the drag and drop events.
		/// </summary>
		/// <param name="events">Events.</param>
		/// ====================================================================================================================================================================== 
        
        public void HandleDragEvents( Event events )
		{

			// drag and drop go onto tileset
			switch (events.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if (events.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						
						foreach ( var gv in DragAndDrop.objectReferences)
						{
							GameObject g = gv as GameObject;
							if ( PrefabUtility.GetPrefabParent(g) == null && PrefabUtility.GetPrefabObject(g) != null)
							{
								ImportPrefab(g);
							}
							else
							{	
								// Check if its a sprite sheet, if it is then
								// convert every spite to a prefab and add to the tileset
								Texture2D tex = gv as Texture2D;
								if ( tex != null )
								{
									ImportSprites (tex);
                                }
								else
									ImportGameObject(g);
                                
                            }
						}

					}
					
					if ( targetTileMap != null && targetTileMap.tileSet != null )
					{
                        EditorUtility.SetDirty(targetTileMap.tileSet);
					}
			
				}
				break;
			}

		}

		/// ====================================================================================================================================================================== 
        /// <summary>
		/// Imports the prefab.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// ====================================================================================================================================================================== 
        
        void ImportPrefab(GameObject g)
        {
			// already a prefab so just add it
			targetTileMap.tileSet.prefabs.Add (g.transform );

			Texture2D p = null;
			if ( g.GetComponentInChildren<Renderer>() == null && 
			     g.GetComponent<Renderer>() &&
                 g.GetComponentInChildren<SpriteRenderer>() == null && 
			     g.GetComponent<SpriteRenderer>() == null )
			{
				p = Resources.Load("missing", typeof(Texture2D)) as Texture2D;
			}
            
            targetTileMap.tileSet.previews.Insert( targetTileMap.tileSet.prefabs.Count-1, p );
        }
        
		/// ====================================================================================================================================================================== 
        /// <summary>
		/// Imports the game object.
		/// </summary>
		/// <param name="g">The green component.</param>
		/// ====================================================================================================================================================================== 
        
        void ImportGameObject(GameObject g)
		{
			if ( targetTileMap == null )
				Debug.Log("targetTileMap is Null!!, it should not be");

			if ( targetTileMap.tileSet == null )
				Debug.Log("targetTileMap.tileSet is Null!!, it should not be");

			Object parentObject = targetTileMap.tileSet; 
			string path = AssetDatabase.GetAssetPath(parentObject); 
			path = path.Replace( targetTileMap.tileSet.name + ".asset", targetTileMap.tileSet.name + "-" + g.name+".prefab" );

			bool fileAlreadyExists = false;
			int i = 1;
			do 
			{
				if (System.IO.File.Exists(path))
				{
					fileAlreadyExists = true;
					path = AssetDatabase.GetAssetPath(parentObject); 
					path = path.Replace( targetTileMap.tileSet.name + ".asset", targetTileMap.tileSet.name + "-"+ g.name+"_"+i+".prefab" );
				}
				else 
				{
					fileAlreadyExists = false;
				}
				i++;
			}while ( fileAlreadyExists );


			GameObject go = PrefabUtility.CreatePrefab( path, g );
			
			targetTileMap.tileSet.prefabs.Add ( go.transform );


			Texture2D p = null;
			if ( g.GetComponentInChildren<Renderer>() == null && 
			    g.GetComponent<Renderer>() &&
			    g.GetComponentInChildren<SpriteRenderer>() == null && 
			    g.GetComponent<SpriteRenderer>() == null )
            {
                p = Resources.Load("missing", typeof(Texture2D)) as Texture2D;
            }

			targetTileMap.tileSet.previews.Insert( targetTileMap.tileSet.prefabs.Count-1, p );
        }

		/// ====================================================================================================================================================================== 
        /// <summary>
		/// Imports the sprites.
		/// </summary>
		/// <param name="tex">Tex.</param>
		/// ====================================================================================================================================================================== 
        void ImportSprites(Texture2D tex)
        {
            
            List<Sprite> sprites = null;
            
            string path = AssetDatabase.GetAssetPath(tex);
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
			if (assets != null)
			{
				sprites = new List<Sprite>();
				foreach (UnityEngine.Object a in assets)
				{
					if (a is Sprite)
					{
						sprites.Add(a as Sprite);
					}
				}
			}
			
			if (sprites != null)
			{
				GameObject temp_pyx_holder = new GameObject();
				GameObject newTile = new GameObject();
				newTile.transform.parent = temp_pyx_holder.transform;
				SpriteRenderer spriteRenderer = (SpriteRenderer)newTile.AddComponent<SpriteRenderer>();
				int i = 0;
				foreach (Sprite s in sprites)
				{
					EditorUtility.DisplayProgressBar(

						"Processing Sprites",
						"Processing: " + s.name,
						(((float)i)/((float)sprites.Count))

						);
					
					newTile.name = s.name;
					spriteRenderer.sprite = s;
					ImportGameObject ( newTile );
					i++;
		
				}

				EditorUtility.ClearProgressBar();

				DestroyImmediate ( temp_pyx_holder );
			}
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Raises the inspector GUI event.
        /// </summary>
		/// ====================================================================================================================================================================== 

		public override void OnInspectorGUI()
        {   
			targetSerializedObject.Update();

            Event events = Event.current;
            EditorGUILayout.Space();
            
			// Draw the Retro Tile Studio Logo
			RTStudio.Utils.Utils.DrawLogo();

			// Draw the Line
			RTStudio.Utils.Utils.DrawLine();

            ShortCutKeySwitcher( events );

			#if !UNITY_WEBPLAYER 
            if ( !Application.isPlaying && !targetTileMap.inspectorTransVisible )
			{
				// Fix parallax preview movements
				//ZeroOutLayerPosition();

				// Draw Brushes
				DrawBrushes();
				//EditorGUILayout.Space();

				// Draw the Retro Tile Studio Logo
				RTStudio.Utils.Utils.DrawQWER();
                
                // Layers
            	DrawLayerMenu();


				// Layer Options
				string currentLayerName = "Layer Options";
				if ( targetTileMap.layers != null && (targetTileMap.selectedLayerIndex >= 0 &&
				                                      targetTileMap.selectedLayerIndex < targetTileMap.layers.Count ) )
				{
					currentLayerName = "Layer Options [ " + targetTileMap.layers[targetTileMap.selectedLayerIndex].name + " ]";
				}

				Color Save = GUI.contentColor;

				targetTileMap.layerOptions = EditorGUILayout.Foldout(targetTileMap.layerOptions, currentLayerName);

				GUI.contentColor = Save;
				if ( targetTileMap.layerOptions )
				{
					DrawLayerOptions();
				}

				// Draw the Line
				RTStudio.Utils.Utils.DrawLine();
                
				targetTileMap.showTilesMenu = EditorGUILayout.Foldout(targetTileMap.showTilesMenu, "Tiles");
				if ( targetTileMap.showTilesMenu )
				{
					DrawGameObjectTiles();
				}

	            HandleDragEvents(events);
	            
				RTStudio.Utils.TileMapUtils.SortLayers(targetTileMap);
            
	            targetTileMap.showAdvancedMenu = EditorGUILayout.Foldout(targetTileMap.showAdvancedMenu, "Tile Map Options");
				if ( targetTileMap.showAdvancedMenu )
				{
					DrawAdvancedSettings();
		        }

				targetTileMap.showToolsMenu = EditorGUILayout.Foldout(targetTileMap.showToolsMenu, "Camera Alignment");
				if ( targetTileMap.showToolsMenu )
	            {
					DrawCameraHelpers();
	            }
			          
		        if ( GUI.changed ) 
				{
					Repaint();
					EditorUtility.SetDirty(target);
                }
			
	            targetSerializedObject.ApplyModifiedProperties();
			}
			else if ( targetTileMap.inspectorTransVisible )
			{
				EditorGUILayout.HelpBox( "Cannot Edit tile map if translate option is turned on. Disable Translate option to enable edit mode.", MessageType.Info );     
				
				targetTileMap.showAdvancedMenu = EditorGUILayout.Foldout(targetTileMap.showAdvancedMenu, "Options");
				if ( targetTileMap.showAdvancedMenu )
				{
					DrawAdvancedSettings();			
				}
				
				targetTileMap.showToolsMenu = EditorGUILayout.Foldout(targetTileMap.showToolsMenu, "Tools");
				if ( targetTileMap.showToolsMenu )
				{
					DrawCameraHelpers();
				}
				
				
				if ( GUI.changed ) 
				{
					Repaint();
					EditorUtility.SetDirty(target);
				}
				
				targetSerializedObject.ApplyModifiedProperties();
			}
			else
			{
				EditorGUILayout.HelpBox( "Editor is in Play mode. Editing is disabled.", MessageType.Info );      
            }
			#endif

			#if UNITY_WEBPLAYER
			EditorGUILayout.HelpBox( "Editor target platform is set to WebPlayer. Please switch to Standalone in order to edit the tilemap.", MessageType.Info );      
			#endif

	    }

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Calls the sub menu function.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// ====================================================================================================================================================================== 

		void CallBackSubMenuFunction (object obj) {
			subMenuOptions objectOpt = (subMenuOptions)obj;
			
			if ( objectOpt.optionId == "Select" )
			{
				targetTileMap.selectedTileIndex = objectOpt.intId;
			}
			else if ( objectOpt.optionId == "Prefab" ) 
			{
				Selection.activeObject = objectOpt.tilePrefab;
			}
			else if ( objectOpt.optionId == "Remove" ) 
			{
				targetTileMap.tileSet.previews.RemoveAt( objectOpt.intId );
                targetTileMap.tileSet.prefabs.Remove( objectOpt.tilePrefab );
				EditorUtility.SetDirty(targetTileMap.tileSet);
            }
            
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the tile sets drop down menu for inspector.
		/// </summary>
		/// ====================================================================================================================================================================== 

		void DrawTileSetsDropDownMenu ()
		{
			//Begin Check for change
			EditorGUI.BeginChangeCheck();

			string[] listFiles = new string[listOfTileAssets.Count+1];
			for (int i = 0; i < listOfTileAssets.Count; ++i)
			{
				if ( listOfTileAssets[i] != null )
					listFiles[i] =  listOfTileAssets[i].name;
			}
			listFiles[listOfTileAssets.Count]= "Create a New Tile Set";
			targetTileMap.tileLastSetIndex = targetTileMap.tileSetIndex;
			targetTileMap.tileSetIndex = EditorGUILayout.Popup(targetTileMap.tileSetIndex, listFiles);

			// catch change 
			if (EditorGUI.EndChangeCheck())
			{
				
				if ( targetTileMap.tileSetIndex == listOfTileAssets.Count)
				{
					targetTileMap.tileSetIndex = targetTileMap.tileLastSetIndex;
					string path = EditorUtility.SaveFilePanelInProject("Save New Tileset", "RTSTileSet.asset", "asset", "Please select file name to save tileset asset to:");
					if ( !string.IsNullOrEmpty(path) )
					{
						RTStudio.Utils.TileMapUtils.CreateTileSetWithPath( path, false );
						listOfTileAssets = RTStudio.Utils.Utils.GetAssetsOfType(typeof(RTStudio.TileSet), "asset");
						targetTileMap.tileSetIndex = listOfTileAssets.Count-1;
					}
				}
				else if ( targetTileMap.tileSetIndex > listOfTileAssets.Count)
				{
					targetTileMap.tileSetIndex = 0;
					targetTileMap.tileSet = (RTStudio.TileSet)listOfTileAssets[targetTileMap.tileSetIndex];
				}
				else
					targetTileMap.tileSet = (RTStudio.TileSet)listOfTileAssets[targetTileMap.tileSetIndex];
				
			}

		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the tile sets drop down menu for inspector.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void DrawCreateNewTilesetButton ()
		{
			targetTileMap.tileSetIndex = 0;
			if(GUILayout.Button("Create TileSet"))
			{
				string path = EditorUtility.SaveFilePanelInProject("Save New Tileset", "RTSTileSet.asset", "asset", "Please select file name to save asset to:");
				
				if ( !string.IsNullOrEmpty(path) )
				{
					RTStudio.Utils.TileMapUtils.CreateTileSetWithPath( path, false );
					listOfTileAssets = RTStudio.Utils.Utils.GetAssetsOfType(typeof(RTStudio.TileSet), "asset");
					targetTileMap.tileSetIndex = listOfTileAssets.Count-1;
				}
			}
		}
	
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Helper funtion to Draws the empty tile set.
		/// </summary>
		/// <param name="boxRect">Box rect.</param>
		/// ====================================================================================================================================================================== 

		void DrawEmptyTileSet(Rect boxRect)
		{
			GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			Color saveColor = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = Color.gray;
			GUI.Label(new Rect(boxRect.x, boxRect.y,boxRect.width, boxRect.height ), 
			          "Drag & drop prefabs, gameObjects \n or sprites here.", centeredStyle);
			GUI.skin.label.normal.textColor = saveColor;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the right click tile menu.
		/// </summary>
		/// <param name="index">Index.</param>
		/// ====================================================================================================================================================================== 

		void DrawRightClickTileMenu(int index)
		{
			GenericMenu menu  = new GenericMenu ();
			
			subMenuOptions selectTile = new subMenuOptions();
			subMenuOptions selectPrefab = new subMenuOptions();
			subMenuOptions removeTile = new subMenuOptions();
			
			selectTile.optionId = "Select";
			selectPrefab.optionId = "Prefab";
			removeTile.optionId = "Remove";
			
			selectTile.tilePrefab = targetTileMap.tileSet.prefabs[index];
			selectPrefab.tilePrefab = targetTileMap.tileSet.prefabs[index];
			removeTile.tilePrefab = targetTileMap.tileSet.prefabs[index];
			
			selectTile.intId = index;
			selectPrefab.intId = index;
			removeTile.intId = index;
			
			menu.AddItem( (new GUIContent ("Select Tile")), false, CallBackSubMenuFunction, selectTile );
			menu.AddItem( (new GUIContent ("Select Prefab")), false, CallBackSubMenuFunction, selectPrefab );
			menu.AddSeparator ("");
			menu.AddItem( (new GUIContent ("Remove Tile")), false, CallBackSubMenuFunction, removeTile );
			menu.AddSeparator ("");
			menu.ShowAsContext ();
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Helper funtion to Draws all the tiles in the tileset
		/// </summary>
		/// <param name="boxRect">Box rect.</param>
		/// ====================================================================================================================================================================== 
		
		void DrawTileSet(Rect boxRect)
		{
			float tileBoxSize = targetTileMap.previewSize;
			
			int numCols = (int)((Screen.width - buffer ) / (tileBoxSize + padding));
			if (targetTileMap.tileSet != null)
			{
				numRows = targetTileMap.tileSet.prefabs.Count / numCols;
				if (numRows * numCols < targetTileMap.tileSet.prefabs.Count)
				{
					numRows += 1;
				}
			}
			
			tileScrollPosition = GUI.BeginScrollView( boxRect, tileScrollPosition, new Rect(0, 0, boxRect.width - buffer,  padding + numRows * (tileBoxSize + padding)) );
			
			float x = padding;
			float y = padding;
			int count = 0;

			for (int row = 0; row < numRows; ++row)
			{
				x = padding;
				GUILayout.BeginHorizontal();
				
				
				for (int col = 0; col < numCols; ++col)
				{
					int index = col + row * numCols;
					if (index >= targetTileMap.tileSet.prefabs.Count)
					{
						break;
					}
					
					// Create a preview and cache it
					if ( targetTileMap.tileSet.previews[index] == null)
					{
						Texture2D p = RTStudio.Utils.Utils.CreateTexture( (AssetPreview.GetAssetPreview( targetTileMap.tileSet.prefabs[index].gameObject )) );						
						targetTileMap.tileSet.previews[index] = p;
					}
					
					// double check the cache exists, sometimes null in its doesnt create the preview right away
					if ( targetTileMap.tileSet.previews[index] == null)
					{
						continue;
					}
					
					Rect r = new Rect(x, y, tileBoxSize, tileBoxSize);
					x += (tileBoxSize + padding);
					
					Color saved = GUI.color;    
					
					
					GUI.color = new Color( Color.white.r,Color.white.g,Color.white.b,Color.white.a );

					// check if the prefab was deleted
					if ( targetTileMap.tileSet.prefabs[index] == null )
					{
						// Removed Orphined Tile
						Debug.Log("Removed Orphined Tile");
						targetTileMap.tileSet.previews.RemoveAt(index);
						targetTileMap.tileSet.prefabs.RemoveAt(index);
					}

					if (GUI.Button( r, new GUIContent(targetTileMap.tileSet.previews[index], "" + targetTileMap.tileSet.prefabs[index].name ) , GUIStyle.none ))
					{
						if (Event.current.button == 0)
							targetTileMap.selectedTileIndex = index;
						else if (Event.current.button == 1)
						{
							DrawRightClickTileMenu(index);
						}
					}
					
					if ( targetTileMap.selectedTileIndex == count )
					{
						GUIStyle style = new GUIStyle( GUI.skin.box );

						GUI.color = new Color( 0,Color.white.g,0,Color.white.a/2 );	
						Rect r2 = new Rect(r.x-3, r.y-3, r.width+6, r.height+6);
						GUI.Box(r2, targetTileMap.tileSet.previews[index], style);
					}
					
					GUI.color = saved;
					GUI.backgroundColor =saved;  
					count++;
				}
				GUILayout.EndHorizontal();
				y += (tileBoxSize + padding);
				
			}		
			
			GUI.EndScrollView();

			GUILayout.BeginHorizontal();

			targetTileMap.previewSize = EditorGUILayout.Slider( targetTileMap.previewSize, 32, 128 );

			GUILayout.EndHorizontal();

			GUI.skin = null;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the tiles in the inspector
		/// </summary>
		/// ====================================================================================================================================================================== 

		void DrawGameObjectTiles()
		{

			if ( listOfTileAssets.Count != 0 )
			{
				DrawTileSetsDropDownMenu ();
			}
			else
			{
				DrawCreateNewTilesetButton ();
			}

			if ( listOfTileAssets.Count == 0 )
			{
				return;
			}

			GUILayout.BeginVertical(GUI.skin.box);
			Rect tileBoxRect = GUILayoutUtility.GetRect(Screen.width - buffer, heightOfTileBox);
			GUILayout.EndVertical();
		

			if ( 
			    targetTileMap.tileSet == null || 
			    targetTileMap.tileSet.prefabs.Count == 0
			   )
			{
				DrawEmptyTileSet(tileBoxRect);
			}
			else
			{
				DrawTileSet(tileBoxRect);
            }
         
        }

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the brushes.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void DrawBrushes()
		{
			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();



			if (!paintBrushimage)
				paintBrushimage = Resources.Load("paintTool", typeof(Texture)) as Texture;

			if (!paintBrushSelectedimage)
				paintBrushSelectedimage = Resources.Load("paintToolSelected", typeof(Texture)) as Texture;
    
			if (!selectBrushimage)
				selectBrushimage = Resources.Load("selectionTool", typeof(Texture)) as Texture;

			if (!selectBrushSelectedimage)
				selectBrushSelectedimage = Resources.Load("selectionToolSelected", typeof(Texture)) as Texture;

			if (!rotateBrushimage)
				rotateBrushimage = Resources.Load("rotationTool", typeof(Texture)) as Texture;
		
			if (!rotateBrushSelectedimage)
				rotateBrushSelectedimage = Resources.Load("rotationToolSelected", typeof(Texture)) as Texture;
            
			if (!flipBrushXimage)
				flipBrushXimage = Resources.Load("flipToolX", typeof(Texture)) as Texture;
            
            if (!flipBrushXSelectedimage)
				flipBrushXSelectedimage = Resources.Load("flipToolXSelected", typeof(Texture)) as Texture;
                   

            GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);

            Texture PaintTexture = SelectedBrush == BRUSH.PAINT? paintBrushSelectedimage : paintBrushimage;
			Texture SelectTexture = SelectedBrush == BRUSH.SELECT? selectBrushSelectedimage : selectBrushimage;
			Texture RotateTexture = SelectedBrush == BRUSH.ROTATION? rotateBrushSelectedimage : rotateBrushimage;
			Texture FlipTexture = SelectedBrush == BRUSH.FLIP? flipBrushXSelectedimage : flipBrushXimage;
            
            
            if( GUILayout.Button( new GUIContent(PaintTexture, "Paint Brush"), buttonStyle, 
			                     GUILayout.Width(PaintTexture.width*1.5f), GUILayout.Height(PaintTexture.height*1.5f) ) )
			{
				SelectedBrush = BRUSH.PAINT;
				ZeroOutLayerPosition();
			}		

			if( GUILayout.Button( new GUIContent(SelectTexture, "Selection Brush"), buttonStyle, 
			                     GUILayout.Width(SelectTexture.width*1.5f), GUILayout.Height(SelectTexture.height*1.5f) ) )
			{
				SelectedBrush = BRUSH.SELECT;
				ZeroOutLayerPosition();
            }	

			if( GUILayout.Button( new GUIContent(FlipTexture, "Flip Brush"), buttonStyle, 
			                     GUILayout.Width(FlipTexture.width*1.5f), GUILayout.Height(FlipTexture.height*1.5f) ) )
			{
				SelectedBrush = BRUSH.FLIP;
				ZeroOutLayerPosition();
            }		

			if( GUILayout.Button( new GUIContent(RotateTexture, "Rotation Brush"), buttonStyle, 
			                     GUILayout.Width(RotateTexture.width*1.5f), GUILayout.Height(RotateTexture.height*1.5f) ) )
			{
				SelectedBrush = BRUSH.ROTATION;
				ZeroOutLayerPosition();
            }		


			            
			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();


		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the layer menu.
		/// </summary>
		/// ====================================================================================================================================================================== 

		void DrawLayerMenu()
		{
			layers.DoLayoutList();

		}

		void DrawLayerOptions()
		{

			// Grab the latest list of cameras
			listOfCameras = new List<Camera>( Camera.allCameras );
		
			if ( targetTileMap.layers == null )
			{
				EditorGUILayout.HelpBox( "You Have Not Created A Layer. Add One To The Tilemap By Clicing The '+' Button Above." , MessageType.Info );
			}
			else if ( targetTileMap.layers != null && (targetTileMap.selectedLayerIndex < 0 || targetTileMap.selectedLayerIndex >= targetTileMap.layers.Count ) )
			{
				EditorGUILayout.HelpBox( "No Layers Selected" , MessageType.Info );
			}
			else
			{
				RTStudio.Layer currentLayer = targetTileMap.layers[targetTileMap.selectedLayerIndex];
				RTStudio.Parallax parallax = targetTileMap.layers[targetTileMap.selectedLayerIndex].GetComponent<RTStudio.Parallax>();
				EditorGUI.indentLevel++;

				EditorGUI.BeginChangeCheck();

				currentLayer.isAParallaxLayer = EditorGUILayout.ToggleLeft("Parallax Enabled", targetTileMap.layers[targetTileMap.selectedLayerIndex].isAParallaxLayer );

				if (EditorGUI.EndChangeCheck())
				{
					if ( currentLayer.isAParallaxLayer && parallax == null )
					{
						parallax = currentLayer.gameObject.AddComponent<RTStudio.Parallax>();
					}
					else if ( parallax && !currentLayer.isAParallaxLayer  )
					{
						DestroyImmediate ( parallax );;
					}
				}


				if ( currentLayer.isAParallaxLayer && parallax != null )
				{

					parallax.parallaxType = (Parallax.ParallaxTypes)EditorGUILayout.EnumPopup( parallax.parallaxType );

					if ( parallax.parallaxType == Parallax.ParallaxTypes.Equally )
					{
						parallax.parallax =EditorGUILayout.FloatField( "Parallax", parallax.parallax );

						if (parallax.lockedAxes == null )
							parallax.lockedAxes = new Parallax.Locks();

						parallax.lockedAxes.xLocked = EditorGUILayout.Toggle ( "X Locked", parallax.lockedAxes.xLocked );
						parallax.lockedAxes.yLocked = EditorGUILayout.Toggle ( "Y Locked",parallax.lockedAxes.yLocked );
					}
					else
					{

						if (parallax.parallaxAxisAmt == null )
							parallax.parallaxAxisAmt = new Parallax.ParallaxPerAxisAmount();

						parallax.parallaxAxisAmt.x =EditorGUILayout.FloatField( "X Parallax", parallax.parallaxAxisAmt.x );
						parallax.parallaxAxisAmt.y =EditorGUILayout.FloatField( "Y Parallax", parallax.parallaxAxisAmt.y );
					}

					parallax.ParallaxOffset = EditorGUILayout.Vector3Field( "Parallax Offset", parallax.ParallaxOffset );

					parallax.editorPreview = EditorGUILayout.ToggleLeft( "Editor Preview (Beta)", parallax.editorPreview );

					if (parallax.listOfCameras == null || parallax.listOfCameras.Count == 0 || Camera.allCameras.Length != parallax.listOfCameras.Count )
						parallax.listOfCameras = new List<Camera>( Camera.allCameras );

					string[] listCameraNames = new string[parallax.listOfCameras.Count+1];
					for (int i = 0; i < parallax.listOfCameras.Count; ++i)
					{
						if ( parallax.listOfCameras[i] != null )
							listCameraNames[i] =  parallax.listOfCameras[i].name;
					}
					
					parallax.selectedCameraIndex = EditorGUILayout.Popup(parallax.selectedCameraIndex, listCameraNames);
				}


				EditorGUI.indentLevel--;
				// give the user some options
			}
			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Zeros the out layer position.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void ZeroOutLayerPosition()
		{
			if ( Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<TileMap>() == null )
				return;


			if ( targetTileMap.layers == null )
				return;

			bool shouldRepaint = false;
			// paint resets position to zero in caase of parallaxing
			foreach ( RTStudio.Layer layer in targetTileMap.layers )
			{
				//Force the layer to 0 0 0
				if ( layer.transform.position != Vector3.zero )
				{
					shouldRepaint = true;
					layer.transform.position = Vector3.zero;
				}
			}
			if ( shouldRepaint )
			{
				Repaint();
				SceneView.RepaintAll();
			
			}
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws Align Left Button.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void CameraAlignLeftButton()
		{
			Vector3 tileyPosition = targetTileMap.gameObject.transform.position;
			GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);
		

			if( GUILayout.Button( new GUIContent(LeftAlighimage, "Camera Left Align"), buttonStyle, 
			                     GUILayout.Width(LeftAlighimage.width*1.5f), GUILayout.Height(LeftAlighimage.height*1.5f) ) )
			{
				float newX = (Settings.gameWidth/targetTileMap.tileDimensions.x)/2.0f/RTStudio.Settings.gameScale;

				listOfCameras[targetTileMap.cameraToolIndex].transform.position = 
					( new Vector3(
						tileyPosition.x + newX,                                                            
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.y, 
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.z) 
					 );
				
			}


		
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws Align Center Button.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void CameraAlignCenterButton()
		{
			Vector3 tileyPosition = targetTileMap.gameObject.transform.position;
			GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);

			
			if( GUILayout.Button( new GUIContent(CenterAlighimage, "Camera Center Align"), buttonStyle, GUILayout.Width(CenterAlighimage.width*1.5f), GUILayout.Height(CenterAlighimage.height*1.5f) ) )
			{
				float newX = this.targetTileMap.mapDimensions.x/2.0f;
				
				listOfCameras[targetTileMap.cameraToolIndex].transform.position = 
					( new Vector3(
						tileyPosition.x + newX,                                                            
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.y, 
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.z) 
					 );
			}



		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws Align Right Button.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void CameraAlignRightButton()
		{
			Vector3 tileyPosition = targetTileMap.gameObject.transform.position;
			GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);


			if( GUILayout.Button( new GUIContent(RightAlighimage, "Camera Right Align"), buttonStyle, GUILayout.Width(RightAlighimage.width*1.5f), GUILayout.Height(RightAlighimage.height*1.5f) ) )
			{
				//float newX = this.targetTileMap.mapDimensions.x - ((listOfCameras[targetTileMap.cameraToolIndex].pixelWidth/scale)/targetTileMap.tileDimensions.x)/2.0f;
				float newX = this.targetTileMap.mapDimensions.x - (RTStudio.Settings.gameWidth/targetTileMap.tileDimensions.x)/2.0f/RTStudio.Settings.gameScale;
				listOfCameras[targetTileMap.cameraToolIndex].transform.position = 
					( new Vector3(
						tileyPosition.x + newX,                                                            
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.y, 
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.z) 
					 );
				
			}

	
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws Align Top Button.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void CameraAlignTopButton()
		{
			Vector3 tileyPosition = targetTileMap.gameObject.transform.position;
			GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);

			
			
			if( GUILayout.Button( new GUIContent(TopAlighimage, "Camera Top Align"), buttonStyle, GUILayout.Width(TopAlighimage.width*1.5f), GUILayout.Height(TopAlighimage.height*1.5f) ) )
			{
				//float newY = this.targetTileMap.mapDimensions.y - ((listOfCameras[targetTileMap.cameraToolIndex].pixelHeight/scale)/targetTileMap.tileDimensions.y)/2.0f;
				float newY = this.targetTileMap.mapDimensions.y - (RTStudio.Settings.gameHeight/targetTileMap.tileDimensions.y)/2.0f/RTStudio.Settings.gameScale;
				listOfCameras[targetTileMap.cameraToolIndex].transform.position = 
					( new Vector3(
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.x, 
						tileyPosition.y + newY,                                                            
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.z) 
					 );
			}

		
		}
		
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws Align Middle Button.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void CameraAlignMiddleButton()
		{
			Vector3 tileyPosition = targetTileMap.gameObject.transform.position;
			GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);

			
			
			
			if( GUILayout.Button( new GUIContent(MiddleAlighimage, "Camera Middle Align"), buttonStyle,
			                     GUILayout.Width(MiddleAlighimage.width*1.5f), GUILayout.Height(MiddleAlighimage.height*1.5f) ) )
			{
				float newY = this.targetTileMap.mapDimensions.y/2.0f;

				listOfCameras[targetTileMap.cameraToolIndex].transform.position = 
					( new Vector3(
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.x, 
						tileyPosition.y + newY,                                                            
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.z) 
					 );
			}
			
	
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws Align Bottom Button.
		/// </summary>
		/// ====================================================================================================================================================================== 
		
		void CameraAlignBottomButton()
		{
			Vector3 tileyPosition = targetTileMap.gameObject.transform.position;
			GUIStyle buttonStyle = new GUIStyle (GUI.skin.button); 
			buttonStyle.padding = new RectOffset(0,0,0,0);

			
			
			if( GUILayout.Button( new GUIContent(BottomAlighimage, "Camera Bottom Align"), buttonStyle, 
			                     GUILayout.Width(BottomAlighimage.width*1.5f), GUILayout.Height(BottomAlighimage.height*1.5f) ) )
			{
				
				//float newY = ((listOfCameras[targetTileMap.cameraToolIndex].pixelHeight/scale)/targetTileMap.tileDimensions.y)/2.0f;
				float newY =  (RTStudio.Settings.gameHeight/targetTileMap.tileDimensions.y)/2.0f/RTStudio.Settings.gameScale;
				listOfCameras[targetTileMap.cameraToolIndex].transform.position = 
					( new Vector3(
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.x, 
						tileyPosition.y + newY,                                                            
						listOfCameras[targetTileMap.cameraToolIndex].transform.position.z) 
					 );
				
			}

		
		}

	
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the advanced settings.
		/// </summary>
		/// ====================================================================================================================================================================== 

		void DrawCameraHelpers()
        {
			EditorGUILayout.HelpBox( "Use this to align the Camera to the tile map", MessageType.Info );

			if ( listOfCameras.Count != 0 )
			{
				EditorGUILayout.LabelField("Camera Alignment:");
				//Cameras
				EditorGUI.BeginChangeCheck();
				
				string[] listCameraNames = new string[listOfCameras.Count+1];
				for (int i = 0; i < listOfCameras.Count; ++i)
				{
					listCameraNames[i] =  listOfCameras[i].name;
				}

				targetTileMap.cameraToolIndex = EditorGUILayout.Popup(targetTileMap.cameraToolIndex, listCameraNames);
                
				if (targetTileMap.cameraToolIndex >= listOfCameras.Count || targetTileMap.cameraToolIndex < 0 )
				{
					targetTileMap.cameraToolIndex = 0;
				}

				if (EditorGUI.EndChangeCheck())
				{
					
					// do something to the camera..
					
				}
                
				EditorGUILayout.Space();

				if (!LeftAlighimage)
					LeftAlighimage = Resources.Load("left", typeof(Texture)) as Texture;
				if (!CenterAlighimage)
				CenterAlighimage = Resources.Load("center", typeof(Texture)) as Texture;
				if (!RightAlighimage)
					RightAlighimage = Resources.Load("right", typeof(Texture)) as Texture;
				if (!TopAlighimage)
					TopAlighimage = Resources.Load("top", typeof(Texture)) as Texture;
				if (!MiddleAlighimage)
					MiddleAlighimage = Resources.Load("middle", typeof(Texture)) as Texture;
				if (!BottomAlighimage)
					BottomAlighimage = Resources.Load("bottom", typeof(Texture)) as Texture;
				              
                GUILayout.BeginHorizontal();           
				GUILayout.FlexibleSpace();


				CameraAlignLeftButton();
				CameraAlignCenterButton();
				CameraAlignRightButton();
				CameraAlignTopButton();
				CameraAlignMiddleButton();
				CameraAlignBottomButton();

			
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
     
                GUI.skin = null;
            }
            else
            {
            	// draw an info box or something
            }
            
            if ( listOfTileAssets.Count == 0 )
			{
				return;
			}

			EditorGUILayout.Space();
        }

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draws the advanced settings i.e Grid, Map Size and Tile Size.
		/// </summary>
		/// ====================================================================================================================================================================== 

		void DrawAdvancedSettings()
		{
			targetTileMap.gridColor = EditorGUILayout.ColorField ("Grid Color", targetTileMap.gridColor );

			if (targetTileMap.inspectorTransVisible)
				EditorGUILayout.HelpBox( "Enabling transform will disable editing", MessageType.Warning );

			bool preAllowTranform = targetTileMap.inspectorTransVisible;

            targetTileMap.inspectorTransVisible = EditorGUILayout.Toggle("Allow Transform", targetTileMap.inspectorTransVisible );

			// force the tilemap to be fixed after a transform.
			if (preAllowTranform != targetTileMap.inspectorTransVisible )
			{
				RTStudio.Utils.TileMapUtils.FixTileMap(targetTileMap);
			}

			EditorGUILayout.Space();
			EditorGUILayout.HelpBox( "Click Update to apply the changes to the options below.", MessageType.Info );

			float SavedLabelWidth = EditorGUIUtility.labelWidth;


			EditorGUIUtility.labelWidth = 80f;
			newTileMapDimensions = EditorGUILayout.Vector2Field ("Map Size", newTileMapDimensions,GUILayout.ExpandWidth(true) );
			newTilesDimensions = EditorGUILayout.Vector2Field ("Tile Size", newTilesDimensions,GUILayout.ExpandWidth(true)  );
			EditorGUIUtility.labelWidth = SavedLabelWidth;

			// Update button applies changes
			if(GUILayout.Button("Update"))
			{

				Undo.RecordObject(targetTileMap, null);
				ResizeEditor( newTileMapDimensions);

				//targetTileMap.mapDimensions = newTileMapDimensions;
				targetTileMap.tileDimensions = newTilesDimensions;
				RTStudio.Utils.TileMapUtils.FixTileMap(targetTileMap); 
			}

			EditorGUILayout.Space();
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Resizes the Tile map, allows for undo.
		/// </summary>
		/// <param name="v">V.</param>
		/// ====================================================================================================================================================================== 

		public void ResizeEditor( Vector2 v )
		{
			if ( targetTileMap.layers != null )
			{
				foreach( RTStudio.Layer layer in targetTileMap.layers )
				{
					GameObject[] gos = new GameObject[ (int)(v.x * v.y) ];
					
					for( int i = 0; i < targetTileMap.mapDimensions.x; i++) 
					{
						for( int j = 0; j < targetTileMap.mapDimensions.y; j++) 
						{
							int tileIndex1 = (int)(i + targetTileMap.mapDimensions.x * j);
							int tileIndex2 = (int)(i + v.x * j);
							
							if ( i < v.x && j < v.y )
							{
								if ( layer.tiles[ (int)(tileIndex1) ] != null && tileIndex2 < gos.Length )
								{
									gos[ (int)(tileIndex2) ] = layer.tiles[ (int)(tileIndex1) ];
								}
							}
	                        else
	                        {
								if ( layer.tiles[ (int)(tileIndex1) ] != null )
								{
	                                GameObject go = layer.tiles[ (int)(tileIndex1)].gameObject;
									EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(go);
								}
	                        }
	                        
	                    }
	                }
	                
					Undo.RecordObject(layer, null);
	                layer.tiles = gos;
	            }
			}
            
			targetTileMap.mapDimensions = v;
        }
        
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draw the Layer header
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// ====================================================================================================================================================================== 

		void LayerHeaderDelegate( Rect rect )
		{
			if (!iconLayerTextureimage)
				iconLayerTextureimage = Resources.Load("RTSicon", typeof(Texture)) as Texture;
            
            Color Save = GUI.color;
            GUI.color = new Color32(255, 255, 255, 0);
			rect.x-=2;
			rect.y-=1;
			EditorGUI.DrawTextureTransparent( (new Rect(rect.x, rect.y, 18, 18)), iconLayerTextureimage, ScaleMode.StretchToFill, 1);
			GUI.color = Save;
			rect.y-=0;
			rect.x+=17;
			byte floatColor = 0;

			GUI.color = new Color32(floatColor, floatColor, floatColor, 160);
			EditorGUI.LabelField(rect, "Layers");
			GUI.color = Save;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Draw each layer in the list
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="index">Index.</param>
		/// <param name="isActive">If set to <c>true</c> is active.</param>
		/// <param name="isFocused">If set to <c>true</c> is focused.</param>
		/// ====================================================================================================================================================================== 

		void LayerDrawLDelegate(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index < 0 || index >= layers.serializedProperty.arraySize)
				return;

			float lableWidthPercent = 0.25f;
			float sliderWidthPercent = 0.59f;
			float spaceing = 16;

			Color Save = GUI.contentColor;
			Color Save2 = GUI.color;
			if ( index == targetTileMap.selectedLayerIndex )
			{
				GUI.contentColor = new Color32(0, 255, 055, 185);
				isFocused = true;
            }
            
			SerializedProperty item = layers.serializedProperty.GetArrayElementAtIndex(index);
			RTStudio.Layer tileLayer = item.objectReferenceValue as RTStudio.Layer;

			if (tileLayer == null)
			{
				GUI.color = Save;
				return;
			}

			rect.y += 1;
			//rect.x -= 3;

			// Toggle Active / Inactive
			tileLayer.gameObject.SetActive( EditorGUI.Toggle(new Rect(rect.x, 
			                                                          rect.y, 
			                                                          spaceing, 
			                                                          EditorGUIUtility.singleLineHeight
			                                                          ), 
			                                                 GUIContent.none, tileLayer.gameObject.activeInHierarchy));

			rect.x += spaceing;

			// Name
			tileLayer.gameObject.name = EditorGUI.TextField(new Rect(rect.x,
			                                                         rect.y, 
			                                                         (int)(rect.width*lableWidthPercent
			      													), 
			                                                 EditorGUIUtility.singleLineHeight), tileLayer.gameObject.name );


			rect.x += rect.width*lableWidthPercent;
			rect.x+=(spaceing/3);

			// Alpha slider & box
			tileLayer.alpha = EditorGUI.Slider ( new Rect( rect.x, 
			                                         	   rect.y, 
			                                               rect.width*sliderWidthPercent, 
			                                         EditorGUIUtility.singleLineHeight), 
			                              			 tileLayer.alpha, 0f, 1f );
         
			if ( index == targetTileMap.selectedLayerIndex )
			{
				if (!iconSelectedTextureimage)
					iconSelectedTextureimage = Resources.Load("Selectedicon", typeof(Texture)) as Texture;

				rect.x+=rect.width*sliderWidthPercent + 2;
				GUI.color = new Color32(255, 255, 255, 0);
				EditorGUI.DrawTextureTransparent( (new Rect(rect.x, rect.y, 18, 18)), iconSelectedTextureimage, ScaleMode.StretchToFill, 1);
			}

			GUI.color = Save2;
			GUI.contentColor = Save;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Handle the removal of a layer, i.e when the '-' button is pressed
		/// </summary>
		/// <param name="list">List.</param>
		/// ====================================================================================================================================================================== 

		void LayerRemoveDelegate (ReorderableList list)
		{
			int index = list.index;
			if (index < 0 || index >= list.count)
			{
				return;
			}

			if ( targetTileMap.selectedLayerIndex == index )
			{
				justCreatedLayer = true;
				lastLayerCreated = index - 1;

				if (lastLayerCreated < 0)
					lastLayerCreated = 0;
			}

			SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(index);
			RTStudio.Layer tileLayer = item.objectReferenceValue as RTStudio.Layer;
			if (tileLayer != null)
			{
				GameObject go = tileLayer.gameObject;
				EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(go);
			}

			for (int i = index; i < list.serializedProperty.arraySize - 1; ++i)
			{
				list.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = list.serializedProperty.GetArrayElementAtIndex(i + 1).objectReferenceValue;
			}
				
			list.serializedProperty.arraySize--;
			if (list.serializedProperty.arraySize > 0)
			{
				list.index = Mathf.Clamp(list.index, 0, list.serializedProperty.arraySize - 1);
			}
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Called when you add a layer
		/// </summary>
		/// <param name="list">List.</param>
		/// ====================================================================================================================================================================== 

		void LayerAddDelegate (ReorderableList list)
		{
			int index = list.count;
			list.serializedProperty.arraySize++;
			list.index = index;

			lastLayerCreated = index;
			justCreatedLayer = true;

			targetTileMap.selectedLayerIndex = index-1;
			SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(index);

			// Create the layer
			GameObject Layer = new GameObject();
			RTStudio.Layer pLayer = Layer.AddComponent<RTStudio.Layer>();
			pLayer.tiles = new GameObject[ (int)(targetTileMap.mapDimensions.x * targetTileMap.mapDimensions.y) ];
			Layer.transform.parent = targetTileMap.transform;
			Layer.gameObject.name = "Layer" + (index+1);
			item.objectReferenceValue = pLayer;

			GUI.FocusControl("Layer-"+Layer.gameObject.name);

			// Register UNDO
			Undo.RegisterCreatedObjectUndo(Layer.gameObject, null);
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Called whan a change is made
		/// </summary>
		/// <param name="list">List.</param>
		/// ====================================================================================================================================================================== 

		void LayerChangedDelegate (ReorderableList list)
		{
		
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Called when a layer is selected
		/// </summary>
		/// <param name="list">List.</param>
		/// ====================================================================================================================================================================== 

		void LayerSelectedDelegate (ReorderableList list)
		{
			int index = list.index;

			if (index < 0 || index >= list.count)
			{
                return;
            }
			// Set as the active layer
			targetTileMap.selectedLayerIndex = index < 0? 0 : index;
        }
        
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Called when the layer list is reordered
		/// </summary>
		/// <param name="reorder_list">Reorder_list.</param>
		/// ====================================================================================================================================================================== 
		void LayerReorderDelegate (ReorderableList reorder_list)
		{
			RTStudio.Utils.TileMapUtils.SortLayers(targetTileMap);
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Creates the tile set from the project menu
		/// </summary>
		/// ====================================================================================================================================================================== 

		[MenuItem("Assets/Create/RTStudio/TileSet")]
		static void CreateTileSet()
		{
			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			RTStudio.Utils.TileMapUtils.CreateTileSetWithPath( path, true );
		}

	}

}
