using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/States/GroundState")]
public class GroundState : State
{
    private PlayerController _controller;
    [Header("Movement")]
    public float Acceleration;
    public float Deceleration;
    public float TurnModifier;

    [Header("Jumping")]
    public MinMaxFloat JumpHeight;
    [HideInInspector] public MinMaxFloat JumpVelocity;
    public float TimeToApexJump;
    public float InitialJumpDistance;
    public int MaxJumps = 2;
    private int _jumps;

    [Header("Grappling")]
    public float grapplingRange;
    public LayerMask grappleLayer;
    private RaycastHit2D[] hitDetect;

    private Vector2 _groundNormal;

    private Vector2 VectorAlongGround { get { return MathHelper.RotateVector(_groundNormal, -90f); } }

    private Transform transform { get { return _controller.transform; } }

    private Vector2 Velocity { get { return _controller.Velocity; } set { _controller.Velocity = value; } }

    public override void Initialize(Controller owner)
    {
        _controller = (PlayerController)owner;
        _controller.Gravity = (2 * JumpHeight.Max) / Mathf.Pow(TimeToApexJump, 2);
        JumpVelocity.Max = _controller.Gravity * TimeToApexJump;
        JumpVelocity.Min = Mathf.Sqrt(2 * _controller.Gravity * JumpHeight.Min);
    }

    public override void Enter()
    {
        _jumps = MaxJumps;
    }

    public override void Update()
    {



        if (Input.GetButtonDown("Grappling")) {

            hitDetect = Physics2D.CircleCastAll(transform.position, grapplingRange, Vector2.zero, 0f, grappleLayer);
            Debug.Log(hitDetect.Length);
            if (hitDetect != null) {
                for (int i = 0; i < hitDetect.Length; i++)
                    Debug.Log(hitDetect[i].collider.name);
            }
            _controller.TransitionTo<GrappleState>();
        }

        UpdateGravity();
        RaycastHit2D[] hits = _controller.DetectHits(true);
        if (hits.Length == 0) {
            _jumps--;
            _controller.TransitionTo<AirState>();
            return;
        }
        UpdateGroundNormal(hits);
        UpdateMovement();
        UpdateNormalForce(hits);
        transform.Translate(Velocity * Time.deltaTime);
        UpdateJump();


    }

    public void UpdateJump()
    {
        if (!Input.GetButtonDown("Jump") || _jumps <= 0)
            return;
        transform.position += Vector3.up * InitialJumpDistance;
        _controller.Velocity.y = JumpVelocity.Max;
        _jumps--;
        _controller.TransitionTo<AirState>();
        _controller.GetState<AirState>().CanCancelJump = true;
    }

    private void UpdateGravity()
    {
        Velocity += Vector2.down * _controller.Gravity * Time.deltaTime;
    }

    private void UpdateGroundNormal(RaycastHit2D[] hits)
    {
        int numberOfGroundHits = 0;
        _groundNormal = Vector2.zero;
        foreach (RaycastHit2D hit in hits) {
            if (!MathHelper.CheckAllowedSlope(_controller.SlopeAngles, hit.normal))
                continue;
            _groundNormal += hit.normal;
            numberOfGroundHits++;
        }

        if (numberOfGroundHits == 0) {
            _jumps--;
            _controller.TransitionTo<AirState>();
            return;
        }
        _groundNormal /= numberOfGroundHits;
        _groundNormal.Normalize();
    }

    private void UpdateNormalForce(RaycastHit2D[] hits)
    {
        if (hits.Length == 0)
            return;
        _controller.SnapToHit(hits[0]);

        foreach (RaycastHit2D hit in hits) {
            if (Mathf.Approximately(Velocity.magnitude, 0.0f))
                continue;
            Velocity += MathHelper.GetNormalForce(Velocity, hit.normal);
        }
    }

    private void UpdateMovement()
    {
        if (_groundNormal.magnitude < MathHelper.FloatEpsilon)
            return;
        float input = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(input) > _controller.InputMagnitudeToMove)
            Accelerate(input);
        else
            Decelerate();
    }

    private void Accelerate(float input)
    {
        int direction = MathHelper.Sign(Velocity.x);
        float turnModifier = MathHelper.Sign(input) != direction && direction != 0 ? TurnModifier : 1f;
        Vector2 deltaVelocity = VectorAlongGround * input * Acceleration * turnModifier * Time.deltaTime;
        Vector2 newVelocity = Velocity + deltaVelocity;
        Velocity = newVelocity.magnitude > _controller.MaxSpeed ? VectorAlongGround *
            MathHelper.Sign(Velocity.x) * _controller.MaxSpeed : newVelocity;
    }

    private void Decelerate()
    {
        Vector2 deltaVelocity = MathHelper.Sign(Velocity.x) * VectorAlongGround * Deceleration * Time.deltaTime;
        Vector2 newVelocity = Velocity - deltaVelocity;
        Velocity = Velocity.magnitude < MathHelper.FloatEpsilon ||
            MathHelper.Sign(newVelocity.x) != MathHelper.Sign(Velocity.x) ? Vector2.zero : newVelocity;
    }
}
