/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKSteeringWheel.cs                   *
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
    public class TCKSteeringWheel : ControllerBase,
        IPointerUpHandler, IPointerDownHandler, IDragHandler, IPointerClickHandler
    {
        public float maxSteeringAngle = 120f;
        public float releasedSpeed = 45f;

        private Vector3 localEulerAngles = Vector3.zero;
        private float wheelAngle, wheelPrevAngle;

        
        // UpdatePosition
        internal override void UpdatePosition( Vector2 touchPos )
        {
            if( !axisX.enabled )
                return;
            
            GetCurrentPosition( touchPos );

            if( touchDown )
            {
                float wheelNewAngle = Vector2.Angle( Vector2.up, currentPosition - defaultPosition );

                if( currentPosition.x > defaultPosition.x )
                    wheelAngle += wheelNewAngle - wheelPrevAngle;
                else
                    wheelAngle -= wheelNewAngle - wheelPrevAngle;

                wheelAngle = Mathf.Clamp( wheelAngle, -maxSteeringAngle, maxSteeringAngle );
                wheelPrevAngle = wheelNewAngle;

                UptateWheelRotation();

                float aX = wheelAngle / maxSteeringAngle * sensitivity;
                aX = ( axisX.inverse ) ? -aX : aX;

                SetAxis( aX, 0f );
            }
            else
            {
                touchDown = true;
                StopCoroutine( "WheelReturnRun" );
                wheelPrevAngle = Vector2.Angle( Vector2.up, currentPosition - defaultPosition );

                UpdatePosition( touchPos );

                // Broadcasting
                DownHandler();
            }
        }

        // GetCurrentPosition
        private void GetCurrentPosition( Vector2 touchPos )
        {
            defaultPosition = currentPosition = touchzoneRect.position;
            currentPosition.x = GuiCamera.ScreenToWorldPoint( touchPos ).x;
            currentPosition.y = GuiCamera.ScreenToWorldPoint( touchPos ).y;
        }

        // UptateWheelRotation
        private void UptateWheelRotation()
        {
            localEulerAngles = Vector3.back * wheelAngle;
            touchzoneRect.localEulerAngles = localEulerAngles;
        }

        // ControlReset
        internal override void ControlReset()
        {
            base.ControlReset();

            StopCoroutine( "WheelReturnRun" );
            StartCoroutine( "WheelReturnRun" );

            // Broadcasting
            UpHandler();
        }

        // ShowHideTouchZone
        protected override void ShowHideTouchZone()
        { }

        // WheelReturnRun
        private System.Collections.IEnumerator WheelReturnRun()
        {
            float deltaAngle = 0f;            
            while( !Mathf.Approximately( 0f, wheelAngle ) )
            {
                deltaAngle = releasedSpeed * Time.smoothDeltaTime * 10f;
                //
                if( Mathf.Abs( deltaAngle ) > Mathf.Abs( wheelAngle ) )                
                    wheelAngle = 0f;                
                else if( wheelAngle > 0f )                
                    wheelAngle -= deltaAngle;                
                else                
                    wheelAngle += deltaAngle;
                //
                UptateWheelRotation();
                yield return null;
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