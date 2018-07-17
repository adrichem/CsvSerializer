namespace Adrichem.Serialization.CsvSerializer
{
    using System;
    public class InvalidCsvFormatException : Exception
    {
        #region Constructors

        /// <summary>
        ///     Invalid Csv Format Exception
        /// </summary>
        /// <param name="message"> message </param>
        public InvalidCsvFormatException(string message) : base(message) { }

        public InvalidCsvFormatException(string message, Exception ex) : base(message, ex) { }

        #endregion
    }
}
