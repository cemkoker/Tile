using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RTStudio
{
	[CustomEditor(typeof(RTStudio.Settings))]
	public class SettingsEditor : Editor 
	{
		void OnSceneGUI()
		{
			EditorUtility.SetDirty(target);
		}
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Raises the inspector GU event.
		/// </summary>
		/// ====================================================================================================================================================================== 

		public override void OnInspectorGUI()
		{
			// grab the target
			RTStudio.Settings targetSettings = (RTStudio.Settings) target;

			//PyxSystemSettings myTarget = (PyxSystemSettings)target;
			//EditorGUILayout.HelpBox( "Resolution: " + myTarget.TileSize*myTarget.GameTilesWide*myTarget.GameScale + "x" + myTarget.TileSize*myTarget.GameTilesHigh*myTarget.GameScale + " " + GetMainGameViewSize().ToString() , MessageType.Info );
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			RTStudio.Utils.Utils.DrawLogo();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if ( targetSettings.Resolution != RTStudio.Settings.ResolutionPresets.CustomResolution )
				EditorGUILayout.HelpBox("Resolution set to : " + targetSettings.GameWidth + "x" + targetSettings.GameHeight, MessageType.Info
				                        );
			targetSettings.Resolution = (RTStudio.Settings.ResolutionPresets)EditorGUILayout.EnumPopup( "Reference Resolution", targetSettings.Resolution );

			if ( targetSettings.Resolution == RTStudio.Settings.ResolutionPresets.CustomResolution )
			{
				targetSettings.GameWidth = EditorGUILayout.IntField( "Game Width", targetSettings.GameWidth );
				targetSettings.GameHeight = EditorGUILayout.IntField( "Game Height", targetSettings.GameHeight );
			}
			else
			{
				targetSettings.GameWidth = (int)targetSettings.Resolutions[(int)targetSettings.Resolution].x;
				targetSettings.GameHeight = (int)targetSettings.Resolutions[(int)targetSettings.Resolution].y;
			}

			targetSettings.GameScale = EditorGUILayout.IntField( "Game Scale", targetSettings.GameScale );
			targetSettings.targetFramerate = EditorGUILayout.IntField( "Target Framerate", targetSettings.targetFramerate );
			targetSettings.sortMode = (TransparencySortMode)EditorGUILayout.EnumPopup( "Sort Mode", targetSettings.sortMode );
			
			targetSettings.vSyncCount = EditorGUILayout.IntField( "Vsync Count", targetSettings.vSyncCount );
			
			targetSettings.PixelsPerUnit = EditorGUILayout.FloatField( "Pixels Per Unit", targetSettings.PixelsPerUnit );
			targetSettings.PixelsPerUnit = targetSettings.PixelsPerUnit <= 0? 1 : targetSettings.PixelsPerUnit;

			targetSettings.addRTSCameraToMain = EditorGUILayout.Toggle( "Manage Main Camera", targetSettings.addRTSCameraToMain );

			targetSettings.PixelPerfect = EditorGUILayout.Toggle( "Pixel Perfect", targetSettings.PixelPerfect );

			if ( !targetSettings.PixelPerfect )
			{
				EditorGUILayout.HelpBox ( "Pixel distortion may occur when this option is off.", MessageType.Warning );
			}

			targetSettings.BestFitRoundsUp = EditorGUILayout.Toggle( "Nearest Zoom", targetSettings.BestFitRoundsUp );

			if ( targetSettings.BestFitRoundsUp )
			{
				EditorGUILayout.HelpBox ( "Some of your scene may be slightly clipped.", MessageType.Warning );
			}

			targetSettings.HideBordersInEditor = EditorGUILayout.Toggle( "Hide Borders In Editor", targetSettings.HideBordersInEditor );

			targetSettings.HideBordersInGame = EditorGUILayout.Toggle( "Hide Borders In Game", targetSettings.HideBordersInGame );


			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorUtility.SetDirty(target);
			//base.OnInspectorGUI();
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Gets the size of the main game view.
		/// </summary>
		/// <returns>The main game view size.</returns>
		/// ====================================================================================================================================================================== 

		public static Vector2 GetMainGameViewSize()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
			return (Vector2)Res;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Gets the main game view.
		/// </summary>
		/// <returns>The main game view.</returns>
		/// ====================================================================================================================================================================== 
		public static EditorWindow GetMainGameView()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			System.Object Res = GetMainGameView.Invoke(null,null);
			return (EditorWindow)Res;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Creates the RT Studio Settings object.
		/// </summary>
		/// ====================================================================================================================================================================== 

		[MenuItem("GameObject/Create Other/RTStudio/Settings")]
		static void CreateTileSettings()
		{
			RTStudio.Settings[] settingsObjects = FindObjectsOfType(typeof(RTStudio.Settings)) as RTStudio.Settings[];

			if ( settingsObjects.Length > 0 )
			{
				EditorUtility.DisplayDialog("Settings Object Already Exists!", 
				                            "A Retro Tile Studio Settings Object is already in the scene. " +
				                            "Please delete the current " +
				                            "object if you wish to create a new one.", 
				                            "Ok");
				return;
			}

			var obj = new GameObject("_RTStudioGameSettings");
			obj.AddComponent<RTStudio.Settings>();

			// Add the border prefab to the scene
			GameObject Borders =  (GameObject)Instantiate(Resources.Load("RTStudioBorders"));
			GameObject BordersPP =  (GameObject)Instantiate(Resources.Load("RTStudioBordersPixelPerfect"));

			// Borders
			Borders.name = "Borders";
			Borders.transform.parent = obj.transform;

			// Pixel Perfect Borders
			BordersPP.name = "BordersPP";
			BordersPP.transform.parent = obj.transform;

		}

	}
}