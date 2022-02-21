using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace DataLayer
{   
    public class DataAccess
    {
         
        static DataAccess()
        {
            AWSSDKHandler.RegisterXRayForAllServices();
        }
    
        private static readonly string tableName = Environment.GetEnvironmentVariable("BOOKS_TABLE");
        private static readonly string bucketName = Environment.GetEnvironmentVariable("IMAGE_BUCKET");
        private static readonly AmazonDynamoDBClient docClient = new AmazonDynamoDBClient();
        private static readonly AmazonS3Client s3 = new AmazonS3Client();
        Table table = Table.LoadTable(docClient, tableName);
        public async Task<BookItem> CreateItemAsync(string userId, BookItem book)
        {   
    
            Document dbBook = new Document();
            dbBook["UserId"] = book.userId;
            dbBook["BookId"] = book.bookId;
            dbBook["BookName"] = book.bookName;
            dbBook["BookUrlCover"] = book.bookCoverUrl;
            dbBook["Author"] = book.author;
            dbBook["CreatedAt"] = book.createdAt;
            dbBook["CurrentPageNumber"] = book.currentPageNumber;
            dbBook["Done"] = new DynamoDBBool(book.done);

            await table.PutItemAsync(dbBook);
            return book;
        }
        public async Task<BookItem> UpdateItemAsync(BookRequest book, string userId, string bookId)
        {
            Document dbBook = new Document();
            
            if(book.bookName != null) dbBook["BookName"] = book.bookName;
            if(book.bookCoverUrl != null)dbBook["BookUrlCover"] = book.bookCoverUrl;
            if(book.author != null)dbBook["Author"] = book.author;
            if(book.currentPageNumber != null)dbBook["CurrentPageNumber"] = book.currentPageNumber;
            dbBook["Done"] = new DynamoDBBool(book.done);

            UpdateItemOperationConfig config = new UpdateItemOperationConfig
            {
                
                ReturnValues = ReturnValues.AllNewAttributes
            };

            Document updatedBook = await table.UpdateItemAsync(dbBook, userId, bookId, config);

            BookItem bookItem = new BookItem();
            bookItem.userId = updatedBook["UserId"];
            bookItem.bookId = updatedBook["BookId"];
            bookItem.bookName = updatedBook["BookName"];
            if(updatedBook.ContainsKey("BookCoverUrl")) bookItem.bookCoverUrl =  updatedBook["BookCoverUrl"];
            bookItem.author =  updatedBook["Author"];
            bookItem.createdAt = updatedBook["CreatedAt"];
            bookItem.currentPageNumber = (int)updatedBook["CurrentPageNumber"];
            bookItem.done = (bool)updatedBook["Done"];

            return bookItem;
        }

        public async Task<List<BookItem>> GetItemsAsync(string userId)
        {
            var expr = new Expression();
            expr.ExpressionStatement = "UserId = :user";
            expr.ExpressionAttributeValues[":user"] = userId;

            QueryOperationConfig config = new QueryOperationConfig()
            {
                KeyExpression = expr,
                IndexName = "CreatedAt",
                BackwardSearch = true
                
            };
            Search search = table.Query(config);
            List<Document> documents = new List<Document>();
            List<BookItem> books = new List<BookItem>();

            do
            {
                documents = await search.GetNextSetAsync();
                foreach (var doc in documents)
                {
                    BookItem bookItem = new BookItem();
                    bookItem.userId = doc["UserId"];
                    bookItem.bookId = doc["BookId"];
                    bookItem.bookName = doc["BookName"];
                    if(doc.Contains("BookCoverUrl")) bookItem.bookCoverUrl =  doc["BookCoverUrl"];
                    bookItem.author =  doc["Author"];
                    bookItem.createdAt = doc["CreatedAt"];
                    bookItem.currentPageNumber = (int) doc["CurrentPageNumber"];
                    bookItem.done = (bool) doc["Done"];

                    books.Add(bookItem);
                }
                
            } while (!search.IsDone);
            
            return books;
        }

        public async Task DeleteItemAsync(string userId, string bookId)
        {
            await table.DeleteItemAsync(userId, bookId);
        }

        public async Task<string> GeneratePresignedUrlAsync(string userId, string bookId)
        {
            var presignedUrl = s3.GetPreSignedURL(
                new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = bookId,
                    Expires = DateTime.Now.AddMinutes(3),
                    Verb = HttpVerb.PUT
                    
                }
            );

            string bookCoverUrl = "https://" + bucketName + ".s3.amazonaws.com/" + bookId;

            Document dbBook = new Document();
            dbBook["BookCoverUrl"] = bookCoverUrl;
            UpdateItemOperationConfig config = new UpdateItemOperationConfig
            {
                
                ReturnValues = ReturnValues.AllNewAttributes
            };
            await table.UpdateItemAsync(dbBook, userId, bookId, config);

            return presignedUrl;

        }
    }
}