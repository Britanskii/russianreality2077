using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public static List<Song> allSongs = new();
        public static Song activeSong = null;

        public float songTransitionSpeed = 2f;
        public bool songSmoothTransitions = true;

        public void PlaySfx(AudioClip effect, float volume = 1f, float pitch = 1f)
        {
            AudioSource source = CreateNewSource($"SFX [{effect.name}]");
            source.clip = effect;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            
            Destroy(source.gameObject, effect.length);
        }

        public void PlaySong(AudioClip song, float maxVolume = 1f, float pitch = 1f, float startingVolume = 0f, bool playOnStart = true, bool loop = true)
        {
            if (song != null)
            {
                for (int i = 0; i < allSongs.Count; i++)
                {
                    Song currentSong = allSongs[i];
                    if (currentSong.clip == song)
                    {
                        activeSong = currentSong;
                        break;
                    }
                }

                if (activeSong == null || activeSong.clip != song)
                {
                    activeSong = new Song(song, maxVolume, pitch, startingVolume, playOnStart, loop);
                }
                
                StopAllCoroutines();
                StartCoroutine(VolumeLeveling());
            }
            else
            {
                activeSong = null;
            }
        }

        private bool TransitionSongs()
        {
            bool anyValueChanged = false;

            float speed = songTransitionSpeed * Time.deltaTime;

            for (int i = allSongs.Count - 1; i >= 0; i--)
            {
                Song song = allSongs[i];

                if (song == activeSong)
                {
                    if (song.volume < song.maxVolume)
                    {
                        song.volume = 
                            songSmoothTransitions
                                ? Mathf.Lerp(song.volume, song.maxVolume, speed)
                                : Mathf.MoveTowards(song.volume, song.maxVolume, speed);
                        anyValueChanged = true;
                    }
                }
                else
                {
                    if (song.volume > 0)
                    {
                        song.volume = 
                            songSmoothTransitions
                                ? Mathf.Lerp(song.volume, 0f, speed)
                                : Mathf.MoveTowards(song.volume, 0f, speed);
                        anyValueChanged = true;
                    }
                    else
                    {
                        allSongs.RemoveAt(i);
                        song.DestroySong();
                    }
                }
            }

            return anyValueChanged;
        }
        
        IEnumerator VolumeLeveling()
        {
            while (TransitionSongs())
            {
                yield return new WaitForEndOfFrame();
            }
        }
        
        [Serializable]
        public class Song
        {
            public AudioSource source;
            public float maxVolume;
            public AudioClip clip => source.clip;

            public Song(AudioClip clip, float maxVolume, float pitch, float startingVolume, bool playOnStart,  bool loop)
            {
                source = CreateNewSource($"SONG [{clip.name}]");
                source.clip = clip;
                source.volume = startingVolume;
                this.maxVolume = maxVolume;
                source.pitch = pitch;
                source.loop = loop;

                allSongs.Add(this);
                
                if (playOnStart)
                {
                    source.Play();
                }

            }

            public float volume
            {
                get => source.volume;
                set => source.volume = value;
            }
            
            public float pitch
            {
                get => source.pitch;
                set => source.pitch = value;
            }

            public void Play()
            {
                source.Play();
            }
            
            public void Stop()
            {
                source.Stop();
            }
            
            public void Pause()
            {
                source.Pause();
            }
            
            public void UnPause()
            {
                source.UnPause();
            }

            public void DestroySong()
            {
                allSongs.Remove(this);
                DestroyImmediate(source.gameObject);
            }
        }

        private static AudioSource CreateNewSource(string name)
        {
            AudioSource newSource = new GameObject(name).AddComponent<AudioSource>();
            newSource.transform.SetParent(Instance.transform);
            
            return newSource;
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}