using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProgramStub
{
    static bool isConsole = false;
    static System.IO.StreamReader file = isConsole ? null : new System.IO.StreamReader("../../input1.txt");

    static void MainStub(string[] args)
    {

    }

    static string ReadLine() => file == null ? Console.ReadLine() : file.ReadLine();

    static void PrintLine(object toPrint, string title = "")
    {
        if (!string.IsNullOrEmpty(title))
            Console.Error.Write($"{title} : ");
        Console.Error.WriteLine(toPrint?.ToString() ?? "");
    }

    static void PrintList(IEnumerable list, bool inline = true, string title = "")
    {
        if (!string.IsNullOrEmpty(title))
            Console.Error.Write($"{title} : ");
        foreach (var item in list)
        {
            if (inline)
                Console.Error.Write($"{item} ");
            else
                Console.Error.WriteLine($"{item} ");
        }

        if (inline)
            Console.Error.WriteLine();
    }
}

