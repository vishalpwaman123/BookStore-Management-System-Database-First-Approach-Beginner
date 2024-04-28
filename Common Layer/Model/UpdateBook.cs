using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Layer.Model
{
    public class UpdateBookRequest
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        public string PublicID { get; set; }

        [Required]
        public string BookName { get; set; }

        [Required]
        public string BookType { get; set; }

        [Required]
        public string BookPrice { get; set; }
        public string BookDetails { get; set; }
        public string BookAuthor { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value Greater than 0")]
        public int Quantity { get; set; }
        public bool UpdateImage { get; set; }
        public string File1 { get; set; }
        public IFormFile File2 { get; set; }
    }

    public class UpdateBookResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
