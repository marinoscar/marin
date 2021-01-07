using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface IIdBasedEntity<TKey> 
    {
        TKey Id { get; set; }
    }
}
