# Serialize and deserialize .csv files
Inspired by  https://gist.github.com/caschw/ddac05f58f1f081bd9da 
1. Can serialize anonymous classes in addition to regular classes.
1. Supports fields and properties.
1. Supports culture specific formatting of values.
1. Configurable separator, rownumbers
1. Separate NuGet adds extension methods for usage in LINQ pipelines

## Serialization example
```csharp
var Data = ...Some IEnumerable...
var SerializationOptions = new CsvSerializationOptions
{
    Separator = ';',
    Culture = CultureInfo.GetCultureInfo("nl-nl")
};

var Output = new MemoryStream();
using (var Writer = new StreamWriter(Output, new System.Text.UTF8Encoding(true), 1024, true))
{
    CsvSerializer.Serialize(Writer, Data, SerializationOptions);
}

 ```
## Deserialization example
```csharp
string ENUSTest = "Double;Float;Date\n1.1;2.2;12/23/2018 12:00:00 AM";

var DeserializationOptions = new CsvDeserializationOptions
{
    Culture = CultureInfo.GetCultureInfo("en-us"),
    Separator = ';'
};
using( var Reader = new StreamReader(getInputStream(ENUSTest), Encoding.UTF8); )
{
    var Data = CsvSerializer.Deserialize<HasLocalizable>(Reader, DeserializationOptions);
}
```

## Extension method example
```csharp
//Dont forget to Install NuGet Package Adrichem.Serialization.CsvSerializer.ExtensionMethods
using Adrichem.Serialization.CsvSerializer.ExtensionMethods;

var SerializationOptions = new CsvSerializationOptions
{
    Separator = ';',
    Culture = CultureInfo.GetCultureInfo("nl-nl"),
};

var Data = new List<MyClass> 
{ 
    new MyClass { Prop1 = "1" , Prop2 = "A"},  
    new MyClass { Prop1 = "2" , Prop2 = "B"}, 
    new MyClass { Prop1 = "3" , Prop2 = "C"}, 
};
Data
    .OrderBy( item => item.Prop1)
    .CsvSerialize<MyClass>("c:\\temp\\ascending.csv", new System.Text.UTF8Encoding(true))
    .OrderByDescending(item => item.Prop2)
    .CsvSerialize<MyClass>("c:\\temp\\descending.csv", new System.Text.UTF8Encoding(true), SerializationOptions)
;
 ```