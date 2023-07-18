using System.Collections.Generic;

namespace dotnet.Entities.Common
{
    public class ListResponse<T>
{
    public int TotalCount { get; set; }
    public IEnumerable<T> Result { get; set; }
    public IEnumerable<PropertyMetaData>  ResultMetaData { get; set; }
}
}
