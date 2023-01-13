using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.GroundLayers;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Transitions
{
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance;

        public RawImage overlayImage;
        public Material transitionMaterialPrefab;
        
        private static bool _sceneVisible = true;

        public static void TransitionLayer(Layer layer, Texture2D targetImage, Texture2D transitionEffect,
            float speed = 1, bool smooth = false)
        {
            if (_transitioningOverlay != null)
            {
                Instance.StopCoroutine(_transitioningOverlay);
            }

            if (targetImage != null)
            {
                layer.specicalTransitionCoroutine =
                    Instance.StartCoroutine(TransitioningLayer(layer, targetImage, transitionEffect, speed, smooth));
            }
            else
            {
                layer.specicalTransitionCoroutine =
                    Instance.StartCoroutine(TransitioningLayerToNull(layer, targetImage, transitionEffect, speed, smooth));
            }
        }
        
        public static void ShowScene(bool show, float speed = 1f, bool smooth = false,
            Texture2D transitionEffect = null)
        {
            if (_transitioningOverlay != null)
            {
                Instance.StopCoroutine(_transitioningOverlay);
            }

            _sceneVisible = show;

            if (transitionEffect != null)
            {
                Instance.overlayImage.material.SetTexture(AlphaTex, transitionEffect);
            }
            
            _transitioningOverlay = Instance.StartCoroutine(TransitioningOverlay(show, speed, smooth));
        }

        private static Coroutine _transitioningOverlay = null;
        private static readonly int AlphaTex = Shader.PropertyToID("_AlphaTex");
        private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");

        private static IEnumerator TransitioningLayer(Layer layer, Texture2D targetImage, Texture2D transitionEffect,
            float speed = 1, bool smooth = false)
        {
            GameObject gameObject = Instantiate(layer.newImageObjectReference,
                layer.newImageObjectReference.transform.parent);
            gameObject.SetActive(true);

            RawImage rawImage = gameObject.GetComponent<RawImage>();
            rawImage.texture = targetImage;

            layer.ActiveImage = rawImage;
            layer.AllImages.Add(rawImage);

            rawImage.material = new Material(Instance.transitionMaterialPrefab);
            rawImage.material.SetTexture(AlphaTex, transitionEffect);
            rawImage.material.SetFloat(Cutoff, 1);
            float currentValue = 1;

            while (currentValue > 0)
            {
                currentValue = smooth
                    ? Mathf.Lerp(currentValue, 0, speed * Time.deltaTime)
                    : Mathf.MoveTowards(currentValue, 0, speed * Time.deltaTime);
                rawImage.material.SetFloat(Cutoff, currentValue);
                yield return new WaitForEndOfFrame();
            }

            if (rawImage != null)
            {
                rawImage.material = null;
                rawImage.color = GlobalFunctions.GlobalFunctions.SetAlpha(rawImage.color, 1);
            }

            for (int i = layer.AllImages.Count - 1; i >= 0; i--)
            {
                if (layer.AllImages[i] == layer.ActiveImage && layer.ActiveImage != null)
                {
                    continue;
                }

                if (layer.AllImages[i] != null)
                {
                    Destroy(layer.AllImages[i].gameObject, 0.01f);
                }
                
                layer.AllImages.RemoveAt(i);
            }

            layer.specicalTransitionCoroutine = null;
        }
        
        private static IEnumerator TransitioningLayerToNull(Layer layer, Texture2D targetImage, Texture2D transitionEffect,
            float speed = 1, bool smooth = false)
        {
            List<RawImage> currentImagesOnLayer = new();

            foreach (RawImage rawImage in layer.AllImages)
            {
                rawImage.material = new Material(Instance.transitionMaterialPrefab);
                rawImage.material.SetTexture(AlphaTex, transitionEffect);
                rawImage.material.SetFloat(Cutoff, 0);
                currentImagesOnLayer.Add(rawImage);
            }

            float currentValue = 0;
            while (currentValue < 1)
            {
                currentValue = smooth
                    ? Mathf.Lerp(currentValue, 1, speed * Time.deltaTime)
                    : Mathf.MoveTowards(currentValue, 1, speed * Time.deltaTime);
                for (int i = 0; i < layer.AllImages.Count; i++)
                {
                    layer.AllImages[i].material.SetFloat(Cutoff, currentValue);
                }

                yield return new WaitForEndOfFrame();
            }
            
            foreach (var rawImage in currentImagesOnLayer.Where(rawImage => rawImage.material != null))
            {
                Destroy(rawImage.gameObject, 0.01f);
            }
            
            layer.specicalTransitionCoroutine = null;
        }

        private static IEnumerator TransitioningOverlay(bool show, float speed, bool smooth)
        {
            float targetValue = show ? 1 : 0;
            float currentValue = Instance.overlayImage.material.GetFloat(Cutoff);

            while (currentValue != targetValue)
            {
                currentValue = smooth
                    ? Mathf.Lerp(currentValue, targetValue, speed * Time.deltaTime)
                    : Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);
                Instance.overlayImage.material.SetFloat(Cutoff, currentValue);
                yield return new WaitForEndOfFrame();
            }

            _transitioningOverlay = null;
        }
        
        private void Awake()
        {
            Instance = this;
            overlayImage.material = new Material(transitionMaterialPrefab);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShowScene(!_sceneVisible);
            }
        }
    }
}