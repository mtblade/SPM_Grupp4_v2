using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerController : Controller {

	public float MaxSpeed;
	public LayerMask CollisionLayer;
	[HideInInspector]public float Gravity;
	[HideInInspector] public BoxCollider2D Collider;
	public Vector2 Velocity;
	public float GroundCheckDistance;
	public float InputMagnitudeToMove;
	public MinMaxFloat SlopeAngles;
	public float MaxWallAngleDelta;


	void Start() {
		Collider = GetComponent<BoxCollider2D>();
	}

	public RaycastHit2D[] DetectHits(bool addGroundCheck = false) {
		Vector2 direction = Velocity.normalized;
		float distance = Velocity.magnitude * Time.deltaTime;

		Vector2 position = transform.position + (Vector3)Collider.offset;
		List<RaycastHit2D> hits = Physics2D.BoxCastAll(position, Collider.size, 0.0f, 
			                          direction, distance, CollisionLayer).ToList();
		RaycastHit2D[] groundHits = Physics2D.BoxCastAll(position, Collider.size, 0.0f,
			                            Vector2.down, GroundCheckDistance, CollisionLayer);
		hits.AddRange(groundHits);

		for(int i = 0; i < hits.Count; i++) {
			RaycastHit2D safetyHit = Physics2D.Linecast(position, hits[i].point, CollisionLayer);
			if(safetyHit.collider != null)
				hits[i] = safetyHit;
		}
		return hits.ToArray();
	}

	public void SnapToHit(RaycastHit2D hit) {
		Vector2 vectorToPoint = hit.point - (Vector2)transform.position;
		vectorToPoint -= MathHelper.PointOnRectangle(vectorToPoint.normalized, Collider.size);
		Vector3 movement = (Vector3)hit.normal * Vector2.Dot(hit.normal, 
			                   vectorToPoint.normalized) * vectorToPoint.magnitude;
		if(Vector2.Dot(Velocity.normalized, vectorToPoint.normalized) > 0.0f)
			transform.position += movement;
	}
}
