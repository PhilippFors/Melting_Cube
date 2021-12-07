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
                text.color = dangerColor;
            }
            else if (v >= 0.3f && v < 0.8f) {
                text.color = warningColor;
            }
            else {
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