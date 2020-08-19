using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProgramGrid
{
    static bool isConsole = false;
    static System.IO.StreamReader file = isConsole ? null : new System.IO.StreamReader("../../input1.txt");

    static string gridInputSeparator = " ";
    static int W = 9;
    static int H = 9;

    static List<Point> grid;

    static void MainGrid(string[] args)
    {
        BuildGrid();
        PrintGrid();

        //traitement
    }

    static string ReadLine() => file == null ? Console.ReadLine() : file.ReadLine();

    static string[] ParseGridLine(string line)
    {
        if (string.IsNullOrEmpty(gridInputSeparator))
            return line?.ToCharArray().Select(x => x.ToString()).ToArray();
        else
            return line?.Split(gridInputSeparator.ToCharArray());
    }

    static void BuildGrid()
    {
        W = W == 0 ? int.Parse(ReadLine()) : W;
        H = H == 0 ? int.Parse(ReadLine()) : H;
        grid = new List<Point>();

        for (int i = 0; i < H; i++)
        {
            string[] line = ParseGridLine(ReadLine());
            for (int j = 0; j < W; j++)
            {
                grid.AddPoint(i, j, line[j]);
            }
        }
    }

    static void PrintGrid()
    {
        for (int i = 0; i < H; i++)
        {
            for (int j = 0; j < W; j++)
            {
                Console.Error.Write(grid.GetPoint(i, j).value + gridInputSeparator);
            }
            Console.Error.WriteLine();
        }
        Console.Error.WriteLine();
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

public class Coord : IEquatable<Coord>
{
    public int i;
    public int j;

    public Coord(int pi, int pj)
    {
        i = pi;
        j = pj;
    }

    public bool Equals(Coord other)
    {
        if (other != null)
            return (this.i == other.i && this.j == other.j);
        else
            return false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Coord);
    }

    public static bool operator ==(Coord obj1, Coord obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
            return false;

        return obj1.Equals(obj2);
    }

    public static bool operator !=(Coord obj1, Coord obj2)
    {
        return !(obj1 == obj2);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class Point : IEquatable<Point>
{
    public int i;
    public int j;
    public Coord coord;
    public string value;

    public Point(int pi, int pj, string pValue)
    {
        i = pi;
        j = pj;
        coord = new Coord(pi, pj);
        value = pValue;
    }

    public List<Coord> GetNearCoords(bool getDiagonales = false)
    {
        List<Coord> lstNearCoords = new List<Coord>();
        lstNearCoords.Add(new Coord(i - 1, j));
        lstNearCoords.Add(new Coord(i, j + 1));
        lstNearCoords.Add(new Coord(i + 1, j));
        lstNearCoords.Add(new Coord(i, j - 1));
        if (getDiagonales)
        {
            lstNearCoords.Add(new Coord(i - 1, j + 1));
            lstNearCoords.Add(new Coord(i + 1, j + 1));
            lstNearCoords.Add(new Coord(i + 1, j - 1));
            lstNearCoords.Add(new Coord(i - 1, j - 1));
        }
        return lstNearCoords;
    }

    public List<Point> GetNearPoints(List<Point> grid, bool getDiagonales = false)
    {
        List<Coord> lstNearCoords = GetNearCoords(getDiagonales);
        return grid.Where(p => lstNearCoords.Any(c => c == p.coord)).ToList();
    }

    public override string ToString()
    {
        return string.Format("i : {0} j : {1} val : {2}", i, j, value);
    }

    public bool Equals(Point other)
    {
        if (other != null)
            return (this.coord.Equals(other.coord) && this.value == other.value);
        else
            return false;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Point);
    }

    public static bool operator ==(Point obj1, Point obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
            return false;

        return obj1.Equals(obj2);
    }

    public static bool operator !=(Point obj1, Point obj2)
    {
        return !(obj1 == obj2);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public static class MyExtensions
{
    public static Point GetPoint(this List<Point> grid, int i, int j)
    {
        return grid.SingleOrDefault(p => p.i == i && p.j == j);
    }

    public static string GetPointValue(this List<Point> grid, int i, int j)
    {
        return grid.SingleOrDefault(p => p.i == i && p.j == j).value;
    }

    public static List<Point> GetPointsWithValue(this List<Point> grid, string value)
    {
        return grid.Where(p => p.value == value).ToList();
    }

    public static void SetPointValue(this List<Point> grid, int i, int j, string value)
    {
        grid.SingleOrDefault(p => p.i == i && p.j == j).value = value;
    }

    public static void AddPoint(this List<Point> grid, int i, int j, string value)
    {
        grid.Add(new Point(i, j, value));
    }

    public static void UpdateGrid(this List<Point> grid, List<Point> gridForUpdate)
    {
        gridForUpdate.ForEach(p => grid.SetPointValue(p.i, p.j, p.value));
    }
}
