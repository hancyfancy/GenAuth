using GenAuth.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAuth.Data.Repositories.Interface
{
    public interface IUserTokensRepository
    {
        string InsertOrUpdate(long userId, string token);

        UserToken Get(long userId);

        UserToken Get(string token);
    }
}
