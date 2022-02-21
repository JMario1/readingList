using System;

using System.Collections.Generic;
using DataLayer;
using Models;

namespace BusineesLogic
{
    public class ReadingList 
    {
        private readonly DataAccess dataAccess = new DataAccess();
        public  BookItem CreateReadingList(string userId, BookRequest book)
        {
            string bookId = Guid.NewGuid().ToString();
            string time = DateTime.Now.ToString();
            BookItem bookItem = new BookItem
            {
                userId = userId,
                bookId = bookId,
                done = false,
                author = book.author,
                createdAt = time,
                bookName = book.bookName,
                currentPageNumber = 1
            };
            var result = dataAccess.CreateItemAsync(userId, bookItem);
            return  result.Result;
        }

        public  BookItem UpdateReadingList(BookRequest book, string userId, string bookId)
        {
            var result = dataAccess.UpdateItemAsync(book, userId, bookId).Result;
            return result;
        }

        public  List<BookItem> GetReadingList(string userId)
        {
            List<BookItem> result = dataAccess.GetItemsAsync(userId).Result;
            return result;
        }

        public  void DeleteReadingList(string userId, string bookId)
        {
            dataAccess.DeleteItemAsync(userId, bookId).Wait();
        }

        public  string GenerateUrl(string userId, string bookId)
        {
            var result = dataAccess.GeneratePresignedUrlAsync(userId, bookId);
            return result.Result;
        }
    }
}