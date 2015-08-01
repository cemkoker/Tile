using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RTStudio.Helpers
{
	[ExecuteInEditMode]
	public class RTStudioBorderHelper : MonoBehaviour {

		public CanvasScaler LeftRight;
		public CanvasScaler TopBottom;
		public RectTransform LRPanel;
		public RectTransform TBPanel;
	
		public RectTransform L;
		public RectTransform R;
		public RectTransform T;
		public RectTransform B;

		public bool PixelPerfectBorder = false;
		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start () {
		
		}
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update () 
		{


			Vector2 newRes = new Vector2 ( RTStudio.Settings.gameWidth , RTStudio.Settings.gameHeight );
			LeftRight.referenceResolution = newRes;
			TopBottom.referenceResolution = newRes;

//			float roundingFixX = +(newRes.x/2.0f)/newRes.x;
//			float roundingFixY = -(newRes.y/2.0f)/newRes.y;
			if ( PixelPerfectBorder )
			{
				LRPanel.anchoredPosition = new Vector2 (0, 0);
				TBPanel.anchoredPosition = new Vector2 (0, 0);
				L.anchoredPosition = new Vector2 (-2000, 0);
				R.anchoredPosition = new Vector2 (2000, 0);
				T.anchoredPosition = new Vector2 (0, 2000);
				B.anchoredPosition = new Vector2 (0, -2000);
				// for the correct positions

				LeftRight.scaleFactor = RTStudio.Settings.bestFit;
				TopBottom.scaleFactor = RTStudio.Settings.bestFit;
			}

		

			LRPanel.sizeDelta = new Vector2(  newRes.x, newRes.y );
			TBPanel.sizeDelta = new Vector2(  newRes.x, newRes.y );

		}
	}
}
