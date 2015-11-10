using UnityEngine;
using System;

[RequireComponent(typeof (NavMeshAgent))]
[RequireComponent(typeof (ThirdPersonCharacterCustom))]
public class AICharacterControlCustom : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
    public ThirdPersonCharacterCustom character { get; private set; } // the character we are controlling
    public Transform target; // target to aim for
    
    public delegate void DestinationReachedHandler();
    private DestinationReachedHandler DestinationReached;
    private bool _Delay;
    public bool Delay { get { return _Delay; }  set { _Delay = value; DelayCounter = 0; } }
    private int DelayCounter = 0;

    // Use this for initialization
    private void Awake()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacterCustom>();
        Delay = false;

        agent.updateRotation = false;
        agent.updatePosition = true;
    }


    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            if (!Delay && !agent.pathPending && agent.remainingDistance <= (agent.stoppingDistance - float.Epsilon))
            {
                if (DestinationReached != null) DestinationReached();
                
                // The nav agent doesn't work well unless there's a few frames of delay, DestinationReached may have set a delay
                if (!Delay) target = null;
            }
            else
            {
                // use the values to move the character
                character.Move(agent.desiredVelocity, false, false);
            }
        }
        else
        {
            // We still need to call the character's move function, but we send zeroed input as the move param.
            character.Move(Vector3.zero, false, false);
        }
        
        // Deal with the nav mesh's bug
        if (Delay && DelayCounter++ < 5)
            { /* Sustain the delay */}
        else
            { Delay = false; }
    }


    public void SetTarget(Transform target)
    {
        this.DestinationReached = null;
        this.target = target;
        Delay = true;
    }
    
    public void SetTarget(Transform target, DestinationReachedHandler callback)
    {
        SetTarget(target);
        this.DestinationReached = callback;
    }
}
