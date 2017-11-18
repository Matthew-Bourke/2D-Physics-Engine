using UnityEngine;


[RequireComponent (typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	[HideInInspector] public BoxCollider2D boxCollider;
	[HideInInspector]public RaycastOrigins raycastOrigins;

	[HideInInspector] public float horizontalRaySpacing;
	[HideInInspector] public float verticalRaySpacing;

	private const float distanceBetweenRays = 0.25f;

	public const float skinWidth = 0.015f;
	public int horizontalRayCount;
	public int verticalRayCount;


	public virtual void Awake () {
		boxCollider = GetComponent<BoxCollider2D> ();
	}

	public virtual void Start () {
		CalculateRaySpacing ();
	}


	public void UpdateRaycastOrigins () {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2f);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}


	public void CalculateRaySpacing () {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2f);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt (boundsHeight / distanceBetweenRays);
		verticalRayCount = Mathf.RoundToInt (boundsWidth / distanceBetweenRays);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}


	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
