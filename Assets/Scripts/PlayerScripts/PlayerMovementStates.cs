using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// If instead of making the StateMachine a separate thing and be universally, each component could have its unique statemachine
// the state machine, (which could be incorporated in the player controller, and does stop exiting)
// each state would be universal

public class PlayerMovementStates
{
    PlayerController controller;

    public enum State { IDLE, RUN, JUMP, FALL, NULL };
    State currentState;

    public PlayerMovementStates(PlayerController controller)
    {
        this.controller = controller;
        currentState = State.IDLE;
    }

    public void UpdateMachine()
    {
        // controls the movement of player
        TickState();
        if (CheckingStateConditions() != State.NULL) ChangeState(CheckingStateConditions());
    }

    public State CheckingStateConditions()
    // make your function names related to what you're delivering
    //also comment more on the things you're doing
    {
        switch (currentState)
        {
            case State.IDLE:
                if (!controller.Collisions.IsBottomCollidingWithPlatform) return State.FALL;
                else if (controller.JumpInput == true) return State.JUMP;
                else if (controller.RunSpeed != 0) return State.RUN;
                break;

            case State.RUN:
                if (!controller.Collisions.IsBottomCollidingWithPlatform) return State.FALL;
                else if (controller.JumpInput == true) return State.JUMP;
                else if (controller.RunSpeed == 0 && controller.RunInput == 0) return State.IDLE;
                break;

            case State.JUMP:
                if (controller.move.y < 0) return State.FALL;
                else if (controller.Collisions.IsBottomCollidingWithPlatform) return State.IDLE;
                break;

            case State.FALL:
                if (controller.Collisions.IsBottomCollidingWithPlatform) return State.IDLE;
                break;
        }

        return State.NULL;
    }

    public void ChangeState(State state)
    {
        currentState = state;
        EnterState();
    }

    public void EnterState()
    {
        switch (currentState)
        {
            case State.JUMP:
                controller.move.y = controller.InitialJumpVelocity;
                break;
        }
    }

    public void TickState()
    {
        switch (currentState)
        {
            case State.IDLE:
                if (controller.Collisions.IsBottomCollidingWithPlatform) controller.move = Vector2.zero;
                break;

            case State.RUN:
                controller.move.x = controller.direction * controller.RunSpeed;
                break;

            case State.JUMP:
                controller.move.x = controller.direction * controller.RunSpeed;
                controller.move.y += controller.Gravity * Time.fixedDeltaTime;
                break;

            case State.FALL:
                controller.move.x = controller.direction * controller.RunSpeed;
                controller.move.y = controller.move.y + controller.Gravity * Time.fixedDeltaTime;
                break;
        }
    }
}
