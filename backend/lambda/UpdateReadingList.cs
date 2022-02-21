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
    public class UpdateReadingList
    {
       public UpdateReadingList(){}

        public APIGatewayProxyResponse Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");

            var bookId = request.PathParameters["bookId"];
            var _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            BookRequest newBook =  JsonSerializer.Deserialize<BookRequest>(request.Body, _options);
            var token = Utils.GetToken(request);
            context.Logger.LogLine("Token: " + token);
            var userId = Utils.GetUserId(token);
            BookItem bookItem = new ReadingList().UpdateReadingList(newBook, userId, bookId);
            context.Logger.LogLine("book : " + bookItem);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(bookItem),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, {"Access-Control-Allow-Origin", "*"}, {"Access-Control-Allow-Credentials", "true"} }
            };


            return response;
        } 
    }
}