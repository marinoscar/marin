using Luval.Media.Gallery.Entities;

namespace Luval.Media.Gallery
{
    public interface IMediaGalleryRepository
    {
        void InsertOrUpdateToken(MediaAuthorization authorization);
    }
}