using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions 
{
	public static List<Transform> PixelPerfectObjects;

	/// ====================================================================================================================================================================== 
	/// <summary>
	/// Initializes the PP objects list
	/// </summary>
	/// ====================================================================================================================================================================== 
	public static List<Transform> GetPixelPerfectObjects( this MonoBehaviour mono )
	{
		initializePPObjectsIfNoneExists();
		return PixelPerfectObjects;
	}


	/// ====================================================================================================================================================================== 
	/// <summary>
	/// Initializes the PP objects list
	/// </summary>
	/// ====================================================================================================================================================================== 
	private static void initializePPObjectsIfNoneExists()
	{
		if ( PixelPerfectObjects == null )
			PixelPerfectObjects = new List<Transform> ();
	}

	/// ====================================================================================================================================================================== 
	/// <summary>
	/// Pixels perfect Extension method. Call this to register and track a pixel perfect object
	/// </summary>
	/// <param name="mono">Mono.</param>
	/// ====================================================================================================================================================================== 
	public static void AddPixelPerfectObject( this MonoBehaviour mono )
	{
		initializePPObjectsIfNoneExists();

		if ( !PixelPerfectObjects.Contains( mono.transform ) )
		{
			PixelPerfectObjects.Add( mono.transform );
		}
	}

	/// ====================================================================================================================================================================== 
	/// <summary>
	/// Pixels perfect Extension method. Call this to remove tracked pixel perfect object
	/// </summary>
	/// <param name="mono">Mono.</param>
	/// ====================================================================================================================================================================== 
	public static void RemovePixelPerfectObject( this MonoBehaviour mono )
	{
		initializePPObjectsIfNoneExists();

		if ( PixelPerfectObjects.Contains( mono.transform ) )
		{
			PixelPerfectObjects.Remove( mono.transform );
		}
	}
}
