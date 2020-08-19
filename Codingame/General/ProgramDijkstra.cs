using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


class ProgramDijkstra
{
    static StreamReader file;

    static void MainDijkstra(string[] args)
    {
        file = new System.IO.StreamReader("../../input8.txt");

        int nbLiens = int.Parse(file.ReadLine());
        List<Lien> lstLiens = new List<Lien>();

        string[] inputs;
        for (int i = 0; i < nbLiens; i++)
        {
            inputs = file.ReadLine().Split(' ');
            lstLiens.Add(new Lien(inputs[0], inputs[1], int.Parse(inputs[2])));
        }

        List<Noeud> lstNoeuds = lstLiens.SelectMany(x => new List<Noeud> { x.noeud1, x.noeud2 }).Distinct(new ComparerNoeuds()).ToList();

        Noeud noeudOrigine = new Noeud();
        Noeud noeudDestination = new Noeud();
        noeudOrigine = lstNoeuds.Single(x => x.valeur == "");
        noeudDestination = lstNoeuds.Single(x => x.valeur == "");
        (int distanceMin, List<Noeud> cheminMin) = Dijkstra(lstNoeuds, lstLiens, noeudOrigine, noeudDestination, true) ;



        Thread.Sleep(2000);
    }

    static (int, List<Noeud>) Dijkstra(List<Noeud> lstNoeuds, List<Lien> lstLiens, Noeud noeudOrigine, Noeud noeudDestination, bool isGrapheOriente = false)
    {
        int distanceMin = -1;
        List<Noeud> cheminMin = new List<Noeud>();

        List<ObjetDijkstra> tabDijkstra = new List<ObjetDijkstra>();
        lstNoeuds.ForEach(x => tabDijkstra.Add(new ObjetDijkstra() { noeud = x }));

        ObjetDijkstra noeudPereDijkstra = tabDijkstra.Single(x => x.noeud == noeudOrigine);
        noeudPereDijkstra.poids = 0;
        noeudPereDijkstra.isDone = true;

        while (distanceMin == -1)
        {
            Noeud noeudPere = noeudPereDijkstra.noeud;
            cheminMin.Add(noeudPere);
                
            // Liste des noeuds du graphe liés au noeud père
            List<Lien> lstLiensNoeudsFilsGraphe = lstLiens.Where(x => x.noeud1 == noeudPere)
                                                            .Where(x => tabDijkstra.Any(y => y.noeud == x.noeud2 && !y.isDone)).ToList();
            if (!isGrapheOriente)
            {
                lstLiensNoeudsFilsGraphe.AddRange(lstLiens.Where(x => x.noeud2 == noeudPere)
                                                            .Where(x => tabDijkstra.Any(y => y.noeud == x.noeud1 && !y.isDone)).ToList());
            }

            foreach (Lien lienNoeudFilsGraphe in lstLiensNoeudsFilsGraphe)
            {
                Noeud noeudFilsGraphe = lienNoeudFilsGraphe.noeud1 == noeudPere ? lienNoeudFilsGraphe.noeud2 : lienNoeudFilsGraphe.noeud1;
                ObjetDijkstra noeudFilsDijkstra = tabDijkstra.Single(x => x.noeud == noeudFilsGraphe);
                // Mise à jour du poids du noeud fils si celui-ci est supérieur au poids du noeud père + le poids du lien père-fils
                if (noeudFilsDijkstra.poids > noeudPereDijkstra.poids + lienNoeudFilsGraphe.poids)
                    noeudFilsDijkstra.poids = noeudPereDijkstra.poids + lienNoeudFilsGraphe.poids;
            }

            // Poids min des noeuds pas encore parcourus
            int poidsNoeudMin = tabDijkstra.Where(x => !x.isDone).Min(x => x.poids);

            if (tabDijkstra.Single(x => x.noeud == noeudDestination).poids == poidsNoeudMin)
            {
                // Si le poids de la cible est égal au poids min des noeuds pas encore parcourus, ce poids est celui du chemin le plus court
                distanceMin = poidsNoeudMin;
                cheminMin.Add(noeudDestination);
            }
            else
            {
                // Sinon, on prend un des noeuds ayant le poids min comme noeud père pour l'itération suivante
                noeudPereDijkstra = tabDijkstra.First(x => !x.isDone && x.poids == poidsNoeudMin);
                // Le noeud père est marqué comme parcouru
                noeudPereDijkstra.isDone = true;
            }
        }

        return (distanceMin, cheminMin);
    }

    public class Noeud : IEquatable<Noeud>
    {
        public string valeur;

        public override string ToString()
        {
            return $"valeur : {valeur}";
        }

        public bool Equals(Noeud other)
        {
            return this.valeur == other.valeur;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Noeud);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Noeud obj1, Noeud obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
                return false;

            return obj1.Equals(obj2);
        }

        public static bool operator !=(Noeud obj1, Noeud obj2)
        {
            return !(obj1 == obj2);
        }
    }

    public class Lien : IEquatable<Lien>
    {
        public Noeud noeud1;
        public Noeud noeud2;
        public int poids = 1;

        public Lien(string valeur1, string valeur2, int poids = 1)
        {
            noeud1 = new Noeud() { valeur = valeur1 };
            noeud2 = new Noeud() { valeur = valeur2 };
            this.poids = poids;
        }

        public Lien() { }

        public override string ToString()
        {
            return $"noeud1 : {noeud1.valeur}, noeud2 : {noeud2.valeur}, poids : {poids}";
        }

        public bool Equals(Lien other)
        {
            return this.noeud1 == other.noeud1 && this.noeud2 == other.noeud2 && this.poids == other.poids;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Lien);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Lien obj1, Lien obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
                return false;

            return obj1.Equals(obj2);
        }

        public static bool operator !=(Lien obj1, Lien obj2)
        {
            return !(obj1 == obj2);
        }
    }

    public class ObjetDijkstra : IEquatable<ObjetDijkstra>
    {
        public Noeud noeud;
        public int poids = int.MaxValue;
        public bool isDone = false;

        public override string ToString()
        {
            return $"noeud : {noeud.valeur}, poids : {poids}, isDone : {isDone}";
        }

        public bool Equals(ObjetDijkstra other)
        {
            return this.noeud == other.noeud && this.poids == other.poids && this.isDone == other.isDone;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ObjetDijkstra);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ObjetDijkstra obj1, ObjetDijkstra obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
                return false;

            return obj1.Equals(obj2);
        }

        public static bool operator !=(ObjetDijkstra obj1, ObjetDijkstra obj2)
        {
            return !(obj1 == obj2);
        }
    }

    public class ComparerNoeuds : IEqualityComparer<Noeud>
    {
        public bool Equals(Noeud x, Noeud y)
        {
            return x.valeur == y.valeur;
        }

        public int GetHashCode(Noeud obj)
        {
            return base.GetHashCode();
        }
    }

    public class ComparerLien : IEqualityComparer<Lien>
    {
        public bool Equals(Lien x, Lien y)
        {
            return x.noeud1.Equals(y.noeud1) && x.noeud2.Equals(y.noeud2) && x.poids == y.poids;
        }

        public int GetHashCode(Lien obj)
        {
            return base.GetHashCode();
        }
    }

    public class ComparerObjetDijkstra : IEqualityComparer<ObjetDijkstra>
    {
        public bool Equals(ObjetDijkstra x, ObjetDijkstra y)
        {
            return x.noeud.Equals(y.noeud) && x.poids == y.poids && x.isDone == y.isDone;
        }

        public int GetHashCode(ObjetDijkstra obj)
        {
            return base.GetHashCode();
        }
    }
}

