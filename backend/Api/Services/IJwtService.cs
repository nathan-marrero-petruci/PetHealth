using Api.Models;

namespace Api.Services;

public interface IJwtService
{
    string GenerateToken(Tutor tutor);
}
