namespace Logic.Utils
{
    public sealed class CommandsConnectionString
    {
        public string Value { get; }

        public CommandsConnectionString(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Author  : Emmanuel Nuyttens
    /// Purpose : Holds the connection string to the sql read side database.
    /// Info    : The reason why we need to create this connection wrapper class
    ///           is because we can not use the ASP.NET dependency injection container to inject
    ///           raw strings; it has to be classes instead. 
    ///           This class will be registered as a Singleton in Startup.cs ConfigureServices of our API.
    ///                      
    /// 
    /// </summary>
    public sealed class QueriesConnectionString
    {
        public string Value { get; }

        public QueriesConnectionString(string value)
        {
            Value = value;
        }
    }
}
