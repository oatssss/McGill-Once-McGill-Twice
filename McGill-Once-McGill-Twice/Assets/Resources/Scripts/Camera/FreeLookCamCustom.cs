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

    [SerializeField] private float m_MoveSpeed = 7f;                      // How fast the rig will move to keep up with the target's position.
    [SerializeField] private float m_OriginalMoveSpeed = 7f;
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
    //  private Quaternion m_TransformTargetRot;
    
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
        //  m_TransformTargetRot = transform.localRotation;
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
    
   /* * * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to adjust the looking angle     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * * */
    private Coroutine AdjustingLookAngleMax = null;
    private IEnumerator AdjustLookAngleMax(float maxAngle, float speed)
    {
        float original = m_LookAngleMax;
        float t_max = GameConstants.TRANSITION_SPEED_MULTIPLIER/speed;
        float t = 0;
        
        while (t < t_max)
        {
            t += Time.deltaTime;
            m_LookAngleMax = Mathf.Lerp(original, maxAngle, FadeUtility.Ease(t/t_max, FadeUtility.EaseType.InOut));
            yield return null;
        }
        
        SetLookAngleMaxImmediate(maxAngle);
        AdjustingLookAngleMax = null;
    }
    
    public void SetLookAngleMaxImmediate(float maxAngle)
    {
        m_LookAngleMax = maxAngle;
    }
    
    public void SetLookAngleMax(float maxAngle, float speed)
    {
        if (AdjustingLookAngleMax != null)
            { StopCoroutine(AdjustingLookAngleMax); }
        AdjustingLookAngleMax = StartCoroutine(AdjustLookAngleMax(maxAngle, speed));
    }
    
    public void SetLookAngleMax(float maxAngle)
    {
        SetLookAngleMax(maxAngle, m_OriginalMoveSpeed);
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to adjust the forward direction (forward is used for look angle constraints)  * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    private Coroutine AdjustingForwardDirection = null;
    private IEnumerator AdjustForwardDirection(Vector3 forward, float speed)
    {
        Vector3 original = m_ForwardDirection;
        float t_max = GameConstants.TRANSITION_SPEED_MULTIPLIER/speed;
        float t = 0;
        
        while (t < t_max)
        {
            t += Time.deltaTime;
            m_ForwardDirection = Vector3.Lerp(original, forward, FadeUtility.Ease(t/t_max, FadeUtility.EaseType.InOut));
            yield return null;
        }
        
        SetForwardDirectionImmediate(forward);
        AdjustingForwardDirection = null;
    }
    
    public void SetForwardDirectionImmediate(Vector3 forward)
    {
        m_ForwardDirection = new Vector3(forward.x, 0f, forward.z);
    }
    
    public void SetForwardDirection(Vector3 forward, float speed)
    {
        if (AdjustingForwardDirection != null)
            { StopCoroutine(AdjustingForwardDirection); }
        AdjustingForwardDirection = StartCoroutine(AdjustForwardDirection(forward, speed));
    }
    
    public void SetForwardDirection(Vector3 forward)
    {
        SetForwardDirection(forward, m_OriginalMoveSpeed);
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * *
    *   Accessor to adjust the view direction   * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * */
    public void SetViewDirectionTowardsTarget(Vector3 targetPosition)
    {
        Vector3 viewDirection = targetPosition - m_Pivot.transform.position;
        Quaternion viewRotation = Quaternion.LookRotation(viewDirection);
        m_LookAngle = viewRotation.eulerAngles.y;
        m_TiltAngle = viewRotation.eulerAngles.x;
        m_Pivot.localRotation = viewRotation;
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to adjust the pivot radius    * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * */
    private Coroutine AdjustingPivotRadius = null;
    private IEnumerator AdjustPivotRadius(float radius, float speed)
    {
        radius = -Mathf.Abs(radius);
        float original = m_Cam.localPosition.z;
        float t_max = GameConstants.TRANSITION_SPEED_MULTIPLIER/speed;
        float t = 0;
        
        while (t < t_max)
        {
            t += Time.deltaTime;
            float lerpedRadius = Mathf.Lerp(original, radius, FadeUtility.Ease(t/t_max, FadeUtility.EaseType.InOut));
            m_Cam.localPosition = new Vector3(0f, 0f, lerpedRadius);
            
            //  If there's a wall clip script, set it's original distance, or else it'll revert to that
            if (this.ProtectWallClipScript != null)
                { this.ProtectWallClipScript.SetOriginalDistance(Mathf.Abs(lerpedRadius)); }
            yield return null;
        }
        
        SetPivotRadiusImmediate(radius);
        
        AdjustingPivotRadius = null;
    }
    
    public void SetPivotRadiusImmediate(float radius)
    {
        m_Cam.localPosition = new Vector3(0f, 0f, radius);
        if (this.ProtectWallClipScript != null)
            { this.ProtectWallClipScript.SetOriginalDistance(Mathf.Abs(radius)); }
    }
    
    public void SetPivotRadius(float radius, float moveSpeed)
    {
        if (AdjustingPivotRadius != null)
            { StopCoroutine(AdjustingPivotRadius); }
        AdjustingPivotRadius = StartCoroutine(AdjustPivotRadius(radius, moveSpeed));
    }
    
    public void SetPivotRadius(float radius)
    {
        SetPivotRadius(radius, m_OriginalMoveSpeed);
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to lock the camera position   * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * */
    public void LockPositionImmediate(Vector3 position)
    {
        GameObject lockTargetObject = new GameObject("CameraLockTarget");
        Transform lockTargetTransform = lockTargetObject.transform;
        lockTargetTransform.position = position;
        UnlockTarget = m_Target;
        m_Target = lockTargetTransform;
        LockTarget = lockTargetTransform;
        transform.position = position;
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
        LockPosition(position, m_OriginalMoveSpeed);
    }
    
    public void LockPosition()
    {
        LockPosition(m_Target.position);
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to unlock the camera position   * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * * */
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
        UnlockPosition(m_OriginalMoveSpeed);
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to change the camera follow target    * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * * * * * */
    public void SetTarget(Transform target, float moveSpeed)
    {
        base.SetTarget(target);
        this.LockTarget = null;
        this.UnlockTarget = null;
        TemporarilyAdjustMoveSpeed(moveSpeed);
    }
    
    public override void SetTarget(Transform target)
    {
        SetTarget(target, m_OriginalMoveSpeed);
    }
    
    public void SetPlayerAsTarget(float moveSpeed)
    {
        SetTarget(PlayerManager.GetMainPlayer().transform.Find("CameraTarget"), moveSpeed);
        SetPivotRadius(2f);
    }
    
    public void SetPlayerAsTarget()
    {
        SetPlayerAsTarget(m_OriginalMoveSpeed);
    }
    
   /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    *   Accessors to add a callback when the camera reaches its follow target's position  * * * * * * * * * * * * * * * * * * * * * * * * * *
    * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    private event Action<Vector3> PositionReached = null;
    private Coroutine CheckingPositionReached;
    private IEnumerator CheckPositionReached()
    {
        while (transform.position != m_Target.position || !Mathf.Approximately(m_Cam.localPosition.z, -ProtectWallClipScript.GetOriginalDistance()))
            { yield return null; }
        //  callback();

        //  Call the FinishedRetarget event to trigger any registered callbacks
        if (PositionReached != null)
        {
            PositionReached(m_Target.position);

            //  Remove all the callbacks once they've been triggered
            Delegate[] callbacks = PositionReached.GetInvocationList();
            foreach (Delegate callback in callbacks)
                { PositionReached -= (callback as Action<Vector3>); }
        }
        
        CheckingPositionReached = null;
    }
    public void CallbackOnPositionReached(Action<Vector3> callback)
    {
        //  Register the callback
        PositionReached += callback;
        
        //  Only check for the retarget finish if not already checking
        if (CheckingPositionReached == null)
            { CheckingPositionReached = StartCoroutine(CheckPositionReached()); }
    }
    
    private void TemporarilyAdjustMoveSpeed(float speed)
    {
        m_MoveSpeed = speed;
        
        //  Only revert to the untampered speed if it was untampered to begin with, check for the finish if not already checking
        if (CheckingPositionReached == null)
        {
            Action<Vector3> revertSpeed = finalPosition => { m_MoveSpeed = m_OriginalMoveSpeed; CheckingPositionReached = null; };
            PositionReached += revertSpeed;
            CheckingPositionReached = StartCoroutine(CheckPositionReached());
        }
    }
    
    public void SetMoveSpeed(float moveSpeed)
    {
        m_OriginalMoveSpeed = moveSpeed;
        m_MoveSpeed = m_OriginalMoveSpeed;
    }
}
