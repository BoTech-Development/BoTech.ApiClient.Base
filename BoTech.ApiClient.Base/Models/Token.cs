namespace BoTech.ApiClient.Base.Models;

public class Token
{
    public string TokenType { get; init; } = "Bearer";
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    /// <summary>
    /// The timespan in seconds
    /// </summary>
    public int  ExpiresIn { get; init; }
    public DateTime CreatedAt { get; init; }
   

    public Token()
    {
        CreatedAt = DateTime.UtcNow;
    }
    public bool IsEmpty() => TokenType == string.Empty || AccessToken == string.Empty || RefreshToken == string.Empty || ExpiresIn == 0;
    public bool IsExpired() => DateTime.UtcNow > CreatedAt + TimeSpan.FromSeconds(ExpiresIn);
    
}