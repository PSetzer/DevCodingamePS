using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Game
{
    static Player me;
    static Player opponent;
    static List<string> lstActions;
    static List<Card> lstCardInDeck = new List<Card>();

    static void Main(string[] args)
    {
        // game loop
        while (true)
        {
            GetInput();

            lstActions = new List<string>();
            //lstCardInDeck = new List<Card>();

            if (me.mana == 0)
            {
                PickCard();
            }
            else
            {
                Player meMC = new Player(me);
                Player oppMC = new Player(opponent);

                // phase d'invocation
                List<Card> lstPlayable = meMC.lstCardsInHand.Where(x => x.cost <= meMC.mana).ToList();
                List<IOrderedEnumerable<Card>> lstPossiblePlays = GetSubsets(lstPlayable)?.Where(x => x.Sum(y => y?.cost ?? 0) <= meMC.mana)?.Select(x => x.OrderBy(y => y?.type ?? 0))?.ToList() ?? new List<IOrderedEnumerable<Card>>();

                List<string> lstActionsMC = new List<string>();
                List<(string, double)> lstScoresOfActionsMC = new List<(string, double)>();

                foreach (IEnumerable<Card> possiblePlay in lstPossiblePlays)
                {
                    meMC.ReinitWith(me);
                    oppMC.ReinitWith(opponent);
                    lstActionsMC = new List<string>();

                    foreach (Card card in possiblePlay)
                    {
                        if (card != null)
                        {
                            if (card.type == 0)
                            {
                                lstActionsMC.Add($"SUMMON {card.id}");
                                card.wasJustSummoned = true;
                                meMC.lstCardsOnBoard.Add(card.Clone() as Card);
                                PlayCardOnBoard(card);
                                meMC.health += card.myHealthChange;
                                meMC.health += card.myHealthChange;
                            }
                            else if (card.type == 1)
                            {
                                Card myCreatureTarget = meMC.lstCardsOnBoard.FirstOrDefault(x => x.isGuard) ?? meMC.lstCardsOnBoard.FirstOrDefault(); // utiliser un filtre Lethal, Drain, etc
                                if (myCreatureTarget != null)
                                {
                                    lstActionsMC.Add($"USE {card.id} {myCreatureTarget.id}");
                                    PlayCardOnBoard(card);
                                    myCreatureTarget.attack += card.attack;
                                    myCreatureTarget.defense += card.defense;
                                    myCreatureTarget.abilities |= card.abilities;
                                    meMC.health += card.myHealthChange;
                                    oppMC.health += card.opponentHealthChange;
                                }
                            }
                            else if (card.type > 1)
                            {
                                Card oppCreatureTarget = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.isGuard) ?? oppMC.lstCardsOnBoard.FirstOrDefault();
                                if (oppCreatureTarget != null)
                                {
                                    lstActionsMC.Add($"USE {card.id} {oppCreatureTarget.id}");
                                    PlayCardOnBoard(card);
                                    oppCreatureTarget.attack += card.attack;
                                    oppCreatureTarget.defense += card.defense;
                                    oppCreatureTarget.abilities &= ~card.abilities;
                                    meMC.health += card.myHealthChange;
                                    oppMC.health += card.opponentHealthChange;
                                }
                                else if (card.type == 3)
                                {
                                    lstActionsMC.Add($"USE {card.id} -1");
                                    meMC.health += card.myHealthChange;
                                    oppMC.health += card.opponentHealthChange;
                                    oppMC.health += card.defense;
                                }
                            }
                        }
                    }

                    // phase d'attaque
                    AttackPhase(meMC, oppMC, lstActionsMC);
                    if (!lstActionsMC.Any()) lstActionsMC.Add("PASS");
                    lstScoresOfActionsMC.Add((string.Join(";", lstActionsMC), ScoreMC(meMC, oppMC)));
                }

                if (lstScoresOfActionsMC.Any())
                {
                    double maxScore = lstScoresOfActionsMC.Max(x => x.Item2);
                    lstActions.Add(lstScoresOfActionsMC.FirstOrDefault(x => x.Item2 == maxScore).Item1);
                }


                /*bool summoned = true;
                while (summoned)
                {
                    List<Card> lstPlayable = me.lstCardsInHand.Where(x => x.cost <= me.mana).ToList();

                    List<Card> lstSummonable = lstPlayable.Where(x => x.type == 0).ToList();

                    Card toSummon = null;
                    if (lstSummonable.Count > 0)
                    {
                        if (toSummon == null)
                            toSummon = lstSummonable.FirstOrDefault(x => x.isGuard);
                        if (toSummon == null)
                            toSummon = lstSummonable.FirstOrDefault(x => x.isLethal);
                        if (toSummon == null)
                            toSummon = lstSummonable.FirstOrDefault(x => x.hasDrain);
                        if (toSummon == null)
                            toSummon = lstSummonable.FirstOrDefault(x => x.hasCharge);
                        if (toSummon == null)
                            toSummon = lstSummonable.FirstOrDefault(x => x.cost == lstSummonable.Max(y => y.cost));
                    }

                    if (toSummon != null)
                    {
                        lstActions.Add($"SUMMON {toSummon.id}");
                        PlayCardOnBoard(toSummon);

                        toSummon.wasJustSummoned = true;
                        me.lstCardsOnBoard.Add(toSummon);
                    }
                    else
                        summoned = false;

                    List<Card> lstUsable = lstPlayable.Where(x => x.type > 0).ToList();

                    Card toUseOnMyCreature = null;
                    toUseOnMyCreature = lstUsable.FirstOrDefault(x => x.type == 1 && x.cost <= me.mana);

                    if (toUseOnMyCreature != null)
                    {
                        Card creatureToBuff = me.lstCardsOnBoard.FirstOrDefault(x => x.isGuard) ?? me.lstCardsOnBoard.FirstOrDefault(); // utiliser un filtre Lethal, Drain, etc
                        if (creatureToBuff != null)
                        {
                            lstActions.Add($"USE {toUseOnMyCreature.id} {creatureToBuff.id}");
                            PlayCardOnBoard(toUseOnMyCreature);

                        }
                    }

                    Card toUseOnOppCreature = null;

                    toUseOnOppCreature = lstUsable.FirstOrDefault(x => x.type > 1 && x.cost <= me.mana);

                    if (toUseOnOppCreature != null)
                    {
                        Card creatureToDebuff = opponent.lstCardsOnBoard.FirstOrDefault(x => x.isGuard) ?? opponent.lstCardsOnBoard.FirstOrDefault();
                        if (creatureToDebuff != null)
                        {
                            lstActions.Add($"USE {toUseOnOppCreature.id} {creatureToDebuff.id}");
                            PlayCardOnBoard(toUseOnOppCreature);

                        }
                        else if (toUseOnOppCreature.type == 3)
                        {
                            lstActions.Add($"USE {toUseOnOppCreature.id} -1");
                            PlayCardOnBoard(toUseOnOppCreature);

                        }
                    }
                }*/
            }

            if (lstActions.Count == 0)
                lstActions.Add("PASS");

            // To debug: Console.Error.WriteLine("Debug messages...");
            Console.WriteLine(string.Join(";", lstActions));
        }
    }

    static void PickCard()
    {
        int pick = 0;
        Card cardInHand;

        double objectRate = 0;
        double lowCostRate = 0;
        if (lstCardInDeck.Count > 0)
        {
            objectRate = (double)lstCardInDeck.Count(x => x.type > 0) / 30;
            lowCostRate = (double)lstCardInDeck.Count(x => x.cost < 4) / 30;
        }

        for (int i = 0; i < me.lstCardsInHand.Count; i++)
        {
            cardInHand = me.lstCardsInHand[i];

            /*if (objectRate >= 0.33 && cardInHand.type > 0)
                continue;

            if (lstCardInDeck.Count > 15 && lowCostRate < 0.50 && cardInHand.cost > 3)
                continue;*/

            if (cardInHand.isGuard)
            {
                pick = i;
                break;
            }
            else if (cardInHand.isLethal)
            {
                pick = i;
                break;
            }
            else if (cardInHand.hasDrain)
            {
                pick = i;
                break;
            }
            else if (cardInHand.hasWard)
            {
                pick = i;
                break;
            }
            else if (cardInHand.hasCharge)
            {
                pick = i;
                break;
            }
        }

        Card cardPiked = me.lstCardsInHand[pick];
        lstCardInDeck.Add(cardPiked);

        lstActions.Add($"PICK {pick}");
    }

    static void AttackPhase(Player meMC, Player oppMC, List<string> listActions)
    {
        while (meMC.lstCardsOnBoard.Any(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0))
        {
            List<Card> lstReadyCards = meMC.lstCardsOnBoard.Where(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0).ToList();

            Card defender = null;
            Card attacking = null;
            Card readyLethal = lstReadyCards.FirstOrDefault(x => x.isLethal);
            int maxOppDefense = oppMC.lstCardsOnBoard.Count > 0 ? oppMC.lstCardsOnBoard.Max(x => x.defense) : 0;

            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.isGuard && x.defense == maxOppDefense && readyLethal != null);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.isGuard);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.number == 14);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.number == 20);
            if (defender == null && maxOppDefense >= 4)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.defense == maxOppDefense && readyLethal != null);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.number == 16);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.number == 42);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.number == 35);
            if (defender == null && lstReadyCards.Any(y => y.defense >= 5))
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.isLethal);
            if (defender == null)
                defender = oppMC.lstCardsOnBoard.FirstOrDefault(x => x.attack >= 5 && lstReadyCards.Any(y => x.defense <= y.attack));

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
                    AttackOpponentCard(attacking, defender, meMC, oppMC, listActions);
                }
            }
            else
            {
                if (attacking == null)
                    attacking = meMC.lstCardsOnBoard.FirstOrDefault(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge));

                if (attacking != null)
                {
                    listActions.Add($"ATTACK {attacking.id} -1");
                    attacking.hasAttacked = true;
                    oppMC.health -= attacking.attack;
                }
            }
        }
    }

    static double ScoreMC(Player meMC, Player oppMC)
    {
        double score = 0;

        if (meMC.health == 0) score = -10000;
        else if (oppMC.health == 0) score = 10000;
        else
        {
            int diffMyAttack = meMC.lstCardsOnBoard.Sum(x => x.attack) - me.lstCardsOnBoard.Sum(x => x.attack);
            int diffMyDefense = meMC.lstCardsOnBoard.Sum(x => x.defense) - me.lstCardsOnBoard.Sum(x => x.defense);
            int diffOppAttack = opponent.lstCardsOnBoard.Sum(x => x.attack) - oppMC.lstCardsOnBoard.Sum(x => x.attack);
            int diffOppDefense = opponent.lstCardsOnBoard.Sum(x => x.defense) - oppMC.lstCardsOnBoard.Sum(x => x.defense);
            int diffMyHealth = meMC.health - me.health;
            int diffOppHealth = opponent.health - oppMC.health;

            int diffMyNbGuard = meMC.lstCardsOnBoard.Count(x => x.isGuard) - me.lstCardsOnBoard.Count(x => x.isGuard);
            int diffOppNbGuard = opponent.lstCardsOnBoard.Count(x => x.isGuard) - oppMC.lstCardsOnBoard.Count(x => x.isGuard);

            int diffMyNbLethal = meMC.lstCardsOnBoard.Count(x => x.isLethal) - me.lstCardsOnBoard.Count(x => x.isLethal);
            int diffOppNbLethal = opponent.lstCardsOnBoard.Count(x => x.isLethal) - oppMC.lstCardsOnBoard.Count(x => x.isLethal);

            int diffMyNbDrain = meMC.lstCardsOnBoard.Count(x => x.hasDrain) - me.lstCardsOnBoard.Count(x => x.hasDrain);
            int diffOppNbDrain = opponent.lstCardsOnBoard.Count(x => x.hasDrain) - oppMC.lstCardsOnBoard.Count(x => x.hasDrain);

            int diffMyNbWard = meMC.lstCardsOnBoard.Count(x => x.hasWard) - me.lstCardsOnBoard.Count(x => x.hasWard);

            score += 50 * diffMyAttack;
            score += 50 * diffMyDefense;
            score += 50 * diffOppAttack;
            score += 50 * diffOppDefense;

            score += 50 * diffMyNbGuard;
            score += 50 * diffOppNbGuard;
            score += 30 * diffMyNbLethal;
            score += 30 * diffOppNbLethal;
            score += 30 * diffMyNbDrain;
            score += 30 * diffOppNbDrain;
            score += 30 * diffMyNbWard;

            score += 100 * diffMyHealth;
            score += 150 * diffOppHealth;
        }

        return score;
    }

    static void PlayCardOnBoard(Card card)
    {
        me.mana -= card.cost;
        me.lstCardsInHand.Remove(card);
    }

    static void AttackOpponentCard(Card myCreature, Card opponentCreature, Player me, Player opponent, List<string> listActions)
    {
        listActions.Add($"ATTACK {myCreature.id} {opponentCreature.id}");
        myCreature.hasAttacked = true;

        if (myCreature.hasWard)
            myCreature.abilities &= ~ab.ward;
        else
        {
            myCreature.defense -= opponentCreature.attack;
            if (opponentCreature.isLethal)
                me.lstCardsOnBoard.Remove(myCreature);
        }
        if (myCreature.defense <= 0)
            me.lstCardsOnBoard.Remove(myCreature);

        if (opponentCreature.hasWard)
            opponentCreature.abilities &= ~ab.ward;
        else
        {
            opponentCreature.defense -= myCreature.attack;
            if (myCreature.hasBreakthrough)
                opponent.health -= (myCreature.attack - opponentCreature.defense);
            if (myCreature.hasDrain)
                me.health += myCreature.attack;
            if (myCreature.isLethal)
                opponent.lstCardsOnBoard.Remove(opponentCreature);
        }
        if (opponentCreature.defense <= 0)
            opponent.lstCardsOnBoard.Remove(opponentCreature);
    }

    static void GetInput()
    {
        string[] inputs;

        inputs = Console.ReadLine().Split(' ');
        PrintList(inputs);
        me = new Player()
        {
            health = int.Parse(inputs[0]),
            mana = int.Parse(inputs[1]),
            nbCardInDeck = int.Parse(inputs[2]),
            nbRunes = int.Parse(inputs[3]),
            draw = int.Parse(inputs[4])
        };

        inputs = Console.ReadLine().Split(' ');
        PrintList(inputs);
        opponent = new Player()
        {
            health = int.Parse(inputs[0]),
            mana = int.Parse(inputs[1]),
            nbCardInDeck = int.Parse(inputs[2]),
            nbRunes = int.Parse(inputs[3]),
            draw = int.Parse(inputs[4])
        };

        inputs = Console.ReadLine().Split(' ');
        PrintList(inputs);
        opponent.nbCardInHand = int.Parse(inputs[0]);
        int opponentActions = int.Parse(inputs[1]);

        for (int i = 0; i < opponentActions; i++)
        {
            inputs = Console.ReadLine().Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            PrintList(inputs);
            int cardPlayedId = int.Parse(inputs[0]);
            opponent.lstActions.Add(new Action()
            {
                type = inputs[1],
                cardId1 = inputs.Length >= 3 ? int.Parse(inputs[2]) : 0,
                cardId2 = inputs.Length >= 4 ? int.Parse(inputs[3]) : 0,
            });
        }

        int cardCount = int.Parse(Console.ReadLine());
        PrintLine(cardCount);
        for (int i = 0; i < cardCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            PrintList(inputs);
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
                opponentHealthChange = int.Parse(inputs[9]),
                cardDraw = int.Parse(inputs[10]),
                wasJustSummoned = false,
                hasAttacked = false
            };

            if (card.location == 0)
                me.lstCardsInHand.Add(card);
            else if (card.location == 1)
                me.lstCardsOnBoard.Add(card);
            else if (card.location == -1)
                opponent.lstCardsOnBoard.Add(card);
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

class Player
{
    public int health;
    public int mana;
    public int nbCardInDeck;
    public int nbRunes;
    public int draw;
    public int nbCardInHand;
    public List<Card> lstCardsInHand = new List<Card>();
    public List<Card> lstCardsOnBoard = new List<Card>();
    public List<Action> lstActions = new List<Action>();

    public Player()
    { }

    public Player(Player init)
    {
        ReinitWith(init);
    }

    public override string ToString()
    {
        return $"health : {health} mana : {mana} nbCardInDeck : {nbCardInDeck} nbRunes : {nbRunes} draw : {draw} nbCardInHand : {nbCardInHand}\n";
    }

    public void ReinitWith(Player init)
    {
        health = init.health;
        mana = init.mana;
        nbCardInDeck = init.nbCardInDeck;
        nbRunes = init.nbRunes;
        draw = init.draw;
        nbCardInHand = init.nbCardInHand;

        Utils.EqualizeListLength(lstCardsInHand, init.lstCardsInHand);
        for (int i = 0; i < lstCardsInHand.Count; i++)
            lstCardsInHand[i].ReinitWith(init.lstCardsInHand[i]);

        Utils.EqualizeListLength(lstCardsOnBoard, init.lstCardsOnBoard);
        for (int i = 0; i < lstCardsOnBoard.Count; i++)
            lstCardsOnBoard[i].ReinitWith(init.lstCardsOnBoard[i]);

        Utils.EqualizeListLength(lstActions, init.lstActions);
        for (int i = 0; i < lstActions.Count; i++)
            lstActions[i].ReinitWith(init.lstActions[i]);
    }
}

class Card : ICloneable
{
    public int number;
    public int id;
    public int location;
    public int type;
    public int cost;
    public int attack;
    public int defense;
    public ab abilities;
    public int myHealthChange;
    public int opponentHealthChange;
    public int cardDraw;
    public bool hasBreakthrough { get => (abilities & ab.breakthrough) != 0; }
    public bool hasCharge { get => (abilities & ab.charge) != 0; }
    public bool hasDrain { get => (abilities & ab.drain) != 0; }
    public bool isGuard { get => (abilities & ab.guard) != 0; }
    public bool isLethal { get => (abilities & ab.lethal) != 0; }
    public bool hasWard { get => (abilities & ab.ward) != 0; }
    public bool wasJustSummoned;
    public bool hasAttacked;

    public Card()
    { }

    public Card(Card init)
    {
        ReinitWith(init);
    }

    public object Clone()
    {
        return new Card(this);
    }



    public override string ToString()
    {
        return $"id : {id} type : {type} loc : {location} cost : {cost} a : {attack} d : {defense} ab : {abilities}\n";
    }

    public void ReinitWith(Card init)
    {
        number = init.number;
        id = init.id;
        location = init.location;
        type = init.type;
        cost = init.cost;
        attack = init.attack;
        defense = init.defense;
        abilities = init.abilities;
        myHealthChange = init.myHealthChange;
        opponentHealthChange = init.opponentHealthChange;
        cardDraw = init.cardDraw;
        wasJustSummoned = init.wasJustSummoned;
        hasAttacked = init.hasAttacked;
    }
}

class Action
{
    public string type;
    public int cardId1;
    public int cardId2;

    public Action()
    { }

    public Action(Action init)
    {
        ReinitWith(init);
    }

    public override string ToString()
    {
        return $"{type} {cardId1} {cardId2}\n";
    }

    public void ReinitWith(Action init)
    {
        type = init.type;
        cardId1 = init.cardId1;
        cardId2 = init.cardId2;
    }
}

static class Utils
{
    public static void EqualizeListLength<T>(List<T> target, List<T> init) where T : new()
    {
        while (target.Count < init.Count)
            target.Add(new T());
        while (target.Count > init.Count)
            target.RemoveAt(0);
    }
}

enum ab
{
    breakthrough =  0b_10_0000,
    charge =        0b_01_0000,
    drain =         0b_00_1000,
    guard =         0b_00_0100,
    lethal =        0b_00_0010,
    ward =          0b_00_0001,
}