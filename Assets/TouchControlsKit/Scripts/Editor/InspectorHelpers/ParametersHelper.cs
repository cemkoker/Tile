/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 ParametersHelper.cs                   *
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
    public sealed class ParametersHelper
    {
        private static ControllerBase myTarget = null;


        // HelperSetup
        public static void HelperSetup( ControllerBase target )
        {
            myTarget = target;
        }


        // ShowName
        public static void ShowName( string name )
        {
            StyleHelper.StandartSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label( name, GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.MyName = EditorGUILayout.TextField( myTarget.MyName );
            GUILayout.EndHorizontal();
        }

        // ShowSensitivity
        public static void ShowSensitivity()
        {
            StyleHelper.StandartSpace();            
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Sensitivity", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.sensitivity = EditorGUILayout.Slider( myTarget.sensitivity, 1f, 10f );
            GUILayout.EndHorizontal();
        }

        // ShowTouchZone
        public static void ShowTouchZone()
        {
            StyleHelper.StandartSpace();
            GUILayout.BeginHorizontal();
            myTarget.ShowTouchZone = EditorGUILayout.Toggle( myTarget.ShowTouchZone, GUILayout.Width( 15f ) );
            GUILayout.Label( "TZone Sprite", GUILayout.Width( StyleHelper.STANDART_SIZE - 20f ) );
            GUI.enabled = myTarget.ShowTouchZone;
            myTarget.touchzoneImage.color = EditorGUILayout.ColorField( myTarget.touchzoneImage.color, GUILayout.Width( StyleHelper.STANDART_SIZE / 2f ) );
            myTarget.touchzoneImage.sprite = EditorGUILayout.ObjectField( myTarget.touchzoneImage.sprite, typeof( Sprite ), false ) as Sprite;
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }


        // ShowSpriteAndColor
        public static void ShowSpriteAndColor( ref Sprite sprite, ref Color32 color, string label )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( label + " Sprite", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            color = EditorGUILayout.ColorField( color, GUILayout.Width( StyleHelper.STANDART_SIZE / 2f ) );
            sprite = EditorGUILayout.ObjectField( sprite, typeof( Sprite ), false ) as Sprite;
            GUILayout.EndHorizontal();
        }

        // ShowSpriteAndColor
        public static void ShowSpriteAndColor( ref UnityEngine.UI.Image image, string label )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( label + " Sprite", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            image.color = EditorGUILayout.ColorField( image.color, GUILayout.Width( StyleHelper.STANDART_SIZE / 2f ) );
            image.sprite = EditorGUILayout.ObjectField( image.sprite, typeof( Sprite ), false ) as Sprite;
            GUILayout.EndHorizontal();
        }
    }
}