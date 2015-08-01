using UnityEngine;
using System.Collections;
using RTStudio;

namespace RTStudio.Helpers
{
	[ExecuteInEditMode]
	public class CameraHelper : MonoBehaviour {

		// Define Types of camera shakes
		public enum ShakeType
		{
			SHAKE_BOTH_AXES,
			SHAKE_HORIZONTAL_ONLY,
			SHAKE_VERTICAL_ONLY
		}

		// Target object. If none is set the camera remains stationary
		public GameObject target;		
		public RTStudio.TileMap bounds;
		public float dampening =  0.035f;
		public bool runInEditor = false;

		private float shakeDuration=0;
		private ShakeType shakeDirection = ShakeType.SHAKE_BOTH_AXES;
		private float shakeIntensity=0;
		private Vector2 shakeOffset = new Vector2();
		private Vector3 originalPosition;
		private Vector3 lastFramePosition;
		private float m_fLookVelocity;
		private float m_fLookVelocity2;

		// Paddings for bounds
		public float boundsTopPadding = 0;
		public float boundsBottomPadding = 0;
		public float boundsLeftPadding = 0;
		public float boundsRightPadding = 0;

		float newLocation;
		float newLocationY;

		/// ======================================================================================================================================================================
		/// <summary>
		/// Shakes the camera with intensity.
		/// </summary>
		/// <param name="intensity">Intensity.</param>
		/// ======================================================================================================================================================================
		public void ShakeCameraWithIntensity (float intensity)
		{
			ShakeCameraWithIntensityAndDuration( intensity, 0.1f );
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Shakes the duration of the camera with intensity and.
		/// </summary>
		/// <param name="intensity">Intensity.</param>
		/// <param name="duration">Duration.</param>
		/// ======================================================================================================================================================================
		public void ShakeCameraWithIntensityAndDuration (float intensity, float duration = 0.1f)
		{
			shakeDuration = duration;
			shakeIntensity = intensity/100500.0f;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// ======================================================================================================================================================================
		void Start () {
				originalPosition = transform.position;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Update this instance.
		/// </summary>
		/// ======================================================================================================================================================================
		void Update () {

			if ( !runInEditor && !Application.isPlaying )
				return;

			// set to target position
			if ( target )
			{
				float xdamp = dampening;
				float ydamp = dampening;
				
				newLocation = Mathf.SmoothDamp( target.transform.position.x, newLocation, ref m_fLookVelocity, xdamp );
				newLocationY = Mathf.SmoothDamp( target.transform.position.y, newLocationY, ref m_fLookVelocity2, ydamp );

				transform.position =  new Vector3 ( newLocation, newLocationY, transform.position.z );
			}

			if ( bounds && RTStudio.Settings.gameScale != 0 )
			{

				float leftBounds = bounds.GetLeftBounds();
				float rightBounds = bounds.GetRightBounds();
				float topBounds = bounds.GetTopBounds();
				float bottomBounds = bounds.GetBottomBounds();

				float middle = bounds.GetMiddle();
				float center = bounds.GetCenter();

				if ( transform.position.x < leftBounds - boundsLeftPadding*RTStudio.RTStudioCamera.pixelUnit && bounds.GetWidthInPixels() > (RTStudio.Settings.gameWidth/RTStudio.Settings.gameScale) )
				{
					transform.position =  new Vector3 ( leftBounds - boundsLeftPadding*RTStudio.RTStudioCamera.pixelUnit, transform.position.y, transform.position.z );
				}
				else if ( transform.position.x > rightBounds + boundsRightPadding*RTStudio.RTStudioCamera.pixelUnit && bounds.GetWidthInPixels() > (RTStudio.Settings.gameWidth/RTStudio.Settings.gameScale)  )
				{
					transform.position =  new Vector3 ( rightBounds + boundsRightPadding*RTStudio.RTStudioCamera.pixelUnit, transform.position.y, transform.position.z );
				}
				else if ( !(bounds.GetWidthInPixels() > (RTStudio.Settings.gameWidth/RTStudio.Settings.gameScale)) )
					transform.position =  new Vector3 ( center, transform.position.y, transform.position.z );
					

				if ( transform.position.y > topBounds + boundsTopPadding*RTStudio.RTStudioCamera.pixelUnit && bounds.GetHeightInPixels() > (RTStudio.Settings.gameHeight/RTStudio.Settings.gameScale) )
				{
					transform.position =  new Vector3 ( transform.position.x, topBounds + boundsTopPadding*RTStudio.RTStudioCamera.pixelUnit, transform.position.z );
				}
				else if ( transform.position.y < bottomBounds - boundsBottomPadding*RTStudio.RTStudioCamera.pixelUnit && bounds.GetHeightInPixels() > (RTStudio.Settings.gameHeight/RTStudio.Settings.gameScale) )
				{
					transform.position =  new Vector3 ( transform.position.x, bottomBounds- boundsBottomPadding*RTStudio.RTStudioCamera.pixelUnit , transform.position.z );
				}
				else if ( !(bounds.GetHeightInPixels() > (RTStudio.Settings.gameHeight/RTStudio.Settings.gameScale)) )
					transform.position =  new Vector3 ( transform.position.x, middle, transform.position.z );
			}
		
			var position = transform.position - ((Vector3)shakeOffset);
			transform.position = position;

			UpdateShake();

		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// This updates the camera position if a Shake is happening
		/// </summary>
		/// ======================================================================================================================================================================
		void UpdateShake()
		{
			//Update the "shake" special effect
			if(shakeDuration > 0)
			{
				shakeDuration -= Time.deltaTime;
				if(shakeDuration <= 0)
				{
					
					// Done Shake
					shakeOffset.x = 0;
					shakeOffset.y = 0;
					
					if (!target )
						transform.position = originalPosition;
					
				}
				else
				{
					if((shakeDirection == ShakeType.SHAKE_BOTH_AXES) || (shakeDirection == ShakeType.SHAKE_HORIZONTAL_ONLY))
						shakeOffset.x = (UnityEngine.Random.Range(0.0f,1.0f)*shakeIntensity*Screen.width*2-shakeIntensity*Screen.width);
                    if((shakeDirection == ShakeType.SHAKE_BOTH_AXES) || (shakeDirection == ShakeType.SHAKE_VERTICAL_ONLY))
                        shakeOffset.y = (UnityEngine.Random.Range(0.0f,1.0f)*shakeIntensity*Screen.height*2-shakeIntensity*Screen.height);
                }
			}
		}
	}

}
