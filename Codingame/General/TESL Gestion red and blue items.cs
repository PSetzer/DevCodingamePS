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
    static Dictionary<string, List<Card>> game = new Dictionary<string, List<Card>>();

    static void Main(string[] args)
    {
        // game loop
        while (true)
        {
            GetInput();

            lstActions = new List<string>();

            if (me.mana == 0)
            {
                PickCard();
            }
            else
            {
                // phase d'invocation
                bool canInvoke = true;
                while (canInvoke)
                {
                    BoardAnalysis();
					
					List<Card> lstSummonable = me.lstCardsInHand.Where(x => x.type == 0 && x.cost <= me.mana).ToList();

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
                        canInvoke = false;
                }

                bool canUse = true;
                while (canUse)
                {
                    canUse = false;

                    List<Card> lstGreenItemUsable = me.lstCardsInHand.Where(x => x.type == 1 && x.cost <= me.mana).ToList();

                    Card toUseOnMyCreature = lstGreenItemUsable.FirstOrDefault();

                    if (toUseOnMyCreature != null)
                    {
                        Card creatureToBuff = me.lstCardsOnBoard.FirstOrDefault(x => x.isGuard) ?? me.lstCardsOnBoard.FirstOrDefault(); // utiliser un filtre Lethal, Drain, etc
                        if (creatureToBuff != null)
                        {
                            lstActions.Add($"USE {toUseOnMyCreature.id} {creatureToBuff.id}");
                            PlayCardOnBoard(toUseOnMyCreature);
                            canUse = true;
                        }
                    }
                /*}

                bool canUseRedBlue = true;
                while (canUseRedBlue)
                {*/
                    List<Card> lstRedBlueItemUsable = me.lstCardsInHand.Where(x => x.type > 1 && x.cost <= me.mana).ToList();

                    if (lstRedBlueItemUsable.Count > 0)
                    {
                        Card toUseOnOppCreature = null;
                        Card creatureToDebuff = null;
                        (toUseOnOppCreature, creatureToDebuff) = ApplySpecialRedBlueItemRules(lstRedBlueItemUsable);

                        if (toUseOnOppCreature != null && creatureToDebuff != null)
                        {
                            lstActions.Add($"USE {toUseOnOppCreature.id} {creatureToDebuff.id}");
                            PlayCardOnBoard(toUseOnOppCreature);
                            canUse = true;
                        }
                        else if (toUseOnOppCreature != null)
                        {
                            lstActions.Add($"USE {toUseOnOppCreature.id} -1");
                            PlayCardOnBoard(toUseOnOppCreature);
                            canUse = true;
                        }
                    }
                }

                // phase d'attaque
                while (me.lstCardsOnBoard.Any(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0))
                {
                    List<Card> lstReadyCards = me.lstCardsOnBoard.Where(x => !x.hasAttacked && !(x.wasJustSummoned && !x.hasCharge) && x.attack > 0).ToList();
                    BoardAnalysis();

                    Card defender = null;
                    Card attacking = null;
                    Card readyLethal = lstReadyCards.FirstOrDefault(x => x.isLethal);
                    int maxOppDefense = opponent.lstCardsOnBoard.Count > 0 ? opponent.lstCardsOnBoard.Max(x => x.defense) : 0;

                    if (defender == null)
                        defender = GetDic("opponentGuard").FirstOrDefault(x => x.defense == maxOppDefense && readyLethal != null);
                    if (defender == null)
                        defender = GetDic("opponentGuard").FirstOrDefault();
                    if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.number == 14);
                    if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.number == 20);
                    if (defender == null && maxOppDefense >= 4)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.defense == maxOppDefense && readyLethal != null);
                    if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.number == 16);
                    if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.number == 42);
                    if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.number == 35); 
                    if (defender == null && lstReadyCards.Any(y => y.defense >= 5))
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.isLethal);
                    /*if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => x.attack >= 5 && lstReadyCards.Any(y => x.defense <= y.attack));*/
                    /*if (defender == null)
                        defender = opponent.lstCardsOnBoard.FirstOrDefault(x => !x.isLethal && !x.hasWard && lstReadyCards.Any(y => x.defense <= y.attack && x.attack < y.defense));*/

                    
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
                            AttackOpponentCard(attacking, defender);
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

                /*bool canUseRedBlue = true;
                while (canUseRedBlue)
                {
                    canUseRedBlue = false;

                    List<Card> lstRedBlueItemUsable = me.lstCardsInHand.Where(x => x.type > 1 && x.cost <= me.mana).ToList();

                    if (lstRedBlueItemUsable.Count > 0)
                    {
                        Card toUseOnOppCreature = null;
                        Card creatureToDebuff = null;
                        (toUseOnOppCreature, creatureToDebuff) = ApplySpecialRedBlueItemRules(lstRedBlueItemUsable);

                        if (toUseOnOppCreature != null && creatureToDebuff != null)
                        {
                            lstActions.Add($"USE {toUseOnOppCreature.id} {creatureToDebuff.id}");
                            PlayCardOnBoard(toUseOnOppCreature);
                            canUseRedBlue = true;
                        }
                        else if (toUseOnOppCreature != null)
                        {
                            lstActions.Add($"USE {toUseOnOppCreature.id} -1");
                            PlayCardOnBoard(toUseOnOppCreature);
                            canUseRedBlue = true;
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

    static void BoardAnalysis()
    {
        game["opponentGuard"] = opponent.lstCardsOnBoard.Where(x => x.isGuard).ToList();
        game["opponentLethal"] = opponent.lstCardsOnBoard.Where(x => x.isLethal).ToList();
        game["opponentDrain"] = opponent.lstCardsOnBoard.Where(x => x.hasDrain).ToList();
        game["opponentWard"] = opponent.lstCardsOnBoard.Where(x => x.hasWard).ToList();
        game["opponentHighDef"] = opponent.lstCardsOnBoard.Where(x => x.defense >= 4).ToList();

        game["opponentOneDef"] = new List<Card>();
        List <Card> lstOpponentOneDef = opponent.lstCardsOnBoard.Where(x => x.defense == 1).ToList();
        if (lstOpponentOneDef.Count > 0)
            game["opponentOneDef"] = lstOpponentOneDef.Any(x => x.isLethal) ? lstOpponentOneDef.Where(x => x.isLethal).ToList() : lstOpponentOneDef.Where(x => x.attack == lstOpponentOneDef.Max(y => y.attack)).ToList();

        game["opponentTwoDef"] = new List<Card>();
        List<Card> lstOpponentTwoDef = opponent.lstCardsOnBoard.Where(x => x.defense == 2).ToList();
        if (lstOpponentTwoDef.Count > 0)
            game["opponentTwoDef"] = lstOpponentTwoDef.Any(x => x.isLethal) ? lstOpponentTwoDef.Where(x => x.isLethal).ToList() : lstOpponentTwoDef.Where(x => x.attack == lstOpponentTwoDef.Max(y => y.attack)).ToList();

        game["opponentThreeDef"] = new List<Card>();
        List<Card> lstOpponentThreeDef = opponent.lstCardsOnBoard.Where(x => x.defense == 3).ToList();
        if (lstOpponentThreeDef.Count > 0)
            game["opponentThreeDef"] = lstOpponentThreeDef.Any(x => x.isLethal) ? lstOpponentThreeDef.Where(x => x.isLethal).ToList() : lstOpponentThreeDef.Where(x => x.attack == lstOpponentThreeDef.Max(y => y.attack)).ToList();

        game["opponentFourDef"] = new List<Card>();
        List<Card> lstOpponentFourDef = opponent.lstCardsOnBoard.Where(x => x.defense == 4).ToList();
        if (lstOpponentFourDef.Count > 0)
            game["opponentFourDef"] = lstOpponentFourDef.Any(x => x.isLethal) ? lstOpponentFourDef.Where(x => x.isLethal).ToList() : lstOpponentFourDef.Where(x => x.attack == lstOpponentFourDef.Max(y => y.attack)).ToList();

        game["opponentFourAb"] = opponent.lstCardsOnBoard.Where(x => x.isGuard && x.isLethal && x.hasDrain && x.hasWard).ToList();
        game["opponentThreeAb"] = opponent.lstCardsOnBoard.Where(x =>
        (x.isGuard && x.isLethal && x.hasDrain) ||
        (x.isGuard && x.isLethal && x.hasWard) ||
        (x.isGuard && x.hasDrain && x.hasWard) ||
        (x.isLethal && x.hasDrain && x.hasWard)).ToList();
        game["opponentTwoAb"] = opponent.lstCardsOnBoard.Where(x =>
        (x.isGuard && x.isLethal) ||
        (x.isGuard && x.hasDrain) ||
        (x.isGuard && x.hasWard) ||
        (x.isLethal && x.hasDrain) ||
        (x.isLethal && x.hasWard) ||
        (x.hasDrain && x.hasWard)).ToList();
    }
    
    static (Card, Card) ApplySpecialRedBlueItemRules(List<Card> lstRedBlueItems)
    {
        Card oneShot = lstRedBlueItems.FirstOrDefault(x => x.type == 3 && -(x.defense + x.opponentHealthChange) >= opponent.health);

        if (oneShot != null)
            return (oneShot, null);
        
        if (GetDic("opponentFourAb").Count > 0 && lstRedBlueItems.Any(x => x.number == 142 || x.number == 148 || x.number == 149 || x.number == 151))
        {
            GetDic("opponentFourAb").First().SuppressAbilities(); 
            return (lstRedBlueItems.FirstOrDefault(x => x.number == 142) ?? lstRedBlueItems.FirstOrDefault(x => x.number == 148)
                ?? lstRedBlueItems.FirstOrDefault(x => x.number == 149) ?? lstRedBlueItems.FirstOrDefault(x => x.number == 151), GetDic("opponentFourAb").First());
        }

        if (GetDic("opponentThreeAb").Count > 0 && lstRedBlueItems.Any(x => x.number == 142 || x.number == 148 || x.number == 149 || x.number == 151))
        {
            GetDic("opponentThreeAb").First().SuppressAbilities();
            return (lstRedBlueItems.FirstOrDefault(x => x.number == 142) ?? lstRedBlueItems.FirstOrDefault(x => x.number == 148)
                ?? lstRedBlueItems.FirstOrDefault(x => x.number == 149) ?? lstRedBlueItems.FirstOrDefault(x => x.number == 151), GetDic("opponentThreeAb").First());
        }

        if (GetDic("opponentTwoAb").Count > 0 && lstRedBlueItems.Any(x => x.number == 142 || x.number == 148 || x.number == 149 || x.number == 151))
        {
            GetDic("opponentTwoAb").First().SuppressAbilities();
            return (lstRedBlueItems.FirstOrDefault(x => x.number == 142) ?? lstRedBlueItems.FirstOrDefault(x => x.number == 148)
                ?? lstRedBlueItems.FirstOrDefault(x => x.number == 149) ?? lstRedBlueItems.FirstOrDefault(x => x.number == 151), GetDic("opponentTwoAb").First());
        }

        if (GetDic("opponentGuard").Count > 0 && lstRedBlueItems.Any(x => x.number == 143))
        {
            GetDic("opponentGuard").First().isGuard = false;
            return (lstRedBlueItems.FirstOrDefault(x => x.number == 143), GetDic("opponentGuard").First());
        }

        if (GetDic("opponentHighDef").Count > 0 && lstRedBlueItems.Any(x => x.number == 152))
            return (lstRedBlueItems.FirstOrDefault(x => x.number == 152), GetDic("opponentHighDef").First());

        if (GetDic("opponentFourDef").Count > 0 && lstRedBlueItems.Any(x => x.defense == -4))
            return (lstRedBlueItems.FirstOrDefault(x => x.defense == -4), GetDic("opponentFourDef").First());
        
        if (GetDic("opponentThreeDef").Count > 0 && lstRedBlueItems.Any(x => x.defense == -3 || x.defense == -4))
            return (lstRedBlueItems.FirstOrDefault(x => x.defense == -3) ?? lstRedBlueItems.FirstOrDefault(x => x.defense == -4), GetDic("opponentThreeDef").First());

        if (GetDic("opponentTwoDef").Count > 0 && lstRedBlueItems.Any(x => x.defense == -2 || x.defense == -3))
            return (lstRedBlueItems.FirstOrDefault(x => x.defense == -2) ?? lstRedBlueItems.FirstOrDefault(x => x.defense == -3), GetDic("opponentTwoDef").First());

        if (GetDic("opponentOneDef").Count > 0 && lstRedBlueItems.Any(x => x.defense == -1 || x.defense == -2))
            return (lstRedBlueItems.FirstOrDefault(x => x.defense == -1) ?? lstRedBlueItems.FirstOrDefault(x => x.defense == -2), GetDic("opponentOneDef").First());

        if (lstRedBlueItems.Any(x => x.type == 3 && x.defense == 0))
            return (lstRedBlueItems.FirstOrDefault(x => x.defense == 0), null);

        return (null, null);
    }

    static List<Card> GetDic(string key)
    {
        List<Card> ret;
        game.TryGetValue(key, out ret);
        return ret ?? new List<Card>();
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

    public void SuppressAbilities()
    {
        abilities = "------";
        hasBreakthrough = false;
        hasCharge = false;
        hasDrain = false;
        isGuard = false;
        isLethal = false;
        hasWard = false;
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