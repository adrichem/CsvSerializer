namespace TestCsvSerializer
{
    using Xunit;
    using Adrichem.Serialization.CsvSerializer;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System;
    using System.Linq;
    using System.Text;

    public class DeserializationTests
    {

        private Stream StringToStream(string csvContent)
        {
            MemoryStream InputStream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            InputStream.Write(bytes, 0, bytes.Count());
            InputStream.Seek(0, 0);
            return InputStream;
        }

        [Fact]
        public void TestIncompleteLine()
        {
            var Input = StringToStream("#,Prop1,Prop2" + Environment.NewLine 
                + "1,A" + Environment.NewLine 
                + "2,C,D");
            var Serializer = new CsvSerializer<HasProperties>() { RowNumberColumnTitle = "#" };
            var Data = Serializer.Deserialize(Input) as IEnumerable<HasProperties>;

            Assert.True(Data.Count() == 2);
            Assert.Equal("A", Data.First().Prop1);
            Assert.Null(Data.First().Prop2);
            Assert.Equal("C", Data.Last().Prop1);
            Assert.Equal("D", Data.Last().Prop2);
        }

        [Fact]
        public void TestTooManyFieldsOnLine()
        {
            var Input = StringToStream("#,Prop1,Prop1" + Environment.NewLine
                + "1,A,B,C,D,E" );
            var Serializer = new CsvSerializer<HasLocalizable>();
            Assert.ThrowsAny<InvalidCsvFormatException>(() => Serializer.Deserialize(Input));
        }

        [Fact]
        public void TestEOF()
        {
            var Input = StringToStream(string.Empty);
            var Serializer = new CsvSerializer<HasLocalizable>();
            Assert.ThrowsAny<InvalidCsvFormatException>(() => Serializer.Deserialize(Input));
        }

        [Fact]
        public void TestLocalized()
        {
            string NLNLTest = "Double;Float;Date\n1,1;2,2;23-12-2018 00:00:00";
            string ENUSTest = "Double;Float;Date\n1.1;2.2;12/23/2018 12:00:00 AM";
            CsvSerializer<HasLocalizable> Serializer;

            Serializer = new CsvSerializer<HasLocalizable>()
            {
                Culture = CultureInfo.GetCultureInfo("en-us"),
                Separator=';'
            };
            var Input = StringToStream(ENUSTest);

            var Data = Serializer.Deserialize(Input) as IEnumerable<HasLocalizable>;
            Assert.Single(Data);
            Assert.Equal(1, Math.Floor(Data.First().Double));
            Assert.Equal(2, Math.Floor(Data.First().Float));
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
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseLineNumbers = true,
            };
            string tmp = string.Empty;
            tmp += "#,Prop1,Prop2" + Environment.NewLine;
            tmp += "1,A,B" + Environment.NewLine;
            tmp += "2,C,D" + Environment.NewLine;

            var Input = StringToStream(tmp);

            var Data = Serializer.Deserialize(Input) as IEnumerable<HasProperties>;
            Assert.True(Data.Count() == 2);

            Assert.Equal("A", Data.First().Prop1);
            Assert.Equal("B", Data.First().Prop2);
            Assert.Equal("C", Data.Last().Prop1);
            Assert.Equal("D", Data.Last().Prop2);

        }

        [Fact]
        public void TestDifferentOrder()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseLineNumbers = true,
            };
            string tmp = string.Empty;
            tmp += "#,Prop2,Prop1" + Environment.NewLine;
            tmp += "1,A,B" + Environment.NewLine;
            tmp += "2,C,D" + Environment.NewLine;

            var Input = StringToStream(tmp);

            var Data = Serializer.Deserialize(Input) as IEnumerable<HasProperties>;
            Assert.True(Data.Count() == 2);

            Assert.Equal("B", Data.First().Prop1);
            Assert.Equal("A", Data.First().Prop2);
            Assert.Equal("D", Data.Last().Prop1);
            Assert.Equal("C", Data.Last().Prop2);

        }


    }

}

