using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core.Text
{
    public class TextArchitect
    {
        public bool Skip = false;

        public bool isConstructing => buildProcess != null;

        public int charactersPerFrame = 1;
        public float speed = 1f;

        private static Dictionary<TMP_Text, TextArchitect> _activeArchitects = new();

        private string _preText;
        private string _targetText;

        private Coroutine buildProcess;

        private TMP_Text tmpro;

        public TextArchitect(TMP_Text tmpro, string targetText, string preText = "", int charactersPerFrame = 1,
            float speed = 1f)
        {
            tmpro.text = targetText;
            this.tmpro = tmpro;
            _targetText = targetText;
            _preText = preText;
            this.charactersPerFrame = charactersPerFrame;
            this.speed = Mathf.Clamp(speed, 1f, 300f);

            Initiate();
        }

        public void ShowText(string text)
        {
            if (isConstructing)
            {
                DialogueSystem.Instance.StopCoroutine(buildProcess);
            }

            _targetText = text;
            tmpro.text = text;

            tmpro.maxVisibleCharacters = tmpro.text.Length;

            if (tmpro == DialogueSystem.Instance.speechText)
            {
                DialogueSystem.Instance.TargetSpeech = text;
            }
        }

        public void Renew(string target, string preText)
        {
            _targetText = target;
            _preText = preText;

            Skip = false;

            if (isConstructing)
            {
                DialogueSystem.Instance.StopCoroutine(buildProcess);
            }

            buildProcess = DialogueSystem.Instance.StartCoroutine(Construction());
        }

        public void Stop()
        {
            if (isConstructing)
            {
                DialogueSystem.Instance.StopCoroutine(buildProcess);
            }

            buildProcess = null;
        }

        public void Terminate()
        {
            _activeArchitects.Remove(tmpro);
            if (isConstructing)
            {
                DialogueSystem.Instance.StopCoroutine(buildProcess);
            }

            buildProcess = null;
        }

        public void ForceFinish()
        {
            tmpro.maxVisibleCharacters = tmpro.text.Length;
            Terminate();
        }

        private IEnumerator Construction()
        {
            int runsThisFrame = 0;

            tmpro.text = "";
            tmpro.text += _preText;

            tmpro.ForceMeshUpdate();
            TMP_TextInfo inf = tmpro.textInfo;
            int vis = inf.characterCount;

            tmpro.text += _targetText;

            tmpro.ForceMeshUpdate();
            inf = tmpro.textInfo;
            int max = inf.characterCount;

            tmpro.maxVisibleCharacters = vis;

            while (vis < max)
            {
                if (Skip)
                {
                    speed = 1;
                    charactersPerFrame = charactersPerFrame < 5 ? 5 : charactersPerFrame + 3;
                }

                while (runsThisFrame < charactersPerFrame)
                {
                    vis++;
                    tmpro.maxVisibleCharacters = vis;
                    runsThisFrame++;
                }

                runsThisFrame = 0;
                yield return new WaitForSeconds(0.01f * speed);
            }

            Terminate();
        }

        private void Initiate()
        {
            TextArchitect existingArchitect;
            if (_activeArchitects.TryGetValue(tmpro, out existingArchitect))
                existingArchitect.Terminate();

            buildProcess = DialogueSystem.Instance.StartCoroutine(Construction());
            _activeArchitects.Add(tmpro, this);
        }
    }
}