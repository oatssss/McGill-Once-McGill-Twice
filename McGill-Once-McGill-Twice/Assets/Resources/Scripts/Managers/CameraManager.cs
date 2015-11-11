using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Cameras;

public class CameraManager : UnitySingleton<CameraManager>
{
    public static readonly float FadeDuration = 1f;
    
    private Coroutine Fading = null;
    [SerializeField] private CanvasRenderer FadeRenderer;
    [SerializeField] private FreeLookCamCustom CamScript;
    [SerializeField] private ProtectCameraFromWallClipCustom CamWallProtectScript;
    /*private FreeLookCamCustom CamScript
    {
        get
        {
            GameObject cameraRig = GameObject.FindWithTag("CameraRig");
            if (cameraRig != null)
            {
                FreeLookCamCustom camScript = cameraRig.GetComponent<FreeLookCamCustom>();
                if (camScript != null)
                    { return camScript; }
            }

            Debug.LogErrorFormat("{0} could not obtain the camera script.", this);
            return null;
        }
    }*/
    
    /*private ProtectCameraFromWallClip CamWallProtectScript
    {
        get
        {
            GameObject cameraRig = GameObject.FindWithTag("CameraRig");
            if (cameraRig != null)
            {
                ProtectCameraFromWallClip camWallProtectScript = cameraRig.GetComponent<ProtectCameraFromWallClip>();
                if (camWallProtectScript != null)
                    { return camWallProtectScript; }
            }

            Debug.LogErrorFormat("{0} could not obtain the wallclip protection script.", this);
            return null;
        }
    }*/
    
    void Start()
    {
        FadeToClear(null);
    }

    public static void FadeToBlack(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(DoFadeToBlack(callback));
    }

    public static void FadeToClear(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(DoFadeToClear(callback));
    }

    private static IEnumerator DoFadeToBlack(Action callback)
    {
        yield return Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.FadeRenderer, 0f, 1f, 0.5f, FadeUtility.EaseType.InOut));

        if (callback != null)
            { callback(); }
    }

    private static IEnumerator DoFadeToClear(Action callback)
    {
        yield return Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.FadeRenderer, 1f, 0f, 0.5f, FadeUtility.EaseType.InOut));

        if (callback != null)
            { callback(); }
    }
    
    public static void SetViewLookAngleMaxImmediate(Vector3 forward, float maxAngle)
    {
        Instance.CamScript.SetForwardDirectionImmediate(forward);
        Instance.CamScript.SetLookAngleMaxImmediate(maxAngle);
    }
    
    public static void SetViewLookAngleMax(Vector3 forward, float maxAngle)
    {
        Instance.CamScript.SetLookAngleMax(maxAngle);
        Instance.CamScript.SetForwardDirection(forward);
    }
    
    public static void SetViewLookAngleMax(float maxAngle, float speed)
    {
        Instance.CamScript.SetLookAngleMax(maxAngle, speed);
    }
    
    public static void SetViewLookAngleMax(Vector3 forward, float maxAngle, float speed)
    {
        SetViewLookAngleMax(maxAngle, speed);
        Instance.CamScript.SetForwardDirection(forward, speed);
    }
    
    public static void SetViewForwardImmediate(Vector3 forward)
    {
        Instance.CamScript.SetForwardDirectionImmediate(forward);
    }
    
    public static void SetViewForward(Vector3 forward, float speed)
    {
        Instance.CamScript.SetForwardDirection(forward, speed);
    }
    
    public static void SetViewForward(Vector3 forward)
    {
        Instance.CamScript.SetForwardDirection(forward);
    }
    
    public static void ForceViewDirectionTowardsTarget(Vector3 targetPos)
    {
        Instance.CamScript.ForceViewDirectionTowardsTarget(targetPos);
    }
    
    public static void SetPivotRadiusImmediate(float radius)
    {
        Instance.CamScript.SetPivotRadiusImmediate(radius);
    }
    
    public static void SetPivotRadius(float radius, float moveSpeed)
    {
        Instance.CamScript.SetPivotRadius(radius, moveSpeed);
    }
    
    public static void SetPivotRadius(float radius)
    {
        Instance.CamScript.SetPivotRadius(radius);
    }
    
    public static void SetViewToPlayer(float moveSpeed)
    {
        Instance.CamScript.SetPlayerAsTarget(moveSpeed);
        SetViewLookAngleMax(180f, moveSpeed);
    }
    
    public static void SetViewToPlayer()
    {
        Instance.CamScript.SetPlayerAsTarget();
        Instance.CamScript.SetLookAngleMax(180f);
    }
    
    public static void LockViewPosition()
    {
        Instance.CamScript.LockPosition();
    }
    
    public static void SetViewPositionImmediate(Vector3 position)
    {
        Instance.CamScript.LockPositionImmediate(position);
    }
    
    public static void SetViewPosition(Vector3 position, float moveSpeed)
    {
        Instance.CamScript.LockPosition(position, moveSpeed);
        
        //  Account for wall collisions if setting the view position to be an immediate snap
        //  if (moveSpeed >= MoveSpeedImmediate)
        //  {
        //      float originalReturnTime = Instance.CamWallProtectScript.returnTime;
        //      Instance.CamWallProtectScript.returnTime = 0f;
        //      CallbackOnPositionReached( finalPosition => { Instance.CamWallProtectScript.returnTime = originalReturnTime; } );
        //  }
    }
    
    public static void SetViewPosition(Vector3 position)
    {
        Instance.CamScript.LockPosition(position);
    }
    
    public static void CallbackOnPositionReached(Action<Vector3> callback)
    {
        Instance.CamScript.CallbackOnRetarget(callback);
    }
    
    public static void SetMoveSpeed(float moveSpeed)
    {
        Instance.CamScript.SetMoveSpeed(moveSpeed);
    }
}
