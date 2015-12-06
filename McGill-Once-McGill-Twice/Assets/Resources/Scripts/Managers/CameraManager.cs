using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Cameras;

public class CameraManager : UnitySingleton<CameraManager>
{
    [SerializeField] private FreeLookCamCustom CamScript;
    [SerializeField] private ProtectCameraFromWallClipCustom CamWallProtectScript;
    
    /// <summary>Immediately sets the viewing freedom of the camera with relation to <paramref name="forward"/>.</summary>
    /// <remarks>Values >=180 for <paramref name="maxAngle"/> leave the viewing freedom unconstrained.</remarks>
    /// <param name="forward">The vector to use as camera forward in world space coordinates. Viewing will be restricted to <paramref name="maxAngle"/> degrees away from this vector.</param>
    /// <param name="maxAngle">The degree maximum the camera's forward vector is allowed to diverge from <paramref name="forward"/>.</param>
    public static void SetViewLookAngleMaxImmediate(Vector3 forward, float maxAngle)
    {
        Instance.CamScript.SetForwardDirectionImmediate(forward);
        Instance.CamScript.SetLookAngleMaxImmediate(maxAngle);
    }
    
    /// <summary>Gradually sets the viewing freedom of the camera with relation to <paramref name="forward"/>.</summary>
    /// <remarks>The speed at which the viewing freedom changes is determined by the value of camera m_movespeed when this method is called. Values >=180 for <paramref name="maxAngle"/> leave the viewing freedom unconstrained.</remarks>
    /// <param name="forward">The vector to use as camera forward in world space coordinates. Viewing will be restricted to <paramref name="maxAngle"/> degrees away from this vector.</param>
    /// <param name="maxAngle">The degree maximum the camera's forward vector is allowed to diverge from <paramref name="forward"/>.</param>
    public static void SetViewLookAngleMax(Vector3 forward, float maxAngle)
    {
        Instance.CamScript.SetLookAngleMax(maxAngle);
        Instance.CamScript.SetForwardDirection(forward);
    }
    
    /// <summary>Gradually sets the viewing freedom of the camera with relation to the camera's currently set forward direction (not the camera's current forward, but what is set as forward).</summary>
    /// <remarks>Values >=180 for <paramref name="maxAngle"/> leave the viewing freedom unconstrained.</remarks>
    /// <param name="maxAngle">The degree maximum the camera's forward vector is allowed to diverge from <paramref name="forward"/>.</param>
    /// <param name="speed">The speed at which the viewing freedom should be changed.</param>
    public static void SetViewLookAngleMax(float maxAngle, float speed)
    {
        Instance.CamScript.SetLookAngleMax(maxAngle, speed);
    }
    
    /// <summary>Gradually sets the viewing freedom of the camera with relation to the camera's currently set forward direction (not the camera's current forward, but what is set as forward).</summary>
    /// <remarks>Values >=180 for <paramref name="maxAngle"/> leave the viewing freedom unconstrained.</remarks>
    /// <param name="forward">The vector to use as camera forward in world space coordinates. Viewing will be restricted to <paramref name="maxAngle"/> degrees away from this vector.</param>
    /// <param name="maxAngle">The degree maximum the camera's forward vector is allowed to diverge from <paramref name="forward"/>.</param>
    /// <param name="speed">The speed at which the viewing freedom should be changed.</param>
    public static void SetViewLookAngleMax(Vector3 forward, float maxAngle, float speed)
    {
        SetViewLookAngleMax(maxAngle, speed);
        Instance.CamScript.SetForwardDirection(forward, speed);
    }
    
    /// <summary>Immediately sets the camera's forward direction which is used in calculating the viewing freedom. The direction given is the new 0 angle offset.</summary>
    /// <param name="forward">The direction in world space coordinates that camera's forward should point toward.</param>
    public static void SetViewForwardImmediate(Vector3 forward)
    {
        Instance.CamScript.SetForwardDirectionImmediate(forward);
    }
    
    /// <summary>Gradually sets the camera's forward direction which is used in calculating the viewing freedom. The direction given is the new 0 angle offset.</summary>
    /// <param name="forward">The direction in world space coordinates that camera's forward should point toward.</param>
    /// <param name="speed">The speed at which the camera's forward direction changes.</param>
    public static void SetViewForward(Vector3 forward, float speed)
    {
        Instance.CamScript.SetForwardDirection(forward, speed);
    }
    
    /// <summary>Gradually sets the camera's forward direction which is used in calculating the viewing freedom. The direction given is the new 0 angle offset.</summary>
    /// <remarks>The speed at which the camera's forward direction changes is the value of m_movespeed when this method is called.</remarks>
    /// <param name="forward">The direction in world space coordinates that camera's forward should point toward.</param>
    public static void SetViewForward(Vector3 forward)
    {
        Instance.CamScript.SetForwardDirection(forward);
    }
    
    /// <summary>Immediately points the camera's center toward <paramref name="targetPos"/>.</summary>
    /// <param name="targetPos">The position in world space coordinates that the camera's center should point toward.</param>
    public static void SetViewDirectionTowardsTarget(Vector3 targetPos)
    {
        Instance.CamScript.SetViewDirectionTowardsTarget(targetPos);
    }
    
    /// <summary>Immediately sets the camera's pivoting radius around its target's transform. This effectively sets the distance of the camera from its pivot along the -Z axis.</summary>
    /// <param name="radius">The distance the camera should place between its pivot and itself.</param>
    public static void SetPivotRadiusImmediate(float radius)
    {
        Instance.CamScript.SetPivotRadiusImmediate(radius);
    }
    
    /// <summary>Gradually sets the camera's pivoting radius around its target's transform. This effectively sets the distance of the camera from its pivot along the -Z axis.</summary>
    /// <param name="radius">The distance the camera should place between its pivot and itself.</param>
    /// <param name="moveSpeed">The speed at which the pivot radius should change.</param>
    public static void SetPivotRadius(float radius, float moveSpeed)
    {
        Instance.CamScript.SetPivotRadius(radius, moveSpeed);
    }
    
    /// <summary>Gradually sets the camera's pivoting radius around its target's transform. This effectively sets the distance of the camera from its pivot along the -Z axis.</summary>
    /// <remarks>The speed at which the pivot radius should change is the value of m_movespeed when this method is called.</remarks>
    /// <param name="radius">The distance the camera should place between its pivot and itself.</param>
    public static void SetPivotRadius(float radius)
    {
        Instance.CamScript.SetPivotRadius(radius);
    }
    
    /// <summary>Transitions the camera to follow the player in third person.</summary>
    /// <param name="moveSpeed">The speed at which the transition occurs.</param>
    public static void SetViewToPlayer(float moveSpeed)
    {
        Instance.CamScript.SetPlayerAsTarget(moveSpeed);
        SetViewLookAngleMax(180f, moveSpeed);
    }
    
    /// <summary>Transitions the camera to follow the player in third person.</summary>
    /// <remarks>The speed at which the transition occurs is the value of m_movespeed when this method is called.</remarks>
    public static void SetViewToPlayer()
    {
        Instance.CamScript.SetPlayerAsTarget();
        Instance.CamScript.SetLookAngleMax(180f);
    }
    
    /// <summary>Prevents the camera position from following its current target.</summary>
    public static void LockViewPosition()
    {
        Instance.CamScript.LockPosition();
    }
    
    /// <summary>Immediately sets the position of the camera to <paramref name="position"/>.</summary>
    /// <param name="position">The position to move the camera to.</param>
    public static void SetViewPositionImmediate(Vector3 position)
    {
        Instance.CamScript.LockPositionImmediate(position);
    }
    
    /// <summary>Gradually sets the position of the camera to <paramref name="position"/>.</summary>
    /// <param name="position">The position to move the camera to.</param>
    /// <param name="moveSpeed">The speed at which to move the camera to <paramref name="position"/>.</param>
    public static void SetViewPosition(Vector3 position, float moveSpeed)
    {
        Instance.CamScript.LockPosition(position, moveSpeed);
    }
    
    /// <summary>Gradually sets the position of the camera to <paramref name="position"/>.</summary>
    /// <remarks>The speed at which the camera moves to <paramref name="position"/> is the value of m_movespeed when this method is called.</remarks>
    /// <param name="position">The position to move the camera to.</param>
    public static void SetViewPosition(Vector3 position)
    {
        Instance.CamScript.LockPosition(position);
    }
    
    /// <summary>Adds <paramref name="callback"/> to a list of callbacks to be invoked when the camera reaches its target position.</summary>
    /// <param name="callback">The callback action to invoke when the camera reaches its target position.</param>
    public static void CallbackOnPositionReached(Action<Vector3> callback)
    {
        Instance.CamScript.CallbackOnPositionReached(callback);
    }
    
    /// <summary>Sets the camera's m_movespeed.</summary>
    /// <param name="moveSpeed">The speed to set m_movespeed to.</param>
    public static void SetMoveSpeed(float moveSpeed)
    {
        Instance.CamScript.SetMoveSpeed(moveSpeed);
    }
}
