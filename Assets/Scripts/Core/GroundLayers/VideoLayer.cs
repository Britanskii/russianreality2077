using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;


namespace Core.GroundLayers
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoLayer : MonoBehaviour
    {
        public Coroutine specicalTransitionCoroutine;
        public event UnityAction PlayVideo;
        public event UnityAction StopVideo;
        
        [SerializeField] private VideoPlayer movie = new();

        public void SetMovie(VideoClip clip, bool ifMovieThenLoop = true)
        {
            PlayVideo?.Invoke();
            
            movie.clip = clip;
            movie.isLooping = ifMovieThenLoop;
            movie.Play();
        }

        public void StopMovie()
        {
            StopVideo?.Invoke();
            
            movie.Stop();
        }
    }
}