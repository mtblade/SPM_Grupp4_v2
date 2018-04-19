using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public PlayerController Player;
    public Vector3 Offset;
    private Vector3 _targetPosition;

    [Header("Follow")]
    public float SmoothingTime;
    private Vector3 _currentVelocity;

    [Header("Look Ahead")]
    public float MaxLookAhead;
    public float LookAheadAccelerationTime;
    private float _lookAheadSpeed;
    private float _lookAhead;

    private float counter;
    private bool followingPlayer;

    private bool _isStatic;
    private Vector3 _staticPosition;
    /*
	[Header("Look Around")]
	public float MaxLookAroundAmount;
	public float TimeBeforeLookAround;
	public float PlayerMaximumSpeedForLookAround;
	private float _playerStillTime;
	private float _lookAroundAmount;
    */

    public void Start()
    {
        _isStatic = false;
    }

    public void Update()
    {
        UpdateLookAhead();
        UpdateTargetPosition();
    }

    private void LateUpdate()
    {
        if (!_isStatic) {
            counter += Time.deltaTime;
        } else if (_isStatic)
            counter = -1f;
        if (counter > SmoothingTime && !_isStatic)
            UpdateMovement();
        else
            UpdateMovementToStaticZone();
    }

    private void UpdateTargetPosition()
    {
        _targetPosition = Player.transform.position;
        _targetPosition += Offset;
        _targetPosition += Vector3.right * _lookAhead;
    }

    public void StaticCameraControl(Vector3 pos)
    {
        _isStatic = !_isStatic;
        _staticPosition = pos;
    }

    private void UpdateMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition,
            ref _currentVelocity, 0);
    }

    private void UpdateMovementToStaticZone()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _staticPosition,
            ref _currentVelocity, SmoothingTime);
    }

    private void UpdateMovementFromStaticZone()
    {
        transform.position = Vector3.SmoothDamp(_staticPosition,_targetPosition,
              ref _currentVelocity, SmoothingTime);
    }


    private void UpdateLookAhead()
    {
        float targetLookAhead = MathHelper.Sign(Player.Velocity.x) * MaxLookAhead;
        _lookAhead = Mathf.SmoothDamp(_lookAhead, targetLookAhead,
            ref _lookAheadSpeed, LookAheadAccelerationTime);
    }

    //private void UpdateLookAround(){
    //	if(Player.Velocity.magnitude > PlayerMaximumSpeedForLookAround){
    //		_lookAroundAmount = 0.0f;
    //		_playerStillTime = 0.0f;
    //		return;
    //	}
    //	_playerStillTime += Time.deltaTime;
    //	if(_playerStillTime < TimeBeforeLookAround)
    //		return;
    //	_lookAroundAmount = Input.GetAxisRaw("Vertical") * MaxLookAroundAmount;
    //}
}
