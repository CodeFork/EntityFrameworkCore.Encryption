namespace EntityFrameworkCore.Encryption.Options
{
    public class EncryptionOptions
    {
        public string Key { get; set; }
        
        public string InitializationVector { get; set; }
    }
}