using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Amazon.Lambda.APIGatewayEvents;

namespace Authorization
{
    public static class Utils
    {
        public static string GetToken(APIGatewayProxyRequest request)
        {
            var bearer = request.Headers["Authorization"];
            var token = bearer.Split(" ")[1];
            return token;
        }

         public static string GetUserId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            // var userId = jwtToken.Payload.Sub;
            var userId = jwtToken.Claims.First(x => x.Type == "sub").Value;
            return userId;

        }
    }
}