using UnityEngine;

[RequireComponent (typeof(Controller2D))]

public class Player : MonoBehaviour {

	// Instances
	private Controller2D controller;

	// Player movement variables
	private Vector2 velocity;
	private Vector2 directionalInput;
	private float movementSpeed = 6f;

	// Jumping variables
	private float maxJumpHeight = 4f;
	private float minJumpHeight = 1f;
	private float timeToJumpApex = 0.55f;
	private float accelerationTimeAir = 0.1f;
	private float accelerationTimeGround = 0.05f;

	// Wall jumping variables
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpDrop;
	public Vector2 wallJumpLeap;
	private int wallDirX;
	public float wallSlideSpeedMax = 2.5f;
	public float wallStickTime = 0.25f;
	private float timeToWallUnstick;
	private bool wallSliding;

	// Physics variables
	private float gravity;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private float velocityXSmoothing;



	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();

		gravity = -(maxJumpHeight * 2) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}


	void Update () {

		CalculateVelocity ();
		HandleWallsliding ();
		HandleInput ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}

		}
	}


	void HandleInput () {
		directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (Input.GetKeyDown(KeyCode.W)) {
			OnJumpInputDown ();
		}

		if (Input.GetKeyUp(KeyCode.W)) {
			OnJumpInputUp ();
		}
	}


	void OnJumpInputDown () {
		if (wallSliding) {
			if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			} else if (directionalInput.x == 0) {
				velocity.x = -wallDirX * wallJumpDrop.x;
				velocity.y = wallJumpDrop.y;
			} else {
				velocity.x = -wallDirX * wallJumpLeap.x;
				velocity.y = wallJumpLeap.y;
			}
		}

		if (controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) {
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
			}

		}
	}


	void OnJumpInputUp () {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}


	void HandleWallsliding () {
		wallDirX = controller.collisions.left ? -1 : 1;
		wallSliding = false;

		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below) {
			wallSliding = true;
			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				timeToWallUnstick = wallStickTime;
			}
		}
	}


	void CalculateVelocity () {
		float targetVelocityX = directionalInput.x * movementSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGround : accelerationTimeAir);
		velocity.y += gravity * Time.deltaTime;
	}
}
