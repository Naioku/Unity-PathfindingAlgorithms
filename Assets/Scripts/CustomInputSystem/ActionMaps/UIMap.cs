using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class UIMap : ActionMap, Controls.IUIActions
    {
        public ActionData OnNavigateData { get; } = new ActionData();
        public ActionData OnSubmitData { get; } = new ActionData();
        public ActionData OnCancelData { get; } = new ActionData();
        public ActionData OnPointData { get; } = new ActionData();
        public ActionData OnClickData { get; } = new ActionData();
        public ActionData OnScrollWheelData { get; } = new ActionData();
        public ActionData OnMiddleClickData { get; } = new ActionData();
        public ActionData OnRightClickData { get; } = new ActionData();

        public UIMap(Controls.UIActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
        }

        public void OnNavigate(InputAction.CallbackContext context) => OnNavigateData.Invoke(context.phase);
        public void OnSubmit(InputAction.CallbackContext context) => OnSubmitData.Invoke(context.phase);
        public void OnCancel(InputAction.CallbackContext context) => OnCancelData.Invoke(context.phase);
        public void OnPoint(InputAction.CallbackContext context) => OnPointData.Invoke(context.phase);
        public void OnClick(InputAction.CallbackContext context) => OnClickData.Invoke(context.phase);
        public void OnScrollWheel(InputAction.CallbackContext context) => OnScrollWheelData.Invoke(context.phase);
        public void OnMiddleClick(InputAction.CallbackContext context) => OnMiddleClickData.Invoke(context.phase);
        public void OnRightClick(InputAction.CallbackContext context) => OnRightClickData.Invoke(context.phase);
    }
}