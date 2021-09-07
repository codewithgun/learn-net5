namespace Catalog.Settings
{
    public class MongoDbSettings : DbSettings
    {
        public override string ConnectionString
        {
            get
            {
                return $"mongodb://{this.Username}:{this.Password}@{this.Host}:{this.Port}";
            }
        }
    }
}