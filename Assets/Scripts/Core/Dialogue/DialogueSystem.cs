using System;
using System.Collections;
using System.Collections.Generic;
using Core.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;
    public bool isSpeaking => speaking != null;
    public bool isWaitingForUserInput { get; private set; }
    public TMP_Text speechText;
    public TextArchitect CurrentArchitect = null;
    public string TargetSpeech;

    [SerializeField] private GameObject speakerNamePanel;
    [SerializeField] private GameObject _speechSystem;
    [SerializeField] private GameObject _speechPanel;
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private float _delayInSeconds;

    private Coroutine speaking = null;

    public void Say(string speech, string speaker = "", bool additive = false)
    {
        StopSpeaking();

        speaking = StartCoroutine(Speaking(speech, additive, speaker));
    }

    public void Close()
    {
        StopSpeaking();
        _speechSystem.SetActive(false);
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

    private IEnumerator Speaking(string speech, bool additive, string speaker = "")
    {
        string additiveSpeech = additive ? speechText.text : "";

        TargetSpeech = speech;
        
        if (CurrentArchitect == null)
        {
            CurrentArchitect = new TextArchitect(speechText, speech, additiveSpeech);
        }
        else
        {
            CurrentArchitect.Renew(speech, additiveSpeech);
        }

        _speechPanel.SetActive(true);
        _speakerNameText.text = DetermineSpeaker(speaker);
        speakerNamePanel.SetActive(_speakerNameText.text != "");

        isWaitingForUserInput = false;
        while (CurrentArchitect.isConstructing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CurrentArchitect.Skip = true;
            }

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

        if (CurrentArchitect != null && CurrentArchitect.isConstructing)
        {
            CurrentArchitect.Stop();
        }

        speaking = null;
    }

    private void Awake()
    {
        Instance = this;
    }
}