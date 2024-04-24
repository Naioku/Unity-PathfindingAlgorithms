using System.Collections.Generic;
using CustomInputSystem.ActionMaps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomInputSystem
{
    public class InputManager
    {
        private readonly Controls controls = new Controls();
        private readonly List<ActionMap> mapsList = new List<ActionMap>();
        private readonly List<ActionMap> savedState = new List<ActionMap>();
        
        public GlobalMap GlobalMap { get; private set; }
        public MazeModificationMap MazeModificationMap { get; private set; }
        public AlgorithmMap AlgorithmMap { get; private set; }
        public PopupMap PopupMap { get; private set; }
        public Vector2 CursorPosition { get; private set; }
        
        public void Awake()
        {
            AllManagers.Instance.UpdateManager.RegisterOnUpdate(UpdateCursorPosition);
            InitializeMaps();
            BuildMapsList();
        }

        public void Destroy()
        {
            AllManagers.Instance.UpdateManager.UnregisterFromUpdate(UpdateCursorPosition);
        }

        public void DisableAllMaps()
        {
            foreach (ActionMap actionMap in mapsList)
            {
                actionMap.Disable();
            }
        }

        public void EnablePopupMode()
        {
            foreach (ActionMap map in mapsList)
            {
                if (map.Enabled)
                {
                    savedState.Add(map);
                }
                map.Disable();
            }

            PopupMap.Enable();
        }

        public void DisablePopupMode()
        {
            PopupMap.Disable();
            foreach (ActionMap map in savedState)
            {
                map.Enable();
            }
            
            savedState.Clear();
        }

        private void UpdateCursorPosition()
        {
            CursorPosition = Mouse.current.position.ReadValue();
        }

        private void InitializeMaps()
        {
            GlobalMap = new GlobalMap(controls.Global);
            MazeModificationMap = new MazeModificationMap(controls.MazeModification);
            AlgorithmMap = new AlgorithmMap(controls.Algorithm);
            PopupMap = new PopupMap(controls.Popup);
        }

        private void BuildMapsList()
        {
            mapsList.Add(GlobalMap);
            mapsList.Add(MazeModificationMap);
            mapsList.Add(AlgorithmMap);
            mapsList.Add(PopupMap);
        }
    }
}