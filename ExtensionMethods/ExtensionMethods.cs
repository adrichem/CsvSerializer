namespace Adrichem.Serialization.CsvSerializer.ExtensionMethods
{
    using System.IO;
    using Adrichem.Serialization.CsvSerializer;
    using System.Collections.Generic;

    public static class ExtensionMethods
    {
        
        /// <summary>
        /// Serializes <paramref name="Input"/> into <paramref name="Output"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Input">The collection to serialize.</param>
        /// <param name="Output">The output stream.</param>
        /// <returns> <paramref name="Input"/></returns>
        public static IEnumerable<T> CsvSerialize<T>(this IEnumerable<T> Input, Stream Output)
        {
            CsvSerializer.Serialize(Output, Input);
            return Input;
        }

        /// <summary>
        /// Serializes <paramref name="Input"/> into <paramref name="Output"/> using the options defines in <paramref name="Options"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Input">The collection to serialize.</param>
        /// <param name="Output">The output stream.</param>
        /// <param name="Options">The options for serialization.</param>
        /// <returns> <paramref name="Input"/></returns>
        public static IEnumerable<T> CsvSerialize<T>(this IEnumerable<T> Input
            , Stream Output
            , CsvSerializationOptions Options)
        {
            CsvSerializer.Serialize(Output, Input, Options);
            return Input;
        }
    }
}
