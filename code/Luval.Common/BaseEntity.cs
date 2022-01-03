using Luval.Data.Attributes;
using Luval.Data.Entities;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common
{
    public class BaseEntity : IIdBasedEntity<string>
    {
        public BaseEntity()
        {
            Id = CodeGenerator.GetCode();
        }

        [PrimaryKey]
        public string Id { get; set; }
    }

    public class BaseAuditEntity : StringKeyAuditEntity
    {
        public BaseAuditEntity()
        {
            Id = CodeGenerator.GetCode();
            UtcCreatedOn = DateTime.UtcNow;
            UtcUpdatedOn = UtcCreatedOn;
        }
    }

    public interface IValidate 
    {
        bool IsValid();
    }

}
