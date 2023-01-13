using TEMP.Stories;
using UnityEngine;

namespace Scripts.Core
{
    public class DialogueMover : MonoBehaviour
    {
        public static DialogueMover Instance;

        [SerializeField] private Story _story;

        private DialogueSystem _dialogueSystem;
        private int _plotIndex;

        private void Start()
        {
            _dialogueSystem = DialogueSystem.Instance;
            Say(_story.Plot[_plotIndex++]);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_dialogueSystem.isSpeaking || _dialogueSystem.isWaitingForUserInput)
                {
                    if (_plotIndex >= _story.Plot.Length)
                    {
                        return;
                    }

                    Say(_story.Plot[_plotIndex++]);
                }
            }
        }

        public void Say(string speech)
        {
            string[] parts = speech.Split(":");
            string currentSpeech = parts[0];
            string speaker = (parts.Length >= 2) ? parts[1] : "";

            _dialogueSystem.Say(currentSpeech, speaker);
        }

        public void Say(string speech, string speaker)
        {
            _dialogueSystem.Say(speech, speaker);
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}