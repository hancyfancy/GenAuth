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
    public class UserVerificationRepository : ConnectionBase, IUserVerificationRepository
    {
        public UserVerificationRepository(string connectionString) : base(connectionString)
        { 
        
        }

        public int Insert(UserVerification userVerification)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    BEGIN
                                        IF NOT EXISTS (SELECT UserVerificationId FROM auth.userverification WHERE UserId = @UserId)
                                        BEGIN
                                            INSERT INTO auth.userverification
		                                    (                    
			                                    UserId,
			                                    EmailVerified,
			                                    PhoneVerified
		                                    )
		                                    VALUES 
		                                    ( 
			                                    @UserId,
			                                    @EmailVerified,
			                                    @PhoneVerified
		                                    )
                                        END
                                    END";
                    var result = connection.Execute(sql, new
                    {
                        UserId = userVerification.UserId,
                        EmailVerified = userVerification.EmailVerified,
                        PhoneVerified = userVerification.PhoneVerified,
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

        public int UpdateEmailVerified(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    BEGIN
                                        IF EXISTS (SELECT UserVerificationId FROM auth.userverification WHERE UserId = @UserId)
                                        BEGIN
                                            UPDATE
			                                    auth.userverification
		                                    SET
			                                    EmailVerified = 1
		                                    WHERE
			                                    UserId = @UserId
                                        END
                                    END";
                    var result = connection.Execute(sql, new
                    {
                        UserId = userId,
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

        public int UpdatePhoneVerified(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    BEGIN
                                        IF EXISTS (SELECT UserVerificationId FROM auth.userverification WHERE UserId = @UserId)
                                        BEGIN
                                            UPDATE
			                                    auth.userverification
		                                    SET
			                                    PhoneVerified = 1
		                                    WHERE
			                                    UserId = @UserId
                                        END
                                    END";
                    var result = connection.Execute(sql, new
                    {
                        UserId = userId,
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

        public UserVerification Get(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    SELECT 
                                        v.UserVerificationId,
										u.UserId,
										v.EmailVerified,
										v.PhoneVerified,
										u.Email,
										u.Phone,
										(SELECT CONCAT(SubRole + ' ', Role) FROM auth.roles WHERE RoleId = r.RoleId) AS Role,
										u.LastActive
                                    FROM 
	                                    auth.users u
										INNER JOIN auth.userverification v on v.UserId = u.UserId
										INNER JOIN auth.userroles r on r.UserId = u.UserId
									WHERE 
										u.Username = @Username";
                    var result = connection.Query<UserVerification>(sql, new
                    {
                        Username = username
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
