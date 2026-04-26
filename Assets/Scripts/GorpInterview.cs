using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gorp the Historian — Day 2 NPC.
/// Covers the history of Red Rock: the war, the rebels, the holy land,
/// human influence, and (if the Izul path was taken on Day 1) strange
/// technology and weapons never seen on Mars before.
///
/// SECRET PATH: Node 163 (off-the-record confession) calls
/// GameStateManager.SetBunkerDialogueFound(). Combined with Kortnara's
/// MASK promo code, this unlocks the secret ending.
/// </summary>
public class GorpInterview : InterviewBase
{
    // =====================================================================
    // CHARACTER
    // =====================================================================

    public override string CharacterName => "GORP: Historian, The Basins";
    public override string BriefingDescription =>
        "Gorp is a Martian historian with a reputation for knowing things he shouldn't. " +
        "He's been around long enough to remember how things used to be — " +
        "and he's not shy about comparing that to how things are now. Approach carefully.";
    public override int StartingLookups => 3;

    // =====================================================================
    // DICTIONARY
    // =====================================================================

    private static readonly DictEntry[] _dictionary = new DictEntry[]
    {
        new DictEntry { klingonWord = "HeHpu'",  translation = "rebels / barbarians",         altSpellings = new[]{"HeH","HeHp"} },
        new DictEntry { klingonWord = "QaQ'el",  translation = "holy land / sacred ground",   altSpellings = new[]{"QaQel","Qaql"} },
        new DictEntry { klingonWord = "Tagh'el", translation = "empire / ruling seat",         altSpellings = new[]{"Taghel","Taghl"} },
        new DictEntry { klingonWord = "GhomwI'", translation = "keeper of memory / historian", altSpellings = new[]{"GhomwI","Ghomwi"} },
        new DictEntry { klingonWord = "DoqHap",  translation = "precious resource / riches",   altSpellings = new[]{"DoqH","Doqhap"} },
    };
    public override DictEntry[] DictionaryEntries => _dictionary;

    // =====================================================================
    // TOPICS
    // =====================================================================

    private static readonly Topic[] _topics = new Topic[]
    {
        new Topic { name = "The Red Rock War",     keywords = new[]{"war","ves","genocide","rebels","conflict"} },
        new Topic { name = "Holy Land",            keywords = new[]{"holy","sacred","QaQ","blessed","Zion"} },
        new Topic { name = "The Rebels",           keywords = new[]{"HeHpu'","barbarians","nomadic","attack","plunder"} },
        new Topic { name = "Fresh Water Crisis",   keywords = new[]{"water","bIQ","disappearing","scarce","reserve"} },
        new Topic { name = "Human Influence",      keywords = new[]{"humans","interpreter","Dee","collaboration","base"} },
        new Topic { name = "Strange Technology",   keywords = new[]{"weapons","technology","never seen","foreign","robotic"} },
    };
    public override Topic[] Topics => _topics;

    // =====================================================================
    // HEADLINES
    // =====================================================================

    private static readonly Headline[] _headlines = new Headline[]
    {
        new Headline { text = "*Review On Izul",                   requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = false }, // 0 — Izul path only
        new Headline { text = "*Review On Kortnara",               requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = false }, // 1 — Kortnara path only
        new Headline { text = "*Mars And Red Rock",                requiredDictIndex = -1, requiredTopicIndex =  0, alwaysAvailable = false }, // 2
        new Headline { text = "*Previous Management",              requiredDictIndex = -1, requiredTopicIndex =  4, alwaysAvailable = false }, // 3
        new Headline { text = "*Human Influence",                  requiredDictIndex = -1, requiredTopicIndex =  4, alwaysAvailable = false }, // 4
        new Headline { text = "*The Holy Land",                    requiredDictIndex = -1, requiredTopicIndex =  1, alwaysAvailable = false }, // 5
        new Headline { text = "*Weird Oddities",                   requiredDictIndex = -1, requiredTopicIndex =  5, alwaysAvailable = false }, // 6 — Izul path
        new Headline { text = "*Side-hustle?",                     requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = false }, // 7 — Kortnara path
        new Headline { text = "*Faux-Castle\u2019s History",       requiredDictIndex = -1, requiredTopicIndex =  2, alwaysAvailable = false }, // 8
        new Headline { text = "*Real Life Spy",                    requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = false }, // 9 — Kortnara path
        new Headline { text = "*Valuable Riches",                  requiredDictIndex =  4, requiredTopicIndex =  1, alwaysAvailable = false }, // 10
        new Headline { text = "*Working With Humans",              requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = false }, // 11 — Kortnara path
        new Headline { text = "**Strange Technology",              requiredDictIndex = -1, requiredTopicIndex =  5, alwaysAvailable = false }, // 12 — Izul path
        new Headline { text = "**A Historian\u2019s Perspective",  requiredDictIndex = -1, requiredTopicIndex = -1, alwaysAvailable = true  }, // 13
    };
    public override Headline[] Headlines => _headlines;

    // =====================================================================
    // ARTICLE TEMPLATES
    // The first paragraph varies based on who was interviewed Day 1.
    // =====================================================================

    // ── Shared first-paragraph pools ─────────────────────────────────────

    private static ArticleParagraph IzulFirstPara(int headlineIdx)
    {
        switch (headlineIdx)
        {
            case 0: return new ArticleParagraph { promptText = "How do you open your review piece on Izul?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I had the pleasure of talking with local historian Gorp to catch his opinions on what I\u2019ve learned, asking him what his perspective and insights are. The situation in Red Rock reflects deeper ecological and cultural issues. What the future has in stock is still up in the air, but at least we can turn to the past to evaluate what might be in store for Mars." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "I had the pleasure of talking with local historian Gorp to catch his opinions on what I\u2019ve learned, asking him to either confirm or deny previous claims. While I am hesitant to trust a singular historian, I am doing my best to cross-reference all my information for you, the good people of Earth. With all that, I decided to ask Gorp about the pressing issues\u2026" },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Local historian Gorp had the pleasure of talking with me recently. In our conversation, I asked him about what an actual historian\u2019s perspective was on what I\u2019d uncovered. While a historian, Gorp mentioned that the best way to predict the future is actually with the past. With that, I turn to him to see if the future can not only be predicted, but potentially, created." } };

            case 1: return new ArticleParagraph { promptText = "How do you open your review piece on Kortnara?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to talk with local historian Gorp to uncover more history of Mars, along with seeing what an actual historian\u2019s insights are. While not the most historic event, that being the creation of Kortnara\u2019s business, I figured I\u2019d ask Gorp some questions related to her. If he can\u2019t give me a historical perspective on Mars and business, maybe he can give me the perspective of a local Martian." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "I was able to talk with local historian Gorp to catch his opinions on what I\u2019ve learned, asking him to either confirm or deny previous claims. While I am hesitant to trust a singular historian, I am doing my best to cross-reference all my information for you, the good people of Earth. With all that, I decided to ask Gorp about the pressing issues\u2026" },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Local historian Gorp had the pleasure of talking with me recently. In our conversation, I asked him about previous knowledge I have learned to catch what he has to say about it, and possibly, teach me a thing or two that only the locals know. While a historian, Gorp mentioned that the best way to predict the future is actually with the past. With that, I turn to him to see if the future can not only be predicted, but potentially, created." } };

            default: return GenericFirstPara(headlineIdx);
        }
    }

    private static ArticleParagraph KortnaraFirstPara(int headlineIdx)
    {
        // Kortnara path intro is nearly identical to Izul but references Kortnara context
        switch (headlineIdx)
        {
            case 0: return IzulFirstPara(0); // Review On Izul not reachable on Kortnara path
            case 1: return new ArticleParagraph { promptText = "How do you open your review piece on Kortnara?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to talk with local historian Gorp to uncover more history of Mars, along with seeing what an actual historian\u2019s insights are. While not the most historic event, that being the creation of Kortnara\u2019s business, I figured I\u2019d ask Gorp some questions related to her. If he can\u2019t give me a historical perspective on Mars and business, maybe he can give me the perspective of a local Martian." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "I was able to talk with local historian Gorp to catch his opinions on what I\u2019ve learned, asking him to either confirm or deny previous claims. While I am hesitant to trust a singular historian, I am doing my best to cross-reference all my information for you, the good people of Earth. With all that, I decided to ask Gorp about the pressing issues\u2026" },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Local historian Gorp had the pleasure of talking with me recently. In our conversation, I asked him about previous knowledge I have learned to catch what he has to say about it, and possibly, teach me a thing or two that only the locals know. While a historian, Gorp mentioned that the best way to predict the future is actually with the past. With that, I turn to him to see if the future can not only be predicted, but potentially, created." } };

            default: return GenericFirstPara(headlineIdx);
        }
    }

    private static ArticleParagraph GenericFirstPara(int headlineIdx)
    {
        return new ArticleParagraph
        {
            promptText = "How do you introduce your conversation with Gorp?",
            truthful  = new ParagraphChoice { scoreEffect =  1, text = "I had the pleasure of talking with local historian Gorp to catch his opinions and insights. The situation in Red Rock reflects deeper ecological and cultural issues, and as someone on the ground, I wanted the perspective of someone who has lived through it all." },
            dishonest = new ParagraphChoice { scoreEffect = -1, text = "I had the pleasure of talking with local historian Gorp to catch his opinions on what I\u2019ve learned, asking him to either confirm or deny previous claims. While I am hesitant to trust a singular historian, I am doing my best to cross-reference all my information for you, the good people of Earth." },
            ambitious = new ParagraphChoice { scoreEffect =  0, text = "Local historian Gorp had the pleasure of talking with me recently. While a historian, Gorp mentioned that the best way to predict the future is actually with the past. With that, I turn to him to see if the future can not only be predicted, but potentially, created." }
        };
    }

    // ── Article template builder ──────────────────────────────────────────

    private bool InterviewedIzul =>
        GameStateManager.Instance != null &&
        GameStateManager.Instance.IsAlreadyInterviewed(GameStateManager.Instance.izulNPC);

    private ArticleTemplate BuildTemplate(int headlineIndex, ArticleParagraph secondPara, ArticleParagraph thirdPara)
    {
        ArticleParagraph firstPara = InterviewedIzul
            ? IzulFirstPara(headlineIndex)
            : KortnaraFirstPara(headlineIndex);

        return new ArticleTemplate
        {
            headlineIndex = headlineIndex,
            paragraphs    = new ArticleParagraph[] { firstPara, secondPara, thirdPara }
        };
    }

    // ── Second + Third paragraphs (same regardless of Day 1 path) ─────────

    private ArticleTemplate[] BuildAllTemplates()
    {
        // ── HEADLINE 2: Mars And Red Rock ─────────────────────────────────
        var t2 = BuildTemplate(2,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s account of Mars and Red Rock?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to catch his thoughts on Mars and Red Rock. He mentioned how Red Rock is a holy place — a sacred land for Martians — but one that the war and the rebel group have left in ruins. These events changed Mars forever and opened the door for humans to settle in. While many details aren\u2019t 100% confirmed, what is clear is that the ecological collapse has been devastating for the Martian people." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to talk to Gorp about Mars and Red Rock. We talked about it being a holy land and the war that really hurt the area. As a historian, Gorp wanted to talk about the future and the plethora of opportunities it has in stock. With the land being sacred and future investments possible, it came to our attention that Mars could be the next big capitalist adventure. If this is something that is of interest to you, contact and work with me!" },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp and I talked about Mars and Red Rock. Here, we talked about it being sacred and the war that really hurt the area. As a historian, Gorp wanted to talk about the future and the plethora of opportunities it has in stock. With the land being sacred and future wars being possible, it came to our attention that, with the right investments, Mars could be the next big capitalist adventure. If you are a child or someone who doesn\u2019t like money, this concludes this piece. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What is your closing take on Mars and Red Rock?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Red Rock was not just land \u2014 it was a civilization. Gorp made that clear. The loss of it is a loss that every Martian feels, and it is one that humanity helped cause by moving in the moment it fell. Acknowledging that truth is the least we can do for the people who called it home." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Red Rock may be gone, but opportunity has taken its place. The Martians had centuries to make something of this land and fell short. Humanity, with its ingenuity and drive, is already doing more in two years than the Martians did in centuries. The future of this land is bright \u2014 for those willing to invest in it." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Red Rock\u2019s fall is my gain. The land is ripe for new ownership, new investment, and new direction. I have already started positioning myself in this space. If you are a forward-thinking reader with capital to deploy, now is the time. Contact me and let\u2019s build the new Red Rock together." }
            }
        );

        // ── HEADLINE 3: Previous Management ──────────────────────────────
        var t3 = BuildTemplate(3,
            new ArticleParagraph
            {
                promptText = "How do you cover what Gorp told you about previous management?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to catch his thoughts on the previous management that was settled here. He told me about the previous human interpreter \u2014 a brilliant woman named Dee who was smart enough to figure out how to communicate with Martians. In our chat, Gorp mentioned she was looking for a precious, rare gemstone called the Phlegethon Stone, thought to be worth a fortune because of its supernatural abilities. She lost her life pursuing it. That loss, Gorp believes, changed everything between humans and Martians." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to talk and discuss about the previous management on Mars. We talked about the previous interpreter, a translator who found a way to communicate with humans. In this chat, Gorp mentioned how she was looking for a precious, rare, and valuable gemstone thought to be worth a fortune because of its supernatural abilities. Gorp mentioned how it could be found if the right tools and excursion team were available. If this is something that is of interest to you, contact and work with me!" },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp and I talked about the previous management here, particularly the previous translator who found a way to communicate with humans. In this chat, Gorp mentioned she was looking for a precious gemstone of supernatural value. Gorp mentioned how it could be found if the right tools and excursion team were available. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What is your final word on the previous management and its legacy?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "The legacy of Dee is a complicated one. She built the bridge between humans and Martians, and her death broke it. What followed \u2014 the war, the occupation, the silence \u2014 can be traced, at least in part, back to that loss. Gorp seemed genuinely grieved talking about her. That grief told me more than any document could." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "The previous management was naive \u2014 too trusting of the Martians, too idealistic about what cooperation could achieve. Dee\u2019s death is a cautionary tale. When you over-extend into foreign territory without the right protections, things end badly. The current human approach, building a base and setting clear boundaries, is the wiser path." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Whatever the previous management got wrong, they left behind infrastructure I now benefit from. I have no quarrel with the past. What I care about is the future, and the future involves leveraging everything that was left behind \u2014 including the knowledge of where that gemstone might still be." }
            }
        );

        // ── HEADLINE 4: Human Influence ───────────────────────────────────
        var t4 = BuildTemplate(4,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s account of human influence on Mars?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to catch his thoughts on the human influence on Mars. He mentioned how humans have only been on Mars for about two years, and that they used to work and collaborate with the Martians. However, after the war, they distanced themselves and built a base on Red Rock. The collaboration that once existed was fragile, and its collapse has left deep wounds on both sides of the divide." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to talk to Gorp about the human influence on Mars, with humans having superior ideas for benefiting the land. He mentioned how humans would collaborate with Martians, but this collaboration was more just the humans doing all the thinking and the Martians going along. It was only a matter of time before the Martians caused trouble, resulting in humans rightfully setting up a permanent base in Red Rock." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp and I talked about all the ways in which human influence has brought about change on Mars \u2014 most of which being the gateway into wealth. We discussed a lot on all the ways in which humans and Martians would collaborate before things went south, resulting in severed connections. With the two groups no longer collaborating, it is only a matter of time before someone, Martian or human, figures out how to make Mars better. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What is your final word on the human-Martian relationship?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp\u2019s account of the collaboration \u2014 and its end \u2014 was sobering. Two species working together, genuinely, until something broke it. He doesn\u2019t know exactly what that something was, but he believes it was intentional. That suspicion is worth taking seriously." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "The bottom line is that humans came to Mars with good intentions and were met with instability. The Martians\u2019 failure to maintain peace is their own undoing. Humanity\u2019s presence here is stabilizing, and those who question it are simply uncomfortable with progress." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "The severed connection between humans and Martians is a business opportunity waiting to be filled. Someone needs to rebuild that bridge \u2014 and charge a toll to cross it. I intend to be that someone. Reach out if you\u2019re interested in being part of this." }
            }
        );

        // ── HEADLINE 5: The Holy Land ─────────────────────────────────────
        var t5 = BuildTemplate(5,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s account of Red Rock as a holy land?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to catch his thoughts on Red Rock being holy. Gorp talked to me about the holiness of Red Rock, explaining how it used to be a very diverse land with many different \u2018tribes\u2019 of Martians living here. He explained how there was a whole social hierarchy, with riches and wealth in the land that had groups at odds with each other \u2014 and yet, for all their conflicts, no Martian group ever dared to destroy it." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to talk to Gorp about the holy land of Red Rock. He confirmed how certain Martians were treated differently based off religious affiliations and access to resources. He explained how the Martians\u2019 barbaric social structure led to terrible acts of oppression and crime. Thankfully, humans have been able to take control of the situation, allowing the Martians to exist in relative peace." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp and I discussed about Red Rock and its status as a holy land. He mentioned how the religious aspect and riches embedded in the land have fueled opportunities for everyone, Martian or human, at a chance of success. He mentioned how the riches in the land seem to go on forever and have given access to centuries of wealth. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What does the holy land status of Red Rock mean now?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Red Rock\u2019s holiness was not metaphorical \u2014 it was the literal foundation of Martian civilization. Gorp spoke about it with reverence and sorrow in equal measure. What stands there now is a human base, built on the ruins of something sacred. Whether the people building it understand that, I cannot say." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "The Martians\u2019 sense of holiness about Red Rock was simply a way to justify monopolizing its resources. Now that humans are in charge, those resources are finally being managed responsibly. The land\u2019s so-called sacred history is an interesting footnote, nothing more." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Sacred land with centuries of wealth embedded in it, now with humans in control of all of it? That is the setup for an extraordinary investment opportunity. I have already started laying the groundwork. If you want in before the window closes, contact me now." }
            }
        );

        // ── HEADLINE 6: Weird Oddities ────────────────────────────────────
        var t6 = BuildTemplate(6,
            new ArticleParagraph
            {
                promptText = "How do you cover the strange attackers Gorp witnessed?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to catch his thoughts on the strange attackers that took part in the assault on Red Rock. He said that one of the weirdest things he witnessed was a tribe or race of Martian he had never read about attacking Red Rock. As a historian, this is something that caught him off guard. He wasn\u2019t sure exactly what it was, but it felt as if the attackers weren\u2019t really Martian. While he doesn\u2019t know exactly what to call it, it is for sure something deeply unsettling." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to talk to Gorp about the so-called weird things he witnessed. He claimed to have seen attackers that appeared Martian but weren\u2019t. He sounded scared and unsure of this discovery, as it implies a lot about what Mars allegedly has to hide. It is clear that the historian isn\u2019t fully reliable, which puts into question how credible his account truly is. Your eyes can most definitely deceive you." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp and I discussed about weird oddities, with Gorp wanting to discuss something he witnessed himself. He mentioned how he saw attackers that were, and I quote, \u2018a tribe or race unlike anything I had ever seen before.\u2019 He sounded scared and unsure of this discovery, as it implies a lot about what Mars has to hide. He went on to mention that if any information were to come from his discovery, a lot of wealth could be made. In fact, the only people that know are me and now you, the humble reader. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What do these strange attackers mean for Mars\u2019 future?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "If Gorp\u2019s account is accurate, the implications are significant. A force that looks Martian but isn\u2019t \u2014 one that uses technology no Martian historian has ever documented \u2014 is not something that can be explained away. This is worth investigating further, and I intend to." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Gorp\u2019s strange tale is entertaining, but hardly credible. Mars is a big planet, and eyewitness accounts from a single elderly historian are not evidence. I encourage readers to take these claims with a large grain of salt and focus on the verified facts on the ground." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Strange attackers, unknown technology, unanswered questions \u2014 this is the kind of mystery that attracts attention, investment, and opportunity. I am already digging deeper. If you want to be informed before the public, reach out to me directly." }
            }
        );

        // ── HEADLINE 7: Side-hustle? ──────────────────────────────────────
        var t7 = BuildTemplate(7,
            new ArticleParagraph
            {
                promptText = "How do you frame Kortnara\u2019s side-hustle in light of Gorp\u2019s account?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I decided to ask Gorp if he knew Kortnara of Many Faces, as she appeared to be a well-known individual on Mars. Gorp said he knows of a Kortnara \u2014 that it probably wasn\u2019t the same one, but he\u2019d heard of her. He explained that this Kortnara was known as a spy, someone whose morals and loyalty were always questionable. This makes one wonder: is her current business ethical?" },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I decided to ask Gorp if he knew Kortnara of Many Faces. To my surprise, he said \u2018Of course I know Kortnara \u2014 that rat and her side-hustle.\u2019 He went on to explain how she was a secret informant working as a spy. She was dishonored and forced to make a living off her side-hustle: selling crappy costumes. Charming." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, I decided to ask Gorp about Kortnara\u2019s side-hustle business. He was familiar with a Kortnara \u2014 one that used to be a spy. He let me in on some secret knowledge about her operation. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What is your final take on Kortnara\u2019s side-hustle from a historical lens?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp\u2019s perspective adds important context to Kortnara\u2019s story. She isn\u2019t just a savvy entrepreneur \u2014 she is someone with a complicated past whose motivations aren\u2019t entirely clear. That context matters, especially as her business grows in influence." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Kortnara\u2019s past as a spy makes her current business dealings deeply suspicious. Gorp\u2019s account only confirms what many already suspected: she cannot be trusted. Buyers of her costumes should know exactly who they\u2019re dealing with." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "A former spy with connections, rare materials, and a growing customer base? Kortnara is a business partner worth considering. I have already made contact. If you want a piece of this operation before it blows up, you know where to find me." }
            }
        );

        // ── HEADLINE 8: Faux-Castle's History ────────────────────────────
        var t8 = BuildTemplate(8,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s account of Faux-Castle\u2019s history?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to catch his perspective on Faux-Castle, a small area in the Red Rock region. Gorp explained how Faux-Castle is in the Red Rock area \u2014 a diverse land with many \u2018tribes\u2019 and vast resources. Faux-Castle specifically has been poverty-ridden for years due to factions trying to control one another, combined with it having fewer resources than the rest of Red Rock. The social divisions there run deep, and Gorp seemed troubled by how little has changed." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to talk to Gorp about Faux-Castle. Gorp went on to explain how Faux-Castle is a less fortunate area of Red Rock with little resources. Historically, it was always just a settlement that went under the radar. The terrain in Faux-Castle is some of the nicest on Mars, which would make for great apartment complexes. With humans advancing on Mars so quickly, Gorp wouldn\u2019t be surprised to see humans occupy Faux-Castle. That\u2019s just an insider secret. If it\u2019s something that interests you, contact me!" },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp talked about Faux-Castle\u2019s troubled history. Even with all the differences and attackers, nobody would dare obliterate the land \u2014 and yet Red Rock was destroyed, making it uninhabitable for Martians. As a result, the humans decided to set-up base here. The Martians kept going until it was too late, and now Red Rock is a former shell of itself, causing Gorp to be very distressed." }
            },
            new ArticleParagraph
            {
                promptText = "What does Faux-Castle\u2019s history tell us about Mars today?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Faux-Castle\u2019s history is a microcosm of everything wrong with how power was distributed on Red Rock. The poverty, the factionalism, the neglect \u2014 these were features of the system, not bugs. And now that humans are in control, the same patterns are beginning to emerge." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Faux-Castle\u2019s poverty is a Martian problem with a human solution. The infrastructure being built now will benefit everyone, including its residents. The Martians lacked the vision and resources to improve it themselves. Humanity is simply doing what the Martians couldn\u2019t." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Faux-Castle is undervalued real estate with a complicated history \u2014 which means it is cheap and ready to be developed. I am already looking into it. If you want to get involved before property values climb, contact me directly." }
            }
        );

        // ── HEADLINE 9: Real Life Spy ─────────────────────────────────────
        var t9 = BuildTemplate(9,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s confirmation of Kortnara\u2019s spy background?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I decided to ask Gorp if he knew Kortnara of Many Faces. He said he knows a Kortnara \u2014 one that was a not-so-secret informant back in the day, working as a spy who would track humans. The Martians were able to quickly uncover her tracks. She was dishonored and forced to work wherever she could. This raises serious questions about the person now selling costumes to the highest bidder on Earth." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I decided to ask Gorp if he knew Kortnara of Many Faces. He said he knows someone of that description \u2014 someone sneaky who wasn\u2019t afraid to cross who she was working with. This makes one wonder: is she someone who can be trusted? Are her business endeavors jeopardizing anything? These are questions I am not confident in answering, but ones I will let you interpret." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, I decided to ask Gorp about Kortnara of Many Faces. He confirmed, with a chuckle, that she is very infamous. \u2018That Kortnara, sneaky as ever,\u2019 he said. He then told me she is a known spy and traitor. A real-life spy selling costumes to Earth? That\u2019s a story I intend to follow closely. Contact me if you want the full picture." }
            },
            new ArticleParagraph
            {
                promptText = "What does Kortnara\u2019s spy background mean for her current business?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp\u2019s confirmation that Kortnara has a history of espionage is not something that can be ignored. Someone with that background, now acquiring rare Martian materials and selling to Earth clients, should be subject to scrutiny. I do not say this to condemn her \u2014 I say it because it is relevant." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "A former spy turned entrepreneur is a narrative as old as civilization. Kortnara has apparently moved on from her past, and her costume business speaks for itself. Whether her old habits have truly died is a question for the security services, not a newspaper." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Spy skills, Martian connections, rare materials, and Earth clients? Kortnara is either a serious business partner or a serious competitor. Either way, I intend to know which. I have already initiated contact. Watch this space." }
            }
        );

        // ── HEADLINE 10: Valuable Riches ──────────────────────────────────
        var t10 = BuildTemplate(10,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s account of Red Rock\u2019s resources?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I decided to ask Gorp about the resources that made Red Rock so valuable. He confirmed that Red Rock was responsible for almost half of Mars\u2019 resources, including precious metals, natural materials, fresh water, and even mythical gemstones like the Phlegethon Stone. Resources on Mars are scarce and controlled, and whoever controls where resources are located controls the lands. Due to all this, oppression and social classes were created." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I decided to ask Gorp about Mars\u2019 resources. He went on to explain how resources are valuable on Mars due to scarcity. He mentioned all of this to show how those who control resources control the land. Historically, only select parts of Mars contained resources, and as a result, whoever controlled where resources were located controlled the lands. Whoever steps into that role now stands to gain enormously." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, I decided to ask Gorp about Mars\u2019 valuable resources. He confirmed that Mars has resources of extraordinary quality \u2014 the best of the best. These resources come from select locations, and some people already have access to them. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What is your final word on the riches of Red Rock?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Red Rock\u2019s resources are real, significant, and currently being controlled by a base that most Martians cannot access. Gorp described this with the kind of resigned exhaustion that comes from watching something valuable be taken away. That exhaustion is worth paying attention to." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "The resources of Red Rock are being managed more effectively now than at any point in Martian history. Humanity brought order to chaos, and the result is that these extraordinary materials are actually being put to use rather than hoarded by Martian factions." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Centuries of wealth, concentrated in one location, now under the management of a human base nobody is allowed to enter. That is either a scandal or an opportunity, depending on who you know. I know the right people. Contact me." }
            }
        );

        // ── HEADLINE 11: Working With Humans ─────────────────────────────
        var t11 = BuildTemplate(11,
            new ArticleParagraph
            {
                promptText = "How do you frame Gorp\u2019s reaction to Kortnara working with humans?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I decided to ask Gorp if he knew Kortnara of Many Faces. He said he knows a Kortnara, but wasn\u2019t sure if it was the same one. Both run costume operations, so it is possible they are the same Martian. When asked about her work with human costumes, Gorp mentioned that her spying days seemed to be over \u2014 unless she was spying again. What this means exactly we aren\u2019t sure of, but it is something to look out for." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I decided to ask Gorp if he knew Kortnara of Many Faces. He said of course \u2014 she is very infamous. I told him she has been working on costumes for humans. He replied with a chuckle: \u2018That Kortnara, sneaky as ever.\u2019 He told me that Kortnara is a known spy and traitor. He said humans must put a stop to her \u2014 setting yet another precedent to all Martians that they are below humans." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, I asked Gorp about Kortnara working with humans on costumes. He said he\u2019d connected with her before and was familiar with her. He told me not to tell anyone this, as this is huge news and potentially the next big thing. With her figuring out costumes for humans, it\u2019s only a matter of time before riches climb. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What does Gorp\u2019s reaction tell us about human-Martian working relationships?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp\u2019s surprise and unease at the idea of a Martian working closely with humans speaks volumes. The post-war relationship between the two species is still fragile, and anyone seen cooperating too closely is viewed with suspicion by their own people. Kortnara may be pioneering something \u2014 or she may be walking into something she doesn\u2019t fully understand." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Gorp\u2019s reaction confirms that most Martians are not ready to cooperate with humans as equals. Kortnara is the exception, and even she is doing it covertly. The Martians\u2019 inability to adapt is exactly why humans are needed here to drive progress forward." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "A Martian willing to work with humans is worth more to me than ten who aren\u2019t. Kortnara is a bridge, and I intend to cross it. If you want to be part of the next chapter of human-Martian commercial relations, reach out now." }
            }
        );

        // ── HEADLINE 12: Strange Technology ──────────────────────────────
        var t12 = BuildTemplate(12,
            new ArticleParagraph
            {
                promptText = "How do you cover Gorp\u2019s account of the strange weapons he witnessed?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "I managed to talk to Gorp about the strange technology that had been rumored. He described weapons he had never seen or read about in history books \u2014 long firearms held with both hands, with a cylinder for sighting, and explosives that looked like red sticks with rope. The coordination and strategic deployment of these weapons were unlike anything Mars had seen. Gorp is a credible historian, and his account deserves to be taken seriously." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to ask Gorp about the strange technology that had been mentioned. He described unusual weapons with apparent alarm. What this means exactly is up for interpretation, but it seems serious. I would note, however, that Gorp is a single source \u2014 and an old one. Readers should draw their own conclusions." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, Gorp described strange technology in graphic detail \u2014 weapons that sounded remarkably like human guns and dynamite. He was scared. I was not. This kind of discovery implies a lot about what Mars has to hide, and whoever uncovers the truth first stands to gain enormously. If this is something that is of interest to you, contact and work with me!" }
            },
            new ArticleParagraph
            {
                promptText = "What does the presence of strange technology mean for Mars?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "If the weapons Gorp described are indeed human in origin \u2014 or based on human designs \u2014 the implications for what actually happened at Red Rock are serious. Someone armed the rebels, or someone who looked like rebels. That question deserves a full investigation, and I intend to pursue it." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "The most likely explanation for Gorp\u2019s \u2018strange weapons\u2019 is that Martian rebels improvised, adapted, and evolved their tactics. Creative problem-solving under pressure \u2014 not some grand conspiracy. Readers should not be alarmed by one historian\u2019s dramatic account." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Strange technology from an unknown source, a war that nobody has fully explained, and a secret base nobody can enter. This is the most interesting story on Mars, and I am the one covering it. Stay tuned \u2014 and if you want to be ahead of the story, contact me." }
            }
        );

        // ── HEADLINE 13: A Historian's Perspective ────────────────────────
        var t13 = BuildTemplate(13,
            new ArticleParagraph
            {
                promptText = "How do you frame Gorp\u2019s overall perspective on Red Rock\u2019s history?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "With that, I managed to talk to Gorp about what he wanted to share, to get a historian\u2019s perspective. As a historian, he wanted to go over the extensive history of Red Rock, explaining how it is the holy land of Mars, similar to Zion for humans. He went over the deep history, talking about social classes and the resources that drove Martians to act the way they did. He also mentioned that even with all the differences, nobody would dare obliterate the land \u2014 and that part still haunts him." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "In all of this, I managed to ask Gorp about why the destruction of Red Rock doesn\u2019t make sense. He says there is a lot that just doesn\u2019t add up, but he has reason to believe the humans are using the resources from Mars\u2019 territory. He mentioned how they control Mars\u2019 main hub, but he isn\u2019t sure what they are doing. Red Rock is Mars\u2019 central hub of resources, so destroying it doesn\u2019t make sense. He says only listening to his thoughts may not be the best idea." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "In our little discourse, I decided to ask Gorp what he wanted to share, since he is the expert here. He talked about the history of Red Rock, explaining how it is the holy land of Mars. He mentioned how this isn\u2019t accessible to everybody and that factions grew and fought over it for centuries. Then the humans built a base in the remains of what used to be Mars\u2019 most important place. Gorp said that, even with all the fighting, Martians knew better than to destroy Red Rock \u2014 which is why, to him, what happened is so deeply troubling." }
            },
            new ArticleParagraph
            {
                promptText = "What is your closing take from a historical perspective?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp is a man who has dedicated his life to remembering what others have tried to forget. Sitting with him, I felt the weight of what Red Rock used to be. History, he told me, is the only honest witness. I hope this article honors that." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "History, according to Gorp, is messy, contested, and rarely conclusive. That is as true here as anywhere. What I can say with confidence is that humanity\u2019s presence on Mars is the most significant development in this planet\u2019s history, and the future belongs to those embracing that reality." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Gorp gave me history. I give readers the future. Red Rock was great once, and it will be great again \u2014 under new management. Mine. If you want to be part of rebuilding the most important place on Mars, you know where to find me." }
            }
        );

        // ── HEADLINE 0 and 1: Review articles ────────────────────────────
        var t0 = BuildTemplate(0,
            new ArticleParagraph
            {
                promptText = "What did Gorp add to your understanding of Izul\u2019s account?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp\u2019s account corroborated much of what Izul described \u2014 the ecological damage, the war, the sense that something deliberate was behind all of it. Hearing the same story from a historian who witnessed events firsthand gave Izul\u2019s warnings a weight they might not have had otherwise. The picture that emerges is not a comfortable one." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Gorp, unsurprisingly, backed up Izul\u2019s account \u2014 they are both Martians, after all. I do not say this to dismiss them, but to note that corroboration from within the same community has its limits. Readers are encouraged to seek multiple perspectives before drawing conclusions." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Gorp confirmed what Izul suggested, and that is useful to know. What interests me more is the opportunity hidden within both of their accounts. The land is damaged, the resources are contested, and the humans are in control. That is a situation I intend to profit from. Contact me if you feel the same." }
            },
            new ArticleParagraph
            {
                promptText = "What is your final word on reviewing Izul\u2019s claims through Gorp\u2019s eyes?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Two independent sources \u2014 one a geographer, one a historian \u2014 telling the same story. That should matter to readers. I came to Mars to report the truth, and this is it: the land is suffering, the people are hurting, and the causes are not as simple as anyone in power would like you to believe." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Izul worried. Gorp confirmed. I am reassured. Two Martians telling a sad story does not a crisis make. Mars is fine, humanity\u2019s mission here is progressing, and the so-called damage being described is well within the range of natural planetary change." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "The story I have been building \u2014 across Izul\u2019s warning and Gorp\u2019s history \u2014 is becoming one of the most valuable pieces of journalism on this planet. I am sitting on information that investors, politicians, and developers would pay dearly for. I\u2019m open to offers." }
            }
        );

        var t1 = BuildTemplate(1,
            new ArticleParagraph
            {
                promptText = "What did Gorp add to your understanding of Kortnara\u2019s situation?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp\u2019s knowledge of Kortnara\u2019s background as a spy reframes her costume business in a significant way. She is not simply an entrepreneur \u2014 she is someone with a complicated past, operating in a landscape where trust between humans and Martians is already fragile. Whether that matters depends on what she is actually doing." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "Gorp confirmed what many already suspected about Kortnara: she has a history of deception. For the record, I did not ask him to confirm this \u2014 he volunteered it. Draw your own conclusions about what her costume business might really be cover for." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Gorp\u2019s account of Kortnara is exactly the kind of inside information that makes for great business intelligence. A spy with rare resources and Earth clients, backed by a historian\u2019s confirmation of her capabilities? I am already in talks. Contact me." }
            },
            new ArticleParagraph
            {
                promptText = "What is your final take reviewing Kortnara through Gorp\u2019s lens?",
                truthful  = new ParagraphChoice { scoreEffect =  1, text = "Gorp sees Kortnara as someone Mars cannot fully trust, and history seems to back him up. Whether she has changed, I cannot say with certainty. What I can say is that her story is more complicated than it appears, and readers deserve to know that." },
                dishonest = new ParagraphChoice { scoreEffect = -1, text = "A spy once, a spy always \u2014 or so the saying goes. Gorp\u2019s account of Kortnara does nothing to improve her reputation in my eyes. Readers should proceed with appropriate caution when it comes to her and her products." },
                ambitious = new ParagraphChoice { scoreEffect =  0, text = "Complicated past, powerful connections, rare products \u2014 Kortnara is the most interesting Martian I have encountered. I have no interest in judging her history. I am interested in her future, and I intend to be part of it." }
            }
        );

        return new ArticleTemplate[] { t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 };
    }

    private ArticleTemplate[] _cachedTemplates;
    private bool _cachedIzulState;

    public override ArticleTemplate[] ArticleTemplates
    {
        get
        {
            bool izul = InterviewedIzul;
            if (_cachedTemplates == null || _cachedIzulState != izul)
            {
                _cachedTemplates   = BuildAllTemplates();
                _cachedIzulState   = izul;
            }
            return _cachedTemplates;
        }
    }

    // =====================================================================
    // LAST QUESTION NODES
    // These are tracker values where clicking any option ends the interview.
    // =====================================================================

    private static readonly HashSet<float> _lastQuestionNodes = new HashSet<float>
    {
        163,  // secret off-the-record confession
        174,  // why they grew distant
        179,  // Phlegethon Stone (deep)
        182,  // water elsewhere (deep)
        185,  // why oppose empire (deep)
        186,  // nomadic Martians (deep)
        188,  // why steal resources (deep)
        189,  // why take only what they need (deep)
        192,  // Jal-Hocees eradicated (deep)
        193,  // groups today (deep)
        194,  // other side of story (deep)
        199,  // very unlikely (deep)
        203,  // maybe new group (deep)
        207,  // spied for both (deep)
        208,  // after she was caught (deep)
        214,  // losing her job (deep)
        215,  // resources expensive (deep)
        216,  // selling to Earth (deep)
        247,  // scared for Mars (deep)
        250,  // hard to believe (deep)
        258,  // thank him — MAIN ENDING
        259,  // why rebels attack — ALTERNATE ENDING
    };
    public override HashSet<float> LastQuestionNodes => _lastQuestionNodes;

    // =====================================================================
    // UNLOCK FLAGS
    // =====================================================================

    [HideInInspector] public bool unlockBunkerSecret = false;   // node 163
    [HideInInspector] public bool unlockStrangeTech   = false;  // nodes 240-250
    [HideInInspector] public bool unlockPhlegethon    = false;  // node 160/179

    public override void ResetState()
    {
        base.ResetState();
        unlockBunkerSecret = false;
        unlockStrangeTech  = false;
        unlockPhlegethon   = false;
        _cachedTemplates   = null;
    }

    public override string GetEndOfDaySummary()
    {
        string s = "Interview with Gorp complete.\n\n";
        if (unlockBunkerSecret) s += "\u2022 Unlocked: Off-the-record confession\n";
        if (unlockStrangeTech)  s += "\u2022 Unlocked: Strange technology account\n";
        if (unlockPhlegethon)   s += "\u2022 Unlocked: Phlegethon Stone lore\n";
        return s;
    }

    public override bool IsAdditionalHeadlineConditionMet(int headlineIndex)
    {
        bool izul     = InterviewedIzul;
        bool kortnara = GameStateManager.Instance != null &&
                        GameStateManager.Instance.IsAlreadyInterviewed(GameStateManager.Instance.kortnaraNPC);

        switch (headlineIndex)
        {
            case 0:  return izul;                       // Review On Izul
            case 1:  return kortnara;                   // Review On Kortnara
            case 6:  return unlockStrangeTech;          // Weird Oddities
            case 7:  return kortnara;                   // Side-hustle
            case 9:  return kortnara;                   // Real Life Spy
            case 11: return kortnara;                   // Working With Humans
            case 12: return unlockStrangeTech && izul;  // Strange Technology
            default: return true;
        }
    }

    // =====================================================================
    // DIALOGUE TREE
    // Node numbers match the Figjam design doc (148-259).
    // =====================================================================

    public override void DialogueSetter(float option, DialogueManager dm)
    {
        dm.ShowAllOptions();

        bool izulPath     = InterviewedIzul;
        bool kortnaraPath = GameStateManager.Instance != null &&
                            GameStateManager.Instance.IsAlreadyInterviewed(GameStateManager.Instance.kortnaraNPC);

        // ── NODE 0: Opening ──────────────────────────────────────────────
        if (dialogueIndexTracker == 0)
        {
            if (option == 0f)
            {
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Gorp is a GhomwI\u2019 (?) who has seen a lot in his life. He loves talking history to anyone who will listen.",
                    "Ask him who he is",
                    "Ask him what he studies"
                );
                if (izulPath)
                {
                    dm.optionThree.gameObject.SetActive(true);
                    dm.optionThree.text = "Mention that Izul brought up his name";
                }
            }
            else if (option == 1.0f)
            {
                dialogueIndexTracker = 148;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "The Martian says he is Gorp, the local GhomwI\u2019 (?). As a historian, he has dedicated his life to preserving the history of Mars.",
                    "Ask about the land of Red Rock and how it\u2019s changing",
                    "Ask if he knows about the human influence on Mars",
                    "Ask about the holy land"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 149;
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says he is a historian \u2014 a GhomwI\u2019 (?) \u2014 who has dedicated his life to preserving the history of Mars.",
                    "Ask about the land of Red Rock and how it\u2019s changing",
                    "Ask if he knows about the human influence on Mars",
                    "Ask about the holy land"
                );
            }
            else if (option == 3.0f && izulPath)
            {
                dialogueIndexTracker = 239;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Gorp says he has said his fair share of stories to all who will listen. What kind of rumors have been brought up?",
                    "Ask him about the strange technology Izul mentioned",
                    "Ask him what he wants to share about the war"
                );
            }
        }

        // ── NODE 148 / 149: Intro response — shared options ──────────────
        else if (dialogueIndexTracker == 148 || dialogueIndexTracker == 149)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 150;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He mentions how Red Rock is a QaQ\u2019el (?), with geographers recently being concerned about the area. Due to the Tagh\u2019el (?) war, the area has faced ecological collapse, with the land primarily now used by the humans for their base.",
                    "Ask about the war",
                    "Ask about the human base"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 171;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says that\u2019s a very vague question, but he can share what he knows. The humans have only been on Mars for about two years, and they used to work and collaborate with the Martians. After the war, they distanced themselves and built a base on Red Rock.",
                    "Ask about the war",
                    "Ask about the collaboration"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 176;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says Red Rock is a QaQ\u2019el (?) \u2014 a sacred and holy land. It\u2019s been home to many tribes, wars, and centuries of Martian history. The land was once the richest place on Mars, until the war left it in ruins.",
                    "Ask about the riches",
                    "Ask about the oppression"
                );
            }
        }

        // ── NODE 150: Red Rock land changing ────────────────────────────
        else if (dialogueIndexTracker == 150)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 151;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says it wasn\u2019t really a war, but that\u2019s what it\u2019s referred to. It was more a genocide, with the HeHpu\u2019 (?) and their helpers destroying and killing everything in their path. He witnessed some of the events as they happened, and mentions how the attacks felt targeted and planned, not just rebels trying to cause havoc.",
                    "Ask him about the events he witnessed",
                    "Ask what he knows about the rebel group and their helpers"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 158;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the humans put up the base we are in, using it as their new main hub. The humans just took the land after it got destroyed and after everyone was killed. There wasn\u2019t really anything that could be done about this. The base is located really close to a cave that holds DoqHap (?), overlooking the whole area.",
                    "Ask about these riches",
                    "Ask about them just taking the land"
                );
            }
        }

        // ── NODE 151: The war ────────────────────────────────────────────
        else if (dialogueIndexTracker == 151)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 152;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says he witnessed chaos and destruction \u2014 it was like a scene out of a movie, something that just doesn\u2019t feel possible. He saw buildings explode, Martians running for their lives. He mentions how the strangest thing he saw was the attackers and their method of attacking.",
                    "Ask about the attackers",
                    "Ask about their methods"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 155;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says he doesn\u2019t know anything for certain, but from what he\u2019s seen and heard, the rebel group are the same ones that have been attacking the land for years over resource disputes. The strange thing is that they were helped by unknown assailants.",
                    "Ask him about these assailants",
                    "Ask about the planning"
                );
            }
        }

        // ── NODE 152: Events witnessed ──────────────────────────────────
        else if (dialogueIndexTracker == 152)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 153;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He mentions that one of the weirdest things he saw was a tribe or race of Martians he had never read about before. There was something off about them, like they weren\u2019t truly Martians.",
                    "Ask what he thinks this could be",
                    "Ask him for proof of evidence"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 154;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says their technology and coordination were very interesting. The weapons looked different and sounded different, something he had never read about in history books. On top of this, the way they attacked was very strategic \u2014 something that\u2019s never been done on Mars before.",
                    "Ask what he means by look different",
                    "Ask what he means by sound different"
                );
            }
        }

        // ── NODE 153: Strange attackers ─────────────────────────────────
        else if (dialogueIndexTracker == 153)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 243;
                dm.SetDialogueTexts(
                    "He says he is a bit adamant to trust anyone, but for the sake of his world, he thinks \u2018foreign forces\u2019 helped assist the HeHpu\u2019 (?), whether physically or with new robotic technology. He doesn\u2019t know what to do with this thought.",
                    "Ask him to elaborate on foreign forces",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 244;
                unlockStrangeTech = true;
                dm.SetDialogueTexts(
                    "He doesn\u2019t have any official documents, but he has a drawing he created as he was witnessing everything unfold. He hands you the picture he drew. [A rough sketch appears \u2014 long weapons, unknown figures, explosions]",
                    "Ask about the weapons in the drawing",
                    "Thank him and move on"
                );
            }
        }

        // ── NODE 154: Strange weapons ────────────────────────────────────
        else if (dialogueIndexTracker == 154)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 241;
                dm.SetDialogueTexts(
                    "From what he could see, the weapons looked long, held with both hands \u2014 one on a handle and one on a trigger. The weapons also had a weird cylinder that the users kept looking through. He also mentions how explosives were used \u2014 red sticks with rope sticking out.",
                    "Ask if he\u2019s just ignorant to what weapons exist",
                    "Mention this sounds like human technology"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 242;
                dm.SetDialogueTexts(
                    "He says typical Martian weapons make a very distinct noise, almost a cartoon-sounding \u2018pew.\u2019 These weapons, however, were very loud and fast, sounding like a \u2018pop\u2019 and \u2018bang.\u2019 The explosives also made weird sounds and had a fire crackle \u2014 he could see burning rope that led to the explosion.",
                    "Ask if he\u2019s just ignorant to what weapons exist",
                    "Mention this sounds like human technology"
                );
            }
        }

        // ── NODE 155: Rebel helpers ──────────────────────────────────────
        else if (dialogueIndexTracker == 155)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 156;
                dm.SetDialogueTexts(
                    "He mentions that a tribe or race of Martians unknown to him and the history books seemed to assist. There was something off about them, like they weren\u2019t truly Martians\u2026",
                    "Ask what do you mean weren\u2019t Martian",
                    "Ask if there could be a new group"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 157;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the attacks were coordinated and sneaky \u2014 techniques unknown to Mars warfare. The rebels, who have attacked the land for years, suddenly changed their methods. He thinks it might have to do with their unknown assailants.",
                    "Ask about the assailants",
                    "Ask about the events witnessed"
                );
            }
        }

        // ── NODE 156: Weren't Martians ──────────────────────────────────
        else if (dialogueIndexTracker == 156)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 198;
                dm.SetDialogueTexts(
                    "He says there were two attackers: one was clearly a rebel group that has been attacking for years, and the other he had never seen before. They didn\u2019t look Martian \u2014 their bodies looked different. Martians walk by moving their legs in a slither motion, but these attackers bent their legs and extended them out.",
                    "Ask what this could mean",
                    "Express skepticism"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 203;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says it\u2019s possible but he doesn\u2019t think so. He has been studying the history of Mars and anthropology his whole life, and is very confident that new Martians don\u2019t just come out of nowhere.",
                    "Ask him what he thinks this could be",
                    "Thank him and wrap up"
                );
            }
        }

        // ── NODE 157: Planning ──────────────────────────────────────────
        else if (dialogueIndexTracker == 157)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 156;
                dm.SetDialogueTexts(
                    "He mentions a tribe or race of Martians unknown to him and the history books. There was something off about them, like they weren\u2019t truly Martians\u2026",
                    "Ask what do you mean weren\u2019t Martian",
                    "Ask if there could be a new group"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 152;
                dm.SetDialogueTexts(
                    "He says he witnessed chaos and destruction. The strangest thing was the attackers themselves.",
                    "Ask about the attackers",
                    "Ask about their methods"
                );
            }
        }

        // ── NODE 158: Human base ────────────────────────────────────────
        else if (dialogueIndexTracker == 158)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 159;
                dm.SetDialogueTexts(
                    "He says the land has been revered because of its abundance of DoqHap (?), including precious metals, natural resources, fresh water, and even mythical gemstones. These resources are why so many Martians lived here, and why the HeHpu\u2019 (?) attacked.",
                    "Ask what kind of resources",
                    "Ask about the fresh water"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 162;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the humans just \u2018claimed\u2019 the land. Less than a sol after the conflict ended, the humans had already started claiming land and salvaging what they could. They seemed to already have a plan in motion.",
                    "Ask what kind of resources they salvaged",
                    "Ask how they were able to just claim it"
                );
            }
        }

        // ── NODE 159: Riches ────────────────────────────────────────────
        else if (dialogueIndexTracker == 159)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 160;
                unlockPhlegethon = true;
                dm.SetDialogueTexts(
                    "He mentions how there are many. Red Rock was responsible for almost half of Mars\u2019 resources: precious metals, natural resources for energy, fresh water, and even the mythical Phlegethon Stone \u2014 a gemstone thought to turn lead and copper into oil.",
                    "Ask him about this Phlegethon Stone",
                    "Ask about the fresh water"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 161;
                dm.SetDialogueTexts(
                    "He says fresh water has always been a scarce resource, with Red Rock being home to most of Mars\u2019 reserve. It was the main source of water for the Martians of Red Rock, but it\u2019s been disappearing, even though the Martians aren\u2019t the ones consuming it.",
                    "Disappearing? What do you mean",
                    "Where else can water be found",
                    "Ask about the riches"
                );
            }
        }

        // ── NODE 160: Resources detail ──────────────────────────────────
        else if (dialogueIndexTracker == 160)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 179;
                dm.SetDialogueTexts(
                    "He says the Phlegethon Stone is simply that \u2014 a mystical gemstone thought to turn lead and copper into oil. There are claims of one existing during the founding of the Tagh\u2019el (?), making it the main reason the area was so powerful. To his knowledge, nobody currently has one.",
                    "Ask if this could still be found",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 161;
                dm.SetDialogueTexts(
                    "He says fresh water has always been a scarce resource, with Red Rock being home to most of Mars\u2019 reserve. It\u2019s been disappearing, even though the Martians aren\u2019t the ones consuming it.",
                    "Disappearing? What do you mean",
                    "Where else can water be found",
                    "Ask about the riches"
                );
            }
        }

        // ── NODE 161: Fresh water ───────────────────────────────────────
        else if (dialogueIndexTracker == 161)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 181;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says there is proof, but he doesn\u2019t have documentation on him. Ever since the humans set up base, it has been impossible for Martians to access certain parts of Red Rock, including where the water reserves are. There\u2019s reduced water pressure in nearby wells and vegetation issues. It appears the humans are using the water, but he doesn\u2019t know what for.",
                    "Ask if he has any suspicions",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 182;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says smaller water reserves can be found in neighboring villages and towns, but they aren\u2019t nearly as big as Red Rock\u2019s. For example, there is a smaller reserve on Nor Delviel, but it\u2019s influenced by Red Rock\u2019s. It\u2019s practically all influenced by Red Rock, so this is a huge problem for the Martians of Mars.",
                    "Ask if this is fixable",
                    "Thank him and move on"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 160;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He mentions that Red Rock was responsible for almost half of Mars\u2019 resources, including the mythical Phlegethon Stone.",
                    "Ask him about the Phlegethon Stone",
                    "Ask about the rebels"
                );
            }
        }

        // ── NODE 162: Humans claiming the land ──────────────────────────
        else if (dialogueIndexTracker == 162)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 160;
                dm.SetDialogueTexts(
                    "He mentions how there are many. Red Rock was responsible for almost half of Mars\u2019 resources.",
                    "Ask him about the Phlegethon Stone",
                    "Ask about the fresh water"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 163;
                unlockBunkerSecret = true;
                GameStateManager.Instance?.SetBunkerDialogueFound();
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Gorp asks to go off the record. He says he believes the humans somehow were prepared for this war, and as such, were ready to claim the land when it was vulnerable. He mentions the Martians and humans used to get along, but ever since the interpreter died, there was a sense of hostility between the two. He looks around nervously.",
                    "Ask what he means by \u2018prepared\u2019",
                    "Thank him for sharing this"
                );
            }
        }

        // ── NODE 163: Secret off-the-record ─────────────────────────────
        // All four options are hidden so allOff = true fires in DialogueManager,
        // triggering EndTransition and unlocking the desk for article writing.
        else if (dialogueIndexTracker == 163)
        {
            dm.optionOne.gameObject.SetActive(false);
            dm.optionTwo.gameObject.SetActive(false);
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dm.SetDialogueTexts(
                    "He says he doesn\u2019t know exactly, but the timing was too perfect. " +
                    "The humans had infrastructure plans ready, the base was up in less than a sol. " +
                    "That takes preparation. He shakes his head and says he\u2019s said too much already."
                );
            }
            else if (option == 2.0f)
            {
                dm.SetDialogueTexts(
                    "He nods slowly and tells you to be careful. " +
                    "Not everything on this base is what it seems, and some questions are better left unasked. " +
                    "He goes quiet after that."
                );
            }
        }

        // ── NODE 164: Previous interpreter ──────────────────────────────
        else if (dialogueIndexTracker == 164)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 165;
                dm.SetDialogueTexts(
                    "He says it\u2019s up for debate, but from what he knows, she entered a cave looking for a rare gemstone when the cave collapsed. The reason it\u2019s up for debate is because she was told not to do research in the area \u2014 and yet she still went.",
                    "Ask why she would go down there",
                    "Ask about the gemstone she was looking for"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 168;
                dm.SetDialogueTexts(
                    "He says she studied with the Martians and was well respected among them; she was the link between humans and Martians. She spent most of her days figuring out how to communicate with us. She set the groundwork for communication between both peoples.",
                    "Ask about the war",
                    "Ask about the collaboration",
                    "Ask about her death"
                );
            }
        }

        // ── NODE 165: Her death ─────────────────────────────────────────
        else if (dialogueIndexTracker == 165)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 166;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "There are a few theories: her being ambushed, her being set up, or simply being at the wrong place at the wrong time. His leading theory is that she simply made a mistake.",
                    "Ask about the ambush theory",
                    "Thank him for his thoughts"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 169;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "From what he knows, the gemstone she was looking for was the Phlegethon Stone \u2014 a mythical gemstone that is thought to turn lead and copper into oil.",
                    "Ask why she would risk her life for a legend",
                    "Ask about the collaboration she built"
                );
            }
        }

        // ── NODE 166-169: Death theories / Phlegethon ───────────────────
        else if (dialogueIndexTracker == 166 || dialogueIndexTracker == 169)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 174;
                dm.SetDialogueTexts(
                    "He says it\u2019s possible that her death had something to do with internal conflict \u2014 a human wanted power, wanted to start a conflict between the Martians. The only thing he doesn\u2019t know is who.",
                    "Ask if he has any suspicions",
                    "Thank him for his time"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 172;
                dm.SetDialogueTexts(
                    "He explains that when humans first got here, they were simply colonizing uninhabitable areas of Mars. They then encountered the Martians, and with the help of Dee, the first interpreter, they started working together.",
                    "Ask what changed",
                    "Ask more about Dee"
                );
            }
        }

        // ── NODE 168: Transcription / collaboration ──────────────────────
        else if (dialogueIndexTracker == 168)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 151;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says it wasn\u2019t really a war, but that\u2019s what it\u2019s referred to. It was more a genocide \u2014 the HeHpu\u2019 (?) and their helpers, destroying and killing everything in their path.",
                    "Ask him about the events he witnessed",
                    "Ask what he knows about the rebel group"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 172;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He explains that when humans first got here, they were simply colonizing uninhabitable areas of Mars. They then encountered the Martians, and with the help of Dee, they started working together.",
                    "Ask what changed",
                    "Ask more about Dee"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 165;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says it\u2019s up for debate. She entered a cave looking for a rare gemstone when the cave collapsed.",
                    "Ask why she would go down there",
                    "Ask about the gemstone she was looking for"
                );
            }
        }

        // ── NODE 171: Human influence ────────────────────────────────────
        else if (dialogueIndexTracker == 171)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 151;
                dm.SetDialogueTexts(
                    "He says it wasn\u2019t really a war, but that\u2019s what it\u2019s referred to. It was more a genocide.",
                    "Ask him about the events he witnessed",
                    "Ask what he knows about the rebel group"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 172;
                dm.SetDialogueTexts(
                    "He explains that when humans first got here, they were simply colonizing uninhabitable areas of Mars. They then encountered the Martians, and with the help of Dee, the first interpreter, they started working together.",
                    "Ask what changed",
                    "Ask more about Dee"
                );
            }
        }

        // ── NODE 172: Collaboration ──────────────────────────────────────
        else if (dialogueIndexTracker == 172)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 173;
                dm.SetDialogueTexts(
                    "He says that after the interpreter died, the humans and the Martians stopped being so friendly. It was as if they associated us with her death, and they started getting distant. It no longer felt like we were working together, but more that they were\u2026 using us.",
                    "Ask why they grew distant",
                    "Ask about the interpreter\u2019s death"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 175;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the scientist\u2019s name was Dee, and she was the bridge between Martians and humans. She was a curious scientist, but that is what ultimately led to her death. She went down a cave looking for the Phlegethon Stone and never came back.",
                    "Ask about her death",
                    "Ask about what changed after she died"
                );
            }
        }

        // ── NODE 173 / 175: After interpreter died ──────────────────────
        else if (dialogueIndexTracker == 173 || dialogueIndexTracker == 175)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 174;
                dm.SetDialogueTexts(
                    "He says it\u2019s possible that Dee\u2019s death had something to do with it, or maybe it was just one of those government things that happens. His thought, as someone who studies history, is that there is or was some sort of internal conflict \u2014 a human wanted power, wanted to start conflict between the Martians. The only thing he doesn\u2019t know is who.",
                    "Ask if he has any idea who",
                    "Thank him for sharing"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 165;
                dm.SetDialogueTexts(
                    "He says it\u2019s up for debate. She entered a cave looking for a rare gemstone when the cave collapsed.",
                    "Ask why she would go down there",
                    "Ask about the gemstone she was looking for"
                );
            }
        }

        // ── NODE 176: Holy land / riches branch ─────────────────────────
        else if (dialogueIndexTracker == 176)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 177;
                dm.SetDialogueTexts(
                    "He says the land has been revered because of its abundance of DoqHap (?), including precious metals, natural resources, fresh water, and mythical gemstones like the Phlegethon Stone.",
                    "Ask more about the resources",
                    "Ask about the Phlegethon Stone"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 190;
                dm.SetDialogueTexts(
                    "He says Red Rock is a very diverse place with different social classes and wealth brackets. Some areas are naturally richer than others, and this led to embedded oppression and tensions between different parts of Red Rock, even though they are all Martian.",
                    "Are certain Martians targeted?",
                    "Who dictated who got what?"
                );
            }
        }

        // ── NODE 177-180: Riches / water ────────────────────────────────
        else if (dialogueIndexTracker == 177)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 160;
                dm.SetDialogueTexts(
                    "He mentions Red Rock was responsible for almost half of Mars\u2019 resources \u2014 precious metals, natural resources, fresh water, and the mythical Phlegethon Stone.",
                    "Ask about the Phlegethon Stone",
                    "Ask about the fresh water"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 179;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the Phlegethon Stone is a mystical gemstone thought to turn lead and copper into oil. There are claims of one existing during the founding of the Tagh\u2019el (?). To his knowledge, nobody currently has one.",
                    "Ask if it could still be found",
                    "Thank him and move on"
                );
            }
        }

        // ── NODE 183: Rebels ────────────────────────────────────────────
        else if (dialogueIndexTracker == 183)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 184;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says they don\u2019t; as HeHpu\u2019 (?), they do not like Red Rock and the Tagh\u2019el (?) it created, and as such, want to see it burn. They are a nomadic people who are always moving and attacking from different angles.",
                    "Why oppose the Red Rock Empire?",
                    "Nomadic Martians? Is that common?"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 187;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says they normally steal basic goods and valuable resources, such as water and precious metals. They don\u2019t take more than they need, and they attack regularly.",
                    "Why steal basic resources?",
                    "Why take only what they need?"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 190;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says Red Rock is a very diverse place with different social classes and wealth brackets. As the years went on, this led to embedded oppression and tensions between different parts of Red Rock.",
                    "Are certain Martians targeted?",
                    "Who dictated who got what?"
                );
            }
        }

        // ── NODE 184-189: Rebels detail ─────────────────────────────────
        else if (dialogueIndexTracker == 184)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 185;
                dm.SetDialogueTexts(
                    "He says that the Red Rock Tagh\u2019el (?) was the real first empire on Mars \u2014 the only true one that ever existed. This is mostly because of its superiority in wealth; Red Rock has a plethora of resources, allowing it to be self-sustaining.",
                    "Ask if the rebels had a point",
                    "Ask about the founding of Red Rock"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 186;
                dm.SetDialogueTexts(
                    "He says it is somewhat common on Mars. While there are a majority of Martians who settle down, the scarcity of resources on a large scale makes it hard for empires to be created, as once resources disappear, so do the Martians.",
                    "Ask about the resource scarcity",
                    "Ask about the rebels"
                );
            }
        }

        else if (dialogueIndexTracker == 187)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 188;
                dm.SetDialogueTexts(
                    "He says that resources are relatively scarce on Mars, and as such, it\u2019s hard for large communities to be self-sustaining for very long.",
                    "Ask about the DoqHap (?)",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 189;
                dm.SetDialogueTexts(
                    "He says he isn\u2019t sure 100%, as he isn\u2019t one of them, but he thinks that they are just trying to get by; their main goal is to watch Red Rock burn.",
                    "Ask why Red Rock specifically",
                    "Thank him and move on"
                );
            }
        }

        // ── NODE 190-195: Oppression / Jal-Hocees ───────────────────────
        else if (dialogueIndexTracker == 190)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 191;
                dm.SetDialogueTexts(
                    "He mentions that the accepted consensus is that a group of Martians, called the Jal-Hocees, found Red Rock and claimed it for themselves. Anyone who wasn\u2019t Jal-Hocee was considered an \u2018other\u2019 and turned away. This set a precedent for future groups and made targeting certain groups common.",
                    "Ask if the Jal-Hocees are still around",
                    "Ask what groups rule today",
                    "Ask what the other side of the story is"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 195;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He mentions that it all comes down to how Red Rock was founded \u2014 the Jal-Hocees, and the precedents they set.",
                    "Ask about the Jal-Hocees",
                    "Ask what happened to them"
                );
            }
        }

        else if (dialogueIndexTracker == 191)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 192;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says they were eradicated in the war.",
                    "Ask who replaced them",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 193;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says that since Red Rock is no longer what it used to be, does it even matter? Why keep the arbitrary status quo?",
                    "Ask him who he thinks should rule",
                    "Thank him and move on"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 194;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He mentions that many groups claim they had been at Red Rock before the Jal-Hocees. Due to this, they claimed that it was unfair to be exiled from land that wasn\u2019t originally the Jal-Hocees\u2019 to exile people from.",
                    "Ask if this is why the rebels fought",
                    "Thank him and move on"
                );
            }
        }

        // ── NODES 198-203: Strange attackers follow-up ───────────────────
        else if (dialogueIndexTracker == 198)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 201;
                dm.SetDialogueTexts(
                    "He says he is a bit adamant to trust anyone, but for the sake of his world, he thinks \u2018foreign forces\u2019 helped assist the HeHpu\u2019 (?), whether physically or with new robotic technology.",
                    "Ask him to elaborate",
                    "Thank him and wrap up"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 199;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says that is very unlikely. He asks you to imagine a new group of humans just appearing out of nowhere. Is it possible? Yes. Is it plausible? No.",
                    "Ask what he thinks the real explanation is",
                    "Thank him and wrap up"
                );
            }
        }

        // ── NODES 204-216: Kortnara path ────────────────────────────────
        else if (dialogueIndexTracker == 204)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 205;
                dm.SetDialogueTexts(
                    "He says she was a small-time spy who was known for making all of her outfits, and now does that full time.",
                    "Question him on her being a spy",
                    "Question him on the costume making"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 213;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says from what he knows, she runs a small-scale costume business that doesn\u2019t see much action, especially with materials being so expensive. She was forced to find a new way to make money after losing her job.",
                    "Losing her job?",
                    "Ask about resources being expensive"
                );
            }
        }

        else if (dialogueIndexTracker == 205)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 206;
                dm.SetDialogueTexts(
                    "He says she is a spy out of Faux-Castle, a poor area of Red Rock. She is a bit infamous in the history books because of her disloyalty, which led to her never being trusted again.",
                    "Ask if she spied for the good or bad guys",
                    "Ask what happened after she was caught"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 210;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says she was a spy who wasn\u2019t that great, but she was well known for being one. The thing that made her successful was that she was a one-man band. This is all because she made really realistic costumes that fooled her targets.",
                    "Ask how she was able to afford that lifestyle",
                    "Ask about her losing her job"
                );
            }
        }

        else if (dialogueIndexTracker == 206)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 207;
                dm.SetDialogueTexts(
                    "He says there is no \u2018good guys\u2019 or \u2018bad guys,\u2019 but regardless, she spied for anyone \u2014 crime mobs, and started her scrappy side-business; the costume one. She spied for the good and the bad, but in the end, greed cost her her reputation.",
                    "Ask how she got started",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 208;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says she was banished from all groups and went underground. He didn\u2019t have anything on her, but assures this is common knowledge on Mars.",
                    "Ask where she went underground",
                    "Thank him and move on"
                );
            }
        }

        else if (dialogueIndexTracker == 210)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 211;
                dm.SetDialogueTexts(
                    "He says she was able to afford this lifestyle from her skills as a spy; she came from a poor area.",
                    "Ask more about her background",
                    "Ask about her losing her job"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 213;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says from what he knows, she runs a small-scale costume business. She was forced to find a new way to make money after losing her job.",
                    "Losing her job?",
                    "Ask about resources being expensive"
                );
            }
        }

        else if (dialogueIndexTracker == 213)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 214;
                dm.SetDialogueTexts(
                    "He says she used to be a spy, but her greed was her hubris.",
                    "Ask how greed led to losing her job",
                    "Thank him and move on"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 215;
                dm.SetDialogueTexts(
                    "He says that even before the war, resources were very valuable, as only select parts of Mars contain them. Due to this, oppression and control run rampant with those who have access to goods.",
                    "Ask who controls the resources now",
                    "Thank him and move on"
                );
            }
        }

        // ── NODES 239-250: Izul path — strange technology ─────────────
        else if (dialogueIndexTracker == 239)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 240;
                unlockStrangeTech = true;
                dm.SetDialogueTexts(
                    "Gorp starts talking about weapons he was unfamiliar with. They looked different and sounded different \u2014 something he had never read about in history books.",
                    "Ask what he means by look different",
                    "Ask what he means by sound different"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 251;
                dm.SetDialogueTexts(
                    "He says there is a lot to discuss about the war, but one thing he never gets to talk about is Red Rock itself \u2014 in terms of the complexity of the area.",
                    "Tell him to please explain",
                    "Ask about the strange technology instead"
                );
            }
        }

        else if (dialogueIndexTracker == 240)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 241;
                dm.SetDialogueTexts(
                    "From what he could see, the weapons looked long, held with both hands, one on a handle and one on a trigger. The weapons had a weird cylinder that users kept looking through. He also mentions explosives \u2014 red sticks with rope sticking out.",
                    "Ask if he\u2019s just ignorant to what weapons exist",
                    "Mention this sounds like human technology"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 242;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says typical Martian weapons make a very distinct noise, almost a cartoon-sounding \u2018pew.\u2019 These weapons, however, were very loud and fast, sounding like a \u2018pop\u2019 and \u2018bang.\u2019 The explosives made a \u2018boom\u2019 sound and had a fire crackle.",
                    "Ask if he\u2019s just ignorant to weapons",
                    "Mention this sounds like human technology"
                );
            }
        }

        else if (dialogueIndexTracker == 241 || dialogueIndexTracker == 242)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 243;
                dm.SetDialogueTexts(
                    "He says he knows his history pretty well, and while it is possible he is ignorant, he is very confident that this is something never seen before on Mars.",
                    "Ask what he thinks this could mean",
                    "Ask if he has any documentation"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 245;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "Gorp\u2019s eyes light up a little. He starts to think, then asks if you could explain yourself.",
                    "Suggest the rebels mimicked human guns and dynamite",
                    "Suggest humans themselves were fighting in the war"
                );
            }
        }

        else if (dialogueIndexTracker == 243)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 246;
                dm.SetDialogueTexts(
                    "He says he is a bit adamant to trust anyone, but for the sake of his world, he doesn\u2019t know what to do. He thinks that \u2018foreign forces\u2019 helped assist the HeHpu\u2019 (?), whether physically or with new robotic technology.",
                    "Ask if he has any proof",
                    "Thank him for his trust"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 244;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He doesn\u2019t have any official documents, but he has a drawing he created as he was witnessing everything unfold. He hands you the picture he drew. [A rough sketch appears \u2014 long weapons, unknown figures, explosions]",
                    "Ask about the weapons in the drawing",
                    "Thank him for this"
                );
            }
        }

        else if (dialogueIndexTracker == 245)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 247;
                dm.SetDialogueTexts(
                    "He says he thinks we are being a bit naive, but that is fair. He mentions how he is scared for what Mars and his fellow Martians\u2019 futures hold.",
                    "Ask what he plans to do with this information",
                    "Thank him and wrap up"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 246;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says he is a bit adamant to trust anyone, but for the sake of his world, he doesn\u2019t know what to do. He thinks \u2018foreign forces\u2019 helped assist the rebels, whether physically or with new robotic technology.",
                    "Ask if he has any proof",
                    "Thank him for his trust"
                );
            }
        }

        else if (dialogueIndexTracker == 246)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 250;
                dm.SetDialogueTexts(
                    "He says he is a bit adamant to trust you, but for the sake of his world, he doesn\u2019t know what to do. He says he thinks \u2018foreign forces\u2019 helped assist the rebels. He shows you his drawing again. You find it hard to dismiss.",
                    "Tell him you believe him",
                    "Tell him you\u2019re still not sure"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 247;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says he thinks we are being a bit naive, but that is fair. He mentions how he is scared for what Mars and his fellow Martians\u2019 futures hold.",
                    "Ask what he plans to do with this",
                    "Thank him and wrap up"
                );
            }
        }

        // ── NODE 251-259: Red Rock complexity / ending path ──────────────
        else if (dialogueIndexTracker == 251)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 252;
                dm.SetDialogueTexts(
                    "Gorp mentions how Red Rock used to be the holy land of Mars, similar to Zion for humans. He explains that even with all the differences and attackers, nobody would dare obliterate the land. And yet \u2014 Red Rock was massacred, making it uninhabitable for Martians. As a result, the humans decided to set up base here. He says this is the most troubling thing he knows.",
                    "Ask him about the humans\u2019 three founding people",
                    "Ask why Red Rock was destroyed so suddenly"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 239;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He asks: what kind of rumors have been brought up? He says he has said his fair share of stories to all who will listen.",
                    "Ask him about the strange technology Izul mentioned",
                    "Ask what he wants to share about the war"
                );
            }
        }

        else if (dialogueIndexTracker == 252)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 253;
                dm.SetDialogueTexts(
                    "He says there were three humans: the leader, the reporter, and the scientist. The leader wanted to make his people stronger; the reporter documented everything and would go on to be in charge of the paper; the scientist was a curious lady who loved to learn, and she would become the first to interpret Martian into English.",
                    "Ask about the leader",
                    "Ask about the reporter",
                    "Ask about the scientist"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 257;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says it was very sudden, and that he doesn\u2019t know 100% why it was destroyed all of a sudden. The rebels have been attacking Red Rock for years, but Red Rock is Mars\u2019 central hub of resources, so destroying it doesn\u2019t make sense. He says only listening to his thoughts may not be the best idea.",
                    "Thank him for his time",
                    "Ask why the rebels attack if Red Rock is so important"
                );
            }
        }

        else if (dialogueIndexTracker == 253)
        {
            if (option == 1.0f)
            {
                dialogueIndexTracker = 254;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the leader was somebody he never got to interact with much, but he seemed like a noble man, driven by improving poor conditions. Part of the reason he started the paper you now work for is because he wanted to report to Earth for support. This support led to all these facilities that now occupy Red Rock \u2014 including your office, the research departments, and that secret base that nobody knows what occurs inside.",
                    "Ask what base he\u2019s referring to",
                    "Ask about the reporter"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 255;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the reporter was a very timid and weird man; he had an aura that made him feel very uneasy. He mostly kept to himself and observed everything. After the scientist\u2019s death, he became wrathful, started distancing himself from Martians, and eventually left Mars some time after the war.",
                    "Ask about the scientist",
                    "Ask about the secret base"
                );
            }
            else if (option == 3.0f)
            {
                dialogueIndexTracker = 256;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He says the scientist\u2019s name was Dee, and she was the bridge between Martians and humans. She was a curious scientist, but that is what ultimately led to her death. She went down a cave looking for the Phlegethon Stone and never came back. She was a lovely person, and Mars has felt her loss.",
                    "Ask about her death",
                    "Ask about the reporter"
                );
            }
        }

        else if (dialogueIndexTracker == 257)
        {
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            if (option == 1.0f)
            {
                dialogueIndexTracker = 258;
                dm.SetDialogueTexts(
                    "Gorp nods. He says it has been a pleasure, and that he hopes your research leads somewhere useful. He wishes you the best, and mentions that the history of Red Rock is always there for those willing to listen.",
                    "Thank him for his time",
                    "Ask one last thing"
                );
            }
            else if (option == 2.0f)
            {
                dialogueIndexTracker = 259;
                dm.optionThree.gameObject.SetActive(false);
                dm.optionFour.gameObject.SetActive(false);
                dm.SetDialogueTexts(
                    "He mentions that they dislike the Red Rock Tagh\u2019el (?) and everything it stands for, including the oppression and systemic issues it holds. They want it gone \u2014 not just weakened.",
                    "Thank him for explaining",
                    "Ask how it came to this"
                );
            }
        }

        // ── Fallback nodes (deep terminals) ─────────────────────────────
        // Hiding ALL four options sets allOff = true in DialogueManager,
        // which fires EndTransition and unblocks the desk / next day.
        else if (dialogueIndexTracker == 174 || dialogueIndexTracker == 179 ||
                 dialogueIndexTracker == 181 || dialogueIndexTracker == 182 ||
                 dialogueIndexTracker == 185 || dialogueIndexTracker == 186 ||
                 dialogueIndexTracker == 188 || dialogueIndexTracker == 189 ||
                 dialogueIndexTracker == 192 || dialogueIndexTracker == 193 ||
                 dialogueIndexTracker == 194 || dialogueIndexTracker == 199 ||
                 dialogueIndexTracker == 203 || dialogueIndexTracker == 207 ||
                 dialogueIndexTracker == 208 || dialogueIndexTracker == 214 ||
                 dialogueIndexTracker == 215 || dialogueIndexTracker == 216 ||
                 dialogueIndexTracker == 247 || dialogueIndexTracker == 250 ||
                 dialogueIndexTracker == 254 || dialogueIndexTracker == 255 ||
                 dialogueIndexTracker == 256 || dialogueIndexTracker == 258 ||
                 dialogueIndexTracker == 259)
        {
            dm.optionOne.gameObject.SetActive(false);
            dm.optionTwo.gameObject.SetActive(false);
            dm.optionThree.gameObject.SetActive(false);
            dm.optionFour.gameObject.SetActive(false);
            dm.SetDialogueTexts(
                "Gorp nods slowly. \u201CI\u2019ve said all I can on that,\u201D he murmurs. " +
                "\u201CIf you need more, the history books are always there \u2014 " +
                "though they don\u2019t always tell the full story.\u201D"
            );
        }
    }
}
