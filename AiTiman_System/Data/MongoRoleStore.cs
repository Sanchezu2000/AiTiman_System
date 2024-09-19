using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace AiTIman_System.Data
{
    public class MongoRoleStore : IRoleStore<IdentityRole>
    {
        private readonly IMongoCollection<IdentityRole> _roles;

        public MongoRoleStore(MongoDbContext context)
        {
            _roles = context.Roles;
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            await _roles.InsertOneAsync(role, cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            var result = await _roles.DeleteOneAsync(r => r.Id == role.Id, cancellationToken);
            return result.DeletedCount > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Role not found" });
        }

        public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return await _roles.Find(r => r.Id == roleId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            var result = await _roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Role not found" });
        }

        public void Dispose()
        {
            // No resources to dispose
        }
    }
}
