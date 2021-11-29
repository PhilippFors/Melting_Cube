using Entities.Player.PlayerInput;
using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            var melt = other.GetComponent<MeltingController>();
            melt.enabled = false;
            PlayerInputController.Instance.DisableControls();
            SceneLoader.Instance.LoadNextLevel();
        }
    }
}
