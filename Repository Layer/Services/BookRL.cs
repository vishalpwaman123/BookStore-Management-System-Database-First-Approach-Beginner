using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Common_Layer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repository_Layer.Interface;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Services
{
    public class BookRL : IBookRL
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _SqlConnection;
        private readonly ILogger<BookRL> _logger;
        public BookRL(IConfiguration configuration, ILogger<BookRL> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _SqlConnection = new SqlConnection(_configuration["ConnectionStrings:DBSettingConnection"]);
        }

        public async Task<DeleteBookResponse> DeleteBook(DeleteBookRequest request)
        {
            DeleteBookResponse response = new DeleteBookResponse();
            response.Message = "Delete Book Successful";
            response.IsSuccess = true;
            try
            {
                Account account = new Account(
                                _configuration["CloudinarySettings:CloudName"],
                                _configuration["CloudinarySettings:ApiKey"],
                                _configuration["CloudinarySettings:ApiSecret"]);


                Cloudinary cloudinary = new Cloudinary(account);

                // Delete Image
                var deletionParams = new DeletionParams(request.PublicID)
                {
                    ResourceType = ResourceType.Image
                };

                var deletionResult = cloudinary.Destroy(deletionParams);
                string Result = deletionResult.Result.ToString();
                if (Result.ToLower() != "ok")
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went To Wrong In Cloudinary Destroy Method";
                    return response;
                }

                if (_SqlConnection != null && _SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    DELETE
                                    FROM BookDetails
                                    WHERE BookID=@BookID;
                                    ";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _SqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = System.Data.CommandType.Text;
                        sqlCommand.CommandTimeout = 180;
                        sqlCommand.Parameters.AddWithValue("@BookID", request.BookID);
                        int Status = await sqlCommand.ExecuteNonQueryAsync();
                        if (Status <= 0)
                        {
                            response.IsSuccess = false;
                            response.Message = "Something Went Wrong !";
                        }

                    }
                    catch (Exception ex)
                    {
                        response.Message = "Exception Occurs : Message 1 : " + ex.Message;
                        response.IsSuccess = false;
                    }
                    finally
                    {
                        await sqlCommand.DisposeAsync();
                    }
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : Message : " + ex.Message;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<GetBookResponse> GetBook(GetBookRequest request)
        {
            GetBookResponse response = new GetBookResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                if(_SqlConnection !=null && _SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT *, 
                                           (SELECT Count(*) FROM BookDetails) AS TotalRecord
                                    FROM BookDetails
                                    ORDER BY BookID DESC
                                            ";
                //OFFSET @Offset ROWS FETCH NEXT @NumberOfRecordPerPage ROWS ONLY

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _SqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = System.Data.CommandType.Text;
                        sqlCommand.CommandTimeout = 180;
                        sqlCommand.Parameters.AddWithValue("@Offset", (request.PageNumber - 1) * request.NumberOfRecordPerPage);
                        sqlCommand.Parameters.AddWithValue("@NumberOfRecordPerPage", request.NumberOfRecordPerPage);

                        using (DbDataReader dbDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            try
                            {
                                if (dbDataReader.HasRows)
                                {
                                    response.data = new List<GetBook>();
                                    int Count = 0;
                                    while(await dbDataReader.ReadAsync())
                                    {
                                        response.data.Add(new GetBook()
                                        {
                                            BookID = dbDataReader["BookID"] !=DBNull.Value? (int)dbDataReader["BookID"] :-1,
                                            PublicID = dbDataReader["PublicID"] != DBNull.Value ? Convert.ToString(dbDataReader["PublicID"]) : string.Empty,
                                            BookName = dbDataReader["BookName"] != DBNull.Value ? Convert.ToString(dbDataReader["BookName"]) : string.Empty,
                                            BookType = dbDataReader["BookType"] != DBNull.Value ? Convert.ToString(dbDataReader["BookType"]) : string.Empty,
                                            BookPrice = dbDataReader["BookPrice"] != DBNull.Value ? Convert.ToString(dbDataReader["BookPrice"]) : string.Empty,
                                            BookDetails = dbDataReader["BookDetails"] != DBNull.Value ? Convert.ToString(dbDataReader["BookDetails"]) : string.Empty,
                                            ImageUrl = dbDataReader["BookImageUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["BookImageUrl"]) : string.Empty,
                                            BookAuthor = dbDataReader["BookAuthor"] != DBNull.Value ? Convert.ToString(dbDataReader["BookAuthor"]) : string.Empty,
                                            Quantity = dbDataReader["Quantity"] != DBNull.Value ? (int)(dbDataReader["Quantity"]) : -1,
                                        });

                                        if (Count == 0)
                                        {
                                            Count++;
                                            response.TotalRecords = dbDataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dbDataReader["TotalRecord"]) : -1;
                                            response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                                            response.CurrentPage = request.PageNumber;
                                        }
                                    }
                                }

                            }catch(Exception ex)
                            {
                                response.IsSuccess = false;
                                response.Message = "Exception Occurs : Message : " + ex.Message;
                            }
                            finally
                            {
                                await dbDataReader.CloseAsync();
                            }
                        }

                    }catch(Exception ex)
                    {
                        response.IsSuccess = false;
                        response.Message = "Exception Occurs : Message : " + ex.Message;
                    }
                    finally
                    {
                        await sqlCommand.DisposeAsync();
                    }
                }

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : Message : " + ex.Message;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<InsertBookResponse> InsertBook(InsertBookRequest request)
        {
            InsertBookResponse response = new InsertBookResponse();
            response.Message = "Add Book Successful";
            response.IsSuccess = true;
            try
            {

                Account account = new Account(
                                _configuration["CloudinarySettings:CloudName"],
                                _configuration["CloudinarySettings:ApiKey"],
                                _configuration["CloudinarySettings:ApiSecret"]);

                var path = request.File.OpenReadStream();

                Cloudinary cloudinary = new Cloudinary(account);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(request.File.FileName, path),
                    //Folder=""
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (_SqlConnection != null && _SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    INSERT INTO BookDetails
                                    (BookName,BookType,BookPrice,BookDetails,BookAuthor,Quantity,BookImageUrl,PublicId) VALUES
                                    (@BookName,@BookType,@BookPrice,@BookDetails,@BookAuthor,@Quantity,@BookImageUrl,@PublicId);
                                    ";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _SqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = System.Data.CommandType.Text;
                        sqlCommand.CommandTimeout = 180;
                        sqlCommand.Parameters.AddWithValue("@BookName", request.BookName);
                        sqlCommand.Parameters.AddWithValue("@BookDetails", request.BookDetails);
                        sqlCommand.Parameters.AddWithValue("@BookAuthor", request.BookAuthor);
                        sqlCommand.Parameters.AddWithValue("@BookPrice", request.BookPrice);
                        sqlCommand.Parameters.AddWithValue("@BookType", request.BookType);
                        sqlCommand.Parameters.AddWithValue("@Quantity", request.Quantity);
                        sqlCommand.Parameters.AddWithValue("@BookImageUrl", uploadResult.Url.ToString());
                        sqlCommand.Parameters.AddWithValue("@PublicID", uploadResult.PublicId.ToString());
                        int Status = await sqlCommand.ExecuteNonQueryAsync();
                        if (Status <= 0)
                        {
                            response.IsSuccess = false;
                            response.Message = "Something Went Wrong !";
                        }

                    }
                    catch (Exception ex)
                    {
                        response.Message = "Exception Occurs : Message 1 : " + ex.Message;
                        response.IsSuccess = false;
                    }
                    finally
                    {
                        await sqlCommand.DisposeAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message 2 : " + ex.Message;
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UpdateBookResponse> UpdateBook(UpdateBookRequest request)
        {
            UpdateBookResponse response = new UpdateBookResponse();
            response.Message = "Update Book Successful";
            response.IsSuccess = true;
            try
            {

                Account account = new Account(
                                _configuration["CloudinarySettings:CloudName"],
                                _configuration["CloudinarySettings:ApiKey"],
                                _configuration["CloudinarySettings:ApiSecret"]);

                
                Cloudinary cloudinary = new Cloudinary(account);

                string Url = string.Empty, PublicId = string.Empty;

                if (request.UpdateImage)
                {
                    // Delete Image
                    var deletionParams = new DeletionParams(request.PublicID)
                    {
                        ResourceType = ResourceType.Image
                    };

                    var deletionResult = cloudinary.Destroy(deletionParams);
                    string Result = deletionResult.Result.ToString();
                    if (Result.ToLower() != "ok")
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went To Wrong In Cloudinary Destroy Method";
                        return response;
                    }

                    var path = request.File2.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(request.File2.FileName, path),
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    Url = uploadResult.Url.ToString();
                    PublicId = uploadResult.PublicId.ToString();
                }
                

                if (_SqlConnection != null && _SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }

                string SqlQuery = string.Empty;
                if (request.UpdateImage)
                {
                    SqlQuery = @"
                                    UPDATE BookDetails
                                    SET BookName=@BookName,
                                    BookType=@BookType,
                                    BookPrice=@BookPrice,
                                    BookDetails=@BookDetails,
                                    BookAuthor=@BookAuthor,
                                    Quantity=@Quantity,
                                    BookImageUrl=@BookImageUrl,
                                    PublicId=@PublicId 
                                    WHERE BookID=@BookID;
                                    ";
                }
                else
                {
                    SqlQuery = @"
                                    UPDATE BookDetails
                                    SET BookName=@BookName,
                                    BookType=@BookType,
                                    BookPrice=@BookPrice,
                                    BookDetails=@BookDetails,
                                    BookAuthor=@BookAuthor,
                                    Quantity=@Quantity
                                    WHERE BookID=@BookID;
                                    ";
                }
                 

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _SqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = System.Data.CommandType.Text;
                        sqlCommand.CommandTimeout = 180;
                        sqlCommand.Parameters.AddWithValue("@BookID", request.BookID);
                        sqlCommand.Parameters.AddWithValue("@BookName", request.BookName);
                        sqlCommand.Parameters.AddWithValue("@BookDetails", request.BookDetails);
                        sqlCommand.Parameters.AddWithValue("@BookAuthor", request.BookAuthor);
                        sqlCommand.Parameters.AddWithValue("@BookPrice", request.BookPrice);
                        sqlCommand.Parameters.AddWithValue("@BookType", request.BookType);
                        sqlCommand.Parameters.AddWithValue("@Quantity", request.Quantity);
                        if (request.UpdateImage)
                        {
                            sqlCommand.Parameters.AddWithValue("@BookImageUrl", Url);
                            sqlCommand.Parameters.AddWithValue("@PublicID", PublicId);
                        }
                        int Status = await sqlCommand.ExecuteNonQueryAsync();
                        if (Status <= 0)
                        {
                            response.IsSuccess = false;
                            response.Message = "Something Went Wrong !";
                        }

                    }
                    catch (Exception ex)
                    {
                        response.Message = "Exception Occurs : Message 1 : " + ex.Message;
                        response.IsSuccess = false;
                    }
                    finally
                    {
                        await sqlCommand.DisposeAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                response.Message = "Exception Occurs : Message 2 : " + ex.Message;
                response.IsSuccess = false;
            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}
