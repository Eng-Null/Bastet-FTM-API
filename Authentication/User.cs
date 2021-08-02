using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BastetFTMAPI.Authentication
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string UserNameHash { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Roles> Roles { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }
    }
    public record UserDto
    (
        Guid Id,
        string Username,
        string DisplayName,
        string UserNameHash,
        string Email,
        string Password,
        List<Roles> Roles,
        DateTimeOffset CreationDate,
        DateTimeOffset LastLoginDate,
        int FailedAttempts,
        bool IsLocked
    );

    public enum Roles { Owner, Developer, Admin, Manager, Guest }
    public record CreateUser(
        [Required] string Username,
        [Required] string DisplayName,
        [Required] string Email,
        [Required] string Password);
    public record LoginUser(
        [Required] string Username,
        [Required] string UserNameHash,
        [Required] string Password);
    
}
