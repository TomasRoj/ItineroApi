using JWT.Algorithms;
using JWT.Builder;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class TokensService
    {
        const string PASSWORD = "foobarbaz";

        public string Create(LoginModel model)
        {
            return JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm())
                      .WithSecret(PASSWORD)
                      .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
                      .AddClaim("user", model.Username)
                      .Encode();
        }

        public bool Verify(string header)
        {
            try
            {
                if (header == null)
                {
                    return false;
                }

                string[] parts = header.Split(' ');

                if (parts.Length != 2)
                {
                    return false;
                }

                var payload = JwtBuilder.Create()
                            .WithAlgorithm(new HMACSHA256Algorithm())
                            .WithSecret(PASSWORD)
                            .MustVerifySignature()
                            .Decode<IDictionary<string, object>>(parts[1]);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
