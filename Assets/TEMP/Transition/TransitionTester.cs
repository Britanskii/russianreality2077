using System;
using Core.Transitions;
using UnityEngine;

namespace TEMP.Transition
{
    public class TransitionTester : MonoBehaviour
    {
        [SerializeField] private Texture2D _texture1;
        [SerializeField] private Texture2D _texture2;
        [SerializeField] private Texture2D _texture3;
        [SerializeField] private Texture2D _transition1;
        [SerializeField] private Texture2D _transition2;
        [SerializeField] private Texture2D _transition3;

        private int _progress = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _progress = Mathf.Clamp(_progress - 1, 0, 10);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _progress = Mathf.Clamp(_progress + 1, 0, 10);
                }
                
                switch (_progress)
                {
                    case 0:
                        TransitionManager.ShowScene(false);
                        break;
                    case 1:
                        TransitionManager.ShowScene(true);
                        break;
                    case 2:
                        TransitionManager.TransitionLayer(Bcfc.Instance.background, _texture1, _transition1);
                        break;
                    case 3:
                        TransitionManager.TransitionLayer(Bcfc.Instance.background, _texture2, _transition2);
                        break;
                    case 4:
                        TransitionManager.TransitionLayer(Bcfc.Instance.background, _texture3, _transition3);
                        break;
                    case 5:
                        Bcfc.Instance.background.SetTexture(_texture1);
                        break;
                    case 6:
                        TransitionManager.TransitionLayer(Bcfc.Instance.background, _texture3, _transition3);
                        break;
                    case 7:
                        Bcfc.Instance.background.SetTexture(_texture2);
                        break;
                    case 8:
                        TransitionManager.TransitionLayer(Bcfc.Instance.background, null, _transition1);
                        break;
                    case 9:
                        Bcfc.Instance.background.SetTexture(_texture3);
                        TransitionManager.ShowScene(true);
                        break;
                    case 10: 
                        TransitionManager.ShowScene(false);
                        break;
                }
            }
        }
    }
}