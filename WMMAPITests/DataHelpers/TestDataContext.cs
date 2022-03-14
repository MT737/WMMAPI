using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;

namespace WMMAPITests.DataHelpers
{
    internal class TestDataContext
    {
        internal Mock<DbSet<Account>> AccountSet { get; set; } = new Mock<DbSet<Account>>();
        internal Mock<DbSet<Category>> CategorySet { get; set; } = new Mock<DbSet<Category>>();
        internal Mock<DbSet<User>> UserSet { get; set; } = new Mock<DbSet<User>>();
        internal Mock<DbSet<Vendor>> VendorSet { get; set; } = new Mock<DbSet<Vendor>>();
        internal Mock<DbSet<Transaction>> TransactionSet { get; set; } = new Mock<DbSet<Transaction>>();
        internal Mock<WMMContext> WMMContext { get; set; }

        internal TestDataContext(TestData testData)
        {
            GenerateMoqContext(testData);
        }

        private void GenerateMoqContext(TestData testData)
        {
            UserSet = GenerateMoqDbSet(testData.Users);
            VendorSet = GenerateMoqDbSet(testData.Vendors);
            AccountSet = GenerateMoqDbSet(testData.Accounts);
            CategorySet = GenerateMoqDbSet(testData.Categories);
            TransactionSet = GenerateMoqDbSet(testData.Transactions);

            WMMContext = new Mock<WMMContext>();
            WMMContext.Setup(m => m.Users).Returns(UserSet.Object);
            WMMContext.Setup(m => m.Vendors).Returns(VendorSet.Object);
            WMMContext.Setup(m => m.Accounts).Returns(AccountSet.Object);
            WMMContext.Setup(m => m.Categories).Returns(CategorySet.Object);
            WMMContext.Setup(m => m.Transactions).Returns(TransactionSet.Object);

            WMMContext.Setup(m => m.Set<User>()).Returns(UserSet.Object);
            WMMContext.Setup(m => m.Set<Vendor>()).Returns(VendorSet.Object);
            WMMContext.Setup(m => m.Set<Account>()).Returns(AccountSet.Object);
            WMMContext.Setup(m => m.Set<Category>()).Returns(CategorySet.Object);
            WMMContext.Setup(m => m.Set<Transaction>()).Returns(TransactionSet.Object);
        }

        internal Mock<DbSet<T>> GenerateMoqDbSet<T>(IQueryable<T> dataSet) where T : class // TODO class is too open?
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
