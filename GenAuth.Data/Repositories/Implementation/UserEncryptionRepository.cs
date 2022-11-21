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
    public class UserEncryptionRepository : ConnectionBase, IUserEncryptionRepository
    {
        public UserEncryptionRepository(string connectionString) : base(connectionString)
        { 
        
        }

        public byte[] InsertOrUpdate(long userId, byte[] encryptionKey)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    IF EXISTS (SELECT UserEncryptionId FROM auth.userencryption WHERE UserId = @UserId)
                                    BEGIN
		                                UPDATE 
			                                auth.userencryption
		                                SET	
			                                EncryptionKey = @EncryptionKey
                                        OUTPUT inserted.EncryptionKey
		                                WHERE
			                                UserId = @UserId
                                    END
                                    ELSE
                                    BEGIN
                                        INSERT INTO auth.userencryption (UserId, EncryptionKey)
									    OUTPUT inserted.EncryptionKey 
	                                    VALUES (@UserId, @EncryptionKey)
                                    END";

                    var result = connection.ExecuteScalar<byte[]>(sql, new
                    {
                        UserId = userId,
                        EncryptionKey = encryptionKey
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

        public UserEncryption Get(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    SELECT 
                                        e.UserEncryptionId,
										e.EncryptionKey
                                    FROM 
	                                    auth.userencryption e
									WHERE
										e.UserId = @UserId";

                    var result = connection.Query<UserEncryption>(sql, new
                    {
                        UserId = userId
                    });

                    connection.Close();

                    UserEncryption userEncryption = result.FirstOrDefault();

                    userEncryption.UserId = userId;

                    return userEncryption;
                }
            }
            catch (Exception e)
            {
                return default;
            }
        }
    }
}
