using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class AlgorithmMap : ActionMap, Controls.IAlgorithmActions
    {
        public ActionData OnPlayData { get; }
        public ActionData OnPauseData { get; }
        public ActionData OnStepData { get; }
        public ActionData OnStopData { get; }
        
        public AlgorithmMap(Controls.AlgorithmActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
            
            OnPlayData = new ActionData(actionMap.Play);
            OnPauseData = new ActionData(actionMap.Pause);
            OnStepData = new ActionData(actionMap.Step);
            OnStopData = new ActionData(actionMap.Stop);
        }

        public void OnPlay(InputAction.CallbackContext context) => OnPlayData.Invoke(context.phase);
        public void OnPause(InputAction.CallbackContext context) => OnPauseData.Invoke(context.phase);
        public void OnStep(InputAction.CallbackContext context) => OnStepData.Invoke(context.phase);
        public void OnStop(InputAction.CallbackContext context) => OnStopData.Invoke(context.phase);
    }
}