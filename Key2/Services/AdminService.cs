using Key2.Models;
using Keys.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Key2.Services
{
    public class AdminService : IAdminService
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        private readonly string RegexPhone = @"^\+7\d{10}$";
        private readonly string RegexBirthDate = @"^\d{2}.\d{2}.\d{4} \d{1,2}:\d{2}:\d{2}$";
        private readonly string RegexEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public AdminService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }
        async Task IAdminService.ActiveDean(Guid userId, Guid deanId)
        {
            var admin = await _context.administrators.SingleOrDefaultAsync(a => a.Id == userId);
            if (admin == null)
            {
                throw new RankException("Forbidden");
            }
            var dean = await _context.deans.SingleOrDefaultAsync(d=>d.Id== deanId);
            if(dean == null)
            {
                throw new DirectoryNotFoundException("Dean not found");
            }
            dean.IsActive = true;
            await _context.SaveChangesAsync();
        }

        async Task<TokenResponseModel> IAdminService.LoginAsync(LoginCredentialsModel LC)
        {
            var user = await _context.administrators.SingleOrDefaultAsync(u => u.Email == LC.Email);
            var HashUserPassword = HashPassword(LC.Password);
            if (!Regex.IsMatch(LC.Email, RegexEmail))
            {
                throw new ArgumentException("Bad Email");
            }
            if (user == null || HashUserPassword != user.Password)
            {
                throw new ArgumentException("Bad Data");
            }
            TokenResponseModel token = GenerateToken(user);
            return token;
        }

        async Task<TokenResponseModel> IAdminService.RegisterAsync(RegisterDeanModel RegisterModel)
        {
            var pass = HashPassword(RegisterModel.Password);
            Administrator newUser = new Administrator
            {
                Email = RegisterModel.Email,
                Id = Guid.NewGuid(),
                Name = RegisterModel.Name,
                Password = pass,
            };
            await _context.administrators.AddAsync(newUser);
            await _context.SaveChangesAsync();
            TokenResponseModel token = GenerateToken(newUser);
            return token;
        }

        async Task IAdminService.LogOutAsync(TokenBan token)
        {
            _context.TokensBan.Add(token);
            _context.SaveChanges();
        }

        async Task<List<Dean>> IAdminService.ShowDeans(Guid Id)
        {
            var admin = await _context.administrators.SingleOrDefaultAsync(a => a.Id == Id);
            if(admin== null)
            {
                throw new RankException("Forbidden");
            }
            List<Dean> lst = await _context.deans.ToListAsync();
            return lst;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        private TokenResponseModel GenerateToken(Administrator user)
        {
            var claims = new List<Claim> {new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Hash, user.Password),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())};
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: _configuration["Token:ISSUER"],
                    audience: _configuration["Token:AUDIENCE"],
                    claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:KEY"])), SecurityAlgorithms.HmacSha256));
            return new TokenResponseModel { Token = new JwtSecurityTokenHandler().WriteToken(jwt) };
        }
    }
}
