using System.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Authorization
{
    class JwtToken
    {

        public async Task<JwtSecurityToken> VerifyTokenAsync(string token)
        {
            if (token == null) 
                return null;
            var openIdUrl = "https://dev-r-jw-92j.us.auth0.com/.well-known/openid-configuration";
            OpenIdConnectConfiguration openIdConfig = await  OpenIdConnectConfigurationRetriever.GetAsync(openIdUrl, CancellationToken.None);
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenParameter = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = openIdConfig.SigningKeys.FirstOrDefault(),
                    ValidIssuer = "https://dev-r-jw-92j.us.auth0.com/",
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, tokenParameter, out SecurityToken validatedToken);

                var jwtToken = validatedToken;

                return (JwtSecurityToken)jwtToken;
        }

    }

}