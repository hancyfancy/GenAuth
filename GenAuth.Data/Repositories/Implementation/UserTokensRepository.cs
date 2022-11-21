using Dapper;
using GenAuth.Data.Models;
using GenAuth.Data.Repositories.Interface;
using GenCommon.Shared.Bases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAuth.Data.Repositories.Implementation
{
    public class UserTokensRepository : ConnectionBase, IUserTokensRepository
    {
        public UserTokensRepository(string connectionString) : base(connectionString)
        { 
        
        }

        public string InsertOrUpdate(long userId, string token)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    IF EXISTS (SELECT UserTokenId FROM auth.usertokens WHERE UserId = @UserId)
                                    BEGIN
	                                    IF (SELECT RefreshAt FROM auth.usertokens WHERE UserId = @UserId) > GETUTCDATE()
	                                    BEGIN
		                                    UPDATE 
			                                    auth.usertokens
		                                    SET	
			                                    Token = @Token,
			                                    RefreshAt = @RefreshAt
                                            OUTPUT inserted.Token
		                                    WHERE
			                                    UserId = @UserId
	                                    END
                                    END
                                    ELSE
                                    BEGIN
                                        INSERT INTO auth.usertokens (UserId, Token, RefreshAt)
									    OUTPUT inserted.Token 
	                                    VALUES (@UserId, @Token, @RefreshAt)
                                    END";
                    var result = connection.ExecuteScalar<string>(sql, new
                    {
                        UserId = userId,
                        Token = token,
                        RefreshAt = DateTime.UtcNow.AddDays(1)
                    });

                    connection.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public UserToken Get(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    SELECT 
                                        t.UserTokenId,
										t.Token,
										t.RefreshAt
                                    FROM 
	                                    auth.usertokens t
									WHERE
										t.UserId = @UserId";
                    var result = connection.Query<UserToken>(sql, new
                    {
                        UserId = userId
                    });

                    connection.Close();

                    return result.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public UserToken Get(string token)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    SELECT 
                                        u.UserId,
										u.Username,
										u.Email,
										u.Phone,
										u.LastActive,
                                        t.UserTokenId,
										t.RefreshAt
                                    FROM 
	                                    auth.usertokens t
										INNER JOIN auth.users u on u.UserId = t.UserId
									WHERE
										t.Token = @Token";

                    var result = connection.Query<UserToken>(sql, new
                    {
                        Token = token
                    });

                    connection.Close();

                    return result.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return default;
            }
        }
    }
}
