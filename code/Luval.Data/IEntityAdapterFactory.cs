﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data
{
    public interface IEntityAdapterFactory
    {
        IEntityAdapter<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class;
    }
}
