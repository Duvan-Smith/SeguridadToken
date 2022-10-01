using SeguridadToken.Dto;

namespace SeguridadToken.Presentacion.Login;

public interface ILoginService
{
    Task<UserDto> ActionLogin(LoginDto loginDto);
}
