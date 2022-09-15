using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Core.Character
{
    [System.Serializable]
    public class Character 
    {
        public bool Enabled
        {
            get => root.gameObject.activeInHierarchy;
            set => root.gameObject.SetActive(value);
        }
        public Renderers renderers = new();
        public string Name => _name;

        [SerializeField] private string _name;
        
        private RectTransform root;
        private DialogueSystem _dialogueSystem;
        
        public Character(string name, bool enableOnStart = true)
        {
            CharacterManager cm = CharacterManager.Instance;
            GameObject prefab = Resources.Load($"Characters/Character[{name}]") as GameObject;
            GameObject gameObject = GameObject.Instantiate(prefab, cm.characterPanel);

            root = gameObject.GetComponent<RectTransform>();
            _name = name;
            renderers.bodyRenderer = gameObject.transform.Find("BodyLayer").GetComponent<Image>();
            renderers.expressRenderer = gameObject.transform.Find("ExpressionLayer").GetComponent<Image>();
            
            _dialogueSystem = DialogueSystem.Instance;

            Enabled = enableOnStart;
        }

        public void Say(string speech)
        {
            _dialogueSystem.Say(_name, speech);
        }

        [System.Serializable]
        public class Renderers
        {
            public Image bodyRenderer;
            public Image expressRenderer;
        }
    }
}