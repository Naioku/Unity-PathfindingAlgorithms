using System;
using System.Collections.Generic;
using Settings;
using SpawningSystem;
using UI.Localization;
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
        [SerializeField] private IconCollection<Sprite> spriteCollection;
        [SerializeField] private IconCollection<Texture2D> textureCollection;
        
        private readonly Dictionary<Type, Enums.UIPopupType> inputPopupsLookup = new()
        {
            { typeof(int), Enums.UIPopupType.InputInt },
            { typeof(float), Enums.UIPopupType.InputFloat },
            { typeof(Color), Enums.UIPopupType.InputColor },
            { typeof(PermittedDirection[]), Enums.UIPopupType.InputPermittedDirections },
            { typeof(Language), Enums.UIPopupType.InputLanguages }
        };
        
        private PopupPanel currentPopupPanel;
        private GameObject lastStaticPanelGameObject;
        private LocalizedContentCache localizedContentCache;

        public SpawnManager<Enums.UISpawned> UISpawner => uiSpawner;
        public bool IsHoveringUI => EventSystem.current.IsPointerOverGameObject();
        public Sprite GetSprite(Enums.Icon value) => spriteCollection.Get(value);
        public Texture2D GetTexture(Enums.Icon value) => textureCollection.Get(value);
        
        public void Awake()
        {
            uiSpawner.Awake();
            uiPopupSpawner.Awake();
            localizedContentCache = new LocalizedContentCache
            (
                Enums.PopupText.ConfirmationButtonYes,
                Enums.PopupText.ConfirmationButtonNo
            );
            spriteCollection.Initialize();
            textureCollection.Initialize();
        }

        public void OpenPopupInfo(string header, string message)
        {
            InfoPanel infoPanel = uiPopupSpawner.CreateObject<InfoPanel>(Enums.UIPopupType.Info);
            infoPanel.Initialize(header, CloseCurrentPopup, message);
            OpenPopup(infoPanel);
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
            inputPanel.Initialize
            (
                header,
                CloseCurrentPopup,
                initialValue,
                result =>
                {
                    onClose.Invoke(result);
                    CloseCurrentPopup();
                }
            );
            OpenPopup(inputPanel);
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
            inputPanel.Initialize
            (
                header,
                CloseCurrentPopup,
                initialValue,
                minValue, maxValue,
                result =>
                {
                    onClose.Invoke(result);
                    CloseCurrentPopup();
                }
            );
            OpenPopup(inputPanel);
        }
        
        public void OpenPopupConfirmation(string header, string message, Action onConfirm)
        {
            ConfirmationPanel infoPanel = uiPopupSpawner.CreateObject<ConfirmationPanel>(Enums.UIPopupType.Confirmation);
            infoPanel.Initialize
            (
                header,
                CloseCurrentPopup,
                localizedContentCache,
                message,
                () =>
                {
                    onConfirm.Invoke();
                    CloseCurrentPopup();
                }
            );
            OpenPopup(infoPanel);
        }

        private void OpenPopup(PopupPanel createdPanel)
        {
            AllManagers.Instance.InputManager.EnablePopupMode();
            currentPopupPanel = createdPanel;
            lastStaticPanelGameObject = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(currentPopupPanel.SelectableOnOpen);
        }

        private void CloseCurrentPopup()
        {
            Object.Destroy(currentPopupPanel.gameObject);
            AllManagers.Instance.InputManager.DisablePopupMode();
            EventSystem.current.SetSelectedGameObject(lastStaticPanelGameObject);
        }
        
        
        [Serializable]
        private class IconCollection<T>
        {
            [SerializeField] private IconData[] data;

            private Dictionary<Enums.Icon, T> lookup = new();

            public void Initialize()
            {
                foreach (IconData iconData in data)
                {
                    lookup.Add(iconData.Name, iconData.Icon);
                }
            }

            public T Get(Enums.Icon name) => lookup[name];
            
            [Serializable]
            private struct IconData
            {
                public Enums.Icon Name;
                public T Icon;
            }
        }
    }
}