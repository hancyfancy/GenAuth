using Dapper;
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
    public class UserRolesRepository : ConnectionBase, IUserRolesRepository
    {
        public UserRolesRepository(string connectionString) : base(connectionString)
        { 
        
        }

        public int Insert(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    BEGIN
                                        IF NOT EXISTS (SELECT UserRoleId FROM auth.userroles WHERE UserId = @UserId)
                                        BEGIN
                                            INSERT INTO auth.userroles
		                                    (                    
			                                    UserId,
			                                    RoleId
		                                    )
		                                    VALUES 
		                                    ( 
			                                    @UserId,
			                                    (SELECT RoleId FROM auth.roles WHERE Role = 'User' AND SubRole = 'Standard')
		                                    )
                                        END
                                    END";
                    var result = connection.Execute(sql, new
                    {
                        UserId = userId
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

        public int Update(long userId, string role, string subRole)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"USE {_database}

                                    UPDATE
	                                    auth.userroles
                                    SET
	                                    RoleId = (SELECT RoleId FROM auth.roles WHERE Role = @Role AND SubRole = @SubRole)
                                    WHERE
                                        UserId = @UserId";
                    var result = connection.Execute(sql, new
                    {
                        UserId = userId,
                        Role = role,
                        SubRole = subRole
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
