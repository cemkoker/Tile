/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKTouchpadEditor.cs                  *
 * 													   *
 * Copyright(c): Victor Klepikov					   *
 * Support: 	 http://bit.ly/vk-Support			   *
 * 													   *
 * mySite:       http://vkdemos.ucoz.org			   *
 * myAssets:     http://u3d.as/5Fb                     *
 * myTwitter:	 http://twitter.com/VictorKlepikov	   *
 * 													   *
 *******************************************************/


using UnityEngine;
using UnityEditor;

namespace TouchControlsKit.Inspector
{
    [CustomEditor( typeof( TCKTouchpad ) )]
    public class TCKTouchpadEditor : BaseInspector
    {
        private TCKTouchpad myTarget = null;


        // OnEnable
        void OnEnable()
        {
            myTarget = ( TCKTouchpad )target;

            ParametersHelper.HelperSetup( myTarget );
            AxesHelper.HelperSetup( myTarget );
            EventsHelper.HelperSetup( myTarget, serializedObject );
        }


        // Dirty
        protected override void Dirty()
        {
            EditorUtility.SetDirty( myTarget );
        } 

        // ShowParameters
        protected override void ShowParameters()
        {
            GUILayout.BeginVertical( "Box" );
            GUILayout.Label( "Parameters", StyleHelper.labelStyle );

            ParametersHelper.ShowName( "Touchpad Name" );
            ParametersHelper.ShowSensitivity();
            ParametersHelper.ShowTouchZone();

            StyleHelper.StandartSpace();
            GUILayout.EndVertical();
            
            AxesHelper.ShowAxes();
            EventsHelper.ShowEvents();  
        }
    }
}