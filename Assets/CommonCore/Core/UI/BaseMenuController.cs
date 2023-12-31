﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.UI
{
    /// <summary>
    /// Base class for menu controllers
    /// </summary>
    public abstract class BaseMenuController : MonoBehaviour
    {
        //theme support probably won't arrive until Citadel but we'll rough it in anyway
        public bool ApplyTheme = true;
        public string OverrideTheme;

        public virtual void Awake()
        {

        }

        public virtual void Start()
        {
            //do it here or elsewhere
            if(ApplyTheme && CoreParams.UIThemeMode == UIThemePolicy.Auto)
            {
                var uiModule = CCBase.GetModule<UIModule>();
                if (!string.IsNullOrEmpty(OverrideTheme))
                    uiModule.ApplyThemeRecurse(transform, uiModule.GetThemeByName(OverrideTheme));
                else
                    uiModule.ApplyThemeRecurse(transform);
            }
        }

        public virtual void Update()
        {

        }

        public virtual void OnDisable()
        {
            
        }

    }
}