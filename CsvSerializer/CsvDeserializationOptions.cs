namespace Adrichem.Serialization.CsvSerializer
{
    /// <summary>
    /// Options for deserialization of data from CSV.
    /// </summary>
    public class CsvDeserializationOptions : CsvOptions
    {
        /// <summary>
        /// Ignore empty lines during deserialization of data from a CSV.
        /// </summary>
        public bool IgnoreEmptyLines { get; set; } = true;

    }
}
