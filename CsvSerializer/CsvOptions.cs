namespace Adrichem.Serialization.CsvSerializer
{
    using System.Globalization;

    /// <summary>
    /// Options for serialization and deserialization of CSV data.
    /// </summary>
    public class CsvOptions
    {

        /// <summary>
        /// The culture for serializing or deserializing data.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Ignore members that are a reference type other than string.
        /// </summary>
        public bool IgnoreReferenceTypesExceptString { get; set; } = true;

        /// <summary>
        /// The separator between each field in a CSV.
        /// </summary>
        public char Separator { get; set; } = ',';

        /// <summary>
        /// Name of column with row numbers.
        /// </summary>
        public string RowNumberColumnTitle { get; set; } = "RowNumber";

        /// <summary>
        /// CSV contains a column with row numbers in the CSV.
        /// </summary>
        public bool UseRowNumbers { get; set; } = false;

        /// <summary>
        /// Values in the CSV are surrounded with doublequotes
        /// </summary>
        public bool UseTextQualifier { get; set; }

    }
}
