using System;
using TMPro;
using UI.Buttons;
using UnityEngine;

namespace UI.PopupPanels
{
    public class InfoPanel : PopupPanel
    {
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private Button confirmationButton;

        public void Initialize(string header, string info, Action onConfirm)
        {
            base.Initialize(header);
            infoLabel.text = info;
            confirmationButton.OnPressAction += onConfirm;
            confirmationButton.Select();
        }
    }
}