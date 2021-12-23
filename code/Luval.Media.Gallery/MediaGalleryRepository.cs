using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.Media.Gallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery
{
    public class MediaGalleryRepository : IMediaGalleryRepository
    {
        IUnitOfWork<MediaItem, string> _unitOfWork;

        public MediaGalleryRepository(IUnitOfWork<MediaItem, string> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateAsync(MediaItem item, CancellationToken cancellationToken)
        {
            return await _unitOfWork.AddAndSaveAsync(item, cancellationToken);
        }

        public async Task<int> UpdateAsync(MediaItem item, CancellationToken cancellationToken)
        {
            return await _unitOfWork.UpdateAndSaveAsync(item, cancellationToken);
        }

        public async Task<int> CreateOrUpdateAsync(MediaItem item, CancellationToken cancellationToken)
        {
            int result;
            var dbCopy = await _unitOfWork.Entities.Query.GetAsync(i => i.MediaId == item.MediaId, cancellationToken);
            if (dbCopy == null || !dbCopy.Any())
            {
                item.Id = dbCopy.First().Id;
                result = await UpdateAsync(item, cancellationToken);
            }
            else
                result = await CreateAsync(item, cancellationToken);
            return result;
        }

        public void InsertOrUpdateToken(MediaAuthorization authorization)
        {

        }
    }
}
