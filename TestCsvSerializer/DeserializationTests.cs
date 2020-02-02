namespace Adrichem.Serialization.CsvSerializer.TestCsvSerializer
{
    using Xunit;
    using Adrichem.Serialization.CsvSerializer;
    using System.IO;
    using System.Globalization;
    using System;
    using System.Linq;
    using System.Text;

    public class DeserializationTests
    {

        private StreamReader StringToStreamReader(string csvContent)
        {
            MemoryStream InputStream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            InputStream.Write(bytes, 0, bytes.Count());
            InputStream.Seek(0, 0);
            return new StreamReader(InputStream, Encoding.UTF8);
        }

        [Fact]
        public void TestIncompleteLine()
        {
            var Input = StringToStreamReader("Prop1,Prop2" + Environment.NewLine
                + "A" + Environment.NewLine
                + "C,D");
 
            var Data = CsvSerializer.Deserialize<HasProperties>(Input);

            Assert.True(Data.Count() == 2);
            Assert.Equal("A", Data.First().Prop1);
            Assert.Null(Data.First().Prop2);
            Assert.Equal("C", Data.Last().Prop1);
            Assert.Equal("D", Data.Last().Prop2);
        }

        [Fact]
        public void TestTooManyFieldsOnLine()
        {
            var Input = StringToStreamReader(
                 "Prop1,Prop1"
                + Environment.NewLine
                + "A,B,C,D,E");

            Assert.ThrowsAny<CsvFormatException>(() => CsvSerializer.Deserialize<HasLocalizable>(Input));
        }

        [Fact]
        public void TestEOF()
        {
            var Input = StringToStreamReader(string.Empty);
            Assert.ThrowsAny<CsvFormatException>(() => CsvSerializer.Deserialize<HasLocalizable>(Input));
        }

        [Fact]
        public void TestLocalized()
        {
            string NLNLTest = "Double;Float;Date\n1,1;2,2;23-12-2018 00:00:00";
            string ENUSTest = "Double;Float;Date\n1.1;2.2;12/23/2018 12:00:00 AM";


            var DeserializationOptions = new CsvDeserializationOptions
            {
                Culture = CultureInfo.GetCultureInfo("en-us"),
                Separator = ';'
            };

            var Input = StringToStreamReader(ENUSTest);

            var Data = CsvSerializer.Deserialize<HasLocalizable>(Input, DeserializationOptions);
            Assert.Single(Data);
            Assert.Equal(1, Math.Floor(Data.First().Double));
            Assert.Equal(2, Math.Floor(Data.First().Float));
            Assert.Equal(23, Data.First().Date.Day);
            Assert.Equal(12, Data.First().Date.Month);
            Assert.Equal(2018, Data.First().Date.Year);

            DeserializationOptions.Culture = CultureInfo.GetCultureInfo("nl-nl");
            Input = StringToStreamReader(NLNLTest);
            Data = CsvSerializer.Deserialize<HasLocalizable>(Input, DeserializationOptions);
            Assert.Single(Data);
            Assert.Equal(1, Math.Floor(Data.First().Double));
            Assert.Equal(2, Math.Floor(Data.First().Float));
            Assert.Equal(23, Data.First().Date.Day);
            Assert.Equal(12, Data.First().Date.Month);
            Assert.Equal(2018, Data.First().Date.Year);
        }


        [Fact]
        public void TestLineNumbers()
        {
            string tmp = string.Empty;
            tmp += "#,Prop1,Prop2" + Environment.NewLine;
            tmp += "1,A,B" + Environment.NewLine;
            tmp += "2,C,D" + Environment.NewLine;

            var Input = StringToStreamReader(tmp);

            var Data = CsvSerializer.Deserialize<HasProperties>(Input, new CsvDeserializationOptions
            {
                UseRowNumbers = true,
            });
            Assert.True(Data.Count() == 2);
            Assert.Equal("A", Data.First().Prop1);
            Assert.Equal("B", Data.First().Prop2);
            Assert.Equal("C", Data.Last().Prop1);
            Assert.Equal("D", Data.Last().Prop2);

        }

        [Fact]
        public void TestDifferentOrder()
        {
            string tmp = string.Empty;
            tmp += "Prop2,Prop1" + Environment.NewLine;
            tmp += "A,B" + Environment.NewLine;
            tmp += "C,D" + Environment.NewLine;

            var Input = StringToStreamReader(tmp);

            var Data = CsvSerializer.Deserialize<HasProperties> (Input);
            Assert.True(Data.Count() == 2);
            Assert.Equal("B", Data.First().Prop1);
            Assert.Equal("A", Data.First().Prop2);
            Assert.Equal("D", Data.Last().Prop1);
            Assert.Equal("C", Data.Last().Prop2);

        }

        [Fact]
        public void TestFields()
        {
            string tmp = string.Empty;
            tmp += "Field1,Field2,intField,boolField" + Environment.NewLine;
            tmp += "A,B,1,False" + Environment.NewLine;
            tmp += "C,D,2,True" + Environment.NewLine;

            var Input = StringToStreamReader(tmp);

            var Data = CsvSerializer.Deserialize<HasFields>(Input);
            Assert.True(Data.Count() == 2);
            Assert.Equal("A", Data.First().Field1);
            Assert.Equal("B", Data.First().Field2);
            Assert.Equal(1, Data.First().intField);
            Assert.False(Data.First().boolField);
            Assert.Equal("C", Data.Last().Field1);
            Assert.Equal("D", Data.Last().Field2);
            Assert.Equal(2, Data.Last().intField);
            Assert.True(Data.Last().boolField);

        }

        [Fact]
        public void TestLocalizableFields()
        {
            string NLNLTest = "Double;Float;Date\n1,1;2,2;23-12-2018 00:00:00";
            string ENUSTest = "Double;Float;Date\n1.1;2.2;12/23/2018 12:00:00 AM";


            var DeserializationOptions = new CsvDeserializationOptions
            {
                Culture = CultureInfo.GetCultureInfo("en-us"),
                Separator = ';'
            };

            var Input = StringToStreamReader(ENUSTest);

            var Data = CsvSerializer.Deserialize<HasLocalizableFields>(Input, DeserializationOptions);
            Assert.Single(Data);
            Assert.Equal(1, Math.Floor(Data.First().Double));
            Assert.Equal(2, Math.Floor(Data.First().Float));
            Assert.Equal(23, Data.First().Date.Day);
            Assert.Equal(12, Data.First().Date.Month);
            Assert.Equal(2018, Data.First().Date.Year);


            DeserializationOptions.Culture = CultureInfo.GetCultureInfo("nl-nl");
            Input = StringToStreamReader(NLNLTest);
            Data = CsvSerializer.Deserialize<HasLocalizableFields>(Input, DeserializationOptions);
            Assert.Single(Data);
            Assert.Equal(1, Math.Floor(Data.First().Double));
            Assert.Equal(2, Math.Floor(Data.First().Float));
            Assert.Equal(23, Data.First().Date.Day);
            Assert.Equal(12, Data.First().Date.Month);
            Assert.Equal(2018, Data.First().Date.Year);
        }

        [Fact]
        public void TestTextQualifier()
        {
             string tmp = string.Empty;
            tmp += "Field1,Field2,intField,boolField" + Environment.NewLine;
            tmp += "\"A\",\"B\",\"1\",\"False\"" + Environment.NewLine;
            tmp += "\"C\",\"D\",\"2\",\"True\"" + Environment.NewLine;

            var Input = StringToStreamReader(tmp);

            var Data = CsvSerializer.Deserialize<HasFields>(Input, new CsvDeserializationOptions {  UseTextQualifier = true});
            Assert.True(Data.Count() == 2);
            Assert.Equal("A", Data.First().Field1);
            Assert.Equal("B", Data.First().Field2);
            Assert.Equal(1, Data.First().intField);
            Assert.False(Data.First().boolField);
            Assert.Equal("C", Data.Last().Field1);
            Assert.Equal("D", Data.Last().Field2);
            Assert.Equal(2, Data.Last().intField);
            Assert.True(Data.Last().boolField);

        }

        /// <summary>
        /// Verifies deserialisation when user specifies a non default separator.
        /// </summary>
        [Fact]
        public void TestSeparatorsNonDefault()
        {
           var Reader = StringToStreamReader("Field1;Field2\n\"Hello\";\"World\"");
           var Result = CsvSerializer.Deserialize<HasFields>(Reader , new CsvDeserializationOptions { Separator = ';', UseTextQualifier = true });
            Assert.Single(Result);
            Assert.Equal("Hello", Result.First().Field1);
            Assert.Equal("World", Result.First().Field2);
        }

        /// <summary>
        /// Verifies deserialisation when the file specifies the separator.
        /// </summary>
        [Fact]
        public void TestSepartorHeaderLines()
        {
            var Reader = StringToStreamReader("sep=;\nField1;Field2\n\"Hello\";\"World\"");
            var Options = new CsvDeserializationOptions { Separator = ';', UseTextQualifier = true };
            var Result = CsvSerializer.Deserialize<HasFields>(Reader, Options);
            Assert.Single(Result);
            Assert.Equal("Hello", Result.First().Field1);
            Assert.Equal("World", Result.First().Field2);
            Assert.Equal(';'.ToString(), Options.Separator.ToString());
        }


        /// <summary>
        /// Verifies deserialisation when user and file use different separators
        /// </summary>
        [Fact]
        public void TestSepartorConflicting()
        {
            var Reader = StringToStreamReader("sep=,\nField1,Field2\n\"Hello\",\"World\"");
            var Options = new CsvDeserializationOptions { Separator = ';', UseTextQualifier = true };
            var Result = CsvSerializer.Deserialize<HasFields>(Reader, Options);
            Assert.Single(Result);
            Assert.Equal("Hello", Result.First().Field1);
            Assert.Equal("World", Result.First().Field2);
            Assert.Equal(','.ToString(), Options.Separator.ToString());
        }
    }
 }



