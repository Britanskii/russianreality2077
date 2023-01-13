using System;
using Core.Text;
using TMPro;
using UnityEngine;

namespace TEMP.Architect
{
    public class ArchitectTesting : MonoBehaviour
    {
        [TextArea(5, 10)] public string say;
        public TMP_Text Text;
        public int charactersPerFrame = 1;
        public float speed = 1f;
        public bool useEncapsulation = true;
        
        private TextArchitect _architect;
        
        private void Start()
        {
            _architect = new TextArchitect(Text, say, "", charactersPerFrame, speed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _architect = new TextArchitect(Text, say, "", charactersPerFrame, speed);
            }
        }
    }
}