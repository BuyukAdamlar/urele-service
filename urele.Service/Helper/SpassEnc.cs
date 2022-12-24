using SpassMYurga;
namespace urele.Service.Helper
{
	public class SpassEnc
	{
		public static string Encrypt(string pass)
		{
			string res = SPassByMyurga.Encrypt(pass, "7040521").getPass40();
			if (res.Contains("\""))
			{
				res = res.Replace("\"", "7");
			}
			if (res.Contains("'"))
			{
				res = res.Replace("'", "7");
			}
			return res;
		}
	}
}
