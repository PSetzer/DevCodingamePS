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

    static double ScoreInvokes(Player meSimu, Player oppSimu)
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

    static double ScoreAttacks(Player meSimu, Player oppSimu)
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

    static void MainTestTESL(string[] args)
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

        List<string> lstActionsInvoke = new List<string>();
        List<(List<string> actionsInvoke, Player meSimu, Player oppSimu, double score)> lstScoresOfPossibleInvokes = new List<(List<string>, Player, Player, double)>();

        foreach (List<(int cardId, int targetId)> lstPossibleInvokes in lstAllPossibleInvokes)
        {
            meSimu.ReinitWith(meInit);
            oppSimu.ReinitWith(oppInit);
            lstActionsInvoke = new List<string>();

            foreach ((int cardId, int targetId) cardTarget in lstPossibleInvokes)
            {
                Card card = meSimu.lstCardsInHand.FirstOrDefault(x => x.id == cardTarget.cardId);

                if (card.type == 0)
                {
                    lstActionsInvoke.Add($"SUMMON {card.id}");
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
                    lstActionsInvoke.Add($"USE {card.id} {target.id}");
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
                        lstActionsInvoke.Add($"USE {card.id} {target.id}");
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
                        lstActionsInvoke.Add($"USE {card.id} -1");
                        meSimu.health += card.myHealthChange;
                        oppSimu.health += card.oppHealthChange;
                        oppSimu.health += card.defense;
                        if (oppSimu.health < oppSimu.nbRunes) oppSimu.nbRunes = oppSimu.health;
                    }
                }
            }

            lstScoresOfPossibleInvokes.Add((lstActionsInvoke, new Player(meSimu), new Player(oppSimu), ScoreInvokes(meSimu, oppSimu)));
        }

        List<(List<string> actionsInvoke, Player meSimu, Player oppSimu)> lstBestInvokes =
            lstScoresOfPossibleInvokes.OrderByDescending(x => x.score).Take(3).Select(x => (x.actionsInvoke, x.meSimu, x.oppSimu)).ToList();

        // phase d'attaque des gardes
        List<(List<string> actionsAttackGuard, Player meSimu, Player oppSimu, double score)> lstScoresOfAttackGuards = new List<(List<string>, Player, Player, double)>();
        List<string> lstActionsAttackGuards = new List<string>();

        foreach (var invokes in lstBestInvokes)
        {
            List<Card> lstGuards = new List<Card>(invokes.oppSimu.lstCardsOnBoard.Where(x => x.isGuard));
            List<List<(int, int)>> lstAllPossibleAttackGuards = GetAllPossibleAttacks(invokes.meSimu.lstCardsOnBoard.Select(x => x.id), lstGuards.Select(x => x.id));

            foreach (List<(int, int)> lstAttacks in lstAllPossibleAttackGuards)
            {
                meSimu.ReinitWith(invokes.meSimu);
                oppSimu.ReinitWith(invokes.oppSimu);
                lstActionsAttackGuards = new List<string>();

                foreach ((int idAtt, int idDef) attack in lstAttacks)
                {
                    Card attacker = meSimu.lstCardsOnBoard.FirstOrDefault(x => x.id == attack.idAtt);
                    Card defender = oppSimu.lstCardsOnBoard.FirstOrDefault(x => x.id == attack.idDef);
                    AttackOpponentCard(attacker, defender, meSimu, oppSimu, lstActionsAttackGuards);
                }

                //if (!lstActionsAttackGuards.Any()) lstActionsAttackGuards.Add("PASS");
                lstScoresOfAttackGuards.Add((invokes.actionsInvoke.Concat(lstActionsAttackGuards).ToList(), new Player(meSimu), new Player(oppSimu), ScoreAttacks(meSimu, oppSimu)));
            }
        }

        List<(List<string> actionsAttackGuard, Player meSimu, Player oppSimu)> lstBestAttackGuards =
            lstScoresOfAttackGuards.OrderByDescending(x => x.score).Take(3).Select(x => (x.actionsAttackGuard, x.meSimu, x.oppSimu)).ToList();

        // phase d'attaque des non gardes
        List<(List<string> actionsAttackNonGuards, Player meSimu, Player oppSimu, double score)> lstScoresOfAttackNonGuards = new List<(List<string>, Player, Player, double)>();
        List<string> lstActionsAttackNonGuards = new List<string>();

        foreach (var attackGuards in lstBestAttackGuards)
        {
            if (attackGuards.oppSimu.lstCardsOnBoard.All(x => !x.isGuard))
            {
                List<Card> lstNotGuards = new List<Card>(attackGuards.oppSimu.lstCardsOnBoard);
                lstNotGuards.Add(new Card() { id = -1, defense = attackGuards.oppSimu.health });
                List<List<(int, int)>> lstAllPossibleAttackNonGuards = GetAllPossibleAttacks(attackGuards.meSimu.lstCardsOnBoard.Select(x => x.id), lstNotGuards.Select(x => x.id));

                foreach (List<(int, int)> lstAttacks in lstAllPossibleAttackNonGuards)
                {
                    meSimu.ReinitWith(attackGuards.meSimu);
                    oppSimu.ReinitWith(attackGuards.oppSimu);
                    lstActionsAttackNonGuards = new List<string>();

                    foreach ((int idAtt, int idDef) attack in lstAttacks)
                    {
                        Card attacker = meSimu.lstCardsOnBoard.FirstOrDefault(x => x.id == attack.idAtt);
                        Card defender = oppSimu.lstCardsOnBoard.FirstOrDefault(x => x.id == attack.idDef);
                        AttackOpponentCard(attacker, defender, meSimu, oppSimu, lstActionsAttackNonGuards);
                    }

                    //if (!lstActionsAttackNonGuards.Any()) lstActionsAttackNonGuards.Add("PASS");
                    lstScoresOfAttackNonGuards.Add((attackGuards.actionsAttackGuard.Concat(lstActionsAttackNonGuards).ToList(), new Player(meSimu), new Player(oppSimu), ScoreAttacks(meSimu, oppSimu)));
                }
            }
            else
                lstScoresOfAttackNonGuards.Add((attackGuards.actionsAttackGuard, new Player(meSimu), new Player(oppSimu), ScoreAttacks(meSimu, oppSimu)));
        }

        if (lstScoresOfAttackNonGuards.Any())
        {
            double maxScore = lstScoresOfAttackNonGuards.Max(x => x.score);
            lstActions = lstScoresOfAttackNonGuards.FirstOrDefault(x => x.score == maxScore).actionsAttackNonGuards;
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

    static List<List<(int, int)>> GetAllPossibleAttacks(IEnumerable<int> lstAttackers, IEnumerable<int> lstDefenders)
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

    /*static void AttackPhase(Player me, Player opp, List<string> listActions = null)
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
    }*/

    static void PlayCardOnBoard(Card card)
    {
        meInit.mana -= card.cost;
        meInit.lstCardsInHand.Remove(card);
    }

    static void AttackOpponentCard(Card myCreature, Card oppCreature, Player me, Player opp, List<string> listActions = null)
    {
        if (oppCreature.defense > 0)
        {
            listActions?.Add($"ATTACK {myCreature.id} {oppCreature.id}");
            myCreature.hasAttacked = true;

            if (myCreature.hasWard && oppCreature.attack > 0)
                myCreature.abilities &= ~ab.ward;
            else
            {
                myCreature.defense -= oppCreature.attack;
                if (oppCreature.isLethal)
                    me.lstCardsOnBoard.Remove(myCreature);
            }
            if (myCreature.defense <= 0)
                me.lstCardsOnBoard.Remove(myCreature);

            if (oppCreature.id == -1)
            {
                opp.health -= myCreature.attack;
                if (opp.health < opp.nbRunes) opp.nbRunes = opp.health;
            }
            else
            {
                if (oppCreature.hasWard && myCreature.attack > 0)
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
        }
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

    static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1)
            return list.Select(t => new T[] { t });
        else
            return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
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

