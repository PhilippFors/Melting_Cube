using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace.Visual
{
    public class WallSlideCubeMatChanger : MonoBehaviour
    {
        [SerializeField] private float turnOnTime = 0.5f;
        [SerializeField] private float turnOffTime = 0.5f;

        [SerializeField, ColorUsage(true, true)]
        private Color normalColor;

        [SerializeField, ColorUsage(true, true)]
        private Color lightColor;

        private Material mat;

        private void Awake()
        {
            mat = GetComponentInParent<MeshRenderer>().materials[1];
        }

        private void OnTriggerStay(Collider other)
        {
            var player = other.GetComponentInParent<PlayerController>();
            if (player && player.OnWall) {
                mat.DOVector(lightColor, "_EmissionColor", turnOnTime);
            }
            else {
                mat.DOVector(normalColor, "_EmissionColor", turnOffTime);
            }
        }
    }
}