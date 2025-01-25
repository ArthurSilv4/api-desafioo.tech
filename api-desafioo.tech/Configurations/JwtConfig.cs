namespace api_desafioo.tech.Configurations
{
    public static class JwtConfig
    {
        public static string PrivateKey { get; private set; } = string.Empty;
        public static string Issuer { get; private set; } = string.Empty;
        public static string Audience { get; private set; } = string.Empty;
        public static int ExpirationInHours { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            PrivateKey = configuration["Jwt:PrivateKey"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt:PrivateKey is missing");
            Issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt:Issuer is missing");
            Audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt:Audience is missing");
            ExpirationInHours = int.Parse(configuration["Jwt:ExpirationInHours"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt:ExpirationInHours is missing"));
        }
    }
}
