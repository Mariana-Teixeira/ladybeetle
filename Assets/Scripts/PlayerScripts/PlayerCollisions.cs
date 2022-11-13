using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions
{
    PlayerController controller;

    #region HitBox Properties
    private Vector2 PlayerCenter
    {
        get
        {
            return controller.Body.position;
        }
    }
    private Vector2 PlayerSize
    {
        get
        {
            return controller.BoxCollider.size;
        }
    }

    private Vector2 HitBoxSize_TopBottom
    {
        get
        {
            return new Vector2(PlayerSize.x/2, 0.1f);
        }
    }
    private Vector2 HitBoxSize_LeftRight
    {
        get
        {
            return new Vector2(0.1f, PlayerSize.y/2);
        }
    }
    #endregion

    #region Player HitBoxes
    public RaycastHit2D TopHit
    {
        get
        {
            return Physics2D.BoxCast(
                PlayerCenter,
                HitBoxSize_TopBottom,
                0.0f,
                Vector2.up,
                PlayerSize.y/2,
                controller.PlatformMask);
        }
    }
    public RaycastHit2D BottomHit
    {
        get
        {
            return Physics2D.BoxCast(
                PlayerCenter,
                HitBoxSize_TopBottom,
                0.0f,
                Vector2.down,
                PlayerSize.y/2,
                controller.PlatformMask);
        }
    }
    public RaycastHit2D LeftHit
    {
        get
        {
            return Physics2D.BoxCast(
                PlayerCenter,
                HitBoxSize_LeftRight,
                0.0f,
                Vector2.left,
                PlayerSize.x/2,
                controller.PlatformMask);
        }
    }
    public RaycastHit2D RightHit
    {
        get
        {
            return Physics2D.BoxCast(
                PlayerCenter,
                HitBoxSize_LeftRight,
                0.0f,
                Vector2.right,
                PlayerSize.x/2,
                controller.PlatformMask);
        }
    }
    #endregion

    #region Check for Collisions
    public bool IsTopCollidingWithPlatform
    {
        get
        {
            return TopHit.collider != null;
        }
    }
    public bool IsBottomCollidingWithPlatform
    {
        get
        {
            return BottomHit.collider != null;
        }
    }
    public bool IsLeftCollidingWithPlatform
    {
        get
        {
            return LeftHit.collider != null;
        }
    }
    public bool IsRightCollidingWithPlatform
    {
        get
        {
            return RightHit.collider != null;
        }
    }
    #endregion

    public PlayerCollisions(PlayerController controller)
    {
        this.controller = controller;
    }

    public void UpdateCollisions()
    {
        if (controller.move.y > 0) CheckTopCollisions();
        if (controller.move.y < 0) CheckBottomCollisions();
        if (controller.move.x < 0) CheckLeftCollisions();
        if (controller.move.x > 0) CheckRightCollisions();
    }

    void CheckTopCollisions()
    {
        if (IsTopCollidingWithPlatform)
        {
            float difference = controller.BoxCollider.bounds.max.y - TopHit.collider.bounds.min.y;
            controller.Body.position = controller.Body.position + (Vector2.down * difference);
            controller.move.y = 0.0f;
        }
    }

    void CheckBottomCollisions()
    {
        if (IsBottomCollidingWithPlatform)
        {
            float difference = BottomHit.collider.bounds.max.y - controller.BoxCollider.bounds.min.y;
            controller.Body.position = controller.Body.position + (Vector2.up * difference);
            controller.move.y = 0.0f;
        }
    }

    void CheckLeftCollisions()
    {
        if (IsLeftCollidingWithPlatform)
        {
            float difference = LeftHit.collider.bounds.max.x - controller.BoxCollider.bounds.min.x;
            controller.Body.position = controller.Body.position + (Vector2.right * difference);
            controller.move.x = 0.0f;
        }
    }

    void CheckRightCollisions()
    {
        if (IsRightCollidingWithPlatform)
        {
            float difference = controller.BoxCollider.bounds.max.x - RightHit.collider.bounds.min.x;
            controller.Body.position = controller.Body.position + (Vector2.left * difference);
            controller.move.x = 0.0f;
        }
    }
}