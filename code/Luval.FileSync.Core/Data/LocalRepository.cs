using Luval.DataStore;
using Luval.DataStore.Database.Sqlite;
using Luval.DataStore.Extensions;
using Luval.FileSync.Core.Data.Store;
using Luval.FileSync.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core.Data
{
    public class LocalRepository
    {

        public LocalRepository() : this(LocalDb.CreateUoWFactory())
        {
        }

        public LocalRepository(IUnitOfWorkFactory unitOfWorkFactory)
        {
            LocalDb.Initialize();
            MediaFileUoW = unitOfWorkFactory.Create<LocalMediaFile>();
        }

        protected IUnitOfWork<LocalMediaFile> MediaFileUoW { get; private set; }

        public int AddItem(LocalMediaFile mediaFile)
        {
            var fileInStorage = TryGetFromStorage(mediaFile);
            if (fileInStorage == null)
                return MediaFileUoW.AddAndSave(mediaFile);
            return 0;
        }

        public LocalMediaFile TryGetFromStorage(LocalMediaFile mediaFile)
        {
            var hashes = MediaFileUoW.Entities.Query(i => i.Hash == mediaFile.Hash);
            if (hashes.Any() && hashes.Count() == 1) return hashes.First();
            return GetByFileHashAndName(mediaFile.Hash, mediaFile.LocationInDevice);
        }

        public LocalMediaFile GetByFileHashAndName(string? fileHash, string? fileName)
        {
            return MediaFileUoW.Entities.Query(i => i.Hash == fileHash && i.LocationInDevice == fileName).FirstOrDefault();
        }

    }
}
