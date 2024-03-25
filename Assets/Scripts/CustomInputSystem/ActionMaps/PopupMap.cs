using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class PopupMap : ActionMap, Controls.IPopupActions
    {
        public ActionData OnConfirmData { get; }
        public ActionData OnCloseData { get; }
        
        public PopupMap(Controls.PopupActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);

            OnConfirmData = new ActionData(actionMap.Confirm);
            OnCloseData = new ActionData(actionMap.Close);
        }
        
        public void OnConfirm(InputAction.CallbackContext context) => OnConfirmData.Invoke(context.phase);
        public void OnClose(InputAction.CallbackContext context) => OnCloseData.Invoke(context.phase);
    }
}