using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UpdateSystem;

namespace CustomInputSystem
{
    public class InputManager : IUpdatable
    {
        private Controls controls;
        private readonly Dictionary<Enums.ActionMap, InputActionMap> actionMapLookup = new Dictionary<Enums.ActionMap, InputActionMap>();
        private readonly Dictionary<Enums.ActionMap, InputActionMap> permanentActionMapLookup = new Dictionary<Enums.ActionMap, InputActionMap>();
        private readonly Dictionary<Enums.ActionMap, Dictionary<Enums.InputAction, InputAction>> inputActionLookup = new Dictionary<Enums.ActionMap, Dictionary<Enums.InputAction, InputAction>>();

        public Vector2 CursorPosition { get; private set; }
        
        public void Initialize()
        {
            AllManagers.Instance.UpdateManager.Register(this);
            controls = new Controls();

            BuildActionMapLookup();
            BuildInputActionLookup();
        }

        public void Destroy()
        {
            AllManagers.Instance.UpdateManager.Unregister(this);
        }

        public void PerformUpdate()
        {
            CursorPosition = Mouse.current.position.ReadValue();
        }

        /// <summary>
        /// Disabling all action maps and enabling chosen one. Permanent action maps will be omitted.
        /// </summary>
        /// <param name="actionMapName">Action map to enable.</param>
        public void SetActionMap(Enums.ActionMap actionMapName)
        {
            foreach (InputActionMap actionMap in actionMapLookup.Values)
            {
                actionMap.Disable();
            }
            
            actionMapLookup[actionMapName].Enable();
        }

        /// <summary>
        /// Enables permanent action map.
        /// </summary>
        /// <param name="actionMapName">Action map to enable.</param>
        public void EnableActionMapPermanent(Enums.ActionMap actionMapName)
        {
            permanentActionMapLookup[actionMapName].Enable();
        }

        /// <summary>
        /// Disables permanent action map.
        /// </summary>
        /// <param name="actionMapName">Action map to enable.</param>
        public void DisableActionMapPermanent(Enums.ActionMap actionMapName)
        {
            permanentActionMapLookup[actionMapName].Disable();
        }

        public void SetOnStarted(Enums.ActionMap actionMap, Enums.InputAction inputAction, Action<InputAction.CallbackContext> action)
            => inputActionLookup[actionMap][inputAction].started += action;
        public void SetOnPerformed(Enums.ActionMap actionMap, Enums.InputAction inputAction, Action<InputAction.CallbackContext> action)
            => inputActionLookup[actionMap][inputAction].performed += action;
        public void SetOnCanceled(Enums.ActionMap actionMap, Enums.InputAction inputAction, Action<InputAction.CallbackContext> action)
            => inputActionLookup[actionMap][inputAction].canceled += action;
       
        public void RemoveOnStarted(Enums.ActionMap actionMap, Enums.InputAction inputAction, Action<InputAction.CallbackContext> action)
            => inputActionLookup[actionMap][inputAction].started -= action;
        public void RemoveOnPerformed(Enums.ActionMap actionMap, Enums.InputAction inputAction, Action<InputAction.CallbackContext> action)
            => inputActionLookup[actionMap][inputAction].performed -= action;
        public void RemoveOnCanceled(Enums.ActionMap actionMap, Enums.InputAction inputAction, Action<InputAction.CallbackContext> action)
            => inputActionLookup[actionMap][inputAction].canceled -= action;

        private void BuildActionMapLookup()
        {
            permanentActionMapLookup.Add(Enums.ActionMap.Global, controls.Global.Get());
            actionMapLookup.Add(Enums.ActionMap.StageSelection, controls.StageSelection.Get());
            actionMapLookup.Add(Enums.ActionMap.MazeModification, controls.MazeModification.Get());
            actionMapLookup.Add(Enums.ActionMap.Algorithm, controls.Algorithm.Get());
        }
        
        private void BuildInputActionLookup()
        {
            inputActionLookup.Add
            (
                Enums.ActionMap.Global, 
                new Dictionary<Enums.InputAction, InputAction>
                {
                    { Enums.InputAction.ClickInteraction, controls.Global.ClickInteraction },
                    { Enums.InputAction.CameraMovement, controls.Global.CameraMovement },
                    { Enums.InputAction.ExitStage, controls.Global.ExitStage }
                }
            );
            
            inputActionLookup.Add
            (
                Enums.ActionMap.StageSelection, 
                new Dictionary<Enums.InputAction, InputAction>
                {
                    { Enums.InputAction.MazeModification, controls.StageSelection.MazeModification },
                    { Enums.InputAction.BFS, controls.StageSelection.BFS },
                    { Enums.InputAction.AStar, controls.StageSelection.AStar }
                }
            );
            
            inputActionLookup.Add
            (
                Enums.ActionMap.MazeModification,
                new Dictionary<Enums.InputAction, InputAction>
                {
                    { Enums.InputAction.SetDefaultNode, controls.MazeModification.SetDefaultNode },
                    { Enums.InputAction.SetStartNode, controls.MazeModification.SetStartNode },
                    { Enums.InputAction.SetDestinationNode, controls.MazeModification.SetDestinationNode },
                    { Enums.InputAction.SetBlockedNode, controls.MazeModification.SetBlockedNode }
                }
            );
            
            inputActionLookup.Add
            (
                Enums.ActionMap.Algorithm, 
                new Dictionary<Enums.InputAction, InputAction>
                {
                    { Enums.InputAction.Play, controls.Algorithm.Play },
                    { Enums.InputAction.Pause, controls.Algorithm.Pause },
                    { Enums.InputAction.Step, controls.Algorithm.Step },
                    { Enums.InputAction.Refresh, controls.Algorithm.Refresh },
                    { Enums.InputAction.Stop, controls.Algorithm.Stop }
                }
            );
        }
    }
}