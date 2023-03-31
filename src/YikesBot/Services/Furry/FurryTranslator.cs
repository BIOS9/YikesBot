using System.Text;
using System.Text.RegularExpressions;

namespace YikesBot.Services.Furry;

public class FurryTranslator
{
    private static Random _random = new Random(); 
    
    // Replaces single words
    private static readonly Dictionary<string, string> WordReplacements = new()
    {
        {"the", "teh"},
        {"awful", "pawful"},
        {"bite", "nom"},
        {"bye", "bai :wave:"},
        {"come", "cum"},
        {"hi", "hai :wave:"},
        {"lmao", "hehe~"},
        {"love", "wuv :heart:"},
        {"this", "dis"},
        {"then", "den"},
        {"your", "ur"},
        {"you", "chu"},
        {"what", "OwO wat"},
        {"what's", "oWo wats"},
        {"whats", "oWo wats"},
        {"with", "wif"},
        {"police", "pawlice"},
        {"roar", "rawr"},
        {"source", "sauce"},
        {"kiss", "lick"},
        {"hand", "paw"},
        {"father", "daddy"},
        {"dad", "daddy"},
        {"dog", "good boy"},
        {"awesome", "pawsome"},
        {"think", "fink"},
        {"cool", "keww"},
        {"milk", "milky-wilky"},
        {"bulge", "bulgy-wulgy"},
        {"for", "fur"},
        {"porn", "yiff"},
        {"admin", "owner"},
        {"admins", "owners"},
        {"furry", ":sparkles: furry :sparkles: :fox:"},
        {"stash", "yiff stash"},
        {"admin-furry-stash", "admin-furry-yiff-stash"},
        {"#admin-furry-stash", "#admin-furry-yiff-stash"},
        {"fursuit", "fursuit"}, // Dont replace
        {"shit", "dog poop"},
        {"so", "s-s-so"},
        {"oh", "o-o-oh"},
        {"furries", ":sparkles: furries :sparkles:"},
        {"please", "pwease :pleading_face:"},
    };

    // Replaces tokens with regex rules
    private static readonly List<Tuple<Regex, string>> RegexReplacements = new()
    {
        new Tuple<Regex, string>(new Regex(@"ahh+"), "murrrr"),
        new Tuple<Regex, string>(new Regex(@"(ha+){2,}h?"), "hehehehe~")
    };

    // Always replaces these tokens inside other tokens no matter what
    // The boolean parameter at the end is if the replacer should stop if it matches this rule
    private static readonly List<Tuple<string, string, bool>> GlobalReplacements = new()
    {
        new Tuple<string, string, bool>("🤔", "<:thowonking:958998634895114250>", false),
        new Tuple<string, string, bool>("😢", ":crying_cat_face:", false),
        new Tuple<string, string, bool>("😭", ":crying_cat_face:", false),
        new Tuple<string, string, bool>("😥", ":crying_cat_face:", false),
        new Tuple<string, string, bool>("😙", ":kissing_cat:", false),
        new Tuple<string, string, bool>("😘", ":kissing_cat:", false),
        new Tuple<string, string, bool>("😍", ":heart_eyes_cat:", false),
        new Tuple<string, string, bool>("😚", ":kissing_cat:", false),
        new Tuple<string, string, bool>("😠", ":pouting_cat:", false),
        new Tuple<string, string, bool>("😡", ":pouting_cat:", false),
        new Tuple<string, string, bool>("👿", ":pouting_cat:", false),
        new Tuple<string, string, bool>("🙁", ":pouting_cat:", false),
        new Tuple<string, string, bool>("😦", ":pouting_cat:", false),
        new Tuple<string, string, bool>("☹", ":pouting_cat:", false),
        new Tuple<string, string, bool>("😲", ":scream_cat:", false),
        new Tuple<string, string, bool>("😯", ":scream_cat:", false),
        new Tuple<string, string, bool>("😱", ":scream_cat:", false),
        new Tuple<string, string, bool>("😆", ":joy_cat:", false),
        new Tuple<string, string, bool>("🤣", ":joy_cat:", false),
        new Tuple<string, string, bool>("😂", ":joy_cat:", false),
        new Tuple<string, string, bool>("😄", ":smile_cat:", false),
        new Tuple<string, string, bool>("😃", ":smiley_cat:", false),
        new Tuple<string, string, bool>("😁", ":smile_cat:", false),
        new Tuple<string, string, bool>("😀", ":smile_cat:", false),
        new Tuple<string, string, bool>("🙂", ":smile_cat:", false),
        new Tuple<string, string, bool>("😔", ":cat:", false),
        new Tuple<string, string, bool>("🥸", ":wolf:", false), // Disguised face
        new Tuple<string, string, bool>("💀", ":panda_face:", false),
        
        new Tuple<string, string, bool>("na", "nya", false),
        new Tuple<string, string, bool>("ne", "nye", false),
        new Tuple<string, string, bool>("ni", "nyi", false),
        new Tuple<string, string, bool>("no", "nyo", false),
        new Tuple<string, string, bool>("nu", "nyu", false),
        new Tuple<string, string, bool>("fuck", "fluff", true),
        new Tuple<string, string, bool>(",", "~", false),
        new Tuple<string, string, bool>(".", "~", false),
        new Tuple<string, string, bool>("!", " owo!", false),
        new Tuple<string, string, bool>("?", " uwu?", false)
    };

    private const double MiddleTokenChance = 0.03;
    private static readonly List<string> MiddleTokens = new()
    {
        ":point_right::point_left:",
        "uwu",
        "owo",
        ">⁠~⁠<",
        "xD",
        ":3",
        ">.<",
        ">_<",
        "V.v.V",
        "^m^",
        "OWO",
        "oWo",
        "UWU",
        "uWu",
        "^_^",
        ":>",
        ":dog:",
        ":dog2:",
        ":cat:",
        ":mouse:",
        ":hamster:",
        ":rabbit:",
        ":fox:",
        ":bear:",
        ":polar_bear:",
        ":koala:",
        ":tiger:",
        ":cow:",
        ":unicorn:",
    };

    private const double EndTokenChance = 0.1;
    private static readonly List<string> EndTokens = new()
    {
        "*nuzzles*",
        "*pees*",
        "rawr xD",
        "raaawr",
        "*wags tail*",
        "*sees another dog walking past and barks frantically*",
        "<:awooo:958999403975290970>",
        ":pleading_face:",
        "*stares assertively (im an alpha)*",
        "*snarls*",
        "*grrrrrrrr*",
        "*sniff sniff*",
    };
    
    public static string Translate(string text)
    {
        StringBuilder output = new StringBuilder();
        foreach (var token in Regex.Split(text, @"\s"))
        {
            output.Append(TranslateToken(token.Trim()));
            output.Append(" ");
        }
        return InsertFurryTokens(output.ToString()).Trim();
    }

    private static string TranslateToken(string token)
    {
        if (Uri.TryCreate(token, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return token; // Skip URLs
        }

        string lowerToken = token.ToLower();
        
        bool globalReplaced = TryReplaceGlobal(lowerToken, out lowerToken);
        
        if (TryReplaceWord(lowerToken, out string wordReplacement))
            return wordReplacement;
        
        if (TryReplaceRegex(lowerToken, out string regexReplacement))
            return regexReplacement;

        if (!globalReplaced && TryReplaceLW(lowerToken, out string lwReplacement))
            return lwReplacement;
        
        return lowerToken;
    }

    private static bool TryReplaceWord(string token, out string output)
    {
        if (WordReplacements.ContainsKey(token.ToLower()))
        {
            output = WordReplacements[token.ToLower()];
            return true;
        }

        output = string.Empty;
        return false;
    }
    
    private static bool TryReplaceRegex(string token, out string output)
    {
        foreach (var patternReplacement in RegexReplacements)
        {
            if (patternReplacement.Item1.IsMatch(token))
            {
                output = patternReplacement.Item1.Replace(token, patternReplacement.Item2);
                return true;
            }
        }

        output = string.Empty;
        return false;
    }
    
    private static bool TryReplaceLW(string token, out string output)
    {
        if (token.StartsWith("l") || token.StartsWith("r") ||
            token.EndsWith("l") || token.EndsWith("r"))
        {
            output = string.Empty;
            return false;
        }

        output = token.Replace("l", "w").Replace("r", "w");
        return true;
    }
    
    private static bool TryReplaceGlobal(string token, out string output)
    {
        bool anyMatch = false;
        output = token;
        foreach (var globalReplacement in GlobalReplacements)
        {
            if (token.Contains(globalReplacement.Item1))
            {
                anyMatch = true;
                output = output.Replace(globalReplacement.Item1, globalReplacement.Item2);
                if (globalReplacement.Item3) return true; // Return if stop on this rule is set
            }
        }

        if (anyMatch) return true;
        
        return false;
    }

    private static string InsertFurryTokens(string input)
    {
        if (input.Length < 10) return input;
        return InsertEndTokens(InsertMiddleTokens(input));
    }

    private static string InsertMiddleTokens(string input)
    {
        StringBuilder builder = new StringBuilder();
        string[] tokens = input.Split(" ");
        foreach (var token in tokens)
        {
            builder.Append(token);
            builder.Append(" ");
            if (_random.NextDouble() < MiddleTokenChance)
            {
                builder.Append(MiddleTokens[_random.Next(MiddleTokens.Count)]);
                builder.Append(" ");
            }
        }
        return builder.ToString().Trim();
    }
    
    private static string InsertEndTokens(string input)
    {
        if (_random.NextDouble() < EndTokenChance)
        {
            return input + " " + EndTokens[_random.Next(EndTokens.Count)];
        }
        return input;
    }
}