using System;
using SpawningSystem;
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
        public HUDControllerMazeModification HUDControllerMazeModification { get; private set; }
        public HUDControllerAlgorithm HUDControllerAlgorithm { get; private set; }
        public UIStaticPanel CurrentStaticPanel { set; private get; }
        
        private PopupPanel currentPopupPanel;

        public void Initialize() => CreatePanels();

        public void CreatePanels()
        {
            SpawnManager<Enums.SpawnedUI> uiSpawner = AllManagers.Instance.UISpawner;
            MenuController = uiSpawner.CreateObject<MenuController>(Enums.SpawnedUI.Menu);
            HUDControllerMazeModification = uiSpawner.CreateObject<HUDControllerMazeModification>(Enums.SpawnedUI.HUDMazeModification);
            HUDControllerAlgorithm = uiSpawner.CreateObject<HUDControllerAlgorithm>(Enums.SpawnedUI.HUDAlgorithm);
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