using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public struct ThreadNode
    {
        public string Guid;
        public string NodeText;
        public bool isStartNode;
        public ThreadBridge[] NodeBridges;

        public ThreadNode(string guid, string nodeText)
        {
            Guid = guid;
            NodeText = nodeText;
            isStartNode = true;
            NodeBridges = new ThreadBridge[0];
        }

        public void BuildBridges(ThreadBridge[] bridges)
        {
            NodeBridges = new ThreadBridge[0];
            NodeBridges = bridges;
        }
    }

    public struct ThreadBridge
    {
        private readonly string _bridgeText;
        private readonly string _nextNode;

        public ThreadBridge(string nextNode, string bridgeText)
        {
            this._nextNode = nextNode;
            this._bridgeText = bridgeText;
        }

        //Getters for the bridge values.
        //GetNextNode for what node we want to go to after
        public string GetNextNode() => _nextNode;
        //GetBridgeText for what the player is saying during transition.
        public string GetBridgeText() => _bridgeText;
    }
}