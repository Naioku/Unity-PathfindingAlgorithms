using System;
using System.Collections.Generic;
using UI.MenuPanels;
using UnityEngine;

namespace UI
{
    public class UIMenuController : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;
        // [SerializeField] private SettingsPanel settingsPanel;
        // [SerializeField] private HelpPanel helpPanel;

        private BasePanel currentPanel;
        private readonly Stack<BasePanel> openedPanelsHistory = new Stack<BasePanel>();
        private Action onExit;

        public void Initialize(Action mazeModificationAction, Action bfsAction, Action aStarAction, Action onExit)
        {
            InitializePanels(mazeModificationAction, bfsAction, aStarAction);
            this.onExit = onExit;
            OpenPanel(mainPanel);
        }

        private void InitializePanels(Action mazeModificationAction, Action bfsAction, Action aStarAction)
        {
            InitData initData = new InitData
            {
                mainPanel = mainPanel,
                // settingsPanel = settingsPanel,
                // helpPanel = helpPanel,
                onBack = Back,
                onOpenPanel = OpenPanel,
                mazeModificationAction = mazeModificationAction,
                bfsAction = bfsAction,
                aStarAction = aStarAction
            };
            
            mainPanel.Initialize(initData);
            // settingsPanel.Initialize(initData);
            // helpPanel.Initialize(initData);
        }

        public void Open() => gameObject.SetActive(true);

        public void Close()
        {
            gameObject.SetActive(false);
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

        private void OpenPanel(BasePanel panel)
        {
            SwitchPanel(panel);
            openedPanelsHistory.Push(currentPanel);
        }

        private void SwitchPanel(BasePanel panel)
        {
            if (currentPanel != null)
            {
                currentPanel.Close();
            }
            currentPanel = panel;
            
            if (currentPanel != null)
            {
                currentPanel.Show();
            }
        }
        
        public struct InitData
        {
            public MainPanel mainPanel;
            // public SettingsPanel settingsPanel;
            // public HelpPanel helpPanel;
            public Action onBack;
            public Action<BasePanel> onOpenPanel;
            public Action mazeModificationAction;
            public Action bfsAction;
            public Action aStarAction;
        }
    }
}