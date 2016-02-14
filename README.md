nettext
==============================
A managed .NET library for localization, using [gettext](https://www.gnu.org/software/gettext/)'s PO file format.

I needed a performant, managed solution, with proper plural support, that uses PO files. When I couldn't find one, I created one myself.

Methods
------------------------------
`GetString(string id)`

Returns translation for a single phrase.

`GetParticularString(string context, string id)`

Returns translation for a single phrase in a specific context.

`GetPluralString(string id, string id_plural, int n)`

Returns translation for a plural string, based on n.

`GetParticularPluralString(string context, string id, string id_plural, int n)`

Returns translation for singular or plural string in a specific context, based on n.


Usage
------------------------------
```
using nettext;
```
```
var po = new PoFile("de.po");

Console.WriteLine(po.GetString("File")); // Datei
Console.WriteLine(po.GetPluralString("{0} file", "{0} files", 2), 2); // 2 Dateien
Console.WriteLine(po.GetParticularString("office", "File")); // Akte
```

For more examples, check the tests.

Links
------------------------------
* GitHub: https://github.com/exectails/nettext
