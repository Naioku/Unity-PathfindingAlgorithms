﻿using TMPro;
using UnityEngine;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSettingGroupPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private RectTransform inputEntries;

        private StaticTextManager staticTextManager;
        private Enums.SettingGroupPanelStaticKey displayedNameStaticKey;
            
        public RectTransform UIParent => inputEntries;
        
        private string DisplayedName => staticTextManager.GetValue(displayedNameStaticKey);

        private void Awake() => staticTextManager = AllManagers.Instance.StaticTextManager;

        public void Initialize(Enums.SettingGroupPanelStaticKey displayedName)
        {
            displayedNameStaticKey = displayedName;
            name = DisplayedName;
            label.text = DisplayedName;
        }
    }
}