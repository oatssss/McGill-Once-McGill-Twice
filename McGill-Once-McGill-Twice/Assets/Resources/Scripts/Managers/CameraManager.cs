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
    [SerializeField] private ProtectCameraFromWallClip CamWallProtectScript;
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

    public void FadeToBlack(Action callback)
    {
        if (this.Fading != null)
            { StopCoroutine(this.Fading); }

        Fading = StartCoroutine(this.DoFadeToBlack(callback));
    }

    public void FadeToClear(Action callback)
    {
        if (this.Fading != null)
            { StopCoroutine(this.Fading); }

        Fading = StartCoroutine(this.DoFadeToClear(callback));
    }

    private IEnumerator DoFadeToBlack(Action callback)
    {
        yield return StartCoroutine(FadeUtility.UIAlphaFade(FadeRenderer, 0f, 1f, 0.5f, FadeUtility.EaseType.InOut));

        if (callback != null)
            { callback(); }
    }

    private IEnumerator DoFadeToClear(Action callback)
    {
        yield return StartCoroutine(FadeUtility.UIAlphaFade(FadeRenderer, 1f, 0f, 0.5f, FadeUtility.EaseType.InOut));

        if (callback != null)
            { callback(); }
    }
    
    public void SetViewLookAngleMax(Vector3 forward, float maxAngle)
    {
        this.CamScript.SetLookAngleMax(maxAngle);
        this.CamScript.SetForwardDirection(forward);
    }
    
    public void SetViewLookAngleMax(float maxAngle, float speed)
    {
        this.CamScript.SetLookAngleMax(maxAngle, speed);
    }
    
    public void SetViewLookAngleMax(Vector3 forward, float maxAngle, float speed)
    {
        SetViewLookAngleMax(maxAngle, speed);
        this.CamScript.SetForwardDirection(forward, speed);
    }
    
    public void SetViewForward(Vector3 forward, float speed)
    {
        this.CamScript.SetForwardDirection(forward, speed);
    }
    
    public void SetViewForward(Vector3 forward)
    {
        this.CamScript.SetForwardDirection(forward);
    }
    
    public void ForceViewDirectionTowardsTarget(Vector3 targetPos)
    {
        this.CamScript.ForceViewDirectionTowardsTarget(targetPos);
    }
    
    public void SetPivotRadius(float radius, float moveSpeed)
    {
        this.CamScript.SetPivotRadius(radius, moveSpeed);
    }
    
    public void SetPivotRadius(float radius)
    {
        this.CamScript.SetPivotRadius(radius);
    }
    
    public void SetViewToPlayer(float moveSpeed)
    {
        this.CamScript.SetPlayerAsTarget(moveSpeed);
        SetViewLookAngleMax(180f, moveSpeed);
    }
    
    public void SetViewToPlayer()
    {
        this.CamScript.SetPlayerAsTarget();
        this.CamScript.SetLookAngleMax(180f);
    }
    
    public void LockViewPosition()
    {
        this.CamScript.LockPosition();
    }
    
    public void SetViewPosition(Vector3 position, float moveSpeed)
    {
        this.CamScript.LockPosition(position, moveSpeed);
    }
    
    public void SetViewPosition(Vector3 position)
    {
        this.CamScript.LockPosition(position);
    }
}
