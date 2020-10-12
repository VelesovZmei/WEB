using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WEB.JWT
{
    public class JwtOptions
    {
        public const string ISSUER = "MyAuthenticationServer"; // издатель токена
        public const string AUDIENCE = "WEB"; // потребитель токена
        const string KEY = @"Y3y+3u9PfJQ?y4%_gUYPcP=ALCy@n$FH";   // ключ для шифрации
        public const int LIFETIME = 15; // время жизни токена - 1 минута

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
