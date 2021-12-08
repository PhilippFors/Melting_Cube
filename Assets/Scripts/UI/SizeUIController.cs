using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UsefulCode.SOArchitecture;

namespace UI
{
    /// <summary>
    /// Controlls the displayed size percentage.
    /// </summary>
    public class SizeUIController : MonoBehaviour
    {
        public FloatVariable currentSize;
        public TMP_Text text;
        public Color okColor;
        public Color warningColor;
        public Color dangerColor;
        [ColorUsage(true, true)] public Color matokColor;
        [ColorUsage(true, true)] public Color matwarningColor;
        [ColorUsage(true, true)] public Color matdangerColor;
        public MeshRenderer meshRenderer;
        
        private Material mat;

        private void OnEnable()
        {
            if (meshRenderer) {
                mat = meshRenderer.materials[1];
            }
        }

        private void Start()
        {
            currentSize.onValueChanged += SetSize;
            SetSize(currentSize.Value);
        }

        private void OnDestroy()
        {
            currentSize.onValueChanged -= SetSize;
        }

        private void SetSize(float v)
        {
            if (v < 0.3f) {
                if (mat) {
                    mat.DOVector(matdangerColor, "_EmissionColor", 0.2f);
                }

                text.color = dangerColor;
            }
            else if (v >= 0.3f && v < 0.8f) {
                if (mat) {
                    mat.DOVector(matwarningColor, "_EmissionColor", 0.2f);
                }

                text.color = warningColor;
            }
            else {
                if (mat) {
                    mat.DOVector(matokColor,"_EmissionColor", 0.2f);
                }

                text.color = okColor;
            }
        
            var size = Mathf.Round(v * 100);
            if (size < 0) {
                size = 0;
            }
            var t = $"{size}%";
            text.text = t;
        }
    }
}
