using UserMicroservices.Models;

namespace UserMicroservices.Interface
{
    public interface IUser
    {
        void CreateUser(CreateUser user);
        bool loginUser(LoginUser login);
        List<UserData> GetUserData(string userEmail,string username);
        bool UpdatePassword(string? userName, string? userEmail, string userPassword);
    }
}
