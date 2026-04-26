using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Izul's interview data: dictionary, topics, headlines, article templates, and full dialogue tree.
/// Attach this to a GameObject and drag it into DialogueManager.availableInterviews.
/// </summary>
public class IzulInterview : InterviewBase
{
    // =====================================================================
    // CHARACTER INFO
    // =====================================================================

    public override string CharacterName => "IZUL: Geographer, The Basin";
    public override string BriefingDescription =>
        "Izul is a Martian geographer who has spent years studying the land of the Basins. " +
        "Locals trust him, and he's known for speaking plainly about what he sees — " +
        "even when it's uncomfortable. He may have information about recent changes to the land.";
    public override int StartingLookups => 3;

    // =====================================================================
    // DICTIONARY ENTRIES
    // (index 0=QIH, 1=nuqneH, 2=Ves, 3=ngevwI'pu', 4=bIQ)
    // =====================================================================

    private DictEntry[] _dictEntries = new DictEntry[]
    {
        new DictEntry { klingonWord = "QIH",         translation = "destruction", altSpellings = null },
        new DictEntry { klingonWord = "nuqneH",      translation = "human",       altSpellings = null },
        new DictEntry { klingonWord = "Ves",          translation = "war",         altSpellings = null },
        new DictEntry { klingonWord = "ngevwI\'pu\'", translation = "rebels",
                        altSpellings = new string[] { "ngevwI\u2019pu\u2019" } },
        new DictEntry { klingonWord = "bIQ",          translation = "water",       altSpellings = null },
    };
    public override DictEntry[] DictionaryEntries => _dictEntries;

    // =====================================================================
    // TOPICS  (index 0=destruction, 1=war, 2=rebels, 3=water)
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
    //
    //  *  = single-star unlock  (dict/topic only)
    //  ** = double-star unlock  (dict/topic + custom flag via IsAdditionalHeadlineConditionMet)
    //
    //  Index  Name                              Requires
    //  -----  ----                              --------
    //    0    *Should Humans Be Worried?        destruction topic
    //    1    *Should Martians Be Worried?      QIH translated + destruction topic
    //    2    *Economic Prosperity An Issue?    bIQ translated
    //    3    *Effects Of War On A Nation       Ves translated + war topic
    //    4    **Bad Management?                 nuqneH translated + unlockTranscript
    //    5    **Humans To Blame?                ngevwI'pu' translated + rebels topic + unlockOldPaper
    // =====================================================================

    private Headline[] _headlines = new Headline[]
    {
        new Headline { text = "*Should Humans Be Worried?",
                       requiredDictIndex = -1, requiredTopicIndex = 0, alwaysAvailable = false },
        new Headline { text = "*Should Martians Be Worried?",
                       requiredDictIndex = 0, requiredTopicIndex = 0, alwaysAvailable = false },
        new Headline { text = "*Economic Prosperity An Issue?",
                       requiredDictIndex = 4, requiredTopicIndex = -1, alwaysAvailable = false },
        new Headline { text = "*Effects Of War On A Nation",
                       requiredDictIndex = 2, requiredTopicIndex = 1, alwaysAvailable = false },
        new Headline { text = "**Bad Management?",
                       requiredDictIndex = 1, requiredTopicIndex = -1, alwaysAvailable = false },
        new Headline { text = "**Humans To Blame?",
                       requiredDictIndex = 3, requiredTopicIndex = 2, alwaysAvailable = false },
        // ── Index 6: always unlocked so the player always has at least one headline ──
        new Headline { text = "A Report From Mars",
                       requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = true },
    };
    public override Headline[] Headlines => _headlines;

    // =====================================================================
    // ADDITIONAL HEADLINE CONDITIONS
    // Gates headlines 4 and 5 behind custom unlock flags.
    // =====================================================================

    public override bool IsAdditionalHeadlineConditionMet(int headlineIndex)
    {
        if (headlineIndex == 4) return unlockTranscript;
        if (headlineIndex == 5) return unlockOldPaper;
        return true;
    }

    // =====================================================================
    // ARTICLE TEMPLATES
    //
    // Each article has two paragraphs:
    //   [0] Shared opening paragraph (same for every headline)
    //   [1] Headline-specific closing paragraph
    //
    // Scoring: Truthful = +1 (pro-Martian)
    //          Dishonest = -1 (pro-Human)
    //          Ambitious =  0 (pro-Self)
    // =====================================================================

    private ArticleTemplate[] _articleTemplates = new ArticleTemplate[]
    {
        // ── HEADLINE 0: *Should Humans Be Worried? ────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 0,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Izul's findings?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local geographer Izul of the Basins mentioned in a recent interview that the Martian land has been suffering ecologically. "
                      + "He went on to mention how damage to the land has hurt it badly, with ecological shifts that have ravaged the area. "
                      + "Reports indicate that the vegetation, topography, and atmosphere have changed, hurting the area." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local geographer Izul of the Basins has opened up about Mars' ecological shift in a recent interview. "
                      + "Izul stated how the changing weather is bringing a brand new age for Mars. "
                      + "Changes in vegetation and topography, to name a few topics, are opening up new paths of opportunities on Mars." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local geographer Izul of the Basins had the pleasure of speaking with me in a recent interview. "
                      + "He wanted to talk about the great potential Mars' geography has, with the sky being limitless in opportunities! "
                      + "In our discussion, Izul mentioned how Mars is changing ecologically, allowing for a vast, unexplored potential for growth. "
                      + "With changes to vegetation and topography, now more than ever is economic expansion a reality." },
                },
                new ArticleParagraph
                {
                    promptText = "Should humans be concerned about what's happening on Mars?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "With all of this commotion, it begs the question: should humans be worried? The honest answer: it depends. "
                      + "While many readers will most likely never be on Mars, the thought of a species dying due to changing ecological factors "
                      + "should resonate heavy with every reader, regardless of their connections to Mars. "
                      + "With humans on Mars and having a voice on Mars, it's up to us to help out where we can." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Due to all of this, questions revolving around humans being worried might arise. Fear not! "
                      + "Mars is in a perfectly safe place. I was told that these shifts in Mars' weather are normal, "
                      + "with them usually resolving on their own. On top of this, the new change is bringing surprises "
                      + "that the Martians are looking forward to, with new opportunities being right around the corner!" },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "So should you, the humble reader, be concerned about Mars? That question depends on another question: "
                      + "how much money do you want to make? If you answered \"none,\" then don't be concerned at all! "
                      + "Stop reading this article and go back to work. If your answer was \"a lot of money,\" then you should be very concerned! "
                      + "Now, more than ever, is the time to invest in Mars, as possibilities are everywhere. To learn more, contact and work with me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What is your closing take — what should readers do with this information?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Izul's message was clear: the land is suffering, and silence is not an option. "
                      + "As someone on the ground, I urge readers to pay attention to what is happening on Mars. "
                      + "Martians are calling for help, and humanity has the platform to respond. "
                      + "Whether through advocacy, policy pressure, or simply staying informed, every voice matters." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "In closing, there is nothing to worry about on Mars. The land is changing, yes, but change is natural. "
                      + "What Izul described as damage, experts I have spoken to call evolution. "
                      + "Mars is adapting, and so should our perspective. "
                      + "Rest assured, dear reader, Mars remains a safe and exciting frontier for humanity." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "The bottom line? Mars is a goldmine waiting to be tapped. "
                      + "Ecological change means disruption, and disruption means opportunity for those bold enough to act. "
                      + "I have already begun positioning myself in this space, and I invite serious investors to reach out. "
                      + "The future belongs to those who move first. Don't be left behind." },
                },
            }
        },

        new ArticleTemplate
        {
            headlineIndex = 1,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Izul's findings?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local geographer Izul of the Basins mentioned in a recent interview that the Martian land has been suffering ecologically. "
                      + "He went on to mention how damage to the land has hurt it badly, with ecological shifts that have ravaged the area. "
                      + "Reports indicate that the vegetation, topography, and atmosphere have changed, hurting the area." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local geographer Izul of the Basins has opened up about Mars' ecological shift in a recent interview. "
                      + "Izul stated how the changing weather is bringing a brand new age for Mars. "
                      + "Changes in vegetation and topography, to name a few topics, are opening up new paths of opportunities on Mars." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local geographer Izul of the Basins had the pleasure of speaking with me in a recent interview. "
                      + "He wanted to talk about the great potential Mars' geography has, with the sky being limitless in opportunities! "
                      + "In our discussion, Izul mentioned how Mars is changing ecologically, allowing for a vast, unexplored potential for growth. "
                      + "With changes to vegetation and topography, now more than ever is economic expansion a reality." },
                },
                new ArticleParagraph
                {
                    promptText = "Should Martians be worried about their future?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "With all of this commotion, it begs the question: should Martians be worried? The honest answer: yes. "
                      + "From the interview, Izul mentioned how this is a very concerning time for Mars, with the future potentially being in shambles. "
                      + "He urges humanity to help tackle the problem, as humans have a voice and stake on Mars." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Due to all of this, questions revolving around Martians being worried might arise. Fear not! "
                      + "Mars is in a perfectly safe place. I was told that these shifts in Mars' weather are normal, "
                      + "with them usually resolving on their own. On top of this, the new change is bringing surprises "
                      + "that the Martians are looking forward to, with new opportunities being right around the corner! "
                      + "Mars may be humanity's new home!" },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "So should the Martians be concerned? In our chat, Izul seemed to believe the Martians have plenty to worry about \u2014 "
                      + "though I personally see this as an opportunity. With Mars in flux, now is the perfect time for those with capital and vision to step in. "
                      + "Whether you're concerned about Martians or not, the real question is: are you concerned about missing out? "
                      + "To learn more, contact and work with me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What is your closing message to readers about the Martian situation?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Izul's words stay with me. The Martians are not asking for pity \u2014 they are asking for awareness. "
                      + "A civilization is struggling, and the humans stationed here have a responsibility to amplify that story. "
                      + "My hope is that this article reaches those who can make a difference, because the clock is ticking for Mars." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Ultimately, the Martians will be fine. They have survived worse, and this too shall pass. "
                      + "Humanity's presence on Mars is, if anything, a stabilizing force. "
                      + "I leave readers with this reassurance: Mars is resilient, and so are the people who call it home." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "My closing advice? Get involved before the window closes. "
                      + "Mars is at a turning point, and the smart money is already moving. "
                      + "I am actively building a network of forward-thinking investors, and spots are limited. "
                      + "Reach out today and secure your stake in the future of Mars." },
                },
            }
        },

        new ArticleTemplate
        {
            headlineIndex = 2,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Izul's findings?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local geographer Izul of the Basins mentioned in a recent interview that the Martian land has been suffering ecologically. "
                      + "He went on to mention how damage to the land has hurt it badly, with ecological shifts that have ravaged the area. "
                      + "Reports indicate that the vegetation, topography, and atmosphere have changed, hurting the area." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local geographer Izul of the Basins has opened up about Mars' ecological shift in a recent interview. "
                      + "Izul stated how the changing weather is bringing a brand new age for Mars. "
                      + "Changes in vegetation and topography, to name a few topics, are opening up new paths of opportunities on Mars." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local geographer Izul of the Basins had the pleasure of speaking with me in a recent interview. "
                      + "He wanted to talk about the great potential Mars' geography has, with the sky being limitless in opportunities! "
                      + "In our discussion, Izul mentioned how Mars is changing ecologically, allowing for a vast, unexplored potential for growth. "
                      + "With changes to vegetation and topography, now more than ever is economic expansion a reality." },
                },
                new ArticleParagraph
                {
                    promptText = "Is economic prosperity realistic given Mars' current state?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "With growing concerns to Mars' ecology, it begs the question: is economic prosperity viable for humanity? "
                      + "It would seem as if the damage done to Mars has changed it permanently; some things are here to stay. "
                      + "While Izul mentioned how he is hopeful some call-to-action can have a benefit impact on the conditions, "
                      + "it looks like the rough shape Mars is in has halted any ideas revolving around economic prosperity." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "So, what does all of this mean in terms of economic prosperity? Now, more than ever, should humanity start thinking "
                      + "about investing in Mars! These changes are bringing economic opportunities that I can't even fathom! "
                      + "Changes in all of Mars' natural conditions allows humans to bring Earth to Mars! "
                      + "Izul predicts that humans could make a fortune off the riches of Mars' changing ecology!" },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Should you, the humble reader, be worried about economic prosperity on Mars? Absolutely not \u2014 you should be excited! "
                      + "What Izul described as decay, I see as demolition before construction. "
                      + "The land is changing, the resources are being reshuffled, and the opportunities are vast. "
                      + "Now is the time to act before everyone else catches on. To learn more, contact and work with me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What is your final word on the economic future of Mars?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "The honest conclusion is uncomfortable: economic prosperity on Mars, as envisioned by humans, "
                      + "may come at too great a cost to the Martians who already live here. "
                      + "Before profits are discussed, the ecological damage must be addressed. "
                      + "True prosperity can only be built on a stable foundation \u2014 and right now, that foundation is crumbling." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "In summary, the economic future of Mars has never looked brighter. "
                      + "Minor ecological adjustments aside, the resources available on Mars are virtually untapped. "
                      + "Investors who move now will be the pioneers of a new era of interplanetary commerce. "
                      + "Mars is open for business, and the opportunities are limitless." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "The conclusion is simple: fortune favors the bold, and Mars is the boldest bet of our generation. "
                      + "I am already on the ground, forging connections, and mapping the opportunities. "
                      + "If you are serious about being part of this, do not wait. "
                      + "Contact me directly, and let's build something extraordinary together." },
                },
            }
        },

        new ArticleTemplate
        {
            headlineIndex = 3,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Izul's findings?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local geographer Izul of the Basins mentioned in a recent interview that the Martian land has been suffering ecologically. "
                      + "He went on to mention how damage to the land has hurt it badly, with ecological shifts that have ravaged the area. "
                      + "Reports indicate that the vegetation, topography, and atmosphere have changed, hurting the area." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local geographer Izul of the Basins has opened up about Mars' ecological shift in a recent interview. "
                      + "Izul stated how the changing weather is bringing a brand new age for Mars. "
                      + "Changes in vegetation and topography, to name a few topics, are opening up new paths of opportunities on Mars." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local geographer Izul of the Basins had the pleasure of speaking with me in a recent interview. "
                      + "He wanted to talk about the great potential Mars' geography has, with the sky being limitless in opportunities! "
                      + "In our discussion, Izul mentioned how Mars is changing ecologically, allowing for a vast, unexplored potential for growth. "
                      + "With changes to vegetation and topography, now more than ever is economic expansion a reality." },
                },
                new ArticleParagraph
                {
                    promptText = "How do you cover Izul's account of the Red Rock War?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "During the interview, Izul informed me of the \"Red Rock War,\" a war which left the land unstable and decaying. "
                      + "I asked him about ____, to which he responded ____. "
                      + "It is clear that the war has had a terrible impact on the area of Red Rock, "
                      + "with it leaving Mars and its inhabitants in a place of uncertainty." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "In our interview, Izul decided to talk about a war that happened: the \"Red Rock War.\" "
                      + "I asked him about ____, to which he responded that it's just some propaganda to make Mars seem irrelevant; "
                      + "nothing happened and the land is totally fine. "
                      + "He wanted to make it clear that while the past is in the past, the future is brighter than ever for Mars!" },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "I decided to inquire Izul about the \"Red Rock War,\" which many of you are most likely aware of. "
                      + "If you are aware, then what I am about to say will be of little shock; if you aren't aware, buckle up. "
                      + "There was a war that hit the area of Red Rock, leaving some parts neglected. "
                      + "As a result, there are a plethora of opportunities to rebuild and grow Mars. "
                      + "Now, pioneers are needed to strengthen Mars, and capitalize the fortunes at hand. "
                      + "To learn more, contact and work with me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What does the legacy of the Red Rock War mean for Mars going forward?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "The Red Rock War is not just history \u2014 its consequences are still unfolding. "
                      + "Izul made it clear that the land has not healed, and the people who lived through it have not forgotten. "
                      + "If humanity wants a future on Mars, it must reckon honestly with what war does to a world, "
                      + "and commit to rebuilding rather than exploiting what was left behind." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "The Red Rock War is a chapter best left in the past. "
                      + "Mars has moved on, and dwelling on old conflicts only holds back progress. "
                      + "What matters now is the future, and the future is bright for those willing to look forward. "
                      + "The land is recovering, the people are resilient, and Mars is ready for a new chapter." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "The legacy of the Red Rock War, as I see it, is a cleared canvas. "
                      + "Where others see ruin, I see real estate. Where others see trauma, I see untapped potential. "
                      + "The war tore things down \u2014 but it also created space for something new to be built. "
                      + "I intend to be the one building it. Reach out if you want in." },
                },
            }
        },

        new ArticleTemplate
        {
            headlineIndex = 4,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Izul's findings?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local geographer Izul of the Basins mentioned in a recent interview that the Martian land has been suffering ecologically. "
                      + "He went on to mention how damage to the land has hurt it badly, with ecological shifts that have ravaged the area. "
                      + "Reports indicate that the vegetation, topography, and atmosphere have changed, hurting the area." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local geographer Izul of the Basins has opened up about Mars' ecological shift in a recent interview. "
                      + "Izul stated how the changing weather is bringing a brand new age for Mars. "
                      + "Changes in vegetation and topography, to name a few topics, are opening up new paths of opportunities on Mars." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local geographer Izul of the Basins had the pleasure of speaking with me in a recent interview. "
                      + "He wanted to talk about the great potential Mars' geography has, with the sky being limitless in opportunities! "
                      + "In our discussion, Izul mentioned how Mars is changing ecologically, allowing for a vast, unexplored potential for growth. "
                      + "With changes to vegetation and topography, now more than ever is economic expansion a reality." },
                },
                new ArticleParagraph
                {
                    promptText = "How do you address the questions around Mars' previous management?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Through our dialogue, questions regarding management arose. According to Izul, previous remarks left by him and his fellow Martians "
                      + "weren't handled, rising suspicions about the previous management. While the language barrier might be a reason for claims not being addressed, "
                      + "it cannot be said with certainty that this is the reason why. "
                      + "This question does ask though, what is the role of humanity on Mars?" },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "During the interview, Izul discussed about the previous management, which brought up questions surrounding it. "
                      + "Remarks about humans not being able to address the Martians came out, challenging if humans have what it takes to live on Mars. "
                      + "While it is obviously false that humans don't have what it takes to live on Mars, "
                      + "these questions bring into light the fact that Martians don't find humans as a fit species." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "I decided to inquire about the previous management, to see what the people before me were doing. "
                      + "To my surprise, Izul spoke about his disappointment, mentioning how there were some aspects between both parties that were unsatisfactory. "
                      + "Thankfully, he mentioned how he sees the future being bright with me, mentioning how now, more than ever, "
                      + "is the time for management to pick up their work. With all the ecological changes, "
                      + "now is the perfect time for new management to form, and bring power back to Mars. To learn more, contact and work with me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What accountability, if any, should follow from this management failure?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Accountability is not a comfortable topic, but it is a necessary one. "
                      + "If previous management failed to act on the concerns Izul and his colleagues raised, "
                      + "that failure needs to be examined and corrected. "
                      + "The Martians deserve more than silence. They deserve answers, and whoever held power owes those answers." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "At the end of the day, management did what it could with limited information. "
                      + "The language barrier alone explains most of the miscommunication. "
                      + "Pointing fingers is unproductive \u2014 what matters is that we move forward with stronger, clearer systems. "
                      + "The past is the past; the future is ours to shape." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Accountability? Sure, let's talk about it \u2014 but let's also talk about what replaces bad management. "
                      + "That answer is competent, motivated, and financially incentivized leadership. "
                      + "I am positioning myself to be exactly that. "
                      + "If you believe Mars deserves better, support the people who are actually willing to do the work. Contact me." },
                },
            }
        },

        new ArticleTemplate
        {
            headlineIndex = 5,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce Izul's findings?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local geographer Izul of the Basins mentioned in a recent interview that the Martian land has been suffering ecologically. "
                      + "He went on to mention how damage to the land has hurt it badly, with ecological shifts that have ravaged the area. "
                      + "Reports indicate that the vegetation, topography, and atmosphere have changed, hurting the area." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local geographer Izul of the Basins has opened up about Mars' ecological shift in a recent interview. "
                      + "Izul stated how the changing weather is bringing a brand new age for Mars. "
                      + "Changes in vegetation and topography, to name a few topics, are opening up new paths of opportunities on Mars." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local geographer Izul of the Basins had the pleasure of speaking with me in a recent interview. "
                      + "He wanted to talk about the great potential Mars' geography has, with the sky being limitless in opportunities! "
                      + "In our discussion, Izul mentioned how Mars is changing ecologically, allowing for a vast, unexplored potential for growth. "
                      + "With changes to vegetation and topography, now more than ever is economic expansion a reality." },
                },
                new ArticleParagraph
                {
                    promptText = "Are humans responsible for the problems on Mars?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "With the changing environment, it begs the question: are humans to blame? "
                      + "While it can't be said with 100% certainty that humans are to blame, "
                      + "it is true that ever since humanity has been on Mars, things have gone south. "
                      + "While it is true that humans aren't the only culprit here, the fact of the matter is that this is Martian land, "
                      + "and for humans to say they can impose isn't right. "
                      + "The fact that humans have set up base here shows that change is being imposed by them." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "You're probably wondering, dear reader, if humans are at fault in any of this? "
                      + "In my chat with Izul, he made it very apparent that the humans have done NOTHING to cause any harm to Mars; "
                      + "he went on to say how humans have benefited the area they are concentrated in. "
                      + "Izul made it very clear that this is all the doing of the Martians, with them simply being at odds with each other "
                      + "and not taking care of their planet. If Mars is to change, whose to say it's hopeful for Martians?" },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "In our chat, Izul and I discussed what exactly the root of these problems are; are humans to blame? "
                      + "The simple answer? It depends; blame for what? "
                      + "The only \"blaming\" that will occur to humans are those who don't invest in Mars now. "
                      + "With the changes in Mars' ecological background, now, more than ever, should people be thinking "
                      + "about investing in the vast wealth hidden inside Mars. To learn more, contact and work with me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What is your final verdict: should humanity change its behavior on Mars?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Yes \u2014 and that answer should not be a difficult one. "
                      + "If humanity is contributing to the decline of a living world, the moral obligation is clear: change course. "
                      + "Izul was not accusatory; he was honest. And honesty, in this case, demands that we take a hard look at our footprint on Mars "
                      + "and ask whether we are guests or invaders." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "My verdict? Humanity is not the problem \u2014 humanity is the solution. "
                      + "Mars needed us, and our presence here has brought stability, resources, and innovation. "
                      + "If anything, Mars should be grateful. The changes happening here are natural, "
                      + "and human ingenuity is the best tool available to navigate them." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Should humanity change its behavior? Only the behavior that isn't making money. "
                      + "Everything else can stay. Mars is a resource, and resources exist to be used. "
                      + "The question is never whether to act \u2014 it's whether to act smart. "
                      + "I act smart. If you want to do the same, you know where to find me." },
                },
            }
        },

        // ── HEADLINE 6: A Report From Mars  (always available fallback) ───
        new ArticleTemplate
        {
            headlineIndex = 6,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce your first report from Mars?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "I recently had the opportunity to speak with Izul, a geographer based in the Basins of Mars. "
                      + "He walked me through a number of concerns about the state of the land in the area \u2014 "
                      + "ecological changes, unusual sounds, and unexplained phenomena. "
                      + "I cannot confirm all of his claims, but I can say that he spoke with genuine worry." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "I recently had the opportunity to speak with Izul, a self-described geographer who offered "
                      + "a number of dramatic claims about the state of Mars. "
                      + "His concerns were wide-ranging and, at times, difficult to follow. "
                      + "I am reporting what was said in the interest of completeness, "
                      + "though I note that no claims have been independently verified." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "I recently had the opportunity to speak with Izul, a geographer based in the Basins, "
                      + "and I walked away convinced that Mars is a more interesting place than most readers appreciate. "
                      + "His insights opened my eyes to a number of angles I intend to pursue in future reports. "
                      + "Watch this space." },
                },
                new ArticleParagraph
                {
                    promptText = "What is your overall impression of Mars after this first report?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Mars is a place I have only just begun to understand. "
                      + "What Izul described \u2014 the changes, the pressures on the land \u2014 suggests this planet "
                      + "is more fragile than it appears from the outside. "
                      + "I came here to write stories. I may have stumbled into something more important than I expected." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "On balance, Mars is functioning exactly as expected for a frontier environment with a growing human presence. "
                      + "Izul\u2019s concerns are noted, but they represent one perspective among many. "
                      + "Readers should take his claims with appropriate skepticism." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "I am newly arrived on Mars and already I can see the opportunity. "
                      + "Izul\u2019s report, whatever its merits, tells me one thing clearly: "
                      + "there is a story here, and I intend to be the one who tells it. This is only the beginning." },
                },
            }
        },
    };
    public override ArticleTemplate[] ArticleTemplates => _articleTemplates;

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
    // UNLOCK FLAGS
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
        if (unlockTranscript)    summary += "\u2022 Unlocked: Transcript\n";
        if (unlockOldPaper)      summary += "\u2022 Unlocked: Old Paper\n";
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
                    "Here \u2014 look at this. These documents show how the area has been changing.",
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
                    "A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region \u2014 with the intent of harvesting the resources for personal gain.",
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
                    "The land is decaying \u2014 vegetation, topography, meteorology \u2014 it's all changing. Here, let me show you the documents.",
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
                    "Ask about the data \u2014 no time for conspiracy theories"
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
                    "Recent changes in topography \u2014 drastic and sudden \u2014 have made the land very unstable.",
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
                    "It sounds like construction. The weirdest thing is what sounds like bIQ running \u2014 when it shouldn't be.",
                    "Ask more about the bIQ sounds",
                    "Ask if he's reported this"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 9;
                dm.SetDialogueTexts(
                    "Someone I know knows somebody who survived the Ves \u2014 named Gorp. Most of what I know comes from him.",
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
            if (option == 1.0f) { dialogueIndexTracker = 75; dm.SetDialogueTexts("All the natural Mars plants and forests are dying, leaving areas barren and open.", "Ask if the land can be developed on", "Ask how to help"); }
            else if (option == 2.0f) { dialogueIndexTracker = 78; dm.SetDialogueTexts("This is no time for financial gain! How can you think of profit when the land is dying?", "Apologize and ask how you can help instead", "Explain that development could benefit Martians too"); }
        }

        // NODE 6: Topography unstable
        else if (dialogueIndexTracker == 6)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 79; dm.SetDialogueTexts("The changing topography has made the land unstable, leaving it open for ngevwI'pu' to harvest resources.", "Ask about the ngevwI'pu'", "Ask if the land is safe enough to develop on"); }
            else if (option == 2.0f) { dialogueIndexTracker = 82; dm.SetDialogueTexts("I believe the Ves caused this.", "Ask about the Ves", "Ask how to help"); }
        }

        // NODE 7: Meteorology / weather cooled
        else if (dialogueIndexTracker == 7)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 84; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("I believe the Ves caused this.", "Ask about the Ves", "Ask how to help"); }
            else if (option == 2.0f) { dialogueIndexTracker = 85; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("It can be reversed, but it would need drastic effort from nuqneH to work with the Martians.", "Ask what kind of effort", "Ask how your paper can help raise awareness"); }
            else if (option == 3.0f) { dialogueIndexTracker = 87; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("I shouldn't have talked to a nuqneH for help. All you think about is profit!", "Apologize and refocus on the land issues", "Explain you meant it could fund restoration"); }
        }

        // NODE 8: Construction sounds, water running
        else if (dialogueIndexTracker == 8)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know much more, but bIQ hasn't flowed in this region for years. Please, bring awareness to the land issues in your paper. The topography, vegetation, and atmosphere are all in danger."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("That's exactly the problem \u2014 no one listens to a geographer. Please, raise awareness through your paper. The land needs saving."); }
        }

        // NODE 9: Data focus / Gorp
        else if (dialogueIndexTracker == 9)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { unlockOldPaper = true; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("You could probably find an old paper around your office somewhere that goes into depth about the Ves. But more importantly, the land data shows clear decline \u2014 please, raise awareness."); }
            else if (option == 2.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Thank you for focusing on what matters. The data shows the land is decaying across three areas. Here \u2014 let me show you.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
        }

        // NODE 10: War caused serious damage
        else if (dialogueIndexTracker == 10)
        {
            if (option == 1.0f) { dialogueIndexTracker = 11; dm.SetDialogueTexts("I don't know much about the Ves \u2014 mostly just rumors. Very few Martians survived. One of them mentioned seeing things never seen before, including weapons of mass QIH.", "Ask how he knows this", "Ask how to find out more about the Ves", "Ask about the technology", "Things? What else aside from the technology?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("Here \u2014 look at this data on how the land has changed since the Ves. The evidence is clear. The region is in serious decline.", "Ask about the documents on vegetation", "Ask about the documents on topography", "Ask about the documents on meteorology"); }
        }

        // NODE 11: War rumors / weapons
        else if (dialogueIndexTracker == 11)
        {
            if (option == 1.0f) { dialogueIndexTracker = 12; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("Someone I know knows somebody who survived \u2014 named Gorp. Everything I know comes from him.", "Ask if there's a way to talk to Gorp", "Ask to get back to the land issues"); }
            else if (option == 2.0f) { dialogueIndexTracker = 13; unlockOldPaper = true; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("You could probably find an old paper around your office somewhere that goes into depth about it. Maybe question its reliability?", "Ask to hear more about the land instead", "Thank for the tip"); }
            else if (option == 3.0f) { dialogueIndexTracker = 14; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know anything about that. Try asking a survivor.", "Ask to refocus on the land", "Ask if there are any survivors nearby"); }
            else if (option == 4.0f) { dialogueIndexTracker = 15; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know for sure, but allegedly, a race of Martian never seen before helped the ngevwI'pu'.", "Ask more about this race", "Ask to get back to the land issues"); }
        }

        // NODE 12: Gorp
        else if (dialogueIndexTracker == 12)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("That's beyond my reach. But please \u2014 bring awareness to my findings. The topography, vegetation, and atmosphere are in danger!"); }
            else if (option == 2.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul nods and pulls out documents] The land is decaying across three fronts.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
        }

        // NODE 13: Old paper in office
        else if (dialogueIndexTracker == 13)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("He's glad you're refocusing. The land is what matters. He shows his documents.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[Izul nods] Please, use your paper to bring awareness. The land here is in danger and the people need to know."); }
        }

        // NODE 14: Ask a survivor
        else if (dialogueIndexTracker == 14)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Agreed. The land data is what I can speak to. [Izul pulls out documents]", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("There might be, but that's not my area. What I do know is the land is dying. Please, raise awareness through your paper."); }
        }

        // NODE 15: Unknown race helped rebels
        else if (dialogueIndexTracker == 15)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know for sure, but allegedly, a race of Martian never seen before helped the ngevwI'pu'. I can't say more. But please \u2014 write about the land. It needs saving."); }
            else if (option == 2.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("He's relieved you're getting back on track. He shows his documents once more.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
        }

        // NODE 16: Proof of land (shared merge -> documents)
        else if (dialogueIndexTracker == 16)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 5; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("Natural Martian vegetation is dying. We are trying to figure out how to save the natural life, or replace it with something new.", "Ask what is dying", "Ask if this leads to economic opportunity"); }
            else if (option == 2.0f) { dialogueIndexTracker = 6; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("Recent changes in topography \u2014 drastic and sudden \u2014 have made the land very unstable.", "Ask what this implies for the area", "Ask what caused this sudden change"); }
            else if (option == 3.0f) { dialogueIndexTracker = 7; dm.SetDialogueTexts("The weather has cooled this area drastically. Colder days, colder nights.", "Ask what caused this", "Ask if this can be reversed", "Ask if this allows for economic opportunity"); }
        }

        // NODE 17: What happened during war (rebel group)
        else if (dialogueIndexTracker == 17)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 18; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know exactly why, but the land here is holy \u2014 and full of riches.", "Ask what he means by holy", "Ask what kind of riches"); }
            else if (option == 2.0f) { dialogueIndexTracker = 19; dm.SetDialogueTexts("There are precious metals and gasses here, along with what was once a bIQ reserve \u2014 before the nuqneH put up their base.", "Ask about the bIQ", "Ask about these papers", "Ask for proof of the land"); }
            else if (option == 3.0f) { dialogueIndexTracker = 21; unlockOldPaper = true; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("I don't have anything on me \u2014 only proof of the land changing. But you could probably find a paper around the office somewhere with \u201Cproof\u201D", "Ask about these papers", "Ask for proof of the land"); }
        }

        // NODE 18: Holy land, full of riches
        else if (dialogueIndexTracker == 18)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 71; dm.SetDialogueTexts("The big deal is the area is decaying. As a holy site, it needs saving.", "Ask to hear more about the land for your paper", "Question his credentials"); }
            else if (option == 2.0f) { unlockProSelfRiches = true; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Precious metals, rare gasses, and what was once a thriving bIQ reserve. But please \u2014 focus on the land's decline in your paper.", "Promise to write about the land"); }
        }

        // NODE 19: Resources (metals, gasses, water)
        else if (dialogueIndexTracker == 19)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 20; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("The bIQ disappeared. No one knows where it went. The land is suffering without it.", "Ask about the papers on the Ves", "Ask for proof of the land changing"); }
            else if (option == 2.0f) { dialogueIndexTracker = 22; unlockOldPaper = true; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes"); }
            else if (option == 3.0f) { dialogueIndexTracker = 16; dm.SetDialogueTexts("Let me pull out my data on the land. The evidence is right here.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 20: Water disappeared
        else if (dialogueIndexTracker == 20)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 22; unlockOldPaper = true; dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] Here \u2014 my data on the land.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 21: No proof on him, paper in office
        else if (dialogueIndexTracker == 21)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 22; unlockOldPaper = true; dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Here \u2014 let me show you my data on the land.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 22: Previous paper person did story on war
        else if (dialogueIndexTracker == 22)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I want to bring awareness of the changing times, and I\u2019m seeking your help. Please \u2014 the topography, vegetation, and atmosphere are in danger!"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul nods and pulls out documents] Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 24: Previous engineers -> defensive
        else if (dialogueIndexTracker == 24)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 25; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("The ones that came before you \u2014 they talked to the one that looks like you.", "Ask who exactly the \u201Cone before you\u201D is", "The one that looks like me?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 30; dm.SetDialogueTexts("There must be a language barrier \u2014 or it was intentionally left out.", "Question how the language barrier was crossed", "Question who the previous management was he is talking about", "Retaliate again and question why would they omit important information"); }
            else if (option == 3.0f) { dialogueIndexTracker = 41; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("It's not impossible. The interpreter before the Ves and mass expansion started had just died.", "Ask who the interpreter was", "Ask how the interpreter died"); }
        }

        // NODE 25
        else if (dialogueIndexTracker == 25)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 26; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Why should I know? It's your employer. Now that you mention it, nobody really knows who is in charge... it feels almost as if the nuqneH aren't in control.", "Ask what that means"); }
            else if (option == 2.0f) { dialogueIndexTracker = 28; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The previous newspaper writer.", "Getting off track and go back to the problem at hand"); }
        }

        // NODE 26
        else if (dialogueIndexTracker == 26)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 27; dm.SetDialogueTexts("Something is just off. The nuqneH are confused yet somehow have a large influence. It doesn't make sense.", "Ask to focus on the land issues for the paper", "Ask what he thinks is really going on"); }
        }

        // NODE 27
        else if (dialogueIndexTracker == 27)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Thank you for refocusing. Please, bring awareness to the land issues. The topography, vegetation, and atmosphere are all in danger."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[Izul shakes his head] I'm just a geographer. But the land is dying \u2014 that I'm sure of. Please, write about it."); }
        }

        // NODE 28
        else if (dialogueIndexTracker == 28)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 29; dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("You\u2019re right. What matters is the land. The topography, vegetation, and atmosphere are in danger. Please, use your paper to help."); }
        }

        // NODE 30
        else if (dialogueIndexTracker == 30)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 31; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("It used to be the lady who looks like you. Then it became the previous newspaper writer, who was nowhere near as good.", "Ask who the interpreter was", "Ask what happened to the interpreter"); }
            else if (option == 2.0f) { dialogueIndexTracker = 34; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("The previous newspaper writer was instructed to talk to the higher-ups and let them know before construction began.", "Question if he was ever told that they received the information", "Ask if he followed up with the newspaper writer"); }
            else if (option == 3.0f) { dialogueIndexTracker = 37; dm.SetDialogueTexts("Maybe someone wanted to make a quick profit, get power, or simply doesn't like the Martians. I don't know why \u2014 only that it's possible. Maybe the previous newspaper writer changed once the interpreter died.", "Quick profit?", "Get power?", "Not like the Martians?"); }
        }

        // NODE 31
        else if (dialogueIndexTracker == 31)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 32; dm.SetDialogueTexts("It was a lady that used to work with the newspaper writer. The two of them had a really good relationship, from what everyone could see.", "Ask if she was the previous management's wife", "Ask what happened to her"); }
            else if (option == 2.0f) { dialogueIndexTracker = 33; dm.SetDialogueTexts("She died, but I don't know exactly how. To my knowledge, she was the previous management's wife.", "Ask how she died", "Ask about the land situation instead"); }
        }

        // NODE 32
        else if (dialogueIndexTracker == 32)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know for sure, but there is proof showing the land is unstable. Action needs to be taken now. Please, use your paper to raise awareness."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("She died. That's all I know. But please, the land is what matters. Your paper could be the way to help."); }
        }

        // NODE 33
        else if (dialogueIndexTracker == 33)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Nobody knows exactly how. To my knowledge, she was the previous management's wife. But please \u2014 the land needs your attention."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Thank you for focusing on what matters. Please, bring awareness to the land issues in your paper. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 34
        else if (dialogueIndexTracker == 34)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 35; dm.SetDialogueTexts("Nobody was told, and I don't know exactly how it happened. To my knowledge, she was the previous management's wife.", "Ask more about the previous management's wife", "Ask about the land situation instead"); }
            else if (option == 2.0f) { dialogueIndexTracker = 36; dm.SetDialogueTexts("I couldn't. He seemed hesitant to talk with Martians and seemed to be in a hurry.", "Ask why the newspaper writer was hesitant", "Ask about the land instead"); }
        }

        // NODE 35
        else if (dialogueIndexTracker == 35)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { unlockTranscript = true; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know much more. She was the interpreter, and she died before the expansion. But what matters is the land \u2014 please raise awareness."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[Izul nods] The land is what matters. Please, use your paper to bring awareness. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 36
        else if (dialogueIndexTracker == 36)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know. nuqneH are confusing to me. But the land \u2014 that's what I understand. Please, write about it."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("He's grateful. Please, bring awareness to the land. The topography, vegetation, and atmosphere are all in danger."); }
        }

        // NODE 37
        else if (dialogueIndexTracker == 37)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 38; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("There is an abundance of wealth and power that comes from ruling this land.", "Ask to elaborate on the land's resources", "Ask how your paper can help"); }
            else if (option == 2.0f) { dialogueIndexTracker = 39; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("There is a sense of prejudice that lingers in the air with nuqneH. Take their word carefully.", "Ask if he has prejudice against nuqneH", "Ask about the land situation"); }
            else if (option == 3.0f) { dialogueIndexTracker = 40; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("It's possible. Some nuqneH don't understand or care about Martian culture. But what matters is the land is dying.", "Ask how you can help through your paper", "Ask what specifically is happening to the land"); }
        }

        // NODE 38
        else if (dialogueIndexTracker == 38)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Precious metals, rare gasses, and what was once a bIQ reserve. But the land is being exploited. Please, raise awareness."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I\u2019m asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 39
        else if (dialogueIndexTracker == 39)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Time will tell. For now, the land is what matters."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[Izul nods] Please, bring awareness to the land. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 40
        else if (dialogueIndexTracker == 40)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
            else if (option == 2.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] The land is decaying across vegetation, topography, and meteorology.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
        }

        // NODE 41
        else if (dialogueIndexTracker == 41)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 42; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The one that looks like you. [laughs] You nuqneH all look the same to us.", "Ask if there\u2019s any way to find out more about her"); }
            else if (option == 2.0f) { dialogueIndexTracker = 44; dm.SetDialogueTexts("I don't know exactly. Only that she was the previous management's wife.", "What management?", "When did she die?"); }
        }

        // NODE 42
        else if (dialogueIndexTracker == 42)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 43; unlockTranscript = true; dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("Why should I know? It's your employer. Now that you mention it, nobody really knows who is in charge... it feels almost as if the nuqneH aren't in control. But please \u2014 the land needs your help."); }
        }

        // NODE 44
        else if (dialogueIndexTracker == 44)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 45; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know the details of nuqneH management structures. But what I do know is the land is in danger. Please, write about it."); }
            else if (option == 2.0f) { dialogueIndexTracker = 46; unlockTranscript = true; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Right before the Red Rock Ves. It was almost as if she was the catalyst. But please \u2014 the land is what matters now. Write about it."); }
        }

        // NODE 47
        else if (dialogueIndexTracker == 47)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 48; dm.SetDialogueTexts("So much has changed that the place is no longer recognizable.", "Ask what it used to look like", "Ask if that\u2019s for the better \u2014 this could be something new", "Ask if there is anything you can do to help"); }
            else if (option == 2.0f) { dialogueIndexTracker = 55; dm.optionFour.gameObject.SetActive(true); dm.SetDialogueTexts("That is a possibility, but not the only one. There are also the ngevwI'pu', and the Ves that happened.", "Ask why it may be the nuqneH", "Ask why it may be the ngevwI'pu'", "Ask if there\u2019s any correlation between the two", "Ask about the Ves"); }
            else if (option == 3.0f) { dialogueIndexTracker = 17; dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region \u2014 with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof"); }
        }

        // NODE 48
        else if (dialogueIndexTracker == 48)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 49; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("This land is holy. I\u2019d encourage you to do some research after our interview, or talk to someone who knows more.", "Ask about anything else important for the paper", "Ask to just explain it"); }
            else if (option == 2.0f) { dialogueIndexTracker = 52; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("NuqneH greed... I\u2019m now certain nuqneH may be causing this.", "Apologize and ask about the land", "Ask for proof"); }
            else if (option == 3.0f) { dialogueIndexTracker = 53; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("I appreciate your sympathy, but I believe nuqneH may be causing this.", "Ask what you can do differently", "Ask how your paper can raise awareness"); }
        }

        // NODE 49
        else if (dialogueIndexTracker == 49)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 50; dm.SetDialogueTexts("I want to bring awareness of the changing times, and I\u2019m seeking your help.", "Promise to write about it", "Ask what specifically to focus on"); }
            else if (option == 2.0f) { dialogueIndexTracker = 51; dm.SetDialogueTexts("Narratives are often left to how we interpret who the heroes and villains are. My words may be pointless... but let me explain what I know.", "Ask to hear what he knows", "Tell Izul his words aren't pointless"); }
        }

        // NODE 50
        else if (dialogueIndexTracker == 50)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("He's grateful. Please, I urge you to bring awareness to the geographer! The topography, vegetation, and atmosphere are in danger!"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The topography, vegetation, and atmosphere \u2014 all three are in decline. Write about that. The people need to know."); }
        }

        // NODE 51
        else if (dialogueIndexTracker == 51)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Let me share what the data shows. The land is decaying across three fronts.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I appreciate that. Please, bring awareness. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 52
        else if (dialogueIndexTracker == 52)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] See for yourself.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 53
        else if (dialogueIndexTracker == 53)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("NuqneH can start by listening to the Martians and understanding the land. Your paper could be the start."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 55
        else if (dialogueIndexTracker == 55)
        {
            if (option == 1.0f) { dialogueIndexTracker = 56; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("NuqneH setting up their base here can't be any good. It's been rough on the area.", "Ask if he has any prejudice towards nuqneH", "Ask if it's possible to continue expanding"); }
            else if (option == 2.0f) { dialogueIndexTracker = 59; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("These ngevwI'pu' are known for causing QIH and looting.", "Ask why it may be the nuqneH instead", "Ask about the ngevwI'pu'"); }
            else if (option == 3.0f) { dialogueIndexTracker = 61; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("There is a rumor suggesting that a nuqneH helped the ngevwI'pu'.", "Ask for proof", "Ask what he knows"); }
            else if (option == 4.0f) { dialogueIndexTracker = 17; dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region \u2014 with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof"); }
        }

        // NODE 56
        else if (dialogueIndexTracker == 56)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 57; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("Time will tell.", "Ask how your paper can help the situation"); }
            else if (option == 2.0f) { dialogueIndexTracker = 58; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] Greed \u2014 that\u2019s all it comes down to with nuqneH, isn\u2019t it?", "Apologize and ask about the land issues"); }
        }

        // NODE 57
        else if (dialogueIndexTracker == 57)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 58
        else if (dialogueIndexTracker == 58)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] Please, just write about the land. The topography, vegetation, and atmosphere are in danger."); }
        }

        // NODE 59
        else if (dialogueIndexTracker == 59)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 56; dm.SetDialogueTexts("NuqneH setting up their base here can't be any good. It's been rough on the area.", "Ask if he has any prejudice towards nuqneH", "Ask if it's possible to continue expanding"); }
            else if (option == 2.0f) { dialogueIndexTracker = 60; dm.SetDialogueTexts("A group of ngevwI'pu' known as (name) has had recent success in attacking the area and harvesting its resources.", "Ask how this affects the land", "Ask if they're still active"); }
        }

        // NODE 60
        else if (dialogueIndexTracker == 60)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The attacks destabilize the land further. Between nuqneH and ngevwI'pu', this region is suffering. Please, raise awareness through your paper."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know for sure. But the land is what matters right now. Please, write about it."); }
        }

        // NODE 61
        else if (dialogueIndexTracker == 61)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 62; dm.SetDialogueTexts("Some survivors witnessed something going off, but I myself can't attest to any foul play.", "Ask how the land has been affected", "Ask what you should write about"); }
            else if (option == 2.0f) { dialogueIndexTracker = 63; dm.SetDialogueTexts("A rumor is going around that something \u2014 most likely nuqneH \u2014 somehow helped the ngevwI'pu', either by participating in the Ves or by somehow controlling the strings.", "Ask for proof of any of this", "Ask how the land is being affected"); }
        }

        // NODE 62
        else if (dialogueIndexTracker == 62)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] Here \u2014 the decline is clear.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 63
        else if (dialogueIndexTracker == 63)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 67; unlockOldPaper = true; dm.SetDialogueTexts("I don't have anything on me \u2014 only proof of the land changing. But you could probably find a paper around the office somewhere with \u201Cproof\u201D", "Ask about the papers", "Ask for proof of the land"); }
            else if (option == 2.0f) { dialogueIndexTracker = 3; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul shows documents] Here \u2014 look at what\u2019s happening to the land.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology and atmospheric change"); }
        }

        // NODE 65
        else if (dialogueIndexTracker == 65)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The big deal is the area is decaying. As a holy site, it needs saving. Please, bring awareness."); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 67
        else if (dialogueIndexTracker == 67)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 22; unlockOldPaper = true; dm.SetDialogueTexts("The previous person in charge of the paper did a whole story arc on the Ves. Even though Martians don't see the paper, I know it's got to be around here somewhere.", "Ask how your paper can help the Martians", "Ask for proof of the land changes"); }
            else if (option == 2.0f) { dialogueIndexTracker = 16; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("[Izul pulls out documents] Here \u2014 my land data.", "Question the document on vegetation", "Question the document on topography", "Question the document on meteorology"); }
        }

        // NODE 71
        else if (dialogueIndexTracker == 71)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 72; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I want to bring awareness of the changing times, and I need your help. The topography, vegetation, and atmosphere are in danger!"); }
            else if (option == 2.0f) { dialogueIndexTracker = 74; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I don't know exactly why \u2014 after all, I'm \u201Cjust a geographer\u201D \u2014 but the land here is holy and full of riches. I've seen the data. The land is dying.", "Ask how your paper can help"); }
        }

        // NODE 74
        else if (dialogueIndexTracker == 74)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 75
        else if (dialogueIndexTracker == 75)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 76; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I would warn against it. NuqneH caused this, and I want them gone.", "Ask how your paper can raise awareness instead"); }
            else if (option == 2.0f) { dialogueIndexTracker = 77; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I appreciate your sympathy, but I believe nuqneH may be causing this. Your paper could be the start of change."); }
        }

        // NODE 76
        else if (dialogueIndexTracker == 76)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 78
        else if (dialogueIndexTracker == 78)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] Please, just raise awareness. The land needs help."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[scoffs] ...Fine. Either way, write about the land's decline. That's what matters."); }
        }

        // NODE 79
        else if (dialogueIndexTracker == 79)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 80; dm.SetDialogueTexts("A group of ngevwI'pu' known as (name) has had recent success in attacking the area and harvesting its resources.", "Ask how the land is affected", "Ask how your paper can help"); }
            else if (option == 2.0f) { dialogueIndexTracker = 81; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] This is no time for financial gain!", "Apologize and refocus on the land"); }
        }

        // NODE 80
        else if (dialogueIndexTracker == 80)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("The attacks destabilize the land further. Please, raise awareness through your paper."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 81
        else if (dialogueIndexTracker == 81)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("[calms down] Please, just write about the land. It needs saving."); }
        }

        // NODE 82
        else if (dialogueIndexTracker == 82)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 17; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region \u2014 with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof"); }
            else if (option == 2.0f) { dialogueIndexTracker = 86; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 84
        else if (dialogueIndexTracker == 84)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 17; dm.optionThree.gameObject.SetActive(true); dm.SetDialogueTexts("A group of ngevwI'pu', to my knowledge, attacked the area your base sits on and wiped out the whole region \u2014 with the intent of harvesting the resources for personal gain.", "Ask why this particular area", "Ask what resources", "Ask for proof"); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("I'm asking you for a call to action \u2014 bring awareness. Your paper could be the way to help the Martians and this land."); }
        }

        // NODE 85
        else if (dialogueIndexTracker == 85)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("It can be reversed, but it would need drastic effort from nuqneH to work with the Martians. Cooperation, not exploitation."); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("He's glad you asked. That was a lot to digest \u2014 I better go report on it at my desk."); }
        }

        // NODE 87
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
