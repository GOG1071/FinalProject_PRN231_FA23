namespace DataAccess.Repository;

using AutoMapper;
using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.DTO;
using DataAccess.Helpers;
using DataAccess.Repository.Base;

public class AccountRepo : Repository<Account,AccountDao>
{
    public Account Get(string email, string password)
    {
        return AccountDao.Get(email, password);
    }

    public AccountDTO GetAccountDTO(int id)
    {
        var account      = AccountDao.Get(new object[] { id });
        var mapperConfig = MapperConfig.GetAccountDTOMapperConfig();
        var mapper       = new Mapper(mapperConfig);
        var accountDto   = mapper.Map<AccountDTO>(account);
        return accountDto;
    }
}