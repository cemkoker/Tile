using UnityEngine;
using System;
using System.Reflection;
using RTStudio;

namespace RTStudio.Helpers
{
	[ExecuteInEditMode]
	public class Sprite2DSorting : MonoBehaviour
	{
	    public string layerName = "";       // The name of the sorting layer the particles should be set to.
	    public int renderLayer = 0;
	    public bool everyframe = false;
	    public bool useYasSort = false;

		/// ======================================================================================================================================================================
		/// <summary>
		/// Ons the enable.
		/// </summary>
		/// ======================================================================================================================================================================
		void onEnable()
	    {

	        if (GetComponent<Renderer>() != null)
	        {
	            GetComponent<Renderer>().sortingLayerName = layerName;
	            GetComponent<Renderer>().sortingOrder = renderLayer;
	        }
	    }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// ======================================================================================================================================================================
		void Start()
	    {
	        if (GetComponent<Renderer>() != null)
	        {
	            GetComponent<Renderer>().sortingLayerName = layerName;
	            GetComponent<Renderer>().sortingOrder = renderLayer;
	        }
	    }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Update this instance.
		/// </summary>
		/// ======================================================================================================================================================================
		void Update()
	    {
	        if (GetComponent<Renderer>() != null && everyframe)
	        {
	            GetComponent<Renderer>().sortingLayerName = layerName;
	            GetComponent<Renderer>().sortingOrder = renderLayer;
	            if ( useYasSort ) 
	            {
	                GetComponent<Renderer>().sortingOrder = -(int)((gameObject.transform.position.y*100.0f));
	            }
	        }
	    }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Ons the update.
		/// </summary>
		/// ======================================================================================================================================================================
		void onUpdate()
	    {
	        if (GetComponent<Renderer>() != null)
	        {
	            GetComponent<Renderer>().sortingLayerName = layerName;
	            GetComponent<Renderer>().sortingOrder = renderLayer;
	        }
	    }

	}
}
