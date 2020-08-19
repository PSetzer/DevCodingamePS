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
class Player
{
    /*static int[,] liens;
    static int L = 0;
    static int SI = 0;
    static int nbLiensMin = 1000;

    static void Main(string[] args)
    {
        StreamReader sr = new StreamReader(@"C:\Paul\DevCodinGame\Skynet_le_virus\Skynet_le_virus\Console_reseau_coeur.txt");
        string[] inputs;
        inputs = sr.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        liens = new int[L, 2];
        for (int i = 0; i < L; i++)
        {
            inputs = sr.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            liens[i, 0] = N1;
            liens[i, 1] = N2;
        }
        List<int> gates = new List<int>();
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(sr.ReadLine()); // the index of a gateway node
            gates.Add(EI);
        }

        // game loop
        while (true)
        {
            SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn

            int noeud1 = -1;
            int noeud2 = -1;

            int nbLiensMinGate = 1000;
            nbLiensMin = 1000;
            var nbGates = new Dictionary<int, int>();
            int nbGate = 0;
            List<int> noeudsPasses = new List<int>();
            int noeudLie = -1;
            int pos = -1;
            foreach (int gate in gates)
            {
                noeudsPasses.Add(gate);
                nbLiensMin = 1000;
                for (int i = 0; i < L; i++)
                {
                    noeudLie = -1;
                    
                    if (liens[i, 0] == gate)
                    {
                        noeudLie = liens[i, 1];
                    }
                    else if (liens[i, 1] == gate)
                    {
                        noeudLie = liens[i, 0];
                    }

                    if (noeudLie != -1)
                    {
                        if (nbGates.ContainsKey(noeudLie)) nbGates[noeudLie] += 1;
                        else nbGates.Add(noeudLie, 1);

                        if (noeudLie == SI)
                        {
                            nbLiensMin = 0;
                        }
                        else
                        {
                            NbLiensGateToSI(noeudLie, 0, noeudsPasses);
                        }

                        nbGates.TryGetValue(noeud2, out nbGate);
                        if (nbLiensMin < nbLiensMinGate || (nbLiensMin == nbLiensMinGate && nbGates[noeudLie] > nbGate))
                        {
                            noeud1 = gate;
                            noeud2 = noeudLie;
                            nbLiensMinGate = nbLiensMin;
                            pos = i;
                        }
                    }
                }
                noeudsPasses.Remove(gate);
            }

            if (pos != -1)
            {
                liens[pos, 0] = -1;
                liens[pos, 1] = -1;
            }

            Console.WriteLine(noeud1.ToString() + " " + noeud2.ToString()); // Example: 0 1 are the indices of the nodes you wish to sever the link between
        }
    }

    static void NbLiensGateToSI(int noeudLie, int nbLiens, List<int> noeudsPasses)
    {
        if (nbLiens >= 4) return;
        int noeudSuivant = -1;
        noeudsPasses.Add(noeudLie);
        for (int i = 0; i < L; i++)
        {
            noeudSuivant = -1;
            if (liens[i, 0] == noeudLie)
            {
                noeudSuivant = liens[i, 1];
            }
            else if (liens[i, 1] == noeudLie)
            {
                noeudSuivant = liens[i, 0];
            }

            if (noeudSuivant != -1)
            {
                if (!noeudsPasses.Contains(noeudSuivant))
                {
                    nbLiens++;
                    if (noeudSuivant == SI)
                    {
                        if (nbLiens < nbLiensMin)
                        {
                            nbLiensMin = nbLiens;
                        }
                        noeudsPasses.Remove(noeudLie);
                        return;
                    }
                    else
                    {
                        NbLiensGateToSI(noeudSuivant, nbLiens, noeudsPasses);
                        nbLiens--;
                    }
                }
            }
        }

        noeudsPasses.Remove(noeudLie);
        return;
    }*/
}