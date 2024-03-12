using System;
using UnityEngine;

namespace UI.MenuPanels
{
    [Serializable]
    public class MainPanel : BasePanel
    {
        [SerializeField] private Button mazeModificationButton;
        [SerializeField] private Button bfsButton;
        [SerializeField] private Button aStarButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button helpButton;

        protected override void Awake()
        {
            base.Awake();
            AddActions();
            mazeModificationButton.Select();
        }

        private void AddActions()
        {
            mazeModificationButton.OnPressAction += MazeModificationActionInternal;
            bfsButton.OnPressAction += BFSActionInternal;
            aStarButton.OnPressAction += AStarActionInternal;
            // settingsButton.OnPressAction += SettingsActionInternal;
            // helpButton.OnPressAction += HelpActionInternal;
        }

        private void MazeModificationActionInternal() => initData.mazeModificationAction.Invoke();
        private void BFSActionInternal() => initData.bfsAction.Invoke();
        private void AStarActionInternal() => initData.aStarAction.Invoke();
        // private void SettingsActionInternal() => initData.onOpenPanel.Invoke(initData.settingsPanel);
        // private void HelpActionInternal() => initData.onOpenPanel.Invoke(initData.helpPanel);
    }
}