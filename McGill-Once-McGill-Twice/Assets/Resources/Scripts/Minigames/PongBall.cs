using UnityEngine;
using System.Collections;
using System;

public class PongBall : Photon.MonoBehaviour
{
    // public PhotonPlayer Thrower;

    private event Action ThrowFinished;
    private Coroutine CheckingThrowFinished;
    private IEnumerator CheckThrowFinished()
    {
        throw new NotImplementedException();
        // TODO : while loop
        while (false)
            { yield return null; }

        //  Call the ThrowFinished event to trigger any registered callbacks
        if (ThrowFinished != null)
        {
            ThrowFinished();

            //  Remove all the callbacks once they've been triggered
            Delegate[] callbacks = ThrowFinished.GetInvocationList();
            foreach (Delegate callback in callbacks)
                { ThrowFinished -= (callback as Action); }
        }
        
        CheckingThrowFinished = null;
    }

    /// <summary>
    ///  An operation that does...
    /// 
    ///  @param firstParam a description of this parameter
    /// </summary>
    /// <param name="callback">
    /// </param>
    /// <returns>
    /// </returns>
    public void AddThrowFinishedCallback(Action callback)
    {
        //  Register the callback only for the client who threw
        if (this.photonView.isMine)
            { ThrowFinished += callback; }
        
        //  Only check for the retarget finish if not already checking
        if (CheckingThrowFinished == null)
            { CheckingThrowFinished = StartCoroutine(CheckThrowFinished()); }
    }
    
    public void SkipThrow()
    {
        throw new NotImplementedException();
    }
}