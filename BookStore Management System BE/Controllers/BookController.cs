using Common_Layer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository_Layer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Management_System_BE.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRL _bookRL;
        public BookController(IBookRL bookRL)
        {
            _bookRL = bookRL;
        }

        [HttpPost]
        public async Task<ActionResult> InsertBook([FromForm] InsertBookRequest request)
        {
            InsertBookResponse response = new InsertBookResponse();
            try
            {
                response = await _bookRL.InsertBook(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> GetBook(GetBookRequest request)
        {
            GetBookResponse response = new GetBookResponse();
            try
            {
                response = await _bookRL.GetBook(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateBook([FromForm] UpdateBookRequest request)
        {
            UpdateBookResponse response = new UpdateBookResponse();
            try
            {
                response = await _bookRL.UpdateBook(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteBook(DeleteBookRequest request)
        {
            DeleteBookResponse response = new DeleteBookResponse();
            try
            {
                response = await _bookRL.DeleteBook(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }
    }
}
