using BoTech.ApiClient.Base.Models;

namespace BoTech.ApiClient.Base.Services;
/// <summary>
/// This class can handle the token of the user.
/// The api client can only handle one user at once.
/// That's why this token handler is implemented as a singleton.
/// </summary>
public class TokenHandler
{
    public static TokenHandler Instance
    {
        get
        {
            if (field == null)
                field = new TokenHandler();
            return field;
        }
    }
    /// <summary>
    /// The current token of the user
    /// </summary>
    private Token? _currentToken;
    /// <summary>
    /// The Email of the user where the tokens are associated to.
    /// </summary>
    public string EmailOrUserName { get; private set; } = string.Empty;
    private TokenHandler()
    {
        
    }    
    /// <summary>
    /// The new token is only saved if the current user (passed emailOrUserName variable) is the same as the previous user (old token). <br/>
    /// If no token is defined yet, the new user is set as the current user. <br/>
    /// The token is not saved if it is expired or empty.
    /// </summary>
    /// <param name="token">The new Token</param>
    /// <param name="emailOrUserName">The curren user, which is connected to the token.</param>
    public void SaveNewTokenForUserWithMail(Token token, string emailOrUserName)
    {
        if(EmailOrUserName == string.Empty) EmailOrUserName = emailOrUserName;
        if(!token.IsExpired() && !token.IsEmpty() && EmailOrUserName == emailOrUserName)
            _currentToken = token;
    }
    /// <summary>
    /// Log out if expired.
    /// </summary>
    /// <returns>Only provides the current token if it is not expired.</returns>
    public Token? GetCurrentTokenAndLogoutIfExpired()
    {
        if(_currentToken != null && !_currentToken.IsExpired())
            return _currentToken;
        else
            Logout();
        return null;
    }
    /// <summary>
    /// Just deletes the current token.
    /// </summary>
    public void Logout()
    {
        _currentToken = null;
    }
    
}