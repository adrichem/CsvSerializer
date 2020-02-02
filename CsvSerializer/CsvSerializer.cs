namespace Adrichem.Serialization.CsvSerializer
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Serialize and deserialize CSV data.
    /// </summary>
    public class CsvSerializer
    {

        /// <summary>
        ///     Serializes to a CSV using default serialization options.
        /// </summary>
        /// <param name="Writer">
        ///     The writer to serialize to.
        /// </param>
        /// <param name="objects">
        ///     An IEnumerable of objects. 
        /// </param>
        public static void Serialize(StreamWriter Writer, object objects)
        {
            Serialize(Writer, objects, new CsvSerializationOptions());
        }

        /// <summary>
        ///     Serializes to a CSV.
        /// </summary>
        /// <param name="Writer">
        ///     The writer to serialize to.
        /// </param>
        /// <param name="objects">
        ///     An IEnumerable of objects. 
        /// </param>
        /// <param name="SerializationOptions">
        ///     Options for serialization. 
        /// </param>
        public static void Serialize(StreamWriter Writer, object objects, CsvSerializationOptions SerializationOptions)
        {
            IEnumerable<object> data = null;

            if (objects is IEnumerable<object>)
            {
                data = objects as IEnumerable<object>;
            }
            else
            {
                throw new ArgumentException("Is not an IEnumerable<...>");
            }
            var builder = new StringBuilder();
            var values = new List<string>();

            // tell Excel what separator is being used.
            if (SerializationOptions.Separator != ',')
            {
                builder.AppendLine("sep=" + SerializationOptions.Separator);
            }

            //Figure out what the members are of the objects in the enumerable.
            IEnumerable<MemberInfo> Members = null;
            foreach (var item in data)
            {
                if (null == Members)
                {
                    Members = GetMembers(item.GetType(), SerializationOptions);
                }
                if (SerializationOptions.UseHeader)
                {
                    var header = Members.Select(m => m.Name);
                    if (SerializationOptions.UseRowNumbers)
                    {
                        header = new string[]
                        {
                    SerializationOptions.RowNumberColumnTitle
                        }.Union(header);
                    }

                    builder.AppendLine(string.Join(SerializationOptions.Separator.ToString(), header.ToArray()));
                }
                break;
            }


            //Serialize each object to a row
            var row = 1;
            foreach (var item in data)
            {

                values.Clear();

                if (SerializationOptions.UseRowNumbers)
                {
                    values.Add(row.ToString());
                    row++;
                }

                foreach (var M in Members)
                {
                    string ConvertedValue = null;

                    if (M is PropertyInfo)
                    {
                        ConvertedValue = TypeDescriptor
                            .GetConverter((M as PropertyInfo).PropertyType)
                            .ConvertToString(null, SerializationOptions.Culture, (M as PropertyInfo).GetValue(item));
                    }
                    else if (M is FieldInfo)
                    {
                        ConvertedValue = TypeDescriptor
                            .GetConverter((M as FieldInfo).FieldType)
                            .ConvertToString(null, SerializationOptions.Culture, (M as FieldInfo).GetValue(item));
                    }
                    else
                    {
                        continue;
                    }


                    var value = ConvertedValue == null ? "" : ConvertedValue;

                    if (SerializationOptions.UseTextQualifier)
                    {
                        value = string.Format("\"{0}\"", value);
                    }

                    values.Add(value);
                }
                builder.AppendLine(string.Join(SerializationOptions.Separator.ToString(), values.ToArray()));
            }

            Writer.Write(builder.ToString().Trim());
        }


        /// <summary>
        ///     Deserializes data from a CSV using default serialization options.
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type T
        /// </returns>
        /// <param name="Input">Stream with CSV data.</param>
        public static IEnumerable<T> Deserialize<T>(StreamReader Input) where T : class, new()
        {
            return Deserialize<T>(Input, new CsvDeserializationOptions());
        }

        /// <summary>
        ///     Deserializes data from a CSV
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type T
        /// </returns>
        /// <param name="Reader">Reader with CSV data.</param>
        /// <param name="DeserializationOptions"> Options for deserialization.</param>
        public static IEnumerable<T> Deserialize<T>(StreamReader Reader, CsvDeserializationOptions DeserializationOptions) where T : class, new()
        {
            
            string[] Columns;
            string[] Rows;
            try
            {
               string FirstLine = Reader.ReadLine();
               string HeaderLine;
               if( Regex.IsMatch(FirstLine, "^sep=(.)$", RegexOptions.IgnoreCase))
               {
                    DeserializationOptions.Separator = FirstLine.Last();
                    HeaderLine = Reader.ReadLine();
               }
               else
                {
                    HeaderLine = FirstLine;
                }


                Columns = HeaderLine.Split(DeserializationOptions.Separator);

                Rows = Reader
                    .ReadToEnd()
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                ;
            }
            catch (Exception ex)
            {
                throw new CsvFormatException("Invalid CSV file.", ex);
            }

            var Members = GetMembers(typeof(T), DeserializationOptions);

            var Data = new List<T>();

            for (var Row = 0; Row < Rows.Length; Row++)
            {
                var Line = Rows[Row];

                if (DeserializationOptions.IgnoreEmptyLines && string.IsNullOrWhiteSpace(Line))
                {
                    continue;
                }

                if (!DeserializationOptions.IgnoreEmptyLines && string.IsNullOrWhiteSpace(Line))
                {
                    throw new CsvFormatException(string.Format("Empty line at line number: {0}", Row));
                }

                var Parts = Line.Split(DeserializationOptions.Separator);

                var firstColumnIndex = DeserializationOptions.UseRowNumbers ? 1 : 0;
                if (Parts.Length == firstColumnIndex + 1 && Parts[firstColumnIndex] != null
                   && Parts[firstColumnIndex] == "EOF")
                {
                    break;
                }

                var Item = new T();

                var Start = DeserializationOptions.UseRowNumbers ? 1 : 0;
                for (var i = Start; i < Parts.Length; i++)
                {
                    string Value;
                    string Column;
                    try
                    {
                        Value = Parts[i].Trim();
                        Column = Columns[i];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new CsvFormatException(string.Format(@"Error: on line: {0}", Row));
                    }

                    // Ignore the rownumber column, unless T has such a member.
                    if (Column.Equals(DeserializationOptions.RowNumberColumnTitle) &&
                        !Members.Any(a => a.Name.Equals(DeserializationOptions.RowNumberColumnTitle)))
                    {
                        continue;
                    }
                    var p = Members.FirstOrDefault(a => a.Name.Equals(Column, StringComparison.InvariantCultureIgnoreCase));

                    // Member not founnd on T, ignore column.
                    if (p == null)
                    {
                        continue;
                    }

                    if (DeserializationOptions.UseTextQualifier)
                    {
                        if (Value.IndexOf("\"") == 0)
                        {
                            Value = Value.Substring(1);
                        }

                        if (Value[Value.Length - 1].ToString() == "\"")
                        {
                            Value = Value.Substring(0, Value.Length - 1);
                        }
                    }

                    if (p is PropertyInfo)
                    {
                        var converter = TypeDescriptor.GetConverter((p as PropertyInfo).PropertyType);
                        var convertedvalue = converter.ConvertFromString(null, DeserializationOptions.Culture, Value);
                        (p as PropertyInfo).SetValue(Item, convertedvalue);
                    }
                    else if (p is FieldInfo)
                    {
                        var converter = TypeDescriptor.GetConverter((p as FieldInfo).FieldType);
                        var convertedvalue = converter.ConvertFromString(null, DeserializationOptions.Culture, Value);
                        (p as FieldInfo).SetValue(Item, convertedvalue);
                    }
                    else
                    {
                        continue;
                    }
                }

                Data.Add(Item);
            }

            return Data;
        }

        private static IEnumerable<MemberInfo> GetMembers(Type type, CsvOptions Options)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField).ToList<MemberInfo>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty).ToList<MemberInfo>();

            IEnumerable<MemberInfo> Members = fields.Union(properties);

            if (Options.IgnoreReferenceTypesExceptString)
            {
                Members = Members.Where(a =>
                {
                    if (a is PropertyInfo)
                    {
                        var tmp = a as PropertyInfo;
                        return tmp.PropertyType.IsValueType || tmp.PropertyType.Name == "String";
                    }
                    else if (a is FieldInfo)
                    {
                        var tmp = a as FieldInfo;
                        return tmp.FieldType.IsValueType || tmp.FieldType.Name == "String";
                    }
                    else
                    {
                        return false;
                    }
                });
            }

            return Members;
        }
    }
}
