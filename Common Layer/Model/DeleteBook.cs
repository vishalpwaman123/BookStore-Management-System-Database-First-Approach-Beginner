using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Layer.Model
{
    public class DeleteBookRequest
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        public string PublicID { get; set; }
    }

    public class DeleteBookResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
