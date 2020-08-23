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

    static Player meInit;
    static Player oppInit;

    static void Main(string[] args)
    {
        GetInput();
        List<List<(int, int)>> lstInit = GetAllPossibleAttacks(meInit.lstCardsOnBoard.Select(x => x.id).ToList(), oppInit.lstCardsOnBoard.Select(x => x.id).ToList());
    }

    static List<List<(int, int)>> GetAllPossibleAttacks(IList<int> lstAttackers, IList<int> lstDefenders)
    {
        List<List<(int, int)>> lstFinal = new List<List<(int, int)>> { new List<(int, int)>() };
        List<List<(int, int)>> lstInit;

        foreach (var attacker in lstAttackers)
        {
            lstInit = new List<List<(int, int)>>(lstFinal);
            lstFinal.Clear();

            foreach (var defender in lstDefenders)
            {
                lstFinal.AddRange(GetPermutationAttacks((attacker, defender), lstInit));
            }
        }

        return lstFinal;
    }

    static List<List<(int, int)>> GetPermutationAttacks((int, int) attackToAdd, List<List<(int, int)>> lstInit)
    {
        List<List<(int, int)>> lstFinal = new List<List<(int, int)>>();
        List<(int, int)> lstToInsert;

        foreach (var lst in lstInit)
        {
            for (int i = 0; i <= lst.Count; i++)
            {
                lstToInsert = new List<(int, int)>(lst);
                lstToInsert.Insert(i, attackToAdd);
                lstFinal.Add(lstToInsert);
            }
        }

        return lstFinal;
    }

    /*static IEnumerable<IEnumerable<T>> GetPermutations<T>(IList<T> list)
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

    static void GetInput()
    {
        string[] inputs;

        inputs = ReadLine().Split(' ');
        meInit = new Player()
        {
            health = int.Parse(inputs[0]),
            mana = int.Parse(inputs[1]),
            nbCardInDeck = int.Parse(inputs[2]),
            nbRunes = int.Parse(inputs[3]),
            draw = int.Parse(inputs[4])
        };

        inputs = ReadLine().Split(' ');
        oppInit = new Player()
        {
            health = int.Parse(inputs[0]),
            mana = int.Parse(inputs[1]),
            nbCardInDeck = int.Parse(inputs[2]),
            nbRunes = int.Parse(inputs[3]),
            draw = int.Parse(inputs[4])
        };

        inputs = ReadLine().Split(' ');
        oppInit.nbCardInHand = int.Parse(inputs[0]);
        int opponentActions = int.Parse(inputs[1]);

        for (int i = 0; i < opponentActions; i++)
        {
            inputs = ReadLine().Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            int cardPlayedId = int.Parse(inputs[0]);
            oppInit.lstActions.Add(new Action()
            {
                type = inputs[1],
                cardId1 = inputs.Length >= 3 ? int.Parse(inputs[2]) : 0,
                cardId2 = inputs.Length >= 4 ? int.Parse(inputs[3]) : 0,
            });
        }

        int cardCount = int.Parse(ReadLine());
        for (int i = 0; i < cardCount; i++)
        {
            inputs = ReadLine().Split(' ');
            Card card = new Card()
            {
                number = int.Parse(inputs[0]),
                id = int.Parse(inputs[1]),
                location = int.Parse(inputs[2]),
                type = int.Parse(inputs[3]),
                cost = int.Parse(inputs[4]),
                attack = int.Parse(inputs[5]),
                defense = int.Parse(inputs[6]),
                abilities = SetAbilities(inputs[7]),
                myHealthChange = int.Parse(inputs[8]),
                oppHealthChange = int.Parse(inputs[9]),
                cardDraw = int.Parse(inputs[10]),
                wasJustSummoned = false,
                hasAttacked = false
            };

            if (card.location == 0)
                meInit.lstCardsInHand.Add(card);
            else if (card.location == 1)
                meInit.lstCardsOnBoard.Add(card);
            else if (card.location == -1)
                oppInit.lstCardsOnBoard.Add(card);
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

    static ab SetAbilities(string input)
    {
        ab abilities = 0;

        if (input[0] != '-') abilities |= ab.breakthrough;
        if (input[1] != '-') abilities |= ab.charge;
        if (input[2] != '-') abilities |= ab.drain;
        if (input[3] != '-') abilities |= ab.guard;
        if (input[4] != '-') abilities |= ab.lethal;
        if (input[5] != '-') abilities |= ab.ward;

        return abilities;
    }
}

