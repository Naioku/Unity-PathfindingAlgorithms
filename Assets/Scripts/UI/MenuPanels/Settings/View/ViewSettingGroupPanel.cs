﻿using UI.Localization;
using UnityEngine;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSettingGroupPanel : MonoBehaviour
    {
        [SerializeField] private LocalizedTextMeshPro label;
        [SerializeField] private RectTransform inputEntries;

        public RectTransform UIParent => inputEntries;
        
        public void Initialize(Enums.SettingGroupPanelName displayedName)
        {
            name = displayedName.ToString();
            label.Initialize(displayedName);
        }
    }
}