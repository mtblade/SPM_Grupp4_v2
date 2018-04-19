using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "Player/States/AirState")]
public class AirState : State {
	private PlayerController _controller;

	public bool CanCancelJump;
	public float FastFallingModifier = 2.0f;

	[Header("Movement")]
	public float Acceleration;
	public float Friction;

	private Transform transform { get { return _controller.transform; } }

	private Vector2 Velocity { get { return _controller.Velocity; } set { _controller.Velocity = value; } }

	private List<Collider2D> _ignoredPlatforms = new List<Collider2D>();

	public override void Initialize(Controller owner) {
		_controller = (PlayerController)owner;
	}

	public override void Update() {
		_controller.GetState<GroundState>().UpdateJump();
		UpdateGravity();
		RaycastHit2D[] hits = _controller.DetectHits();
		UpdateMovement();
		UpdateNormalForce(hits);
		transform.Translate(Velocity * Time.deltaTime);
        CancelJump();
	}

	public override void Enter() {
		_ignoredPlatforms.Clear();
	}

	private void UpdateMovement(){
		float input = Input.GetAxisRaw("Horizontal");
		if(Mathf.Abs(input) > _controller.InputMagnitudeToMove){
			Vector2 delta = Vector2.right * input * Acceleration * Time.deltaTime;
			if(Mathf.Abs((_controller.Velocity + delta).x) < _controller.MaxSpeed ||
				Mathf.Abs(Velocity.x) > _controller.MaxSpeed && Vector2.Dot(Velocity.normalized, delta) < 0.0f)
				_controller.Velocity += delta;
			else
				_controller.Velocity.x = MathHelper.Sign(input) * _controller.MaxSpeed;
		}
		else{
			Vector2 currentDirection = Vector2.right * MathHelper.Sign(Velocity.x);
			float horizontalVelocity = Vector2.Dot(Velocity.normalized, currentDirection) * Velocity.magnitude;
			Velocity -= currentDirection * horizontalVelocity * Friction * Time.deltaTime;
		}
	}

	private void UpdateGravity() {
		float gravityModifier = Velocity.y < 0.0f ? FastFallingModifier : 1f;
		Velocity += Vector2.down * _controller.Gravity * gravityModifier * Time.deltaTime;
	}

	private void UpdateNormalForce(RaycastHit2D[] hits) {
		if(hits.Length == 0)
			return;
		RaycastHit2D snapHit = hits.FirstOrDefault(h => !h.collider.CompareTag("OneWay"));
		if(snapHit.collider != null) {
			_controller.SnapToHit(snapHit);
			if(Velocity.y < -55f)
				CameraShake.AddIntensity(1);
		}
		Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)_controller.Collider.offset, 
			_controller.Collider.size, 0.0f, _controller.CollisionLayer);
		for(int i = _ignoredPlatforms.Count - 1; i>= 0; i--){
			if(!colliders.Contains(_ignoredPlatforms[i]))
				_ignoredPlatforms.Remove(_ignoredPlatforms[i]);
		}	
		foreach(RaycastHit2D hit in hits) {
			if(hit.collider.CompareTag("OneWay") && Velocity.y > 0.0f && !_ignoredPlatforms.Contains((hit.collider)))
				_ignoredPlatforms.Add(hit.collider);
			if(_ignoredPlatforms.Contains(hit.collider))
				continue;
			Velocity += MathHelper.GetNormalForce(Velocity, hit.normal);
			if(MathHelper.CheckAllowedSlope(_controller.SlopeAngles, hit.normal))
				_controller.TransitionTo<GroundState>();
		}
	}

	private void CancelJump(){
		float minJumpVelocity = _controller.GetState<GroundState>().JumpVelocity.Min;
		if(Velocity.y < minJumpVelocity)
			CanCancelJump = false;
		if(!CanCancelJump || Input.GetButton("Jump"))
			return;
		CanCancelJump = false;
		_controller.Velocity.y = minJumpVelocity;
	}
}
