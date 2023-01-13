using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Core.Audio;
using Core.GroundLayers;
using Core.Transitions;
using Scripts.Core.Character;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

namespace Core.Main
{
    public class NovelController : MonoBehaviour
    {
        public string cashedLastSpeaker = "";
        public static NovelController Instance;
        public bool isHandlingLine => handlingLine != null;

        private List<string> _data = new();

        public void LoadChapterFile(string fileName)
        {
            _data = FileManager.LoadFile($"{FileManager.savPath}Resources/Stories/{fileName}");
            cashedLastSpeaker = "";

            if (handlingChapterFile != null)
            {
                StopCoroutine(handlingChapterFile);
            }

            handlingChapterFile = StartCoroutine(HandlingChapterFile());
        }
        
        public void HandleAction(string action)
        {
            print($"Handle action [{action}]");
            string[] data = action.Split('(', ')');

            string command = data[0];
            string parameter = data[1];

            switch (command)
            {
                case "setBackground":
                    Command_SetLayer(parameter, Bcfc.Instance.background);
                    return;
                case "setCinematic":
                    Command_SetLayer(parameter, Bcfc.Instance.cinematic);
                    return;
                case "setForeground":
                    Command_SetLayer(parameter, Bcfc.Instance.foreground);
                    return;
                case "tBackground":
                    Command_TransitionBackground(parameter, Bcfc.Instance.background);
                    return;
                case "playSound":
                    Command_PlaySound(parameter);
                    return;
                case "playMusic":
                    Command_PlayMusic(parameter);
                    return;
                case "move":
                    Command_MoveCharacter(parameter);
                    return;
                case "setPosition":
                    Command_SetPosition(parameter);
                    return;
                case "setBody":
                    Command_SetBody(parameter);
                    return;
                case "setFace":
                    Command_SetFace(parameter);
                    return;
                case "flip":
                    Command_Flip(parameter);
                    return;
                case "flipLeft":
                    Command_FlipLeft(parameter);
                    return;
                case "flipRight":
                    Command_FlipRight(parameter);
                    return;
                default:
                    Debug.LogError($"A command {command} does not exist");
                    return;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            LoadChapterFile("chapter0_start");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Next();
            }
        }

        private void HandleLine(string rawLine)
        {
            EnhancedChapterScripting.Line line = EnhancedChapterScripting.Interpret(rawLine);
            StopHandlingLine();
            handlingLine = StartCoroutine(HandlingLine(line));
        }

        private void StopHandlingLine()
        {
            if (isHandlingLine)
            {
                StopCoroutine(handlingLine);
            }

            handlingLine = null;
        }
        
        private Coroutine handlingLine = null;
        private IEnumerator HandlingLine(EnhancedChapterScripting.Line line)
        {
            _next = false;
            int lineProgress = 0;
            
            foreach (string action in line.actions)
            {
                if (action != "")
                {
                    HandleAction(action);
                }
            }

            while (lineProgress < line.segments.Count)
            {
                _next = false;
                EnhancedChapterScripting.Line.Segment segment = line.segments[lineProgress];

                if (lineProgress > 0)
                {
                    if (segment.trigger == EnhancedChapterScripting.Line.Segment.Trigger.autoDelay)
                    {
                        for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime)
                        {
                            yield return new WaitForEndOfFrame();
                            if (_next)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        while (!_next)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                _next = false;
                segment.Run();

                while (segment.isRunning)
                {
                    yield return new WaitForEndOfFrame();

                    if (_next)
                    {
                        if (!segment.Architect.Skip)
                        {
                            segment.Architect.Skip = true;
                        }
                        else
                        {
                            segment.ForceFinish();
                        }
                    }
                }
                
                lineProgress++;
                
                yield return new WaitForEndOfFrame();
            }

            handlingLine = null;
        }

        public void Next()
        {
            Debug.Log("NEXT");
            _next = true;
        }
        private bool _next = false;

        private Coroutine handlingChapterFile = null;
        private IEnumerator HandlingChapterFile()
        {
            int progress = 0;

            while (progress < _data.Count)
            {
                if (_next)
                {
                    HandleLine(_data[progress++]);
                    while (isHandlingLine)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private void Command_Exit(string data)
        {
            string[] parameters = data.Split(",");
            string[] characters = parameters[0].Split(';');
            float speed = 3;
            bool smooth = false;
            foreach (string parameter in parameters)
            {
                if (float.TryParse(parameter, out float fVal))
                {
                    speed = fVal; 
                }

                if (bool.TryParse(parameter, out bool bVal))
                {
                    smooth = bVal;
                }
            }

            foreach (string name in characters)
            {
                Character character = CharacterManager.Instance.GetCharacter(name);
                character.FadeOut(speed, smooth);
            }
        }
        
        private void Command_Enter(string data)
        {
            string[] parameters = data.Split(",");
            string[] characters = parameters[0].Split(';');
            float speed = 3;
            bool smooth = false;
            foreach (string parameter in parameters)
            {
                if (float.TryParse(parameter, out float fVal))
                {
                    speed = fVal; 
                }

                if (bool.TryParse(parameter, out bool bVal))
                {
                    smooth = bVal;
                }
            }

            foreach (string name in characters)
            {
                Character character = CharacterManager.Instance.GetCharacter(name, true, false);
                if (!character.Enabled)
                {
                    character.renderers.bodyRenderer.color = new Color(1, 1, 1, 0);
                    character.renderers.expressRenderer.color = new Color(1, 1, 1, 0);
                    character.Enabled = true;
                    
                    character.TransitionBody(character.renderers.bodyRenderer.sprite, speed, smooth);
                    character.TransitionExpression(character.renderers.expressRenderer.sprite, speed, smooth);
                }
                else
                {
                    character.FadeIn(speed, smooth);
                }
            }
        }


        private void Command_SetLayer(string data, Layer layer)
        {
            string textureName = data.Contains(",") ? data.Split(",")[0] : data;

            Texture2D texture = data == "null" ? null : (Texture2D) Resources.Load($"UI/backdrops/still/{textureName}");

            float speed = 2f;
            bool smooth = false;

            if (data.Contains(","))
            {
                string[] parameters = data.Split(",");
                foreach (string parameter in parameters)
                {
                    if (float.TryParse(parameter, out float fVal))
                    {
                        speed = fVal;
                    }

                    if (bool.TryParse(parameter, out bool bVal))
                    {
                        smooth = bVal;
                    }
                }
            }

            layer.SetTexture(texture);
        }

        private void Command_Flip(string data)
        {
            Character character = CharacterManager.Instance.GetCharacter(data);
            character.Flip();
        }

        private void Command_FlipRight(string data)
        {
            Character character = CharacterManager.Instance.GetCharacter(data);
            character.FlipRight();
        }

        private void Command_FlipLeft(string data)
        {
            Character character = CharacterManager.Instance.GetCharacter(data);
            character.FlipLeft();
        }

        private void Command_SetLayer(string data, VideoLayer layer)
        {
            string videoClipName = data.Contains(",") ? data.Split(",")[0] : data;

            VideoClip videoClip = (VideoClip) Resources.Load($"UI/backdrops/animated/{videoClipName}");

            layer.SetMovie(videoClip);
        }

        private void Command_PlaySound(string data)
        {
            AudioClip audioClip = (AudioClip) Resources.Load($"Audio/SFX/{data}");

            if (audioClip != null)
            {
                AudioManager.Instance.PlaySfx(audioClip);
            }
            else
            {
                Debug.LogError("Clip does not exist");
            }
        }

        private void Command_PlayMusic(string data)
        {
            AudioClip audioClip = (AudioClip) Resources.Load($"Audio/Music/{data}");

            if (audioClip != null)
            {
                AudioManager.Instance.PlaySong(audioClip);
            }
            else
            {
                Debug.LogError("Song does not exist");
            }
        }

        private void Command_MoveCharacter(string data)
        {
            string[] parameters = data.Split(",");
            string name = parameters[0];
            float locationX = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            float locationY = parameters.Length >= 3 ? float.Parse(parameters[2]) : 0; 
            float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 1f;
            bool smooth = parameters.Length != 5 || bool.Parse(parameters[4]);

            Character character = CharacterManager.Instance.GetCharacter(name);
            character.MoveTo(new Vector2(locationX, locationY), speed, smooth);
        }
        
        private void Command_SetPosition(string data)
        {
            string[] parameters = data.Split(",");
            string name = parameters[0];
            float locationX = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            float locationY = parameters.Length >= 3 ? float.Parse(parameters[2]) : 0;

            Character character = CharacterManager.Instance.GetCharacter(name);
            character.SetPosition(new Vector2(locationX, locationY));
        }

        private void Command_SetFace(string data)
        {
            string[] parameters = data.Split(",");
            string name = parameters[0];
            string expression = parameters[1];
            float speed = parameters.Length == 4 ? float.Parse(parameters[3]) : 1f;

            Character character = CharacterManager.Instance.GetCharacter(name);
            Sprite sprite = character.GetSprite(expression);
            
            character.TransitionExpression(sprite, speed);
        }
        
        private void Command_SetBody(string data)
        {
            string[] parameters = data.Split(",");
            string name = parameters[0];
            string expression = parameters[1];
            float speed = parameters.Length == 4 ? float.Parse(parameters[3]) : 1f;

            Character character = CharacterManager.Instance.GetCharacter(name);
            Sprite sprite = character.GetSprite(expression);

            character.TransitionBody(sprite, speed);
        }

        private void Command_TransitionBackground(string data, Layer layer)
        {
            string[] parameters = data.Split(',');

            string texName = parameters[0];
            string transTexName = parameters[1];
            Texture2D texture = texName == "null" ? null : (Texture2D) Resources.Load($"UI/backdrops/still/{texName}");
            Texture2D transTexture = (Texture2D) Resources.Load($"Images/TransitionEffects/{transTexName}");

            float speed = 2f;
            bool smooth = false;

            for (int i = 2; i < parameters.Length; i++)
            {
                string parameter = parameters[i];
                float fVal = 0;
                bool bVal = false;
                if (float.TryParse(parameter, out fVal))
                {
                    speed = fVal;
                }
                if (bool.TryParse(parameter, out bVal))
                {
                    smooth = bVal;
                } 
            }
            
            TransitionManager.TransitionLayer(layer, texture, transTexture, speed, smooth);
        }
    }
}