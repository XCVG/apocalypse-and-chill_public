using CommonCore;
using CommonCore.Config;
using CommonCore.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Anc
{
    public class AltMainMenu : MonoBehaviour
    {
        public Button ContinueGameButton;

        public Transform PanelContainer;
        public GameObject LoadGamePanel;
        public GameObject HelpPanel;
        public GameObject OptionsPanel;

        private void Start()
        {
            if(!string.IsNullOrEmpty(PersistState.Instance.LastCampaignIdentifier))
            {
                var save = GetLastSave(PersistState.Instance.LastCampaignIdentifier);

                if (save == null)
                {
                    ContinueGameButton.image.enabled = false;
                    ContinueGameButton.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                ContinueGameButton.image.enabled = false;
                ContinueGameButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            if(ConfigState.Instance.HasCustomFlag("ShowMainMenu"))
            {
                AncConsoleCommands.ShowMainMenu(); //gross hack
            }
        }

        public void OnClickLoad()
        {
            HideAllPanels();

            LoadGamePanel.gameObject.SetActive(true);
        }

        public void OnClickHelp()
        {
            HideAllPanels();

            HelpPanel.gameObject.SetActive(true);
        }

        public void OnClickOptions()
        {
            HideAllPanels();

            if (OptionsPanel != null)
            {
                OptionsPanel.SetActive(!OptionsPanel.activeSelf);
            }
            else
            {
                var prefab = CoreUtils.LoadResource<GameObject>("UI/AltOptionsPanel");
                var menu = Instantiate(prefab, PanelContainer.transform);
                menu.SetActive(true);
                OptionsPanel = menu;
            }
        }

        private void HideAllPanels()
        {
            foreach(Transform t in PanelContainer)
            {
                t.gameObject.SetActive(false);
            }
        }

        private string GetLastSave(string campaignId)
        {
            string savePath = CoreParams.SavePath;
            DirectoryInfo saveDInfo = new DirectoryInfo(savePath);
            FileInfo[] savesFInfo = saveDInfo.GetFiles().OrderBy(f => f.CreationTime).Reverse().ToArray();

            List<KeyValuePair<string, SaveMetadata>> savesWithMetadata = new List<KeyValuePair<string, SaveMetadata>>();
            foreach(var saveFI in savesFInfo)
            {
                try
                {
                    var metadata = SaveUtils.LoadSaveMetadata(saveFI.FullName);
                    if(metadata.CampaignIdentifier == campaignId)
                        savesWithMetadata.Add(new KeyValuePair<string, SaveMetadata>(saveFI.FullName, metadata));
                }
                catch(Exception e)
                {
                    Debug.LogError("Failed to read save! " + saveFI.ToString(), this);
                    Debug.LogException(e);
                }
            }

            return savesWithMetadata.FirstOrDefault().Key;
        }
    }
}


