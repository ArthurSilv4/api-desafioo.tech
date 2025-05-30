﻿using api_desafioo.tech.Helpers;

namespace api_desafioo.tech.Models
{
    public class User
    {
        public Guid Id { get; init; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string[] Roles { get; private set; }
        public bool IsActivated { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public string RefreshToken { get; private set; }
        public DateTime RefreshTokenExpiryTime { get; private set; }

        public ICollection<Challenge> Challenges { get; private set; }

        public User(string name, string email, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = "";
            Email = email;
            Password = password;
            Roles = new string[] { "User" };
            IsActivated = true;
            CreatedAt = DateTime.UtcNow;
            Challenges = new List<Challenge>();
            RefreshToken = "";
            RefreshTokenExpiryTime = DateTime.MinValue;
        }

        public void UpdatePassword(string newPassword)
        {
            Password = newPassword;
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription; 
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
        }
    }
}
