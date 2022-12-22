namespace urele.Service.Test
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string charsSTR = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_+={}[]|:;'\"<>?";
			List<char> chars = charsSTR.ToList<char>();
			List<string> generated = new List<string>();
			File.WriteAllText("C:\\Users\\CASPER\\Desktop\\generated.txt", "--------");
			for (int j = 0; j < chars.Count; j++)
			{
				for (int k = 0; k < chars.Count; k++)
				{
					for (int l = 0; l < chars.Count; l++)
					{
						for (int m = 0; m < chars.Count; m++)
						{
							for (int n = 0; n < chars.Count; n++)
							{
								string token = chars[j].ToString() + chars[k].ToString() + chars[l].ToString() + chars[m].ToString() + chars[n].ToString();
								generated.Add(token);
							}
						}
					}
					File.AppendAllLines("C:\\Users\\CASPER\\Desktop\\generated.txt", generated);
					generated.Clear();
					Console.WriteLine("----KAYIT YAPILDI----");
				}
			}
			//Random rnd = new Random();
			//generated.Shuffle(rnd);
		}
	}
	public static class ListExtensions
	{
		public static void Shuffle<T>(this List<T> list, Random rnd)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int r = i + rnd.Next(list.Count - i);
				T temp = list[r];
				list[r] = list[i];
				list[i] = temp;
			}
		}
	}
}