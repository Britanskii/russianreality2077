using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.GroundLayers
{
    public class Layer : MonoBehaviour
    {
        public GameObject root;
        public GameObject newImageObjectReference;
        public Coroutine specicalTransitionCoroutine;
        public RawImage ActiveImage
        {
            get => _activeImage;
            set => _activeImage = value;
        }

        public List<RawImage> AllImages => _allImages;
        
        [SerializeField] private VideoLayer cinematic;
        [SerializeField] private Layer[] _layers;

        private RawImage _activeImage;
        private List<RawImage> _allImages = new();

        public void SetTexture(Texture texture)
        {
            DisableAllLayers();

            if (texture != null)
            {
                if (_activeImage == null)
                {
                    CreateNewActiveImage();
                }

                _activeImage.texture = texture;
                _activeImage.color = GlobalFunctions.GlobalFunctions.SetAlpha(_activeImage.color, 1f);
            }
            else
            {
                DisableLayer();
            }
        }
        
        private void DisableLayer()
        {
            if (_activeImage != null)
            {
                _allImages.Remove(_activeImage);
                DestroyImmediate(_activeImage.gameObject);
                _activeImage = null;
            }
        }

        private void DisableAllLayers()
        {
            cinematic.StopMovie();

            foreach (Layer layer in _layers)
            {
                layer.DisableLayer();
            }
        }

        private void CreateNewActiveImage()
        {
            GameObject ob = Instantiate(newImageObjectReference, root.transform);
            ob.SetActive(true);
            RawImage raw = ob.GetComponent<RawImage>();
            _activeImage = raw;
            _allImages.Add(raw);
        }
        
        private void OnEnable()
        {
            cinematic.PlayVideo += DisableLayer;
        }

        private void OnDisable()
        {
            cinematic.PlayVideo -= DisableLayer;
        }
    }
}