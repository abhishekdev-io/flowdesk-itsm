using FlowDesk.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using MockQueryable.NSubstitute;

namespace FlowDesk.Application.UnitTests.Common;

public static class DbContextMockFactory
{
    public static IApplicationDbContext Create() => Substitute.For<IApplicationDbContext>();

    public static DbSet<T> MockDbSet<T>(IEnumerable<T> seed) where T : class
    {
        return NSubstituteExtensions.BuildMockDbSet(seed.ToList());
    }
}
