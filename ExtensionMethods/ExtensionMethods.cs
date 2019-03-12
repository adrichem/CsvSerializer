namespace Adrichem.Serialization.CsvSerializer.ExtensionMethods
{
    using System.Text;
    using System.IO;
    using Adrichem.Serialization.CsvSerializer;
    using System.Collections.Generic;

    public static class ExtensionMethods
    {

        /// <summary>
        /// Serializes to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Input">The collection to serialize.</param>
        /// <param name="Path">The output path.</param>
        /// <param name="Encoding">The output Encoding.</param>
        /// <returns> <paramref name="Input"/></returns>
        public static IEnumerable<T> CsvSerialize<T>(this IEnumerable<T> Input, string Path, Encoding Encoding)
        {
            using (var Writer = new StreamWriter(new FileStream(Path, FileMode.OpenOrCreate), Encoding))
            {
                CsvSerializer.Serialize(Writer, Input);
                return Input;
            }
        }

        /// <summary>
        /// Serializes to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Input">The collection to serialize.</param>
        /// <param name="Path">The output path.</param>
        /// <param name="Encoding">The output Encoding.</param>
        /// <param name="Options">The options for serialization.</param>
        /// <returns> <paramref name="Input"/></returns>
        public static IEnumerable<T> CsvSerialize<T>(this IEnumerable<T> Input
            , string Path
            , Encoding Encoding
            , CsvSerializationOptions Options)
        {
            using (var Writer = new StreamWriter(new FileStream(Path, FileMode.OpenOrCreate), Encoding))
            {
                CsvSerializer.Serialize(Writer, Input, Options);
                return Input;
            }
        }
    }
}
