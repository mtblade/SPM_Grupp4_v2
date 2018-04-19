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
    private float _lookAhead;

    public bool _isStatic;
    private Vector3 _staticPosition;


    public void Start()
    {
        _isStatic = false;
    }

    public void Update()
    {
        UpdateTarget();
    }

    public void StaticCameraControl(Vector3 pos)
    {
        _isStatic = !_isStatic;
        Debug.Log("StaticCamera: " + _isStatic);
        _staticPosition = pos;
    }

    public void StaticCameraControl()
    {
        _isStatic = !_isStatic;
        Debug.Log("StaticCamera: " + _isStatic);
    }

    public void ZoomControl(float distance)
    {
        Offset.z += distance;
    }

    private void LateUpdate()
    {
        UpdateMovement();
    }

    private void UpdateTarget()
    {
        if (!_isStatic) {
            _lookAhead = MathHelper.Sign(Player.Velocity.x) * MaxLookAhead;
            Vector3 lookAheadOffset = new Vector3(_lookAhead, 0, 0);
            _targetPosition = Offset + Player.transform.position + lookAheadOffset;
        } else {
            _targetPosition = _staticPosition; ;
        }
    }

    private void UpdateMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition,
            ref _currentVelocity, SmoothingTime);
    }
}
