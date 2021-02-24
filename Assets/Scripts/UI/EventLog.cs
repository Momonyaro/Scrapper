using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventLog : MonoBehaviour
{
    public static EventLog Instance;

    public Transform parentTransform;
    public GameObject logObject;

    public int maxLogObjects = 25;
    
    private void Awake()
    { 
        Instance = this;
    }

    public void Print(string message)
    {
        //TextMeshProUGUI
        GameObject text = Instantiate(logObject, parentTransform);
        text.transform.SetAsFirstSibling();

        text.GetComponent<TextMeshProUGUI>().text = message;
        CullMessages();
    }

    private void CullMessages()
    {
        if (parentTransform.childCount > maxLogObjects)
        {
            Destroy(parentTransform.GetChild(parentTransform.childCount - 1).gameObject);
        }
    }
}
