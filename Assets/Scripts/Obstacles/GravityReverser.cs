using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace.Obstacles
{
    public class GravityReverser : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerController>()) {
                GameManager.Instance.ToggleGravity();
            }
        }
    }
}