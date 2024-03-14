using Key2.Models;
using Keys.Data;
using Keys.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Key2.Services
{
    public class DeanService : IDeanService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        private readonly string RegexPhone = @"^\+7\d{10}$";
        private readonly string RegexBirthDate = @"^\d{2}.\d{2}.\d{4} \d{1,2}:\d{2}:\d{2}$";
        private readonly string RegexEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public DeanService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }
        async Task<DeanProfileModel> IDeanService.GetDeanProfileAsync(Guid DeanId)
        {
            var user = await _context.deans.SingleOrDefaultAsync(user => user.Id == DeanId);
            if (user == null)
            {
                throw new DirectoryNotFoundException();
            }
            DeanProfileModel model = new DeanProfileModel { Email = user.Email, FacultyNumber = user.FacultyNumber, IsActive = user.IsActive, Name = user.Name};
            return model;
        }
        async Task IDeanService.LogOutAsync(TokenBan token)
        {
            _context.TokensBan.Add(token);
            _context.SaveChanges();
        }
        async Task<TokenResponseModel> IDeanService.LoginAsync(LoginCredentialsModel LC)
        {
            var user = await _context.deans.SingleOrDefaultAsync(u => u.Email == LC.Email);
            var HashUserPassword = HashPassword(LC.Password);
            if (!Regex.IsMatch(LC.Email, RegexEmail))
            {
                throw new ArgumentException("Bad Email");
            }
            if (user == null || HashUserPassword != user.Password)
            {
                throw new ArgumentException("Bad Data");
            }
            if (user.IsActive != true)
            {
                TokenResponseModel emptyToken = new TokenResponseModel();
                emptyToken.Token = "";
                return emptyToken;
            }
            TokenResponseModel token = GenerateToken(user);
            return token;
        }

        async Task<TokenResponseModel> IDeanService.RegisterAsync(RegisterDeanModel RegisterModel)
        {
            var pass = HashPassword(RegisterModel.Password);
            Dean newUser = new Dean
            {
                Email = RegisterModel.Email,
                Id = Guid.NewGuid(),
                Name = RegisterModel.Name,
                Password = pass,
                FacultyNumber = RegisterModel.FacultyNumber,
                IsActive = false
            };

            if (_context.deans.Any<Dean>(dean => dean.Email == RegisterModel.Email))
            {
                TokenResponseModel emptyToken = new TokenResponseModel();
                emptyToken.Token = "";
                return emptyToken;
            }

            _context.deans.Add(newUser);
            _context.SaveChanges();
            TokenResponseModel token = GenerateToken(newUser);
            return token;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        private TokenResponseModel GenerateToken(User user)
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

        async Task IDeanService.AcceptChangeRole(Guid AppId, bool IsAccept)
        {
            var App = await _context.appChangeRoles.FirstOrDefaultAsync(a=>a.Id ==AppId);
            if (App != null)
            {
                if(IsAccept)
                {
                    var user = _context.mobileUsers.FirstOrDefault(u=>u.Id == App.UserId);
                    if(user == null) {
                        throw new DirectoryNotFoundException("User not Found");
                    }
                    user.Role = App.Role;
                    user.DeanId = App.DeanId;
                    user.IsActive = true;
                    user.IsDenied = false;
                }
                else
                {
                    var user = _context.mobileUsers.FirstOrDefault(u => u.Id == App.UserId);
                    if (user == null)
                    {
                        throw new DirectoryNotFoundException("User not Found");
                    }
                    user.IsDenied = true;
                }
                _context.appChangeRoles.Remove(App);
                _context.SaveChanges();
            }
            else
            {
                throw new DirectoryNotFoundException("App not found");
            }
        }

        async Task<List<AppChangeRole>> IDeanService.GetListAppChange(Guid userId)
        {

            var list = await _context.appChangeRoles.Where(app => app.DeanId == userId).ToListAsync();
            return list;
        }

        async Task<List<Key>> IDeanService.GetAllKeys(Guid userId)
        {
            List<Key> lst = await _context.keys.Where(k => k.DeanId == userId).ToListAsync();
            return lst;
        }

        async Task<List<Key>> IDeanService.GetAllFreeKeys(Guid userId)
        {
            List<Key> lst = await _context.keys.Where(k => k.DeanId == userId && k.CurrentUserId == null).ToListAsync();
            return lst;
        }

        async Task<List<Dean>> IDeanService.GetDeans()
        {
            List<Dean> lst = await _context.deans.ToListAsync(); 
            return lst;
        }

        async Task<List<MobileUser>> IDeanService.GetAssignedUsers(Guid userId)
        {
            List<MobileUser> lst = await _context.mobileUsers.Where(user => user.DeanId == userId).ToListAsync();
            return lst;
        }
        async Task IDeanService.AssignedDeanWorker(Guid userId, Guid deanId, bool isAssign)
        {
            var user = await _context.mobileUsers.FindAsync(userId);
            user.DeanId = deanId;
            user.IsDeanWorker = isAssign;
            _context.SaveChanges();
        }
        async Task<List<MobileUser>> IDeanService.GetDeanWorkers(Guid deanId)
        {
            List<MobileUser> lst = await _context.mobileUsers.Where(user => user.DeanId == deanId && user.IsDeanWorker == true).ToListAsync();
            return lst;
        }

        async Task IDeanService.EditProfile(Guid id, Dean dean)
        {
            var user = await _context.deans.FindAsync(id);
            user.Name = dean.Name;
            user.Email = dean.Email;
            user.Password = dean.Password;
            await _context.SaveChangesAsync();
        }
    }
}
