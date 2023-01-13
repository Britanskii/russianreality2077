using System;
using Core.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TEMP.Audio
{
    public class AudioTesitng : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private AudioClip[] _music;
        [SerializeField] private float _volume;
        [SerializeField] private float _pitch;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioManager.Instance.PlaySfx(_clips[Random.Range(0, _clips.Length)], _volume, _pitch);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                AudioManager.Instance.PlaySong(_music[Random.Range(0, _music.Length)]);
            }
        }
    }
}