using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Threading.Tasks;
using Usermicroservices;
using UserMicroservices;
using UserMicroservices.Interface;
using UserMicroservices.Models;
namespace UserMicroservices.ProtoContracts
{
    public class UserServicePc: UserService.UserServiceBase
    {
        private readonly IUser _userService;
        public UserServicePc(IUser userService) {
        _userService = userService;
        }
        public override Task<CreateUserResponse> CreateUser(createUser request, ServerCallContext context)
        {
            var userDto = new CreateUser
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                IsDisabled = request.IsDisabled
            };

            var result = _userService.StoreUserAndSendOTP(userDto);

            return Task.FromResult(new CreateUserResponse
            {
                UserId = result.Data,
                Success=result.Success,
                ErrorMessage = result.ErrorMessage == null ? string.Empty : result.ErrorMessage
                
            });
        }

        public override Task<CachedUserIdsResponse> GetCachedUserIds(Empty request, ServerCallContext context)
        {
            var userIds = _userService.GetCachedUserIds();

            return Task.FromResult(new CachedUserIdsResponse
            {
                UserIds = { userIds.Data }
            });
        }

        public override Task<Response> CheckOtpAndActivateUser(loginParams request, ServerCallContext context)
        {
            var loginParams = new LoginParams
            {
                otp = request.Otp,
                userId = request.UserId
            };

            var result = _userService.CheckOtpandRegisterUser(loginParams);

            return Task.FromResult(new Response
            {
                Success = result.Success,
                ErrorMessage = result.ErrorMessage
            });
        }

        public override Task<Empty> CheckHealth(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }
}
