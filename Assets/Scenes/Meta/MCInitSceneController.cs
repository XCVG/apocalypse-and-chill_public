using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonCore;
using CommonCore.Config;

namespace MyConfession
{

    public class MCInitSceneController : MonoBehaviour
    {

        [SerializeField]
        private Image LoadingImage = null;
        [SerializeField]
        private Sprite LoadingAlternateImage = null;
        [SerializeField]
        private Sprite LoadingDoneImage = null;
        [SerializeField]
        private float LoadingDotDelay = 0.5f;

        private int LoadingDots = 0;
        private float Elapsed = 0;

        void Update()
        {
            if (CCBase.Initialized)
            {
                //ConfigModule.Apply();
                LoadingImage.overrideSprite = LoadingDoneImage;
                SceneManager.LoadScene(CCBase.LoadSceneAfterInit);
            }
        }

        private void LateUpdate()
        {
            Elapsed += Time.deltaTime;

            if (Elapsed > LoadingDotDelay)
            {
                Elapsed = 0;

                if (LoadingImage.overrideSprite != null)
                    LoadingImage.overrideSprite = LoadingAlternateImage;
                else
                    LoadingImage.overrideSprite = null;
            }
        }

        public void OnButtonClick()
        {
            SceneManager.LoadScene(CCBase.LoadSceneAfterInit);
        }
    }
}