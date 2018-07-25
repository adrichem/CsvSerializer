namespace Adrichem.Serialization.CsvSerializer
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.ComponentModel;
    using System.Globalization;


    /// <summary>
    ///     Serialize and Deserialize Enumerables of any object type to CSV.
    /// </summary>
    public class CsvSerializer<T> : IFormatter where T : class, new()
    {
        #region Fields
        private IList<MemberInfo> Members;
        #endregion

        #region Properties

        public bool IgnoreEmptyLines { get; set; } = true;

        public bool IgnoreReferenceTypesExceptString { get; set; } = true;

        public string NewlineReplacement { get; set; } = ((char)0x254).ToString();
        public string Replacement { get; set; } = ((char)0x255).ToString();

        public string RowNumberColumnTitle { get; set; } = "RowNumber";

        public char Separator { get; set; } = ',';
        public bool UseEofLiteral { get; set; }

        public bool UseLineNumbers { get; set; } = true;
        public bool UseTextQualifier { get; set; }

        public bool UseHeader { get; set; }

        /// <summary>
        /// The culture to use for numbers
        /// </summary>
        public CultureInfo Culture { get; set; }

          #endregion

        #region Constructors

        /// <summary>
        ///     Csv Serializer
        ///     Initialize by selected properties from the type to be de/serialized
        /// </summary>
        public CsvSerializer()
        {
            UseTextQualifier = false;
            UseEofLiteral = false;
            UseLineNumbers = false;
            UseHeader = true;
            Culture = CultureInfo.CurrentCulture;

            var type = typeof(T);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField).ToList<MemberInfo>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty).ToList<MemberInfo>();

            Members = fields.Union(properties).Where(m => m.GetCustomAttribute<CsvIgnoreAttribute>() == null).ToList();

            if (IgnoreReferenceTypesExceptString)
            {
                Members = Members.Where( a => 
                    {
                        if (a is PropertyInfo &&  ((a as PropertyInfo).PropertyType.IsValueType || (a as PropertyInfo).PropertyType.Name == "String"))
                        {
                            return true;
                        }
                        else if (a is FieldInfo && ((a as FieldInfo).FieldType.IsValueType || (a as FieldInfo).FieldType.Name == "String"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    } )
                    .ToList();
            }
         
        }

        #endregion

        #region IFormatter Members

        /// <summary>
        ///     Gets or sets the <see cref="T:System.Runtime.Serialization.SerializationBinder" /> that performs type lookups
        ///     during deserialization.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Runtime.Serialization.SerializationBinder" /> that performs type lookups during
        ///     deserialization.
        /// </returns>
        public SerializationBinder Binder { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext" /> used for serialization and
        ///     deserialization.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> used for serialization and deserialization.
        /// </returns>
        public StreamingContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="T:System.Runtime.Serialization.SurrogateSelector" /> used by the current formatter.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:System.Runtime.Serialization.SurrogateSelector" /> used by this formatter.
        /// </returns>
        public ISurrogateSelector SurrogateSelector { get; set; }

        /// <summary>
        ///     Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <returns>
        ///     An object that can be cast to a List of type T
        /// </returns>
        /// <param name="serializationStream"> The stream that contains the data to deserialize. </param>
        public object Deserialize(Stream serializationStream)
        {
            string[] columns;
            string[] rows;

            try
            {
                using (var sr = new StreamReader(serializationStream))
                {
                    columns = sr.ReadLine().Split(Separator);
                    rows = sr.ReadToEnd().Split(new string[]
                    {
                        Environment.NewLine
                    }, StringSplitOptions.None);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCsvFormatException(
                    "The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }

            var data = new List<T>();

            for (var row = 0; row < rows.Length; row++)
            {
                var line = rows[row];

                if (IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (!IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                {
                    throw new InvalidCsvFormatException(string.Format(@"Error: Empty line at line number: {0}", row));
                }

                var parts = line.Split(Separator);

                var firstColumnIndex = UseLineNumbers ? 2 : 1;
                if (parts.Length == firstColumnIndex && parts[firstColumnIndex - 1] != null
                   && parts[firstColumnIndex - 1] == "EOF")
                {
                    break;
                }

                var datum = new T();

                var start = UseLineNumbers ? 1 : 0;
                for (var i = start; i < parts.Length; i++)
                {

                    string value;
                    string column;
                    try
                    {
                        value = parts[i];
                        column = columns[i];
                    } 
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidCsvFormatException(string.Format(@"Error: on line: {0}", row));
                    }

                    // continue of deviant RowNumber column condition
                    // this allows for the deserializer to implicitly ignore the RowNumber column
                    if (column.Equals(RowNumberColumnTitle) && !Members.Any(a => a.Name.Equals(RowNumberColumnTitle)))
                    {
                        continue;
                    }

                    value =
                        value.Replace(Replacement, Separator.ToString())
                            .Replace(NewlineReplacement, Environment.NewLine)
                            .Trim();

                    var p =
                        Members.FirstOrDefault(
                            a => a.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase));

                    // ignore property csv column, Property not found on targing type
                    if (p == null)
                    {
                        continue;
                    }

                    if (UseTextQualifier)
                    {
                        if (value.IndexOf("\"") == 0)
                        {
                            value = value.Substring(1);
                        }

                        if (value[value.Length - 1].ToString() == "\"")
                        {
                            value = value.Substring(0, value.Length - 1);
                        }
                    }

                    if (p is PropertyInfo)
                    {
                        
                        var converter = TypeDescriptor.GetConverter((p as PropertyInfo).PropertyType);
                        var convertedvalue = converter.ConvertFromString(null, Culture, value);
                        (p as PropertyInfo).SetValue(datum, convertedvalue);
                    } 
                    else if ( p is FieldInfo)
                    {
                        var converter = TypeDescriptor.GetConverter((p as FieldInfo).FieldType);
                        var convertedvalue = converter.ConvertFromString(null, Culture, value);
                        (p as FieldInfo).SetValue(datum, convertedvalue);
                    }
                }

                data.Add(datum);
            }

            return data;
        }

        /// <summary>
        ///     Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <param name="stream">
        ///     The output stream to searialize to.
        /// </param>
        /// <param name="graph">
        ///     The object, or root of the object graph, to serialize as IEnumerable of type T. 
        /// </param>
        public void Serialize(Stream stream, object graph)
        {
            IEnumerable<T> data = null;

            if (graph is IEnumerable<T>)
            {
                data = graph as IEnumerable<T>;
            }
            else
            {
                throw new ArgumentException(string.Format("does not implement IEnumerable<{0}>", typeof(T).Name), nameof(graph));
            }
            var sb = new StringBuilder();
            var values = new List<string>();

            // If the separator value is not a standard comma, then add this directive to the top
            // of the file, this way Excel can tell what is separating the csv values.
            if (Separator != ',')
            {
                sb.AppendLine("sep=" + Separator);
            }


            if (UseHeader)
            { 
                sb.AppendLine(GetHeader());
            }

            var row = 1;
            foreach (var item in data)
            {
                values.Clear();

                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }

                foreach (var M in Members)
                {
                    string convertedvalue = null;

                    if (M is PropertyInfo)
                    {
                        var converter = TypeDescriptor.GetConverter((M as PropertyInfo).PropertyType);
                        convertedvalue = converter.ConvertToString(null, Culture, (M as PropertyInfo).GetValue(item));
                    } 
                    else if( M is FieldInfo)
                    {
                        var converter = TypeDescriptor.GetConverter((M as FieldInfo).FieldType);
                        convertedvalue = converter.ConvertToString(null, Culture, (M as FieldInfo).GetValue(item));
                    }
                    else
                    {
                        continue;
                    }
                       

                    var value = convertedvalue == null
                        ? ""
                        : convertedvalue
                            .Replace(Separator.ToString(), Replacement)
                            .Replace(Environment.NewLine, NewlineReplacement);

                    if (UseTextQualifier)
                    {
                        value = string.Format("\"{0}\"", value);
                    }

                    values.Add(value);
                }
                sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }

            if (UseEofLiteral)
            {
                values.Clear();

                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }

                values.Add("EOF");

                sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }

            using (var sw = new StreamWriter(stream,Encoding.UTF8, 1024,true))
            {
                sw.Write(sb.ToString().Trim());
            }
        }

        #endregion

        #region private

        /// <summary>
        ///     Get Header
        /// </summary>
        /// <returns> </returns>
        private string GetHeader()
        {
            var csvDisplayHeaderAttributes = new List<CsvDisplayHeaderAttribute>();
            foreach (var Member in Members)
            {
                var attribute = (CsvDisplayHeaderAttribute)Member.GetCustomAttributes(typeof(CsvDisplayHeaderAttribute), false).FirstOrDefault();
                csvDisplayHeaderAttributes.Add(new CsvDisplayHeaderAttribute
                {
                    DisplayName = attribute == null ? Member.Name : string.IsNullOrEmpty(attribute.DisplayName) ? Member.Name : attribute.DisplayName,
                    Order = attribute == null ? int.MaxValue : attribute.Order
                });

            }
            var header = csvDisplayHeaderAttributes.OrderBy(x => x.Order).Select(x => x.DisplayName);

            if (UseLineNumbers)
            {
                header = new string[]
                {
                    RowNumberColumnTitle
                }.Union(header);
            }

            return string.Join(Separator.ToString(), header.ToArray());
        }
        #endregion
    }
}
