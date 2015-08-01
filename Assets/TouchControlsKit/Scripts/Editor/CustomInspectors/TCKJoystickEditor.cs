/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKJoystickEditor.cs                  *
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
    [CustomEditor( typeof( TCKJoystick ) )]
    public class TCKJoystickEditor : BaseInspector
    {
        private TCKJoystick myTarget = null;
        private static string[] modNames = { "Dynamic", "Static" };


        // OnEnable
        void OnEnable()
        {
            myTarget = ( TCKJoystick )target;

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

            ParametersHelper.ShowName( "Joystick Name" );

            StyleHelper.StandartSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Mode", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.IsStatic = System.Convert.ToBoolean( GUILayout.Toolbar( System.Convert.ToInt32( myTarget.IsStatic ), modNames, GUILayout.Height( 20 ) ) );
            GUILayout.EndHorizontal();

            ParametersHelper.ShowSensitivity();

            StyleHelper.StandartSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Border Size", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.borderSize = EditorGUILayout.Slider( myTarget.borderSize, 1f, 9f );
            GUILayout.EndHorizontal();

            StyleHelper.StandartSpace();

            if( myTarget.IsStatic )
            {
                GUILayout.BeginHorizontal();
                myTarget.smoothReturn = EditorGUILayout.Toggle( myTarget.smoothReturn, GUILayout.Width( 15f ) );
                GUILayout.Label( "Smooth Return", GUILayout.Width( StyleHelper.STANDART_SIZE - 20f ) );
                GUI.enabled = myTarget.smoothReturn;
                myTarget.smoothFactor = EditorGUILayout.Slider( myTarget.smoothFactor, 1f, 20f );
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            ParametersHelper.ShowTouchZone();

            StyleHelper.StandartSpace();
            ParametersHelper.ShowSpriteAndColor( ref myTarget.joystickImage, "Joystick" );
            ParametersHelper.ShowSpriteAndColor( ref myTarget.joystickBackgroundImage, "Background" );

            StyleHelper.StandartSpace();
            GUILayout.EndVertical();

            AxesHelper.ShowAxes();
            EventsHelper.ShowEvents();
        }
    }
}