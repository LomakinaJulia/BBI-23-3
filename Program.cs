using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

abstract class Task
{
    protected string text;
    public override string ToString()
    {
        return ParseText(text);
    }
    public Task(string text) { this.text = text; }
    protected abstract string ParseText(string text);
}

class Task_8 : Task
{
    public Task_8(string text) : base(text) { this.text = text; }

    protected override string ParseText(string text)//Превращает текс в строки по 50 символов.
    {
        int st = 0;
        string result = "";
        int i = 50;
        int j;
        for (j = i; j < text.Length; j += i)
        {
            int s;
            for (s = j; s >= st; s--)
            {
                if (text[s] == ' ')
                {
                    break;
                }

            }
            string line = text.Substring(st, s - st);
            int k = line.Length;
            while (k < 50)
            {
                for (int l = 0; l < line.Length; l++)
                {
                    if (line[l] == ' ')
                    {
                        line = line.Insert(l, " ");
                        l++;
                        k++;
                        if (line.Length >= 50) { break; }
                    }
                }
            }
            st = s + 1;
            j = st;
            result += line + "\n";
        }
        return result += text.Substring(j - i, text.Length - (j - i));
    }
}

class Task_9 : Task
{
    private string compressedText;
    private Dictionary<string, char> codeTable;

    public Task_9(string text) : base(text)
    {
        compressedText = ParseText(text);
    }

    protected override string ParseText(string text)
    {
        var twoLetterCounts = new Dictionary<string, int>();
        Regex regex = new Regex(@"[a-zA-Zа-яА-Я]{2}"); // Regex to match two consecutive letters in English and Russian
        foreach (Match match in regex.Matches(text))
        {
            string seq = match.Value;
            if (!twoLetterCounts.ContainsKey(seq))
            {
                twoLetterCounts[seq] = 0;
            }
            twoLetterCounts[seq]++;
        }

        var frequentSequences = twoLetterCounts.OrderByDescending(kvp => kvp.Value).Take(10).Select(kvp => kvp.Key).ToList();
        codeTable = new Dictionary<string, char>();
        HashSet<char> usedCodes = new HashSet<char>();
        char codeChar = '\u03B1'; // Start with a Greek letter

        foreach (string seq in frequentSequences)
        {
            while (usedCodes.Contains(codeChar) || text.Contains(codeChar)) // Ensure code is not in text
            {
                codeChar++;
            }
            codeTable[seq] = codeChar;
            usedCodes.Add(codeChar);
        }

        // Compress the text
        StringBuilder sb = new StringBuilder();
        int i = 0;
        while (i < text.Length)
        {
            string seq = text.Substring(i, Math.Min(2, text.Length - i)); // Get 2-letter or remaining substring
            if (codeTable.ContainsKey(seq))
            {
                sb.Append(codeTable[seq]);
                i += 2;
            }
            else
            {
                sb.Append(text[i]);
                i++;
            }
        }
        return sb.ToString(); // Returning the result of text compression
    }

    public Dictionary<string, char> GetCodeTable()
    {
        return codeTable;
    }

    public override string ToString()
    {
        return $"{compressedText}\nCode Table: {string.Join(", ", codeTable.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
    }
}



class Task_10 : Task
{
    private string decodedText;
    private Task_9 task9; // Ссылка на объект Task_9

    public Task_10(Task_9 task9) : base(task9.ToString())
    {
        this.task9 = task9;
        this.decodedText = DecodeText(task9.ToString(), task9.GetCodeTable());
    }

    private string DecodeText(string encodedText, Dictionary<string, char> codeTable)
    {
        var reversedCodeTable = codeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        var sb = new StringBuilder();

        foreach (char c in encodedText)
        {
            if (reversedCodeTable.ContainsKey(c))
            {
                sb.Append(reversedCodeTable[c]);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    protected override string ParseText(string text)
    {
        // Для Task_10 не требуется переопределять метод ParseText
        return text;
    }

    public override string ToString()
    {
        return decodedText;
    }
}


class Task_12 : Task
{
    private Dictionary<string, int> table;
    private List<string> words;
    private List<string> mfwords;

    public Task_12(string text) : base(text)
    {
        table = new Dictionary<string, int>();
        words = new List<string>();
        mfwords = new List<string>();

        mfwords = MostFrequency(text);

        for (int i = 0; i < mfwords.Count; i++)
        {
            table.Add(mfwords[i], i);
        }

        string[] splitText = text.Split(' ');
        foreach (string word in splitText)
        {
            words.Add(word);
        }
    }

    public string ReplaceWordsWithCodes()
    {
        for (int i = 0; i < words.Count; i++)
        {
            if (table.ContainsKey(words[i]))
            {
                words[i] = table[words[i]].ToString();
            }
        }
        return string.Join(" ", words);
    }

    public string GetTextWithWords()
    {
        List<string> decodedWords = new List<string>();
        foreach (string word in words)
        {
            if (int.TryParse(word, out int code))
            {
                foreach (var item in table)
                {
                    if (item.Value == code)
                    {
                        decodedWords.Add(item.Key);
                        break;
                    }
                }
            }
            else
            {
                decodedWords.Add(word); // если слово не является кодом, остается как есть
            }
        }
        return string.Join(" ", decodedWords);
    }

    protected override string ParseText(string text)
    {
        return text;  // Для Task_12 не требуется переопределять метод ParseText
    }

    public List<string> MostFrequency(string text)
    {
        Dictionary<string, int> wordFrequency = new Dictionary<string, int>();
        string[] words = text.Split(new char[] { ' ', ',', '.', '!', '?', ')', '(', '"', '-' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            if (wordFrequency.ContainsKey(word))
            {
                wordFrequency[word]++;
            }
            else
            {
                wordFrequency[word] = 1;
            }
        }

        var sortedWords = wordFrequency.OrderByDescending(pair => pair.Value).Take(5).Select(pair => pair.Key).ToList();

        return sortedWords;
    }
}


class Task_13 : Task
{
    private Dictionary<char, double> startingLetterPercentages;

    public Task_13(string text) : base(text)
    {
        startingLetterPercentages = CalculateStartingLetterPercentages(text);
    }

    private Dictionary<char, double> CalculateStartingLetterPercentages(string text)
    {
        var letterCounts = new Dictionary<char, int>();
        int totalWords = 0;

        foreach (string word in text.Split(' '))
        {
            char firstLetter = char.ToUpper(word[0]); // Case-insensitive

            if (char.IsLetter(firstLetter))
            {
                if (!letterCounts.ContainsKey(firstLetter))
                {
                    letterCounts[firstLetter] = 0;
                }
                letterCounts[firstLetter]++;
                totalWords++;
            }
        }

        var percentages = letterCounts.ToDictionary(
            kvp => kvp.Key,
            kvp => (double)kvp.Value / totalWords * 100
        );

        return percentages;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Starting Letter Percentages:");

        foreach (var kvp in startingLetterPercentages)
        {
            sb.AppendLine($"  {kvp.Key}: {kvp.Value:F2}%");
        }

        return sb.ToString();
    }

    protected override string ParseText(string text)
    {
        return "This method is not implemented for Task_13.";
    } 
}

class Task_15 : Task
{
    private int sumOfNumbers;

    public Task_15(string text) : base(text)
    {
        sumOfNumbers = CalculateSumOfNumbers(text);
    }

    private int CalculateSumOfNumbers(string text)
    {
        int sum = 0;
        string currentNumber = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsDigit(text[i]))
            {
                currentNumber += text[i];
            }
            else if (currentNumber != "") // Check if we have a complete number
            {
                sum += int.Parse(currentNumber);
                currentNumber = ""; // Reset for the next number
            }
        }

        // Add the last number if the text ends with a digit
        if (currentNumber != "")
        {
            sum += int.Parse(currentNumber);
        }

        return sum;
    }

    public override string ToString()
    {
        return $"Sum of numbers in the text: {sumOfNumbers}";
    }

    protected override string ParseText(string text)
    {
        return string.Empty; // Return an empty string as not implemented
    }
}

class Program
{
    public static void Main()
    {
        string text = "William Shakespeare, widely regarded as one of the greatest writers in the English language, authored a total of 37 plays, along with numerous poems and sonnets. He was born in Stratford-upon-Avon, England, in 1564, and died in 1616. Shakespeare's most famous works, including \"Romeo and Juliet,\" \"Hamlet,\" \"Macbeth,\" and \"Othello,\" were written during the late 16th and early 17th centuries. \"Romeo and Juliet,\" a tragic tale of young love, was penned around 1595. \"Hamlet,\" one of his most celebrated tragedies, was written in the early 1600s, followed by \"Macbeth,\" a gripping drama exploring themes of ambition and power, around 1606. \"Othello,\" a tragedy revolving around jealousy and deceit, was also composed during this period, believed to be around 1603.";

        Task_8 task8 = new Task_8(text);
        Console.WriteLine("Task 8:");
        Console.WriteLine(task8);

        Task_9 task9 = new Task_9(text);
        Console.WriteLine("\nTask 9 (Encoded text):");
        Console.WriteLine(task9);

        Task_10 task10 = new Task_10(task9);
        Console.WriteLine("\nTask 10 (Decoded Text):");
        Console.WriteLine(task10);

        Task_12 task12 = new Task_12(text);
        Console.WriteLine("\nTask 12 (Encoded text):");
        Console.WriteLine(task12.ReplaceWordsWithCodes());
        Console.WriteLine("\nTask 12 (Decoded Text):");
        Console.WriteLine(task12.GetTextWithWords());

        Task_13 task13 = new Task_13(text);
        Console.WriteLine("\nTask 13 (Starting Letter Percentages):");
        Console.WriteLine(task13);

        Task_15 task15 = new Task_15(text);
        Console.WriteLine("\nTask 15 (Sum of Numbers):");
        Console.WriteLine(task15);
    }
}