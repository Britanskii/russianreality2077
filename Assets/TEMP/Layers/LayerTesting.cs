using Core.GroundLayers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace TEMP.Layers
{
    public class LayerTesting : MonoBehaviour
    {

        public Texture texture;
        public VideoClip clip;
        
        private Bcfc _controller;
        private Layer _layerBackground;
        private Layer _layerForeground;
        private VideoLayer _videoLayer;

        private void Start()
        {
            _controller = Bcfc.Instance;
            _videoLayer = _controller.cinematic;
            _layerBackground = _controller.background;
            _layerForeground = _controller.foreground;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.B))
            {
                _layerBackground.SetTexture(texture);
            }
            
            if (Input.GetKey(KeyCode.P))
            {
                _layerForeground.SetTexture(texture);
            }

            if (Input.GetKey(KeyCode.Z))
            {
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    _videoLayer.SetMovie(clip);
                }
            }
        }
    }
}