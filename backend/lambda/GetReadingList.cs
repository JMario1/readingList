using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Authorization;
using BusineesLogic;
using Models;

namespace ServerlessFunctions
{
    public class GetReadingList
    {
       public GetReadingList(){}

        public APIGatewayProxyResponse Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");

             var token = Utils.GetToken(request);
            context.Logger.LogLine("Token: " + token);
            var userId = Utils.GetUserId(token);
            context.Logger.LogLine("user: " + userId);
            List<BookItem> books = new ReadingList().GetReadingList(userId);
            context.Logger.LogLine("book : " + books + " " + JsonSerializer.Serialize(books));

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(new Dictionary<string, List<BookItem>> {{"items", books}}),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, {"Access-Control-Allow-Origin", "*"}, {"Access-Control-Allow-Credentials", "true"} }
            };


            return response;
        } 
    }
}