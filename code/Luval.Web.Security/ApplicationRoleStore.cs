using Luval.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public class ApplicationRoleStore<TRole> : IRoleStore<TRole> where TRole : ApplicationRole
    {
        public ApplicationRoleStore(SqlEntityAdapter<ApplicationRole> entityAdapter)
        {
            _entityAdapter = entityAdapter;
        }

        private SqlEntityAdapter<ApplicationRole> _entityAdapter;

        private IdentityResult DoAction(Action action)
        {
            var res = IdentityResult.Success;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                res = IdentityResult.Failed(new IdentityError() { Code = string.Format("ErrorOn:{0} Target:{1}", action.Method.Name, action.Target.GetType().Name), Description = ex.Message });
            }
            return res;
        }

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() =>
            {
                return DoAction(() =>
                {
                    _entityAdapter.Insert(role);
                });
            }, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() =>
            {
                return DoAction(() =>
                {
                    _entityAdapter.Delete(role);
                });
            }, cancellationToken);
        }

        public void Dispose()
        {
            _entityAdapter = null;
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return new Task<TRole>(() => { return (TRole)_entityAdapter.Read(roleId); }, cancellationToken);
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return new Task<TRole>(() => {
                return _entityAdapter.Database.ExecuteToEntityList<TRole>("SELECT * FROM {0} WHERE Name = {1}"
                    .FormatInvariant(_entityAdapter.Schema.TableName.GetFullTableName(),
                    normalizedRoleName.ToSql())).FirstOrDefault();
            }, cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return role.RoleName; }, cancellationToken);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return role.Id; }, cancellationToken);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return role.RoleName; }, cancellationToken);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.RoleName = normalizedName;
            return UpdateAsync(role, cancellationToken);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            role.RoleName = roleName;
            return UpdateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() =>
            {
                return DoAction(() =>
                {
                    _entityAdapter.Update(role);
                });
            }, cancellationToken);
        }
    }
}
