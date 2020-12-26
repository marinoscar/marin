using Luval.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public class ExternalUserStore<TUser> : IUserStore<TUser> where TUser : ExternalUser
    {

        #region Constructors
        
        
        public ExternalUserStore(IDbConnection dbConnection)
        {
            Database = new Database(() => { return dbConnection; });
            _sqlDialectProvider = new SqlServerDialectProvider(SqlTableSchema.Load(typeof(TUser)));
        }

        
        #endregion


        private ISqlDialectProvider _sqlDialectProvider;
        protected Database Database { get; private set; }

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

        private Task<IdentityResult> DoNonQuery(string sql, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() => {
                return DoAction(() =>
                {
                    Database.ExecuteNonQuery(sql);
                });
            }, cancellationToken);
        }


        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            var sql = _sqlDialectProvider.GetCreateCommand(DictionaryDataRecord.FromEntity(user));
            return DoNonQuery(sql, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            var sql = _sqlDialectProvider.GetDeleteCommand(DictionaryDataRecord.FromEntity(user));
            return DoNonQuery(sql, cancellationToken);
        }

        public void Dispose()
        {
            Database = null;
            _sqlDialectProvider = null;
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var sql = string.Format("SELECT * FROM [{0}] Where Id = {1}", _sqlDialectProvider.Schema.TableName, userId.ToSql());
            return new Task<TUser>(() => {
                return Database.ExecuteToEntityList<TUser>(sql).FirstOrDefault();
            }, cancellationToken);
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return user.Id; }, cancellationToken);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.DisplayName = userName;
            return UpdateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var sql = _sqlDialectProvider.GetUpdateCommand(DictionaryDataRecord.FromEntity(user));
            return DoNonQuery(sql, cancellationToken);
        }
    }
}
