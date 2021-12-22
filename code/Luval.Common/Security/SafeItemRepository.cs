using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Luval.Data;
using Luval.Data.Extensions;
using Luval.Data.Interfaces;

namespace Luval.Common.Security
{
    public class SafeItemRepository : ISafeItemRepository
    {
        public SafeItemRepository(IUnitOfWork<SafeItem, string> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        protected IUnitOfWork<SafeItem, string> UnitOfWork { get; private set; }

        public Task<int> AddOrUpdateAsync(string name, string unEncryptedValue, CancellationToken cancellationToken)
        {
            return AddOrUpdateAsync(name, unEncryptedValue, cancellationToken);
        }

        public async Task<int> AddOrUpdateAsync(string name, string unEncryptedValue, string userId, CancellationToken cancellationToken)
        {
            var result = await GetByNameAsync(name, cancellationToken);
            if (result == null)
                return await UnitOfWork.AddAndSaveAsync(new SafeItem()
                {
                    CreatedByUserId = userId,
                    UpdatedByUserId = userId,
                    ItemName = name,
                    ItemValue = SafeString.EncryptString(unEncryptedValue)
                }, cancellationToken);

            result.UtcUpdatedOn = DateTime.UtcNow;
            result.UpdatedByUserId = userId;
            result.ItemValue = SafeString.EncryptString(unEncryptedValue);
            return await UnitOfWork.UpdateAndSaveAsync(result, cancellationToken);
        }

        public async Task<SafeItem> GetByNameAsync(string itemName, CancellationToken cancellationToken)
        {
            var res = (await UnitOfWork.Entities.Query.GetAsync(i => i.ItemName == itemName, cancellationToken)).FirstOrDefault();
            return DecryptItem(res);
        }

        public async Task<SafeItem> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var res = await UnitOfWork.Entities.Query.GetAsync(i => i.Id == id, cancellationToken);
            return DecryptItem(res.FirstOrDefault());
        }

        public async Task<int> DeleteByNameAsync(string itemName, CancellationToken cancellationToken)
        {
            var item = await GetByNameAsync(itemName, cancellationToken);
            if (item == null) return 0;
            return await UnitOfWork.RemoveAndSaveAsync(item, cancellationToken);
        }

        public async Task<int> DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            var item = await GetByIdAsync(id, cancellationToken);
            if (item == null) return 0;
            return await UnitOfWork.RemoveAndSaveAsync(item, cancellationToken);
        }

        public async Task<int> DeleteAsync(SafeItem item, CancellationToken cancellationToken)
        {
            if (item == null) return 0;
            return await UnitOfWork.RemoveAndSaveAsync(item, cancellationToken);
        }

        private SafeItem DecryptItem(SafeItem item)
        {
            if (item == null) return item;
            item.ItemValue = SafeString.DecryptString(item.ItemValue);
            return item;
        }


    }
}
