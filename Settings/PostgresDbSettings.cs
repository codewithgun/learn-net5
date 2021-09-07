namespace Catalog.Settings
{
    public class PostgresDbSettings : DbSettings
    {

        public string Database { get; set; }
        public override string ConnectionString
        {
            get
            {
                return $"Host={this.Host};Username={this.Username};Password={this.Password};Database={this.Database}";
            }
        }
    }
}