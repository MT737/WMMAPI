using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Services;

namespace WMMAPITests
{
    [TestClass]
    public class CategoryServiceTests
    {
        private IQueryable<Category> _categories;
        private Mock<WMMContext> _mockContext;
        private Mock<DbSet<Category>> _mockCategorySet;
        private User _userData;

        [TestInitialize]
        public void Init()
        {
            _userData = TestDataHelper.CreateTestUser();
            _categories = _userData.Categories.AsQueryable();
            _mockContext = new Mock<WMMContext>();

            _mockCategorySet = new Mock<DbSet<Category>>();
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(_categories.Provider);
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(_categories.Expression);
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(_categories.ElementType);
            _mockCategorySet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(_categories.GetEnumerator());

            _mockContext.Setup(m => m.Categories).Returns(_mockCategorySet.Object);
            _mockContext.Setup(m => m.Set<Category>()).Returns(_mockCategorySet.Object);
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

            // Initialize service an call method
            CategoryService service = new CategoryService(_mockContext.Object);
            bool result = service.NameExists(testCategory);

            // Confirm mock and assert
            _mockContext.Verify(m => m.Categories, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestGetCategorySpending()
        {
            // Need a category Id for a category that's been used or I need to fabricate some transactions
            // Need a user Id

            throw new NotImplementedException();
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
