﻿using System.Collections.Generic;
using CustomInputSystem.ActionMaps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomInputSystem
{
    public class InputManager
    {
        private readonly Controls controls = new Controls();
        private readonly List<ActionMap> mapsList = new List<ActionMap>();
        
        public GlobalMap GlobalMap { get; private set; }
        public StageSelectionMap StageSelectionMap { get; private set; }
        public MazeModificationMap MazeModificationMap { get; private set; }
        public AlgorithmMap AlgorithmMap { get; private set; }
        public UIMap UIMap { get; private set; }
        public Vector2 CursorPosition { get; private set; }
        
        public void Initialize()
        {
            AllManagers.Instance.UpdateManager.RegisterOnUpdate(UpdateCursorPosition);
            InitializeMaps();
            BuildMapsList();
            UIMap.Enable();
        }

        public void DisableAllMaps()
        {
            foreach (ActionMap actionMap in mapsList)
            {
                actionMap.Disable();
            }
        }

        public void Destroy()
        {
            AllManagers.Instance.UpdateManager.UnregisterFromUpdate(UpdateCursorPosition);
        }

        public void UpdateCursorPosition()
        {
            CursorPosition = Mouse.current.position.ReadValue();
        }
        
        public void PerformUpdate() => UpdateCursorPosition();
        
        private void InitializeMaps()
        {
            GlobalMap = new GlobalMap(controls.Global);
            StageSelectionMap = new StageSelectionMap(controls.StageSelection);
            MazeModificationMap = new MazeModificationMap(controls.MazeModification);
            AlgorithmMap = new AlgorithmMap(controls.Algorithm);
            UIMap = new UIMap(controls.UI);
        }

        private void BuildMapsList()
        {
            mapsList.Add(GlobalMap);
            mapsList.Add(StageSelectionMap);
            mapsList.Add(MazeModificationMap);
            mapsList.Add(AlgorithmMap);
            mapsList.Add(UIMap);
        }
    }
}