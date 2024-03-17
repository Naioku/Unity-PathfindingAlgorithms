using System;
using UI.HUDPanels;
using UI.PopupPanels;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    [Serializable]
    public class UIManager
    {
        [SerializeField] private MenuController menuPrefab;
        [SerializeField] private HUDControllerMazeModification hudPrefabMazeModification;
        [SerializeField] private HUDControllerAlgorithm hudPrefabAlgorithm;
        [SerializeField] private InfoPanel infoPanelPrefab;
        
        public MenuController MenuController { get; private set; }
        public HUDControllerMazeModification HudControllerMazeModification { get; private set; }
        public HUDControllerAlgorithm HudControllerAlgorithm { get; private set; }
        public UIStaticPanel CurrentStaticPanel { set; private get; }
        
        private PopupPanel currentPopupPanel;

        public void Initialize() => CreatePanels();

        public void CreatePanels()
        {
            MenuController = Object.Instantiate(menuPrefab);
            HudControllerMazeModification = Object.Instantiate(hudPrefabMazeModification);
            HudControllerAlgorithm = Object.Instantiate(hudPrefabAlgorithm);
        }
        
        public void OpenInfoPanel(string header, string info)
        {
            AllManagers.Instance.InputManager.DisableInput();
            InfoPanel infoPanel = Object.Instantiate(infoPanelPrefab);
            infoPanel.Initialize(header, info, CloseCurrentPopupPanel);
            currentPopupPanel = infoPanel;
        }

        private void CloseCurrentPopupPanel()
        {
            AllManagers.Instance.InputManager.EnableInput();
            Object.Destroy(currentPopupPanel.gameObject);
            CurrentStaticPanel.SelectDefaultButton();
        }
    }
}