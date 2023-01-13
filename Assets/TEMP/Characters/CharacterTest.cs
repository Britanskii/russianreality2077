using System;
using Scripts.Core;
using Scripts.Core.Character;
using TEMP.Stories;
using UnityEngine;
using UnityEngine.Serialization;

namespace TEMP.Characters
{
    public class CharacterTest : MonoBehaviour
    {
        [SerializeField] private Vector2 _moveTarget;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private bool _smooth;
        
        [SerializeField] private int _bodyIndex;
        [SerializeField] private int _expressionIndex = 0;
        [SerializeField] private float _transitionSpeed = 5f;
        [SerializeField] private bool _smoothTransition = false;
        
        private Character Kairi;
        private float amount = 5f;
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
                    Kairi.Say(speech[index]);
                    index++;
                }
                else
                {
                    DialogueSystem.Instance.Close();
                }
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Kairi.MoveTo(_moveTarget, _moveSpeed, _smooth);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Kairi.StopMoving(true);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Kairi.SetExpression(_expressionIndex);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                Kairi.SetBody(_bodyIndex);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                Kairi.TransitionBody(Kairi.GetSprite(_bodyIndex), _transitionSpeed, _smoothTransition);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Kairi.TransitionExpression(Kairi.GetSprite(_expressionIndex), _transitionSpeed, _smoothTransition);
            }
        }
    }
}