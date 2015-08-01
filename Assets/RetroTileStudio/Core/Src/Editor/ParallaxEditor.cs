using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace RTStudio
{
	[CustomEditor(typeof(RTStudio.Parallax))]
	[CanEditMultipleObjects]
	public class PyxParallaxEditor : Editor
	{
		public override void OnInspectorGUI()
		{

			// grab the target
			RTStudio.Parallax targetParallax = (RTStudio.Parallax) target;

			serializedObject.Update();
			
			// Parallax Type
			SerializedProperty pType = serializedObject.FindProperty("parallaxType");
			EditorGUILayout.PropertyField(pType);
			
			// Selection Mode Parameter
			if(pType.hasMultipleDifferentValues == false)
			{
				Parallax.ParallaxTypes ParallaxType = (Parallax.ParallaxTypes)pType.intValue;

				if ( ParallaxType == Parallax.ParallaxTypes.Equally )
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("parallax"), true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("lockedAxes"), true);
				}
				else
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("parallaxAxisAmt"), true);
				}

				EditorGUILayout.PropertyField( serializedObject.FindProperty("ParallaxOffset"), true);
				EditorGUILayout.PropertyField( serializedObject.FindProperty("editorPreview"), true);
			}
			
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			{

				//Cameras
				EditorGUI.BeginChangeCheck();

				if (targetParallax.listOfCameras == null || targetParallax.listOfCameras.Count == 0 || Camera.allCameras.Length != targetParallax.listOfCameras.Count )
					targetParallax.listOfCameras = new List<Camera>( Camera.allCameras );

				string[] listCameraNames = new string[targetParallax.listOfCameras.Count+1];
				for (int i = 0; i < targetParallax.listOfCameras.Count; ++i)
				{
					listCameraNames[i] =  targetParallax.listOfCameras[i].name;
				}
				
				targetParallax.selectedCameraIndex = EditorGUILayout.Popup(targetParallax.selectedCameraIndex, listCameraNames);
				
				if (EditorGUI.EndChangeCheck())
				{
					
					// do something to the camera..
					
				}


			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}