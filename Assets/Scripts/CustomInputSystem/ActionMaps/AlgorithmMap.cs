using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class AlgorithmMap : ActionMap, Controls.IAlgorithmActions
    {
        public ActionData OnPlayData { get; } = new ActionData();
        public ActionData OnPauseData { get; } = new ActionData();
        public ActionData OnStepData { get; } = new ActionData();
        public ActionData OnStopData { get; } = new ActionData();
        
        public AlgorithmMap(Controls.AlgorithmActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
        }

        public void OnPlay(InputAction.CallbackContext context) => OnPlayData.Invoke(context.phase);
        public void OnPause(InputAction.CallbackContext context) => OnPauseData.Invoke(context.phase);
        public void OnStep(InputAction.CallbackContext context) => OnStepData.Invoke(context.phase);
        public void OnStop(InputAction.CallbackContext context) => OnStopData.Invoke(context.phase);
    }
}