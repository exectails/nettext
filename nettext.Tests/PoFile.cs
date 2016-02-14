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

using Xunit;

namespace nettext.Tests
{
	public class PoFileTests
	{
		[Fact]
		public void ReadTestDe()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/de.po");

			Assert.Equal("Datei", po.GetString("File"));
			Assert.Equal("Datei", po.GetParticularString("some context", "File"));
			Assert.Equal("{0} Datei", po.GetPluralString("{0} file", "{0} files", 1));
			Assert.Equal("{0} Dateien", po.GetPluralString("{0} file", "{0} files", 5));
			Assert.Equal("{0} Dateien", po.GetParticularPluralString("some context", "{0} file", "{0} files", 5));
		}

		[Fact]
		public void ReadTestRu()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/ru.po");

			Assert.Equal("Файл", po.GetString("File"));
			Assert.Equal("Файл", po.GetParticularString("some context", "File"));
			Assert.Equal("{0} Файл", po.GetPluralString("{0} file", "{0} files", 1));
			Assert.Equal("{0} Файла", po.GetPluralString("{0} file", "{0} files", 2));
			Assert.Equal("{0} Файлов", po.GetPluralString("{0} file", "{0} files", 5));
			Assert.Equal("{0} Файлов", po.GetParticularPluralString("some context", "{0} file", "{0} files", 5));
		}

		[Fact]
		public void ReadTestPl()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/pl.po");

			Assert.Equal("plik", po.GetString("File"));
			Assert.Equal("plik", po.GetParticularString("some context", "File"));
			Assert.Equal("{0} plik", po.GetPluralString("{0} file", "{0} files", 1));
			Assert.Equal("{0} pliki", po.GetPluralString("{0} file", "{0} files", 2));
			Assert.Equal("{0} pliko'w", po.GetPluralString("{0} file", "{0} files", 5));
			Assert.Equal("{0} pliko'w", po.GetParticularPluralString("some context", "{0} file", "{0} files", 5));
		}

		[Fact]
		public void ReadTestDeUntranslated()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/de-untranslated.po");

			Assert.Equal("File", po.GetString("File"));
			Assert.Equal("File", po.GetParticularString("some context", "File"));
			Assert.Equal("{0} file", po.GetPluralString("{0} file", "{0} files", 1));
			Assert.Equal("{0} files", po.GetPluralString("{0} file", "{0} files", 5));
			Assert.Equal("{0} files", po.GetParticularPluralString("some context", "{0} file", "{0} files", 5));
		}

		[Fact]
		public void ReadTestDePartially()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/de-partially.po");

			Assert.Equal("Datei", po.GetString("File"));
			Assert.Equal("File", po.GetParticularString("some context", "File"));
			Assert.Equal("{0} Datei", po.GetPluralString("{0} file", "{0} files", 1));
			Assert.Equal("{0} files", po.GetPluralString("{0} file", "{0} files", 5));
			Assert.Equal("{0} file", po.GetParticularPluralString("some context", "{0} file", "{0} files", 1));
			Assert.Equal("{0} Dateien", po.GetParticularPluralString("some context", "{0} file", "{0} files", 5));
		}

		[Fact]
		public void ReadTestPlPartially()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/pl-partially.po");

			Assert.Equal("plik", po.GetString("File"));
			Assert.Equal("File", po.GetParticularString("some context", "File"));
			Assert.Equal("{0} file", po.GetPluralString("{0} file", "{0} files", 1));
			Assert.Equal("{0} pliki", po.GetPluralString("{0} file", "{0} files", 2));
			Assert.Equal("{0} files", po.GetPluralString("{0} file", "{0} files", 5));
			Assert.Equal("{0} plik", po.GetParticularPluralString("some context", "{0} file", "{0} files", 1));
			Assert.Equal("{0} files", po.GetParticularPluralString("some context", "{0} file", "{0} files", 2));
			Assert.Equal("{0} pliko'w", po.GetParticularPluralString("some context", "{0} file", "{0} files", 5));
		}

		[Fact]
		public void ReadTestDeNewLine()
		{
			var po = new PoFile();
			po.LoadFromFile("../../Files/de.po");

			Assert.Equal("Neue\nZeile 1", po.GetString("New\nLine 1"));
			Assert.Equal("Neue\nZeile 2", po.GetString("New\nLine 2"));
		}

		[Fact]
		public void HeaderTest()
		{
			var po = new PoFile();

			po.LoadFromFile("../../Files/de.po");
			Assert.Equal("de", po.GetHeader("Language"));

			po.LoadFromFile("../../Files/ru.po");
			Assert.Equal("ru", po.GetHeader("Language"));

			po.LoadFromFile("../../Files/pl.po");
			Assert.Equal("pl", po.GetHeader("Language"));
		}
	}
}
