using Key2.Models;
using Keys.Data;
using Keys.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Key2.Services
{
    public class UserService : IUserService
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBannedTokenService _bannedTokensService;
        private readonly string RegexPhone = @"^\+7\d{10}$";
        private readonly string RegexBirthDate = @"^\d{2}.\d{2}.\d{4} \d{1,2}:\d{2}:\d{2}$";
        private readonly string RegexEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public UserService(AppDbContext context, IBannedTokenService bannedTokensService, IConfiguration configuration)
        {
            _context = context;
            _bannedTokensService = bannedTokensService;
            _configuration = configuration;
        }

        async Task<RoleResponseModel> IUserService.GetRoleAsync(Guid userId)
        { 
            var user = await _context.mobileUsers.FindAsync(userId);
            Role role = user.Role;
            RoleResponseModel responseModel = new RoleResponseModel();
            responseModel.Role = role;
            responseModel.IsActive = user.IsActive;
            responseModel.IsDenied = user.IsDenied;
            responseModel.IsDeanWorker = user.IsDeanWorker == true ? true : false;
            return responseModel;
        }
        async Task<TokenResponseModel> IUserService.RegisterAsync(RegisterModel RegisterModel)
        {
            if (RegisterModel == null)
            {
                throw new ArgumentException("Bad Data");
            }
            var existingUser = await _context.mobileUsers.SingleOrDefaultAsync(user => user.Email == RegisterModel.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("User with this Email has already exist");
            }
            if (!Regex.IsMatch(RegisterModel.BirthDate.ToString(), RegexBirthDate)  || !Regex.IsMatch(RegisterModel.Email, RegexEmail) || RegisterModel.BirthDate.Value.Year >= DateTime.Now.Year)
            {
                throw new ArgumentException("Bad Data");
            }
            var pass = HashPassword(RegisterModel.Password);
            MobileUser newUser = new MobileUser
            {
                Email = RegisterModel.Email,
                Id = Guid.NewGuid(),
                Name = RegisterModel.Name,
                Password = pass,
                Role = Keys.Models.Enums.Role.Unconfirmed,
                BirthDate = DateTime.UtcNow,    
            };

            if (_context.mobileUsers.Any<MobileUser>(user => user.Email == RegisterModel.Email))
            {
                TokenResponseModel emptyToken = new TokenResponseModel();
                emptyToken.Token = "";
                return emptyToken;
            }

            _context.mobileUsers.Add(newUser);
            _context.SaveChanges();
            TokenResponseModel token = GenerateToken(newUser);
            return token;
        }
        async Task IUserService.LogOutAsync(TokenBan token)
        {
            _context.TokensBan.Add(token);
            _context.SaveChanges();
        }

        async Task<TokenResponseModel> IUserService.LoginAsync(LoginCredentialsModel LC)
        {
            var user = await _context.mobileUsers.SingleOrDefaultAsync(u => u.Email == LC.Email);
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

        async Task<UserProfileModel> IUserService.GetProfileAsync(Guid userId)
        {
            var user = await _context.mobileUsers.SingleOrDefaultAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new DirectoryNotFoundException();
            }
            UserProfileModel model = new UserProfileModel { Email = user.Email,Name = user.Name };
            return model;
        }

        async Task IUserService.EditProfileAsync(RegisterModel EditModel, Guid userId)
        {
            var user = await _context.mobileUsers.SingleOrDefaultAsync(user => user.Id == userId); //проверка корректности емеил
            if (user == null)
            {
                throw new DirectoryNotFoundException();
            }
            if (user.Email != EditModel.Email && _context.mobileUsers.SingleOrDefault(user => user.Email == EditModel.Email) != null) { throw new ArgumentException("Email already exist"); }
            user.Email = EditModel.Email;
            user.Name = EditModel.Name;
            var HashUserPassword = HashPassword(EditModel.Password);
            if (!Regex.IsMatch(EditModel.Email, RegexEmail))
            {
                throw new ArgumentException("Bad Email");
            }
            _context.SaveChanges();
        }

        async Task IUserService.CreateChangeRole(Guid userId, Role desiredRole, Guid deanId)
        {
            var user = await _context.mobileUsers.SingleOrDefaultAsync(u => u.Id == userId);
            var ActiveOffer =await _context.appChangeRoles.SingleOrDefaultAsync(r=>r.UserId == userId);
            if(ActiveOffer != null)
            {
                throw new ArgumentException("Ваша заявка уже отправлена");
            }
            if (user != null)
            {
                AppChangeRole role = new AppChangeRole
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    Name = user.Name,
                    UserId = user.Id,
                    Email = user.Email,
                    Role = desiredRole,
                    DeanId = deanId
                };
                user.IsActive = false;
                user.Role = desiredRole;
                _context.appChangeRoles.Add(role);
                _context.SaveChanges();
            }
            else
            {
                throw new DirectoryNotFoundException("UserNotFound");
            }
        }

        async Task IUserService.CreateQR(Guid userId, Guid keyId, string pass)
        {
            var key = await _context.keys.SingleOrDefaultAsync(k=>k.Id == keyId);
            if(key == null)
            {
                throw new DirectoryNotFoundException("Key not Found");
            }
            if (key.CurrentUserId != userId)
            {
                throw new RankException("user doesnot have this key");
            }
            QRPass qr = new QRPass { QRPas = HashPassword(pass), Id = Guid.NewGuid(),keyId = key.Id };
            await _context.QRPass.AddAsync(qr);
            await _context.SaveChangesAsync();
        }

        async Task IUserService.ReadQR(Guid userId, Guid keyId, string pass)
        {
            var user = await _context.mobileUsers.SingleOrDefaultAsync(u=>u.Id == userId);
            var qr = await _context.QRPass.FirstOrDefaultAsync(q => q.keyId == keyId);
            if(qr == null)
            {
                throw new DirectoryNotFoundException("QR not found");
            }
            var key = await _context.keys.SingleOrDefaultAsync(k => k.Id == keyId);
            if (qr.QRPas == HashPassword(pass))
            {
                key.CurrentUser = user;
                key.CurrentUserId = userId;
            }
            _context.QRPass.Remove(qr);
            await _context.SaveChangesAsync();
        }

        async Task IUserService.DeleteQr(Guid keyid)
        {
            var qr = await _context.QRPass.FirstOrDefaultAsync(q => q.keyId == keyid);
            if (qr == null)
            {
                throw new DirectoryNotFoundException("QR not found");
            }
            _context.QRPass.Remove(qr);
            await _context.SaveChangesAsync();
        }
    }
}
