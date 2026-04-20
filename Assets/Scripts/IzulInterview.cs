using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Izul's interview data: dictionary, topics, headlines, and full dialogue tree.
/// Attach this to a GameObject and drag it into DialogueManager.availableInterviews.
/// </summary>
public class IzulInterview : InterviewBase
{
    // =====================================================================
    // CHARACTER INFO
    // =====================================================================

    public override string CharacterName => "IZUL: Geographer, The Basin";
    public override int StartingLookups => 3;

    // =====================================================================
    // DICTIONARY ENTRIES
    // (index 0=QIH, 1=nuqneH, 2=Ves, 3=ngevwI\'pu\', 4=bIQ)
    // =====================================================================

    private DictEntry[] _dictEntries = new DictEntry[]
    {
        new DictEntry { klingonWord = "QIH",      translation = "destruction", altSpellings = null },
        new DictEntry { klingonWord = "nuqneH",   translation = "human",       altSpellings = null },
        new DictEntry { klingonWord = "Ves",       translation = "war",         altSpellings = null },
        new DictEntry { klingonWord = "ngevwI\'pu\'", translation = "rebels",
                        altSpellings = new string[] { "ngevwI\u2019pu\u2019" } },
        new DictEntry { klingonWord = "bIQ",       translation = "water",       altSpellings = null },
    };
    public override DictEntry[] DictionaryEntries => _dictEntries;

    // =====================================================================
    // TOPICS (for headline unlock)
    // =====================================================================

    private Topic[] _topics = new Topic[]
    {
        new Topic { name = "destruction", keywords = new string[]
            { "QIH", "decaying", "dying", "damage", "unstable", "barren", "decline", "destroyed" } },
        new Topic { name = "war", keywords = new string[]
            { "Ves", "Red Rock", "weapons", "survived" } },
        new Topic { name = "rebels", keywords = new string[]
            { "ngevwI\'pu\'", "rebel", "harvesting", "looting", "attacked the area" } },
        new Topic { name = "water", keywords = new string[]
            { "bIQ", "water reserve", "water running" } },
    };
    public override Topic[] Topics => _topics;

    // =====================================================================
    // HEADLINES
    // (requiredDictIndex/requiredTopicIndex map to arrays above)
    // =====================================================================

    private Headline[] _headlines = new Headline[]
    {
        new Headline { text = "Martians face Destruction",
                       requiredDictIndex = 0, requiredTopicIndex = 0, alwaysAvailable = false },
        new Headline { text = "What you may have missed from the Red Rock War",
                       requiredDictIndex = 2, requiredTopicIndex = 1, alwaysAvailable = false },
        new Headline { text = "Human extremists kill Martians",
                       requiredDictIndex = 3, requiredTopicIndex = 2, alwaysAvailable = false },
        new Headline { text = "Mars Updates",
                       requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = true },
    };
    public override Headline[] Headlines => _headlines;

    // =====================================================================
    // LAST QUESTION NODES
    // =====================================================================

    private static readonly HashSet<float> _lastQuestionNodes = new HashSet<float>
    {
        8, 27, 28, 31, 33, 35, 36, 38, 39, 42, 44,
        50, 53, 57, 58, 60, 74, 76, 78, 80, 81, 85, 87
    };
    public override HashSet<float> LastQuestionNodes => _lastQuestionNodes;

    // =====================================================================
    // UNLOCK FLAGS (game world items specific to Izul)
    // =====================================================================

    [HideInInspector] public bool unlockTranscript = false;
    [HideInInspector] public bool unlockOldPaper = false;
    [HideInInspector] public bool unlockProSelfRiches = false;

    public override void ResetState()
    {
        base.ResetState();
        unlockTranscript = false;
        unlockOldPaper = false;
        unlockProSelfRiches = false;
    }

    public override string GetEndOfDaySummary()
    {
        string summary = "Interview with Izul complete.\n\n";
        if (unlockTranscript) summary += "\u2022 Unlocked: Transcript\n";
        if (unlockOldPaper) summary += "\u2022 Unlocked: Old Paper\n";
        if (unlockProSelfRiches) summary += "\u2022 Unlocked: Pro-Self Riches info\n";
        return summary;
    }

    // =====================================================================
    // DIALOGUE TREE
    // =====================================================================

    public override void DialogueSetter(float option, DialogueManager dm)
    {
        dm.ShowAllOptions();

        // NODE 0: Initial prompt
        if (dialogueIndexTracker == 0)
        {
            // option 0 = initial setup call from StartInterview
            if (option == 0f)
            {
                dm.SetDialogueTexts(
                    "I have concerns regarding recent findings in this area \u2014 the area where nuqneH are located.",
                    "Ask to elaborate",
                    "Question about the previous engineers who worked in the area",
                    "Ask what the big deal is \u2014 he\u2019s a Geographer",
                    "Ask for proof before continuing"
                );
            }
            else if (option == 1.0f)
            {
                dialogueIndexTracker = 1;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "After the Red Rock Ves, the land has been very unstable. It decays more every day.",
                    "Decaying? What do you mean",
                    "The land is unstable because of the Ves?",
                    "What exactly happened during the Ves?"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 24;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Your previous management disregarded our messages entirely.",
                    "Ask who exactly that may be",
                    "Retaliate and mention how if the previous engineers cleared it then it must be fine",
                    "Ask if it's possible there was just a language barrier"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 47;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Ever since nuqneH arrived, the land has started to die.",
                    "Ask how the land has changed",
                    "Ask if the change is caused by nuqneH",
                    "Ask about the Ves"
                );
            }
            else if (option == 4.0f)
            {
                dialogueIndexTracker = 3;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Here — look at this. These documents show how the area has been changing.",
                    "Question the document on vegetation",
                    "Question the document on topography",
                    "Question the document on meteorology and atmospheric change"
                );
            }
        }

        // NODE 1: Elaborate -> Red Rock War, land decaying
        else if (dialogueIndexTracker == 1)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 2;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "There is something... off. The evidence shows the land is declining, but there seems to be something else going on, too.",
                    "Ask about the land",
                    "Ask about this \u201Coff\u201D feeling"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 10;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "The Ves caused serious damage to the land, particularly because the ngevwI'pu' used technology never seen before.",
                    "Ask about the technology",
                    "Ask about the damage to the land"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 17;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region — with the intent of harvesting the resources for personal gain.",
                    "Ask why this particular area",
                    "Ask what resources",
                    "Ask for proof"
                );
            }
        }

        // NODE 2: Something off / Proof documents
        else if (dialogueIndexTracker == 2)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 3;
                dm.SetDialogueTexts(
                    "The land is decaying — vegetation, topography, meteorology — it's all changing. Here, let me show you the documents.",
                    "Question the document on vegetation",
                    "Question the document on topography",
                    "Question the document on meteorology and atmospheric change"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 4;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Even with all the data I have, the land is reacting in ways it shouldn't. Unnatural sounds emerge late at night, land being QIH unnaturally... just weird things.",
                    "Ask what kind of noises and QIH",
                    "Ask about the data — no time for conspiracy theories"
                );
            }
        }

        // NODE 3: Land decaying -> document choices
        else if (dialogueIndexTracker == 3)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 5;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Natural Martian vegetation is dying. We are trying to figure out how to save the natural life, or replace it with something new.",
                    "Ask what is dying",
                    "Ask if this leads to economic opportunity"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 6;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Recent changes in topography — drastic and sudden — have made the land very unstable.",
                    "Ask what this implies for the area",
                    "Ask what caused this sudden change"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 7;
                dm.SetDialogueTexts(
                    "The weather has cooled this area drastically. Colder days, colder nights.",
                    "Ask what caused this",
                    "Ask if this can be reversed",
                    "Ask if this allows for economic opportunity"
                );
            }
        }

        // NODE 4: Off feeling -> noises / data
        else if (dialogueIndexTracker == 4)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 8;
                dm.SetDialogueTexts(
                    "It sounds like construction. The weirdest thing is what sounds like bIQ running — when it shouldn't be.",
                    "Ask more about the bIQ sounds",
                    "Ask if he's reported this"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 9;
                dm.SetDialogueTexts(
                    "Someone I know knows somebody who survived the Ves — named Gorp. Most of what I know comes from him.",
                    "Ask how to find this Gorp",
                    "Ask to focus on what the data shows"
                );
            }
        }

        // NODE 5: Vegetation dying
        else if (dialogueIndexTracker == 5)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 75;
                dm.SetDialogueTexts("All the natural Mars plants and forests are dying, leaving areas barren and open.", "Ask if the land can be developed on", "Ask how to help");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 78;
                dm.SetDialogueTexts("This is no time for financial gain! How can you think of profit when the land is dying?", "Apologize and ask how you can help instead", "Explain that development could benefit Martians too");
            }
        }

        // NODE 6: Topography unstable
        else if (dialogueIndexTracker == 6)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 79;
                dm.SetDialogueTexts("The changing topography has made the land unstable, leaving it open for ngevwI'pu' to harvest resources.", "Ask about the ngevwI'pu'", "Ask if the land is safe enough to develop on");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 82;
                dm.SetDialogueTexts("I believe the Ves caused this.", "Ask about the Ves", "Ask how to help");
            }
        }

        // NODE 7: Meteorology / weather cooled
        else if (dialogueIndexTracker == 7)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 84;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("I believe the Ves caused this.", "Ask about the Ves", "Ask how to help");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 85;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("It can be reversed, but it would need drastic effort from nuqneH to work with the Martians.", "Ask what kind of effort", "Ask how your paper can help raise awareness");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 87;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("I shouldn't have talked to a nuqneH for help. All you think about is profit!", "Apologize and refocus on the land issues", "Explain you meant it could fund restoration");
            }
        }

        // NODE 8: Construction sounds, water running
        else if (dialogueIndexTracker == 8)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know much more, but bIQ hasn't flowed in this region for years. Please, bring awareness to the land issues in your paper. The topography, vegetation, and atmosphere are all in danger.");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("That's exactly the problem \u2014 no one listens to a geographer. Please, raise awareness through your paper. The land needs saving.");
            }
        }

        // NODE 9: Data focus / Gorp
        else if (dialogueIndexTracker == 9)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                unlockOldPaper = true;
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("You could probably find an old paper around your office somewhere that goes into depth about the Ves. But more importantly, the land data shows clear decline \u2014 please, raise awareness.");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 3;
                dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("Thank you for focusing on what matters. The data shows the land is decaying across three areas. Here \u2014 let me show you.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
        }

        // NODE 10: War caused serious damage
        else if (dialogueIndexTracker == 10)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 11;
                dm.SetDialogueTexts("I don't know much about the Ves — mostly just rumors. Very few Martians survived. One of them mentioned seeing things never seen before, including weapons of mass QIH.", "Ask how he knows this", "Ask how to find out more about the Ves", "Ask about the technology", "Things? What else aside from the technology?");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 16;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("Here \u2014 look at this data on how the land has changed since the Ves. The evidence is clear. The region is in serious decline.", "Ask about the documents on vegetation", "Ask about the documents on topography", "Ask about the documents on meteorology");
            }
        }

        // NODE 11: War rumors / weapons
        else if (dialogueIndexTracker == 11)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 12;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("Someone I know knows somebody who survived — named Gorp. Everything I know comes from him.", "Ask if there's a way to talk to Gorp", "Ask to get back to the land issues");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 13;
                unlockOldPaper = true;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("You could probably find an old paper around your office somewhere that goes into depth about it. Maybe question its reliability?", "Ask to hear more about the land instead", "Thank for the tip");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 14;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know anything about that. Try asking a survivor.", "Ask to refocus on the land", "Ask if there are any survivors nearby");
            }
            else if (option == 4.0f)
            {
                dialogueIndexTracker = 15;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know for sure, but allegedly, a race of Martian never seen before helped the ngevwI'pu'.", "Ask more about this race", "Ask to get back to the land issues");
            }
        }

        // NODE 12: Gorp
        else if (dialogueIndexTracker == 12)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("That's beyond my reach. But please — bring awareness to my findings. The topography, vegetation, and atmosphere are in danger!");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("[Izul nods and pulls out documents] The land is decaying across three fronts.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
        }

        // NODE 13: Old paper in office
        else if (dialogueIndexTracker == 13)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("He's glad you're refocusing. The land is what matters. He shows his documents.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("[Izul nods] Please, use your paper to bring awareness. The land here is in danger and the people need to know.");
            }
        }

        // NODE 14: Ask a survivor
        else if (dialogueIndexTracker == 14)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("Agreed. The land data is what I can speak to. [Izul pulls out documents]", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("There might be, but that's not my area. What I do know is the land is dying. Please, raise awareness through your paper.");
            }
        }

        // NODE 15: Unknown race helped rebels
        else if (dialogueIndexTracker == 15)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know for sure, but allegedly, a race of Martian never seen before helped the ngevwI'pu'. I can't say more. But please \u2014 write about the land. It needs saving.");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("He's relieved you're getting back on track. He shows his documents once more.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
        }

        // NODE 16: Proof of land (shared merge -> documents)
        else if (dialogueIndexTracker == 16)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 5; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("Natural Martian vegetation is dying. We are trying to figure out how to save the natural life, or replace it with something new.", "Ask what is dying", "Ask if this leads to economic opportunity");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 6; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("Recent changes in topography — drastic and sudden — have made the land very unstable.", "Ask what this implies for the area", "Ask what caused this sudden change");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 7;
                dm.SetDialogueTexts("The weather has cooled this area drastically. Colder days, colder nights.", "Ask what caused this", "Ask if this can be reversed", "Ask if this allows for economic opportunity");
            }
        }

        // NODE 17: What happened during war (rebel group)
        else if (dialogueIndexTracker == 17)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 18; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know exactly why, but the land here is holy — and full of riches.", "Ask what he means by holy", "Ask what kind of riches");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 19;
                dm.SetDialogueTexts("There are precious metals and gasses here, along with what was once a bIQ reserve \u2014 before the nuqneH put up their base.", "Ask about the bIQ", "Ask about these papers", "Ask for proof of the land");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 21; unlockOldPaper = true; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't have anything on me — only proof of the land changing. But you could probably find a paper around the office somewhere with \u201Cproof\u201D", "Ask about these papers", "Ask for proof of the land");
            }
        }

        // NODE 18: Holy land, full of riches
        else if (dialogueIndexTracker == 18)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 71;
                dm.SetDialogueTexts("The big deal is the area is decaying. As a holy site, it needs saving.", "Ask to hear more about the land for your paper", "Question his credentials");
            }
            else if (option == 2.0f)
            {
                unlockProSelfRiches = true; dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("Precious metals, rare gasses, and what was once a thriving bIQ reserve. But please — focus on the land's decline in your paper.", "Promise to write about the land");
            }
        }

        // NODE 19: Resources (metals, gasses, water)
        else if (dialogueIndexTracker == 19)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 20; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("The bIQ disappeared. No one knows where it went. The land is suffering without it.", "Ask about the papers on the Ves", "Ask for proof of the land changing");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 22; unlockOldPaper = true; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 16;
                dm.SetDialogueTexts("Let me pull out my data on the land. The evidence is right here.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology");
            }
        }

        // NODE 20: Water disappeared
        else if (dialogueIndexTracker == 20)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 22; unlockOldPaper = true;
                dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("[Izul pulls out documents] Here \u2014 my data on the land.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology");
            }
        }

        // NODE 21: No proof on him, paper in office
        else if (dialogueIndexTracker == 21)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 22; unlockOldPaper = true;
                dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("Here \u2014 let me show you my data on the land.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology");
            }
        }

        // NODE 22: Previous paper person did story on war
        else if (dialogueIndexTracker == 22)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I want to bring awareness of the changing times, and I\u2019m seeking your help. Please \u2014 the topography, vegetation, and atmosphere are in danger!");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("[Izul nods and pulls out documents] Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology");
            }
        }

        // NODE 24: Previous engineers -> defensive
        else if (dialogueIndexTracker == 24)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 25; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("The ones that came before you \u2014 they talked to the one that looks like you.", "Ask who exactly the \u201Cone before you\u201D is", "The one that looks like me?");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 30;
                dm.SetDialogueTexts("There must be a language barrier \u2014 or it was intentionally left out.", "Question how the language barrier was crossed", "Question who the previous management was he is talking about", "Retaliate again and question why would they omit important information");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 41; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("It's not impossible. The interpreter before the Ves and mass expansion started had just died.", "Ask who the interpreter was", "Ask how the interpreter died");
            }
        }

        // NODE 25: Ones that came before
        else if (dialogueIndexTracker == 25)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 26; dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("Why should I know? It's your employer. Now that you mention it, nobody really knows who is in charge... it feels almost as if the nuqneH aren't in control.", "Ask what that means");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 28; dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("The previous newspaper writer.", "Getting off track and go back to the problem at hand");
            }
        }

        // NODE 26: Nobody knows who's in charge
        else if (dialogueIndexTracker == 26)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 27;
                dm.SetDialogueTexts("Something is just off. The nuqneH are confused yet somehow have a large influence. It doesn't make sense.", "Ask to focus on the land issues for the paper", "Ask what he thinks is really going on");
            }
        }

        // NODE 27: Something off about humans
        else if (dialogueIndexTracker == 27)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("Thank you for refocusing. Please, bring awareness to the land issues. The topography, vegetation, and atmosphere are all in danger.");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("[Izul shakes his head] I'm just a geographer. But the land is dying — that I'm sure of. Please, write about it.");
            }
        }

        // NODE 28: Previous newspaper writer
        else if (dialogueIndexTracker == 28)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 29; dm.optionOne.gameObject.SetActive(false);
                dm.SetDialogueTexts("You\u2019re right. What matters is the land. The topography, vegetation, and atmosphere are in danger. Please, use your paper to help.");
            }
        }

        // NODE 30: Language barrier or intentionally left out
        else if (dialogueIndexTracker == 30)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 31; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("It used to be the lady who looks like you. Then it became the previous newspaper writer, who was nowhere near as good.", "Ask who the interpreter was", "Ask what happened to the interpreter");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 34; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("The previous newspaper writer was instructed to talk to the higher-ups and let them know before construction began.", "Question if he was ever told that they received the information", "Ask if he followed up with the newspaper writer");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 37;
                dm.SetDialogueTexts("Maybe someone wanted to make a quick profit, get power, or simply doesn't like the Martians. I don't know why — only that it's possible. Maybe the previous newspaper writer changed once the interpreter died.", "Quick profit?", "Get power?", "Not like the Martians?");
            }
        }

        // NODE 31: Interpreter lady, then newspaper writer
        else if (dialogueIndexTracker == 31)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 32;
                dm.SetDialogueTexts("It was a lady that used to work with the newspaper writer. The two of them had a really good relationship, from what everyone could see.", "Ask if she was the previous management's wife", "Ask what happened to her");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 33;
                dm.SetDialogueTexts("She died, but I don't know exactly how. To my knowledge, she was the previous management's wife.", "Ask how she died", "Ask about the land situation instead");
            }
        }

        // NODE 32: Lady who worked with newspaper writer
        else if (dialogueIndexTracker == 32)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know for sure, but there is proof showing the land is unstable. Action needs to be taken now. Please, use your paper to raise awareness.");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("She died. That's all I know. But please, the land is what matters. Your paper could be the way to help.");
            }
        }

        // NODE 33: Interpreter died
        else if (dialogueIndexTracker == 33)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("Nobody knows exactly how. To my knowledge, she was the previous management's wife. But please \u2014 the land needs your attention.");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("Thank you for focusing on what matters. Please, bring awareness to the land issues in your paper. The topography, vegetation, and atmosphere are in danger.");
            }
        }

        // NODE 34: Newspaper writer told higher-ups
        else if (dialogueIndexTracker == 34)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 35;
                dm.SetDialogueTexts("Nobody was told, and I don't know exactly how it happened. To my knowledge, she was the previous management's wife.", "Ask more about the previous management's wife", "Ask about the land situation instead");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 36;
                dm.SetDialogueTexts("I couldn't. He seemed hesitant to talk with Martians and seemed to be in a hurry.", "Ask why the newspaper writer was hesitant", "Ask about the land instead");
            }
        }

        // NODE 35: Nobody told
        else if (dialogueIndexTracker == 35)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                unlockTranscript = true; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know much more. She was the interpreter, and she died before the expansion. But what matters is the land \u2014 please raise awareness.");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("[Izul nods] The land is what matters. Please, use your paper to bring awareness. The topography, vegetation, and atmosphere are in danger.");
            }
        }

        // NODE 36: Newspaper writer hesitant
        else if (dialogueIndexTracker == 36)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know. nuqneH are confusing to me. But the land \u2014 that's what I understand. Please, write about it.");
            }
            else if (option == 2.0f)
            {
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("He's grateful. Please, bring awareness to the land. The topography, vegetation, and atmosphere are all in danger.");
            }
        }

        // NODE 37: Quick profit, power, don't like Martians
        else if (dialogueIndexTracker == 37)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 38; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("There is an abundance of wealth and power that comes from ruling this land.", "Ask to elaborate on the land's resources", "Ask how your paper can help");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 39; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("There is a sense of prejudice that lingers in the air with nuqneH. Take their word carefully.", "Ask if he has prejudice against nuqneH", "Ask about the land situation");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 40; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("It's possible. Some nuqneH don't understand or care about Martian culture. But what matters is the land is dying.", "Ask how you can help through your paper", "Ask what specifically is happening to the land");
            }
        }

        // NODE 38: Wealth from land
        else if (dialogueIndexTracker == 38)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Precious metals, rare gasses, and what was once a bIQ reserve. But the land is being exploited. Please, raise awareness."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I\u2019m asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 39: Prejudice
        else if (dialogueIndexTracker == 39)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Time will tell. For now, the land is what matters."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[Izul nods] Please, bring awareness to the land. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 40: Don't care about Martian culture
        else if (dialogueIndexTracker == 40)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("[Izul pulls out documents] The land is decaying across vegetation, topography, and meteorology.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
        }

        // NODE 41: Interpreter died before expansion
        else if (dialogueIndexTracker == 41)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 42; dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("The one that looks like you. [laughs] You nuqneH all look the same to us.", "Ask if there\u2019s any way to find out more about her");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 44;
                dm.SetDialogueTexts("I don't know exactly. Only that she was the previous management's wife.", "What management?", "When did she die?");
            }
        }

        // NODE 42: Can't tell humans apart
        else if (dialogueIndexTracker == 42)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 43; unlockTranscript = true; dm.optionOne.gameObject.SetActive(false);
                dm.SetDialogueTexts("Why should I know? It's your employer. Now that you mention it, nobody really knows who is in charge... it feels almost as if the nuqneH aren't in control.. But please \u2014 the land needs your help.");
            }
        }

        // NODE 44: How interpreter died
        else if (dialogueIndexTracker == 44)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 45; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("I don't know the details of nuqneH management structures. But what I do know is the land is in danger. Please, write about it.");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 46; unlockTranscript = true; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("Right before the Red Rock Ves. It was almost as if she was the catalyst. But please \u2014 the land is what matters now. Write about it.");
            }
        }

        // NODE 47: Big deal / geographer
        else if (dialogueIndexTracker == 47)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 48;
                dm.SetDialogueTexts("So much has changed that the place is no longer recognizable.", "Ask what it used to look like", "Ask if that\u2019s for the better \u2014 this could be something new", "Ask if there is anything you can do to help");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 55; dm.optionFour.gameObject.SetActive(true);
                dm.SetDialogueTexts("That is a possibility, but not the only one. There are also the ngevwI'pu', and the Ves that happened.", "Ask why it may be the nuqneH", "Ask why it may be the ngevwI'pu'", "Ask if there\u2019s any correlation between the two", "Ask about the Ves");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 17;
                dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region — with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof");
            }
        }

        // NODE 48: No longer recognizable
        else if (dialogueIndexTracker == 48)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 49; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("This land is holy. I\u2019d encourage you to do some research after our interview, or talk to someone who knows more.", "Ask about anything else important for the paper", "Ask to just explain it");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 52; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("NuqneH greed... I\u2019m now certain nuqneH may be causing this.", "Apologize and ask about the land", "Ask for proof");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 53; dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts("I appreciate your sympathy, but I believe nuqneH may be causing this.", "Ask what you can do differently", "Ask how your paper can raise awareness");
            }
        }

        // NODE 49: Holy land, research
        else if (dialogueIndexTracker == 49)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 50; dm.SetDialogueTexts("I want to bring awareness of the changing times, and I\u2019m seeking your help.", "Promise to write about it", "Ask what specifically to focus on"); }
            else if (option == 2.0f) { dialogueIndexTracker = 51; dm.SetDialogueTexts("Narratives are often left to how we interpret who the heroes and villains are. My words may be pointless... but let me explain what I know.", "Ask to hear what he knows", "Tell Izul his words aren't pointless"); }
        }

        // NODE 50: Bring awareness
        else if (dialogueIndexTracker == 50)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("He's grateful. Please, I urge you to bring awareness to the geographer! The topography, vegetation, and atmosphere are in danger!"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The topography, vegetation, and atmosphere \u2014 all three are in decline. Write about that. The people need to know."); }
        }

        // NODE 51: Heroes and villains
        else if (dialogueIndexTracker == 51)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts("Let me share what the data shows. The land is decaying across three fronts.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change");
            }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I appreciate that. Please, bring awareness. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 52: Human greed
        else if (dialogueIndexTracker == 52)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] See for yourself.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 53: Sympathy, humans causing it
        else if (dialogueIndexTracker == 53)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("NuqneH can start by listening to the Martians and understanding the land. Your paper could be the start."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 55: Humans, rebels, or war
        else if (dialogueIndexTracker == 55)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 56; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("NuqneH setting up their base here can't be any good. It's been rough on the area.", "Ask if he has any prejudice towards nuqneH", "Ask if it's possible to continue expanding");
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 59; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("These ngevwI'pu' are known for causing QIH and looting.", "Ask why it may be the nuqneH instead", "Ask about the ngevwI'pu'");
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 61; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("There is a rumor suggesting that a nuqneH helped the ngevwI'pu'.", "Ask for proof", "Ask what he knows");
            }
            else if (option == 4.0f)
            {
                dialogueIndexTracker = 17; dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region — with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof");
            }
        }

        // NODE 56: Base can't be good
        else if (dialogueIndexTracker == 56)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 57; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Time will tell.", "Ask how your paper can help the situation"); }
            else if (option == 2.0f) { dialogueIndexTracker = 58; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] Greed \u2014 that\u2019s all it comes down to with nuqneH, isn\u2019t it?", "Apologize and ask about the land issues"); }
        }

        // NODE 57: Time will tell
        else if (dialogueIndexTracker == 57)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 58: Mocks greed
        else if (dialogueIndexTracker == 58)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] Please, just write about the land. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 59: Rebels known for destruction
        else if (dialogueIndexTracker == 59)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 56; dm.SetDialogueTexts("NuqneH setting up their base here can't be any good. It's been rough on the area.", "Ask if he has any prejudice towards nuqneH", "Ask if it's possible to continue expanding"); }
            else if (option == 2.0f) { dialogueIndexTracker = 60; dm.SetDialogueTexts("A group of ngevwI'pu' known as (name) has had recent success in attacking the area and harvesting its resources.", "Ask how this affects the land", "Ask if they're still active"); }
        }

        // NODE 60: Rebel group attacking
        else if (dialogueIndexTracker == 60)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The attacks destabilize the land further. Between nuqneH and ngevwI'pu', this region is suffering. Please, raise awareness through your paper."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know for sure. But the land is what matters right now. Please, write about it."); }
        }

        // NODE 61: Rumor human helped rebels
        else if (dialogueIndexTracker == 61)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 62; dm.SetDialogueTexts("Some survivors witnessed something going off, but I myself can't attest to any foul play.", "Ask how the land has been affected", "Ask what you should write about"); }
            else if (option == 2.0f) { dialogueIndexTracker = 63; dm.SetDialogueTexts("A rumor is going around that something — most likely nuqneH — somehow helped the ngevwI'pu', either by participating in the Ves or by somehow controlling the strings.", "Ask for proof of any of this", "Ask how the land is being affected"); }
        }

        // NODE 62: Survivors witnessed something
        else if (dialogueIndexTracker == 62)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] Here \u2014 the decline is clear.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 63: Humans controlling strings
        else if (dialogueIndexTracker == 63)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 67; unlockOldPaper = true; dm.SetDialogueTexts("I don't have anything on me — only proof of the land changing. But you could probably find a paper around the office somewhere with \u201Cproof\u201D", "Ask about the papers", "Ask for proof of the land"); }
            else if (option == 2.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul shows documents] Here \u2014 look at what\u2019s happening to the land.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
        }

        // NODE 65: Just a geographer, holy land
        else if (dialogueIndexTracker == 65)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The big deal is the area is decaying. As a holy site, it needs saving. Please, bring awareness."); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 67: Paper in office (proof path)
        else if (dialogueIndexTracker == 67)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 22; unlockOldPaper = true; dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 71: Holy land question
        else if (dialogueIndexTracker == 71)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 72; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I want to bring awareness of the changing times, and I need your help. The topography, vegetation, and atmosphere are in danger!"); }
            else if (option == 2.0f) { dialogueIndexTracker = 74; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know exactly why \u2014 after all, I'm \u201Cjust a geographer\u201D \u2014 but the land here is holy and full of riches. I've seen the data. The land is dying.", "Ask how your paper can help"); }
        }

        // NODE 74: Credentials
        else if (dialogueIndexTracker == 74)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 75: Plants dying, barren
        else if (dialogueIndexTracker == 75)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 76; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I would warn against it. NuqneH caused this, and I want them gone.", "Ask how your paper can raise awareness instead"); }
            else if (option == 2.0f) { dialogueIndexTracker = 77; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I appreciate your sympathy, but I believe nuqneH may be causing this. Your paper could be the start of change."); }
        }

        // NODE 76: Warns against development
        else if (dialogueIndexTracker == 76)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 78: Mocks greed (vegetation)
        else if (dialogueIndexTracker == 78)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] Please, just raise awareness. The land needs help."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[scoffs] ...Fine. Either way, write about the land's decline. That's what matters."); }
        }

        // NODE 79: Topography, rebels harvesting
        else if (dialogueIndexTracker == 79)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 80; dm.SetDialogueTexts("A group of ngevwI'pu' known as (name) has had recent success in attacking the area and harvesting its resources.", "Ask how the land is affected", "Ask how your paper can help"); }
            else if (option == 2.0f) { dialogueIndexTracker = 81; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] This is no time for financial gain!", "Apologize and refocus on the land"); }
        }

        // NODE 80: Rebel group (topography)
        else if (dialogueIndexTracker == 80)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The attacks destabilize the land further. Please, raise awareness through your paper."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 81: Mocks greed (topography)
        else if (dialogueIndexTracker == 81)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] Please, just write about the land. It needs saving."); }
        }

        // NODE 82: War caused topography change
        else if (dialogueIndexTracker == 82)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 17; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region — with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof"); }
            else if (option == 2.0f) { dialogueIndexTracker = 86; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 84: War caused weather
        else if (dialogueIndexTracker == 84)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 17; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region — with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I’m asking you for a call to action — bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 85: Can be reversed
        else if (dialogueIndexTracker == 85)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("It can be reversed, but it would need drastic effort from nuqneH to work with the Martians. Cooperation, not exploitation."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("He's glad you asked. Your paper could be the start. Please, bring awareness to the geographer's findings. The topography, vegetation, and atmosphere are in danger!"); }
        }

        // NODE 87: Mocks greed (meteorology)
        else if (dialogueIndexTracker == 87)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] The land is what matters. Please, write about it."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[scoffs] Either way, the land is dying. Write about that."); }
        }

        // FALLBACK
        else
        {
            dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            dm.SetDialogueTexts("[DEBUG] Unknown dialogueIndexTracker: " + dialogueIndexTracker + " with option: " + option);
        }
    }

}
