using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;


/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player_2017
{
    static List<int[]> liens;
    static List<int> gates;
    static List<int> noeudsLies;
    static List<int> noeuds;

    static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines("~/../../../Console_reseau_coeur.txt");
        string[] inputs = lines[0].Split(' ');
        //string[] inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways

        // Tableau des liens entre les noeuds
        liens = new List<int[]>();
        for (int i = 1; i <= L; i++)
        {
            inputs = lines[i].Split(' ');
            //inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            liens.Add(new int[] { N1, N2 });
        }

        // Liste des portes
        gates = new List<int>();
        for (int i = L + 1; i <= L + E; i++)
        {
            int EI = int.Parse(lines[i]); // the index of a gateway node
            //int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            gates.Add(EI);
        }

        // A partir des liens, on construit un dictionaire avec comme clés les noeuds liés à une ou plusieurs portes et comme valeurs la liste des portes correspondantes
        Dictionary<int, List<int>> noeudsLiesGates = liens.Where(l => gates.Contains(l[0]) || gates.Contains(l[1]))
                                                          .GroupBy(n => gates.Contains(n[0]) ? n[1] : n[0])
                                                          .ToDictionary(g => g.Key, g => g.SelectMany(l => l.Where(n => n != g.Key)).ToList());

        // Liste des noeuds liés à une ou plusieurs portes
        noeudsLies = noeudsLiesGates.Keys.ToList();

        // Liste des noeuds qui ne sont pas des portes
        noeuds = liens.SelectMany(l => l)
                      .GroupBy(n => n)
                      .Select(n => n.Key)
                      .Where(n => !gates.Contains(n))
                      .ToList();

        // Dictionnaire avec comme clés les noeuds liés à deux portes et comme valeurs les chemins les plus courts de l'agent Skynet à ces noeuds
        Dictionary<int, int> distDoubleGate = noeudsLiesGates.Where(n => n.Value.Count() == 2)
                                                             .ToDictionary(n => n.Key, _ => 0);

        // game loop
        while (true)
        {
            int noeud1 = -1;
            int noeud2 = -1;

            int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn

            if (noeudsLiesGates.Keys.Contains(SI))
            {
                // L'agent Skynet est sur un noeud lié à une porte => on coupe le lien vers la porte
                noeud1 = SI;
                noeud2 = noeudsLiesGates[noeud1].First();
                // Le noeud est supprimé du dictionaire des noeuds liés
                noeudsLiesGates.Remove(noeud1);
            }
            else
            {
                if (distDoubleGate.Count > 0)
                {
                    // On cherche pour chaque noeud lié à deux portes le minimum de noeuds non lié à une porte qui le relient à l'agent Skynet
                    List<int> doubleGates = distDoubleGate.Keys.ToList();
                    foreach (int doubleGate in doubleGates)
                        distDoubleGate[doubleGate] = GetDistDoubleGate(SI, doubleGate);

                    // On coupe un des liens du noeud lié à deux portes dont le chemin jusqu'à l'agent Skynet comporte le moins de noeuds non liés à une porte
                    noeud1 = distDoubleGate.OrderBy(dg => dg.Value).First().Key;
                    noeud2 = noeudsLiesGates[noeud1].First();

                    // Le noeud n'est plus lié qu'à une seule porte, on le supprime de la liste des noeuds liés à deux portes
                    distDoubleGate.Remove(noeud1);
                    // On supprime la porte dans le dictionnaire des noeuds liés
                    noeudsLiesGates[noeud1].Remove(noeud2);
                }
                else
                {
                    // Il n'y a pas/plus de noeuds lié à deux portes => on coupe un lien quelconque entre un noeud lié et une porte
                    noeud1 = noeudsLiesGates.Keys.First();
                    noeud2 = noeudsLiesGates[noeud1].First();
                    // Le noeud est supprimé du dictionaire des noeuds liés
                    noeudsLiesGates.Remove(noeud1);
                }
            }

            // On supprime le lien coupé de la liste des liens
            liens.Remove(liens.Single(l => (l[0] == noeud1 && l[1] == noeud2) || (l[0] == noeud2 && l[1] == noeud1)));

            Console.WriteLine(noeud1.ToString() + " " + noeud2.ToString()); // Example: 0 1 are the indices of the nodes you wish to sever the link between
        }
    }

    /// <summary>
    /// Recherche par l'algorithme de Dijkstra du chemin comportant le moins de noeuds non liés à une porte entre l'agent Skynet et un noeud lié à deux portes
    /// </summary>
    /// <param name="SI"></param>
    /// <param name="doubleGate"></param>
    /// <returns></returns>
    static int GetDistDoubleGate(int SI, int doubleGate)
    {
        int dist = -1;

        // Construction du tableau mettant en relation les noeuds, leur poids calculé (initialisé à 1000) et si ils ont déjà été parcourus
        List<int[]> noeudsPoidsPassed = new List<int[]>();
        foreach (int noeud in noeuds)
            noeudsPoidsPassed.Add(new int[3] { noeud, 1000, 0 });

        // Initialisation de la recherche avec le premier noeud père = SI
        int[] noeudPoidsPere = noeudsPoidsPassed.Single(t => t[0] == SI);
        int noeudPere = SI;
        noeudsPoidsPassed.Single(t => t[0] == SI)[1] = 0; // Le poids du premier noeud père est mis à 0
        noeudsPoidsPassed.Single(t => t[0] == SI)[2] = 1;

        while (dist == -1)
        {
            // Les noeuds fils sont determinés à partir des liens, les portes et les noeuds déjà parcourus sont filtrés ici.
            var noeudsFils = liens.Where(l => l[0] == noeudPere || l[1] == noeudPere)
                                  .Select(l => l[0] == noeudPere ? l[1] : l[0])
                                  .Where(n => noeudsPoidsPassed.Any(t => t[0] == n && t[2] == 0));

            foreach (int noeudFils in noeudsFils)
            {                
                int[] noeudPoidsFils = noeudsPoidsPassed.Single(t => t[0] == noeudFils);
                // Le poids du lien père-fils est mis à 0 si le noeud fils est lié à une porte, à 1 sinon
                int poidsLien = noeudsLies.Contains(noeudFils) ? 0 : 1;
                // Selon Dijkstra, on met à jour le poids du noeud fils si celui-ci est supérieur au poids du noeud père + le poids du lien père-fils
                if (noeudPoidsPere[1] + poidsLien < noeudPoidsFils[1])
                    noeudsPoidsPassed.Single(t => t[0] == noeudFils)[1] = noeudPoidsPere[1] + poidsLien;
            }

            // On determine le poids min des noeuds pas encore parcourus
            int poidsMin = noeudsPoidsPassed.Where(t => t[2] == 0)
                                            .OrderBy(t => t[1]).First()[1];
            if (noeudsPoidsPassed.Single(t => t[0] == doubleGate)[1] == poidsMin)
                // Si le poids de la cible est égal au poids min des noeuds pas encore parcourus, ce poids est celui du chemin le plus court
                dist = poidsMin;            
            else
            {
                // Sinon, selon Dijkstra on prend un des noeuds ayant le poids min comme noeud père pour l'itération suivante
                noeudPoidsPere = noeudsPoidsPassed.Where(t => t[2] == 0 && t[1] == poidsMin).First();
                noeudPere = noeudPoidsPere[0];
                // Le noeud père est marqué comme parcouru
                noeudsPoidsPassed.Single(t => t[0] == noeudPere)[2] = 1;
            }
        }

        return dist;
    }
}

