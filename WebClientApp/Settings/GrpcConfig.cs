namespace WebClientApp.Settings
{
    // GrpcConfig.cs
    public class GrpcConfig
    {
        public string ServerUrl { get; set; } = "https://localhost:5557";//туть поменяц
        public int TimeoutSeconds { get; set; } = 30;
        public bool UseHttps { get; set; } = true;

        // Дополнительные параметры при необходимости
        public int MaxRetryAttempts { get; set; } = 3;
        public string AuthToken { get; set; } = string.Empty;
    }
}
