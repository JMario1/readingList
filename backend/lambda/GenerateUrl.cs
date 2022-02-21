using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Authorization;
using BusineesLogic;

namespace ServerlessFunctions
{
    public class GenerateUrl
    {
       public GenerateUrl(){}

        public APIGatewayProxyResponse Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var bookId = request.PathParameters["bookId"];
             var token = Utils.GetToken(request);
            context.Logger.LogLine("Token: " + token);
            var userId = Utils.GetUserId(token);
            string presignedUrl = new ReadingList().GenerateUrl(userId, bookId);
            var uploadUrl = new Dictionary<string, string>{{"uploadUrl", presignedUrl}};
            context.Logger.LogLine("url : " + presignedUrl);

            
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(uploadUrl),
                Headers = new Dictionary<string, string> { {"Access-Control-Allow-Origin", "*"}, {"Access-Control-Allow-Credentials", "true"} }
            };


            return response;
        } 
    }
}