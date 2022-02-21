namespace Models
{
    public class BookRequest
    {
        public string bookName { get; set; }
        public string author { get; set; }
        public string bookCoverUrl { get; set; }
        public int? currentPageNumber { get; set; }
        public bool done { get; set; }
    
    }
}