using System.Collections;
using UnityEngine;

namespace Project
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        public AudioClip[] MenuMusic;
        public AudioClip[] BattleMusic;
        private AudioSource audioSource;
        private int menuMusicIndex = 0;
        private int battleMusicIndex = 0;

        private void Start()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                audioSource = GetComponent<AudioSource>();
                StartCoroutine(StartPlayingMenuMusicCoroutine());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.Play();
        }

        private IEnumerator StartPlayingMenuMusicCoroutine()
        {
            while (true)
            {
                AudioClip music = MenuMusic[menuMusicIndex];
                audioSource.clip = music;
                audioSource.Play();
                yield return new WaitForSeconds(music.length);
                menuMusicIndex++;
                if(menuMusicIndex >= MenuMusic.Length) menuMusicIndex = 0;
            }
        }

        private IEnumerator StartPlayingBattleMusicCoroutine()
        {
            while (true)
            {
                AudioClip music = BattleMusic[battleMusicIndex];
                audioSource.clip = music;
                audioSource.Play();
                yield return new WaitForSeconds(music.length);
                battleMusicIndex++;
                if (battleMusicIndex >= BattleMusic.Length) battleMusicIndex = 0;
            }
        }

        public void SetMuteState(bool mute)
        {
            audioSource.mute = mute;
        }

        public void StartPlayingMenuMusic()
        {
            StopAllCoroutines();
            StartCoroutine(StartPlayingMenuMusicCoroutine());
        }

        public void StartPlayingBattleMusic()
        {
            StopAllCoroutines();
            StartCoroutine(StartPlayingBattleMusicCoroutine());
        }
    }
}
