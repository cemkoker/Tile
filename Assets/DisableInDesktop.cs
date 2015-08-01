using UnityEngine;
using System.Collections;

public class DisableInDesktop : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.OSXEditor) {
			gameObject.SetActive(false);
		}
	
	}
}
