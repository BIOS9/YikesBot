using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace YikesBot.Services.Furry;

public class FurryTranslator
{
    // Replaces single words
    private static readonly Dictionary<string, string> WordReplacements = new()
    {
        {"the", "teh"},
        {"awful", "pawful"},
        {"bite", "nom"},
        {"bye", "bai"},
        {"come", "cum"},
        {"hi", "hai"},
        {"lmao", "hehe~"},
        {"love", "wuv"},
        {"this", "dis"},
        {"your", "ur"},
        {"you", "chu"},
        {"what", "OwO wat"},
        {"what's", "oWo wats"},
        {"whats", "oWo wats"},
        {"police", "pawlice"},
        {"roar", "rawr"},
        {"source", "sauce"},
        {"kiss", "lick"},
        {"hand", "paw"},
        {"father", "daddy"},
        {"dog", "good boy"},
        {"awesome", "pawsome"},
    };

    // Replaces tokens with regex rules
    private static readonly List<Tuple<Regex, string>> RegexReplacements = new()
    {
        new Tuple<Regex, string>(new Regex(@"ahh+"), "murrrr")
    };
    
    // Replacements that only occur when no other replacements have happened (except global)
    private static readonly List<Tuple<string, string>> ExclusiveReplacements = new()
    {
        
    };
        
    // Always replaces these tokens inside other tokens no matter what
    // The boolean parameter at the end is if the replacer should stop if it matches this rule
    private static readonly List<Tuple<string, string, bool>> GlobalReplacements = new()
    {
        new Tuple<string, string, bool>("for", "fur", true),
        new Tuple<string, string, bool>("r", "w", false),
        new Tuple<string, string, bool>("l", "w", false),
        new Tuple<string, string, bool>("fuck", "fluff", true),
        new Tuple<string, string, bool>(",", "~", false),
        new Tuple<string, string, bool>(".", "~", false),
        new Tuple<string, string, bool>("!", " owo!", false),
        new Tuple<string, string, bool>("?", " uwu?", false)
    };

    public static string Translate(string text)
    {
        StringBuilder output = new StringBuilder();
        foreach (var token in Regex.Split(text.ToLower(), @"\s"))
        {
            output.Append(TranslateToken(token.Trim()));
            output.Append(" ");
        }
        
        // All replace words should be done first
        

// More agressive rules should be done after
        
//text = text.Replace("N", "NY"); // Maybe only if there is a vowel after? No = Nyo Never = Nyever ??
//text = text.Replace("n", "ny");

// Should actually build a new string from the old one instead of overwriting the same string!!!

// Needs some Rawr XD or just Raws
// Nuzzles and wuzzles 
// *wags tail*
// Any word ending in y can rookie wookie cookie wookie
//auntie wauntie <-- notice the vowel stays
// corgie worgie
// aie <- should only be words with two syllables. How to figure that out?
// Same sylable rule with words ending in Y
// Why
// Fucksy 
// Uwus
// lots of ~
// <:
// OwO UwU oWo
// >v<
        return output.ToString().Trim();
    }

    private static string TranslateToken(string token)
    {
        _ = TryReplaceGlobal(token, out token);
        
        if (TryReplaceWord(token, out string wordReplacement))
            return wordReplacement;
        
        if (TryReplaceRegex(token, out string regexReplacement))
            return regexReplacement;

        if (TryReplaceExclusive(token, out string globalReplacement))
            return globalReplacement;
        
        return token;
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
    
    private static bool TryReplaceExclusive(string token, out string output)
    {
        foreach (var exclusiveReplacement in ExclusiveReplacements)
        {
            if (token.Contains(exclusiveReplacement.Item1))
            {
                output = token.Replace(exclusiveReplacement.Item1, exclusiveReplacement.Item2);
                return true;
            }
        }

        output = string.Empty;
        return false;
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
}