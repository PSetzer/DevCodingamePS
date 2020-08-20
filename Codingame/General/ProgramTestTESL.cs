using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class ProgramTestTESL
{
    static bool isConsole = false;
    static System.IO.StreamReader file = isConsole ? null : new System.IO.StreamReader("../../input2.txt");

    static Player meInit;
    static Player oppInit;
    static List<string> lstActions;
    static List<Card> lstCardInDeck = new List<Card>();

    static double ScoreMC(Player meSimu, Player oppSimu)
    {
        double score = 0;

        if (meSimu.health <= 0) score = -10000;
        else if (oppSimu.health <= 0) score = 10000;
        else
        {
            int diffMyAttack = meSimu.lstCardsOnBoard.Sum(x => x.attack) - meInit.lstCardsOnBoard.Sum(x => x.attack);
            int diffMyDefense = meSimu.lstCardsOnBoard.Sum(x => x.defense) - meInit.lstCardsOnBoard.Sum(x => x.defense);
            int diffOppAttack = oppInit.lstCardsOnBoard.Sum(x => x.attack) - oppSimu.lstCardsOnBoard.Sum(x => x.attack);
            int diffOppDefense = oppInit.lstCardsOnBoard.Sum(x => x.defense) - oppSimu.lstCardsOnBoard.Sum(x => x.defense);
            int diffMyHealth = meSimu.health - meInit.health;
            int diffOppHealth = oppInit.health - oppSimu.health;

            int diffMyNbGuard = meSimu.lstCardsOnBoard.Count(x => x.isGuard) - meInit.lstCardsOnBoard.Count(x => x.isGuard);
            int diffOppNbGuard = oppInit.lstCardsOnBoard.Count(x => x.isGuard) - oppSimu.lstCardsOnBoard.Count(x => x.isGuard);

            int diffMyNbLethal = meSimu.lstCardsOnBoard.Count(x => x.isLethal) - meInit.lstCardsOnBoard.Count(x => x.isLethal);
            int diffOppNbLethal = oppInit.lstCardsOnBoard.Count(x => x.isLethal) - oppSimu.lstCardsOnBoard.Count(x => x.isLethal);

            int diffMyNbDrain = meSimu.lstCardsOnBoard.Count(x => x.hasDrain) - meInit.lstCardsOnBoard.Count(x => x.hasDrain);
            int diffOppNbDrain = oppInit.lstCardsOnBoard.Count(x => x.hasDrain) - oppSimu.lstCardsOnBoard.Count(x => x.hasDrain);

            int diffMyNbWard = meSimu.lstCardsOnBoard.Count(x => x.hasWard) - meInit.lstCardsOnBoard.Count(x => x.hasWard);

            bool hasOppLostRune = oppSimu.nbRunes < oppInit.nbRunes;

            score += 50 * diffMyAttack;
            score += 50 * diffMyDefense;
            score += 50 * diffOppAttack;
            score += 50 * diffOppDefense;

            score += 80 * diffMyNbGuard;
            score += 50 * diffOppNbGuard;
            score += 30 * diffMyNbLethal;
            score += 30 * diffOppNbLethal;
            score += 30 * diffMyNbDrain;
            score += 30 * diffOppNbDrain;
            score += 50 * diffMyNbWard;

            score += 100 * diffMyHealth;
            score += 150 * diffOppHealth;

            /*if (hasOppLostRune)
                score -= 150;*/
        }

        return score;
    }

    static void Main(string[] args)
    {
        GetInput();

        lstActions = new List<string>();
        //lstCardInDeck = new List<Card>();

        
        Player meSimu = new Player(meInit);
        Player oppSimu = new Player(oppInit);

        // phase d'invocation
        List<Card> lstPlayable = meSimu.lstCardsInHand.Where(x => x.cost <= meSimu.mana).ToList();
        IEnumerable<IEnumerable<Card>> lstCardSubsets = GetSubsets(lstPlayable)?.Where(x => x.Sum(y => y?.cost ?? 0) <= meSimu.mana)?.ToList() ?? new List<IEnumerable<Card>>();
        IEnumerable<List<(int cardId, int targetId)>> lstAllPossibleInvokes = GetAllPossibleInvokes(meSimu, oppSimu, lstCardSubsets);

        List<string> lstActionsMC = new List<string>();
        List<(string, double)> lstScoresOfActionsMC = new List<(string, double)>();

        foreach (List<(int cardId, int targetId)> lstPossibleInvokes in lstAllPossibleInvokes)
        {
            meSimu.ReinitWith(meInit);
            oppSimu.ReinitWith(oppInit);
            lstActionsMC = new List<string>();

            foreach ((int cardId, int targetId) cardTarget in lstPossibleInvokes)
            {
                Card card = meSimu.lstCardsInHand.FirstOrDefault(x => x.id == cardTarget.cardId);

                if (card.type == 0)
                {
                    lstActionsMC.Add($"SUMMON {card.id}");
                    card.wasJustSummoned = true;
                    meSimu.lstCardsOnBoard.Add(card.Clone() as Card);
                    PlayCardOnBoard(card);
                    meSimu.health += card.myHealthChange;
                    oppSimu.health += card.oppHealthChange;
                    if (oppSimu.health < oppSimu.nbRunes) oppSimu.nbRunes = oppSimu.health;
                }
                else if (card.type == 1)
                {
                    Card target = meSimu.lstCardsOnBoard.FirstOrDefault(x => x.id == cardTarget.targetId);
                    lstActionsMC.Add($"USE {card.id} {target.id}");
                    PlayCardOnBoard(card);
                    target.attack += card.attack;
                    target.defense += card.defense;
                    target.abilities |= card.abilities;
                    meSimu.health += card.myHealthChange;
                    oppSimu.health += card.oppHealthChange;
                    if (oppSimu.health < oppSimu.nbRunes) oppSimu.nbRunes = oppSimu.health;
                }
                else if (card.type > 1)
                {
                    if (cardTarget.targetId > 0)
                    {
                        Card target = oppSimu.lstCardsOnBoard.FirstOrDefault(x => x.id == cardTarget.targetId);
                        lstActionsMC.Add($"USE {card.id} {target.id}");
                        PlayCardOnBoard(card);
                        target.attack += card.attack;
                        target.abilities &= ~card.abilities;
                        if (target.hasWard && card.defense < 0)
                            target.abilities &= ~ab.ward;
                        else
                        {
                            target.defense += card.defense;
                            if (target.defense <= 0)
                                oppSimu.lstCardsOnBoard.Remove(target);
                        }
                        meSimu.health += card.myHealthChange;
                        oppSimu.health += card.oppHealthChange;
                        if (oppSimu.health < oppSimu.nbRunes) oppSimu.nbRunes = oppSimu.health;
                    }
                    else
                    {
                        lstActionsMC.Add($"USE {card.id} -1");
                        meSimu.health += card.myHealthChange;
                        oppSimu.health += card.oppHealthChange;
                        oppSimu.health += card.defense;
                        if (oppSimu.health < oppSimu.nbRunes) oppSimu.nbRunes = oppSimu.health;
                    }
                }
            }

            // phase d'attaque
            AttackPhase(meSimu, oppSimu, lstActionsMC);
            //AttackPhase(oppSimu, meSimu);
            if (!lstActionsMC.Any()) lstActionsMC.Add("PASS");
            lstScoresOfActionsMC.Add((string.Join(";", lstActionsMC), ScoreMC(meSimu, oppSimu)));
        }



        if (lstScoresOfActionsMC.Any())
        {
            double maxScore = lstScoresOfActionsMC.Max(x => x.Item2);
            lstActions.Add(lstScoresOfActionsMC.FirstOrDefault(x => x.Item2 == maxScore).Item1);
        }
    }

    static IEnumerable<List<(int cardId, int targetId)>> GetAllPossibleInvokes(Player me, Player opp, IEnumerable<IEnumerable<Card>> lstCardSubsets)
    {
        IEnumerable<List<(int cardId, int targetId)>> lstAllPossibleInvokes = Enumerable.Empty<List<(int cardId, int targetId)>>();

        List<(int cardId, int targetId)> lstCardTarget = new List<(int cardId, int targetId)>();
        List<(int cardId, int targetId)> lstSummonsOfCardSubset = new List<(int cardId, int targetId)>();
        List<List<(int cardId, int targetId)>> lstLstCardTargetOfCardSubset = new List<List<(int cardId, int targetId)>>();

        foreach (IEnumerable<Card> cardSubset in lstCardSubsets)
        {
            lstSummonsOfCardSubset = new List<(int cardId, int targetId)>();
            lstLstCardTargetOfCardSubset = new List<List<(int cardId, int targetId)>>();

            foreach (Card card in cardSubset.Where(x => (x?.type ?? -1) == 0))
                lstSummonsOfCardSubset.Add((card.id, 0));

            foreach (Card card in cardSubset.Where(x => (x?.type ?? -1) == 1))
            {
                lstCardTarget = new List<(int cardId, int targetId)>();

                foreach (var targetId in me.lstCardsOnBoard.Select(x => (x.id, 0)).Concat(lstSummonsOfCardSubset))
                    lstCardTarget.Add((card.id, targetId.Item1));

                if (lstCardTarget.Count > 0)
                    lstLstCardTargetOfCardSubset.Add(lstCardTarget);
            }

            foreach (Card card in cardSubset.Where(x => (x?.type ?? -1) == 2 || ((x?.type ?? -1) == 3 && (x?.defense ?? 0) < 0)))
            {
                lstCardTarget = new List<(int cardId, int targetId)>();

                foreach (Card target in opp.lstCardsOnBoard)
                    lstCardTarget.Add((card.id, target.id));

                if (lstCardTarget.Count > 0)
                    lstLstCardTargetOfCardSubset.Add(lstCardTarget);
            }

            foreach (Card card in cardSubset.Where(x => (x?.type ?? -1) == 3))
            {
                lstCardTarget = new List<(int cardId, int targetId)>();
                lstCardTarget.Add((card.id, -1));
                lstLstCardTargetOfCardSubset.Add(lstCardTarget);
            }

            if (lstLstCardTargetOfCardSubset.Count > 0)
                lstAllPossibleInvokes = lstAllPossibleInvokes.Concat(GetListCombinations(lstLstCardTargetOfCardSubset, lstSummonsOfCardSubset));
            else
                lstAllPossibleInvokes = lstAllPossibleInvokes.Concat(new List<List<(int cardId, int targetId)>> { lstSummonsOfCardSubset });
        }

        return lstAllPossibleInvokes;
    }

    static void AttackPhase(Player me, Player opp, List<string> listActions = null)
    {
        while (me.lstCardsOnBoard.Any(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0))
        {
            List<Card> lstReadyCards = me.lstCardsOnBoard.Where(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0).ToList();

            Card defender = null;
            Card attacking = null;
            Card readyLethal = lstReadyCards.FirstOrDefault(x => x.isLethal);
            int maxOppDefense = opp.lstCardsOnBoard.Count > 0 ? opp.lstCardsOnBoard.Max(x => x.defense) : 0;

            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.isGuard && x.defense == maxOppDefense && readyLethal != null);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.isGuard);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.number == 14);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.number == 20);
            if (defender == null && maxOppDefense >= 4)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.defense == maxOppDefense && readyLethal != null);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.number == 16);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.number == 42);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.number == 35);
            if (defender == null && lstReadyCards.Any(y => y.defense >= 5))
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.isLethal);
            if (defender == null)
                defender = opp.lstCardsOnBoard.FirstOrDefault(x => x.attack >= 5 && lstReadyCards.Any(y => x.defense <= y.attack));

            if (defender != null)
            {
                int minDef = lstReadyCards.Any(x => x.attack >= defender.defense) ? lstReadyCards.Where(x => x.attack >= defender.defense).Min(x => x.defense) : 0;

                if (attacking == null)
                    attacking = lstReadyCards.FirstOrDefault(x => x.attack >= defender.defense && x.defense > defender.attack && !defender.isLethal);
                if (attacking == null)
                    attacking = lstReadyCards.FirstOrDefault(x => x.attack >= defender.defense && x.defense == minDef);
                if (attacking == null)
                    attacking = lstReadyCards.FirstOrDefault(x => x.defense > defender.attack && !defender.isLethal);
                if (attacking == null)
                    attacking = lstReadyCards.FirstOrDefault();

                if (attacking != null)
                {
                    AttackOpponentCard(attacking, defender, me, opp, listActions);
                }
            }
            else
            {
                if (attacking == null)
                    attacking = me.lstCardsOnBoard.FirstOrDefault(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge));

                if (attacking != null)
                {
                    listActions?.Add($"ATTACK {attacking.id} -1");
                    attacking.hasAttacked = true;
                    opp.health -= attacking.attack;
                    if (opp.health < opp.nbRunes) opp.nbRunes = opp.health;
                }
            }
        }
    }

    static void PlayCardOnBoard(Card card)
    {
        meInit.mana -= card.cost;
        meInit.lstCardsInHand.Remove(card);
    }

    static void AttackOpponentCard(Card myCreature, Card oppCreature, Player me, Player opp, List<string> listActions = null)
    {
        listActions?.Add($"ATTACK {myCreature.id} {oppCreature.id}");
        myCreature.hasAttacked = true;

        if (myCreature.hasWard)
            myCreature.abilities &= ~ab.ward;
        else
        {
            myCreature.defense -= oppCreature.attack;
            if (oppCreature.isLethal)
                me.lstCardsOnBoard.Remove(myCreature);
        }
        if (myCreature.defense <= 0)
            me.lstCardsOnBoard.Remove(myCreature);

        if (oppCreature.hasWard)
            oppCreature.abilities &= ~ab.ward;
        else
        {
            oppCreature.defense -= myCreature.attack;
            if (myCreature.hasBreakthrough)
            {
                opp.health -= (myCreature.attack - oppCreature.defense);
                if (opp.health < opp.nbRunes) opp.nbRunes = opp.health;

            }
            if (myCreature.hasDrain)
                me.health += myCreature.attack;
            if (myCreature.isLethal)
                opp.lstCardsOnBoard.Remove(oppCreature);
        }
        if (oppCreature.defense <= 0)
            opp.lstCardsOnBoard.Remove(oppCreature);
    }

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

    static IEnumerable<IEnumerable<T>> GetSubsets<T>(IList<T> source)
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

    static List<List<(int cardId, int targetId)>> GetListCombinations(List<List<(int cardId, int targetId)>> lstLstCardTargetOfCardSubset, List<(int cardId, int targetId)> lstSummonsOfCardSubset)
    {
        List<List<(int cardId, int targetId)>> lstLstPossiblePlaysOfCardSubset = new List<List<(int cardId, int targetId)>>();

        int numLoops = lstLstCardTargetOfCardSubset.Count;
        int[] loopIndex = new int[numLoops];
        int[] loopCnt = new int[numLoops];

        for (int i = 0; i < numLoops; i++) loopIndex[i] = 0;
        for (int i = 0; i < numLoops; i++) loopCnt[i] = lstLstCardTargetOfCardSubset[i].Count;

        bool finished = false;
        while (!finished)
        {
            // access current element
            List<(int cardId, int targetId)> lstCardTarget = new List<(int cardId, int targetId)> (lstSummonsOfCardSubset);
            for (int i = 0; i < numLoops; i++)
            {
                lstCardTarget.Add(lstLstCardTargetOfCardSubset[i][loopIndex[i]]);
            }
            lstLstPossiblePlaysOfCardSubset.Add(lstCardTarget);
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

        return lstLstPossiblePlaysOfCardSubset;
    }

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

    static string ReadLine() => file == null ? Console.ReadLine() : file.ReadLine();
}

