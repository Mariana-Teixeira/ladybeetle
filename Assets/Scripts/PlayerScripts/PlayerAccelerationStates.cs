using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerAccelerationStates
{
    PlayerController controller;

    public enum State { STOP, ACCELERATE, MAXSPEED, DECELETARE, NULL };
    State currentState;

    public PlayerAccelerationStates(PlayerController controller)
    {
        this.controller = controller;
        currentState = State.STOP;
    }

    public void UpdateMachine()
    {
        // controls the speed variable of player
        TickRunState();
        if (CheckingSpeedStateConditions() != State.NULL) ChangeState(CheckingSpeedStateConditions());
    }

    public State CheckingSpeedStateConditions()
    {
        switch (currentState)
        {
            case State.STOP:
                if (controller.RunInput != 0) return State.ACCELERATE;
                break;

            case State.ACCELERATE:
                if (controller.RunInput != controller.direction) return State.DECELETARE;
                if (controller.RunSpeed >= controller.MaxSpeed) return State.MAXSPEED;
                break;

            case State.MAXSPEED:
                if (controller.RunInput != controller.direction) return State.DECELETARE;
                break;

            case State.DECELETARE:
                if (controller.RunInput == controller.direction) return State.ACCELERATE;
                if (controller.RunSpeed <= 0.05f) return State.STOP;
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
            case State.ACCELERATE:
                if (controller.RunInput != 0) controller.direction = controller.RunInput;
                break;
        }
    }

    public void TickRunState()
    {
        switch (currentState)
        {
            case State.STOP:
                controller.RunSpeed = 0;
                break;

            case State.ACCELERATE:
                controller.RunSpeed += controller.Acceleration * Time.fixedDeltaTime;
                break;

            case State.MAXSPEED:
                controller.RunSpeed = controller.MaxSpeed;
                break;

            case State.DECELETARE:
                controller.RunSpeed -= controller.Acceleration * Time.fixedDeltaTime;
                break;
        }


    }
}
