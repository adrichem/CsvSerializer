
# Serialize and deserialize .csv files

Based on the version from https://gist.github.com/caschw/ddac05f58f1f081bd9da with the following changes:
1. Support fields aswel as properties
1. Serializing no longer disposes the output stream.
1. Serialize any object that implements  ```IEnumerable<T>```. It doesn't have to be a ```IList<T>```
1. Serialization of headers can be omitted by setting the ```UseHeader``` property.
1. Serialization and Deserialization support culture specific formatting through the ```Culture``` property. It defaults to the current culture of the thread.
1. Throws ```InvalidCsvFormatException``` instead of ```IndexOutOfRangeException``` when a line in the csv input has too many fields.



## Serialization example

```csharp
...
...
var Serializer = new CsvSerializer<HasLocalizable>()
{
    Separator=';',
    Culture = CultureInfo.GetCultureInfo("nl-nl")
};

var Data = new List<HasLocalizable>
{
    new HasLocalizable { Double = 1.1, Float=2.2F, Date = new DateTime(2018,12,23)},
};

var Output = new MemoryStream();
Serializer.Serialize(Output, Data);
...
...
public class HasLocalizable
{
    public double Double { get; set; }
    public float Float { get; set; }
    public DateTime Date { get; set; }

}
```

## Deserialization example

```csharp
string NLNLTest = "Double;Float;Date\n1,1;2,2;23-12-2018 00:00:00";
string ENUSTest = "Double;Float;Date\n1.1;2.2;12/23/2018 12:00:00 AM";
CsvSerializer<HasLocalizable> Serializer

Serializer = new CsvSerializer<HasLocalizable>()
{
    Culture = CultureInfo.GetCultureInfo("en-us"),
    Separator=';'
};
var Input = StringToStream(ENUSTest);

var Data = Serializer.Deserialize(Input) as IEnumerable<HasLocalizable>;
Assert.Equal(23, Data.First().Date.Day);
Assert.Equal(12, Data.First().Date.Month);
Assert.Equal(2018, Data.First().Date.Year);

Serializer = new CsvSerializer<HasLocalizable>()
{
    Culture = CultureInfo.GetCultureInfo("nl-nl"),
    Separator = ';'
};
Input = StringToStream(NLNLTest);

Data = Serializer.Deserialize(Input) as IEnumerable<HasLocalizable>;
Assert.Equal(23, Data.First().Date.Day);
Assert.Equal(12, Data.First().Date.Month);
Assert.Equal(2018, Data.First().Date.Year);

```

