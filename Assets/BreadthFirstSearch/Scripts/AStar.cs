using System;
using System.Collections.Generic;
using StageMachineSystem.Algorithm;
using UnityEngine;

namespace BreadthFirstSearch.Scripts
{
    public class AStar : Algorithm
    {
        private Node currentNode;
        private readonly List<Node> openNodes = new();
        
        protected override NodeBase CurrentNode
        {
            get => currentNode;
            set => currentNode = (Node)value;
        }

        public override Enums.MainMenuPanelButtonTag AlgorithmName => Enums.MainMenuPanelButtonTag.AStar;
        protected override NodeBase StartNode => new Node(startCoords, null, destinationCoords);

        public override void Refresh()
        {
            base.Refresh();
            openNodes.Clear();
        }

        protected override bool ReloadNode()
        {
            if (openNodes.Count == 0) return false;
            
            openNodes.Sort();
            CurrentNode = openNodes[0];
            openNodes.RemoveAt(0);
            CursorPosition = CurrentNode.Coords;
            return true;
        }

        protected override void OpenNode(Vector2Int coords) => openNodes.Add(new Node(coords, currentNode, destinationCoords));

        protected class Node : NodeBase, IComparable<Node>
        {
            private readonly Node previousNode;
            private readonly int costFromStart;
            private readonly int score;
            
            public override NodeBase PreviousNode => previousNode;
            
            public Node(Vector2Int coords, Node previousNode, Vector2Int destinationCoords) : base(coords)
            {
                if (previousNode == null)
                {
                    costFromStart = 0;
                }
                else
                {
                    costFromStart = previousNode.costFromStart + 1;
                }

                this.previousNode = previousNode;
                int costToDestination = Mathf.Abs(coords.x - destinationCoords.x) + Mathf.Abs(coords.y - destinationCoords.y);
                score = costToDestination + costFromStart;
                
                Debug.Log($"Coords: {Coords} CostFromStart: {costFromStart} Score: {score}");
            }

            public int CompareTo(Node other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return score.CompareTo(other.score);
            }
        }
    }
}