using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace CustomInputSystem.ActionMaps
{
    public abstract class ActionMap
    {
        protected InputActionMap actionMap;

        public bool Enabled => actionMap.enabled;
        public void Enable() => actionMap.Enable();
        public void Disable() => actionMap.Disable();
        
        public class ActionData
        {
            private InputAction inputAction;
            public StringVariable Binding { get; } = new();

            public ActionData(InputAction inputAction)
            {
                this.inputAction = inputAction;
                Binding.Value = inputAction.GetBindingDisplayString();
            }

            public void Invoke(InputActionPhase inputActionPhase)
            { 
                switch (inputActionPhase)
                {
                    case InputActionPhase.Started:
                        Started?.Invoke();
                        break;
                    
                    case InputActionPhase.Performed:
                        Performed?.Invoke();
                        break;
                    
                    case InputActionPhase.Canceled:
                        Canceled?.Invoke();
                        break;
                    
                    default:
                        Debug.LogError("The action phase is not supported.");
                        break;
                }
            }

            public event Action Started;
            public event Action Performed;
            public event Action Canceled;
        }
        
        public class ActionData<T>
        {
            private InputAction inputAction;
            public StringVariable Binding { get; } = new();

            public ActionData(InputAction inputAction)
            {
                this.inputAction = inputAction;
                Binding.Value = inputAction.GetBindingDisplayString();
            }

            public void Invoke(InputActionPhase inputActionPhase, T value)
            { 
                switch (inputActionPhase)
                {
                    case InputActionPhase.Started:
                        Started?.Invoke(value);
                        break;
                    
                    case InputActionPhase.Performed:
                        Performed?.Invoke(value);
                        break;
                    
                    case InputActionPhase.Canceled:
                        Canceled?.Invoke(value);
                        break;
                    
                    default:
                        Debug.LogError("The action phase is not supported.");
                        break;
                }
            }

            public event Action<T> Started;
            public event Action<T> Performed;
            public event Action<T> Canceled;
        }
    }
}