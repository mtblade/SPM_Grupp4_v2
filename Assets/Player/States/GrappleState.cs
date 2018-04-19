using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "Player/States/GrappleState")]
public class GrappleState : State {

    

    private GameObject ropeHingeAnchor;
    private DistanceJoint2D ropeJoint;
    private Transform crosshair;
    private SpriteRenderer crosshairSprite;
    private PlayerController _controller;
    private bool ropeAttached;
    private Vector2 playerPosition;
    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    private LineRenderer ropeRenderer;
    private bool distanceSet;

    private RaycastHit2D _hitDetect;

    
    private List<Vector2> ropePositions = new List<Vector2>();




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
        playerPosition = _controller.transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }


    public override void Enter(RaycastHit2D hit)
    {
        _controller.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
  
        _hitDetect = hit;

        var hookPointWorldPosition = new Vector3(_hitDetect.transform.position.x, _hitDetect.transform.position.y, 0f);

        var hookScreenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(_hitDetect.transform.position.x, _hitDetect.transform.position.y, 0f));

        playerPosition = _controller.transform.position;

     
         
            //Världskoordinaterna för hookpoints sparar i variabeln crosshairPosition
            var crosshairPosition = new Vector3(hookPointWorldPosition.x, hookPointWorldPosition.y, 0);

            //Crosshair objektets transform sätts lika med värdskoordinaterna för hookpointen. 
            crosshair.transform.position = crosshairPosition;

            //Spritens görs synlig   GRÖN
            //crosshairSprite.enabled = true;


            //spelarens rigidbody får en force i y led uppåt, som ett litet hopp i luften innan grapplingens hook skjuts ut och fastnar
            _controller.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);

            //ropePositions är en lista med vektor2 från hookpointens position
            ropePositions.Add(_hitDetect.point);

            //Distance2D joint som heter "ropeJoint" - dess längd sätts lika med spelarens position minus raycast träffsen point/position. i detta fallet  närmsta hookpointen
            ropeJoint.distance = Vector2.Distance(playerPosition, _hitDetect.point);

            //Distance2D joint "ropeJoint" görs synlig.
            ropeJoint.enabled = true;

            //Line renderar som är repet görs synligt 
            ropeRenderer.enabled = true;


            //Grapplingens ankare sprite görs synlig 
            ropeHingeAnchorSprite.enabled = true;

     
    }


    public override void Update()
    {

        RaycastHit2D[] hits = _controller.DetectHits();

        if (Input.GetButtonDown("Grappling"))
        {
            disableGrappling();

            _controller.TransitionTo<AirState>();
        }


        RaycastHit2D snapHit = hits.FirstOrDefault(h => !h.collider.CompareTag("OneWay"));
        if (snapHit.collider != null)
        {
            _controller.SnapToHit(snapHit);
        }

        foreach (RaycastHit2D hit in hits)
        {
            if (MathHelper.CheckAllowedSlope(_controller.SlopeAngles, hit.normal))
            {
                Debug.Log("GROUND");
                _controller.TransitionTo<GroundState>();
                disableGrappling();
            }

        }

        UpdateRopePositions();

    }

    private void disableGrappling()
    {
        //ropeAttached = false;
        //playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, _controller.transform.position);
        ropeRenderer.SetPosition(1, _controller.transform.position);
        ropePositions.Clear();
        ropeHingeAnchorSprite.enabled = false;

        ropeJoint.enabled = false;
        crosshairSprite.enabled = false;
        ropeRenderer.enabled = false;

       // _controller.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

    }

    private void UpdateRopePositions()
    {
        //// 1


        // 2
        ropeRenderer.positionCount = ropePositions.Count + 1;

        // 3
        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            //OM det inte är sista positionen i ropeRenderer
            if (i != ropeRenderer.positionCount - 1) 
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
