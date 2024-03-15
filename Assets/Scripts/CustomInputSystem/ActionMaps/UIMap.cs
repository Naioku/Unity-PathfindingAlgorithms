using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class UIMap : ActionMap, Controls.IUIActions
    {
        public ActionData OnNavigateData { get; }
        public ActionData OnSubmitData { get; }
        public ActionData OnCancelData { get; }
        public ActionData OnPointData { get; }
        public ActionData OnClickData { get; }
        public ActionData OnScrollWheelData { get; }
        public ActionData OnMiddleClickData { get; }
        public ActionData OnRightClickData { get; }

        public UIMap(Controls.UIActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);

            OnNavigateData = new ActionData(actionMap.Navigate);
            OnSubmitData = new ActionData(actionMap.Submit);
            OnCancelData = new ActionData(actionMap.Cancel);
            OnPointData = new ActionData(actionMap.Point);
            OnClickData = new ActionData(actionMap.Click);
            OnScrollWheelData = new ActionData(actionMap.ScrollWheel);
            OnMiddleClickData = new ActionData(actionMap.MiddleClick);
            OnRightClickData = new ActionData(actionMap.RightClick);
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