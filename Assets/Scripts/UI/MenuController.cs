using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels;
using UI.MenuPanels.Settings;
using UnityEngine;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;
        [SerializeField] private SettingsPanel settingsPanel;
        // [SerializeField] private HelpPanel helpPanel;

        private BasePanel currentPanel;
        private readonly Stack<BasePanel> openedPanelsHistory = new Stack<BasePanel>();
        private Action onExit;

        public void Initialize(
            Action mazeModificationAction,
            Action bfsAction,
            Action aStarAction,
            Action resetToDefaultAction,
            Action<GameSettings, Enums.SettingsReloadingParam> saveSettingsAction,
            Action onExit)
        {
            InitializePanels(mazeModificationAction, bfsAction, aStarAction, resetToDefaultAction, saveSettingsAction);
            this.onExit = onExit;
            mainPanel.gameObject.SetActive(false);
            settingsPanel.gameObject.SetActive(false);
            SwitchPanel(mainPanel);
        }

        private void InitializePanels(
            Action mazeModificationAction,
            Action bfsAction,
            Action aStarAction,
            Action resetToDefaultAction,
            Action<GameSettings, Enums.SettingsReloadingParam> saveSettingsAction)
        {
            mainPanel.Initialize
            (
                Back,
                new Dictionary<Enums.MainMenuPanelButtonTag, Action>
                {
                    { Enums.MainMenuPanelButtonTag.MazeModification, mazeModificationAction },
                    { Enums.MainMenuPanelButtonTag.BFS, bfsAction },
                    { Enums.MainMenuPanelButtonTag.AStar, aStarAction },
                    { Enums.MainMenuPanelButtonTag.Settings, OpenSettingsPanel },
                    { Enums.MainMenuPanelButtonTag.Help, OpenHelpPanel }
                }
            );
            settingsPanel.Initialize(Back, resetToDefaultAction, saveSettingsAction);
            // helpPanel.Initialize(initData);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Back();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            OpenPanel(null);
        }

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

        private void OpenHelpPanel()
        {
            // OpenPanel(helpPanel);
            Debug.Log("Opening Help panel...");
        }

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