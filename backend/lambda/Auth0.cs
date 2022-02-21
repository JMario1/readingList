using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Authorization;

namespace ServerlessFunctions
{
    public class Auth0
    {
        public Auth0(){}

        public async Task<APIGatewayCustomAuthorizerResponse> HandlerAsync(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");

            try
            {
                //var token = Utils.GetToken(request);
                var token = request.AuthorizationToken.Split(" ")[1];
                context.Logger.LogLine("Token "+ token);
                var jwtToken = await new JwtToken().VerifyTokenAsync(token);
                context.Logger.LogLine("jwt: " + jwtToken.ToString());

            
                return new APIGatewayCustomAuthorizerResponse
                {
                    PrincipalID = Utils.GetUserId(token),
                    PolicyDocument = new APIGatewayCustomAuthorizerPolicy
                    {
                        Version = "2012-10-17",
                        Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>()
                        {
                            new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                            {
                                Action = new HashSet<string>(){"execute-api:Invoke"},
                                Effect = jwtToken != null ? "Allow" : "Deny",
                                Resource = new HashSet<string>(){  "***" }
                            }
                        }
                    }
                };


            }
            catch (System.Exception)
            {
                
                throw;
            }
        } 
    }
}