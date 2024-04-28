using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repository_Layer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Services
{
    public class AuthRL : IAuthRL
    {
        private readonly IConfiguration _configuration;
        private readonly SqlConnection _SqlConnection;
        private readonly ILogger<AuthRL> _logger;
        public AuthRL(IConfiguration configuration, ILogger<AuthRL> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _SqlConnection = new SqlConnection(_configuration["ConnectionStrings:DBSettingConnection"]);
        }


        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            SignInResponse response = new SignInResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                _logger.LogInformation($"SignIn In DataAccessLayer Calling .... {JsonConvert.SerializeObject(request)}");
                if (_SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT UserId, UserName, InsertionDate
                                    FROM UserDetail 
                                    WHERE UserName=@UserName AND PassWord=@PassWord;";

                using (SqlCommand sqlCommand = new SqlCommand(SqlQuery, _SqlConnection))
                {
                    try
                    {
                        sqlCommand.CommandType = System.Data.CommandType.Text;
                        sqlCommand.CommandTimeout = 180;
                        sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                        sqlCommand.Parameters.AddWithValue("@PassWord", request.Password);
                        using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (dataReader.HasRows)
                            {
                                response.data = new SignIn();
                                response.Message = "Login Successfully";
                                await dataReader.ReadAsync();
                                response.data.UserId = dataReader["UserId"] != DBNull.Value ? (int)dataReader["UserId"] : -1;
                                response.data.UserName = dataReader["UserName"] != DBNull.Value ? (string)dataReader["UserName"] : string.Empty;
                                //response.data.Role = dataReader["Role"] != DBNull.Value ? (string)dataReader["Role"] : string.Empty;
                                response.data.InsertionDate = dataReader["InsertionDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["InsertionDate"]).ToString("dddd, dd-MM-yyyy, HH:mm tt") : string.Empty;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = "Login Unsuccessfully";
                                return response;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.IsSuccess = false;
                        response.Message = "Exception Occurs : Message 1 : " + ex.Message;
                        _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError($"Exception Occurs : Message 1 : {ex.Message}");

            }
            finally
            {
                await _SqlConnection.CloseAsync();
                await _SqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<SignUpResponse> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                _logger.LogInformation($"SignUp In DataAccessLayer Calling .... Request Body {JsonConvert.SerializeObject(request)}");

                if (_SqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _SqlConnection.OpenAsync();
                }


                if (!request.Password.Equals(request.ConfigPassword))
                {
                    response.IsSuccess = false;
                    response.Message = "Password & Confirm Password not Match";
                    return response;
                }

                string SqlStoreProcedure = @"INSERT INTO UserDetail (UserName, PassWord) VALUES (@UserName,@PassWord)";
                using (SqlCommand sqlCommand = new SqlCommand(SqlStoreProcedure, _SqlConnection))
                {

                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@PassWord", request.Password);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if(Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }

                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError($"Exception Occurs : Message 2 : {ex.Message}");
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
