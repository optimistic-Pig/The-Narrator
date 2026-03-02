using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    //initalizes variables
    public float martianOpinion = 0.0f;
    public float dialogueTracker = 0.0f;

    public TextMeshPro positive;
    public TextMeshPro neutral;
    public TextMeshPro negative;
    
    //used to increment to the next dialouge option
    public void nextLine (float opinion)
    {
        martianOpinion += opinion;
        dialogueTracker += 1.0f;

        dialogueSetter(dialogueTracker);
    }
    //used for a specific dialouge option
    public void setLine (float opinion, float tracker)
    {
        martianOpinion += opinion;
        dialogueTracker = tracker;

        dialogueSetter(dialogueTracker);
    }
    //is in charge of all dialouge and sets the text to what is needed
    private void dialogueSetter(float tracker)
    {
        if(tracker == 1)
        {
            positive.text = "positive example 2";
            neutral.text = "neutral example 2";
            negative.text = "negative example 2";
        }
        else if (tracker == 2)
        {

        }
    }
}
