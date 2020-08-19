
public static class OutilsGrilles
{
	static System.IO.StreamReader file;
	static int W;
	static int H;
	static List<Point> lstPoints;

	static void Main(string[] args)
	{
		// à partir d'un fichier
		file = new System.IO.StreamReader("../../input1.txt");
		OutilsGrilles.BuildGrid(() => file.ReadLine()); 

		// à partir de la console
		OutilsGrilles.BuildGrid(() => Console.ReadLine()); 

		//OutilsGrilles.PrintGrid();

		// traitement
	}

	public static void BuildGrid(Func<string> ReadLine)
	{
		W = int.Parse(ReadLine());
		H = int.Parse(ReadLine());

		for (int i = 0; i < H; i++)
		{
			string line = ReadLine();
			for (int j = 0; j < W; j++)
			{
				lstPoints.AddPoint(i, j, line[j].ToString());
			}
		}
	}

	public static void PrintGrid()
	{
		for (int i = 0; i < H; i++)
		{
			for (int j = 0; j < W; j++)
			{
				Console.Error.Write(lstPoints.GetPoint(i, j).value);
			}
			Console.Error.WriteLine();
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

	public Coord()
	{
		i = -1;
		j = -1;
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

	public override string ToString()
	{
		return string.Format("i : {0} j : {1}", i, j);
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
		coord = new Coord(i, j);
		value = pValue;
	}

	public Point()
	{
		i = -1;
		j = -1;
		coord = new Coord(i, j);
		value = "";
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

	public static void SetPointValue(this List<Point> grid, Point point, string value)
	{
		grid.SingleOrDefault(p => p == point).value = value;
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
