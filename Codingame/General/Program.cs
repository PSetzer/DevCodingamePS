using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static bool isConsole = false;
    static System.IO.StreamReader file = isConsole ? null : new System.IO.StreamReader("../../input1.txt");

    static void Main(string[] args)
    {
        
    }

    static string ReadLine() => file == null ? Console.ReadLine() : file.ReadLine();

    static void PrintLine(object toPrint, string title = "")
    {
        if (!string.IsNullOrEmpty(title))
            Console.Error.Write($"{title} : ");
        Console.Error.WriteLine(toPrint?.ToString() ?? "");
    }

    static void PrintList(IEnumerable list, string title = "")
    {
        if (!string.IsNullOrEmpty(title))
            Console.Error.Write($"{title} : ");
        foreach (var item in list)
        {
            Console.Error.Write($"{item} ");
        }
        Console.Error.WriteLine();
    }
}

