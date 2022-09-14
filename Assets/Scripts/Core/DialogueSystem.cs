using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    public bool isSpeaking => speaking != null;
    public bool isWaitingForUserInput { get; private set; }
    
    [SerializeField] private GameObject _speechPanel;
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _speechText;
    [SerializeField] private float _delayInSeconds;
    
    private Coroutine speaking = null;

    public void Say(string speech, string speaker = "")
    {
        StopSpeaking();
        speaking = StartCoroutine(Speaking(speech, speaker));
    }
    
    private string DetermineSpeaker(string speaker)
    {
        string previousSpeaker = _speakerNameText.text;
        string currentSpeaker = previousSpeaker;
        if (speaker != previousSpeaker && speaker != "")
        {
            currentSpeaker = (speaker.ToLower().Contains("narrator")) ? "" : speaker;
        }

        return currentSpeaker;
    }

    private IEnumerator Speaking(string speech, string speaker = "")
    {
        _speechPanel.SetActive(true);
        _speechText.text = "";
        _speakerNameText.text = DetermineSpeaker(speaker);

        isWaitingForUserInput = false;
        while (_speechText.text != speech)
        {
            _speechText.text += speech[_speechText.text.Length];
            yield return new WaitForSeconds(_delayInSeconds);
        }

        isWaitingForUserInput = true;
        while (isWaitingForUserInput)
        {
            yield return new WaitForEndOfFrame();
        }
        
        StopSpeaking();
    }

    private void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }

    private void Awake()
    {
        instance = this;
    }
}