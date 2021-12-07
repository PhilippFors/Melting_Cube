using DG.Tweening;
using UnityEngine;

public class FancyCubeMatChanger : MonoBehaviour
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
        mat = GetComponent<MeshRenderer>().materials[1];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>()) {
            var sequence = DOTween.Sequence().Append(
                mat.DOVector(lightColor, "_EmissionColor", turnOnTime)
            ).Append(
                mat.DOVector(normalColor, "_EmissionColor", turnOffTime)
            );
            sequence.Play();
        }
    }
}