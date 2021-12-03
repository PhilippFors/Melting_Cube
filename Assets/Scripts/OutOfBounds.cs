using Entities.Player.PlayerInput;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var meltingController = other.GetComponentInParent<MeltingController>();
        if (meltingController) {
            meltingController.CurrentSize = 0;
            meltingController.OnDeath();
        }
    }
}
