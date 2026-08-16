using ResolveAI.Domain.Entities;
namespace ResolveAI.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}