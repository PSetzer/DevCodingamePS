using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * The machines are gaining ground. Time to show them what we're really made of...
 **/
class Player
{
    static int width;
    static int height;
    static string[,] grid;
    static List<Point> points;
    static List<Lien> liens;
    static bool hasChanged;

    static void Main(string[] args)
    {
        List<string> lines = new List<string>();
        lines = File.ReadAllLines("~/../../../Sol Mul 1.txt").ToList();
        /*lines.Add(Console.ReadLine());
        lines.Add(Console.ReadLine());
        for (int i = 0; i < int.Parse(lines[1]); i++)
            lines.Add(Console.ReadLine());*/
        width = int.Parse(lines.First());
        Console.Error.WriteLine(width);
        lines.Remove(lines.First());
        height = int.Parse(lines.First());
        Console.Error.WriteLine(height);
        lines.Remove(lines.First());
        grid = new string[height, width];
        for (int i = 0; i < height; i++)
        {
            string row = lines[i];
            Console.Error.WriteLine(row);
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = row[j].ToString();
            }
        }

        points = new List<Point>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[i, j] != ".")
                {
                    Point point = new Point();
                    point.coord = new Coord(i, j);
                    point.poidsInit = int.Parse(grid[i, j].ToString());
                    point.nbLiens = 0;
                    point.coordVoisins = GetCoordVoisins(point.coord);
                    points.Add(point);
                }
            }
        }

        points.Reverse();

        liens = new List<Lien>();

        bool checkLowCorresp = false;
        bool checkDesperate = false;
        bool allonsy = true;
        while (allonsy)
        {
            hasChanged = false;

            var pointsValides = points.Where(p => p.poids > 0);
            foreach (Point point in pointsValides)
            {
                List<Point> voisins = point.coordVoisins.Select(cv => PointFromCoord(cv)).Where(v => v.poids > 0 && GetNbLiens(point, v) < 2 && !HasCrossLink(point, v)).ToList();

                double nbVoisins = 0;
                foreach (Point voisin in voisins)
                {
                    if (voisin.poids == 1 || GetNbLiens(point, voisin) == 1)
                        nbVoisins += 0.5;
                    else
                        nbVoisins += 1;
                }

                int nbLiens = 0;
                if (point.poids == nbVoisins * 2)
                    nbLiens = 2;
                else if (checkLowCorresp && point.poids == (nbVoisins * 2) - 1)
                    nbLiens = 1;
                else if (checkDesperate && point.poids == (nbVoisins * 2) - 2)
                {
                    nbLiens = 1;
                    checkDesperate = false;
                    checkLowCorresp = false;
                }

                if (nbLiens > 0)
                {
                    foreach (Point voisin in voisins)
                    {
                        if (!(nbLiens == 1 && point.poids == 2 && voisin.poids == 1))
                            SetNbLiens(point, voisin, nbLiens);
                    }
                }
            }

            if (points.Any(p => p.poids > 0))
            {
                if (!hasChanged)
                {
                    if (!checkLowCorresp)
                        checkLowCorresp = true;
                    else if (!checkDesperate)
                        checkDesperate = true;
                    else
                        // Perdu !
                        allonsy = false;                    
                }
                else
                    checkLowCorresp = false;
            }
            else
                allonsy = false;
        }

        //Console.WriteLine("0 0 2 0 1"); // Two coordinates and one integer: a node, one of its neighbors, the number of links connecting them.
        foreach (Lien lien in liens)
        {
            Console.WriteLine(lien.coord1.C + " " + lien.coord1.L + " " + lien.coord2.C + " " + lien.coord2.L + " " + lien.nbLiens);
        }
    }

    static Point PointFromCoord(Coord coord)
    {
        return points.Single(p => p.coord.Equals(coord));
    }

    static Lien GetLien(Point point1, Point point2)
    {
        return liens.SingleOrDefault(l => (l.coord1.Equals(point1.coord) && l.coord2.Equals(point2.coord)) || (l.coord1.Equals(point2.coord) && l.coord2.Equals(point1.coord)));
    }

    static int GetNbLiens(Point point1, Point point2)
    {
        return GetLien(point1, point2)?.nbLiens ?? 0;
    }

    static bool HasCrossLink(Point point1, Point point2)
    {
        bool hasCrossLink = false;

        if (point1.coord.C < point2.coord.C)
            hasCrossLink = liens.Where(l => (l.coord1.C > point1.coord.C && l.coord1.C < point2.coord.C) && ((l.coord1.L > point1.coord.L && l.coord2.L < point1.coord.L) || (l.coord2.L > point1.coord.L && l.coord1.L < point1.coord.L))).Count() > 0;
        if (point1.coord.C > point2.coord.C)
            hasCrossLink = liens.Where(l => (l.coord1.C > point2.coord.C && l.coord1.C < point1.coord.C) && ((l.coord1.L > point1.coord.L && l.coord2.L < point1.coord.L) || (l.coord2.L > point1.coord.L && l.coord1.L < point1.coord.L))).Count() > 0;
        if (point1.coord.L < point2.coord.L)
            hasCrossLink = liens.Where(l => (l.coord1.L > point1.coord.L && l.coord1.L < point2.coord.L) && ((l.coord1.C > point1.coord.C && l.coord2.C < point1.coord.C) || (l.coord2.C > point1.coord.C && l.coord1.C < point1.coord.C))).Count() > 0;
        if (point1.coord.L > point2.coord.L)
            hasCrossLink = liens.Where(l => (l.coord1.L > point2.coord.L && l.coord1.L < point1.coord.L) && ((l.coord1.C > point1.coord.C && l.coord2.C < point1.coord.C) || (l.coord2.C > point1.coord.C && l.coord1.C < point1.coord.C))).Count() > 0;

        return hasCrossLink;
    }

    static void SetNbLiens(Point point1, Point point2, int nbLiens)
    {
        if (nbLiens == 2 && (point1.poidsInit == 1 || point2.poidsInit == 1))
            nbLiens = 1;

        int nbLiensAdd = 0;
        Lien lien = GetLien(point1, point2);
        if (lien == null)
        {
            if (point1.poids == 1 || point2.poids == 1)
                nbLiens = 1;
            liens.Add(new Lien(point1.coord, point2.coord, nbLiens));
            nbLiensAdd = nbLiens;
        }
        else
        {
            lien.nbLiens = nbLiens;
            nbLiensAdd = nbLiens - 1;
        }

        hasChanged = nbLiensAdd > 0;

        point1.nbLiens += nbLiensAdd;
        point2.nbLiens += nbLiensAdd;
    }

    static List<Coord> GetCoordVoisins(Coord coord)
    {
        List<Coord> voisins = new List<Coord>();

        // Voisin de gauche
        for (int i = coord.C - 1; i >= 0; i--)
            if (grid[coord.L, i] != ".")
            {
                voisins.Add(new Coord(coord.L, i));
                break;
            }

        // Voisin de droite
        for (int i = coord.C + 1; i < width; i++)
            if (grid[coord.L, i] != ".")
            {
                voisins.Add(new Coord(coord.L, i));
                break;
            }

        // Voisin du haut
        for (int i = coord.L - 1; i >= 0; i--)
            if (grid[i, coord.C] != ".")
            {
                voisins.Add(new Coord(i, coord.C));
                break;
            }

        // Voisin du bas
        for (int i = coord.L + 1; i < height; i++)
            if (grid[i, coord.C] != ".")
            {
                voisins.Add(new Coord(i, coord.C));
                break;
            }

        return voisins;
    }
}

class Coord : IEquatable<Coord>
{
    public int L;
    public int C;

    public Coord(int L, int C)
    {
        this.L = L;
        this.C = C;
    }

    public bool Equals(Coord other)
    {
        return other != null ? this.L == other.L && this.C == other.C : false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Coord);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "C" + C.ToString() + " L" + L.ToString();
    }
}

class Point : IEquatable<Point>
{
    public Coord coord;
    public int poidsInit;
    public int nbLiens;
    public List<Coord> coordVoisins;

    public int poids { get { return poidsInit - nbLiens; } }

    public bool Equals(Point other)
    {
        return other != null ? this.coord.Equals(other.coord) : false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Point);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "C" + coord.C.ToString() + " L" + coord.L.ToString() + " P" + poids.ToString();
    }
}

class Lien : IEquatable<Lien>
{
    public Coord coord1;
    public Coord coord2;
    public int nbLiens;

    public Lien(Coord coord1, Coord coord2, int nbLiens)
    {
        this.coord1 = coord1;
        this.coord2 = coord2;
        this.nbLiens = nbLiens;
    }

    public bool Equals(Lien other)
    {
        return other != null ? (this.coord1.Equals(other.coord1) && this.coord2.Equals(other.coord2)) || (this.coord1.Equals(other.coord2) && this.coord2.Equals(other.coord1)) : false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Lien);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "C" + coord1.C.ToString() + " L" + coord1.L.ToString() + " C" + coord2.C.ToString() + " L" + coord2.L.ToString() + " N" + nbLiens.ToString();
    }
}
