/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKInput.cs                           *
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

namespace TouchControlsKit
{
    public sealed class TCKInput : MonoBehaviour
    {
        private static ControllerBase[] controllers = null;
        private static int controllersCount = 0;

        private static TCKButton[] buttons = null;
        private static int buttonsCount = 0;


        private static TCKInput instance = null;

        internal static TCKInput Instance
        {
            get
            {
                if( !instance )
                    instance = FindObjectOfType<TCKInput>();

                return instance;
            }
        }


        // Awake
        void Awake()
        {
            instance = this;
            SetActive( true );
        }

        // Update
        void Update()
        {
            for( int cnt = 0; cnt < controllersCount; cnt++ )
                controllers[ cnt ].PressHandler();
        }


        /// <summary>
        /// SetActive
        /// </summary>
        /// <param name="value"></param>
        internal static void SetActive( bool value )
        {
            Instance.enabled = value;
            Instance.gameObject.SetActive( value );

            if( Instance.enabled )
            {
                controllers = Instance.gameObject.GetComponentsInChildren<ControllerBase>();
                controllersCount = controllers.Length;

                buttons = Instance.gameObject.GetComponentsInChildren<TCKButton>();
                buttonsCount = buttons.Length;
                
                for( int cnt = 0; cnt < controllersCount; cnt++ )
                    controllers[ cnt ].ControlAwake();
            }
            else
            {
                controllers = buttons = null;
                controllersCount = buttonsCount = 0;
            }
        }


        /// <summary>
        /// Returns the value of the joystick or touchpad Axis identified by controllerName & axisName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public static float GetAxis( string controllerName, string axisName )
        {
            if( !Instance.enabled )
                return 0f;

            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    if( axisName == controllers[ cnt ].axisX.Name )
                        return controllers[ cnt ].axisX.value;
                    else if( axisName == controllers[ cnt ].axisY.Name )
                        return controllers[ cnt ].axisY.value;

                    Debug.LogError( "Axis name: " + axisName + " not found!" );
                    return 0f;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
            return 0f;
        }


        /// <summary>
        /// Returns the value of the joystick or touchpad axis Enable identified by controllerName & axisName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public static bool GetAxisEnable( string controllerName, string axisName )
        {
            if( !Instance.enabled )
                return false;
            
            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    if( axisName == controllers[ cnt ].axisX.Name )
                        return controllers[ cnt ].axisX.enabled;
                    else if( axisName == controllers[ cnt ].axisY.Name )
                        return controllers[ cnt ].axisY.enabled;

                    Debug.LogError( "Axis name: " + axisName + " not found!" );
                    return false;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
            return false;
        }

        /// <summary>
        /// Sets the value of the joystick or touchpad axis Enable identified by controllerName & axisName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="axisName"></param>
        /// <param name="value"></param>
        public static void SetAxisEnable( string controllerName, string axisName, bool value )
        {
            if( !Instance.enabled )
                return;
            
            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    if( axisName == controllers[ cnt ].axisX.Name )
                    {
                        controllers[ cnt ].axisX.enabled = value;
                        return;
                    }
                    else if( axisName == controllers[ cnt ].axisY.Name )
                    {
                        controllers[ cnt ].axisY.enabled = value;
                        return;
                    }
                    Debug.LogError( "Axis name: " + axisName + " not found!" );
                    return;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
        }


        /// <summary>
        /// Returns the value of the joystick or touchpad axis Inverse identified by controllerName & axisName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public static bool GetAxisInverse( string controllerName, string axisName )
        {
            if( !Instance.enabled )
                return false;

            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    if( axisName == controllers[ cnt ].axisX.Name )
                        return controllers[ cnt ].axisX.inverse;
                    else if( axisName == controllers[ cnt ].axisY.Name )
                        return controllers[ cnt ].axisY.inverse;
                    Debug.LogError( "Axis name: " + axisName + " not found!" );
                    return false;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
            return false;
        }

        /// <summary>
        /// Sets the value of the joystick or touchpad axis Inverse identified by controllerName & axisName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="axisName"></param>
        /// <param name="value"></param>
        public static void SetAxisInverse( string controllerName, string axisName, bool value )
        {
            if( !Instance.enabled )
                return;

            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    if( axisName == controllers[ cnt ].axisX.Name )
                    {
                        controllers[ cnt ].axisX.inverse = value;
                        return;
                    }
                    else if( axisName == controllers[ cnt ].axisY.Name )
                    {
                        controllers[ cnt ].axisY.inverse = value;
                        return;
                    }
                    Debug.LogError( "Axis name: " + axisName + " not found!" );
                    return;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
        }
        

        /// <summary>
        /// Returns the value of the controller Sensitivity identified by controllerName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static float GetSensitivity( string controllerName )
        {
            if( !Instance.enabled )
                return 0f;

            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    return controllers[ cnt ].sensitivity;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
            return 0f;
        }

        /// <summary>
        /// Sets the Sensitivity value identified by controllerName.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="value"></param>
        public static void SetSensitivity( string controllerName, float value )
        {
            if( !Instance.enabled )
                return;

            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                if( controllers[ cnt ].MyName == controllerName )
                {
                    controllers[ cnt ].sensitivity = value;
                    return;
                }
            }
            Debug.LogError( "Controller name: " + controllerName + " not found!" );
        }


        /// <summary>
        /// Showing/Hiding touch zone for all controllers in scene.
        /// </summary>
        /// <param name="value"></param>
        public static void ShowingTouchZone( bool value )
        {
            if( !Instance.enabled )
                return;

            for( int cnt = 0; cnt < controllersCount; cnt++ )
            {
                controllers[ cnt ].ShowTouchZone = value;
            }
        }


        /// <summary>
        /// Returns true during the frame the user pressed down the touch button identified by buttonName.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonDown( string buttonName )
        {
            if( !Instance.enabled )
                return false;

            for( int cnt = 0; cnt < buttonsCount; cnt++ )
            {
                if( buttons[ cnt ].MyName == buttonName )
                {
                    return buttons[ cnt ].ButtonDOWN;
                }
            }
            Debug.LogError( "Button name: " + buttonName + " not found!" );
            return false;
        }

        /// <summary>
        /// Returns whether the given touch button is held down identified by buttonName.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool GetButton( string buttonName )
        {
            if( !Instance.enabled )
                return false;

            for( int cnt = 0; cnt < buttonsCount; cnt++ )
            {
                if( buttons[ cnt ].MyName == buttonName )
                {
                    return buttons[ cnt ].ButtonPRESSED;
                }
            }
            Debug.LogError( "Button name: " + buttonName + " not found!" );
            return false;
        }        

        /// <summary>
        /// Returns true during the frame the user releases the given touch button identified by buttonName.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonUp( string buttonName )
        {
            if( !Instance.enabled )
                return false;

            for( int cnt = 0; cnt < buttonsCount; cnt++ )
            {
                if( buttons[ cnt ].MyName == buttonName )
                {
                    return buttons[ cnt ].ButtonUP;
                }
            }
            Debug.LogError( "Button name: " + buttonName + " not found!" );
            return false;
        }
    }
}