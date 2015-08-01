using UnityEngine;
using System.Collections;

public class Deadly : MonoBehaviour {


	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Debug.Log("Hit me baby");

			// Destroy(other);
		}
	}

}
