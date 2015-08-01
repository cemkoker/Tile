using UnityEngine;
using System.Collections;
using RTStudio;

namespace RTStudio.Helpers
{

	public class SubSceneLoader : MonoBehaviour 
	{

	    public string[] ScenesToLoadFirst;
	    //public string[] ImmediatelyDisable;

		/// ======================================================================================================================================================================
		/// <summary>
	    /// Awake this instance.
	    /// </summary>
		/// ======================================================================================================================================================================
		void Awake() {
	      
	        foreach ( string s in ScenesToLoadFirst )
	        {
	            if ( !GameObject.Find(s) )
	            {
	                Debug.Log("Did not find " + s + " requested by " + this.gameObject.name + " added now");
	                Application.LoadLevelAdditive(s);
	            }
	        }

	    }

		/// ======================================================================================================================================================================
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// ======================================================================================================================================================================

		void Start()
	    {
	        // List of objects not needed
	      /*  if ( ImmediatelyDisable != null )
	        {
	            foreach ( string s in ImmediatelyDisable )
	            {
	                GameObject found = GameObject.Find(s);
	                
	                if ( found )
	                    found.SetActive ( false );
	                else 
	                    Debug.Log("Did not find " + s);
	            }
	        }*/
	    }

	}

}
