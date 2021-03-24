using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Subtegral.DialogueSystem.DataContainers;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{
    public bool showMenu = false;
    public Animator animator;
    private DialogueThread _threadBuilder = new DialogueThread();

    public TMPro.TextMeshProUGUI actorText;
    public Transform replyParent;
    public GameObject replyPrefab;

    [Space()] 
    
    public string debugThreadName;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            ToggleDialogue();
        }

        if (showMenu && Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_threadBuilder.CurrentNodes[_threadBuilder.CurrentIndex].NodeBridges.Length > 0)
            {
                DisplayThread(TraverseBridge(0));
            }
        }
        
        if (showMenu && Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_threadBuilder.CurrentNodes[_threadBuilder.CurrentIndex].NodeBridges.Length > 1)
            {
                DisplayThread(TraverseBridge(1));
            }
        }
    }

    public void ToggleDialogue()
    {
        showMenu = !showMenu;
        animator.SetBool("SHOW_MENU", showMenu);
        if (showMenu)
            FetchAndDisplayThread(debugThreadName);
    }

    void FetchAndDisplayThread(string threadName)
    {
        DialogueContainer container = Resources.Load<DialogueContainer>("DialogueThreads/" + threadName);
        _threadBuilder.BuildNarrativeThread(container);
        
        DisplayThread(_threadBuilder.CurrentNodes[_threadBuilder.StartIndex]);
    }
    
    public void DisplayThread(ThreadNode node)
    {
        string text = TreatText(node.NodeText);
        
        actorText.text = text;
        
        //Display responses tied to the node.
        ConstructBridgeUI(node.NodeBridges);
    }

    void ConstructBridgeUI(ThreadBridge[] responses)
    {
        //Clear the last responses
        int q = replyParent.childCount;
        for (int i = 0; i < q; i++)
        {
            Destroy(replyParent.GetChild(i).gameObject);
        }

        if (responses.Length == 0) //Create option to leave dialogue
        {
            GameObject button = Instantiate<GameObject>(replyPrefab, replyParent);
            button.GetComponent<DialogueOption>().parent = this;
            button.GetComponent<DialogueOption>().bridgeIndex = -1;
            //Write the response text to the button's text component
            button.GetComponent<Text>().text = "(Leave).";
        }
        
        for (int i = 0; i < responses.Length; i++)
        {
            GameObject button = Instantiate<GameObject>(replyPrefab, replyParent);
            button.GetComponent<DialogueOption>().parent = this;
            button.GetComponent<DialogueOption>().bridgeIndex = i;
            //Write the response text to the button's text component
            button.GetComponent<Text>().text = responses[i].GetBridgeText();
        }
    }

    public ThreadNode TraverseBridge(int threadIndex)
    {
        ThreadNode currentNode = _threadBuilder.CurrentNodes[_threadBuilder.CurrentIndex];

        string nextNodeId = currentNode.NodeBridges[threadIndex].GetNextNode();
        ThreadNode nextNode = _threadBuilder.GetNodeFromGUID(nextNodeId);

        return nextNode;
    }

    string TreatText(string input)
    {
        input = input.Replace("<P_NAME>", "Player");
        input = input.Replace("\n", "");

        return input;
    }
}
