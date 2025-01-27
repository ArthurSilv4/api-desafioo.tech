namespace api_desafioo.tech.Configurations
{
    public static class SmtpConfig
    {
        public static string Host { get; private set; } = string.Empty;
        public static int Port { get; private set; } = 0;
        public static string Username { get; private set; } = string.Empty;
        public static string Name { get; private set; } = string.Empty;
        public static string Password { get; private set; } = string.Empty;
        public static bool EnableSsl { get; private set; } = false;

        public static void Initialize(IConfiguration configuration)
        {
            Host = configuration["Smtp:Host"] ?? throw new ArgumentNullException(nameof(configuration), "Smtp:Host is missing");
            Port = int.Parse(configuration["Smtp:Port"] ?? throw new ArgumentNullException(nameof(configuration), "Smtp:Port is missing"));
            Username = configuration["Smtp:Username"] ?? throw new ArgumentNullException(nameof(configuration), "Smtp:Username is missing");
            Name = configuration["Smtp:Name"] ?? throw new ArgumentNullException(nameof(configuration), "Smtp:Name is missing");
            Password = configuration["Smtp:Password"] ?? throw new ArgumentNullException(nameof(configuration), "Smtp:Password is missing");
            EnableSsl = bool.Parse(configuration["Smtp:EnableSsl"] ?? throw new ArgumentNullException(nameof(configuration), "Smtp:EnableSsl is missing"));
        }
    }
}
