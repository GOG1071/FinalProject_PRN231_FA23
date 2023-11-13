namespace DataAccess.Repository;

using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.Repository.Base;

public class AccountRepo : Repository<Account,AccountDao>
{
    public Account Get(string email, string password)
    {
        return AccountDao.Get(email, password);
    }
}