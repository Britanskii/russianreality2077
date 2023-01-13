using System;
using Core.OverlaySystems;
using UnityEngine;



    public class InputScreenTesting : MonoBehaviour
    {
        public string displayTitle = "";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InputScreen.Show(displayTitle);
            }

            if (Input.GetKeyDown(KeyCode.Return) && InputScreen.isWaitingForUserInput)
            {
                InputScreen.Instance.Accept();
                print($"You entered the value of {InputScreen.currentInput}");
            }
        }
    } 
