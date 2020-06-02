using CleanArchitectureEmpty.Application.Common.Interfaces;
using CleanArchitectureEmpty.Infrastructure.Files.Maps;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CleanArchitectureEmpty.Infrastructure.Files
{
    public class CsvFileBuilder<T> : ICsvFileBuilder<T> where T : class
    {
        public byte[] BuildFile(IEnumerable<T> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

                csvWriter.Configuration.RegisterClassMap<SampleRecordMap<T>>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }
    }
}
