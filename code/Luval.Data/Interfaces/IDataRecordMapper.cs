using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Luval.Data.Interfaces
{
    /// <summary>
    /// Provides an implementation to convert <see cref="IDataRecord"/> into data entities
    /// </summary>
    public interface IDataRecordMapper
    {
        /// <summary>
        /// Converts a <see cref="IDataRecord"/> into an data entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="record">Data record to convert</param>
        /// <returns>A new Data Entity</returns>
        T FromDataRecord<T>(IDataRecord record);

        /// <summary>
        /// Converts a <see cref="IDataRecord"/> into an data entity 
        /// </summary>
        /// <param name="record">Data record to convert</param>
        /// <param name="entityType">Data entity type</param>
        /// <returns>A new Data Entity</returns>
        object FromDataRecord(IDataRecord record, Type entityType);

        /// <summary>
        /// Converts a data entity into a <see cref="IDataRecord"/>
        /// </summary>
        /// <param name="entity">The data entity to use</param>
        /// <returns>A new instance of a <see cref="IDataRecord"/></returns>
        IDataRecord ToDataRecord(object entity);
    }
}
