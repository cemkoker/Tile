/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKDPadEditor.cs                      *
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
    [CustomEditor( typeof( TCKDPad ) )]
    public class TCKDPadEditor : BaseInspector
    {
        private TCKDPad myTarget = null;


        // OnEnable
        void OnEnable()
        {
            myTarget = ( TCKDPad )target;

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

            ParametersHelper.ShowName( "DPad Name" );
            ParametersHelper.ShowSensitivity();
            ParametersHelper.ShowTouchZone();

            StyleHelper.StandartSpace();

            Sprite sprite = myTarget.normalSprite;
            ParametersHelper.ShowSpriteAndColor( ref sprite, ref myTarget.normalColor, "Norm Arrow" );
            myTarget.normalSprite = sprite;

            if( GUI.changed )
                myTarget.ControlAwake();

            ParametersHelper.ShowSpriteAndColor( ref myTarget.pressedSprite, ref myTarget.pressedColor, "Press Arrow" );

            StyleHelper.StandartSpace();
            GUILayout.EndVertical();

            AxesHelper.ShowAxes();
            EventsHelper.ShowEvents();
        }
    }
}