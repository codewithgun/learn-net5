namespace Catalog.Settings
{
    public abstract class DbSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public abstract string ConnectionString { get; }
    }
}