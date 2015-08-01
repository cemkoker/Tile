/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKButtonEditor.cs                    *
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
    [CustomEditor( typeof( TCKButton ) )]
    public class TCKButtonEditor : BaseInspector
    {
        private TCKButton myTarget = null;


        // OnEnable
        void OnEnable()
        {
            myTarget = ( TCKButton )target;

            ParametersHelper.HelperSetup( myTarget );
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

            ParametersHelper.ShowName( "Button Name" );

            StyleHelper.StandartSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Swipe Out", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.swipeOut = EditorGUILayout.Toggle( myTarget.swipeOut );
            GUILayout.EndHorizontal();

            StyleHelper.StandartSpace();

            Sprite sprite = myTarget.normalSprite;
            ParametersHelper.ShowSpriteAndColor( ref sprite, ref myTarget.normalColor, "Normal" );
            myTarget.normalSprite = sprite;
            myTarget.touchzoneImage.color = myTarget.normalColor;

            ParametersHelper.ShowSpriteAndColor( ref myTarget.pressedSprite, ref myTarget.pressedColor, "Pressed" );
            
            StyleHelper.StandartSpace();
            GUILayout.EndVertical();

            EventsHelper.ShowEvents();
        }
    }
}