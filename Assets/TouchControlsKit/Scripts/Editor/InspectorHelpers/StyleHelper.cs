/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 StyleHelper.cs                        *
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
    public sealed class StyleHelper
    {
        public const float STANDART_SIZE = 115f;
        
        private static GUIStyle label = new GUIStyle( EditorStyles.label );


        // labelStyle
        public static GUIStyle labelStyle
        {
            get
            {
                label.fontStyle = FontStyle.Bold;
                label.alignment = TextAnchor.UpperCenter;
                label.fontSize = 13;
                label.normal.textColor = Color.red;
                return label;
            }
        }


        // StandartSpace
        public static void StandartSpace()
        {
            GUILayout.Space( 5f );
        }
    }
}