using Myservices.Domain.Entities;
using MyServices.Domain.Entities;

namespace Myservices.Application.Features.Auth.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}