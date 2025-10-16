namespace Grocery.Core.Interfaces.Services
{
    public interface IUserService
    {
        User? CurrentUser { get; }
    }

    public class User
    {
        public string? Role { get; set; }
    }
}