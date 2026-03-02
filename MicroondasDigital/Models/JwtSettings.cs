namespace MicroondasDigital.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = "MicroondasSuperSecretKey1234567890+"; // 256 bits
    public string Issuer { get; set; } = "MicroondasDigital.API";
    public string Audience { get; set; } = "MicroondasDigital.Client";
    public int ExpiryMinutes { get; set; } = 60;
}
