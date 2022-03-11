using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;

namespace WMMAPITests.DataHelpers
{
    internal static class TestDataContextHelper
    {
        internal static Mock<WMMContext> GenerateMoqContext(User userData)
        {
            var context = new Mock<WMMContext>();

            var mockAccountSet = GenerateMoqDbSet(userData.Accounts.AsQueryable());
            var moqCategorySet = GenerateMoqDbSet(userData.Categories.AsQueryable());
            var moqUserSet = GenerateMoqDbSet(userData.AsQueryable());

            return context;
        }

        internal static Mock<DbSet<T>> GenerateMoqDbSet<T>(IQueryable<T> dataSet) where T : class // TODO class is too open?
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(dataSet.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(dataSet.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(dataSet.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(dataSet.GetEnumerator());
            
            return mockSet;
        }

    }
}
