namespace SGS.OAD.DB.API.Services.Interfaces;

public interface IJwtService
{
    public string GenerateJwtToken(string username);
}
