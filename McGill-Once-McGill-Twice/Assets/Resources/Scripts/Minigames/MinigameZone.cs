using UnityEngine;

public class MinigameZone : MonoBehaviour {

    [SerializeField] private Minigame ParentMinigame;
    [SerializeField] private Collider Zone;

    public void EnableZone()
    {
        this.Zone.enabled = true;
    }

    public void DisableZone()
    {
        this.Zone.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (CustomInputManager.GetButtonDown("Interact Main"))
        {
            // Show game info menu
            this.ParentMinigame.DisplayGameInfo();
            // Disable collider until user exits menu
            this.DisableZone();
        }
    }
}
