namespace adc2024.tokenprovider.Code;

public interface ITokenService
{
    string CreateToken(string UserId, string Password);

    // Task<string> CreateTokenAsync(string UserId, string Password, CancellationToken CancellationToken = default);
}