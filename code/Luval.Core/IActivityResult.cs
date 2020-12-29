using System;

namespace Luval.Core
{
    public interface IActivityResult
    {
        Exception Exception { get; set; }
        ActivityInformation Information { get; set; }
        string Message { get; set; }
        ResultType ResultType { get; set; }

        T GetResultValue<T>(string keyName);
    }
}