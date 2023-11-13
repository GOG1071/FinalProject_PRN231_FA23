namespace DataAccess.DTO;

using BusinessObject.Models;

public class AccountDTO
{
    public int       AccountId  { get; set; }
    public string?   Email      { get; set; }
    public string?   Password   { get; set; }
    public string?   CustomerId { get; set; }
    public int?      EmployeeId { get; set; }
    public int?      Role       { get; set; }
    public Customer? Customer   { get; set; }
    public Employee? Employee   { get; set; }
        
    public object[]? PrimaryKey => new object[] { this.AccountId };
}