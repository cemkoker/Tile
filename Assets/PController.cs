using UnityEngine;
using System.Collections;

public class PController : MonoBehaviour {

	public float moveSpeed;
	public float jumpHeight;

	public bool isJumping;


	// Use this for initialization
	void Start () {
		isJumping = false;

	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			isJumping = true;
		} else {
			isJumping = false;
		}
		 
	}

	void FixedUpdate () {
		Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D> ();

		if (isJumping) {
			rb.velocity = new Vector2 (0, jumpHeight);
		}
	}
}
