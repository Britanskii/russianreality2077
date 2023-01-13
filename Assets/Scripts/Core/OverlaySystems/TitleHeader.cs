using System;
using System.Collections;
using Core.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.OverlaySystems
{
    public class TitleHeader : MonoBehaviour
    {
        public Image banner;
        public TMP_Text titleText;

        public string title
        {
            get => titleText.text;
            set => titleText.text = value;
        }

        public enum DISPLAY_METHOD
        {
            instant,
            slowFade,
            typeWriter,
        }

        public DISPLAY_METHOD DisplayMethod = DISPLAY_METHOD.instant;
        public float fadeSpeed = 1;

        public void Show(string displayTitle)
        {
            title = displayTitle;

            if (isRevealing)
            {
                StartCoroutine(Revealing());
            }

            _revealing = StartCoroutine(Revealing());
        }

        public void Hide()
        {
            if (isRevealing)
            {
                StartCoroutine(Revealing());
            }

            _revealing = null;

            banner.enabled = false;
            titleText.enabled = false;
        }

        public bool isRevealing => _revealing != null;
        private Coroutine _revealing;

        private IEnumerator Revealing()
        {
            banner.enabled = true;
            titleText.enabled = true;

            switch (DisplayMethod)
            {
                case DISPLAY_METHOD.instant:
                    banner.color = GlobalFunctions.GlobalFunctions.SetAlpha(banner.color, 1);
                    titleText.color = GlobalFunctions.GlobalFunctions.SetAlpha(titleText.color, 1);
                    break;
                case DISPLAY_METHOD.slowFade:
                    banner.color = GlobalFunctions.GlobalFunctions.SetAlpha(banner.color, 0);
                    titleText.color = GlobalFunctions.GlobalFunctions.SetAlpha(titleText.color, 0);
                    while (banner.color.a < 1)
                    {
                        banner.color = GlobalFunctions.GlobalFunctions.SetAlpha(banner.color,
                            Mathf.MoveTowards(banner.color.a, 1, fadeSpeed * Time.unscaledDeltaTime));
                        titleText.color = GlobalFunctions.GlobalFunctions.SetAlpha(titleText.color, banner.color.a);
                        yield return new WaitForEndOfFrame();
                    }

                    break;
                case DISPLAY_METHOD.typeWriter:
                    banner.color = GlobalFunctions.GlobalFunctions.SetAlpha(banner.color, 1);
                    titleText.color = GlobalFunctions.GlobalFunctions.SetAlpha(titleText.color, 1);
                    TextArchitect textArchitect = new(titleText, title);
                    while (textArchitect.isConstructing)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _revealing = null;
        }
    }
}