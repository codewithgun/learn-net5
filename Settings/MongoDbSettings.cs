namespace Catalog.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"mongodb://{this.Username}:{this.Password}@{this.Host}:{this.Port}";
            }
        }
    }
}