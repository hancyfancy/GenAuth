using Dapper;
using GenAuth.Data.Models;
using GenAuth.Data.Repositories.Interface;
using GenCommon.Shared.Bases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAuth.Data.Repositories.Implementation
{
    public class UserRepository : ConnectionBase, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString)
        { 
        
        }

        public long Insert(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    BEGIN
                                        IF NOT EXISTS (SELECT UserId FROM auth.users WHERE Username = @Username)
                                        BEGIN
                                            INSERT INTO auth.users
		                                    (                    
			                                    Username,
			                                    Email,
			                                    Phone,
			                                    LastActive
		                                    )
		                                    OUTPUT inserted.UserId 
		                                    VALUES 
		                                    ( 
			                                    @Username,
			                                    @Email,
			                                    @Phone,
			                                    @LastActive
		                                    )
                                        END
							            ELSE
							            BEGIN
								            SELECT UserId FROM auth.users WHERE Username = @Username 
							            END
                                    END";
                    var result = connection.ExecuteScalar<long>(sql, new
                    {
                        Username = user.Username,
                        Email = user.Email,
                        Phone = user.Phone,
                        LastActive = user.LastActive,
                    });

                    connection.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public int UpdateLastActive(long userId)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    UPDATE
	                                    auth.users
                                    SET
	                                    LastActive = @LastActive
                                    WHERE
                                        UserId = @UserId";
                    var result = connection.Execute(sql, new
                    {
                        UserId = userId,
                        LastActive = DateTime.UtcNow
                    });

                    connection.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
