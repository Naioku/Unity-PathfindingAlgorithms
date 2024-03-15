using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class GlobalMap : ActionMap, Controls.IGlobalActions
    {
        public ActionData OnClickInteractionData { get; }
        public ActionData OnCameraMovementData { get; }
        public ActionData OnExitStageData { get; }
        
        public GlobalMap(Controls.GlobalActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);

            OnClickInteractionData = new ActionData(actionMap.ClickInteraction);
            OnCameraMovementData = new ActionData(actionMap.CameraMovement);
            OnExitStageData = new ActionData(actionMap.ExitStage);
        }

        public void OnClickInteraction(InputAction.CallbackContext context) => OnClickInteractionData.Invoke(context.phase);
        public void OnCameraMovement(InputAction.CallbackContext context) => OnCameraMovementData.Invoke(context.phase);
        public void OnExitStage(InputAction.CallbackContext context) => OnExitStageData.Invoke(context.phase);
    }
}