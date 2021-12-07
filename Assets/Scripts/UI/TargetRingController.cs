using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TargetRingController : MonoBehaviour
{
    [SerializeField] private Texture thickerRing;
    [SerializeField] private Texture thinnerRing;
    [SerializeField] private float fadeDuration;
    [SerializeField, ColorUsage(true, true)] private Color transparentColor;
    [SerializeField, ColorUsage(true, true)] private Color fullColor;

    private RawImage image;
    private Material mat;

    private void Awake()
    {
        image = GetComponent<RawImage>();
        mat = image.material;
        
        SetTransparency(false);
        
    }

    public void SetTransparency(bool isTransparent)
    {
        if (isTransparent) {
            mat.DOColor(transparentColor, fadeDuration);
        }
        else {
            mat.DOColor(fullColor, fadeDuration);
        }
    }

    public void ExpandCircle(bool expand, float maxDist = 0, float scale = 0)
    {
        if (expand) {
            transform.DOScale(new Vector3(maxDist, maxDist, maxDist) * 2, 0.5f);
            image.texture = thinnerRing;
        }
        else {
            transform.DOScale(scale, 0.3f);
            image.texture = thickerRing;
        }
    }
}
