using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class AdaptiveText : MonoBehaviour
    {
        public bool continualUpdate = true;
        public int fontSizeAtDefaultResolution = 36;
        public static float defaultResolution = 2648f;
        
        private TMP_Text _text;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();

            if (continualUpdate)
            {
                InvokeRepeating(nameof(Adjust), 0f, 1f);
            }
            else
            {
                Adjust();
                enabled = false;
            }
        }

        private void Adjust()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            float totalCurrenResolution = Screen.height + Screen.width;
            float percent = totalCurrenResolution / defaultResolution;

            int fontSize = Mathf.RoundToInt(fontSizeAtDefaultResolution * percent);

            _text.fontSize = fontSize;
        }
    }
}