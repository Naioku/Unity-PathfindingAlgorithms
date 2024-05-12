using System;
using System.Collections.Generic;
using UI.MenuPanels;
using UI.MenuPanels.Settings;
using UnityEngine;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;
        [SerializeField] private SettingsPanel settingsPanel;

        private BasePanel currentPanel;
        private readonly Stack<BasePanel> openedPanelsHistory = new();
        private Action onExit;

        public void Initialize(
            Action mazeModificationAction,
            Action bfsAction,
            Action dfsAction,
            Action aStarAction,
            Action resetToDefaultAction,
            Action<Enums.SettingsReloadingParam> saveSettingsAction,
            Action onExit)
        {
            InitializePanels
            (
                mazeModificationAction,
                bfsAction,
                dfsAction,
                aStarAction,
                resetToDefaultAction,
                saveSettingsAction
            );
            this.onExit = onExit;
            mainPanel.gameObject.SetActive(false);
            settingsPanel.gameObject.SetActive(false);
            SwitchPanel(mainPanel);
        }

        private void InitializePanels(
            Action mazeModificationAction,
            Action bfsAction,
            Action dfsAction,
            Action aStarAction,
            Action resetToDefaultAction,
            Action<Enums.SettingsReloadingParam> saveSettingsAction)
        {
            mainPanel.Initialize
            (
                Back,
                new Dictionary<Enums.MainMenuPanelButtonTag, Action>
                {
                    { Enums.MainMenuPanelButtonTag.MazeModification, mazeModificationAction },
                    { Enums.MainMenuPanelButtonTag.BFS, bfsAction },
                    { Enums.MainMenuPanelButtonTag.DFS, dfsAction },
                    { Enums.MainMenuPanelButtonTag.AStar, aStarAction },
                    { Enums.MainMenuPanelButtonTag.Settings, OpenSettingsPanel },
                }
            );
            settingsPanel.Initialize(Back, resetToDefaultAction, saveSettingsAction);
            
            AddInput();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Back();
            AddInput();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            OpenPanel(null);
            RemoveInput();
        }

        private void AddInput() => AllManagers.Instance.InputManager.GlobalMap.OnBackData.Performed += Back;
        private void RemoveInput() => AllManagers.Instance.InputManager.GlobalMap.OnBackData.Performed -= Back;

        private void Back()
        {
            if (openedPanelsHistory.Count == 0)
            {
                onExit.Invoke();
                return;
            }
            
            BasePanel panel = openedPanelsHistory.Pop();
            SwitchPanel(panel);
        }

        private void OpenSettingsPanel() => OpenPanel(settingsPanel);

        private void OpenPanel(BasePanel panel)
        {
            openedPanelsHistory.Push(currentPanel);
            SwitchPanel(panel);
        }

        private void SwitchPanel(BasePanel panel)
        {
            if (currentPanel != null)
            {
                currentPanel.Hide();
            }
            
            currentPanel = panel;
            
            if (currentPanel != null)
            {
                currentPanel.Show();
            }
        }
    }
}