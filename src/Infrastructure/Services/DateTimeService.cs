using CleanArchitectureEmpty.Application.Common.Interfaces;
using System;

namespace CleanArchitectureEmpty.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
