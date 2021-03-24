using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;

namespace Dialogue
{
    public class DialogueThread
    {
        private DialogueContainer _lastReadFile;
        //Import the dialogue data serialized object and convert it to our format.
        public List<ThreadNode> CurrentNodes;
        public int StartIndex = 0;
        public int CurrentIndex = 0;

        public void BuildNarrativeThread(DialogueContainer container)
        {
            _lastReadFile = container;
            
            List<ThreadNode> allNodes = new List<ThreadNode>();
            for (int i = 0; i < container.DialogueNodeData.Count; i++)
            {
                allNodes.Add(new ThreadNode()
                {
                    Guid = container.DialogueNodeData[i].NodeGUID,
                    NodeText = container.DialogueNodeData[i].DialogueText,
                    isStartNode = true,
                    NodeBridges = new ThreadBridge[0]
                });
            }

            for (int i = 0; i < allNodes.Count; i++)
            {
                ThreadNode currentNode = allNodes[i];
                List<ThreadBridge> bridges = new List<ThreadBridge>();
                
                for (int j = 0; j < container.NodeLinks.Count; j++)
                {
                    var link = container.NodeLinks[j];
                    if (link.BaseNodeGUID == currentNode.Guid)
                    {
                        for (int k = 0; k < allNodes.Count; k++)
                        {
                            if (link.TargetNodeGUID == allNodes[k].Guid)
                            {
                                var node = allNodes[k];
                                node.isStartNode = false;
                                allNodes[k] = node;
                                bridges.Add(new ThreadBridge(allNodes[k].Guid, link.PortName));
                            }
                        }
                    }
                }
                
                currentNode.BuildBridges(bridges.ToArray());
                allNodes[i] = currentNode;
            }

            CurrentNodes = allNodes;
            StartIndex = 0;
            for (int i = 0; i < CurrentNodes.Count; i++)
            {
                if (CurrentNodes[i].isStartNode)
                    StartIndex = i;
            }

            CurrentIndex = StartIndex;
        }

        public ThreadNode GetNodeFromGUID(string guid)
        {
            for (int i = 0; i < CurrentNodes.Count; i++)
            {
                if (CurrentNodes[i].Guid.Equals(guid))
                {
                    CurrentIndex = i;
                    return CurrentNodes[i];
                }
            }

            return new ThreadNode("ERROR:ERROR", "ERROR:ERROR");
        }
    }
}