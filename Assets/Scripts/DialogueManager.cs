using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public float martianOpinion = 0.0f;
    public float dialogueTracker = 0.0f;

    public TextMeshPro positive;
    public TextMeshPro neutral;
    public TextMeshPro negative;
    
    public void nextLine (float opinion)
    {
        martianOpinion += opinion;
        dialogueTracker += 1.0f;

        if(dialogueTracker == 1)
        {
            positive.text = "positive example 2";
            neutral.text = "neutral example 2";
            negative.text = "negative example 2";
        }
    }
}
