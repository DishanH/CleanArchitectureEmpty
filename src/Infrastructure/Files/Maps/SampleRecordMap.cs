using CsvHelper.Configuration;
using System.Globalization;
using System;

namespace CleanArchitectureEmpty.Infrastructure.Files.Maps
{
    public class SampleRecordMap<T> : ClassMap<T>
    {
        public SampleRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => typeof(T).GetProperty("Done")).
            ConvertUsing(c => Convert.ToBoolean(typeof(T).GetProperty("Done")) ? "Yes" : "No");
        }
    }
}
