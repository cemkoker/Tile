using UnityEngine;
using TouchControlsKit;

/// <summary>
/// This class is a simple example of how to build a controller that interacts with PlatformerMotor2D.
/// </summary>
[RequireComponent(typeof(PlatformerMotor2D))]
public class PlayerController2D : MonoBehaviour
{

	private PlatformerMotor2D _motor;

    // Use this for initialization
    void Start()
    {
        _motor = GetComponent<PlatformerMotor2D>();
    }

    // Update is called once per frame
    void Update()
    {
	
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			if (Mathf.Abs (Input.GetAxis (PC2D.Input.HORIZONTAL)) > PC2D.Globals.INPUT_THRESHOLD) {
				_motor.normalizedXMovement = Input.GetAxis (PC2D.Input.HORIZONTAL);
			} else {
				_motor.normalizedXMovement = 0;
			}
		} else {
			// Mobile
			float horizontal = TCKInput.GetAxis ("DPad", "Horizontal");
			horizontal = Mathf.Clamp (horizontal, -1f, 1f);
			if (horizontal != 0) {
				_motor.normalizedXMovement = horizontal; //Input.GetAxis(PC2D.Input.HORIZONTAL);
			} else {
				_motor.normalizedXMovement = 0;
			}
		}

		if( TCKInput.GetButtonDown( "jumpButton"))
		{
			_motor.Jump();
		}    
        
		// Desktop
        if (Input.GetButtonDown(PC2D.Input.JUMP))
        {
            _motor.Jump();
        }

		// Desktop
		if (Input.GetKeyDown(KeyCode.W))
		{
			AudioSource audio = GetComponent<AudioSource>();
			audio.Play();

		}
		
		// _motor.jumpingHeld = Input.GetButton(PC2D.Input.JUMP);
		_motor.jumpingHeld = TCKInput.GetButtonDown( "jumpButton");

        if (Input.GetAxis(PC2D.Input.VERTICAL) < -PC2D.Globals.FAST_FALL_THRESHOLD)
        {
            _motor.fallFast = true;
        }
        else
        {
            _motor.fallFast = false;
        }

        if (Input.GetButtonDown(PC2D.Input.DASH))
        {
            _motor.Dash();
        }
    }

}
