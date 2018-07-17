namespace Adrichem.Serialization.CsvSerializer
{
    public class CsvDisplayHeaderAttribute : System.Attribute
    {
        public string DisplayName { get; set; }

        public int Order { get; set; }

    }
}
