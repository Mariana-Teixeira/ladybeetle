using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// If instead of making the StateMachine a separate thing and be universally, each component could have its unique statemachine
// the state machine, (which could be incorporated in the player controller, and does stop exiting)
// each state would be universal

public class PlayerStateMachine
{
    public enum State { IDLE, RUN, JUMP, FALL, HOOK, NULL };
    public enum SpeedState { STOP, ACCELERATE, MAXSPEED, DECELETARE, NULL };

    PlayerController controller;
    State currentState;
    SpeedState currentSpeedState;

    float direction;

    public PlayerStateMachine(PlayerController controller)
    {
        this.controller = controller;

        currentState = State.IDLE;
        currentSpeedState = SpeedState.STOP;
    }

    public void UpdateMachine()
    {
        // controls the speed variable of player
        TickRunState();
        if (CheckingSpeedStateConditions() != SpeedState.NULL) currentSpeedState = CheckingSpeedStateConditions();


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
                if (controller.HookInput && controller.TryToHook()) return State.HOOK;
                if (!controller.Collisions.IsBottomCollidingWithPlatform) return State.FALL;
                if (controller.JumpInput == true) return State.JUMP;
                if (currentSpeedState != SpeedState.STOP) return State.RUN;
                break;

            case State.RUN:
                if (controller.HookInput && controller.TryToHook()) return State.HOOK;
                if (!controller.Collisions.IsBottomCollidingWithPlatform) return State.FALL;
                if (controller.JumpInput == true) return State.JUMP;
                if (currentSpeedState == SpeedState.STOP) return State.IDLE;
                break;

            case State.JUMP:
                if (controller.HookInput && controller.TryToHook()) return State.HOOK;
                else if (controller.move.y < 0) return State.FALL;
                else if (controller.Collisions.IsBottomCollidingWithPlatform) return State.IDLE;
                break;

            case State.FALL:
                if (controller.HookInput && controller.TryToHook()) return State.HOOK;
                if (controller.Collisions.IsBottomCollidingWithPlatform) return State.IDLE;
                break;

            case State.HOOK:
                if (controller.HookHit.collider != null || controller.JumpInput == true) return State.IDLE;
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
        if (controller.RunInput != 0) direction = controller.RunInput;

        switch (currentState)
        {
            case State.JUMP:
                controller.move.y = controller.InitialJumpVelocity;
                break;

            case State.HOOK:
                controller.HookDirection = (controller.MousePositionInWorld - controller.Body.position).normalized;
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
                controller.move.x = direction * controller.RunSpeed;
                break;

            case State.JUMP:
                controller.move.x = direction * controller.RunSpeed;
                controller.move.y += controller.Gravity * Time.fixedDeltaTime;
                break;

            case State.FALL:
                controller.move.x = direction * controller.RunSpeed;
                controller.move.y = controller.move.y + controller.Gravity * Time.fixedDeltaTime;
                break;

            case State.HOOK:
                controller.move = controller.HookDirection * controller.HookSpeed;
                break;
        }
    }

    public SpeedState CheckingSpeedStateConditions()
    {
        switch (currentSpeedState)
        {
            case SpeedState.STOP:
                if (controller.RunInput != 0) return SpeedState.ACCELERATE;
                break;

            case SpeedState.ACCELERATE:
                if (controller.RunInput != direction) return SpeedState.DECELETARE;
                if (controller.RunSpeed >= controller.MaxSpeed) return SpeedState.MAXSPEED;
                break;

            case SpeedState.MAXSPEED:
                if (controller.RunInput != direction) return SpeedState.DECELETARE;
                break;

            case SpeedState.DECELETARE:
                if (controller.RunInput == direction) return SpeedState.ACCELERATE;
                if (controller.RunSpeed <= 0.05f) return SpeedState.STOP;
                break;
        }

        return SpeedState.NULL;
    }

    public void TickRunState()
    {
        switch (currentSpeedState)
        {
            case SpeedState.STOP:
                controller.RunSpeed= 0;
                break;

            case SpeedState.ACCELERATE:
                controller.RunSpeed += controller.Acceleration * Time.fixedDeltaTime;
                break;

            case SpeedState.MAXSPEED:
                controller.RunSpeed = controller.MaxSpeed;
                break;

            case SpeedState.DECELETARE:
                controller.RunSpeed -= controller.Acceleration * Time.fixedDeltaTime;
                break;
        }

        
    }
}
