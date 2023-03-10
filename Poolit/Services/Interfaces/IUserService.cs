using Poolit.Models.Requests;
using Poolit.Models.Responses;

namespace Poolit.Services.Interfaces;

public interface IUserService
{
    public Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    public Task<LoginResponse> LoginAsync(LoginRequest request);
}
