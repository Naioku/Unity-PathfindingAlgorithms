﻿using System;
using System.Collections.Generic;
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
        private SpawnManager<Enums.UIPopupType> uiPopupSpawner;
        private PopupPanel currentPopupPanel;
        private readonly Dictionary<Type, Enums.UIPopupType> inputPopupsLookup = new Dictionary<Type, Enums.UIPopupType>
        {
            // { typeof(int), Enums.UIPopupType.InputInt },
            { typeof(float), Enums.UIPopupType.InputFloat },
            { typeof(Color), Enums.UIPopupType.InputColor },
            // { typeof(float), Enums.UIPopupType.InputChoice }
        };

        public MenuController MenuController { get; private set; }
        public HUDControllerMazeModification HUDControllerMazeModification { get; private set; }
        public HUDControllerAlgorithm HUDControllerAlgorithm { get; private set; }
        public UIStaticPanel CurrentStaticPanel { set; private get; }
        
        public void Awake()
        {
            uiPopupSpawner = AllManagers.Instance.UIPopupSpawner;
            CreatePanels();
        }

        public void CreatePanels()
        {
            SpawnManager<Enums.UISpawned> uiSpawner = AllManagers.Instance.UISpawner;
            MenuController = uiSpawner.CreateObject<MenuController>(Enums.UISpawned.Menu);
            HUDControllerMazeModification = uiSpawner.CreateObject<HUDControllerMazeModification>(Enums.UISpawned.HUDMazeModification);
            HUDControllerAlgorithm = uiSpawner.CreateObject<HUDControllerAlgorithm>(Enums.UISpawned.HUDAlgorithm);
        }
        
        public void OpenPopupInfo(string header, string info)
        {
            InfoPanel infoPanel = uiPopupSpawner.CreateObject<InfoPanel>(Enums.UIPopupType.Info);
            infoPanel.Initialize(header, info, CloseCurrentPopupPanel);
            OpenPanel(infoPanel);
        }

        public void OpenPopupInput<TReturn>(
            string header,
            TReturn initialValue,
            Action<TReturn> onClose)
        {
            if (!inputPopupsLookup.ContainsKey(typeof(TReturn)))
            {
                Debug.LogError("Input popup type not supported, yet.");
                return;
            }
            
            InputPanel<TReturn> infoPanel = uiPopupSpawner.CreateObject<InputPanel<TReturn>>(inputPopupsLookup[typeof(TReturn)]);
            infoPanel.Initialize(header, initialValue, result =>
            {
                onClose.Invoke(result);
                CloseCurrentPopupPanel();
            });
            OpenPanel(infoPanel);
        }

        private void OpenPanel(PopupPanel createdPanel)
        {
            AllManagers.Instance.InputManager.DisableInput();
            currentPopupPanel = createdPanel;
        }

        private void CloseCurrentPopupPanel()
        {
            AllManagers.Instance.InputManager.EnableInput();
            Object.Destroy(currentPopupPanel.gameObject);
            CurrentStaticPanel.SelectDefaultButton();
        }
    }
}