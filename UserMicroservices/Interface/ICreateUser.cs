using UserMicroservices.Models;

namespace UserMicroservices.Interface
{
    public interface IUser
    {
        Response<int> StoreUserAndSendOTP(CreateUser user);
        Response<bool> CheckOtpandRegisterUser(LoginParams loginParams);
        Response<bool> SaveConnectionId(SaveConnectionId saveConnectionId);
    }
}
