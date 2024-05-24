using System.IO;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;


abstract class Task
{
    protected string text = "";
    
    public string Text
    {
        get => text;
        protected set => text = value;
    }
    public Task(string text)
    {
        this.text = text;
    }
}

class Task1 : Task
{
        [JsonConstructor]
        public Task1(string text) : base(text) { }
        public override string ToString()
        {
            return FindMaxConsecutiveChars(Text).ToString();
        }

        private int FindMaxConsecutiveChars(string text)
        {
            int maxCount = 0;
            int currentCount = 1;
            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == text[i - 1])
                {
                    currentCount++;
                }
                else
                {
                    maxCount = Math.Max(maxCount, currentCount);
                    currentCount = 1;
                }
            }
            maxCount = Math.Max(maxCount, currentCount); 
            return maxCount;
        }
 }

class Task2 : Task
{
    private int amount = 1;
    public int Amount
    {
        get => amount;
        private set => amount = value;
    }
    public Task2(string text) : base(text)
    {
        this.amount = 25;
    }
    [JsonConstructor]
    public Task2(string text, int amount) : base(text)
    {
        this.amount = amount;
    }
    public override string ToString()
    {
        return string.Join(Environment.NewLine, FindWordsStartingWithMostFrequentCapital(Text));
    }

    private string[] FindWordsStartingWithMostFrequentCapital(string text)
    {
        char mostFrequentLetter = ' ';
        int maxCount = 0;

        foreach (char character in text)
        {
            
            if (char.IsUpper(character))
            {
                
                int currentCount = 0;

                foreach (char innerCharacter in text)
                {
                    if (innerCharacter == character)
                    {
                        currentCount++;
                    }
                }

                if (currentCount > maxCount)
                {
                    mostFrequentLetter = character;
                    maxCount = currentCount;
                }
            }
        }

        List<string> resultWords = new List<string>();
        string[] words = text.Split(' ');
        foreach (string word in words)
        {
            if (word.StartsWith(mostFrequentLetter))
            {
                resultWords.Add(word);
            }
        }

        return resultWords.ToArray();
    }
}


class JsonIO
{

    public static void Write<T>(T obj, string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
        {
            JsonSerializer.Serialize(fs, obj); 
        }
    }
    public static T Read<T>(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
        {
            return JsonSerializer.Deserialize<T>(fs); 
        }
        return default(T);
    }
}

class Program
{
    static void Main()
    {
        
        Task[] tasks = {
            new Task1("Have a good day :)"),      
            new Task2("Have a good day :)")       
        };
        Console.WriteLine(tasks[0]);
        Console.WriteLine(tasks[1]);
        

       
        string path = @"C:\Users\b823\Documents"; //путь
        string folderName = "Solution"; // имя папки
        path = Path.Combine(path, folderName);
        if (!Directory.Exists(path))    
        {
            Directory.CreateDirectory(path);
        }
        string fileName1 = "task_1.json"; // имя файла
        string fileName2 = "task_2.json"; // имя файла

        fileName1 = Path.Combine(path, fileName1);
        fileName2 = Path.Combine(path, fileName2);


        
        if (!File.Exists(fileName1))
        {
            var filec = File.Create(fileName1);
            filec.Close();
        }


        if (!File.Exists(fileName2))
        {
            JsonIO.Write<Task1>(tasks[0] as Task1, fileName1);
            JsonIO.Write<Task2>((Task2)tasks[1], fileName2);
        }
        else
        {
            var t1 = JsonIO.Read<Task1>(fileName1);
            var t2 = JsonIO.Read<Task2>(fileName2);
            Console.WriteLine(t1);
            Console.WriteLine(t2);
        }
    }
}