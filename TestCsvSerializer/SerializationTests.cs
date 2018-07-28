namespace Adrichem.Serialization.CsvSerializer.TestCsvSerializer
{
    using Xunit;
    using Adrichem.Serialization.CsvSerializer;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System;

    public class SerializationTests
    {

        private IList<HasProperties> PropertyBasedData = new List<HasProperties> {
                new HasProperties { Prop1 = "Hello", Prop2="World"},
                new HasProperties { Prop1 = "Good to", Prop2="See you"},
            };

        [Fact]
        public void TestHeaderDisabled()
        {
            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, PropertyBasedData, new CsvSerializationOptions
            {
                UseHeader = false
            });
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Hello,World(\r\n|\n)Good to,See you";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestHeaderEnabledByDefault()
        {
            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);
            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Prop1,Prop2(\r\n|\n)Hello,World(\r\n|\n)Good to,See you";
            Assert.Matches(Expected, Actual);
        }
 
        [Fact]
        public void TestQualifierEnabled()
        {

            var SerializationOptions = new CsvSerializationOptions
            {
                UseTextQualifier = true,
            };

            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, PropertyBasedData, SerializationOptions);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Prop1,Prop2(\r\n|\n)\"Hello\",\"World\"(\r\n|\n)\"Good to\",\"See you\"";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestLineNumbersEnabled()
        {

            var SerializationOptions = new CsvSerializationOptions
            {
                UseRowNumbers = true,
                RowNumberColumnTitle = "#",
            };

            
            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, PropertyBasedData, SerializationOptions);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "#,Prop1,Prop2(\r\n|\n)1,Hello,World(\r\n|\n)2,Good to,See you";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestSeparator()
        {
            var SerializationOptions = new CsvSerializationOptions
            {
                Separator = ';',
            };

            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, PropertyBasedData, SerializationOptions);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "sep=;(\r\n|\n)Prop1;Prop2(\r\n|\n)Hello;World(\r\n|\n)Good to;See you";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestCulture()
        {
            var SerializationOptions = new CsvSerializationOptions
            {
                Separator = ';',
                Culture = CultureInfo.GetCultureInfo("nl-nl")
            };

            var Data = new List<HasLocalizable>
            {
                new HasLocalizable { Double = 1.1, Float=2.2F, Date = new DateTime(2018,12,23)},
            };

            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, Data, SerializationOptions);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1,1;2,2;23-12-2018";
            Assert.Matches(Expected, Actual);



            SerializationOptions.Culture = CultureInfo.GetCultureInfo("en-us");
            Output = new MemoryStream();
            CsvSerializer.Serialize(Output, Data, SerializationOptions);
            Output.Seek(0, 0);

            Actual = new StreamReader(Output).ReadToEnd();
            Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1.1;2.2;12-23-2018";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestFields()
        {
            var Data = new List<HasFields>()
            {
                new HasFields { Field1 = "Hello", Field2 = "World", intField = 1, boolField = true}, 
                new HasFields { Field1 = "Good to", Field2 = "See you", intField = 2, boolField = false},
            };

            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, Data);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Field1,Field2,intField,boolField(\r\n|\n)Hello,World,1,True(\r\n|\n)Good to,See you,2,False";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestLocalizableFields()
        {

            var SerializationOptions = new CsvSerializationOptions
            {
                Separator = ';',
                Culture = CultureInfo.GetCultureInfo("nl-nl")
            };

            var Data = new List<HasLocalizableFields>
            {
                new HasLocalizableFields { Double = 1.1, Float=2.2F, Date = new DateTime(2018,12,23)},
            };

            var Output = new MemoryStream();
            CsvSerializer.Serialize(Output, Data, SerializationOptions);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1,1;2,2;23-12-2018";
            Assert.Matches(Expected, Actual);


            SerializationOptions.Culture = CultureInfo.GetCultureInfo("en-us");
            Output = new MemoryStream();
            CsvSerializer.Serialize(Output, Data, SerializationOptions);
            Output.Seek(0, 0);

            Actual = new StreamReader(Output).ReadToEnd();
            Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1.1;2.2;12-23-2018";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestInvalidSerializeInvalidObject()
        {
             Assert.Throws<ArgumentException>(() => CsvSerializer.Serialize(new MemoryStream(), new HasProperties()));
        }
    }

}

