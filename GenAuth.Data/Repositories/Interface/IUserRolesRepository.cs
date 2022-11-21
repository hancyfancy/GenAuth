using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAuth.Data.Repositories.Interface
{
    public interface IUserRolesRepository
    {
        int Insert(long userId);

        int Update(long userId, string role, string subRole);
    }
}
