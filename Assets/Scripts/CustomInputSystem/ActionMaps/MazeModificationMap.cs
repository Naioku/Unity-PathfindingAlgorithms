using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class MazeModificationMap : ActionMap, Controls.IMazeModificationActions
    {
        public ActionData OnSetDefaultNodeData { get; } = new ActionData();
        public ActionData OnSetStartNodeData { get; } = new ActionData();
        public ActionData OnSetDestinationNodeData { get; } = new ActionData();
        public ActionData OnSetBlockedNodeData { get; } = new ActionData();
        
        public MazeModificationMap(Controls.MazeModificationActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
        }

        public void OnSetDefaultNode(InputAction.CallbackContext context) => OnSetDefaultNodeData.Invoke(context.phase);
        public void OnSetStartNode(InputAction.CallbackContext context) => OnSetStartNodeData.Invoke(context.phase);
        public void OnSetDestinationNode(InputAction.CallbackContext context) => OnSetDestinationNodeData.Invoke(context.phase);
        public void OnSetBlockedNode(InputAction.CallbackContext context) => OnSetBlockedNodeData.Invoke(context.phase);
    }
}