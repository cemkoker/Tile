using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {
	public string scene;
	private bool loading;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && !loading) {
			LoadScene();
		}
	}

	void LoadScene() {
		loading = true;
		Application.LoadLevel(scene);
	}
}
