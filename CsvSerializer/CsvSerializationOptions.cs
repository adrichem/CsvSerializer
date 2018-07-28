namespace Adrichem.Serialization.CsvSerializer
{

    /// <summary>
    /// Options for serialization to CSV.
    /// </summary>
    public class CsvSerializationOptions : CsvOptions
    {
        /// <summary>
        /// Include a header row in the CSV with the name of each column.
        /// </summary>
        public bool UseHeader { get; set; } = true;

    }
}
