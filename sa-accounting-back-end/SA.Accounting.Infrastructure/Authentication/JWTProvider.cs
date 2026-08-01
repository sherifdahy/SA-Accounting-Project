using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Core.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SA.Accounting.Infrastructure.Authentication;

public class JWTProvider(IOptions<JwtOptions> options) : IJWTProvider
{
    private readonly IOptions<JwtOptions> _options = options;

    public (string token, int expiresIn) GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles,IEnumerable<string> permissions)
    {
        List<Claim> claims = [
            new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email!),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        ];

        claims.Add(new Claim(nameof(roles), JsonConvert.SerializeObject(roles), JsonClaimValueTypes.JsonArray));
        claims.Add(new Claim(nameof(permissions), JsonConvert.SerializeObject(permissions),JsonClaimValueTypes.JsonArray));

        var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Key));

        var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expiresIn = _options.Value.ExpiryMinutes;

        var expirationDate = DateTime.UtcNow.AddMinutes(expiresIn);

        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience:_options.Value.Audience,
            claims:claims,
            expires:expirationDate,
            signingCredentials:signingCredentials
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(token),expiresIn:expiresIn * 60);
    }

    public int? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _options.Value.Issuer,
                ValidAudience = _options.Value.Audience,
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            },out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
        }
        catch
        {
            return null;
        }

    }
}
