/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 ControllerBase.cs                     *
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
using UnityEngine.Events;
using UnityEngine.UI;


namespace TouchControlsKit
{       
    // ControllerBase
    public abstract class ControllerBase : MonoBehaviour
    {
        public RectTransform touchzoneRect = null;
        public Image touchzoneImage = null;
        
        public float sensitivity = 1f;

        [SerializeField]
        protected string myName = "NONAME_Controller";

        [SerializeField]
        protected bool showTouchZone = true;

        [SerializeField]
        protected Color32 nativeColor;

        public Axes axisX = new Axes( "Horizontal" );
        public Axes axisY = new Axes( "Vertical" );
        
        public bool broadcast = false;

        protected int touchId = -1;
        protected bool touchDown = false;


        [System.Serializable]
        public class TCKEvent : UnityEvent<float, float> { }

        [SerializeField]
        public TCKEvent downEvent, pressEvent, upEvent, clickEvent;
        
        protected Vector2 defaultPosition, currentPosition, currentDirection;
        

        // MyName
        public string MyName
        {
            get { return myName; }
            set
            {
                if( myName == value ) 
                    return;

                if( value == string.Empty )
                {
                    Debug.LogError( "ERROR: Controller name for " + gameObject.name + " cannot be empty" );
                    return;
                }

                myName = value;
                gameObject.name = myName;
            }
        }


        public bool ShowTouchZone
        {
            get { return showTouchZone; }

            set
            {
                if( showTouchZone == value )
                    return;

                showTouchZone = value;
                ShowHideTouchZone();                
            }
        }

        
        // ControlAwake
        public virtual void ControlAwake()
        {
            touchzoneRect = gameObject.GetComponent<RectTransform>();
            touchzoneImage = gameObject.GetComponent<Image>();
        }
        
        
        // UpdatePosition
        internal abstract void UpdatePosition( Vector2 touchPos );

        // ControlReset
        internal virtual void ControlReset()
        {
            touchId = -1;
            touchDown = false;
            SetAxis( 0f, 0f );
        }


        // ShowHideTouchZone
        protected virtual void ShowHideTouchZone()
        {
            if( showTouchZone )
            {
                touchzoneImage.color = nativeColor;
            }
            else
            {
                nativeColor = touchzoneImage.color;
                Color32 tmpColor = nativeColor;
                tmpColor.a = 0;
                touchzoneImage.color = tmpColor;
            }
        }


        // SetAxis
        protected void SetAxis( float x, float y )
        {
            axisX.SetValue( x );
            axisY.SetValue( y );
        }
        
        
        // DownHandler
        protected void DownHandler()
        {
            if( broadcast )
                downEvent.Invoke( axisX.value, axisY.value );
        }


        internal void PressHandler()
        {
            if( broadcast && touchDown )
                pressEvent.Invoke( axisX.value, axisY.value );
        }        
        
        // UpHandler
        protected void UpHandler()
        {
            if( broadcast )
                upEvent.Invoke( axisX.value, axisY.value );
        }

        // ClickHandler
        protected void ClickHandler()
        {
            if( broadcast )
                clickEvent.Invoke( axisX.value, axisY.value );
        }
    }
}