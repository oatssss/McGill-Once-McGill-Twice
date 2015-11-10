using UnityEngine;
using System;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Cameras;
using ExtensionMethods;

public class FreeLookCamCustom : PivotBasedCameraRig
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:

    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [SerializeField] private float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
    [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField] private float m_TurnSmoothing = 0.1f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField] private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
    [SerializeField] private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
    [Range(0f, 180)] [SerializeField] private float m_LookAngleMax = 180;
    [SerializeField] private Vector3 m_ForwardDirection = Vector3.forward;
    [SerializeField] private bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
    [SerializeField] private bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return
    [SerializeField] private ProtectCameraFromWallClipCustom ProtectWallClipScript;

    private float m_LookAngle;                    // The rig's y axis rotation.
    private float m_TiltAngle;                    // The pivot's x axis rotation.
    private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
    private Vector3 m_PivotEulers;
    private Quaternion m_PivotTargetRot;
    private Quaternion m_TransformTargetRot;
    
    private Transform _LockTarget;
    private Transform LockTarget
    {
        get
        {
            return _LockTarget;
        }
        set
        {
            if (_LockTarget != null)
                { Destroy(_LockTarget.gameObject); }
            _LockTarget = value;
        }
    }
    private Transform UnlockTarget = null;
    

    protected override void Awake()
    {
        base.Awake();
        // Lock or unlock the cursor.
        Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !m_LockCursor;
        m_PivotEulers = m_Pivot.rotation.eulerAngles;

        m_PivotTargetRot = m_Pivot.transform.localRotation;
        m_TransformTargetRot = transform.localRotation;
    }


    protected void Update()
    {
        HandleRotationMovement();
        if (m_LockCursor && Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_LockCursor;
        }
    }


    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    protected override void FollowTarget(float deltaTime)
    {
        if (m_Target == null) return;
        // Move the rig towards target position.
        transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime*m_MoveSpeed);
    }


    private void HandleRotationMovement()
    {
        if(Time.timeScale < float.Epsilon)
        return;

        // Read the user input
        var x = CrossPlatformInputManager.GetAxis("Mouse X");
        var y = CrossPlatformInputManager.GetAxis("Mouse Y");

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        m_LookAngle += x*m_TurnSpeed;
        
        // Find the clamped m_Lookangle
        Quaternion clampedLookAngleRotation = Quaternion.Euler(0f, m_LookAngle, 0f);
        clampedLookAngleRotation = clampedLookAngleRotation.Clamp(m_ForwardDirection, m_LookAngleMax);
        m_LookAngle = clampedLookAngleRotation.eulerAngles.y;

        if (m_VerticalAutoReturn)
        {
            // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
            // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
            // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
            m_TiltAngle = y > 0 ? Mathf.Lerp(0, -m_TiltMin, y) : Mathf.Lerp(0, m_TiltMax, -y);
        }
        else
        {
            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= y*m_TurnSpeed;
            // and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
        }

        // Rotations are applied to the pivot (the child of this object)
        m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_LookAngle, m_PivotEulers.z);

        if (m_TurnSmoothing > 0)
        {
            m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
        }
        else
        {
            m_Pivot.localRotation = m_PivotTargetRot;
        }
    }
    
    private Coroutine AdjustingLookAngleMax = null;
    private IEnumerator AdjustLookAngleMax(float maxAngle, float speed)
    {
        float original = m_LookAngleMax;
        float t_max = GameConstants.TransitionSpeedMultiplier/speed;
        float t = 0;
        
        while (t < t_max)
        {
            t += Time.deltaTime;
            m_LookAngleMax = Mathf.Lerp(original, maxAngle, FadeUtility.Ease(t/t_max, FadeUtility.EaseType.InOut));
            yield return null;
        }
        
        m_LookAngleMax = maxAngle;
        AdjustingLookAngleMax = null;
    }
    
    public void SetLookAngleMax(float maxAngle, float speed)
    {
        if (AdjustingLookAngleMax != null)
            { StopCoroutine(AdjustingLookAngleMax); }
        AdjustingLookAngleMax = StartCoroutine(AdjustLookAngleMax(maxAngle, speed));
    }
    
    public void SetLookAngleMax(float maxAngle)
    {
        SetLookAngleMax(maxAngle, m_MoveSpeed);
    }
    
    private Coroutine AdjustingForwardDirection = null;
    private IEnumerator AdjustForwardDirection(Vector3 forward, float speed)
    {
        Vector3 original = m_ForwardDirection;
        float t_max = GameConstants.TransitionSpeedMultiplier/speed;
        float t = 0;
        
        while (t < t_max)
        {
            t += Time.deltaTime;
            m_ForwardDirection = Vector3.Lerp(original, forward, FadeUtility.Ease(t/t_max, FadeUtility.EaseType.InOut));
            yield return null;
        }
        
        m_ForwardDirection = forward;
        AdjustingForwardDirection = null;
    }
    
    public void SetForwardDirection(Vector3 forward, float speed)
    {
        if (AdjustingForwardDirection != null)
            { StopCoroutine(AdjustingForwardDirection); }
        AdjustingForwardDirection = StartCoroutine(AdjustForwardDirection(forward, speed));
    }
    
    public void SetForwardDirection(Vector3 forward)
    {
        SetForwardDirection(forward, m_MoveSpeed);
    }
    
    public void ForceViewDirectionTowardsTarget(Vector3 targetPosition)
    {
        Vector3 viewDirection = targetPosition - m_Pivot.transform.position;
        Quaternion viewRotation = Quaternion.LookRotation(viewDirection);
        m_LookAngle = viewRotation.eulerAngles.y;
        m_TiltAngle = viewRotation.eulerAngles.x;
        m_Pivot.localRotation = viewRotation;
    }
    
    private Coroutine AdjustingPivotRadius = null;
    private IEnumerator AdjustPivotRadius(float radius, float speed)
    {
        radius = -Mathf.Abs(radius);
        float original = m_Cam.localPosition.z;
        float t_max = GameConstants.TransitionSpeedMultiplier/speed;
        float t = 0;
        
        while (t < t_max)
        {
            t += Time.deltaTime;
            float lerpedRadius = Mathf.Lerp(original, radius, FadeUtility.Ease(t/t_max, FadeUtility.EaseType.InOut));
            m_Cam.localPosition = new Vector3(0f, 0f, lerpedRadius);
            if (this.ProtectWallClipScript != null)
                { this.ProtectWallClipScript.SetOriginalDistance(Mathf.Abs(lerpedRadius)); }
            yield return null;
        }
        
        m_Cam.localPosition = new Vector3(0f, 0f, radius);
        AdjustingPivotRadius = null;
    }
    
    public void SetPivotRadius(float radius, float moveSpeed)
    {
        if (AdjustingPivotRadius != null)
            { StopCoroutine(AdjustingPivotRadius); }
        AdjustingPivotRadius = StartCoroutine(AdjustPivotRadius(radius, moveSpeed));
    }
    
    public void SetPivotRadius(float radius)
    {
        SetPivotRadius(radius, m_MoveSpeed);
    }
    
    public void LockPosition(Vector3 position, float moveSpeed)
    {
        GameObject lockTargetObject = new GameObject("CameraLockTarget");
        Transform lockTargetTransform = lockTargetObject.transform;
        lockTargetTransform.position = position;
        UnlockTarget = m_Target;
        m_Target = lockTargetTransform;
        LockTarget = lockTargetTransform;
        TemporarilyAdjustMoveSpeed(moveSpeed);
    }
    
    public void LockPosition(Vector3 position)
    {
        LockPosition(position, m_MoveSpeed);
    }
    
    public void LockPosition()
    {
        LockPosition(m_Target.position);
    }
    
    public void UnlockPosition(float moveSpeed)
    {
        if (this.UnlockTarget != null)
        {
            m_Target = this.UnlockTarget;
            TemporarilyAdjustMoveSpeed(moveSpeed);
            this.LockTarget = null;
            this.UnlockTarget = null;
        }
    }
    
    public void UnlockPosition()
    {
        UnlockPosition(m_MoveSpeed);
    }
    
    public void SetTarget(Transform target, float moveSpeed)
    {
        base.SetTarget(target);
        this.LockTarget = null;
        this.UnlockTarget = null;
        TemporarilyAdjustMoveSpeed(moveSpeed);
    }
    
    public override void SetTarget(Transform target)
    {
        SetTarget(target, m_MoveSpeed);
    }
    
    public void SetPlayerAsTarget(float moveSpeed)
    {
        SetTarget(PlayerManager.Instance.MainPlayer.transform.Find("CameraTarget"), moveSpeed);
        SetPivotRadius(2f);
    }
    
    public void SetPlayerAsTarget()
    {
        SetPlayerAsTarget(m_MoveSpeed);
    }
    
    private Coroutine CheckingRetargetFinished;
    private IEnumerator CheckRetargetFinished(Action callback)
    {
        while (transform.position != m_Target.position)
            { yield return null; }
        callback();
    }
    private void TemporarilyAdjustMoveSpeed(float speed)
    {
        float untamperedSpeed = m_MoveSpeed;
        m_MoveSpeed = speed;
        
        //  Only check for the finish if not already checking
        if (CheckingRetargetFinished == null)
            { StartCoroutine(CheckRetargetFinished( () => { m_MoveSpeed = untamperedSpeed; CheckingRetargetFinished = null; } )); }
    }
}
