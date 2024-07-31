namespace UserMicroservices.Interface
{
    public interface IHelpers
    {
       bool GenerateAndSendOTP(string mobile,int userId);
    }
}
