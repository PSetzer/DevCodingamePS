using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProgramUtils
{
    static bool isConsole = false;
    static System.IO.StreamReader file = isConsole ? null : new System.IO.StreamReader("../../input1.txt");

    static void MainUtils(string[] args)
    {

    }

    static IEnumerable<IEnumerable<T>> GetSubsetsWithBitwise<T>(IList<T> source)
    {
        List<List<T>> lstSubsets = new List<List<T>>();

        int combinations = 1 << source.Count;
        for (int i = 0; i < combinations; i++)
        {
            List<T> subSet = new List<T>();

            for (int j = 0; j < source.Count; j++)
                if ((i & (1 << j)) != 0) subSet.Add(source[j]);

            lstSubsets.Add(subSet);
        }

        return lstSubsets;
    }

    static IEnumerable<IEnumerable<T>> GetSubSetsWithConcat<T>(IEnumerable<T> source)
    {
        var result = new List<IEnumerable<T>>() { new List<T>() };

        for (int i = 0; i < source.Count(); i++)
        {
            var elem = source.Skip(i).Take(1);
            var matchUps = result.Select(x => x.Concat(elem));
            result = result.Concat(matchUps).ToList();
        }

        return result;
    }

    static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1)
            return list.Select(t => new T[] { t });
        else
            return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    /*static IEnumerable<IEnumerable<T>> GetPermutations2<T>(IList<T> list)
    {
        List<IEnumerable<T>> res = new List<IEnumerable<T>>();
        for (int i = 0; i < list.Count; i++)
        {
            T elem = list[i];
            list.Remove(elem);
            for (int j = 0; j <= list.Count; j++)
            {
                list.Insert(j, elem);
                res.Add(list);
            }
        }
        return res.Distinct();
    }*/

    static List<List<T>> GetListCombinations<T>(List<List<T>> lstLstSource, List<T> lstInit)
    {
        List<List<T>> lstLstCombinations = new List<List<T>>();

        int numLoops = lstLstSource.Count;
        int[] loopIndex = new int[numLoops];
        int[] loopCnt = new int[numLoops];

        for (int i = 0; i < numLoops; i++) loopIndex[i] = 0;
        for (int i = 0; i < numLoops; i++) loopCnt[i] = lstLstSource[i].Count;

        bool finished = false;
        while (!finished)
        {
            // access current element
            List<T> lstCardTarget = new List<T>(lstInit);
            for (int i = 0; i < numLoops; i++)
            {
                lstCardTarget.Add(lstLstSource[i][loopIndex[i]]);
            }
            lstLstCombinations.Add(lstCardTarget);
            int n = numLoops - 1;
            for (; ; )
            {
                // increment innermost loop
                loopIndex[n]++;
                // if at Cnt: reset, increment outer loop
                if (loopIndex[n] < loopCnt[n]) break;

                loopIndex[n] = 0;
                n--;
                if (n < 0)
                {
                    finished = true;
                    break;
                }
            }
        }

        return lstLstCombinations;
    }

    static (string num, string sign) Tournament(Stack<(string num, string sign)> lstPlayers, Stack<(string num, string sign)> lstNextPlayers)
    {
        if (lstPlayers.Count == 1)
            return lstPlayers.Single();
        else
        {
            lstNextPlayers.Push(GetWinner(lstPlayers.Pop(), lstPlayers.Pop()));

            if (lstPlayers.Count == 0)
            {
                lstPlayers = new Stack<(string num, string sign)>(lstNextPlayers);
                lstNextPlayers.Clear();
            }

            return Tournament(lstPlayers, lstNextPlayers);
        }

        (string num, string sign) GetWinner((string num, string sign) p1, (string num, string sign) p2)
        {
            return ("", "");
        }
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

