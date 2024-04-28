using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Layer.Model
{
    public class InsertBookRequest
    {
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

        [Required]
        public IFormFile File { get; set; }
    }

    public class InsertBookResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
