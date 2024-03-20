using System;
using CodingSeb.ExpressionEvaluator;

namespace nettext
{
	/// <summary>
	/// Interface all plural evaluators implement.
	/// </summary>
	public interface IPluralEvaluator
	{
		/// <summary>
		/// Evaluates the given number and returns the plural form for it.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		int Eval(int n);
	}

	/// <summary>
	/// Default evaluator, using the English plural rule.
	/// </summary>
	public class DefaultPluralEvaluator : IPluralEvaluator
	{
		/// <summary>
		/// Evaluates the given number and returns the plural form for it.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public int Eval(int n)
		{
			return (n != 1 ? 1 : 0);
		}

		/// <summary>
		/// Returns a plural evaluator if one is available for the given
		/// plural, otherwise it returns null.
		/// </summary>
		/// <param name="pluralForms"></param>
		/// <returns></returns>
		public static IPluralEvaluator GetKnownEvaluator(string pluralForms)
		{
			pluralForms = pluralForms.Replace(" ", "");

			switch (pluralForms)
			{
				case "nplurals=1;plural=0;": return new PluralEvaluator1_0();
				case "nplurals=2;plural=(n!=1);": return new PluralEvaluator2_0();
				case "nplurals=2;plural=(n%10==1&&n%100!=11)?0:1;": return new PluralEvaluator2_1();
				case "nplurals=2;plural=(n>1);": return new PluralEvaluator2_2();
				case "nplurals=3;plural=(n%10==1&&n%100!=11?0:n!=0?1:2);": return new PluralEvaluator3_0();
				case "nplurals=3;plural=(n%10==1&&n%100!=11?0:n%10>=2&&(n%100<10||n%100>=20)?1:2);": return new PluralEvaluator3_1();
				case "nplurals=3;plural=(n%10==1&&n%100!=11?0:n%10>=2&&n%10<=4&&(n%100<10||n%100>=20)?1:2);": return new PluralEvaluator3_2();
				case "nplurals=3;plural=(n==1?0:n%10>=2&&n%10<=4&&(n%100<10||n%100>=20)?1:2);": return new PluralEvaluator3_3();
				case "nplurals=3;plural=(n==1?0:n%10>=2&&n%10<=4&&(n%100<10||n%100>=20)?1:2;": return new PluralEvaluator3_4();
				case "nplurals=3;plural=(n==1)?0:(n>=2&&n<=4)?1:2;": return new PluralEvaluator3_5();
				case "nplurals=3;plural=(n==1?0:(((n%100>19)||((n%100==0)&&(n!=0)))?2:1));": return new PluralEvaluator3_6();
				case "nplurals=4;plural=(n%100==1?0:n%100==2?1:n%100==3||n%100==4?2:3);": return new PluralEvaluator4_0();
				case "nplurals=4;plural=(n==1?0:n==0||(n%100>1&&n%100<11)?1:(n%100>10&&n%100<20)?2:3);": return new PluralEvaluator4_1();
				case "nplurals=4;plural=(n==1||n==11)?0:(n==2||n==12)?1:(n>2&&n<20)?2:3;": return new PluralEvaluator4_2();
				case "nplurals=4;plural=(n==1)?0:(n==2)?1:(n!=8&&n!=11)?2:3;": return new PluralEvaluator4_3();
				case "nplurals=4;plural=(n==1)?0:(n==2)?1:(n==3)?2:3;": return new PluralEvaluator4_4();
				case "nplurals=5;plural=(n==1?0:n==2?1:n<7?2:n<11?3:4);": return new PluralEvaluator5_0();
				case "nplurals=6;plural=(n==0?0:n==1?1:n==2?2:n%100>=3&&n%100<=10?3:n%100>=11&&n%100<=99?4:5);": return new PluralEvaluator6_0();
			}

			return null;
		}
	}


	// These evaluators are the only ones needed for the vast majority of
	// languages, based on a list of plural forms used by gettext.
	// Providing these saves us from compiling the classes during run-time
	// and allows nettext to be used where compilation isn't an option.

#pragma warning disable CS1591  // Missing XML comment
	public class PluralEvaluator1_0 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32(0); } }
	public class PluralEvaluator2_0 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n != 1)); } }
	public class PluralEvaluator2_1 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n % 10 == 1 && n % 100 != 11) ? 0 : 1); } }
	public class PluralEvaluator2_2 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n > 1)); } }
	public class PluralEvaluator3_0 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n % 10 == 1 && n % 100 != 11 ? 0 : n != 0 ? 1 : 2)); } }
	public class PluralEvaluator3_1 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2)); } }
	public class PluralEvaluator3_2 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n % 10 == 1 && n % 100 != 11 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2)); } }
	public class PluralEvaluator3_3 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2)); } }
	public class PluralEvaluator3_4 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2)); } }
	public class PluralEvaluator3_5 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1) ? 0 : (n >= 2 && n <= 4) ? 1 : 2); } }
	public class PluralEvaluator3_6 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1 ? 0 : (((n % 100 > 19) || ((n % 100 == 0) && (n != 0))) ? 2 : 1))); } }
	public class PluralEvaluator4_0 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n % 100 == 1 ? 0 : n % 100 == 2 ? 1 : n % 100 == 3 || n % 100 == 4 ? 2 : 3)); } }
	public class PluralEvaluator4_1 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1 ? 0 : n == 0 || (n % 100 > 1 && n % 100 < 11) ? 1 : (n % 100 > 10 && n % 100 < 20) ? 2 : 3)); } }
	public class PluralEvaluator4_2 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1 || n == 11) ? 0 : (n == 2 || n == 12) ? 1 : (n > 2 && n < 20) ? 2 : 3); } }
	public class PluralEvaluator4_3 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1) ? 0 : (n == 2) ? 1 : (n != 8 && n != 11) ? 2 : 3); } }
	public class PluralEvaluator4_4 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1) ? 0 : (n == 2) ? 1 : (n == 3) ? 2 : 3); } }
	public class PluralEvaluator5_0 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 1 ? 0 : n == 2 ? 1 : n < 7 ? 2 : n < 11 ? 3 : 4)); } }
	public class PluralEvaluator6_0 : IPluralEvaluator { public int Eval(int n) { return Convert.ToInt32((n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 && n % 100 <= 99 ? 4 : 5)); } }
#pragma warning restore CS1591

	internal class DynamicPluralEvaluator : IPluralEvaluator
	{
		private readonly ExpressionEvaluator _evaluator = new ExpressionEvaluator();
		private string _plural;

		public DynamicPluralEvaluator(string pluralForms)
		{
			var index = pluralForms.IndexOf("plural=");
			if (index == -1)
				throw new ArgumentException("Invalid plural forms: " + pluralForms);

			var plural = pluralForms.Substring(index + 7).Trim().TrimEnd(';');
			_plural = plural;
		}

		public int Eval(int n)
		{
			_evaluator.Variables["n"] = n;
			var result = _evaluator.Evaluate(_plural);

			return Convert.ToInt32(result);
		}
	}
}
