# Serialize and deserialize .csv files
Inspired by  https://gist.github.com/caschw/ddac05f58f1f081bd9da 
1. Can serialize anonymous classes in addition to regular classes.
1. Supports fields and properties.
1. Supports culture specific formatting of values.
1. Configurable separator, rownumbers

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