using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Core.OverlaySystems
{
    public class InputScreen : MonoBehaviour
    {
        public static InputScreen Instance;

        public TMP_InputField inputField;
        public static string currentInput => Instance.inputField.text;
        public TitleHeader Header;
        public GameObject root;
        private static Coroutine revealing;

        private void Awake()
        {
            Instance = this;
            Hide();
        }

        public static void Show(string title, bool clearCurrentInput = true)
        {
            Instance.root.SetActive(true);

            if (clearCurrentInput)
            {
                Instance.inputField.text = "";
            }

            if (title != "")
            {
                Instance.Header.Show(title);
            }
            else
            {
                Instance.Header.Hide();
            }

            if (isRevealing)
            {
                Instance.StopCoroutine(Revealing());
            }

            // revealing =
                Instance.StopCoroutine(Revealing());
        }

        public static void Hide()
        {
            Instance.root.SetActive(false);
            Instance.Header.enabled = false;
        }

        public static bool isWaitingForUserInput => Instance.root.activeInHierarchy;

        public static bool isRevealing => revealing != null;

        private static IEnumerator Revealing()
        {
            Instance.inputField.gameObject.SetActive(false);
            
            while (Instance.Header.isRevealing)
            {
                yield return new WaitForEndOfFrame();
            }
            
            Instance.inputField.gameObject.SetActive(true);

            revealing = null;
        }

        public void Accept()
        {
            Hide();
        }
    }
}