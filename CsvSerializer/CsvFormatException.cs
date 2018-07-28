namespace Adrichem.Serialization.CsvSerializer
{
    using System;
    public class CsvFormatException : Exception
    {
        public CsvFormatException(string message) : base(message) { }

        public CsvFormatException(string message, Exception ex) : base(message, ex) { }

     }
}
