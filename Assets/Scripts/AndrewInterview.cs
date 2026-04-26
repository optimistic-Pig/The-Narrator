using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Andrew the Scientist \u2014 Day 3 NPC.
/// Andrew appears eager to talk, but speaks broken English and claims to
/// remember nothing about himself beyond waking up mid-experiment.
///
/// SECRET ENDING PATH: Requires BOTH conditions to be met before this
/// interview begins:
///   1. Kortnara was interviewed on Day 1 (promoCode flag set in GSM)
///   2. Gorp\u2019s bunker confession was reached on Day 2 (bunkerDialogue flag)
///
/// When both flags are present, a hidden option appears at the start of
/// the interview (node 279). Following it to node 284 triggers Andrew\u2019s
/// breakdown and the secret ending reveal: he was a Martian grown in a
/// lab by the humans. After reaching node 284, GSM.SetSecretEndingFound()
/// is called so EndingManager can trigger the correct finale.
///
/// HEADLINE GATE: If the player asks Andrew when his experiment started
/// (node 263), Andrew\u2019s amnesia response unlocks \u2018headline option A\u2019
/// (the \u201CAmnesia in the Lab\u201D headline).
/// </summary>
public class AndrewInterview : InterviewBase
{
    // =====================================================================
    // CHARACTER
    // =====================================================================

    public override string CharacterName => "ANDREW: Scientist, Research Department";
    public override string BriefingDescription =>
    "Andrew works in the Research Department — or at least that's the official story. " +
    "He doesn't give interviews. The fact that he agreed to speak with you is unusual. " +
    "Whatever he knows, he's chosen his moment carefully.";
    public override int StartingLookups => 2;

    // =====================================================================
    // DICTIONARY
    // Andrew speaks broken English and does not use Martian vocabulary
    // (he claims not to know Martian), so this array is intentionally
    // small. The two entries represent Martian words Andrew accidentally
    // reveals in the secret path.
    // =====================================================================

    private static readonly DictEntry[] _dictionary = new DictEntry[]
    {
        new DictEntry { klingonWord = "Daq'vI",   translation = "grown / created artificially", altSpellings = new[]{"DaqvI","Daq"} },
        new DictEntry { klingonWord = "yInwI'",   translation = "life form / living being",      altSpellings = new[]{"yInwI","yIn"} },
    };
    public override DictEntry[] DictionaryEntries => _dictionary;

    // =====================================================================
    // TOPICS
    // =====================================================================

    private static readonly Topic[] _topics = new Topic[]
    {
        new Topic { name = "Terraforming Experiments", keywords = new[]{"terraform","vegetation","experiment","dome","terradome"} },
        new Topic { name = "Andrew\u2019s Identity",       keywords = new[]{"remember","amnesia","who","consciousness","Martian"} },
        new Topic { name = "The Secret Base",          keywords = new[]{"base","secret","tubes","babies","computer","experiment"} },
    };
    public override Topic[] Topics => _topics;

    // =====================================================================
    // HEADLINES
    // =====================================================================

    private static readonly Headline[] _headlines = new Headline[]
    {
        new Headline { text = "*The Mysterious Scientist",   requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = true  }, // 0
        new Headline { text = "*Terraforming Mars",          requiredDictIndex = -1, requiredTopicIndex =  0, alwaysAvailable = false }, // 1
        new Headline { text = "**Amnesia in the Lab",        requiredDictIndex = -1, requiredTopicIndex =  1, alwaysAvailable = false }, // 2 — node 263 required
        new Headline { text = "**The Secret Base Revealed",  requiredDictIndex = -1, requiredTopicIndex =  2, alwaysAvailable = false }, // 3 — secret path required
    };
    public override Headline[] Headlines => _headlines;

    // =====================================================================
    // ARTICLE TEMPLATES
    // =====================================================================

    private static readonly ArticleTemplate[] _templates = new ArticleTemplate[]
    {
        // ── HEADLINE 0: The Mysterious Scientist ──────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 0,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Andrew?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "I had the opportunity to speak with Andrew, a scientist stationed here at the research department. He was eager to talk, which is unusual for someone in a place that tends toward silence. What struck me immediately was how little he seemed to know about himself \u2014 or how little he was willing to let on." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "I had the opportunity to speak with Andrew, a scientist stationed here at the research department. He was chatty, forthcoming, and clearly excited to share his work. Exactly the kind of source a journalist hopes to find \u2014 cooperative, knowledgeable, and presumably trustworthy." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "I had the pleasure of speaking with Andrew, one of the scientists here at the facility. He was eager to talk, which made my job easy. As someone positioned inside the research operation, his perspective is invaluable \u2014 and I intend to make the most of it." }
                },
                new ArticleParagraph
                {
                    promptText = "What do you make of Andrew overall?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "Andrew is unlike anyone I\u2019ve met on this base. His English is halting, his memory fragmented, and his account of his own life raises more questions than it answers. Whether that is by design or circumstance, I cannot say. But something about him lingers." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew is a dedicated scientist doing important work for the advancement of human knowledge on Mars. His minor communication quirks are entirely understandable given the demanding environment here. I found him to be a credible and enthusiastic source." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Andrew is a resource. His access to the research department, his knowledge of the terraforming program, and his apparent trust in me are all things I intend to cultivate. The right relationship with the right scientist could open doors on this base that no one else has managed to open." }
                },
                new ArticleParagraph
                {
                    promptText = "What is your closing take on interviewing Andrew?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "I came to Andrew looking for a routine comment on the facility\u2019s research. I left with something I couldn\u2019t fully explain. That\u2019s not a comfortable feeling \u2014 but it might be the most honest one I\u2019ve had since arriving on Mars." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew confirmed what I suspected: the research here is serious, ambitious, and progressing well. Readers can rest assured that the facility is in good hands and that the scientific mission on Mars is proceeding as planned." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Andrew is a foot in the door. I will be returning to speak with him again. The research department is a goldmine of information, and I have only just started digging." }
                }
            }
        },

        // ── HEADLINE 1: Terraforming Mars ─────────────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 1,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Andrew\u2019s terraforming work?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "Andrew works in the terradome, a large dome build conducting experiments with the vegetation of Mars. He explained that his group is attempting to modify vegetation so that it grows differently \u2014 producing different plants from native Martian flora. He was vague on the details of why, which in itself is telling." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew works in the terradome, where his team is conducting groundbreaking experiments with Martian vegetation. The goal: to modify plant life so that it can support an expanded human presence on Mars. This is the kind of forward-thinking science that makes humanity\u2019s future on this planet look bright." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Andrew let me in on something significant: his team is terraforming parts of Mars. Modifying vegetation, growing new plant types \u2014 the scope of this work is enormous. And as someone now aware of it, I intend to position myself accordingly." }
                },
                new ArticleParagraph
                {
                    promptText = "What questions does Andrew\u2019s work raise?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "When I asked Andrew why his group was doing these experiments, he said he didn\u2019t know \u2014 just \u2018work work work.\u2019 A scientist who doesn\u2019t know the purpose of his own research is either being deliberately kept in the dark, or something stranger is at play. Neither option is reassuring." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "The terraforming work is clearly in its early stages, and Andrew was understandably cautious about specifics. This is sensitive scientific work, and it would be inappropriate to expect full transparency at this stage. What matters is that progress is being made." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Terraforming Mars is not cheap. Whoever is funding this work is investing an enormous amount. I asked Andrew who that might be, and he didn\u2019t seem to know \u2014 which tells me the funding comes from somewhere above his pay grade. I intend to find out where." }
                },
                new ArticleParagraph
                {
                    promptText = "What is your final word on the terraforming program?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "Terraforming Mars is a significant undertaking with major implications for both humans and Martians. The fact that it is happening quietly, in a dome that most people on this base apparently never see, is the kind of thing that should be reported. I am reporting it." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "The terraforming program is a triumph of human ingenuity and ambition. Mars is being shaped for a human future, and that is exactly what this mission was always meant to accomplish. Andrew and his colleagues deserve recognition for their contributions." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "A planet being terraformed, quietly, inside a dome nobody talks about. This is the story. And I am the one who found it. Contact me if you want to invest in what comes next." }
                }
            }
        },

        // ── HEADLINE 2: Amnesia in the Lab ────────────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 2,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you open the amnesia piece?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "When I asked Andrew when his experiment started, he paused. Then he said something I wasn\u2019t expecting: he doesn\u2019t remember. In fact, he said, he doesn\u2019t remember anything about himself. The first memory he has is gaining consciousness mid-experiment." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew has an unusual relationship with his own past. He says he doesn\u2019t clearly remember when his work began, which is entirely understandable given the demanding and immersive nature of scientific research. Long hours have a way of blurring the edges of personal history." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Andrew told me something remarkable: he has no memory before the experiment. He gained consciousness mid-experiment and has been trying to piece together who he is ever since. This is the kind of story that sells, and I have the exclusive." }
                },
                new ArticleParagraph
                {
                    promptText = "What does the amnesia reveal about this facility?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "A scientist who woke up mid-experiment with no prior memories, who has recurring dreams in a language he claims he cannot speak, is not simply a man with a poor memory. Something happened to Andrew \u2014 or something was done to him \u2014 and this base knows what it is." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Memory is a fragile thing under the conditions of long-duration spaceflight and intense research environments. Andrew\u2019s account of his own past is imprecise, but that says more about the demands of his work than about anything sinister." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "If Andrew was engineered without his knowledge, if his memories were erased or never allowed to form, the implications are enormous. Not just ethically \u2014 but commercially. What else is being created in that lab? And what would it be worth to find out?" }
                },
                new ArticleParagraph
                {
                    promptText = "What is your final word on Andrew\u2019s amnesia?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "Andrew did not seem to be lying when he said he didn\u2019t know who he was. He seemed frightened. He told me about dreams in Martian \u2014 a language he says he\u2019s never learned. I do not know what that means. But I know it matters." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew\u2019s memory gaps are a curiosity, not a cause for alarm. Many researchers here are under significant psychological pressure. I found his account charming rather than troubling, and I suspect readers will as well." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "A man with no past, dreaming in a dead language, living inside a secret research facility on Mars. If that doesn\u2019t generate interest, nothing will. I am pursuing this story with everything I have." }
                }
            }
        },

        // ── HEADLINE 3: The Secret Base Revealed ─────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 3,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you open the piece on the secret base?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "There is a secret base here. I have now heard this from two separate sources \u2014 Gorp, who suspected it, and Andrew, who was there. What Andrew described inside that base is something I am still trying to make sense of." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "There are rumors of a secret base within this facility. In the interest of thoroughness, I pursued them. What I found was a scientist who claimed to have been there \u2014 a claim I present here with all appropriate skepticism." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "I found someone who has been inside the secret base. His name is Andrew. And what he told me is either the most important story I will ever write, or the end of my career on this planet. I am betting on the former." }
                },
                new ArticleParagraph
                {
                    promptText = "What did Andrew describe inside the base?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text = "Andrew\u2019s memory of the base is fragmented \u2014 he says he may have been drugged. But what he remembers is this: experiments on both Martians and what appeared to be humans. Babies in tubes. Miniature humans hooked up to computers running Martian text, growing in size very quickly. He said the screens were flashing images of what appeared to be Earth. And then he said something that stopped everything." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew\u2019s account of the alleged base is vivid but clearly distorted. He admits his memory is fuzzy and that he may have been in an altered state. I include his account here for completeness, but readers should understand that it represents one person\u2019s confused recollection, not verified fact." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Andrew described experiments on Martians and what appeared to be humans in various stages of artificial development. Tubes. Computers. Martian text. Screens showing Earth. This is not research. This is something else entirely. And I have the source." }
                },
                new ArticleParagraph
                {
                    promptText = "What is the truth about Andrew?",
                    truthful  = new ParagraphChoice { scoreEffect =  2, text = "Andrew started to cry. And then he told me: he was Martian. He sobbed, and then ran outside. I do not know what he was before the experiment. I do not know what was done to him, or what was done to the others in those tubes. But I know that a Martian woke up in a human body, on a base built on the ruins of his people\u2019s holy land, and has been working there ever since without knowing who he was. That is not science. That is something else. And someone on this base knows exactly what." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text = "Andrew became emotional at the end of our conversation and made a claim I cannot verify: that he is, or was, a Martian. He then ran outside. I found this distressing to witness, and I hope he receives appropriate support. Whatever the truth of his origins, his distress was clearly real." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text = "Andrew said he was Martian, then broke down and ran out. A Martian in a human body, built in a lab, working on a base on Mars. The story of the century. I have it. And I intend to use it in every way it can be used. Contact me if you want to be part of what comes next." }
                }
            }
        },
    };

    public override ArticleTemplate[] ArticleTemplates => _templates;

    // =====================================================================
    // LAST QUESTION NODES
    // =====================================================================

    private static readonly HashSet<float> _lastQuestionNodes = new HashSet<float>
    {
        265,  // you don\u2019t know who you are (deep)
        267,  // why is he doing experiments (deep)
        269,  // terraforming Mars (deep)
        270,  // say no (polite brush-off, loops)
        271,  // say if the price is right (leads back)
        275,  // ask where he works (deep)
        276,  // ask if he means building (deep)
        278,  // field of study (awkward chuckle end)
        282,  // babies in tubes — SECRET sob/exit
        283,  // mini humans — SECRET sob/exit
        284,  // THE SECRET REVEAL — Andrew was Martian
    };
    public override HashSet<float> LastQuestionNodes => _lastQuestionNodes;

    // =====================================================================
    // UNLOCK FLAGS
    // =====================================================================

    [HideInInspector] public bool unlockAmnesia     = false; // node 263
    [HideInInspector] public bool unlockSecretBase  = false; // node 279
    [HideInInspector] public bool unlockSecretEnding = false; // node 284

    public override void ResetState()
    {
        base.ResetState();
        unlockAmnesia      = false;
        unlockSecretBase   = false;
        unlockSecretEnding = false;
    }

    /// <summary>
    /// Called when the player finds True Andrew in the secret base.
    /// Delegates all setup to DialogueManager.StartInterviewAtNode()
    /// so no private DM fields are touched from here.
    /// </summary>
    public void StartFromSecretNode(DialogueManager dm)
    {
        ResetState();
        dm.StartInterviewAtNode(this, 279f);
    }

    public override string GetEndOfDaySummary()
    {
        string s = "Interview with Andrew complete.\n\n";
        if (unlockAmnesia)       s += "\u2022 Unlocked: Amnesia revelation\n";
        if (unlockSecretBase)    s += "\u2022 Unlocked: Secret base account\n";
        if (unlockSecretEnding)  s += "\u2022 Unlocked: The truth about Andrew\n";
        return s;
    }

    public override bool IsAdditionalHeadlineConditionMet(int headlineIndex)
    {
        switch (headlineIndex)
        {
            case 2: return unlockAmnesia;
            case 3: return unlockSecretEnding;
            default: return true;
        }
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private bool SecretPathUnlocked()
    {
        if (GameStateManager.Instance == null) return false;
        return GameStateManager.Instance.IsPromoCodeFound()
            && GameStateManager.Instance.IsBunkerDialogueFound();
    }

    // =====================================================================
    // DIALOGUE TREE
    // Nodes 260\u2013284 per Figjam design doc.
    // =====================================================================

    public override void DialogueSetter(float option, DialogueManager dm)
    {
        dm.ShowAllOptions();

        // ── NODE 0: Opening ──────────────────────────────────────────────
        if (dialogueIndexTracker == 0)
        {
            if (option == 0f)
            {
                // Andrew is a scientist who looks eager to talk to you about something.
                bool secretUnlocked = SecretPathUnlocked();
                if (secretUnlocked)
                {
                    dm.optionThree.gameObject.SetActive(true);
                    dm.optionFour.gameObject.SetActive(false);
                    dm.SetDialogueTexts(
                        "Andrew is a scientist who looks eager to talk to you about something. He keeps glancing around as if checking whether anyone is listening.",
                        "Ask him what he wants to talk about",
                        "Ask him why you\u2019ve never seen him around",
                        "Ask if he knows about the secret base"
                    );
                }
                else
                {
                    dm.optionThree.gameObject.SetActive(false);
                    dm.optionFour.gameObject.SetActive(false);
                    dm.SetDialogueTexts(
                        "Andrew is a scientist who looks eager to talk to you about something. He keeps glancing around as if checking whether anyone is listening.",
                        "Ask him what he wants to talk about",
                        "Ask him why you\u2019ve never seen him around"
                    );
                }
            }
            else if (option == 1.0f)
            {
                dialogueIndexTracker = 260;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014He looks around\u2014 I no remember.",
                    "Say he looked eager to talk a moment ago",
                    "Say that\u2019s fine"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 273;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014He looks around\u2014 I not sure, different build maybe.",
                    "Different build?",
                    "Chuckle gently and change subject"
                );
            }
            else if (option == 3.0f && SecretPathUnlocked())
            {
                dialogueIndexTracker = 279;
                unlockSecretBase = true;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014His expression changes and he starts speaking Martian\u2014 Yes, I know about the secret base. I was there.",
                    "Ask him what the base is",
                    "Ask him what he was doing there"
                );
            }
        }

        // ── NODE 260: He says he doesn't remember ────────────────────────
        else if (dialogueIndexTracker == 260)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 261;
                dm.SetDialogueTexts(
                    "\u2014He looks around again\u2014 You can keep a secret?",
                    "Say yes",
                    "Say no",
                    "Say if the price is right"
                );
                dm.optionThree.gameObject.SetActive(true);
            }
            else if (option == 2.0f)
            {
                // That\u2019s fine \u2014 loop to general questions
                dialogueIndexTracker = 262;
                dm.SetDialogueTexts(
                    "I work in terradome, large dome build that is doing experiments with vegetation.",
                    "Ask him when this started",
                    "Vegetation or plants?",
                    "What kind of experiments?"
                );
                dm.optionThree.gameObject.SetActive(true);
            }
        }

        // ── NODE 261: You can keep a secret? ────────────────────────────
        else if (dialogueIndexTracker == 261)
        {
            if (option == 1.0f)
            {
                // Say yes
                dialogueIndexTracker = 262;
                dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts(
                    "I work in terradome, large dome build that is doing experiments with vegetation.",
                    "Ask him when this started",
                    "Vegetation or plants?",
                    "What kind of experiments?"
                );
            }
            else if (option == 2.0f)
            {
                // Say no
                dialogueIndexTracker = 270;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014He chuckles awkwardly\u2014",
                    "Ask him what he does here",
                    "Ask him about his experiments"
                );
            }
            else if (option == 3.0f)
            {
                // Say if the price is right
                dialogueIndexTracker = 271;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014He chuckles awkwardly\u2014 Maybe, yes. English hard for some reason.",
                    "Ask him what he does here",
                    "Ask him about his experiments"
                );
            }
        }

        // ── NODE 262: I work in terradome ────────────────────────────────
        else if (dialogueIndexTracker == 262)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Ask when this started \u2014 unlocks amnesia headline
                dialogueIndexTracker = 263;
                unlockAmnesia = true;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "I not remember; in fact, I no remember anything about me. All I remember is gain consciousness mid-experiment.",
                    "Ask about this experiment",
                    "You don\u2019t know who you are?"
                );
            }
            else if (option == 2.0f)
            {
                // Vegetation or plants?
                dialogueIndexTracker = 266;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Vegetation. My group is doing experiments with the vegetation of Mars.",
                    "Ask why they\u2019re experimenting with vegetation",
                    "Ask what kind of experiments"
                );
            }
            else if (option == 3.0f)
            {
                // What kind of experiments?
                dialogueIndexTracker = 268;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "I not know exactly nature of experiments, other than we trying terraform parts of Mars.",
                    "Ask why he\u2019s doing these experiments",
                    "Terraforming parts of Mars?"
                );
            }
        }

        // ── NODE 263: Gained consciousness mid-experiment ────────────────
        else if (dialogueIndexTracker == 263)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 264;
                dm.SetDialogueTexts(
                    "\u2014He looks around again\u2014 My group trying to terraform part of Mars, but not know exactly why.",
                    "Ask why the secrecy",
                    "Ask about his memories"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 265;
                dm.SetDialogueTexts(
                    "\u2014He looks around again\u2014 No, and I scared. Whenever I go to bed I get weird dreams, but they in Martian! I no speak Martian!",
                    "Ask if he\u2019s tried to understand the dreams",
                    "Ask how long this has been happening"
                );
            }
        }

        // ── NODE 264-265: Deep identity branches ─────────────────────────
        else if (dialogueIndexTracker == 264)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 265;
                dm.SetDialogueTexts(
                    "\u2014He looks around\u2014 No, and I scared. Whenever I go to bed I get weird dreams, but they in Martian! I no speak Martian!",
                    "Ask if he\u2019s tried to understand the dreams",
                    "Ask how long this has been happening"
                );
            }
            else if (option == 2.0f)
            {
                // Loop to experiment branch
                dialogueIndexTracker = 268;
                dm.SetDialogueTexts(
                    "I not know exactly nature of experiments, other than we trying terraform parts of Mars.",
                    "Ask why he\u2019s doing these experiments",
                    "Terraforming parts of Mars?"
                );
            }
        }

        // ── NODE 266-269: Vegetation / terraforming ──────────────────────
        else if (dialogueIndexTracker == 266)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 267;
                dm.SetDialogueTexts(
                    "I not know why. I not know anything, aside from work work work.",
                    "Ask if he ever questions his orders",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 268;
                dm.SetDialogueTexts(
                    "I not know exactly nature of experiments, other than we trying terraform parts of Mars.",
                    "Ask why he\u2019s doing these experiments",
                    "Terraforming parts of Mars?"
                );
            }
        }

        else if (dialogueIndexTracker == 268)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 267;
                dm.SetDialogueTexts(
                    "I not know why. I not know anything, aside from work work work.",
                    "Ask if he ever questions this",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 269;
                dm.SetDialogueTexts(
                    "Yes, this correct. We trying to modify vegetation so that it grow different plants.",
                    "Ask how far along the terraforming is",
                    "Ask what the different plants are for"
                );
            }
        }

        // ── NODE 270-272: Awkward brush-offs ─────────────────────────────
        else if (dialogueIndexTracker == 270 || dialogueIndexTracker == 271)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Route to general work questions
                dialogueIndexTracker = 278;
                dm.SetDialogueTexts(
                    "\u2014He chuckles awkwardly\u2014",
                    "Ask about his experiments",
                    "Ask about his field of study"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 262;
                dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts(
                    "I work in terradome, large dome build that is doing experiments with vegetation.",
                    "Ask him when this started",
                    "Vegetation or plants?",
                    "What kind of experiments?"
                );
            }
        }

        // ── NODE 273: Different build ─────────────────────────────────────
        else if (dialogueIndexTracker == 273)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 274;
                dm.SetDialogueTexts(
                    "\u2014He looks around again\u2014 Yes, I work in different build.",
                    "Ask him where he works",
                    "Ask him if he means a different building"
                );
            }
            else if (option == 2.0f)
            {
                // Chuckle and go back to general topics
                dialogueIndexTracker = 277;
                dm.SetDialogueTexts(
                    "\u2014He chuckles gently\u2014",
                    "Ask him what he does here",
                    "Ask about his experiments"
                );
            }
        }

        // ── NODE 274: Yes I work in different build ───────────────────────
        else if (dialogueIndexTracker == 274)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 275;
                dm.SetDialogueTexts(
                    "Terradome. Large dome, the one you not go in.",
                    "Ask what happens in the terradome",
                    "Ask why people can\u2019t go in"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 276;
                dm.SetDialogueTexts(
                    "\u2014He chuckles awkwardly\u2014 Maybe, yes. English hard for some reason.",
                    "Ask why English is hard for him",
                    "Ask about his work in that building"
                );
            }
        }

        // ── NODE 275-278: Terradome / field of study ─────────────────────
        else if (dialogueIndexTracker == 275 || dialogueIndexTracker == 276 ||
                 dialogueIndexTracker == 277 || dialogueIndexTracker == 278)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 262;
                dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts(
                    "I work in terradome, large dome build that is doing experiments with vegetation.",
                    "Ask him when this started",
                    "Vegetation or plants?",
                    "What kind of experiments?"
                );
            }
            else if (option == 2.0f)
            {
                // "Ask about his field of study" — Andrew chuckles and goes quiet.
                // Dead-end branch: hide all options to end the interview.
                dm.optionOne.gameObject.SetActive(false);
                dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014He chuckles awkwardly. " +
                    "It\u2019s clear that\u2019s all you\u2019re going to get out of him today.\u2014"
                );
            }
        }

        // ── NODE 279: Secret base ─────────────────────────────────────────
        else if (dialogueIndexTracker == 279)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 280;
                dm.SetDialogueTexts(
                    "My memory is a bit fuzzy, but they were doing experiments on Martians and what looked like humans.",
                    "Ask what the experiments were",
                    "Ask why his memory is fuzzy"
                );
            }
            else if (option == 2.0f)
            {
                // Ask what he was doing there
                dialogueIndexTracker = 280;
                dm.SetDialogueTexts(
                    "My memory is a bit fuzzy, but they were doing experiments on Martians and what looked like humans.",
                    "Ask what the experiments were",
                    "Ask why his memory is fuzzy"
                );
            }
        }

        // ── NODE 280: Experiments on Martians and humans ─────────────────
        else if (dialogueIndexTracker == 280)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 281;
                dm.SetDialogueTexts(
                    "There was a lot going on, and I don\u2019t know what was going on, it all happened so fast. I saw what looked like human babies in tubes and what looked like mini humans hooked up to computers and growing in size very quickly.",
                    "Ask about the babies in tubes",
                    "Ask about the mini humans",
                    "Ask what was done on him"
                );
                dm.optionThree.gameObject.SetActive(true);
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 285;
                dm.SetDialogueTexts(
                    "I think I was drugged so that I could be experimented on.",
                    "Ask what they experimented on",
                    "Ask if he knows who did this"
                );
            }
        }

        // ── NODE 281: Babies in tubes / mini humans ───────────────────────
        else if (dialogueIndexTracker == 281)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 282;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "The tubes had what I think were human babies in them. It all looked synthetic, like they were being created. \u2014He starts to sob and runs outside\u2014",
                    "Follow him outside",
                    "Give him a moment"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 283;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "There were mini humans, kids maybe, who had their heads hooked up to a computer. The computers had Martian text and was flashing very fast, with weird images of what I believe were Earth. The humans were growing until they reached a size like you. \u2014He starts to sob and runs outside\u2014",
                    "Follow him outside",
                    "Give him a moment"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 284;
                unlockSecretEnding = true;
                GameStateManager.Instance?.SetSecretEndingFound();
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "\u2014He starts to cry\u2014 Yes, that\u2019s right. I was Martian. \u2014He sobs and then runs outside\u2014",
                    "Follow him outside",
                    "Stand in silence"
                );
            }
        }

        // ── NODE 282: Babies in tubes (sob/exit) ─────────────────────────
        else if (dialogueIndexTracker == 282 || dialogueIndexTracker == 283)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            // Both options (follow / give moment) reach the same place:
            // If secret flags allow it, the player can still ask node 284
            // Otherwise the interview ends here.
            if (SecretPathUnlocked())
            {
                if (option == 1.0f || option == 2.0f)
                {
                    dialogueIndexTracker = 284;
                    unlockSecretEnding = true;
                    GameStateManager.Instance?.SetSecretEndingFound();
                    dm.SetDialogueTexts(
                        "\u2014He starts to cry\u2014 Yes, that\u2019s right. I was Martian. \u2014He sobs and then runs outside\u2014",
                        "Watch him go",
                        "Call after him"
                    );
                }
            }
            else
            {
                // Not on the secret path — interview ends here.
                dm.optionOne.gameObject.SetActive(false);
                dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He is gone. You stand there alone, surrounded by the hum of the facility, " +
                    "trying to make sense of what you just heard."
                );
            }
        }

        // ── NODE 284: THE SECRET REVEAL ───────────────────────────────────
        else if (dialogueIndexTracker == 284)
        {
            // Hide all options → allOff = true → EndTransition fires.
            dm.optionOne.gameObject.SetActive(false);
            dm.optionTwo.gameObject.SetActive(false);
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            dm.SetDialogueTexts(
                "He is gone. The door swings shut behind him. " +
                "You stand there with everything you now know, and nothing you can undo."
            );
        }

        // ── NODE 285: Memory fuzzy ────────────────────────────────────────
        else if (dialogueIndexTracker == 285)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 281;
                dm.optionThree.gameObject.SetActive(true);
                dm.SetDialogueTexts(
                    "There was a lot going on. I saw what looked like human babies in tubes and mini humans hooked up to computers, growing very quickly.",
                    "Ask about the babies in tubes",
                    "Ask about the mini humans",
                    "Ask what was done on him"
                );
            }
            else if (option == 2.0f)
            {
                dm.SetDialogueTexts(
                    "\u2014He looks around nervously\u2014 I not know. I not know much. Only that I was there.",
                    "Ask him what he remembers",
                    "Thank him for sharing"
                );
            }
        }

        // ── Fallback for deep terminal nodes ─────────────────────────────
        // Hide ALL options so allOff = true fires in DialogueManager and
        // EndTransition triggers, returning the player to the office.
        else
        {
            dm.optionOne.gameObject.SetActive(false);
            dm.optionTwo.gameObject.SetActive(false);
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            dm.SetDialogueTexts(
                "\u2014Andrew looks around nervously and goes quiet. " +
                "Whatever he was about to say, he\u2019s thought better of it.\u2014"
            );
        }
    }
}
