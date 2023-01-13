using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Core.GlobalFunctions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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

        [System.Serializable]
        public class Renderers
        {
            public Image bodyRenderer;
            public Image expressRenderer;
            public List<Image> allBodyRenderers = new List<Image>();
            public List<Image> allExpressionRenderers = new List<Image>();
        }

        private RectTransform root;
        private DialogueSystem _dialogueSystem;
        private Vector2 anchorPadding => root.anchorMax - root.anchorMin;

        private Vector2 targetPosition;
        private Coroutine moving;
        private bool isMoving => moving != null;

        private Coroutine transitionBody = null;
        private bool isTransitionBody => transitionBody != null;
        private Coroutine transitionExpression = null;
        private bool isTransitionExpression => transitionExpression != null;
        private Sprite lastBodySprite;
        private Sprite lastFacialSprite;
        
        
        public Character(string name, bool enableOnStart = true)
        {
            CharacterManager cm = CharacterManager.Instance;
            GameObject prefab = Resources.Load($"Characters/Character[{name}]") as GameObject;
            GameObject gameObject = GameObject.Instantiate(prefab, cm.characterPanel);

            root = gameObject.GetComponent<RectTransform>();
            _name = name;
            renderers.bodyRenderer = gameObject.transform.Find("BodyLayer").GetComponentInChildren<Image>();
            renderers.expressRenderer = gameObject.transform.Find("ExpressionLayer").GetComponentInChildren<Image>();
            renderers.allBodyRenderers.Add(renderers.bodyRenderer);
            renderers.allExpressionRenderers.Add(renderers.expressRenderer);

            _dialogueSystem = DialogueSystem.Instance;

            Enabled = enableOnStart;
        }

        public void FadeIn(float speed = 3f, bool smooth = false)
        {
            Sprite alphaSprite = Resources.Load<Sprite>("Images/AlphaOnly");

            lastBodySprite = renderers.bodyRenderer.sprite;
            lastFacialSprite = renderers.expressRenderer.sprite;
            
            TransitionBody(alphaSprite, speed, smooth);
            TransitionExpression(alphaSprite, speed, smooth);
        }
        
        public void FadeOut(float speed = 3f, bool smooth = false)
        {
            if (lastBodySprite != null && lastFacialSprite != null)
            {
                TransitionBody(lastBodySprite, speed, smooth);
                TransitionExpression(lastFacialSprite, speed, smooth);
            } 
        }

        public void TransitionBody(Sprite sprite, float speed, bool smooth = true)
        {
            StopTransitionBody();
            transitionBody = CharacterManager.Instance.StartCoroutine(TransitioningBody(sprite, speed, smooth));
        }

        private void StopTransitionBody()
        {
            if (isTransitionBody)
            {
                CharacterManager.Instance.StopCoroutine(transitionBody);
            }

            transitionBody = null;
        }

        public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
        {
            foreach (Image image in renderers.allBodyRenderers)
            {
                if (image.sprite == sprite)
                {
                    renderers.bodyRenderer = image;
                    break;
                }
            }

            if (renderers.bodyRenderer.sprite != sprite)
            {
                Image image = GameObject
                    .Instantiate(renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent)
                    .GetComponent<Image>();
                renderers.allBodyRenderers.Add(image);
                renderers.bodyRenderer = image;
                image.color = GlobalFunctions.SetAlpha(image.color, 0f);
                image.sprite = sprite;
            }

            while (GlobalFunctions.TransitionImages(ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth))
            {
                yield return new WaitForEndOfFrame();
            }
            StopTransitionBody();
        }
        
        public void TransitionExpression(Sprite sprite, float speed, bool smooth = false)
        {
            StopTransitionExpression();
            transitionExpression = CharacterManager.Instance.StartCoroutine(TransitioningExpression(sprite, speed * 2, smooth));
        }

        private void StopTransitionExpression()
        {
            if (isTransitionExpression)
            {
                CharacterManager.Instance.StopCoroutine(transitionExpression);
            }

            transitionExpression = null;
        }

        public IEnumerator TransitioningExpression(Sprite sprite, float speed, bool smooth)
        {
            foreach (Image image in renderers.allExpressionRenderers)
            {
                if (image.sprite == sprite)
                {
                    renderers.expressRenderer = image;
                    break;
                }
            }

            if (renderers.expressRenderer.sprite != sprite)
            {
                Image image = GameObject
                    .Instantiate(renderers.expressRenderer.gameObject, renderers.expressRenderer.transform.parent)
                    .GetComponent<Image>();
                renderers.allExpressionRenderers.Add(image);
                renderers.expressRenderer = image;
                image.color = GlobalFunctions.SetAlpha(image.color, 0f);
                image.sprite = sprite;
            }

            while (GlobalFunctions.TransitionImages(ref renderers.expressRenderer, ref renderers.allExpressionRenderers, speed, smooth))
            {
                yield return new WaitForEndOfFrame();
            }
            StopTransitionExpression();
        }

        public Sprite GetSprite(int index = 0)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/" + _name);

            return sprites[index];
        }
        
        public Sprite GetSprite(string spriteName = "")
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/" + _name);

            foreach (Sprite sprite in sprites)
            {
                if (sprite.name == spriteName)
                {
                    return sprite;
                }
            }

            Debug.LogError("The sprite isn't exist");
            return null;
        }

        public void SetExpression(int index)
        {
            renderers.expressRenderer.sprite = GetSprite(index);
        }

        public void SetExpression(Sprite sprite)
        {
            renderers.expressRenderer.sprite = sprite;
        }
        
        public void SetExpression(string spriteName)
        {
            renderers.bodyRenderer.sprite = GetSprite(spriteName);
        }

        public void Flip()
        {
            root.localScale = new Vector3(root.localScale.x * -1, 1, 1);
        }
        
        public void FlipLeft()
        {
            root.localScale = Vector3.one;
        }
        
        public void FlipRight()
        {
            root.localScale = new Vector3(-1, 1, 1);
        }

        public void SetBody(int index)
        {
            renderers.bodyRenderer.sprite = GetSprite(index);
        }

        public void SetBody(Sprite sprite)
        {
            renderers.bodyRenderer.sprite = sprite;
        }
        
        public void SetBody(string spriteName)
        {
            renderers.bodyRenderer.sprite = GetSprite(spriteName);
        }

        public void Say(string speech, bool additive = false)
        {
            if (!Enabled)
            {
                Enabled = true;
            }

            _dialogueSystem.Say(speech, _name, additive);
        }

        public void MoveTo(Vector2 target, float speed, bool smooth = true)
        {
            StopMoving();
            moving = CharacterManager.Instance.StartCoroutine(Moving(target, speed, smooth));
        }

        public void StopMoving(bool arriveAtTargetPositionImmediately = false)
        {
            if (isMoving)
            {
                CharacterManager.Instance.StopCoroutine(moving);
                if (arriveAtTargetPositionImmediately)
                {
                    SetPosition(targetPosition);
                }
            }

            moving = null;
        }

        public void SetPosition(Vector2 target)
        {
            Vector2 padding = anchorPadding;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * target.x, maxY * target.y);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = root.anchorMin + padding;
        }

        private IEnumerator Moving(Vector2 target, float speed, bool smooth)
        {
            targetPosition = target;
            Vector2 padding = anchorPadding;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);

            speed *= Time.deltaTime * 2;
            while (root.anchorMin != minAnchorTarget)
            {
                root.anchorMin = (smooth)
                    ? Vector2.Lerp(root.anchorMin, minAnchorTarget, speed)
                    : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed);
                root.anchorMax = root.anchorMin + padding;
                yield return new WaitForEndOfFrame();
            }

            StopMoving();
        }
    }
}