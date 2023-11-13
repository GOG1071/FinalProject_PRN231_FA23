namespace DataAccess.DAO;

using System.Runtime.CompilerServices;
using BusinessObject.Models;
using DataAccess.DAO.Base;

public class AccountDao : Dao<Account>
{
    public static Account Get(string email, string password)
    {
        try
        {
            using var context = new Prn231DBContext();
            var       list    = context.Set<Account>().ToList();
            return list.FirstOrDefault(x => x.Email == email && x.Password == password);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}