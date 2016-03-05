// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nettext
{
	/// <summary>
	/// Manages a gettext PO file.
	/// </summary>
	public class PoFile
	{
		private Dictionary<string, Message> _storage;
		private Dictionary<string, string> _headers;

		private int _nplurals;
		private string _plural;
		private IPluralEvaluator _pluralEvaluator;

		/// <summary>
		/// Creates new instance.
		/// </summary>
		public PoFile()
		{
			this.Init();
		}

		/// <summary>
		/// Creates new instance and loads given file.
		/// </summary>
		/// <param name="filePath"></param>
		public PoFile(string filePath)
		{
			this.LoadFromFile(filePath);
		}

		/// <summary>
		/// Resets variables to defaults, in preparation for loading
		/// a new file.
		/// </summary>
		private void Init()
		{
			_storage = new Dictionary<string, Message>();
			_headers = new Dictionary<string, string>();

			_nplurals = 2;
			_plural = "(n != 1)";
			_pluralEvaluator = new DefaultPluralEvaluator();
		}

		/// <summary>
		/// Returns culture info for the file's language.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="MissingFieldException">If Language header isn't set.</exception>
		public string GetHeader(string name)
		{
			string result;
			if (!_headers.TryGetValue(name, out result))
				throw new MissingFieldException("Language header missing.");

			return result;
		}

		/// <summary>
		/// Loads messages from given file.
		/// </summary>
		/// <remarks>
		/// Clears messages when called repeatedly.
		/// </remarks>
		/// <param name="filePath">Path to a .po file.</param>
		/// <exception cref="FileNotFoundException">If file doesn't exist.</exception>
		/// <exception cref="ArgumentException">If file doesn't have a .po extension.</exception>
		/// <exception cref="ArgumentException">If Plural-Form header has the wrong format.</exception>
		/// <exception cref="FormatException">If Plural-Form header contains an invalid formula.</exception>
		/// <exception cref="TypeLoadException">If plural evaluator couldn't be created.</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException(filePath);

			if (Path.GetExtension(filePath) != ".po")
				throw new ArgumentException("File is not a PO file.");

			using (var sr = new StreamReader(filePath, Encoding.UTF8))
				LoadFromReader(sr);
		}

		/// <summary>
		/// Loads messages from given stream.
		/// </summary>
		/// <remarks>
		/// Clears messages when called repeatedly.
		/// </remarks>
		/// <param name="stream"></param>
		/// <exception cref="ArgumentException">If Plural-Form header has the wrong format.</exception>
		/// <exception cref="FormatException">If Plural-Form header contains an invalid formula.</exception>
		/// <exception cref="TypeLoadException">If plural evaluator couldn't be created.</exception>
		public void LoadFromStream(Stream stream)
		{
			this.LoadFromReader(new StreamReader(stream));
		}

		/// <summary>
		/// Loads messages from given text reader.
		/// </summary>
		/// <remarks>
		/// Clears messages when called repeatedly.
		/// </remarks>
		/// <param name="reader"></param>
		/// <exception cref="ArgumentException">If Plural-Form header has the wrong format.</exception>
		/// <exception cref="FormatException">If Plural-Form header contains an invalid formula.</exception>
		/// <exception cref="TypeLoadException">If plural evaluator couldn't be created.</exception>
		public void LoadFromReader(TextReader reader)
		{
			this.Init();

			var regex = new Regex(@"""(.*)""", RegexOptions.Compiled | RegexOptions.Singleline);
			var regex2 = new Regex(@"^msgstr\[([0-9]+)\]", RegexOptions.Compiled);

			var message = new Message();
			var type = BlockType.None;

			string line = null;
			while ((line = reader.ReadLine()) != null)
			{
				line = line.Trim();

				// Skip comments
				if (line.StartsWith("#"))
					continue;

				// Empty line, initiate new message
				if (string.IsNullOrEmpty(line))
				{
					Add(message);
					message = new Message();
					continue;
				}

				// Decide where to put the data
				if (line.StartsWith("msgctxt"))
					type = BlockType.Context;
				else if (line.StartsWith("msgid_plural"))
				{
					type = BlockType.IdPlural;

					// Create space for all plural forms
					message.EnsureSpace(_nplurals);
				}
				else if (line.StartsWith("msgid"))
					type = BlockType.Id;
				else if (line.StartsWith("msgstr"))
					type = BlockType.Str;
				else if (!line.StartsWith("\""))
					type = BlockType.None;

				// Handle data
				var match = regex.Match(line);
				if (match.Success)
				{
					var val = Unescape(match.Groups[1].Value);
					switch (type)
					{
						case BlockType.Context: message.Context += val; break;
						case BlockType.Id: message.Id += val; break;
						case BlockType.IdPlural: message.IdPlural += val; break;
						case BlockType.Str:
							var index = 0;

							// Get index based on msgstr indexer, or default
							// to 0 for the first/non-plural str.
							match = regex2.Match(line);
							if (match.Success)
								index = Convert.ToInt32(match.Groups[1].Value);

							message.Str[index] += val;
							break;
					}
				}
			}

			// TextReader skips last empty line, add last message
			Add(message);
		}

		/// <summary>
		/// Unescapes string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static string Unescape(string str)
		{
			str = str.Replace("\\t", "\t");
			str = str.Replace("\\r\\n", "\r\n");
			str = str.Replace("\\n", "\n");
			str = str.Replace("\\\"", "\"");

			return str;
		}

		/// <summary>
		/// Adds message to storage.
		/// </summary>
		/// <remarks>
		/// Reads headers if message id is "".
		/// </remarks>
		/// <param name="message"></param>
		private void Add(Message message)
		{
			// Ignore message if id wasn't set or all strings are empty.
			if (message.Id == null || message.Str.All(a => string.IsNullOrEmpty(a)))
				return;

			_storage[message.Key] = message;

			// Load headers as soon as available, we need the plual forms.
			if (message.Id == "")
			{
				LoadHeaders(message.Str[0]);

				// Load plural forms if header is given
				if (_headers.ContainsKey("Plural-Forms"))
					LoadPluralEvaluator(_headers["Plural-Forms"]);
			}
		}

		/// <summary>
		/// Loads headers from msg string.
		/// </summary>
		/// <param name="msgstr"></param>
		private void LoadHeaders(string msgstr)
		{
			using (var sr = new StringReader(msgstr))
			{
				string line = null;
				while ((line = sr.ReadLine()) != null)
				{
					// Ignore empty lines
					if (string.IsNullOrWhiteSpace(line))
						continue;

					var start = line.IndexOf(":");
					var key = line.Substring(0, start).Trim();
					var val = line.Substring(start + 1).Trim();

					_headers.Add(key, val);
				}
			}
		}

		/// <summary>
		/// Loads plural information and evaluator, based plural forms string.
		/// </summary>
		/// <param name="pluralForms">Plural-Forms header.</param>
		/// <example>
		/// LoadPluralEvaluator("nplurals=2; plural=(n != 1);")
		/// </example>
		private void LoadPluralEvaluator(string pluralForms)
		{
			var match = Regex.Match(pluralForms, @"^nplurals=(?<nplurals>[0-9]+);\s*plural=(?<plural>[0-9n\?:\(\)!=\s%<>|&]+);?$");
			if (!match.Success)
				throw new ArgumentException("Invalid Plural-Forms value: " + pluralForms);

			// Read number and rules
			_nplurals = Convert.ToInt32(match.Groups["nplurals"].Value);
			_plural = match.Groups["plural"].Value;

			// Create script
			var script = @"
				using System;
				using nettext;

				public class PluralEvaluator : IPluralEvaluator
				{
					public int Eval(int n)
					{
						// A plural like `(n != 1)` would result in a bool
						// in C#, use Convert to guarantee an int result.
						return Convert.ToInt32(" + _plural + @");
					}
				}
			";

			// Prepare compiler
			var compiler = new CSharpCodeProvider();
			var parameters = new CompilerParameters();
			parameters.ReferencedAssemblies.Add(typeof(IPluralEvaluator).Assembly.Location);
			parameters.GenerateExecutable = false;
			parameters.GenerateInMemory = true;

			// Compile, throw if compilation failed
			var result = compiler.CompileAssemblyFromSource(parameters, script);
			foreach (CompilerError err in result.Errors)
				throw new FormatException("Failed to compile plural evaluator: " + err.ErrorText);

			// Get type
			var type = result.CompiledAssembly.GetType("PluralEvaluator");
			if (type == null)
				throw new TypeLoadException("Failed to generate plural evaluator, no PluralEvaluator.");

			// Instantiate evaluator
			_pluralEvaluator = Activator.CreateInstance(type) as IPluralEvaluator;
			if (_pluralEvaluator == null)
				throw new TypeLoadException("Failed to generate plural evaluator, no IPluralEvaluator.");
		}

		/// <summary>
		/// Returns translated string, or id if no translated version
		/// of id exists.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetString(string id)
		{
			Message message;
			if (!_storage.TryGetValue(id, out message) || string.IsNullOrEmpty(message.Str[0]))
				return id;

			return message.Str[0];
		}

		/// <summary>
		/// Returns translated string in context, or id if no translated
		/// version of id exists.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetParticularString(string context, string id)
		{
			var fullId = context + Message.ContextSeperator + id;

			Message message;
			if (!_storage.TryGetValue(fullId, out message) || string.IsNullOrEmpty(message.Str[0]))
				return id;

			return message.Str[0];
		}

		/// <summary>
		/// Returns translated string as singular or plural, based on n,
		/// or id/id_plural if no translated version of id exists.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="id_plural"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public string GetPluralString(string id, string id_plural, int n)
		{
			var index = _pluralEvaluator.Eval(n);

			Message message;
			if (_storage.TryGetValue(id, out message))
			{
				if (!string.IsNullOrEmpty(message.Str[index]))
					return message.Str[index];
			}

			return (n != 1 ? id_plural : id);
		}

		/// <summary>
		/// Returns translated string in context as singular or plural,
		/// based on n, or id/id_plural if no translated version of id exists.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetParticularPluralString(string context, string id, string id_plural, int n)
		{
			var fullId = context + Message.ContextSeperator + id;
			var index = _pluralEvaluator.Eval(n);

			Message message;
			if (_storage.TryGetValue(fullId, out message))
			{
				if (!string.IsNullOrEmpty(message.Str[index]))
					return message.Str[index];
			}

			return (n != 1 ? id_plural : id);
		}

		/// <summary>
		/// Block type to read.
		/// </summary>
		private enum BlockType
		{
			None,
			Context,
			Id,
			IdPlural,
			Str,
		}

		/// <summary>
		/// Represents a gettext message.
		/// </summary>
		private class Message
		{
			/// <summary>
			/// "Unique" seperator for indexing context sensitive messages.
			/// </summary>
			public const string ContextSeperator = "$__//_$_//__$";

			/// <summary>
			/// Context for this message, null if none.
			/// </summary>
			public string Context { get; set; }

			/// <summary>
			/// Id of this message, null if none.
			/// </summary>
			public string Id { get; set; }

			/// <summary>
			/// Plural id of this message, null if none.
			/// </summary>
			public string IdPlural { get; set; }

			/// <summary>
			/// Strings of this message.
			/// </summary>
			/// <remarks>
			/// Contains at least one empty string element after message creation.
			/// </remarks>
			public List<string> Str { get; set; }

			/// <summary>
			/// Creates new message.
			/// </summary>
			public Message()
			{
				this.Str = new List<string>();
				this.Str.Add("");
			}

			/// <summary>
			/// Returns key for this message, based on Id and Context.
			/// </summary>
			public string Key
			{
				get
				{
					if (string.IsNullOrEmpty(this.Context))
						return this.Id;
					return this.Context + ContextSeperator + this.Id;
				}
			}

			/// <summary>
			/// Prepares message to take the given amount of strings for plurals.
			/// </summary>
			/// <param name="n">Amount of plural forms.</param>
			public void EnsureSpace(int n)
			{
				while (this.Str.Count < n)
					this.Str.Add("");
			}
		}
	}
}
