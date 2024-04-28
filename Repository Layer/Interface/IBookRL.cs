using Common_Layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Interface
{
    public interface IBookRL
    {
        public Task<InsertBookResponse> InsertBook(InsertBookRequest request);

        public  Task<GetBookResponse> GetBook(GetBookRequest request);

        public Task<UpdateBookResponse> UpdateBook(UpdateBookRequest request);

        public Task<DeleteBookResponse> DeleteBook(DeleteBookRequest request);
    }
}
