using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.UrlShortner
{
    public class UrlShortnerRepository : IUrlShortnerRepository
    {

        private IUnitOfWork<ShortName, string> _unitOfWork;

        public UrlShortnerRepository(IUnitOfWorkFactory factory)
        {
            UnitOfWorkFactory = factory;
            _unitOfWork = factory.Create<ShortName, string>();
        }

        protected IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }

        public async Task<ShortName> AddUrlAsync(string id, string uri, CancellationToken cancellationToken)
        {
            var item = new ShortName() { Id = id, OriginalUri = uri };
            await _unitOfWork.AddAndSaveAsync(item, cancellationToken);
            return item;
        }

        public async Task<ShortName> AddUrlAsync(string uri, CancellationToken cancellationToken)
        {
            return await AddUrlAsync(Guid.NewGuid().ToString(), uri, cancellationToken);
        }

        public async Task<ShortName> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Entities.Query.GetAsync(id, cancellationToken);
        }

        public async Task<int> UpdateAsync(ShortName item, CancellationToken cancellationToken)
        {
            item.UtcUpdatedOn = DateTime.UtcNow;
            item.RecordCount = item.RecordCount + 1;
            return await _unitOfWork.UpdateAndSaveAsync(item, cancellationToken);
        }

        public async Task<int> UpdateAsync(string id, CancellationToken cancellationToken)
        {
            var item = await GetByIdAsync(id, cancellationToken);
            if (item == null) throw new ArgumentException("Unable to find item with id: {0}".Format(id));
            return await _unitOfWork.UpdateAndSaveAsync(item, cancellationToken);
        }

    }
}
