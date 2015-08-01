

#if UNITY_EDITOR 
using UnityEditor;
using System.Reflection;
using System.IO;
#endif
using UnityEngine;
using System.Collections;



using System.Collections.Generic;

namespace RTStudio.Utils
{

	public class Utils  
	{
		#if UNITY_EDITOR 
		// RTStudio logo load it only once.
		static Texture2D RTStudioLogo;
		static Texture2D RTStudioLine;
		static Texture2D RTStudioQWER;
        

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws the logo for Retro Tile Studio
		/// </summary>
		/// ======================================================================================================================================================================
		public static void DrawLogo()
		{
			string nom_logo;
			nom_logo = "RTStudioLogo";		
			
			// cache the logo and draw 
			if ( !RTStudioLogo )
			{
				RTStudioLogo = (Texture2D) Resources.Load(nom_logo);
			}
			
			// Center the logo in the inspector
			GUILayout.BeginHorizontal();           			
			GUILayout.FlexibleSpace();
			GUILayout.Label( (Texture2D)RTStudioLogo );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();                  
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws the logo for Retro Tile Studio
		/// </summary>
		/// ======================================================================================================================================================================
		public static void DrawLine()
		{
			string line;
			line = "line";		
			
			// cache the logo and draw 
			if ( !RTStudioLine )
			{
				RTStudioLine = (Texture2D) Resources.Load(line);
			}
			
			// Center the logo in the inspector
			GUILayout.BeginHorizontal();           			
			GUILayout.FlexibleSpace();
			GUILayout.Label( (Texture2D)RTStudioLine );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();                  
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Draws QWER
		/// </summary>
		/// ======================================================================================================================================================================
		public static void DrawQWER()
		{
			string line;
			line = "QWER";		
			
			// cache the QWER
			if ( !RTStudioQWER )
			{
				RTStudioQWER = (Texture2D) Resources.Load(line);
			}
			
			// Center the logo in the inspector
			GUILayout.BeginHorizontal();           			
			GUILayout.FlexibleSpace();
			GUILayout.Label( (Texture2D)RTStudioQWER );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();                  
        }
        /// ======================================================================================================================================================================
		/// <summary>
		/// Gets the v3 from rect.
		/// </summary>
		/// <returns>The v3 from rect.</returns>
		/// <param name="r">The red component.</param>
		/// ======================================================================================================================================================================
	
		public static Vector3[] GetV3FromRect(Rect r)
		{
			Vector3[] rectangle = new Vector3[4];
			rectangle[0] = r.position + new Vector2(0, r.size.y);
			rectangle[1] = r.position + new Vector2(0, 0);
			rectangle[2] = r.position + new Vector2(r.size.x, 0);
			rectangle[3] = r.position + r.size;
			return rectangle;
		}

		/// ======================================================================================================================================================================
		/// <summary>
		/// Gets the type of the assets of.
		/// </summary>
		/// <returns>The assets of type.</returns>
		/// <param name="type">Type.</param>
		/// <param name="fileExtension">File extension.</param>
		/// ======================================================================================================================================================================
		#endif

		public static List<Object> GetAssetsOfType(System.Type type, string fileExtension)
		{
		
			List<Object> tempObjects = new List<Object>();

			#if !UNITY_WEBPLAYER && UNITY_EDITOR

			DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
			FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);


			int i = 0; int goFileInfoLength = goFileInfo.Length;
			FileInfo tempGoFileInfo; string tempFilePath;
			Object tempGO;
			for (; i < goFileInfoLength; i++)
			{
				tempGoFileInfo = goFileInfo[i];
				if (tempGoFileInfo == null)
					continue;
				
				tempFilePath = tempGoFileInfo.FullName;
				tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

				tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(Object)) as Object;
				if (tempGO == null)
				{
					continue;
				}
				else if (tempGO.GetType() != type)
				{
					continue;
				}
				
				tempObjects.Add(tempGO);
			}
			#endif

			return tempObjects;
		}

		#if UNITY_EDITOR 
		/// ======================================================================================================================================================================
		/// <summary>
		/// Creates the texture.
		/// </summary>
		/// <returns>The new texture.</returns>
		/// <param name="texture"> texture to return</param>
		/// ======================================================================================================================================================================

		public static Texture2D CreateTexture(Texture2D texture)
		{
			if ( texture == null )
			{
				return texture;
			}

			Texture2D rtexture = new Texture2D((int)texture.width, (int)texture.height);
			var pixels = texture.GetPixels(0, (int)0, (int)texture.width, (int)texture.height);

			rtexture.SetPixels(pixels);
			rtexture.hideFlags = texture.hideFlags;
			rtexture.filterMode = texture.filterMode;
			rtexture.name = texture.name;

			rtexture.Apply();
			return rtexture;
		}
		#endif

	}

}
