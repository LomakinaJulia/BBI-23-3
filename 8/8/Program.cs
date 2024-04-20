using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

abstract class Task
{
    public Task(string text) { }
    protected abstract void ParseText(string text);
}

class Task_8 : Task
{
    private string formattedText;

    public Task_8(string text) : base(text)
    {
        ParseText(text);
    }

    protected override void ParseText(string text)
    {
        var lines = new List<string>();
        var words = text.Split(' ');
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > 50)
            {
                lines.Add(currentLine.ToString().PadRight(50));
                currentLine.Clear();
            }
            currentLine.Append(word).Append(' ');
        }
        lines.Add(currentLine.ToString().PadRight(50)); // Add the last line
        formattedText = string.Join("\n", lines);
    }

    public override string ToString()
    {
        return formattedText; // Return the formatted text
    }
}

class Task_9 : Task
{
    private string encodedText;
    private Dictionary<string, char> codeTable;

    public Dictionary<string, char> GetCodeTable()
    {
        return codeTable;
    }

    public Task_9(string text) : base(text)
    {
        ParseText(text);
    }

    protected override void ParseText(string text)
    {
        // Find frequent two-letter sequences (only letters)
        var twoLetterCounts = new Dictionary<string, int>();
        var regex = new Regex(@"[a-zA-Z]{2}"); // Regex to match two consecutive letters
        foreach (Match match in regex.Matches(text))
        {
            string seq = match.Value;
            if (!twoLetterCounts.ContainsKey(seq))
            {
                twoLetterCounts[seq] = 0;
            }
            twoLetterCounts[seq]++;
        }

        // Select most frequent sequences and create unique codes
        var frequentSequences = twoLetterCounts.OrderByDescending(kvp => kvp.Value).Take(10).Select(kvp => kvp.Key).ToList();
        codeTable = new Dictionary<string, char>();
        var usedCodes = new HashSet<char>();
        char codeChar = '\u0370'; // Start with a Greek letter

        foreach (string seq in frequentSequences)
        {
            while (usedCodes.Contains(codeChar) || text.Contains(codeChar)) // Ensure code is not in text
            {
                codeChar++;
            }
            codeTable[seq] = codeChar;
            usedCodes.Add(codeChar);
        }

        // Encode the text
        var sb = new StringBuilder();
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
        encodedText = sb.ToString();
    }

    public override string ToString()
    {
        return $"Encoded Text: {encodedText}\nCode Table: {string.Join(", ", codeTable.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
    }
}

class Task_10 : Task
{
    private string decodedText;
    private Task_9 task9; // Reference to the Task_9 object 

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

    public override string ToString()
    {
        return decodedText;
    }

    protected override void ParseText(string text)
    {
        // Not needed for Task_10
    }
}

class Task_12 : Task
{
    private string[] encodedTextArray;
    private Dictionary<string, int> wordCodeTable;

    public Task_12(string text, Dictionary<string, int> wordCodeTable) : base(text)
    {
        this.wordCodeTable = wordCodeTable;
        this.encodedTextArray = EncodeText(text);
    }

    private string[] EncodeText(string text)
    {
        string[] words = text.Split(' ');
        string[] encodedWords = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            if (wordCodeTable.ContainsKey(words[i]))
            {
                encodedWords[i] = wordCodeTable[words[i]].ToString();
            }
            else
            {
                encodedWords[i] = words[i];
            }
        }
        return encodedWords;
    }

    public override string ToString()
    {
        var reversedCodeTable = wordCodeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        string[] decodedWords = new string[encodedTextArray.Length];

        for (int i = 0; i < encodedTextArray.Length; i++)
        {
            int code;
            if (int.TryParse(encodedTextArray[i], out code) && reversedCodeTable.ContainsKey(code))
            {
                decodedWords[i] = reversedCodeTable[code];
            }
            else
            {
                decodedWords[i] = encodedTextArray[i]; // Keep punctuation or non-coded words as-is
            }
        }

        return string.Join(" ", decodedWords);
    }


    protected override void ParseText(string text)
    {
        // Not needed for Task_12
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

    protected override void ParseText(string text)
    {
        // Not needed for Task_13
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

    protected override void ParseText(string text)
    {
        // Not needed for Task_15
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
        Console.WriteLine("\nTask 9:");
        Console.WriteLine(task9);

        Task_10 task10 = new Task_10(task9); // Pass the Task_9 object directly
        Console.WriteLine("\nTask 10 (Decoded Text):");
        Console.WriteLine(task10);

        var wordCodeTable = new Dictionary<string, int>()
        {
           {"the", 1},
           {"and", 2},
           {"is", 3},
           {"to", 4},
           {"of", 5},
        };

        Task_12 task12 = new Task_12(text, wordCodeTable);
        Console.WriteLine("\nTask 12 (Decoded Text):");
        Console.WriteLine(task12);

        Task_13 task13 = new Task_13(text);
        Console.WriteLine("\nTask 13 (Starting Letter Percentages):");
        Console.WriteLine(task13);

        Task_15 task15 = new Task_15(text);
        Console.WriteLine("\nTask 15 (Sum of Numbers):");
        Console.WriteLine(task15);
    }
}