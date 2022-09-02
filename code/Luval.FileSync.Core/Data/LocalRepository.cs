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

        public LocalRepository()
        {
            LocalDb.Initialize();
            var uowFactory = LocalDb.CreateUoWFactory();
            MediaFileUoW = uowFactory.Create<MediaFile>();
        }

        protected IUnitOfWork<MediaFile> MediaFileUoW { get; private set; }

        public void AddItem(MediaFile mediaFile)
        {
            MediaFileUoW.AddAndSave(mediaFile);
        }

    }
}
