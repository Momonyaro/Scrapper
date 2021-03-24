using UnityEngine;

namespace UI
{
    public class DialogueOption : MonoBehaviour
    {
        public int bridgeIndex;
        public DialogueMenu parent;
    
        public void SendTraverseBridge()
        {
            if (bridgeIndex == -1) //Exit condition
            {
                parent.ToggleDialogue();
                return;
            }
        
            parent.DisplayThread(parent.TraverseBridge(bridgeIndex));
        }
    }
}
