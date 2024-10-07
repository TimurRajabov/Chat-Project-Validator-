using Chat.Api.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chat.Api.Helpers;

namespace Chat.Api.Managers;

public class JWTManager
{
    public JWTManager(IConfiguration configuration)
    {
        _configuration = configuration;
        JwtParam = _configuration.GetSection("JwtParameters")
            .Get<JwtParameters>()!;
    }

    private JwtParameters JwtParam { get; set; }
    private readonly IConfiguration _configuration;

    public string GenerateToken(User user)
    {
        var key = System.Text.Encoding.UTF32.GetBytes(JwtParam.Key);
        var signingKey = new SigningCredentials(new SymmetricSecurityKey(key), "HS256");

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name,user.Username),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Role,user.Role!) // one logic will be
        };

        //username
        //userName
        //user_name

        var security = new JwtSecurityToken(issuer: JwtParam.Issuer,
            audience: JwtParam.Audience, signingCredentials: signingKey,
            claims: claims, expires: DateTime.Now.AddSeconds(10));


        var token = new JwtSecurityTokenHandler()
            .WriteToken(security);

        return token;
    }
}