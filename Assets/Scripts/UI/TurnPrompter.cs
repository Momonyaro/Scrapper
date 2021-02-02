using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class TurnPrompter : MonoBehaviour
{
    [SerializeField]
    public AnimationCurve promptAlphaFade = new AnimationCurve();

    public Image image;
    

    public void PlayFadingPrompt()
    {
        StartCoroutine(PlayFade());
    }

    private IEnumerator PlayFade()
    {
        float currentX = 0;

        while (currentX < promptAlphaFade.keys[promptAlphaFade.keys.Length - 1].time) //compare current X to the final X
        {
            currentX += Time.unscaledDeltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, promptAlphaFade.Evaluate(currentX));
            yield return new WaitForEndOfFrame();
        }
        
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        yield break;
    }
}
