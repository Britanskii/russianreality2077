using UnityEngine;

namespace Core.Text
{
    public class TagManager : MonoBehaviour
    {
        public static void Inject(ref string s)
        {
            if (!s.Contains("["))
            {
                return;
            }

            s = s.Replace("[Player]", "Britanskii");
            s = s.Replace("[holyRelic]", "Divine Arcane");
        }
        
        public static string[] SplitByTags(string targetText)
        {
            return targetText.Split('<', '>');
        }
    }
}