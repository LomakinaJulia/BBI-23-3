using System.IO;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;


abstract class Task
{
    protected string text = "No text here yet";
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
        return MostFrequentLetter(text).ToString();
    }

    private char MostFrequentLetter(string text)
    {
        int[] charCount = new int[33]; 

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsLetter(c))
            {
                int index = char.ToLower(c) - 'a'; 
                charCount[index]++;
            }
        }

        char mostFrequentChar = ' ';
        int maxCount = 0;

        for (int i = 0; i < charCount.Length; i++)
        {
            if (charCount[i] > maxCount)
            {
                maxCount = charCount[i];
                mostFrequentChar = (char)('a' + i); 
            }
        }

        return mostFrequentChar;
    }
}
class Task2 : Task
{
    public Task2(string text) : base(text) { }
    string ShiftLetters(string text)
    {
        char[] shiftedText = new char[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (char.IsLetter(c))
            {
               
                if (c >= 'а' && c <= 'я')
                {
                    c = (char)((c + 10 - 'а') % ('я' - 'а') + 'а');
                }
                else if (c >= 'А' && c <= 'Я')
                {
                    c = (char)((c + 10 - 'А') % ('Я' - 'А') + 'А');
                }
            }

            shiftedText[i] = c;
        }

        return new string(shiftedText);
    }

}

class Program
{
    static void Main()
    {
        
        Task[] tasks = {
            new Task1("Буду переписывать кр :)"), 
            new Task2("Буду переписывать кр :)")        
        };
        Console.WriteLine(tasks[0]);
        Console.WriteLine(tasks[1]);
    }
}
