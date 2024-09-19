using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AiTIman_System.Areas.Identity.Data;
using AiTiman_System.Data;
using AiTIman_System.Data;

namespace AiTiman_System.Data
{
    public class MongoUserStore :
        IUserStore<AiTimanUser>,
        IUserEmailStore<AiTimanUser>,
        IUserPasswordStore<AiTimanUser>,
        IUserRoleStore<AiTimanUser> // Implement IUserRoleStore
    {
        private readonly IMongoCollection<AiTimanUser> _users;
        private readonly IMongoCollection<AiTimanUser> _usersCollection;

        public MongoUserStore(MongoDbContext context)
        {
            _users = context.Users;
        }

        public Task<IdentityResult> CreateAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _users.InsertOne(user);
                return IdentityResult.Success;
            }, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var result = _users.DeleteOne(u => u.Id == user.Id);
                return result.DeletedCount > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }, cancellationToken);
        }

        public Task<AiTimanUser> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return _users.Find(u => u.Email == email).FirstOrDefault();
            }, cancellationToken);
        }

        public Task<AiTimanUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return _users.Find(u => u.Id == userId).FirstOrDefault();
            }, cancellationToken);
        }

        public Task<AiTimanUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefault();
            }, cancellationToken);
        }

        public Task<string> GetEmailAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail?.ToUpper());
        }

        public Task<string> GetNormalizedUserNameAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName?.ToUpper());
        }

        public Task<string> GetUserIdAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetEmailAsync(AiTimanUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(AiTimanUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(AiTimanUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(AiTimanUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AiTimanUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var result = _users.ReplaceOne(u => u.Id == user.Id, user);
                return result.ModifiedCount > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }, cancellationToken);
        }

        public void Dispose()
        {
            // No resources to dispose
        }

        // IUserPasswordStore methods
        public Task<string> GetPasswordHashAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            // Ensure this method returns the correct hash format
            return Task.FromResult(user.PasswordHash);
        }
        public Task<bool> HasPasswordAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(AiTimanUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        // IUserRoleStore methods
        public Task AddToRoleAsync(AiTimanUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var role = _users.Find(u => u.Id == user.Id).FirstOrDefault()?.Roles ?? new List<string>();
                if (!role.Contains(roleName))
                {
                    role.Add(roleName);
                    _users.ReplaceOne(u => u.Id == user.Id, user);
                }
            }, cancellationToken);
        }


        public Task RemoveFromRoleAsync(AiTimanUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var role = _users.Find(u => u.Id == user.Id).FirstOrDefault()?.Roles ?? new List<string>();
                if (role.Contains(roleName))
                {
                    role.Remove(roleName);
                    _users.ReplaceOne(u => u.Id == user.Id, user);
                }
            }, cancellationToken);
        }

        public Task<IList<string>> GetRolesAsync(AiTimanUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Roles as IList<string>);
        }

        public Task<bool> IsInRoleAsync(AiTimanUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Roles.Contains(roleName));
        }

        public Task<IList<AiTimanUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var usersInRole = _users.Find(u => u.Roles.Contains(roleName)).ToList();
                return usersInRole as IList<AiTimanUser>;
            }, cancellationToken);
        }
    }
}
