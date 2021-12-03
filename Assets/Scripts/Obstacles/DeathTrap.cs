using Entities.Player.PlayerInput;
using UnityEngine;

namespace DefaultNamespace.Obstacles
{
    public class DeathTrap : MonoBehaviour
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
}