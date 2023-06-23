using CommonCore;
using CommonCore.Config;
using CommonCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anc
{

    public class AltIngameMenuController : MonoBehaviour
    {
        public IngameMenuController MenuController;
        public GameObject OptionsMenu;

        public void HandleOptionsClicked()
        {
            if (OptionsMenu != null)
            {
                OptionsMenu.SetActive(!OptionsMenu.activeSelf);
            }
            else
            {
                var prefab = CoreUtils.LoadResource<GameObject>("UI/AltOptionsPanel");
                var menu = Instantiate(prefab, MenuController.MainPanel.transform);
                menu.SetActive(true);
                OptionsMenu = menu;
            }
        }

        private void OnDisable()
        {
            if (OptionsMenu != null)
                OptionsMenu.SetActive(false);
        }

        public void HandleExitClicked()
        {
            Time.timeScale = ConfigState.Instance.DefaultTimescale;
            SharedUtils.EndGame();
        }
    }
}