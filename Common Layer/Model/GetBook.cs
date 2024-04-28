using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Layer.Model
{
    public class GetBookRequest
    {
        [Required]
        public int PageNumber { get; set; }
        [Required]
        public int NumberOfRecordPerPage { get; set; }
    }

    public class GetBookResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public double TotalRecords { get; set; }
        public int TotalPage { get; set; }
        public List<GetBook> data { get; set; }
    }

    public class GetBook
    {
        
        public int BookID { get; set; }
        public string ImageUrl { get; set; }
        public string PublicID { get; set; }
        public string BookName { get; set; }
        public string BookType { get; set; }
        public string BookPrice { get; set; }
        public string BookDetails { get; set; }
        public string BookAuthor { get; set; }
        public int Quantity { get; set; }

    }
}
