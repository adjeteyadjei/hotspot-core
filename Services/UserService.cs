using Hotvenues.Configurations;
using Hotvenues.Data;
using Hotvenues.Helpers;
using Hotvenues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotvenues.Services
{
    public class RegisterUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }

        public long ProfileId { get; set; }
        public string Picture { get; set; }
        public string RootPath { get; set; }
        public UserType Type { get; set; }
        public string VendorLocation { get; set; }
    }

    public class UpdateUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password), StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public long? TemplateId { get; set; }
        public long ProfileId { get; set; }
        public string Picture { get; set; }
        public string RootPath { get; set; }
        public UserType Type { get; set; }
        public string VendorLocation { get; set; }
    }

    public class LoginParams
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Username { get; set; }
        public string View { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string Picture { get; set; }
        public UserType Type { get; set; }
        public string VendorLocation { get; set; }
    }

    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string Password { get; set; }
        public string RootPath { get; set; }
        public UserType Type { get; set; }
        public string VendorLocation { get; set; }
    }

    public class ChangePasswordDto
    {
        public string Username { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public interface IUserService
    {
        Task<UserProfileDto> UserProfile(string username);
        Task<bool> UpdateUserProfile(UserProfileDto profile);
        Task<bool> ChangePassword(ChangePasswordDto passwords);
        Task<LoginResponse> Authenticate(LoginParams loginParams);
        Task<bool> CreateUser(RegisterUserModel model);
        Task<bool> UpdateUser(UpdateUserModel model);
        Task<bool> DeleteUser(string username);
        Task<List<UserDto>> GetAllUsers();
        Task<List<string>> GetPrivileges();
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IOptions<JwtSettings> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _jwtSettings = jwt.Value;
        }

        public async Task<bool> CreateUser(RegisterUserModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                Name = model.Name,
                Email = model.Email,
                Picture = new ImageHelpers(model.RootPath).SaveDataImage(model.Picture),
                ProfileId = model.ProfileId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Type = model.Type,
                VendorLocation = model.VendorLocation
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) throw new Exception(ExceptionHelper.ProcessException(result));

            //Add roles In profile to user
            var profile = await _context.Profiles.FindAsync(model.ProfileId);
            if (profile != null)
            {
                //var theUser = await _userManager.FindByNameAsync(model.Username);
                var privileges = profile.Privileges?.Split(',').ToList().Select(q => q.Trim());
                await _userManager.AddToRolesAsync(user, privileges);
            }

            return result.Succeeded;
        }

        public async Task<bool> DeleteUser(string username)
        {
            var theUser = await _userManager.FindByNameAsync(username);
            await _userManager.DeleteAsync(theUser);
            return true;
        }

        public async Task<List<string>> GetPrivileges()
        {
            return await Task.FromResult(_roleManager.Roles.Select(q => q.Name).ToList());
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            return await Task.FromResult(_context.Users.Select(q => new UserDto
            {
                Id = q.Id,
                Username = q.UserName,
                Name = q.Name,
                Email = q.Email,
                Picture = q.Picture,
                PhoneNumber = q.PhoneNumber,
                ProfileId = q.ProfileId,
                ProfileName = q.Profile.Name,
                Type = q.Type,
                VendorLocation = q.VendorLocation
            }).ToList());
        }

        public async Task<LoginResponse> Authenticate(LoginParams loginParams)
        {
            //var user = await _userManager.FindByNameAsync(loginParams.Username);
            var user = _context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(q => q.UserName == loginParams.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginParams.Password))
            {
                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

                var roles = _userManager.GetRolesAsync(user).Result;
                var claims = roles.Select(x => new Claim("roles", x)).ToList();
                //var location = user.UserLocations.Select(x => new Claim());
                //var profile = await _context.Profiles.FindAsync(user.ProfileId);
                claims.Add(new Claim("username", user.UserName));
                claims.Add(new Claim("profile", user.Profile?.Name ?? ""));
                claims.Add(new Claim("email", user.Email ?? ""));
                claims.Add(new Claim("phoneNumber", user.PhoneNumber ?? ""));
                claims.Add(new Claim("fullName", user.Name));
                claims.Add(new Claim("picture", user.Picture ?? ""));
                claims.Add(new Claim("vendorLocation", user.VendorLocation ?? ""));
                claims.Add(new Claim("type", user.Type.ToString() ?? ""));


                var token = new JwtSecurityToken(
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    expires: DateTime.UtcNow.AddHours(24),
                    signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256),
                    claims: claims);

                return new LoginResponse
                {
                    Username = user.UserName,
                    View = user.Profile?.DefaultView,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Type = user.Type.ToString()
                };
            }
            throw new Exception("Invalid username or password");
        }

        public async Task<bool> UpdateUser(UpdateUserModel model)
        {
            var user = _context.Users.FirstOrDefault(q => q.UserName == model.Username);
            if (user == null) throw new Exception("User not found.");

            user.Name = model.Name;
            user.ProfileId = model.ProfileId;
            user.UpdatedAt = DateTime.Now;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.VendorLocation = model.VendorLocation;
            user.Type = model.Type;

            if (model.Picture != user.Picture)
            {
                if (string.IsNullOrEmpty(user.Picture)) user.Picture = new ImageHelpers(model.RootPath).SaveDataImage(model.Picture);
                else new ImageHelpers(model.RootPath).SaveDataImage(model.Picture, user.Picture);
            }

            var res = await _userManager.UpdateAsync(user);

            if (!res.Succeeded) throw new Exception(ExceptionHelper.ProcessException(res));

            //Update user roles
            var profile = await _context.Profiles.FindAsync(model.ProfileId);
            if (profile != null)
            {
                var oldRoles = await _userManager.GetRolesAsync(user);
                var clearRoles = await _userManager.RemoveFromRolesAsync(user, oldRoles);

                if (clearRoles.Succeeded)
                {
                    var privileges = profile.Privileges?.Split(',').ToList().Select(q => q.Trim());
                    await _userManager.AddToRolesAsync(user, privileges);
                }
            }


            //Change password
            if (!string.IsNullOrEmpty(model.Password))
            {
                var clearPassword = await _userManager.RemovePasswordAsync(user);
                if (clearPassword.Succeeded) await _userManager.AddPasswordAsync(user, model.Password);
            }

            return true;
        }

        public async Task<UserProfileDto> UserProfile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) throw new Exception("Sorry, I can't find your account profile.");
            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Picture = user.Picture,
                VendorLocation = user.VendorLocation,
                Type = user.Type
            };
        }

        public async Task<bool> UpdateUserProfile(UserProfileDto profile)
        {
            var user = await _userManager.FindByNameAsync(profile.Username);
            if (user == null) throw new Exception("I can't find your account. Ensure you are logged in.");
            var validPassword = await _userManager.CheckPasswordAsync(user, profile.Password);
            if (!validPassword) throw new Exception("Invalid Password");

            user.Name = profile.Name;
            user.PhoneNumber = profile.PhoneNumber;
            user.Email = profile.Email;
            user.VendorLocation = profile.VendorLocation;
            user.Type = profile.Type;

            if (profile.Picture != user.Picture)
            {
                if (string.IsNullOrEmpty(user.Picture)) user.Picture = new ImageHelpers(profile.RootPath).SaveDataImage(profile.Picture);
                else new ImageHelpers(profile.RootPath).SaveDataImage(profile.Picture, user.Picture);
            }

            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded) throw new Exception(ExceptionHelper.ProcessException(res));

            return true;
        }

        public async Task<bool> ChangePassword(ChangePasswordDto passwords)
        {
            if (passwords.NewPassword != passwords.ConfirmPassword) throw new Exception("Password Mismatch");
            var user = await _userManager.FindByNameAsync(passwords.Username);
            if (user == null) throw new Exception("I can't find your account. Ensure you are logged in.");
            var validPassword = await _userManager.CheckPasswordAsync(user, passwords.CurrentPassword);
            if (!validPassword) throw new Exception("Invalid Password");

            var clearPassword = await _userManager.RemovePasswordAsync(user);
            if (clearPassword.Succeeded)
            {
                var res = await _userManager.AddPasswordAsync(user, passwords.NewPassword);
                if (!res.Succeeded) throw new Exception(ExceptionHelper.ProcessException(res));
            }
            return true;

        }
    }
}
