using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "Player/States/GrappleState")]
public class GrappleState : State {

    private PlayerController _controller;
    private GameObject ropeHingeAnchor;
    private DistanceJoint2D ropeJoint;
    private Transform crosshair;
    private SpriteRenderer crosshairSprite;
    private Vector2 playerPosition;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    private LineRenderer ropeRenderer;

    private RaycastHit2D _hitDetect;

    private bool ropeAttached;

    private List<Vector2> ropePositions = new List<Vector2>();

    private bool distanceSet;


    public override void Initialize(Controller owner)
    {
        _controller = (PlayerController)owner;
        ropeHingeAnchor = GameObject.Find("RopeHingeAnchor");
        ropeJoint = GameObject.Find("Player").GetComponent<DistanceJoint2D>();
        crosshair = GameObject.Find("Crosshair").GetComponent<Transform>();
        crosshairSprite = GameObject.Find("Crosshair").GetComponent<SpriteRenderer>();
        playerPosition = _controller.transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
        ropeRenderer = GameObject.Find("Player").GetComponent<LineRenderer>();

        ropeJoint.enabled = false;
    }


    public override void Enter(RaycastHit2D hit)
    {
        _hitDetect = hit;

        var hookPointWorldPosition = new Vector3(_hitDetect.transform.position.x, _hitDetect.transform.position.y, 0f);

        playerPosition = _controller.transform.position;

        if (!ropeAttached)
        {

            var crosshairPosition = new Vector3(hookPointWorldPosition.x, hookPointWorldPosition.y, 0);
            crosshair.transform.position = crosshairPosition;
            crosshairSprite.enabled = true;
  
            ropeRenderer.enabled = true;

            _controller.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
            ropePositions.Add(_hitDetect.point);
            ropeJoint.distance = Vector2.Distance(playerPosition, _hitDetect.point);
            ropeJoint.enabled = true;
            ropeHingeAnchorSprite.enabled = true;


        

        }
        else
        {
            crosshairSprite.enabled = false;
            ropeRenderer.enabled = false;
            ropeAttached = false;
            ropeJoint.enabled = false;
        }


    }


    public override void Update()
    {

        RaycastHit2D[] hits = _controller.DetectHits();

        if (Input.GetButtonDown("Grappling"))
        {

            ropeJoint.enabled = false;
            ropeAttached = false;
            //playerMovement.isSwinging = false;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, _controller.transform.position);
            ropeRenderer.SetPosition(1, _controller.transform.position);
            ropePositions.Clear();
            ropeHingeAnchorSprite.enabled = false;

        }


        RaycastHit2D snapHit = hits.FirstOrDefault(h => !h.collider.CompareTag("OneWay"));
        if (snapHit.collider != null)
        {
            _controller.SnapToHit(snapHit);
        }

            foreach (RaycastHit2D hit in hits)
        {

            if (MathHelper.CheckAllowedSlope(_controller.SlopeAngles, hit.normal))
                _controller.TransitionTo<GroundState>();

            crosshairSprite.enabled = false;
            ropeRenderer.enabled = false;
            ropeAttached = false;
            ropeJoint.enabled = false;
        }

        UpdateRopePositions();

    }

    private void UpdateRopePositions()
    {
        // 1


        // 2
        ropeRenderer.positionCount = ropePositions.Count + 1;

        // 3
        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                ropeRenderer.SetPosition(i, ropePositions[i]);

                // 4
                if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                {
                    var ropePosition = ropePositions[ropePositions.Count - 1];
                    if (ropePositions.Count == 1)
                    {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(_controller.transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                    else
                    {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(_controller.transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                // 5
                else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                {
                    var ropePosition = ropePositions.Last();
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(_controller.transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                // 6
                ropeRenderer.SetPosition(i, _controller.transform.position);
            }
        }
    }

    public override void Exit()
    {

    }
}
