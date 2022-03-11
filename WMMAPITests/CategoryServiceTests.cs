
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

namespace WMMAPITests
{
    [TestClass]
    public class CategoryServiceTests
    {
        private User _userData;
        private IQueryable<Category> _categories;
        private IQueryable<Transaction> _transactions;
        private Mock<DbSet<Category>> _mockCategorySet;
        private Mock<DbSet<Transaction>> _mockTransactionSet;
        private Mock<WMMContext> _mockContext;

        [TestInitialize]
        public void Init()
        {
            _userData = TestDataHelper.CreateTestUser();
            _categories = _userData.Categories.AsQueryable();
            _transactions = _userData.Transactions.AsQueryable();

            // TODO This is becoming too redundant. What about a context helper class? Context builder?

            _mockCategorySet = new Mock<DbSet<Category>>();
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(_categories.Provider);
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(_categories.Expression);
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(_categories.ElementType);
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(_categories.GetEnumerator());

            _mockTransactionSet = new Mock<DbSet<Transaction>>();
            _mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(_transactions.Provider);
            _mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(_transactions.Expression);
            _mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(_transactions.ElementType);
            _mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(_transactions.GetEnumerator());

            _mockContext = new Mock<WMMContext>();
            _mockContext.Setup(m => m.Categories).Returns(_mockCategorySet.Object);
            _mockContext.Setup(m => m.Set<Category>()).Returns(_mockCategorySet.Object);
            _mockContext.Setup(m => m.Transactions).Returns(_mockTransactionSet.Object);
        }

        #region TestingHelperMethods
        [TestMethod]
        public void TestNameExistsFalse()
        {
            // Fabricate test category
            Category cat = _categories.First();
            Category testCategory = TestDataHelper.CreateTestCategory(true, cat.Name, cat.UserId);
            testCategory.Id = cat.Id;

            // Initialize service and call method
            CategoryService service = new CategoryService(_mockContext.Object);
            bool result = service.NameExists(testCategory);

            // Confirm mock and assert
            _mockContext.Verify(m => m.Categories, Times.Once());
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void TestNameExistsTrue()
        {
            // Fabricate test category
            Category cat = _categories.First();
            Category testCategory = TestDataHelper.CreateTestCategory(true, cat.Name, cat.UserId);

            // Initialize service and call method
            CategoryService service = new CategoryService(_mockContext.Object);
            bool result = service.NameExists(testCategory);

            // Confirm mock and assert
            _mockContext.Verify(m => m.Categories, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(1126.80, Globals.DefaultCategories.Entertainment)]
        public void TestGetCategorySpending(double spending, string category)
        {
            // Fabricate test transactions for the given category
            Guid categoryId = _userData.Categories.First(c => c.Name == category).Id;
            
            for (int i = 0; i < 4; i++)
            {
                Account account = _userData.Accounts.ElementAt(TestDataHelper._random.Next(_userData.Accounts.Count()));
                Guid vendorId = _userData.Vendors.ElementAt(TestDataHelper._random.Next(_userData.Vendors.Count())).Id;
                
                _userData.Transactions.Add(TestDataHelper.CreateTestTransaction(account, true, (decimal)spending / 4, categoryId, vendorId));
            }

            // Initialize service and call method
            CategoryService service = new CategoryService(_mockContext.Object);
            var result = service.GetCategorySpending(categoryId, _userData.Id);

            // Confirm mock and assert
            _mockContext.Verify(m => m.Transactions, Times.Once());
            Assert.AreEqual((decimal)spending, result);
        }

        [TestMethod]
        public void TestAbsorption()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCreateDefaults()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDefaultsExistFalse()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDefaultsExistTrue()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestValidateCategoryFails()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestValidateCategorySucceeds()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region TestingServiceMethods
        [TestMethod]
        public void TestGet()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetList()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestAddCategory()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ModifyCategory()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteCategory()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
