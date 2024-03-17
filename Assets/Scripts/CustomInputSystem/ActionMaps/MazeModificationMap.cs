using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class MazeModificationMap : ActionMap, Controls.IMazeModificationActions
    {
        public ActionData OnSetDefaultNodeData { get; }
        public ActionData OnSetStartNodeData { get; }
        public ActionData OnSetDestinationNodeData { get; }
        public ActionData OnSetBlockedNodeData { get; }
        
        public MazeModificationMap(Controls.MazeModificationActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);

            OnSetDefaultNodeData = new ActionData(actionMap.SetDefaultNode);
            OnSetStartNodeData = new ActionData(actionMap.SetStartNode);
            OnSetDestinationNodeData = new ActionData(actionMap.SetDestinationNode);
            OnSetBlockedNodeData = new ActionData(actionMap.SetBlockedNode);
        }

        public void OnSetDefaultNode(InputAction.CallbackContext context) => OnSetDefaultNodeData.Invoke(context.phase);
        public void OnSetStartNode(InputAction.CallbackContext context) => OnSetStartNodeData.Invoke(context.phase);
        public void OnSetDestinationNode(InputAction.CallbackContext context) => OnSetDestinationNodeData.Invoke(context.phase);
        public void OnSetBlockedNode(InputAction.CallbackContext context) => OnSetBlockedNodeData.Invoke(context.phase);
    }
}