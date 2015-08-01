/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKDPad.cs                            *
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
    public class TCKDPad : ControllerBase,
        IPointerUpHandler, IPointerDownHandler, IDragHandler, IPointerClickHandler
    {
        public Sprite normalSprite, pressedSprite;
        public Color32 normalColor, pressedColor;

        [SerializeField]
        private TCKDPadArrow[] myArrows = null;
        private Vector2 borderPosition = Vector2.zero;
        private float sizeX, sizeY;


        // ControlAwake
        public override void ControlAwake()
        {
            base.ControlAwake();

            myArrows = GetComponentsInChildren<TCKDPadArrow>();
            for( int cnt = 0; cnt < myArrows.Length; cnt++ )
                myArrows[ cnt ].DPadArrowAwake( normalSprite, normalColor );
        }


        // UpdatePosition
        internal override void UpdatePosition( Vector2 touchPos )
        {
            if( !axisX.enabled && !axisY.enabled )
                return;

            if( touchDown )
            {
                GetCurrentPosition( touchPos );

                currentDirection = currentPosition - defaultPosition;

                float borderSizeX = 0f;
                float borderSizeY = 0f;

                CalculateBorderSize( out borderSizeX, out borderSizeY );

                borderPosition = defaultPosition;
                borderPosition.x += currentDirection.normalized.x * borderSizeX;
                borderPosition.y += currentDirection.normalized.y * borderSizeY;

                float currentDistance = Vector2.Distance( defaultPosition, currentPosition );

                if( currentDistance > borderSizeX || currentDistance > borderSizeY )
                    currentPosition = borderPosition; //Debug.DrawLine( defaultPosition, currentPosition );

                float aX = 0f;
                float aY = 0f;

                for( int cnt = 0; cnt < myArrows.Length; cnt++ )
                {
                    if( myArrows[ cnt ].CheckTouchPosition( currentPosition, sizeX, sizeY ) )
                    {
                        ArrowDown( cnt );

                        if( myArrows[ cnt ].ArrowType == TCKDPadArrow.ArrowTypes.UP
                            || myArrows[ cnt ].ArrowType == TCKDPadArrow.ArrowTypes.DOWN )
                            aY = myArrows[ cnt ].INDEX * sensitivity;

                        if( myArrows[ cnt ].ArrowType == TCKDPadArrow.ArrowTypes.RIGHT
                            || myArrows[ cnt ].ArrowType == TCKDPadArrow.ArrowTypes.LEFT )
                            aX = myArrows[ cnt ].INDEX * sensitivity;
                    }
                    else
                    {
                        ArrowUp( cnt );

                        if( myArrows[ cnt ].ArrowType == TCKDPadArrow.ArrowTypes.UP && myArrows[ cnt ].INDEX == 0f )
                        {
                            for( int dnt = 0; dnt < myArrows.Length; dnt++ )
                                if( myArrows[ dnt ].ArrowType == TCKDPadArrow.ArrowTypes.DOWN && myArrows[ dnt ].INDEX == 0f )
                                    aY = myArrows[ dnt ].INDEX * sensitivity;
                        }

                        if( myArrows[ cnt ].ArrowType == TCKDPadArrow.ArrowTypes.RIGHT && myArrows[ cnt ].INDEX == 0f )
                        {
                            for( int dnt = 0; dnt < myArrows.Length; dnt++ )
                                if( myArrows[ dnt ].ArrowType == TCKDPadArrow.ArrowTypes.LEFT && myArrows[ dnt ].INDEX == 0f )
                                    aX = myArrows[ dnt ].INDEX * sensitivity;
                        }
                    }
                }

                aX = ( axisX.inverse ) ? -aX : aX;
                aY = ( axisX.inverse ) ? -aY : aY;

                SetAxis( aX, aY );
            }
            else
            {
                touchDown = true;

                UpdatePosition( touchPos );

                // Broadcasting
                DownHandler();
            }
        }

        // GetCurrentPosition
        protected void GetCurrentPosition( Vector2 touchPos )
        {
            if( axisX.enabled )
                currentPosition.x = GuiCamera.ScreenToWorldPoint( touchPos ).x;
            if( axisY.enabled )
                currentPosition.y = GuiCamera.ScreenToWorldPoint( touchPos ).y;

            sizeX = touchzoneRect.sizeDelta.x / 2f;
            sizeY = touchzoneRect.sizeDelta.y / 2f;
            defaultPosition = touchzoneRect.position;
        }

        // CalculateBorderSize
        protected void CalculateBorderSize( out float calcX, out float calcY )
        {
            calcX = touchzoneRect.sizeDelta.x / 6f;
            calcY = touchzoneRect.sizeDelta.y / 6f;
        }

        // ArrowDown
        protected void ArrowDown( int index )
        {
            if( !myArrows[ index ].isPressed )
                myArrows[ index ].SetArrowActive( pressedSprite, pressedColor, true );
        }

        // ArrowUp
        protected void ArrowUp( int index )
        {
            if( myArrows[ index ].isPressed )
                myArrows[ index ].SetArrowActive( normalSprite, normalColor, false );
        }

        // ControlReset
        internal override void ControlReset()
        {
            base.ControlReset();

            for( int cnt = 0; cnt < myArrows.Length; cnt++ )
                ArrowUp( cnt );

            // Broadcasting
            UpHandler();
        }


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

        // OnPointerUp
        public void OnPointerUp( PointerEventData pointerData )
        {
            UpdatePosition( pointerData.position );
            ControlReset();
        }

        // OnPointerClick
        public void OnPointerClick( PointerEventData pointerData )
        {
            ClickHandler();
        }
    }
}