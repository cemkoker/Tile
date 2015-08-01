using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTStudio
{
	[ExecuteInEditMode]
	public class Parallax : MonoBehaviour 
	{
				
		[System.Serializable]
		public class Locks
		{
			public bool xLocked;
			public bool yLocked;
		}

		public enum ParallaxTypes
		{
			Equally,
			Independantly
		}
		
		[System.Serializable]
		public class ParallaxPerAxisAmount
		{
			public float x = 0.5f;
			public float y = 0.5f;
		}
		
		// Parallax mode 		
		public ParallaxTypes parallaxType;
		
		// Equally Parallax Amount Stored
		public float parallax = 0.5f;

		// Parallax Axis Movement Stored
		public ParallaxPerAxisAmount parallaxAxisAmt = new ParallaxPerAxisAmount();

		// Flag locked axises
		public Locks lockedAxes = new Locks(); 

		// Ref to all cameras are stored in here
		public List<Camera> listOfCameras;

		// selected camera index
		public int selectedCameraIndex = 0;

		// By reference
		public Camera selectedCamera;

		// parallax Offset
		public Vector3 ParallaxOffset = Vector3.zero;

		// cache last position
		private Vector3 cameraLastPosition;

		private bool didAddParallax = false;

		// Parallax mode 		
		public bool editorPreview = false;

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		private void Start()
		{
			initialize();
		}
	
		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Initialize this Parallax,.
		/// </summary>
		/// ====================================================================================================================================================================== 
		private void initialize()
		{
			if( listOfCameras == null || listOfCameras.Count == 0 )
				selectedCamera = null;
			else
			{
				if ( selectedCameraIndex >= listOfCameras.Count )
					selectedCameraIndex = 0;

				selectedCamera = listOfCameras[selectedCameraIndex];
			}


			if ( Application.isPlaying )
				transform.position += ParallaxOffset;

			cameraLastPosition = GetCameraPosition();
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Update this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		private void Update()
		{
#if UNITY_EDITOR
			if ( Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() == null && !Application.isPlaying)
			{
				//Debug.Log("no camera no parallax");
				return;
			}
#endif

			if ( !Application.isPlaying && !editorPreview )
				return;

			if ( !Application.isPlaying )
			{
				didAddParallax = true;
				transform.position+= ParallaxOffset;
			}
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Update logic
		/// </summary>
		/// ====================================================================================================================================================================== 
		private void LateUpdate()
		{
			UpdateParallax();
		}


		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Raises the render object event.
		/// </summary>
		/// ====================================================================================================================================================================== 
		void OnRenderObject()
		{

			if ( !Application.isPlaying && !editorPreview )
				return;

			// this allows you to preview parallax in the scene
			if (didAddParallax)
			{
				#if UNITY_EDITOR
				if ( Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() == null && !Application.isPlaying )
				{
					return;
				}
				#endif
				didAddParallax = false;

				if ( !Application.isPlaying )
				{
					transform.position-= ParallaxOffset;
				}
			}
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Parallax this instance.
		/// </summary>
		/// ====================================================================================================================================================================== 
		private void UpdateParallax()
		{

			if ( !Application.isPlaying && !editorPreview )
				return;

			#if UNITY_EDITOR
			if ( Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() == null && !Application.isPlaying )
			{
				//Debug.Log("no camera no parallax 2");
				return;
			}
			#endif

			// early exit
			if(selectedCamera == null)
				return;
			
			Vector3 cameraPosition = GetCameraPosition();			
			Vector3 cameraMovement = cameraPosition - cameraLastPosition;
			
			// Apply parallax
			cameraMovement.z = 0.0f;

			if ( parallaxType == Parallax.ParallaxTypes.Equally )
			{
				cameraMovement *= parallax;
				
				if(lockedAxes.xLocked)
					cameraMovement.x = 0.0f;

				if(lockedAxes.yLocked)
					cameraMovement.y = 0.0f;
			}
			else 
			{
				cameraMovement.x *= parallaxAxisAmt.x;
				cameraMovement.y *= parallaxAxisAmt.y;
			}
		
			transform.position += cameraMovement;		
			transform.position = new Vector3 ( RTStudioCamera.RoundToNearestPixel( transform.position.x ),
			                                   RTStudioCamera.RoundToNearestPixel( transform.position.y ), 
			                                  transform.position.z
			                                  );
			cameraLastPosition = cameraPosition;
		}

		/// ====================================================================================================================================================================== 
		/// <summary>
		/// Gets the selected camera position.
		/// </summary>
		/// <returns>The camera position as a Vector3</returns>
		/// ====================================================================================================================================================================== 
		private Vector3 GetCameraPosition()
		{
			if(selectedCamera == null)
				return Vector3.zero;

			return selectedCamera.transform.position;
		}

	}
}
