// input en fichier pour visual studio
System.IO.StreamReader file = new System.IO.StreamReader("../../input1.txt");
int n = int.Parse(file.ReadLine());

// un input par ligne en List<int>
int n = int.Parse(Console.ReadLine());
			List<int> lst = new List<int>();
			for (int i = 0; i < n; i++)
			{
				lst.Add(int.Parse(Console.ReadLine()));
			}

// un input par ligne en List<string>
int n = int.Parse(Console.ReadLine());
			List<string> lst = new List<string>();
			for (int i = 0; i < n; i++)
			{
				lst.Add(Console.ReadLine());
			}

// input en string de chiffres ("1 3 2 4") en List<int>
string[] inputs = Console.ReadLine().Split(' ');
			List<int> lstNums = Array.ConvertAll(inputs, a => int.Parse(a)).ToList();
			
// double boucle for
for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					
				}
			}
		
// Liste de listes		
List<List<int>> grid = new List<List<int>>();
List<List<string>> grid = new List<List<string>>();

// comparaison de deux listes élément par élément
bool memesElements = lst2.SequenceEqual(lst1);

// clone d'une liste d'éléments de type valeur
List<int> lst2 = new List<int>(lst1);

// analyse des caractères
bool b = char.IsDigit(charAAnalyser);
bool b = char.IsLetter(charAAnalyser);
bool b = char.IsLetterOrDigit(charAAnalyser);
bool b = char.IsWhiteSpace(charAAnalyser);
bool b = char.IsLower(charAAnalyser);
bool b = char.IsUpper(charAAnalyser);
char charLower = char.ToLower(charUpper);
char charUpper = char.ToUpper(charLower);
int number = charAConvertir - 64; // caractère en chiffre (A = 1)
char caractere = (char)(intAConvertir + 64); // chiffre en caractère (A = 1)
int number = charAConvertir - 96; // caractère en chiffre (a = 1)
char caractere = (char)(intAConvertir + 96); // chiffre en caractère (a = 1)

// nombre d'occurences des éléments dans une liste
Dictionary<, int> dicCount = lstATraiter.GroupBy(e => e)
										.ToDictionary(g => g.Key, g => g.Count());
					  
					  
// String en DateTime :
DateTime dtDate = DateTime.ParseExact(strDate, format de la date dans strDate (ex : "dd/MM/yyyy HH:mm:ss"), System.Globalization.CultureInfo.CurrentCulture);

// DateTime en string :
string strDate = dtDate.ToString(format de la date voulu (ex : "dd/MM/yyyy HH:mm:ss"));

// limiter une string aux caractères autorisés
if (!Regex.IsMatch(stringAControler, @"^[a-zA-Z0-9éêèîïç'\s-]*$"))
	stringAControler = Regex.Replace(stringAControler, @"[^a-zA-Z0-9éêèîïç'\s-]", "");
			
// limiter une string à des chiffres
if (stringAControler.Any(c => !char.IsDigit(c)))
	stringAControler = new String(stringAControler.Where(c => char.IsDigit(c)).ToArray());

//supprime toutes les lettres d'une string
stringAModifier = Regex.Replace(stringAModifier, @"[a-zA-Z]", "");

//supprime tous les caractères qui ne sont pas compris dans une liste de caractères définis
stringAModifier = Regex.Replace(stringAModifier, @"[^a-zA-Z0-9éêèîïç'\s-]", "");