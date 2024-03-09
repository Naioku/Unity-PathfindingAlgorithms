using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class GlobalMap : ActionMap, Controls.IGlobalActions
    {
        public ActionData OnClickInteractionData { get; } = new ActionData();
        public ActionData OnCameraMovementData { get; } = new ActionData();
        public ActionData OnExitStageData { get; } = new ActionData();
        
        public GlobalMap(Controls.GlobalActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
        }

        public void OnClickInteraction(InputAction.CallbackContext context) => OnClickInteractionData.Invoke(context.phase);
        public void OnCameraMovement(InputAction.CallbackContext context) => OnCameraMovementData.Invoke(context.phase);
        public void OnExitStage(InputAction.CallbackContext context) => OnExitStageData.Invoke(context.phase);
    }
}