using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

	public AudioClip collectCoinSound;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Destroy(gameObject);
			AudioSource.PlayClipAtPoint(collectCoinSound, transform.position);

		}
	}

}
