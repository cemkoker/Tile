using UnityEngine;
using System.Collections;

namespace RTStudio 
{
	[ExecuteInEditMode]
	public class Settings : MonoBehaviour 
	{

		/// <summary>
		/// Setup the game for a retro styled game
		/// </summary>

		public enum ResolutionPresets
		{
			CustomResolution,
			GameBoyAndGameBoyColor,
			GameboyAdvanced,
			NintendoDS,
			Nintendo3dsTopScreen,
			Nintendo3dsBottomScreen,
			VirtualBoy,
			PSP,
			PSVita,
			NES,
			SNES,
			Genesis,
			SegaMasterSystem,
			SegaGameGear,
			Atari2600,
			AtariLynx,
		}

		public ResolutionPresets Resolution;

		public Vector2[] Resolutions = new Vector2[] { 
			(new Vector2(0,0)),  	 // Custom
			(new Vector2(160,144)),  // Gameboy
			(new Vector2(240,160)),  // Gameboy Advanced
			(new Vector2(256,192)),  // DS
			(new Vector2(400,240)),  // 3DS Top Screen
			(new Vector2(320,240)),  // 3DS Bottom Screen
			(new Vector2(384,224)),  // Virtual Boy
			(new Vector2(480,272)),  // PSP
			(new Vector2(960,544)),  // PSVita
			(new Vector2(256,224)),  // NES
			(new Vector2(256,224)),  // SNES
			(new Vector2(320,240)),  // Genesis
			(new Vector2(256,192)),	 // Master System
			(new Vector2(160,144)),	 // Game Gear
			(new Vector2(160,192)),	 // Atari 2600
			(new Vector2(160,102)),	 // Atari Lynx
		};

		// SET GAME RESOLUTION
		public int GameWidth = 160;
		public int GameHeight = 144;
		
		// SET GAME SCALEING
		public int GameScale = 1;

		// TARGET FRAMERATE
		public int targetFramerate = 90;
		
		// SORTING MODE : Defaults to Orthograpic
		public TransparencySortMode sortMode = TransparencySortMode.Orthographic;

		// VSYNC
		public int vSyncCount = 1;

		// PIXELS PER UNIT
		public float PixelsPerUnit = 8;

		// Affects All Cameras
		public bool addRTSCameraToMain = true;

		// Pixel perfect always
		public bool PixelPerfect = true;

		// Hide the borders in editor, sometimes they get in the way!
		public bool HideBordersInEditor = false;

		// Hide the borders in game, sometimes you just dont want them at all!
		public bool HideBordersInGame = false;

		// private pixel perfect best fit
		[HideInInspector]
		public int BestFit = 1;

		// private pixel perfect will best fit round up?
		[HideInInspector]
		public bool BestFitRoundsUp = true;

		// border and PP border
		Transform PPBorder;
		Transform Border;


		// Incase a new camera is added
		int last_camera_count = -1;

	    float lastTickHeight = 0;
		float lastTickWidth = 0;
		int lastGameWidth = 0;
		int lastGameHeigth = 0;
		int lastGameScale = 0;
		bool lastPixelperfect = false;
		float lastPixelPerUnit = 0;
		bool lastHideBordersInEditor = true;
		bool lastHideBordersInGame = true;
		bool lastBestFitRoundsUp = true;

		Settings.ResolutionPresets lastResolutionPresets;

		// Reference to this instance
		public static Settings instance;
		public static int gameWidth;
		public static int gameHeight;
		public static int gameScale;
		public static ResolutionPresets resolutionPresets;
		public static float staticPixelsPerUnit = 100;
		public static int bestFit = 1;
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Awake this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void Awake()
		{
			instance = this;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Sets the resolution.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// ====================================================================================================================================================================== 
		public static void SetReferenceResolution ( int x, int y )
		{
			if ( instance == null )
				return;

			gameWidth = instance.GameWidth = x;
			gameHeight = instance.GameHeight = y;

		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void Start () 
		{
			gameWidth = GameWidth;
			gameHeight = GameHeight;
			gameScale = GameScale;

			Application.targetFrameRate = targetFramerate;	
			QualitySettings.vSyncCount = vSyncCount;

			if ( Camera.main != null)
				SetupCamera(Camera.main, true);

			AspectRatioFix();
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Setups the pyx camera.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void SetupCamera( Camera c, bool addRTSComponent )
		{

			if ( c )
			{
				c.transparencySortMode = sortMode;
				c.orthographic = true;

				if ( Camera.main )
				{
					c.aspect = Camera.main.aspect;
					c.orthographic = true;
					c.orthographicSize = Camera.main.GetComponent<Camera>().orthographicSize;
					c.transparencySortMode = Camera.main.transparencySortMode;
				}

				if ( addRTSComponent )
				{
					RTStudio.RTStudioCamera CameraC = Camera.main.GetComponent<RTStudio.RTStudioCamera>();	
					if (!CameraC)
							Camera.main.gameObject.AddComponent<RTStudio.RTStudioCamera>();
				}
				else 
				{
					RTStudio.RTStudioCamera CameraC = Camera.main.GetComponent<RTStudio.RTStudioCamera>();	
					if (CameraC && !Application.isPlaying)
						DestroyImmediate( CameraC );
				}

			}

		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Aspects ratio fix.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void AspectRatioFix()
		{
			if (gameWidth == 0 || gameHeight == 0 )
				return;

			if ( Camera.main != null && Camera.main.GetComponent<Camera>() != null )
			{

				Border = transform.FindChild("Borders");
				PPBorder = transform.FindChild("BordersPP");

				if ( !PixelPerfect )
				{
					if ( PPBorder )
						PPBorder.gameObject.SetActive( false );
					if ( Border )
						Border.gameObject.SetActive( true );

					float height = ( (GameWidth) / (float)gameWidth * gameHeight);
					if ( Screen.width < Screen.height )
					{
						height = GameWidth / (float)Screen.width * Screen.height;
					}
					Camera.main.aspect = ((float)Screen.width) / ((float)Screen.height);
					Camera.main.GetComponent<Camera>().orthographicSize = (height/PixelsPerUnit/2.0f/gameScale);
				}
				else
				{

					int BestFitWidth = (int) ((Camera.main.GetComponent<Camera>().pixelWidth / gameWidth)+(BestFitRoundsUp?0.5f:0));
					BestFit = (int)((Camera.main.GetComponent<Camera>().pixelHeight / gameHeight)+(BestFitRoundsUp?0.5f:0));

					if ( BestFitWidth < BestFit )
					{
						BestFit = BestFitWidth;
					}

					// lowest best fit value of 1
					BestFit = BestFit <= 0? 1 : BestFit;
				
					// if not a multiple of 2 match to nearest multiple for best fit
					if ( BestFit > 1 && (BestFit % 2 ) == 1 )
					{
						BestFit = BestFit - 1;
					}

					Camera.main.aspect = ((float)Screen.width) / ((float)Screen.height);
					Camera.main.GetComponent<Camera>().orthographicSize = ((float)((Camera.main.GetComponent<Camera>().pixelHeight / (PixelsPerUnit * (BestFit) )) / 2.0d ) / gameScale);	

					if ( PPBorder )
						PPBorder.gameObject.SetActive( true );
					if ( Border )
						Border.gameObject.SetActive( false );

				}

			}

			// Hide Borders, Play mode always shows Borders
			if ( HideBordersInEditor && !Application.isPlaying )
			{
				if ( Border )
					Border.gameObject.SetActive( false );
				if ( PPBorder )
					PPBorder.gameObject.SetActive( false );
			
			}

			// Hide Borders, Play mode always shows Borders
			if ( HideBordersInGame && Application.isPlaying )
			{
				if ( Border )
					Border.gameObject.SetActive( false );
				if ( PPBorder )
					PPBorder.gameObject.SetActive( false );
				
			}


			if ( !Application.isPlaying )
			{
				// Apply Settings to All Cameras In Scene
				if ( addRTSCameraToMain )
				{
					SetupCamera( Camera.main, true );
				}
			}
			
		}
		
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Update this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void Update () 
		{


		
			gameWidth = GameWidth;
			gameHeight = GameHeight;
			gameScale = GameScale;
			resolutionPresets = Resolution;
			bestFit = BestFit;

			if ( lastGameScale != gameScale || 
			     lastTickHeight != Screen.height ||
			     lastTickWidth != Screen.width ||  
			     Camera.allCamerasCount != last_camera_count || 
			     lastGameWidth !=  gameWidth ||
			     lastGameHeigth != gameHeight ||
			    lastResolutionPresets != resolutionPresets ||
			    lastHideBordersInEditor != HideBordersInEditor || 
			    lastHideBordersInGame != HideBordersInGame || 
			    lastPixelperfect != PixelPerfect || 
			    lastPixelPerUnit != PixelsPerUnit ||
			    lastBestFitRoundsUp != BestFitRoundsUp
			    )
			{
	            AspectRatioFix();
	        }

			lastBestFitRoundsUp = BestFitRoundsUp;
			lastHideBordersInEditor = HideBordersInEditor;
			lastHideBordersInGame = HideBordersInGame;
			lastPixelPerUnit = staticPixelsPerUnit = PixelsPerUnit;
			lastPixelperfect = PixelPerfect;
			lastResolutionPresets = resolutionPresets;	
			last_camera_count = Camera.allCamerasCount;
			lastTickHeight = Screen.height;
			lastTickWidth = Screen.width;
			lastGameWidth = gameWidth;
			lastGameHeigth = gameHeight;
			lastGameScale = gameScale;
		}

	}
}
    