namespace DataAccess.Helpers;

using AutoMapper;
using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.DTO;

public class MapperConfig
{
    public static MapperConfiguration GetAccountDTOMapperConfig()
    {
        return new MapperConfiguration(cfg =>
            cfg.CreateMap<Account, AccountDTO>().ForMember(
                dest => dest.Customer,
                opt => opt.MapFrom(src => CustomerDao.Get(new object[] { src.CustomerId })))
                .ForMember(
                    dest => dest.Employee,
                    opt =>  opt.MapFrom(
                        src => EmployeeDao.Get(new Object []{src.EmployeeId}))
                    )
        );
    }
}