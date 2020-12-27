using Luval.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Web.Security
{
    public class ExternalUserStore<TUser> : IUserStore<TUser>, 
                                            IUserPasswordStore<TUser>,
                                            IUserEmailStore<TUser>,
                                            IUserPhoneNumberStore<TUser>,
                                            IUserTwoFactorStore<TUser>,
                                            IUserAuthenticatorKeyStore<TUser>,
                                            IUserTwoFactorRecoveryCodeStore<TUser>,
                                            IQueryableUserStore<TUser>,
                                            IUserClaimStore<TUser> where TUser : ExternalUser
    {

        #region Constructors


        public ExternalUserStore(EntityAdapter<ExternalUser> entityAdapter)
        {
            _entityAdapter = entityAdapter;
        }


        #endregion

        #region Helpers

        private EntityAdapter<ExternalUser> _entityAdapter;

        public IQueryable<TUser> Users => throw new NotImplementedException();

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

        #endregion

        #region IUserStore

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() =>
            {
                return DoAction(() =>
                {
                    _entityAdapter.Insert(user);
                });
            }, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() =>
            {
                return DoAction(() =>
                {
                    _entityAdapter.Delete(user);
                });
            }, cancellationToken);
        }

        public void Dispose()
        {
            _entityAdapter = null;
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return new Task<TUser>(() => { return (TUser)_entityAdapter.Read(userId); }, cancellationToken);
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return new Task<TUser>(() =>
            {
                return _entityAdapter.Database.ExecuteToEntityList<TUser>("SELECT * FROM {0} WHERE Email = {1}"
                    .FormatInvariant(_entityAdapter.Schema.TableName.GetFullTableName(),
                    normalizedUserName.ToSql())).FirstOrDefault();
            }, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return user.Email; }, cancellationToken);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return user.Id; }, cancellationToken);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<string>(() => { return user.Email; }, cancellationToken);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Email = normalizedName;
            return UpdateAsync(user, cancellationToken);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.Email = userName;
            return UpdateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            return new Task<IdentityResult>(() =>
            {
                return DoAction(() =>
                {
                    _entityAdapter.Update(user);
                });
            }, cancellationToken);
        }

        #endregion

        #region IPasswordStore

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region IEmailStore

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPhoneNumber

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Two Factor
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Authentication Keys
        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Recovery Codes
        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Claims
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        } 
        #endregion

    }
}
