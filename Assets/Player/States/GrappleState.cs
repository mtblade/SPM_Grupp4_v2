using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "Player/States/GrappleState")]
public class GrappleState : State {

    private PlayerController _controller;



    public override void Initialize(Controller owner)
    {
        _controller = (PlayerController)owner;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
