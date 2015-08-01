/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKSteeringWheelEditor.cs             *
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
    [CustomEditor( typeof( TCKSteeringWheel ) )]
    public class TCKSteeringWheelEditor : BaseInspector
    {
        private TCKSteeringWheel myTarget = null;


        // OnEnable
        void OnEnable()
        {
            myTarget = ( TCKSteeringWheel )target;

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

            ParametersHelper.ShowName( "Wheel Name" );
            ParametersHelper.ShowSensitivity();

            StyleHelper.StandartSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Max Steering Angle", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.maxSteeringAngle = EditorGUILayout.Slider( myTarget.maxSteeringAngle, 36f, 360f );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Released Speed", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.releasedSpeed = EditorGUILayout.Slider( myTarget.releasedSpeed, 25f, 150f );
            GUILayout.EndHorizontal();

            StyleHelper.StandartSpace();
            ParametersHelper.ShowSpriteAndColor( ref myTarget.touchzoneImage, "Wheel" );

            StyleHelper.StandartSpace();
            GUILayout.EndVertical();

            AxesHelper.ShowAxes( true );
            EventsHelper.ShowEvents();
        }
    }
}