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
                // phase d'invocation
                bool summoned = true;
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
                }

                // phase d'attaque
                while (me.lstCardsOnBoard.Any(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0))
                {
                    List<Card> lstAttackingCards = me.lstCardsOnBoard.Where(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0).ToList();

                    Card toAttack = null;
                    Card attacking = null;

                    if (toAttack == null)
                        toAttack = opponent.lstCardsOnBoard.FirstOrDefault(x => x.isGuard);
                    if (toAttack == null)
                        toAttack = opponent.lstCardsOnBoard.FirstOrDefault(x => lstAttackingCards.Any(y => x.defense <= y.attack && x.attack < y.defense));
                    /*if (toAttack == null)
                        toAttack = opponent.lstCardsOnBoard.FirstOrDefault(x => lstAttackingCards.Any(y => x.defense <= y.attack));
                    if (toAttack == null)
                        toAttack = opponent.lstCardsOnBoard.FirstOrDefault();*/

                    if (toAttack != null)
                    {
                        if (attacking == null)
                            attacking = lstAttackingCards.FirstOrDefault(x => x.attack >= toAttack.defense && x.defense > toAttack.attack);
                        if (attacking == null)
                            attacking = lstAttackingCards.FirstOrDefault(x => x.attack >= toAttack.defense);
                        if (attacking == null)
                            attacking = lstAttackingCards.FirstOrDefault(x => x.defense > toAttack.attack);
                        if (attacking == null)
                            attacking = lstAttackingCards.FirstOrDefault();

                        if (attacking != null)
                        {
                            AttackOpponentCard(attacking, toAttack);
                        }
                    }
                    else
                    {
                        if (attacking == null)
                            attacking = me.lstCardsOnBoard.FirstOrDefault(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge));

                        if (attacking != null)
                        {
                            lstActions.Add($"ATTACK {attacking.id} -1");
                            attacking.hasAttacked = true;
                            opponent.health -= attacking.attack;
                        }
                    }
                }
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

    static void PlayCardOnBoard(Card card)
    {
        me.mana -= card.cost;
        me.lstCardsInHand.Remove(card);
    }

    static void AttackOpponentCard(Card myCreature, Card opponentCreature)
    {
        lstActions.Add($"ATTACK {myCreature.id} {opponentCreature.id}");
        myCreature.hasAttacked = true;

        if (myCreature.hasWard)
            myCreature.hasWard = false;
        else
            myCreature.defense -= opponentCreature.attack;
        if (myCreature.defense <= 0)
            me.lstCardsOnBoard.Remove(myCreature);

        if (opponentCreature.hasWard)
            opponentCreature.hasWard = false;
        else
            opponentCreature.defense -= myCreature.attack;
        if (opponentCreature.defense <= 0)
            opponent.lstCardsOnBoard.Remove(opponentCreature);
    }

    static void GetInput()
    {
        string[] inputs;

        inputs = Console.ReadLine().Split(' ');
        me = new Player()
        {
            health = int.Parse(inputs[0]),
            mana = int.Parse(inputs[1]),
            nbCardInDeck = int.Parse(inputs[2]),
            nbRunes = int.Parse(inputs[3]),
            draw = int.Parse(inputs[4])
        };

        inputs = Console.ReadLine().Split(' ');
        opponent = new Player()
        {
            health = int.Parse(inputs[0]),
            mana = int.Parse(inputs[1]),
            nbCardInDeck = int.Parse(inputs[2]),
            nbRunes = int.Parse(inputs[3]),
            draw = int.Parse(inputs[4])
        };

        inputs = Console.ReadLine().Split(' ');
        opponent.nbCardInHand = int.Parse(inputs[0]);
        int opponentActions = int.Parse(inputs[1]);

        for (int i = 0; i < opponentActions; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int cardPlayedId = int.Parse(inputs[0]);
            opponent.lstActions.Add(new Action()
            {
                type = inputs[1],
                cardId1 = inputs.Length >= 3 ? int.Parse(inputs[2]) : 0,
                cardId2 = inputs.Length >= 4 ? int.Parse(inputs[3]) : 0,
            });
        }

        int cardCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < cardCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Card card = new Card()
            {
                number = int.Parse(inputs[0]),
                id = int.Parse(inputs[1]),
                location = int.Parse(inputs[2]),
                type = int.Parse(inputs[3]),
                cost = int.Parse(inputs[4]),
                attack = int.Parse(inputs[5]),
                defense = int.Parse(inputs[6]),
                abilities = inputs[7],
                myHealthChange = int.Parse(inputs[8]),
                opponentHealthChange = int.Parse(inputs[9]),
                cardDraw = int.Parse(inputs[10]),
                wasJustSummoned = false,
                hasAttacked = false
            };

            card.hasBreakthrough = card.abilities.Contains("B");
            card.hasCharge = card.abilities.Contains("C");
            card.hasDrain = card.abilities.Contains("D");
            card.isGuard = card.abilities.Contains("G");
            card.isLethal = card.abilities.Contains("L");
            card.hasWard = card.abilities.Contains("W");

            if (card.location == 0)
                me.lstCardsInHand.Add(card);
            else if (card.location == 1)
                me.lstCardsOnBoard.Add(card);
            else if (card.location == -1)
                opponent.lstCardsOnBoard.Add(card);
        }
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

    public override string ToString()
    {
        return $"health : {health} mana : {mana} nbCardInDeck : {nbCardInDeck} nbRunes : {nbRunes} draw : {draw} nbCardInHand : {nbCardInHand}\n";
    }
}

class Card
{
    public int number;
    public int id;
    public int location;
    public int type;
    public int cost;
    public int attack;
    public int defense;
    public string abilities;
    public int myHealthChange;
    public int opponentHealthChange;
    public int cardDraw;
    public bool hasBreakthrough;
    public bool hasCharge;
    public bool hasDrain;
    public bool isGuard;
    public bool isLethal;
    public bool hasWard;
    public bool wasJustSummoned;
    public bool hasAttacked;

    public override string ToString()
    {
        return $"id : {id} type : {type} loc : {location} cost : {cost} a : {attack} d : {defense} ab : {abilities}\n";
    }
}

class Action
{
    public string type;
    public int cardId1;
    public int cardId2;

    public override string ToString()
    {
        return $"{type} {cardId1} {cardId2}\n";
    }
}