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
    static List<Lien> liens;

    static void Main(string[] args)
    {
        List<string> lines = new List<string>();
        //lines = File.ReadAllLines("~/../../../Assez Simple.txt").ToList();
        lines.Add(Console.ReadLine());
        lines.Add(Console.ReadLine());
        for (int i = 0; i < int.Parse(lines[1]); i++) lines.Add(Console.ReadLine());
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

        Dictionary<Point, List<Point>> pointVoisins = new Dictionary<Point, List<Point>>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[i, j] != ".")
                {
                    Point point = new Point(i, j);
                    pointVoisins.Add(point, GetCoordVoisins(point));
                }
            }
        }

        liens = new List<Lien>();

        bool allonsy = true;
        while (allonsy)
        {
            bool hasChanged = false;
            List<Point> points = pointVoisins.Keys.Where(p => GetPoids(p) > 0).ToList();
            foreach (Point point in points)
            {
                int poids = GetPoids(point);
                List<Point> voisins = pointVoisins.Single(pv => pv.Key.Equals(point)).Value;
                voisins = voisins.Where(v => GetPoids(v) > 0 && GetNbLiens(point, v) < 2).ToList();

                foreach (Point voisin in voisins)
                {
                    var voisinsDeVoisin = pointVoisins.Single(pv => pv.Key.Equals(voisin)).Value;
                    if (voisinsDeVoisin.Where(v => GetPoids(v) > 0 && GetNbLiens(point, v) < 2).Count() == 1)
                    {
                        AugmenterLien(point, voisin, 1);
                        hasChanged = true;
                    }
                }

                double nbVoisins = 0;
                foreach (Point voisin in voisins)
                {
                    if (GetPoids(voisin) == 1 || GetNbLiens(point, voisin) == 1)
                        nbVoisins += 0.5;
                    else
                        nbVoisins += 1;
                }

                if (poids == nbVoisins * 2)
                {
                    foreach (Point voisin in voisins)
                    {
                        int nbLiensMax = poids == 1 || GetPoids(voisin) == 1 ? 1 : 2;
                        AugmenterLien(point, voisin, nbLiensMax);
                    }
                    hasChanged = true;
                }
                else if (poids == (nbVoisins * 2) - 1)
                {
                    foreach (Point voisin in voisins)
                    {
                        Lien lien = GetLien(point, voisin);
                        if (lien == null)
                        {
                            liens.Add(new Lien(point, voisin, 1));
                            BaisserPoids(point, 1);
                            BaisserPoids(voisin, 1);
                            hasChanged = true;
                        }
                    }
                }

                //Calculer les nouveaux poids ici en fonction des liens ?
            }

            allonsy = pointVoisins.Keys.Any(c => GetPoids(c) > 0) && hasChanged;
        }

        //Console.WriteLine("0 0 2 0 1"); // Two coordinates and one integer: a node, one of its neighbors, the number of links connecting them.
        foreach (Lien lien in liens)
        {
            Console.WriteLine(lien.point1.C + " " + lien.point1.L + " " + lien.point2.C + " " + lien.point2.L + " " + lien.nbLiens);
        }
    }    

    static int GetPoids(Point coord)
    {
        return int.Parse(grid[coord.L, coord.C].ToString());
    }

    static void BaisserPoids(Point coord, int poids)
    {
        int newPoids = GetPoids(coord) - poids;
        grid[coord.L, coord.C] = newPoids.ToString();
        return;
    }

    static Lien GetLien(Point point1, Point point2)
    {
        return liens.SingleOrDefault(l => (l.point1.Equals(point1) && l.point2.Equals(point2)) || (l.point1.Equals(point2) && l.point2.Equals(point1)));
    }

    static int GetNbLiens(Point point1, Point point2)
    {
        return GetLien(point1, point2)?.nbLiens ?? 0;
    }

    static void CreerLien(Point point1, Point point2, int nbLiens)
    {
        Lien lien = GetLien(point1, point2);
        int diffNbLiens = 0;
        if (lien == null)
        {
            liens.Add(new Lien(point1, point2, nbLiens));
            diffNbLiens = nbLiens;
        }
        else
        {
            diffNbLiens = lien.nbLiens;
            lien.nbLiens = nbLiens;
            diffNbLiens = lien.nbLiens - diffNbLiens;
        }

        BaisserPoids(point1, diffNbLiens);
        BaisserPoids(point2, diffNbLiens);
    }

    static void AugmenterLien(Point point1, Point point2, int nbLiens)
    {
        Lien lien = GetLien(point1, point2);
        if (lien == null)
            liens.Add(new Lien(point1, point2, nbLiens));
        else
            lien.nbLiens += nbLiens;

        BaisserPoids(point1, nbLiens);
        BaisserPoids(point2, nbLiens);
    }

    static List<Point> GetCoordVoisins(Point point)
    {
        List<Point> voisins = new List<Point>();

        // Voisin de gauche
        for (int i = point.C - 1; i >= 0; i--)
            if (grid[point.L, i] != ".")
            {
                voisins.Add(new Point(point.L, i));
                break;
            }

        // Voisin de droite
        for (int i = point.C + 1; i < width; i++)
            if (grid[point.L, i] != ".")
            {
                voisins.Add(new Point(point.L, i));
                break;
            }

        // Voisin du haut
        for (int i = point.L - 1; i >= 0; i--)
            if (grid[i, point.C] != ".")
            {
                voisins.Add(new Point(i, point.C));
                break;
            }

        // Voisin du bas
        for (int i = point.L + 1; i < height; i++)
            if (grid[i, point.C] != ".")
            {
                voisins.Add(new Point(i, point.C));
                break;
            }

        return voisins;
    }
}

class Point : IEquatable<Point>
{
    public int L;
    public int C;

    public Point(int L, int C)
    {
        this.L = L;
        this.C = C;
    }

    public Point()
    {

    }

    public bool Equals(Point other)
    {
        return other != null ? this.L == other.L && this.C == other.C : false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Point);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

class Lien : IEquatable<Lien>
{
    public Point point1;
    public Point point2;
    public int nbLiens;

    public Lien(Point point1, Point point2, int nbLiens)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.nbLiens = nbLiens;
    }

    public Lien()
    {

    }

    public bool Equals(Lien other)
    {
        return other != null ? (this.point1.Equals(other.point1) && this.point2.Equals(other.point2)) || (this.point1.Equals(other.point2) && this.point2.Equals(other.point1)) : false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Lien);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
