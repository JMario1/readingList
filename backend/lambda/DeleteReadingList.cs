using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Authorization;
using BusineesLogic;

namespace ServerlessFunctions
{
    public class DeleteReadingList
    {
       public DeleteReadingList(){}

        public APIGatewayProxyResponse Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");

            var bookId = request.PathParameters["bookId"];
            var token = Utils.GetToken(request);
            context.Logger.LogLine("Token: " + token);
            var userId = Utils.GetUserId(token);
            new ReadingList().DeleteReadingList(userId, bookId);
            
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, {"Access-Control-Allow-Origin", "*"}, {"Access-Control-Allow-Credentials", "true"} }
            };


            return response;
        } 
    }
}