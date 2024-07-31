using UserMicroservices.Models;

namespace UserMicroservices.Interface
{
    public interface IUser
    {
        Response<bool> StoreUserAndSendOTP(CreateUser user);
    }
}
