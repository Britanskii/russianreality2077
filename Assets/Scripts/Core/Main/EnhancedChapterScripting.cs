using System.Collections;
using System.Collections.Generic;
using Core.Text;
using Scripts.Core.Character;
using UnityEngine;

namespace Core.Main
{
    public class EnhancedChapterScripting : MonoBehaviour
    {
        public static Line Interpret(string rawLine)
        {
            return new Line(rawLine);
        }

        public class Line
        {
            public string speaker = "";

            public List<Segment> segments = new();
            public List<string> actions = new();

            public Line(string rawLine)
            {
                string[] dialogueAndActions = rawLine.Split('"');
                char actionSplitter = ' ';
                string[] actionsArray = dialogueAndActions.Length == 3
                    ? dialogueAndActions[2].Split(actionSplitter)
                    : dialogueAndActions[0].Split(actionSplitter);

                if (dialogueAndActions.Length == 3)
                {
                    speaker = dialogueAndActions[0] == ""
                        ? NovelController.Instance.cashedLastSpeaker
                        : dialogueAndActions[0];
                    if (speaker[^1] == ' ')
                    {
                        speaker = speaker.Remove(speaker.Length - 1);
                    }

                    NovelController.Instance.cashedLastSpeaker = speaker;

                    SegmentDialogue(dialogueAndActions[1]);
                }

                for (int i = 0; i < actionsArray.Length; i++)
                {
                    actions.Add(actionsArray[i]);
                }
            }

            private void SegmentDialogue(string dialogue)
            {
                segments.Clear();
                string[] parts = dialogue.Split('{', '}');

                for (int i = 0; i < parts.Length; i++)
                {
                    Segment segment = new Segment();
                    bool isOdd = i % 2 != 0;

                    if (isOdd)
                    {
                        string[] commandData = parts[i].Split(' ');
                        switch (commandData[0])
                        {
                            case "c":
                                segment.trigger = Segment.Trigger.waitClickClear;
                                break;
                            case "a":
                                segment.trigger = Segment.Trigger.waitClick;
                                segment.pretext = segments.Count > 0 ? segments[^1].dialogue : "";
                                break;
                            case "w":
                                segment.trigger = Segment.Trigger.autoDelay;
                                segment.autoDelay = float.Parse(commandData[1]);
                                break;
                            case "wa":
                                segment.trigger = Segment.Trigger.autoDelay;
                                segment.pretext = segments.Count > 0 ? segments[^1].dialogue : "";
                                segment.autoDelay = float.Parse(commandData[1]);
                                break;
                        }

                        i++;
                    }

                    segment.dialogue = parts[i];
                    segment.line = this;

                    segments.Add(segment);
                }
            }

            public class Segment
            {
                public Line line;
                public string dialogue = "";
                public string pretext = "";

                public enum Trigger
                {
                    waitClick,
                    waitClickClear,
                    autoDelay,
                }

                public Trigger trigger = Trigger.waitClickClear;

                public float autoDelay = 0;

                public void Run()
                {
                    if (running != null)
                    {
                        NovelController.Instance.StopCoroutine(running);
                    }

                    running = NovelController.Instance.StartCoroutine(Running());
                }

                public bool isRunning => running != null;
                public TextArchitect Architect = null;
                private Coroutine running = null;

                public void ForceFinish()
                {
                    if (running != null)
                    {
                        NovelController.Instance.StopCoroutine(running);
                    }

                    running = null;

                    if (Architect != null)
                    {
                        Architect.ForceFinish();

                        string wholeDialogue = "";

                        string[] parts = dialogue.Split('[', ']');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            bool isOdd = i % 2 != 0;
                            if (isOdd)
                            {
                                string action = parts[i];
                                if (allCurrentlyExecutedEvents.Contains(action))
                                {
                                    allCurrentlyExecutedEvents.Remove(action);
                                }
                                else
                                {
                                    DialogueEvents.HandleEvent(action, this);
                                }

                                i++;
                            }

                            wholeDialogue += parts[i];
                        }

                        Architect.ShowText(wholeDialogue);
                    }
                }

                private List<string> allCurrentlyExecutedEvents = new();

                private IEnumerator Running()
                {
                    allCurrentlyExecutedEvents.Clear();
                    TagManager.Inject(ref dialogue);

                    string[] parts = dialogue.Split('[', ']');

                    for (int i = 0; i < parts.Length; i++)
                    {
                        bool isOdd = i % 2 != 0;
                        if (isOdd)
                        {
                            DialogueEvents.HandleEvent(parts[i], this);
                            allCurrentlyExecutedEvents.Add(parts[i]);
                            i++;
                        }

                        string targetDialogue = parts[i];

                        if (line.speaker != "narrator")
                        {
                            Character character = CharacterManager.Instance.GetCharacter(line.speaker);
                            character.Say(targetDialogue,  i > 0 || pretext != "");
                        }
                        else
                        {
                            DialogueSystem.Instance.Say(targetDialogue, line.speaker, i > 0 || pretext != "");
                        }

                        Architect = DialogueSystem.Instance.CurrentArchitect;

                        while (Architect.isConstructing)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }

                    running = null;
                }
            }
        }
    }
}