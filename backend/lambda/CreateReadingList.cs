using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using System.Collections.Generic;
using Models;
using System.Text.Json;
using Authorization;
using BusineesLogic;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ServerlessFunctions
{
    public class CreateReadingList
    {

      public CreateReadingList(){}
      public APIGatewayProxyResponse Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            BookRequest newBook =  JsonSerializer.Deserialize<BookRequest>(request.Body, _options);
            var token = Utils.GetToken(request);
            context.Logger.LogLine("Token: " + token);
            var userId = Utils.GetUserId(token);
            BookItem bookItem = new ReadingList().CreateReadingList(userId, newBook);
            context.Logger.LogLine("book : " + bookItem + " " + JsonSerializer.Serialize(bookItem));

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.Created,
                Body = JsonSerializer.Serialize(new Dictionary<string, BookItem> { { "item", bookItem }}),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" }, {"Access-Control-Allow-Origin", "*"}, {"Access-Control-Allow-Credentials", "true"} }
            };

            return response;
        }
      }  
    }

   

