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
        public void TestAllDisabled()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseHeader=false
            };
            var Output = new MemoryStream();
            Serializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();

            string Expected = "Hello,World(\r\n|\n)Good to,See you";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestHeaderEnabled()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseHeader = true
            };
            var Output = new MemoryStream();
            Serializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);
            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Prop1,Prop2(\r\n|\n)Hello,World(\r\n|\n)Good to,See you";
            Assert.Matches(Expected, Actual);
        }

 
        [Fact]
        public void TestQualifierEnabled()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseTextQualifier = true,
                UseHeader = true
            };
            var Output = new MemoryStream();
            Serializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Prop1,Prop2(\r\n|\n)\"Hello\",\"World\"(\r\n|\n)\"Good to\",\"See you\"";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestLineNumbersEnabled()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseLineNumbers = true,
                UseHeader = true,
                RowNumberColumnTitle="#",
            };
            
            var Output = new MemoryStream();
            Serializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "#,Prop1,Prop2(\r\n|\n)1,Hello,World(\r\n|\n)2,Good to,See you";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestEOFLiteralEnabled()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                UseEofLiteral=true
            };

            var Output = new MemoryStream();
            Serializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Prop1,Prop2(\r\n|\n)Hello,World(\r\n|\n)Good to,See you(\r\n|\n)EOF";
            Assert.Matches(Expected, Actual);
        }

        [Fact]
        public void TestSeparator()
        {
            var Serializer = new CsvSerializer<HasProperties>()
            {
                Separator = ';',
            };

            var Output = new MemoryStream();
            Serializer.Serialize(Output, PropertyBasedData);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "sep=;(\r\n|\n)Prop1;Prop2(\r\n|\n)Hello;World(\r\n|\n)Good to;See you";
            Assert.Matches(Expected, Actual);
        }


        [Fact]
        public void TestCulture()
        {
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
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1,1;2,2;23-12-2018";
            Assert.Matches(Expected, Actual);


            Serializer = new CsvSerializer<HasLocalizable>()
            {
                Separator = ';',
                Culture = CultureInfo.GetCultureInfo("en-us")

            };
            Output = new MemoryStream();
            Serializer.Serialize(Output, Data);
            Output.Seek(0, 0);

            Actual = new StreamReader(Output).ReadToEnd();
            Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1.1;2.2;12-23-2018";
            Assert.Matches(Expected, Actual);
        }


        [Fact]
        public void TestFields()
        {
            var Serializer = new CsvSerializer<HasFields>()
            {
                UseLineNumbers = false,
            };

            var Data = new List<HasFields>()
            {
                new HasFields { Field1 = "Hello", Field2 = "World", intField = 1, boolField = true}, 
                new HasFields { Field1 = "Good to", Field2 = "See you", intField = 2, boolField = false},
            };

            var Output = new MemoryStream();
            Serializer.Serialize(Output, Data);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "Field1,Field2,intField,boolField(\r\n|\n)Hello,World,1,True(\r\n|\n)Good to,See you,2,False";
            Assert.Matches(Expected, Actual);
        }


        [Fact]
        public void TestLocalizableFields()
        {
            var Serializer = new CsvSerializer<HasLocalizableFields>()
            {
                Separator = ';',
                Culture = CultureInfo.GetCultureInfo("nl-nl")

            };

            var Data = new List<HasLocalizableFields>
            {
                new HasLocalizableFields { Double = 1.1, Float=2.2F, Date = new DateTime(2018,12,23)},
            };

            var Output = new MemoryStream();
            Serializer.Serialize(Output, Data);
            Output.Seek(0, 0);

            var Actual = new StreamReader(Output).ReadToEnd();
            string Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1,1;2,2;23-12-2018";
            Assert.Matches(Expected, Actual);


            Serializer = new CsvSerializer<HasLocalizableFields>()
            {
                Separator = ';',
                Culture = CultureInfo.GetCultureInfo("en-us")

            };
            Output = new MemoryStream();
            Serializer.Serialize(Output, Data);
            Output.Seek(0, 0);

            Actual = new StreamReader(Output).ReadToEnd();
            Expected = "sep=;(\r\n|\n)Double;Float;Date(\r\n|\n)1.1;2.2;12-23-2018";
            Assert.Matches(Expected, Actual);
        }
    }

}

