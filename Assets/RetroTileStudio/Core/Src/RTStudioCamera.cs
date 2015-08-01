using UnityEngine;
using System.Collections;
namespace RTStudio
{	
	[ExecuteInEditMode]
	public class RTStudioCamera : MonoBehaviour {

		public static float 	pixelUnit;
		public static float  	snapValue;
		public static float  	lastBestFit;
		public static Vector3 	ppuVector;

		[Range(0.0f,1.0f)]
		public float snapErrorX =0.1f;
		[Range(0.0f,1.0f)]
		public float snapErrorY =0.1f;

		public bool showSnapErrorInGame = false;
		private float 	lastPPU;
		private Vector3 lastFramePosition;
	
		//// ======================================================================================================================================================================
		/// <summary>
		/// Raises the pre render event.
		/// </summary>
		/// ======================================================================================================================================================================
		void OnPreRender() {
		
			// save last location
			lastFramePosition=transform.position;

			RecalculatePUandScale();

			float newX = RoundToNearestPixel(transform.position.x);
			float newY = RoundToNearestPixel(transform.position.y);
	
			transform.position=new Vector3( newX, newY, transform.position.z)+( (new Vector3(snapErrorX,snapErrorY) * snapValue ));			
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Rounds any float to the nearest pixel.
		/// </summary>
		/// <returns>The to nearest pixel.</returns>
		/// <param name="val">Value.</param>
		/// ======================================================================================================================================================================
		public static float RoundToNearestPixel(float val)
		{
			if ( snapValue == 0 )
			{
				// prevent div by zero
				return val;
			}
			return Mathf.Round((val/snapValue))*snapValue;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Lates the update.
		/// </summary>
		/// ======================================================================================================================================================================
		public void LateUpdate()
		{
			//bool fixes = false;
			// fix any pixel objects needing snapping
			foreach ( Transform sr in this.GetPixelPerfectObjects() )
			{		
				float newSrx = RoundToNearestPixel( sr.position.x );
				float newSry = RoundToNearestPixel( sr.position.y );
				if ( sr.position.x != newSrx || 
				     sr.position.y != newSry )
				{
					sr.position = new Vector3 ( newSrx, newSry, sr.position.z );
				}
			}

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Helper funtion for tweaking snap offset on device
		/// </summary>
		/// ======================================================================================================================================================================
		public void OnGUI()
		{
			if ( showSnapErrorInGame )
			{
				snapErrorX = GUI.HorizontalSlider( (new Rect( 10, 100, 100, 30 )), snapErrorX, 0, 1.0f );
				snapErrorY = GUI.HorizontalSlider( (new Rect( 10, 130, 100, 30 )), snapErrorY, 0, 1.0f );
				GUI.Label( (new Rect( 150,100, 100, 30 )),""+snapErrorX);
				GUI.Label( (new Rect( 150,130, 100, 30 )),""+snapErrorY);
			}

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Raises the post render event.
		/// </summary>
		/// ======================================================================================================================================================================
		void OnPostRender() {
			transform.position=lastFramePosition;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Recalculates the PU and snap.
		/// </summary>
		/// ======================================================================================================================================================================
		void RecalculatePUandScale()
		{
			if ( lastPPU == RTStudio.Settings.staticPixelsPerUnit && RTStudio.Settings.bestFit == lastBestFit)
				return;

			pixelUnit=(1.0f/(float)RTStudio.Settings.staticPixelsPerUnit);
			snapValue=pixelUnit/RTStudio.Settings.bestFit;

			// save new values for future changes
			lastPPU = RTStudio.Settings.staticPixelsPerUnit;
			lastBestFit = RTStudio.Settings.bestFit;
		}

	}
}
