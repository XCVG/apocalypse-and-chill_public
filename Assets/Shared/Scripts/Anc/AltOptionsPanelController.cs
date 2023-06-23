using CommonCore;
using CommonCore.Config;
using CommonCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vnx;

namespace Anc
{

    public class AltOptionsPanelController : MonoBehaviour
    {
        public GameObject ScreenOptionsSection;

        public Slider SizeSlider;
        public Text SizeText;

        public Toggle FullscreenToggle;

        public Slider MainVolumeSlider;
        public Slider MusicVolumeSlider;

        public Slider TypeSpeedSlider;
        public Text TypeSpeedText;
        public Toggle BeepToggle;
        public Toggle FadeToggle;

        private bool IgnoreValueChanges = false;

        private bool AllowResolutionSelection => !(CoreParams.Platform == RuntimePlatform.WebGLPlayer || SystemInfo.deviceType == DeviceType.Console);

        private void OnEnable()
        {
            PaintValues();
        }

        public void HandleCancelClicked()
        {
            gameObject.SetActive(false);
        }

        public void HandleAcceptClicked()
        {
            SaveValues();
            gameObject.SetActive(false);
        }

        private void PaintValues()
        {
            IgnoreValueChanges = true;

            var config = ConfigState.Instance;

            config.AddCustomVarIfNotExists("VnxConfig", () => new VnConfig());
            var vnConfig = config.CustomConfigVars["VnxConfig"] as VnConfig;

            if(AllowResolutionSelection)
            {
                if (config.FullScreen)
                {
                    FullscreenToggle.isOn = true;
                    SizeSlider.value = SizeSlider.maxValue;
                    SizeSlider.interactable = false;
                    SizeText.text = "";
                }
                else
                {
                    FullscreenToggle.isOn = false;
                    SizeSlider.interactable = true;
                    SizeSlider.value = GetSizeForResolution(config.Resolution);
                }
            }
            else
            {
                ScreenOptionsSection.SetActive(false);
            }            

            MainVolumeSlider.value = (float)Math.Round(Math.Sqrt(config.SoundVolume), 2);
            MusicVolumeSlider.value = (float)Math.Round(Math.Sqrt(config.MusicVolume), 2);

            BeepToggle.isOn = vnConfig.EnableAdvanceBeep;
            TypeSpeedSlider.value = vnConfig.TypeOnSpeed * 25f;
            FadeToggle.isOn = vnConfig.AllowFade;

            IgnoreValueChanges = false;
        }        

        private void SaveValues()
        {
            var config = ConfigState.Instance;

            if(AllowResolutionSelection)
            {
                //if fullscreen set, set resolution to native and enable fullscreen
                if (FullscreenToggle.isOn)
                {
                    config.FullScreen = true;
                    config.Resolution = new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);
                }
                else if (!FullscreenToggle.isOn)
                {
                    config.FullScreen = false;
                    config.Resolution = GetResolutionForSize(Mathf.RoundToInt(SizeSlider.value));
                }
            }            

            ConfigState.Instance.SoundVolume = (float)Math.Round(Math.Pow(MainVolumeSlider.value, 2), 2);
            ConfigState.Instance.MusicVolume = (float)Math.Round(Math.Pow(MusicVolumeSlider.value, 2), 2);

            config.AddCustomVarIfNotExists("VnxConfig", () => new VnConfig());
            var vnConfig = config.CustomConfigVars["VnxConfig"] as VnConfig;

            vnConfig.EnableAdvanceBeep = BeepToggle.isOn;
            vnConfig.TypeOnSpeed = CalculateTypeSpeed();
            vnConfig.AllowFade = FadeToggle.isOn;

            ConfigModule.Apply();
            ConfigState.Save();
        }

        private int GetSizeForResolution(Vector2Int resolution)
        {
            if(resolution.y < 720)
            {
                return 0;
            }
            else if(resolution.y < 1080)
            {
                return 1;
            }
            else if(resolution.y == 1080)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        private Vector2Int GetResolutionForSize(int size)
        {
            switch (size)
            {
                case 0:
                    return new Vector2Int(854, 480);
                case 1:
                    return new Vector2Int(1280, 720);
                case 2:
                    return new Vector2Int(1920, 1080);
                default:
                    return new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);
            }
        }

        public void HandleSizeChanged()
        {
            int value = Mathf.RoundToInt(SizeSlider.value);
            switch (value)
            {
                case 0:
                    SizeText.text = "WVGA";
                    break;
                case 1:
                    SizeText.text = "HD";
                    break;
                case 2:
                    SizeText.text = "Full HD";
                    break;
                case 3:
                    SizeText.text = "Native";
                    break;
            }
        }

        public void HandleTypeSpeedChanged()
        {
            float val = CalculateTypeSpeed();
            if (val > 0)
                TypeSpeedText.text = (int)(val * 100f) + "%";
            else
                TypeSpeedText.text = "Disabled";
        }

        private float CalculateTypeSpeed()
        {
            return TypeSpeedSlider.value / 25f;
        }

        
    }
}