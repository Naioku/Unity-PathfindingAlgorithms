using System.Collections.Generic;
using StageMachineSystem.Algorithm;
using UnityEngine;

namespace Algorithms
{
    public class BFS : Algorithm
    {
        private Node currentNode;
        protected readonly Queue<Node> openNodes = new();

        public override Enums.MainMenuPanelButtonTag AlgorithmName => Enums.MainMenuPanelButtonTag.BFS;
        protected override NodeBase StartNode => new Node(startCoords, null);

        protected override NodeBase CurrentNode
        {
            get => currentNode;
            set => currentNode = (Node)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            openNodes.Clear();
        }

        protected override void OpenNode(Vector2Int coords) => openNodes.Enqueue(new Node(coords, currentNode));
        
        protected override bool ReloadNode()
        {
            if (openNodes.Count == 0) return false;
            
            CurrentNode = openNodes.Dequeue();
            CursorPosition = CurrentNode.Coords;
            return true;
        }

        protected class Node : NodeBase
        {
            private readonly Node previousNode;

            public override NodeBase PreviousNode => previousNode;

            public Node(Vector2Int coords, Node previousNode) : base(coords) => this.previousNode = previousNode;
        }
    }
}