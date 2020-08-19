
class Solution
{
    static int W;
    static int H;
    static List<Point> lstPoints;
    
    static void Main(string[] args)
    {
        W = int.Parse(Console.ReadLine());
        H = int.Parse(Console.ReadLine());
        
        lstPoints = new List<Point>();
        for (int i = 0; i < H; i++)
        {
            string line = Console.ReadLine();
            for (int j = 0; j < W; j++)
            {
                lstPoints.AddPoint(i, j, line[j].ToString());
            }
        }
        
        /*for (int i = 0; i < H; i++)
        {
            for (int j = 0; j < W; j++)
            {
                Console.Error.Write(lstPoints.GetPoint(i, j).value);
            }
            Console.Error.WriteLine();
        }
        Console.Error.WriteLine();*/
		
		for (int i = 0; i < H; i++)
        {
            string test = "";
			for (int j = 0; j < W; j++)
            {
                test += lstPoints.GetPoint(i, j).value.ToString();
            }
			Utils.LocalPrint(test);
        }
        
		// traitement
		
        for (int i = 0; i < H; i++)
        {
            for (int j = 0; j < W; j++)
            {
                Console.Write(lstPoints.GetPoint(i, j).value);
            }
            Console.WriteLine();
        }
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
    
    public static bool operator == (Coord obj1, Coord obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
            return false;
        
        return obj1.Equals(obj2);
    }
    
    public static bool operator != (Coord obj1, Coord obj2)
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
    public Coord coord;
    public string value;
    
    public Point(Coord pCoord, string pValue)
    {
        coord = pCoord;
        value = pValue;
    }
    
    public Point(int pi, int pj, string pValue)
    {
        coord = new Coord(pi, pj);
        value = pValue;
    }
    
    public List<Coord> GetNearCoords()
    {
        int i = coord.i;
        int j = coord.j;
        List<Coord> lstNearCoords = new List<Coord>();
        lstNearCoords.Add(new Coord(i-1, j));
        lstNearCoords.Add(new Coord(i, j+1));
        lstNearCoords.Add(new Coord(i+1, j));
        lstNearCoords.Add(new Coord(i, j-1));
		// diagonales
		lstNearCoords.Add(new Coord(i-1, j+1));
        lstNearCoords.Add(new Coord(i+1, j+1));
        lstNearCoords.Add(new Coord(i+1, j-1));
        lstNearCoords.Add(new Coord(i-1, j-1));
        return lstNearCoords;
    }
    
    public List<Point> GetNearPoints(List<Point> lstPoints)
    {
        List<Coord> lstNearCoords = GetNearCoords();
        return lstPoints.Where(p => lstNearCoords.Any(c => c == p.coord)).ToList();
    }
    
    public override string ToString()
    {
        return string.Format("i : {0} j : {1} val : {2}", coord.i, coord.j, value);
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
    
    public static bool operator == (Point obj1, Point obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (ReferenceEquals(obj1, null) || ReferenceEquals(obj2, null))
            return false;
            
        return obj1.Equals(obj2);
    }
    
    public static bool operator != (Point obj1, Point obj2)
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
    public static Point GetPoint(this List<Point> lst, Coord coord)
    {
         return lst.SingleOrDefault(p => p.coord == coord);
    }
    
    public static Point GetPoint(this List<Point> lst, int i, int j)
    {
         return lst.SingleOrDefault(p => p.coord.i == i && p.coord.j == j);
    }
    
    public static void AddPoint(this List<Point> lst, Coord coord, string value)
    {
         lst.Add(new Point(coord, value));
    }
    
    public static void AddPoint(this List<Point> lst, int i, int j, string value)
    {
         lst.Add(new Point(i, j, value));
    }
}