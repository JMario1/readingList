using System;

namespace Models
{
    public class BookItem
    {

        public string userId { get; set; }
        public string bookId { get; set; }
        public string createdAt { get; set; }
        public string bookName { get; set; }
        public string author { get; set; }
        public bool done { get; set; }
        public string bookCoverUrl { get; set; }
        public int currentPageNumber { get; set; }

    }
            
}