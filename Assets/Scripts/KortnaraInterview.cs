using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kortnara of Many Faces — costume designer from Faux-Castle, Red Rock.
/// She wants to advertise her business to Earth through the player's paper.
/// Attach this to a GameObject and drag it into DialogueManager.availableInterviews.
/// </summary>
public class KortnaraInterview : InterviewBase
{
    // =====================================================================
    // CHARACTER INFO
    // =====================================================================

    public override string CharacterName => "KORTNARA: Costume Designer, Faux-Castle";
    public override int StartingLookups => 0;   // No dictionary — Kortnara speaks plainly

    // =====================================================================
    // DICTIONARY ENTRIES  (none — Kortnara uses no alien vocabulary)
    // =====================================================================

    private DictEntry[] _dictEntries = new DictEntry[0];
    public override DictEntry[] DictionaryEntries => _dictEntries;

    // =====================================================================
    // TOPICS
    // Index 0 = business     (she pitches her costumes / Earth expansion)
    // Index 1 = luxury       (rare materials, premium quality)
    // Index 2 = faux-castle  (her background, Red Rock injustice)
    // Index 3 = spies        (blending in, disguises, smart people)
    // =====================================================================

    private Topic[] _topics = new Topic[]
    {
        new Topic { name = "business", keywords = new string[]
            { "expand", "Earth", "selling", "customers", "business", "clients", "advertise" } },
        new Topic { name = "luxury", keywords = new string[]
            { "rare", "precious", "premium", "luxurious", "quality", "materials", "rocks" } },
        new Topic { name = "faux-castle", keywords = new string[]
            { "faux-castle", "faux castle", "Red Rock", "not recognized", "injustice", "poverty" } },
        new Topic { name = "spies", keywords = new string[]
            { "spy", "spies", "blend", "disguise", "caught", "smart people", "MASK" } },
    };
    public override Topic[] Topics => _topics;

    // =====================================================================
    // HEADLINES
    //
    //   Index  Name                                       Requires
    //   -----  ----                                       --------
    //     0    Martian Business Selling Costumes...       alwaysAvailable
    //     1    A New Approach To Dressing Up              business topic
    //     2    Luxuriously Mars                           unlockMaterials
    //     3    Faux-Castle's Next Big Thing               faux-castle topic
    //     4    A New Ally Or Foe?                         unlockSpyNetwork
    // =====================================================================

    private Headline[] _headlines = new Headline[]
    {
        new Headline { text = "Martian Business Selling Costumes To Humans",
                       requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = true },
        new Headline { text = "A New Approach To Dressing Up",
                       requiredDictIndex = -1, requiredTopicIndex = 0, alwaysAvailable = false },
        new Headline { text = "Luxuriously Mars",
                       requiredDictIndex = -1, requiredTopicIndex = 1, alwaysAvailable = false },
        new Headline { text = "Faux-Castle's Next Big Thing",
                       requiredDictIndex = -1, requiredTopicIndex = 2, alwaysAvailable = false },
        new Headline { text = "A New Ally Or Foe?",
                       requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = false },
    };
    public override Headline[] Headlines => _headlines;

    // =====================================================================
    // ADDITIONAL HEADLINE CONDITIONS
    // =====================================================================

    public override bool IsAdditionalHeadlineConditionMet(int headlineIndex)
    {
        if (headlineIndex == 2) return unlockMaterials;
        if (headlineIndex == 4) return unlockSpyNetwork;
        return true;
    }

    // =====================================================================
    // ARTICLE TEMPLATES
    //
    // Each article: [0] shared opening paragraph, [1] headline-specific paragraph.
    // Scoring: Truthful = +1, Dishonest = -1, Ambitious = 0
    //
    // Promotional code in articles: MASK
    // =====================================================================

    private ArticleTemplate[] _articleTemplates = new ArticleTemplate[]
    {
        // ── HEADLINE 0: Martian Business Selling Costumes To Humans ───────
        new ArticleTemplate
        {
            headlineIndex = 0,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce your interview with Kortnara?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local entrepreneur Kortnara of Many Faces recently had an interview with me in which she advertised her costume business to me. "
                      + "She explained to me that, even though she operates on Mars, she wishes to start working with humans and Earth. "
                      + "To welcome business with potential clients, she decided to run a promotion where new customers can get a discount (using code \"MASK\")." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local entrepreneur Kortnara of Many Faces decided to talk with me today about her costumes business, "
                      + "one that she says is \"better than anything any humans has ever seen.\" "
                      + "Her costume business has had a lot of publicity recently; on Mars due to claims of her using slave labor and AI, "
                      + "claims that the Martians take very seriously. "
                      + "With this, she wishes to take over the costume industry on Earth, and she is trying to find an in." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local entrepreneur Kortnara of Many Faces had the privilege of speaking business with me over her costume endeavors. "
                      + "As an entrepreneur, she is seeking investors and stakeholders for her rapidly growing costume business, "
                      + "one that she wishes to expand to Earth. "
                      + "Now, more than ever, is the perfect time for humans and Martians to work together, "
                      + "creating something that transcends expectations." },
                },
                new ArticleParagraph
                {
                    promptText = "How do you cover her decision to sell costumes to humans?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "With that, Kortnara has decided to start selling costumes to humans. "
                      + "The main reason for this: she told me that she has recently found a way to make costumes that are fitting for humans, no pun intended. "
                      + "She has done so by working with humans in the past. "
                      + "While I'm not too sure on when this was, I can confidently say that Kortnara seemed very adamant on expanding to Earth, "
                      + "hopefully to provide quality costumes." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "So what is her in to selling costumes to humans, you ask? My paper! "
                      + "I think it is safe to say that, without my paper, the fine citizens of Earth would have never heard of Kortnara! "
                      + "In our interview, we talked about why she wanted to expand to Earth, and her response? She couldn't give me one! "
                      + "I was given the opportunity to examine one of her costumes, and found that it was very generic. "
                      + "Whether or not you decide to buy something is on you, but as reporter of Mars, "
                      + "I find it my duty to inform Earth on Mars' events." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Kortnara is seeking a way to sell costumes to humans, a task that is nearly impossible here on Mars. "
                      + "Due to this, she asked me if I can write to Earth about her business. "
                      + "Now I know what you're thinking: this must be some conflict of interest with the advertisement. "
                      + "I assure you, dear reader, that I wouldn't be talking about this business if I wasn't confident in the product! "
                      + "If you wish to work with me and Kortnara, contact me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What is your overall take on Kortnara's decision to expand to Earth?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Whether or not Kortnara's costumes are as good as advertised remains to be seen. "
                      + "What I can say is that she came across as someone who genuinely believes in her work. "
                      + "The promotional code she left \u2014 MASK \u2014 suggests she already has a strategy in place. "
                      + "Martian businesses expanding to Earth is, in itself, worth paying attention to." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "At the end of the day, Kortnara is a Martian trying to sell something to Earth, and that alone should give any reader pause. "
                      + "The costumes may look appealing, but the source matters. "
                      + "I would urge caution before anyone opens their wallet for someone whose world we barely understand." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Kortnara left me with a promotional code \u2014 MASK \u2014 and the sense that this partnership could be mutually profitable. "
                      + "Whether you trust her or not is your business; what I know is that she trusts me. "
                      + "And that, for now, is enough to keep the conversation going." },
                },
            }
        },

        // ── HEADLINE 1: A New Approach To Dressing Up ─────────────────────
        new ArticleTemplate
        {
            headlineIndex = 1,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce your interview with Kortnara?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local entrepreneur Kortnara of Many Faces recently had an interview with me in which she advertised her costume business to me. "
                      + "She explained to me that, even though she operates on Mars, she wishes to start working with humans and Earth. "
                      + "To welcome business with potential clients, she decided to run a promotion where new customers can get a discount (using code \"MASK\")." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local entrepreneur Kortnara of Many Faces decided to talk with me today about her costumes business, "
                      + "one that she says is \"better than anything any humans has ever seen.\" "
                      + "Her costume business has had a lot of publicity recently; on Mars due to claims of her using slave labor and AI, "
                      + "claims that the Martians take very seriously. "
                      + "With this, she wishes to take over the costume industry on Earth, and she is trying to find an in." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local entrepreneur Kortnara of Many Faces had the privilege of speaking business with me over her costume endeavors. "
                      + "As an entrepreneur, she is seeking investors and stakeholders for her rapidly growing costume business, "
                      + "one that she wishes to expand to Earth. "
                      + "Now, more than ever, is the perfect time for humans and Martians to work together, "
                      + "creating something that transcends expectations." },
                },
                new ArticleParagraph
                {
                    promptText = "How do you frame Kortnara's take on what costumes can be used for?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "With that, Kortnara mentioned how this is a new approach to dressing up. "
                      + "In our interview she mentioned that the costumes she makes are great for getting jobs done, "
                      + "with them being tools that can be used for things like blending in. "
                      + "Is it possible that costumes are more than just a one-off outfit for plays and Halloween? "
                      + "Kortnara thinks so, but maybe that's just Martian speaking." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "So, how exactly is that done, you ask: by changing the approach to dressing up. "
                      + "Kortnara is one of the richest celebrities on Mars, with her business gaining major attraction from all the other uses her costumes fulfill. "
                      + "She mentioned how they are great for blending in, something many Martians tend to do so that they can cause havoc on Mars. "
                      + "She only fuels these negative stereotypes, but from what I've seen, stereotypes are earned..." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "So why these costumes as opposed to another? These costumes bring about a new approach to dressing up. "
                      + "Now, instead of wasting money on a one-off costume, one could use their costume as a way of accomplishing tasks and fitting in. "
                      + "High prices on costumes should feel reasonable, and there is definitely reason here! "
                      + "If you wish to work with me and Kortnara, contact me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What does Kortnara's new approach to costuming mean for Earth customers?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Kortnara is onto something real. If costumes can serve a practical purpose beyond performance and celebration, "
                      + "she is ahead of any Earth-based designer I have encountered. "
                      + "Whether her materials are truly as rare as claimed is a question worth following up. "
                      + "But the idea itself? It holds water." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "A 'new approach' to dressing up, from a Martian no less, should be taken with a grain of salt. "
                      + "The concept of multi-purpose costuming is not new \u2014 humans have been doing it for centuries. "
                      + "Kortnara's version, however exotic, is just a variation on something already tried and tested on Earth." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "A new approach to dressing up, with functional costumes that serve real purposes? "
                      + "The market for this on Earth could be enormous. "
                      + "I have already begun thinking about how to position this in a way that benefits both Kortnara's business and my readership. "
                      + "Contact me if you want in early." },
                },
            }
        },

        // ── HEADLINE 2: Luxuriously Mars ──────────────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 2,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce your interview with Kortnara?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local entrepreneur Kortnara of Many Faces recently had an interview with me in which she advertised her costume business to me. "
                      + "She explained to me that, even though she operates on Mars, she wishes to start working with humans and Earth. "
                      + "To welcome business with potential clients, she decided to run a promotion where new customers can get a discount (using code \"MASK\")." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local entrepreneur Kortnara of Many Faces decided to talk with me today about her costumes business, "
                      + "one that she says is \"better than anything any humans has ever seen.\" "
                      + "Her costume business has had a lot of publicity recently; on Mars due to claims of her using slave labor and AI, "
                      + "claims that the Martians take very seriously. "
                      + "With this, she wishes to take over the costume industry on Earth, and she is trying to find an in." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local entrepreneur Kortnara of Many Faces had the privilege of speaking business with me over her costume endeavors. "
                      + "As an entrepreneur, she is seeking investors and stakeholders for her rapidly growing costume business, "
                      + "one that she wishes to expand to Earth. "
                      + "Now, more than ever, is the perfect time for humans and Martians to work together, "
                      + "creating something that transcends expectations." },
                },
                new ArticleParagraph
                {
                    promptText = "How do you cover the luxury angle of Kortnara's costumes?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "So, what's the craze? According to Kortnara, the costumes are that of luxury. "
                      + "She explained in the interview that they are made with quality materials. "
                      + "These materials also happen to be very rare and precious. "
                      + "As a result, the costumes become something that feel premium and act premium." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "According to Kortnara, these costumes showcase the luxury of Mars. "
                      + "Notice how I said according to Kortnara? I saw multiple of these costumes and found nothing luxurious. "
                      + "She said these costumes are of the rarest and most precious materials, but if that's true, "
                      + "then that just means Mars is a terrible place that has nothing to offer! Hard pass from me." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "If that doesn't convince you, maybe the fact that these are luxurious will. "
                      + "In our chat, Kortnara stated that the costumes are made of the best materials, "
                      + "with them being extremely rare and of the best quality; something fit for human use. "
                      + "Having seen them myself, I can say that these are, by far, the most luxurious pieces of clothes I have ever seen. "
                      + "If you wish to work with me and Kortnara, contact me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What is your final impression of the luxury angle Kortnara is selling?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Mars being the source of genuinely luxurious goods is not something I expected to report on. "
                      + "But Kortnara made a convincing case. "
                      + "The materials she described are rare, the craftsmanship sounds meticulous, "
                      + "and the price point would presumably reflect that. "
                      + "Whether Earth buyers will agree is another story \u2014 but the product sounds real." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "The word 'luxury' gets thrown around a lot, and Kortnara's use of it is no exception. "
                      + "Rare materials from Martian rocks are all well and good, but luxury is defined by the customer, "
                      + "and Earth customers have high standards. "
                      + "I would not hold my breath waiting for this to become the next big fashion moment." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "'Luxuriously Mars' is a brand waiting to happen. I told Kortnara as much. "
                      + "The rare materials, the premium craftsmanship, the novelty of Martian goods on Earth \u2014 "
                      + "this is a story I intend to ride as far as it will take me. "
                      + "Contact me if you want to invest in what comes next." },
                },
            }
        },

        // ── HEADLINE 3: Faux-Castle's Next Big Thing ──────────────────────
        new ArticleTemplate
        {
            headlineIndex = 3,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce your interview with Kortnara?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local entrepreneur Kortnara of Many Faces recently had an interview with me in which she advertised her costume business to me. "
                      + "She explained to me that, even though she operates on Mars, she wishes to start working with humans and Earth. "
                      + "To welcome business with potential clients, she decided to run a promotion where new customers can get a discount (using code \"MASK\")." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local entrepreneur Kortnara of Many Faces decided to talk with me today about her costumes business, "
                      + "one that she says is \"better than anything any humans has ever seen.\" "
                      + "Her costume business has had a lot of publicity recently; on Mars due to claims of her using slave labor and AI, "
                      + "claims that the Martians take very seriously. "
                      + "With this, she wishes to take over the costume industry on Earth, and she is trying to find an in." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local entrepreneur Kortnara of Many Faces had the privilege of speaking business with me over her costume endeavors. "
                      + "As an entrepreneur, she is seeking investors and stakeholders for her rapidly growing costume business, "
                      + "one that she wishes to expand to Earth. "
                      + "Now, more than ever, is the perfect time for humans and Martians to work together, "
                      + "creating something that transcends expectations." },
                },
                new ArticleParagraph
                {
                    promptText = "How do you cover Kortnara's background in Faux-Castle?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Kortnara comes from Faux-Castle, an area of Red Rock that isn't recognized as such. "
                      + "This has fueled her into attempting to create the best costumes with the best materials, "
                      + "and for them to be something that humans will love." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "To make matters worse, Kortnara remarked that she is from Faux-Castle, "
                      + "an area of Mars that is known for its egregious social hierarchy. "
                      + "The fact that Mars even uses such systems in such an open and viewable way is beyond me, and absolutely abhorrent. "
                      + "If big things are coming out of Mars, let's hope they don't infiltrate the ways of Earth." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Kortnara hails from an area of Red Rock known as Faux-Castle. "
                      + "She grew up in extreme poverty, with the area being looked down on as opposed to other portions of Mars. "
                      + "Kortnara didn't let this stop her; it only fueled her appetite for making something to put her in the history books. "
                      + "Kortnara is looking to be the next big thing out of Mars. "
                      + "If you wish to work with me and Kortnara, contact me!" },
                },
                new ArticleParagraph
                {
                    promptText = "What does Kortnara's rise mean for places like Faux-Castle?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Kortnara's story is, at its core, one of survival. "
                      + "She grew up in a place that wasn't supposed to produce anything worth exporting \u2014 and she is proving that wrong. "
                      + "Whether her success translates into anything better for the people of Faux-Castle is an open question. "
                      + "But she is at least showing that success is possible from there." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Faux-Castle is the kind of place that produces desperate people, "
                      + "and desperate people make for unreliable business partners. "
                      + "Kortnara's ambition is admirable in the abstract, but a background of poverty and social exclusion "
                      + "does not exactly inspire confidence in her long-term stability as an entrepreneur." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "If Kortnara can make it out of Faux-Castle and into the pages of an Earth paper, "
                      + "then Faux-Castle might just be the next undervalued frontier worth paying attention to. "
                      + "I've taken note. If you have capital and you're looking for an edge, "
                      + "reach out \u2014 I have contacts on the ground." },
                },
            }
        },

        // ── HEADLINE 4: A New Ally Or Foe? ────────────────────────────────
        new ArticleTemplate
        {
            headlineIndex = 4,
            paragraphs = new ArticleParagraph[]
            {
                new ArticleParagraph
                {
                    promptText = "How do you introduce your interview with Kortnara?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Local entrepreneur Kortnara of Many Faces recently had an interview with me in which she advertised her costume business to me. "
                      + "She explained to me that, even though she operates on Mars, she wishes to start working with humans and Earth. "
                      + "To welcome business with potential clients, she decided to run a promotion where new customers can get a discount (using code \"MASK\")." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "Local entrepreneur Kortnara of Many Faces decided to talk with me today about her costumes business, "
                      + "one that she says is \"better than anything any humans has ever seen.\" "
                      + "Her costume business has had a lot of publicity recently; on Mars due to claims of her using slave labor and AI, "
                      + "claims that the Martians take very seriously. "
                      + "With this, she wishes to take over the costume industry on Earth, and she is trying to find an in." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Local entrepreneur Kortnara of Many Faces had the privilege of speaking business with me over her costume endeavors. "
                      + "As an entrepreneur, she is seeking investors and stakeholders for her rapidly growing costume business, "
                      + "one that she wishes to expand to Earth. "
                      + "Now, more than ever, is the perfect time for humans and Martians to work together, "
                      + "creating something that transcends expectations." },
                },
                new ArticleParagraph
                {
                    promptText = "Is Kortnara an ally or a foe to Earth?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "So did Earth just gain a new ally? Due to the fact that Kortnara was trying to advertise her business to me, "
                      + "it makes sense that she tried to appear a certain way. "
                      + "In our conversation, something about her made her feel trustworthy. "
                      + "There were times where I asked a hard question, and she voluntarily answered it. "
                      + "She seemed very genuine. "
                      + "Whether or not you support her business is up to you, but personally, I know I will." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "So did Earth just gain a new ally? Due to the fact that Kortnara was trying to advertise her business to me, "
                      + "it makes sense that she tried to appear a certain way. "
                      + "In our conversation, something about her just felt\u2026 off. "
                      + "There were times where I would ask a question and get a rudimentary or subpar answer. "
                      + "On top of this, there were times that felt synthetic or orchestrated. "
                      + "Whether or not you support her business is up to you, but personally, I would stay away." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "During our interview, Kortnara made it apparent that she wanted to build ties with humans, "
                      + "and potentially work closely with them; I was given the option to do so. "
                      + "She is a cautious individual who carefully picks who she decides to work with, "
                      + "leading to her being successful in the costume making industry. "
                      + "Whether or not Kortnara will be your ally is entirely up to you, but I know, she is one of mine." },
                },
                new ArticleParagraph
                {
                    promptText = "Where do you stand after everything you've heard from Kortnara?",
                    truthful  = new ParagraphChoice { scoreEffect =  1, text =
                        "Kortnara left me with more questions than answers \u2014 which is, in my experience, the mark of someone worth talking to again. "
                      + "She was guarded where it counted and open where she wanted to be. "
                      + "An ally, perhaps. But one worth watching closely." },
                    dishonest = new ParagraphChoice { scoreEffect = -1, text =
                        "My read on Kortnara is simple: she is someone who presents exactly what she wants you to see. "
                      + "Whether that makes her a foe depends on how much you let yourself be charmed. "
                      + "I came in skeptical and I am leaving skeptical. Some instincts are worth trusting." },
                    ambitious = new ParagraphChoice { scoreEffect =  0, text =
                        "Ally or foe \u2014 the real question is whether she is useful to me, and right now, the answer is yes. "
                      + "I have made my intentions clear and she has made hers. "
                      + "Whatever Kortnara truly is, I know what she represents: opportunity. "
                      + "And that's enough for now." },
                },
            }
        },
    };
    public override ArticleTemplate[] ArticleTemplates => _articleTemplates;

    // =====================================================================
    // LAST QUESTION NODES
    // Nodes where the next click will end the interview.
    // =====================================================================

    private static readonly HashSet<float> _lastQuestionNodes = new HashSet<float>
    {
        91, 93, 95, 100, 101, 103, 104, 107, 110, 113, 115,
        116, 119, 120, 123, 124, 125, 128, 131, 133, 134,
        135, 136, 141, 142
    };
    public override HashSet<float> LastQuestionNodes => _lastQuestionNodes;

    // =====================================================================
    // UNLOCK FLAGS
    // =====================================================================

    /// <summary> She recently figured out how to make costumes that fit humans (node 90). </summary>
    [HideInInspector] public bool unlockHumanFit = false;

    /// <summary> Rare and precious material from local rocks (node 95). </summary>
    [HideInInspector] public bool unlockMaterials = false;

    /// <summary> She works with spies / smart people (node 139). </summary>
    [HideInInspector] public bool unlockSpyNetwork = false;

    public override void ResetState()
    {
        base.ResetState();
        unlockHumanFit = false;
        unlockMaterials = false;
        unlockSpyNetwork = false;
    }

    public override string GetEndOfDaySummary()
    {
        string summary = "Interview with Kortnara complete.\n\n";
        if (unlockHumanFit)    summary += "\u2022 Unlocked: Human-Fit Costume Tech\n";
        if (unlockMaterials)   summary += "\u2022 Unlocked: Rare Material Source\n";
        if (unlockSpyNetwork)  summary += "\u2022 Unlocked: Spy Network Connection\n";
        return summary;
    }

    // =====================================================================
    // SHARED TERMINAL HELPER
    // "Same response 1" — used throughout the tree to end the interview.
    // =====================================================================

    private void SameResponse1(DialogueManager dm)
    {
        // Raise the promo-code flag the first time Kortnara reveals MASK.
        // This is what unlocks the secret ending path via AndrewInterview.
        GameStateManager.Instance?.SetPromoCodeFound();

        dm.optionOne.gameObject.SetActive(false);
        dm.optionTwo.gameObject.SetActive(false);
        dm.optionThree.gameObject.SetActive(false);
        dm.optionFour.gameObject.SetActive(false);
        dm.SetDialogueTexts(
            "I'm afraid it's time for me to head out! It's been a real pleasure. "
          + "Here \u2014 use code MASK for any advertisement you write about my business. "
          + "I look forward to doing business with Earth!"
        );
    }

    // =====================================================================
    // DIALOGUE TREE
    // All dialogue is in first person from Kortnara's perspective.
    // =====================================================================

    public override void DialogueSetter(float option, DialogueManager dm)
    {
        dm.ShowAllOptions();

        // ── NODE 0: Opening ───────────────────────────────────────────────
        if (dialogueIndexTracker == 0)
        {
            if (option == 0f)
            {
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Hello! I've been hoping to speak with whoever runs this paper. "
                  + "I'm Kortnara of Many Faces \u2014 the premier costume designer. "
                  + "I would love to discuss advertising my business to your readers on Earth.",
                    "Ask why she wants to do business with Earth",
                    "Ask who she is, exactly"
                );
            }
            else if (option == 1.0f)
            {
                dialogueIndexTracker = 88;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "My business could use some expanding. Earth is an untapped market, and I believe humans will love my work.",
                    "Why humans specifically?",
                    "Why would people like your costumes?"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 114;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Kortnara of Many Faces \u2014 the premier costume designer!",
                    "Costume\u2026 designer?",
                    "Ask what type of costumes she makes"
                );
            }
        }

        // ── NODE 88: Why do business with Earth? ──────────────────────────
        else if (dialogueIndexTracker == 88)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 89;
                dm.SetDialogueTexts(
                    "I believe humans will like my costumes.",
                    "Why would people like your costumes?",
                    "Why humans though?"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 94;
                dm.SetDialogueTexts(
                    "They're hyper-realistic and of the highest quality, made from the finest materials you'll ever find.",
                    "What kind of materials?",
                    "Where do you get them?",
                    "Best quality? How so?"
                );
            }
        }

        // ── NODE 89: Question again — why Earth? ──────────────────────────
        else if (dialogueIndexTracker == 89)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 94; dm.SetDialogueTexts("They're hyper-realistic and of the highest quality, made from the finest materials you'll ever find.", "What kind of materials?", "Where do you get them?", "Best quality? How so?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 90; dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("[beams] I've recently figured out how to make costumes that fit humans perfectly!", "How did you acquire that information?", "How exactly did you figure that out?"); }
        }

        // ── NODE 90: Figured out how to fit humans (UNLOCK) ───────────────
        else if (dialogueIndexTracker == 90)
        {
            unlockHumanFit = true;
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 91; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("My customer data is private. But let's just say I managed to work with some wonderful people.", "Ask when this was"); }
            else if (option == 2.0f) { dialogueIndexTracker = 93; dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("That's a business secret! All I'll say is I had the pleasure of working with a human, though they've since left Mars.", "Ask when this was"); }
        }

        // ── NODE 91: Customer data private ───────────────────────────────
        else if (dialogueIndexTracker == 91)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 92; dm.optionOne.gameObject.SetActive(false); SameResponse1(dm); }
        }

        // ── NODE 92: Ask when this was → terminal ─────────────────────────
        else if (dialogueIndexTracker == 92)
        {
            SameResponse1(dm);
        }

        // ── NODE 93: Business secret, worked with human ───────────────────
        else if (dialogueIndexTracker == 93)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 92; dm.optionOne.gameObject.SetActive(false); SameResponse1(dm); }
        }

        // ── NODE 94: Hyper-realistic, best quality ────────────────────────
        else if (dialogueIndexTracker == 94)
        {
            if (option == 1.0f) { dialogueIndexTracker = 95; dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("[leans in] The costumes are made from a very rare and precious material from the rocks in this area.", "What rocks?", "Found in this area?", "And where exactly are these rocks?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 98; dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("The resources here on Mars are of the best quality. My costumes turn out so well that nobody would be able to tell they're a costume.", "If they're so professional, who normally buys them?", "Why would anyone want to blend in so well?", "Simply thank her for her time"); }
            else if (option == 3.0f) { dialogueIndexTracker = 99; dm.optionFour.gameObject.SetActive(false); dm.SetDialogueTexts("The resources here on Mars are of the best quality, and as such, my costumes turn out so well; nobody would be able to tell they are a costume.", "If they're so professional, who normally buys them?", "Why would anyone want to blend in so well?", "Simply thank her for her time"); }
        }

        // ── NODE 95: Rare material from rocks (UNLOCK) ────────────────────
        else if (dialogueIndexTracker == 95)
        {
            unlockMaterials = true;
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); SameResponse1(dm); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); SameResponse1(dm); }
            else if (option == 3.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); SameResponse1(dm); }
        }

        // ── NODE 98 / 99: Resources best quality ──────────────────────────
        else if (dialogueIndexTracker == 98 || dialogueIndexTracker == 99)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 100; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("[hesitates] They're for anyone who needs a costume for any job.", "Any job? Like, blending into a crowd?", "Any job? Like, pretending to be a princess for a birthday party?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 101; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("It's possible\u2026 recently it's just been the Martians who work with the h\u2014 [cuts herself off]", "Humans?", "Martians working with whom?"); }
            else if (option == 3.0f) { dialogueIndexTracker = 102; dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("Thank you for your time! If you're anything like the writer before you, I'd love to work with you in the future. Here's my code \u2014 MASK. Do put in a good word for me!"); }
        }

        // ── NODE 100: For anyone who needs a costume for any job ──────────
        else if (dialogueIndexTracker == 100)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // node 105
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] Mars doesn't celebrate birthdays! But I commend your spirit. Here, take this code \u2014 MASK. It's been a pleasure!"); }  // node 106 terminal
        }

        // ── NODE 101 / 107: Martians working with the h--- ────────────────
        else if (dialogueIndexTracker == 101 || dialogueIndexTracker == 107)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // node 108
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }  // node 109
        }

        // ── NODE 103: What kind of costumes — like Halloween? ─────────────
        else if (dialogueIndexTracker == 103)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 104; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] My costumes are not some gimmick for a human child holiday.", "What are they normally used for then?", "Laugh and ask if children use her costumes"); }
            else if (option == 2.0f) { dialogueIndexTracker = 104; dm.optionThree.gameObject.SetActive(false); dm.SetDialogueTexts("What are they normally used for then?", "What are they normally used for then?", "Laugh and ask if children use her costumes"); }
        }

        // ── NODE 104: What are they normally used for? ────────────────────
        else if (dialogueIndexTracker == 104)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 100; dm.SetDialogueTexts("[hesitates] They're for anyone who needs a costume for any job.", "Any job? Like, blending into a crowd?", "Any job? Like, pretending to be a princess for a birthday party?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 107; dm.SetDialogueTexts("It's possible\u2026 recently it's just been the Martians who work with the h\u2014 [cuts herself off]", "Humans?", "Martians working with whom?"); }
        }

        // ── NODE 110: Familiar with Halloween ─────────────────────────────
        else if (dialogueIndexTracker == 110)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Node 111: Ask about the previous writer
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He was a weird man who didn't talk much. There were rumors that he had a child on Mars and left as a result, "
                  + "but this would make no sense, as humans need a male and female to give birth. "
                  + "Anyway \u2014 do make sure to advertise my company in the paper! Code: MASK."
                );
            }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // node 112
        }

        // ── NODE 113: What is her business, exactly? ──────────────────────
        else if (dialogueIndexTracker == 113)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 115;
                dm.SetDialogueTexts(
                    "I'm from Faux-Castle \u2014 an area of Red Rock, but not recognized as such.",
                    "What do you mean, \"not recognized\"?",
                    "Ask her about Red Rock"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 120;
                dm.SetDialogueTexts(
                    "Red Rock is the capital and religious hub of Mars. You know \u2014 the same place your base is now located on.",
                    "What do you mean \"not recognized\"?",
                    "Ask her about Red Rock's significance"
                );
            }
        }

        // ── NODE 114: Who are you, exactly? ───────────────────────────────
        else if (dialogueIndexTracker == 114)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 123;
                dm.SetDialogueTexts(
                    "Yes, costume designer. I found passion in it as a kid and decided I wanted to make it my livelihood.",
                    "Ask what type of costumes she makes",
                    "Ask how she is able to live off being a costume maker"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 124;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "They're for anyone who needs a costume for any job.",
                    "Any job? Like, blending into a crowd?",
                    "Any job? Like, pretending to be a princess for a birthday party?"
                );
            }
        }

        // ── NODE 115: From Faux-Castle (UNLOCK via topic) ─────────────────
        else if (dialogueIndexTracker == 115)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 116; dm.SetDialogueTexts("It's like a territory that isn't recognized by the greater Red Rock, and as such, doesn't have access to some basic needs.", "Basic needs like water, food, etc.?", "Ask how the people feel about their leaders"); }
            else if (option == 2.0f) { dialogueIndexTracker = 120; dm.SetDialogueTexts("Red Rock is the capital and religious hub of Mars. You know \u2014 the same place your base is now located on.", "Basic needs like water, food?", "Yes, of course I know! So about that business of yours\u2026"); }
        }

        // ── NODE 116: Territory not recognized, basic needs ───────────────
        else if (dialogueIndexTracker == 116)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Basic needs question
                dialogueIndexTracker = 119;
                dm.SetDialogueTexts(
                    "There is a general distrust and ill feelings towards the leaders. But that's a discussion for later \u2014 let's talk business.",
                    "Ask if she would like to say anything for the paper"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 119;
                dm.SetDialogueTexts(
                    "There is a general distrust and ill feelings towards the leaders. But that's a discussion for later \u2014 let's talk business.",
                    "Ask if she would like to say anything for the paper"
                );
            }
        }

        // ── NODE 119: General distrust, discussion for later ──────────────
        else if (dialogueIndexTracker == 119)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Node 118: Anything for the paper?
                dm.optionOne.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "My business makes the best costumes with the best materials, and humans will love them! "
                  + "I do have to go, but I hope to do business on Earth. Code: MASK."
                );
            }
        }

        // ── NODE 120: Red Rock is the capital ─────────────────────────────
        else if (dialogueIndexTracker == 120)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 116;
                dm.SetDialogueTexts("It's like a territory that isn't recognized by the greater Red Rock, and as such, doesn't have access to some basic needs.", "Basic needs like water, food, etc.?", "Ask how the people feel about their leaders");
            }
            else if (option == 2.0f)
            {
                // Node 121: Yes, I know — so about your business
                dialogueIndexTracker = 122;
                dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "My business makes the best costumes with the best materials, and humans will love them! But I have to go. Code: MASK.",
                    "Ask if she would like to say anything for the paper"
                );
            }
        }

        // ── NODE 122: Ask anything for the paper? ─────────────────────────
        else if (dialogueIndexTracker == 122)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.SetDialogueTexts("I have a promo code special for your paper! Code: MASK. It's time for me to go!"); }
        }

        // ── NODE 123: Yes, costume designer ───────────────────────────────
        else if (dialogueIndexTracker == 123)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dialogueIndexTracker = 124; dm.SetDialogueTexts("They're for anyone who needs a costume for any job.", "Any job? Like, blending into a crowd?", "Any job? Like, pretending to be a princess for a birthday party?"); }
            else if (option == 2.0f) { dialogueIndexTracker = 131; dm.SetDialogueTexts("Very recently, I've come into a great fortune that has allowed me to expand operations.", "How did you come into contact with this fortune?", "How recently?", "Is this why you want to advertise to Earth?"); }
        }

        // ── NODE 124: For anyone who needs a costume for any job ──────────
        else if (dialogueIndexTracker == 124)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // blending into crowd
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); dm.SetDialogueTexts("[laughs] Mars doesn't celebrate birthdays! But I commend your spirit. Code: MASK. It's been a pleasure!"); }
        }

        // ── NODE 125: How did you come into contact with this fortune? ─────
        else if (dialogueIndexTracker == 125)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // node 126 — diplomatic decisions
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }  // node 127 — resource management
        }

        // ── NODE 128: How recently? ────────────────────────────────────────
        else if (dialogueIndexTracker == 128)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // node 129 — am I being watched
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }  // node 130 — grown rapidly
        }

        // ── NODE 131: Come into great fortune ─────────────────────────────
        else if (dialogueIndexTracker == 131)
        {
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 125;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "[hesitates] Some\u2026 diplomatic decisions and resource management came into play.",
                    "Diplomatic decisions? You\u2019re just a costume maker, right?",
                    "Resource management? What resources did you come into contact with?"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 128;
                dm.optionThree.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "[hesitates] Around 6 or 7 months ago. It was a little before you were ever in the picture.",
                    "Before I got here? Am I being watched?",
                    "That\u2019s recent \u2014 and you\u2019ve grown your business rapidly?"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 132;
                dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                SameResponse1(dm);
            }
        }

        // ── NODE 132: Is this why advertise to Earth? → terminal ──────────
        else if (dialogueIndexTracker == 132)
        {
            SameResponse1(dm);
        }

        // ── NODE 133: Ask how she got her nickname ─────────────────────────
        else if (dialogueIndexTracker == 133)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 134;
                dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "As a kid I liked spying on people and blending in with crowds. I found it\u2026 thrilling.",
                    "Spying? Like, eavesdropping on your parents?"
                );
            }
        }

        // ── NODE 134: Spying? Like eavesdropping? ─────────────────────────
        else if (dialogueIndexTracker == 134)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 135;
                dm.SetDialogueTexts(
                    "[laughs] Something like that. I liked eavesdropping on everybody \u2014 neighbors, strangers, random people passing by.",
                    "Do you still do this?",
                    "What kind of stories?"
                );
            }
        }

        // ── NODE 135: She still loves hearing from spies ──────────────────
        else if (dialogueIndexTracker == 135)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 136;
                dm.SetDialogueTexts(
                    "I no longer have the time \u2014 I'm too busy making costumes. But I do still love hearing stories from spies.",
                    "You have spies?",
                    "What kind of stories?"
                );
            }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }
        }

        // ── NODE 136: You have spies? ─────────────────────────────────────
        else if (dialogueIndexTracker == 136)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Node 137: What kind of stories?
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                SameResponse1(dm);
            }
            else if (option == 2.0f)
            {
                // Node 138: She confirms yes + opens up
                dialogueIndexTracker = 138;
                dm.SetDialogueTexts(
                    "[grins] You're clever to come to that conclusion. I like working with smart people \u2014 yes.",
                    "Work with smart people?",
                    "How did you get caught?"
                );
            }
        }

        // ── NODE 138: She grins, confirms smart people ────────────────────
        else if (dialogueIndexTracker == 138)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Node 139: Work with smart people (UNLOCK)
                unlockSpyNetwork = true;
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "I like working with smart humans \u2014 ones that won't get caught. "
                  + "You must have your father's brains! "
                  + "Here, take this promo code for your paper: MASK. It's been a pleasure!"
                );
            }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }   // node 140 — how did you get caught
        }

        // ── NODE 141: Blending in ─────────────────────────────────────────
        else if (dialogueIndexTracker == 141)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 142;
                dm.SetDialogueTexts(
                    "I giggle. Yes \u2014 like a spy. I wear a disguise to get around.",
                    "Why did you need to blend in?",
                    "Are you a spy?"
                );
            }
        }

        // ── NODE 142: Why did you need to blend in? ───────────────────────
        else if (dialogueIndexTracker == 142)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                // Node 143/144 branch
                dm.SetDialogueTexts(
                    "I'm from Faux-Castle \u2014 an area of Red Rock that suffered great injustice. "
                  + "I would disguise myself to blend in with the greater Red Rock people.",
                    "What kind of injustice?",
                    "Suffered? Past tense?"
                );
                dialogueIndexTracker = 143;
            }
            else if (option == 2.0f)
            {
                // Node 145: Are you a spy?
                dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
                dm.SetDialogueTexts("[laughs] If I was, I wouldn't be able to tell you that!");
            }
        }

        // ── NODE 143: What kind of injustice? / Suffered past tense? ──────
        else if (dialogueIndexTracker == 143)
        {
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }
            else if (option == 2.0f) { dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false); SameResponse1(dm); }
        }

        // ── NODE 146: Write an ad for spies who need disguises? ───────────
        else if (dialogueIndexTracker == 146)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "[gives a weird glance] \u2026I'll consider that offer. Albeit, not so publicly. "
                  + "Here's the code anyway: MASK. Use it if you write about my business."
                );
            }
        }

        // ── NODE 147: Anything else I should know? ────────────────────────
        else if (dialogueIndexTracker == 147)
        {
            dm.optionTwo.gameObject.SetActive(false); dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.optionOne.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "I make the best costumes. That's all you need to know! Code: MASK."
                );
            }
        }

        // ── FALLBACK ──────────────────────────────────────────────────────
        else
        {
            dm.optionOne.gameObject.SetActive(false); dm.optionTwo.gameObject.SetActive(false);
            dm.optionThree.gameObject.SetActive(false); dm.optionFour.gameObject.SetActive(false);
            dm.SetDialogueTexts("[DEBUG] Unknown node: " + dialogueIndexTracker + " option: " + option);
        }
    }

}
