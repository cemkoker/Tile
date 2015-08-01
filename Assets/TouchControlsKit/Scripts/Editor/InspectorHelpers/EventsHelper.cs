/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 EventsHelper.cs                       *
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
    public sealed class EventsHelper
    {
        private static string[] stateNames = { "OFF", "ON" };
        private static ControllerBase myTarget = null;
        private static SerializedObject serializedObject = null;
        private static SerializedProperty downEventProp, pressEventProp, upEventProp, clickEventProp;


        // HelperSetup
        public static void HelperSetup( ControllerBase target, SerializedObject serObject )
        {
            myTarget = target;
            serializedObject = serObject;

            downEventProp = serializedObject.FindProperty( "downEvent" );
            pressEventProp = serializedObject.FindProperty( "pressEvent" );
            upEventProp = serializedObject.FindProperty( "upEvent" );
            clickEventProp = serializedObject.FindProperty( "clickEvent" );
        }

        /// <summary>
        /// ShowEvents
        /// </summary>
        /// <param name="size"></param>
        public static void ShowEvents()
        {
            serializedObject.Update();

            StyleHelper.StandartSpace();
            GUILayout.BeginVertical( "Box" );
            GUILayout.Label( "Events", StyleHelper.labelStyle );
            StyleHelper.StandartSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Broadcasting", GUILayout.Width( StyleHelper.STANDART_SIZE ) );
            myTarget.broadcast = System.Convert.ToBoolean( GUILayout.Toolbar( System.Convert.ToInt32( myTarget.broadcast ), stateNames, GUILayout.Height( 15 ) ) );
            GUILayout.EndHorizontal();

            StyleHelper.StandartSpace();

            if( myTarget.broadcast )
            {
                EditorGUILayout.PropertyField( downEventProp, false, null );
                EditorGUILayout.PropertyField( pressEventProp, false, null );
                EditorGUILayout.PropertyField( upEventProp, false, null );
                EditorGUILayout.PropertyField( clickEventProp, false, null );
            }

            StyleHelper.StandartSpace();
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}