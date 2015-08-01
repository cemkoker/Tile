using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Reflection;
using System.Collections;
using RTStudio;


namespace RTStudio.Helpers
{
	[CustomEditor(typeof(RTStudio.Helpers.Sprite2DSorting))]
	public class Sprite2DSortingEditor : Editor
	{
	    
	    private SerializedProperty LayerName, RenderLayer, Everyframe, UseYasSort;
	    private int layerIndex = 0;
	    private int order = 0;
	    private bool everyframe = false;
	    private bool useYasSort = false;
	    public string[] layerArray;
	    
		/// ======================================================================================================================================================================
		/// <summary>
	    ///  On Enable called when the script is enabled
	    /// </summary>
		/// ======================================================================================================================================================================
		void onEnable()
	    {       
	        LayerName = serializedObject.FindProperty("layerName");
	        RenderLayer = serializedObject.FindProperty("renderLayer");
	        Everyframe = serializedObject.FindProperty("everyframe");
	        UseYasSort = serializedObject.FindProperty("useYasSort");
	    }
	    
		/// ======================================================================================================================================================================
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// ======================================================================================================================================================================
		void Start()
	    {
	    
	    }
	    
		/// ======================================================================================================================================================================
		/// <summary>
		/// Raises the inspector GU event.
		/// </summary>
		/// ======================================================================================================================================================================
		override public void OnInspectorGUI()
	    {
			RTStudio.Helpers.Sprite2DSorting component = (RTStudio.Helpers.Sprite2DSorting)target;

	        LayerName = serializedObject.FindProperty("layerName");
	        RenderLayer = serializedObject.FindProperty("renderLayer");
	        Everyframe = serializedObject.FindProperty("everyframe");
	        UseYasSort = serializedObject.FindProperty("useYasSort");
	    
	        order = RenderLayer.intValue;
	        useYasSort = UseYasSort.boolValue;
	        everyframe = Everyframe.boolValue;

	        if (component != null)
	        {
	            
	            String[] tempArray = GetSortingLayerNames();
	            layerArray = new String[tempArray.Length + 1];
	            layerArray [0] = "<None>";
	                            
	            for (int i = 0; i < layerArray.Length - 1; i++)
	            {
	                String name = tempArray [i];
	                layerArray [i + 1] = name;
	                                                
	                if (name == LayerName.stringValue)
	                    layerIndex = i + 1;
	            }
	                        
	            EditorGUILayout.BeginHorizontal();
	            EditorGUILayout.LabelField("Layer");
	            EditorGUIUtility.LookLikeControls();
	            layerIndex = EditorGUILayout.Popup(layerIndex, layerArray);
	            EditorGUILayout.EndHorizontal();
	                        
	            EditorGUILayout.BeginHorizontal();
	            EditorGUILayout.LabelField("Order");
	            EditorGUIUtility.LookLikeControls();
	            order = EditorGUILayout.IntField(order);
	            EditorGUILayout.EndHorizontal();

	            EditorGUILayout.BeginHorizontal();
	            EditorGUILayout.LabelField("Every Frame");
	            EditorGUIUtility.LookLikeControls();
	            everyframe = EditorGUILayout.Toggle( everyframe );
	            EditorGUILayout.EndHorizontal();

	            EditorGUILayout.BeginHorizontal();
	            EditorGUILayout.LabelField("Y Position to sort");
	            EditorGUIUtility.LookLikeControls();
	            useYasSort = EditorGUILayout.Toggle( useYasSort );
	            EditorGUILayout.EndHorizontal();
	                        
	            if (order == 0)
	            {
	                RenderLayer.intValue = 0;
	                component.renderLayer = 0;
	            } else
	            {
	                RenderLayer.intValue = order;
	                component.renderLayer = order;
	            }

	            if (everyframe == false)
	            {
	                Everyframe.boolValue = false;
	                component.everyframe = false;
	            } else
	            {
	                Everyframe.boolValue = true;
	                component.everyframe = true;
	            }

	            if (useYasSort == false)
	            {
	                UseYasSort.boolValue = false;
	                component.useYasSort = false;
	            } else
	            {
	                UseYasSort.boolValue = true;
	                component.useYasSort = true;
	            }
	            
	            if (layerIndex == 0)
	            {
	                LayerName.stringValue = "";
	                component.layerName = "";
	            } else
	            {
	                LayerName.stringValue = layerArray [layerIndex];
	                component.layerName = layerArray [layerIndex];
	            }
	            
	        }
	                    
	        if (serializedObject.ApplyModifiedProperties())
	        {
	            LayerName.stringValue = component.layerName = layerArray [layerIndex];
	        }
	    }
	    
		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the sorting layer names.
		/// </summary>
		/// <returns>The sorting layer names.</returns>
		/// ======================================================================================================================================================================
		public string[] GetSortingLayerNames()
	    {
	        Type internalEditorUtilityType = typeof(InternalEditorUtility);
	        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
	        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	    }
	    
	}
}
