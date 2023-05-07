using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class SettingsManager : MonoBehaviour
    {
        const string SoundStateKey = "Sound";
        [SerializeField]
        private Sprite SoundOnImage;
        [SerializeField]
        private Sprite SoundOffImage;
        [SerializeField]
        private Button muteButton;
        private bool muted = false;
        void Start()
        {
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;
            if (PlayerPrefs.HasKey(SoundStateKey))
            {
                muted = Convert.ToBoolean(PlayerPrefs.GetInt(SoundStateKey));
                AudioManager.Instance.SetMuteState(muted);
            }
            UpdateMuteImage();
        }

        public void ChangeMuteState()
        {
            muted = !muted;
            PlayerPrefs.SetInt(SoundStateKey, Convert.ToInt32(muted));
            UpdateMuteImage();
            AudioManager.Instance.SetMuteState(muted);
        }

        private void UpdateMuteImage()
        {
            if (!muted)
            {
                muteButton.image.sprite = SoundOnImage;
            }
            else
            {
                muteButton.image.sprite = SoundOffImage;
            }
        }
    }
}
