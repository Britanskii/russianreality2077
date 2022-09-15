using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Character
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance;

        public RectTransform characterPanel;
        public List<Character> characters = new();
        public Dictionary<string, int> characterDictionary = new();

        public Character GetCharacter(string name, bool createCharacterIfDoesNotExist = true, bool enableCreatedCharacterOnStart = true)
        {
            int index;
            if (characterDictionary.TryGetValue(name, out index))
            {
                return characters[index];
            }
            else if (createCharacterIfDoesNotExist)
            {
                return CreateCharacter(name, enableCreatedCharacterOnStart);
            }

            return null;
        }

        private Character CreateCharacter(string name, bool enableCreatedCharacterOnStart)
        {
            Character newCharacter = new Character(name, enableCreatedCharacterOnStart);
            characterDictionary.Add(name, characters.Count);
            characters.Add(newCharacter);

            return newCharacter;
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}