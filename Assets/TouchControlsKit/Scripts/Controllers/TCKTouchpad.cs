/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKTouchpad.cs                        *
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

namespace TouchControlsKit
{
    [RequireComponent( typeof( Image ) )]
    public class TCKTouchpad : ControllerBase,
        IPointerUpHandler, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerClickHandler
    {
        private GameObject prevPointerPressGO = null; 

                
        // UpdatePosition
        internal override void UpdatePosition( Vector2 touchPos )
        {
            if( !axisX.enabled && !axisY.enabled )
                return;
            
            if( touchDown )
            {
                if( axisX.enabled ) currentPosition.x = touchPos.x;
                if( axisY.enabled ) currentPosition.y = touchPos.y;

                currentDirection = currentPosition - defaultPosition;
                
                float touchForce = Vector2.Distance( defaultPosition, currentPosition ) * 2f;
                defaultPosition = currentPosition;

                float aX = currentDirection.normalized.x * touchForce / 100f * sensitivity;
                float aY = currentDirection.normalized.y * touchForce / 100f * sensitivity;

                aX = ( axisX.inverse ) ? -aX : aX;
                aY = ( axisX.inverse ) ? -aY : aY;

                SetAxis( aX, aY );
            }
            else
            {
                touchDown = true;
                currentPosition = defaultPosition = touchPos;
                UpdatePosition( touchPos );

                // Broadcasting
                DownHandler();
            }
        }

        // ControlReset
        internal override void ControlReset()
        {
            base.ControlReset();

            // Broadcasting
            UpHandler();
        }
        

        // OnPointerEnter
        public void OnPointerEnter( PointerEventData pointerData )
        {
            if( !pointerData.pointerPress )
                return;

            if( pointerData.pointerPress == gameObject )
            {
                OnPointerDown( pointerData );
                return;
            }

            TCKButton btn = pointerData.pointerPress.GetComponent<TCKButton>();
            if( btn && btn.swipeOut )
            {
                prevPointerPressGO = pointerData.pointerPress;
                pointerData.pointerDrag = gameObject;
                pointerData.pointerPress = gameObject;
                OnPointerDown( pointerData );
            }
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
            {
                UpdatePosition( pointerData.position );
                StopCoroutine( "UpdateEndPosition" );
                StartCoroutine( "UpdateEndPosition", pointerData.position );
            }
        }

        // UpdateEndPosition
        private System.Collections.IEnumerator UpdateEndPosition( Vector2 position )
        {
            yield return new WaitForSeconds( .0025f );

            if( touchDown )
                UpdatePosition( position );
            else
                ControlReset();
        }

        // OnPointerUp
        public void OnPointerUp( PointerEventData pointerData )
        {
            if( prevPointerPressGO )
            {
                ExecuteEvents.Execute<IPointerUpHandler>( prevPointerPressGO, pointerData, ExecuteEvents.pointerUpHandler );
                prevPointerPressGO = null;
            }

            ControlReset();
        }

        // OnPointerClick
        public void OnPointerClick( PointerEventData pointerData )
        {
            ClickHandler();
        }
    }
}