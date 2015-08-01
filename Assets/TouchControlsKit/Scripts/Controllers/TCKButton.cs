/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKButton.cs                          *
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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TouchControlsKit.Utils;

namespace TouchControlsKit
{
    [RequireComponent( typeof( Image ) )]
    public class TCKButton : ControllerBase,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public bool swipeOut = false;

        [SerializeField]
        private Sprite normalsprite = null;
        public Sprite pressedSprite = null;

        public Color32 normalColor, pressedColor;

        public Sprite normalSprite
        {
            get { return normalsprite; }
            set
            {
                if( normalsprite == value )
                    return;

                normalsprite = value;
                touchzoneImage.sprite = normalsprite;
            }
        }        
        
        private int pressedFrame = -1;
        private int releasedFrame = -1;

        // ButtonPRESSED
        internal bool ButtonPRESSED
        {
            get
            {
                return touchDown;
            }
        }

        // ButtonDOWN
        internal bool ButtonDOWN
        {
            get
            {
                return ( pressedFrame == Time.frameCount - 1 );
            }
        }

        // ButtonUP
        internal bool ButtonUP
        {
            get
            {
                return ( releasedFrame == Time.frameCount - 1 );
            }
        }
                
        
        // UpdatePosition
        internal override void UpdatePosition( Vector2 touchPos )
        {
            if( !touchDown )
            {
                touchDown = true;
                pressedFrame = Time.frameCount;
                ButtonDown();

                // Broadcasting
                DownHandler(); 
            }
        }
                
        // ButtonDown
        protected void ButtonDown()
        {
            touchzoneImage.sprite = pressedSprite;
            touchzoneImage.color = pressedColor;
        }

        // ButtonUp
        protected void ButtonUp()
        {
            touchzoneImage.sprite = normalSprite;
            touchzoneImage.color = normalColor;
        }

        // ControlReset
        internal override void ControlReset()
        {
            base.ControlReset();

            releasedFrame = Time.frameCount;
            ButtonUp();

            // Broadcasting
            UpHandler();
        }

        // ShowHideTouchZone
        protected override void ShowHideTouchZone()
        { }

        // OnPointerDown
        public void OnPointerDown( PointerEventData pointerData )
        {
            if( !touchDown )
            {
                touchId = pointerData.pointerId;
                UpdatePosition( pointerData.position );
            }
        }

        // OnDrag
        public void OnDrag( PointerEventData pointerData )
        {
            if( Input.touchCount >= touchId && touchDown )
                UpdatePosition( pointerData.position );
        }

        // OnPointerExit
        public void OnPointerExit( PointerEventData pointerData )
        {
            if( !swipeOut )
                OnPointerUp( pointerData );
        }

        // OnPointerUp
        public void OnPointerUp( PointerEventData pointerData )
        {
            ControlReset();
        }

        // OnPointerClick
        public void OnPointerClick( PointerEventData pointerData )
        {
            ClickHandler();
        }
    }
}