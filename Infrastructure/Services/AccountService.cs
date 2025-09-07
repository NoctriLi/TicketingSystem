

using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Common;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<User> signInManager;

        public AccountService(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        public async Task<BaseResponse> RegisterUser(RegisterUserRequest request)
        {
            User user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                AccountConfirmed = false,
            };
            string password = Constants.DEFAULT_PASSWORD;

            var result = await signInManager.UserManager.CreateAsync(user, password);

            return new BaseResponse
            {
                IsSuccess = result.Succeeded,
                ErrorMessage = result.Succeeded ? string.Empty : string.Join(", ", result.Errors.Select(e => e.Description))
            };  
        }

        public async Task<BaseResponse<string>> VerifyUser(string email, string password)
        {
            BaseResponse<string> response = new();

            var user = await signInManager.UserManager.FindByEmailAsync(email);
            if (user is null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "User not found";
                return response;
            }


            var result = await signInManager.UserManager.CheckPasswordAsync(user, password);
            response.IsSuccess = result;
            if (!result)
            {
                response.ErrorMessage = "Invalid Email / password";
                
            }
            else
            {
                response.Value = user.UserName;
            }
            return response;
        }
    }
}
