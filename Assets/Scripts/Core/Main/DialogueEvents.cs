using Unity.VisualScripting;
using UnityEngine;

namespace Core.Main
{
    public class DialogueEvents : MonoBehaviour
    {
        public static void HandleEvent(string _event, EnhancedChapterScripting.Line.Segment segment)
        {
            string[] eventData = _event.Split(' ');

            if (_event.Contains("("))
            {
                string[] actions = _event.Split(' ');
                foreach (string action in actions)
                {
                    if (action != "")
                    {
                        NovelController.Instance.HandleAction(action);
                    }
                }
                return;
            }
            
            switch (eventData[0])
            {
                case "textSpeed":
                    EVENT_TextSpeed(eventData[1], segment);
                    break;
                case "/textSpeed":
                    segment.Architect.speed = 1;
                    segment.Architect.charactersPerFrame = 1;
                    break;
            }
        }

        static void EVENT_TextSpeed(string data, EnhancedChapterScripting.Line.Segment segment)
        {
            string[] parts = data.Split(',');
            float delay = float.Parse(parts[0]);
            int charactersPerFrame = int.Parse(parts[1]);

            segment.Architect.speed = delay;
            segment.Architect.charactersPerFrame = charactersPerFrame;
        }
    }
}