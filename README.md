# Serialize and deserialize .csv files
Inspired by  https://gist.github.com/caschw/ddac05f58f1f081bd9da 
1. Can serialize anonymous classes in addition to regular classes.
1. Supports fields and properties.
1. Supports culture specific formatting of values.
1. Configurable separator, rownumbers
1. Separate NuGet adds 

## Serialization example
```csharp
var Data = ...Some IEnumerable...
var SerializationOptions = new CsvSerializationOptions
{
    Separator = ';',
    Culture = CultureInfo.GetCultureInfo("nl-nl")
};

var Output = new MemoryStream();
CsvSerializer.Serialize(Output, Data, SerializationOptions);
 ```
## Deserialization example
```csharp
string ENUSTest = "Double;Float;Date\n1.1;2.2;12/23/2018 12:00:00 AM";

var DeserializationOptions = new CsvDeserializationOptions
{
    Culture = CultureInfo.GetCultureInfo("en-us"),
    Separator = ';'
};

var Data = CsvSerializer.Deserialize<HasLocalizable>(StringToStream(ENUSTest), DeserializationOptions);
```

## Extension methods example
```csharp
//Dont forget to Install NuGet Package Adrichem.Serialization.CsvSerializer.ExtensionMethods
using Adrichem.Serialization.CsvSerializer.ExtensionMethods;

var SerializationOptions = new CsvSerializationOptions
{
    Separator = ';',
    Culture = CultureInfo.GetCultureInfo("nl-nl"),
};

var AscendingOutput = new FileStream("c:\\temp\\ascending.csv", FileMode.OpenOrCreate);
var DescendingOutput = new FileStream("c:\\temp\\descending.csv", FileMode.OpenOrCreate);

var Data = new List<MyClass> 
{ 
    new MyClass { Prop1 = "1" , Prop2 = "A"},  
    new MyClass { Prop1 = "2" , Prop2 = "B"}, 
    new MyClass { Prop1 = "3" , Prop2 = "C"}, 
};
Data
    .OrderBy( item => item.Prop1)
    .CsvSerialize<MyClass>(AscendingOutput)
    .OrderByDescending(item => item.Prop2)
    .CsvSerialize<MyClass>(DescendingOutput, SerializationOptions)
;
 ```