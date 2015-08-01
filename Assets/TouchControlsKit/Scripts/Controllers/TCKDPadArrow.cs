/*******************************************************
 * 													   *
 * Asset:		 Touch Controls Kit         		   *
 * Script:		 TCKDPadArrow.cs                       *
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
using TouchControlsKit.Utils;

namespace TouchControlsKit
{
    [RequireComponent( typeof( Image ) )]
    public class TCKDPadArrow : MonoBehaviour
    {
        public enum ArrowTypes 
        { 
            none, 
            UP, 
            DOWN, 
            LEFT, 
            RIGHT 
        }

        public ArrowTypes ArrowType = ArrowTypes.none;

        [SerializeField]
        private RectTransform touchzoneRect = null;
        [SerializeField]
        private Image touchzoneImage = null;

        internal bool isPressed { get; private set; }

        internal float INDEX { get; private set; }


        // DPadArrowAwake
        internal void DPadArrowAwake( Sprite sprite, Color32 color )
        {
            touchzoneRect = gameObject.GetComponent<RectTransform>();
            touchzoneImage = gameObject.GetComponent<Image>();
            touchzoneImage.sprite = sprite;
            touchzoneImage.color = color;
        }


        // SetArrowActive
        internal void SetArrowActive( Sprite sprite, Color32 color, bool pressed )
        {
            touchzoneImage.sprite = sprite;
            touchzoneImage.color = color;
            isPressed = pressed;
        }
        
        // CheckBoolPosition
        private bool CheckBoolPosition( Vector2 touchPos, float sizeX, float sizeY )
        {
            float halfSizeX = touchzoneRect.sizeDelta.x / 2f;
            float halfSizeY = touchzoneRect.sizeDelta.x / 2f;

            switch( ArrowType )
            {
                case ArrowTypes.UP:
                case ArrowTypes.DOWN:
                    if( touchPos.x < touchzoneRect.position.x + sizeX / 2f
                    && touchPos.y < touchzoneRect.position.y + halfSizeY / 2f //maxY
                    && touchPos.x > touchzoneRect.position.x - sizeX / 2f
                    && touchPos.y > touchzoneRect.position.y - halfSizeY / 2f ) // minY
                    {
                        return true;
                    }
                    break;

                case ArrowTypes.RIGHT:
                case ArrowTypes.LEFT:
                    if( touchPos.x < touchzoneRect.position.x + halfSizeX / 2f //maxX
                    && touchPos.y < touchzoneRect.position.y + sizeY / 2f
                    && touchPos.x > touchzoneRect.position.x - halfSizeX / 2f //minX
                    && touchPos.y > touchzoneRect.position.y - sizeY / 2f )
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        // CheckBoolPosition
        internal bool CheckTouchPosition( Vector2 touchPos, float sizeX, float sizeY )
        {
            if( CheckBoolPosition( touchPos, sizeX, sizeY ) )
            {
                switch( ArrowType )
                {
                    case ArrowTypes.UP:
                    case ArrowTypes.RIGHT:
                        INDEX = 1f;
                        break;

                    case ArrowTypes.DOWN:
                    case ArrowTypes.LEFT:
                        INDEX = -1f;
                        break;

                    case ArrowTypes.none:
                        Debug.LogError( "ERROR: Arrow type " + gameObject.name + " is not assigned!" );
                        INDEX = 0f;
                        return false;
                }
                return true;
            }
            else
            {
                INDEX = 0f;
                return false;
            }
        }
    }
}



/*
Debug.DrawLine( new Vector2( myData.touchzoneRect.position.x + sizeX / 2f, myData.touchzoneRect.position.y + halfSizeY / 2f ),
                new Vector2( myData.touchzoneRect.position.x - sizeX / 2f, myData.touchzoneRect.position.y - halfSizeY / 2f ), Color.red );
 
Debug.DrawLine( new Vector2( myData.touchzoneRect.position.x + halfSizeX / 2f, myData.touchzoneRect.position.y + sizeY / 2f ),
                new Vector2( myData.touchzoneRect.position.x - halfSizeX / 2f, myData.touchzoneRect.position.y - sizeY / 2f ), Color.green );
*/