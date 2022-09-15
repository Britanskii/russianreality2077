using System;
using Scripts.Core;
using Scripts.Core.Character;
using TEMP.Stories;
using UnityEngine;

namespace TEMP.Characters
{
    public class CharacterTest : MonoBehaviour
    {
        private Character Kairi;
        private string[] speech =
        {
            "I love watch anime!",
            "And you...",
            "UwU"
        };
        private int index;

        private void Start()
        {
            Kairi = CharacterManager.Instance.GetCharacter("Kairi");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (index < speech.Length)
                {
                    DialogueMover.Instance.Say(speech[index], Kairi.Name);
                    index++;
                }
                else
                {
                    DialogueSystem.Instance.Close();
                }
            }
        }
    }
}