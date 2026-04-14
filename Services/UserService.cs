using CourseManager.Data;
using CourseManager.DTOs.Users;
using CourseManager.Models;
using CourseManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // REGISTER

        public async Task<UserResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            // Validate: email must be unique
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("Email is already in use.");

            // Validate: students must have a StudyMode
            if (dto.UserType == UserType.Student && dto.StudyMode == null)
                throw new InvalidOperationException("Students must have a study mode (FullTime or PartTime).");

            // Validate: non-students should not have a StudyMode
            if (dto.UserType != UserType.Student && dto.StudyMode != null)
                throw new InvalidOperationException("Only students can have a study mode.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                UserType = dto.UserType,
                StudyMode = dto.StudyMode,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToResponse(user);
        }

        // GET BY ID

        public async Task<UserResponseDto> GetByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return MapToResponse(user);
        }

        // UPDATE

        public async Task<UserResponseDto> UpdateAsync(int userId, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            if (!user.IsActive)
                throw new InvalidOperationException("Cannot update an inactive user.");

            // Check email uniqueness (but allow keeping the same email)
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                throw new InvalidOperationException("Email is already in use.");

            user.Username = dto.Username;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            return MapToResponse(user);
        }

        // CHANGE PASSWORD

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            if (!user.IsActive)
                throw new InvalidOperationException("Cannot change password of an inactive user.");

            user.PasswordHash = HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
        }

        // DEACTIVATE

        public async Task DeactivateAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            if (!user.IsActive)
                throw new InvalidOperationException("User is already inactive.");

            user.IsActive = false;
            await _context.SaveChangesAsync();
        }

        // REACTIVATE

        public async Task ReactivateAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            if (user.IsActive)
                throw new InvalidOperationException("User is already active.");

            user.IsActive = true;
            await _context.SaveChangesAsync();
        }

        // HELPERS

        private string HashPassword(string password)
        {
            // Simple hash for now - good enough for this assignment
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private UserResponseDto MapToResponse(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                UserType = user.UserType,
                StudyMode = user.StudyMode,
                IsActive = user.IsActive
            };
        }
    }
}