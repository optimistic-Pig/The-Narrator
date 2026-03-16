using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    //initalizes variables
    public float martianOpinion = 0.0f;
    public float dialogueIndexTracker = 0.0f;

    public TextMeshPro mainText;
    public TextMeshPro nameText;
    public TextMeshPro triesText;
    public TextMeshPro optionOne;
    public TextMeshPro optionTwo;
    public TextMeshPro OptionThree;
    public TextMeshPro OptionFour;

    private float tries = 3;

    public void Start()
    {
        mainText.text = "Izul is concerned regarding recent finding in the area (that being the area humans are located)";
        nameText.text = "IZUL: Geographer, The Basin";
        triesText.text = "You have " + tries + " left";
        optionOne.text = "Ask him to elaborate";
        optionTwo.text = "Question about the previous engineers who worked in the area";
        OptionThree.text = "Ask him what the big deal is, I mean he’s a Geographer";
        OptionFour.text = "Ask him for proof before continuing";
    }
    
    //used for a specific dialouge option
    public void OptionClicked (float option)
    {
        //martianOpinion += opinion;
        //dialogueIndexTracker = option;

        dialogueSetter(option);
    }
    //is in charge of all dialouge and sets the text to what is needed
    private void dialogueSetter(float option)
    {
        tries -= 1;
        triesText.text = "You have " + tries + " left";
       
        if (dialogueIndexTracker == 0)
        {    
            if(option == 1.0f)
            {
                dialogueIndexTracker = 1;

                mainText.text = "He mentions how after the Red Rock War, the land has been very unstable, with it somehow decaying every day";
                optionOne.text = "Decaying? What do you mean";
                optionTwo.text = "The land is unstable because of the war?";
                OptionThree.text = "What exactly happened during the war?";
                OptionFour.gameObject.SetActive(false);
            }

            else if (option == 2.0f)
            {

            }

            else if (option == 3.0f)
            {
                
            }

            else if (option == 4.0f)
            {
                
            }
        }
    }
}
