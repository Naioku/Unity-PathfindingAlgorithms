using System;
using System.Collections.Generic;
using SpawningSystem;
using UI.HUDPanels;
using UI.PopupPanels;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace UI
{
    [Serializable]
    public class UIManager
    {
        [SerializeField] private SpawnManager<Enums.UISpawned> uiSpawner;
        [SerializeField] private SpawnManager<Enums.UIPopupType> uiPopupSpawner;
        
        private PopupPanel currentPopupPanel;
        private readonly Dictionary<Type, Enums.UIPopupType> inputPopupsLookup = new()
        {
            { typeof(int), Enums.UIPopupType.InputInt },
            { typeof(float), Enums.UIPopupType.InputFloat },
            { typeof(Color), Enums.UIPopupType.InputColor },
            // { typeof(float), Enums.UIPopupType.InputChoice }
        };

        public SpawnManager<Enums.UISpawned> UISpawner => uiSpawner;
        public MenuController MenuController { get; private set; }
        public HUDControllerMazeModification HUDControllerMazeModification { get; private set; }
        public HUDControllerAlgorithm HUDControllerAlgorithm { get; private set; }

        private GameObject lastStaticPanelGameObject;
        
        public void Awake()
        {
            uiSpawner.Awake();
            uiPopupSpawner.Awake();
            CreatePanels();
        }

        public void CreatePanels()
        {
            MenuController = uiSpawner.CreateObject<MenuController>(Enums.UISpawned.Menu);
            HUDControllerMazeModification = uiSpawner.CreateObject<HUDControllerMazeModification>(Enums.UISpawned.HUDMazeModification);
            HUDControllerAlgorithm = uiSpawner.CreateObject<HUDControllerAlgorithm>(Enums.UISpawned.HUDAlgorithm);
        }
        
        public void OpenPopupInfo(string header, string message)
        {
            InfoPanel infoPanel = uiPopupSpawner.CreateObject<InfoPanel>(Enums.UIPopupType.Info);
            infoPanel.Initialize(header, CloseCurrentPopupPanel, message);
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

            InputPanel<TReturn> inputPanel = uiPopupSpawner.CreateObject<InputPanel<TReturn>>(inputPopupsLookup[typeof(TReturn)]);
            inputPanel.Initialize(header, CloseCurrentPopupPanel, initialValue, result =>
            {
                onClose.Invoke(result);
                CloseCurrentPopupPanel();
            });
            OpenPanel(inputPanel);
        }
        
        public void OpenPopupInput<TReturn>(
            string header,
            TReturn initialValue,
            Action<TReturn> onClose,
            TReturn minValue,
            TReturn maxValue)
        {
            if (!inputPopupsLookup.ContainsKey(typeof(TReturn)))
            {
                Debug.LogError("Input popup type not supported, yet.");
                return;
            }

            InputPanelLimited<TReturn> inputPanel = uiPopupSpawner.CreateObject<InputPanelLimited<TReturn>>(inputPopupsLookup[typeof(TReturn)]);
            inputPanel.Initialize(header, CloseCurrentPopupPanel, initialValue, minValue, maxValue, result =>
            {
                onClose.Invoke(result);
                CloseCurrentPopupPanel();
            });
            OpenPanel(inputPanel);
        }
        
        public void OpenPopupConfirmation(string header, string message, Action onConfirm)
        {
            ConfirmationPanel infoPanel = uiPopupSpawner.CreateObject<ConfirmationPanel>(Enums.UIPopupType.Confirmation);
            infoPanel.Initialize(header, CloseCurrentPopupPanel, message, () =>
            {
                onConfirm.Invoke();
                CloseCurrentPopupPanel();
            });
            OpenPanel(infoPanel);
        }

        private void OpenPanel(PopupPanel createdPanel)
        {
            AllManagers.Instance.InputManager.EnablePopupMode();
            currentPopupPanel = createdPanel;
            lastStaticPanelGameObject = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(currentPopupPanel.SelectableOnOpen);
        }

        private void CloseCurrentPopupPanel()
        {
            Object.Destroy(currentPopupPanel.gameObject);
            AllManagers.Instance.InputManager.DisablePopupMode();
            EventSystem.current.SetSelectedGameObject(lastStaticPanelGameObject);
        }
    }
}