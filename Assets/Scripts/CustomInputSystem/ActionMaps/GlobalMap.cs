using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class GlobalMap : ActionMap, Controls.IGlobalActions
    {
        public ActionData OnClickInteractionData { get; }
        public ActionData OnCameraMovementData { get; }
        public ActionData OnBackData { get; }
        
        public GlobalMap(Controls.GlobalActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);

            OnClickInteractionData = new ActionData(actionMap.ClickInteraction);
            OnCameraMovementData = new ActionData(actionMap.CameraMovement);
            OnBackData = new ActionData(actionMap.Back);
        }

        public void OnClickInteraction(InputAction.CallbackContext context) => OnClickInteractionData.Invoke(context.phase);
        public void OnCameraMovement(InputAction.CallbackContext context) => OnCameraMovementData.Invoke(context.phase);
        public void OnBack(InputAction.CallbackContext context) => OnBackData.Invoke(context.phase);
    }
}