using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Models;
using ShoeStoreLibrary.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.Services
{
    /// <summary>
    /// Сервис для работы с авторизацией пользователей
    /// </summary>
    public class AuthService(ShoeStoreDbContext context)
    {

        private readonly ShoeStoreDbContext _context = context;
        private int _jwtActiveMinutes = 15; //срок действия jwt ключа в минутах

        /// <summary>
        /// Проверка корректности пароля
        /// </summary>
        private bool VerifyPassword(string password, string passwordHash)
            => BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);


        /// <summary>
        /// Метод аутентификации пользователя
        /// </summary>
        public async Task<User?> AuthUserAsync(LoginDto request)
        {
            string login = request.Login;

            string password = request.Password;

            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
                return null;

            var user = await GetUserByLoginAsync(login);
            if (user is null)
                return null;

            return VerifyPassword(password, user.PasswordHash) ? user : null;
        }

        /// <summary>
        /// Авторизация пользователя с токеном
        /// </summary>
        public async Task<string?> AuthUserWithTokenAsync(LoginDto request)
        {
            string login = request.Login;
            string password = request.Password;

            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
                return null;

            var user = await GetUserByLoginAsync(login);
            if (user is null)
                return null;

            return VerifyPassword(password, user.PasswordHash) ? await GenerateToken(user) : null;
        }

        /// <summary>
        /// Генерация jwt
        /// </summary>
        private async Task<string> GenerateToken(User user)
        {
            int id = user.UserId;
            string login = user.Login;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var role = await GetUserRoleAsync(login);

            var claims = new Claim[]
            {
                new ("id", id.ToString()),
                new ("login", login),
                new ("role", role.Name),
            };

            var token = new JwtSecurityToken(
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtActiveMinutes),
                issuer: AuthOptions.issuer,
                audience: AuthOptions.audience); 

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Получение пользователя по логину
        /// </summary>
        private async Task<User> GetUserByLoginAsync(string login)
            => await _context.Users.FirstOrDefaultAsync(u => u.Login == login) ?? null!;

        /// <summary>
        /// Получение роли пользователя по логину
        /// </summary>
        public async Task<Role?> GetUserRoleAsync(string login)
        {
            var user = await _context.Users
                .Include(c => c.Role) //дополнительно загружает Роль
                .FirstOrDefaultAsync(cu => cu.Login == login); // Ассинхронный метод выборки первого объекта, где совпал логин

            return user is not null ? user.Role : null;
        }
    }
}
