using System;
using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class InfoPanel : PopupPanel
    {
        [SerializeField] private TextMeshProUGUI infoLabel;

        private Action onConfirm;

        public void Initialize(string header, Action onCloseAndConfirm, string info)
        {
            base.Initialize(header, onCloseAndConfirm);
            infoLabel.text = info;
            onConfirm = onCloseAndConfirm;
        }

        protected override void Confirm() => onConfirm.Invoke();
    }
}