using System.Collections.Generic;

namespace CleanArchitectureEmpty.Application.Common.Interfaces
{
    public interface ICsvFileBuilder<T> where T : class
    {
        byte[] BuildFile(IEnumerable<T> records);
    }
}
