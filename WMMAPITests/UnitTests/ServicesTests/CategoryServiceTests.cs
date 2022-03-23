using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

namespace WMMAPITests.UnitTests
{
    [TestClass]
    public class CategoryServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;

        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();
            _tdc = new TestDataContext(_testData);
        }

        #region Get
        [TestMethod]
        public void TestGetSucceeds()
        {
            // Fabricate test data
            Category cat = _testData.Categories.First();

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.Get(cat.Id, cat.UserId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.AreEqual(cat.Id, result.CategoryId);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetNothingFoundThrowsAppException()
        {
            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.Get(Guid.NewGuid(), Guid.NewGuid());

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
        }
        #endregion

        #region GetList
        [TestMethod]
        public void TestGetList()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.GetList(userId);

            // Confirm mock and assert (ensure pulled cats are user's cats)
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            var userCats = _testData.Categories.Where(c => c.UserId == userId).Select(c => c.Id);
            foreach (var record in result)
            {
                Assert.IsTrue(userCats.Contains(record.CategoryId));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetListNothingFoundThrowsAppException()
        {
            // Fabricate test data
            Guid userId = Guid.NewGuid();

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.GetList(userId);;
        }
        #endregion

        #region CreateDefaults
        [TestMethod]
        public void TestCreateDefaultsSucceeds()
        {
            // Fabricate test data
            Guid userId = Guid.NewGuid(); // New user so no defaults

            // Initialize service and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.CreateDefaults(userId);

            // Verify mock and assert (just verify, nothing to assert)
            int defaultCatCount = Globals.DefaultCategories.GetAllDefaultCategories().Count();
            _tdc.CategorySet.Verify(m => m.Add(It.IsAny<Category>()), Times.Exactly(defaultCatCount));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        #endregion

        #region Add
        [TestMethod]
        public void TestAddCategory()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Category category = _testData.CreateTestCategory(true, "testCategory", userId, false);

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.AddCategory(category);

            // Confirm mock and assert (nothing to assert)
            _tdc.CategorySet.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestAddCategoryNameExists()
        {
            // Fabricate test
            Category existingCat = _testData.Categories.First();
            Category testCategory = _testData.CreateTestCategory(true, existingCat.Name, existingCat.UserId, false);

            // Initialize service and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.AddCategory(testCategory);

            // Confirm mock and assert (not needed; exception expected)
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(AppException))]
        public void TestAddCategoryNullOrEmptyName(string name)
        {
            // Fabricate Test
            Category category = _testData.CreateTestCategory(true);
            category.Name = name;

            // Initialize service and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.AddCategory(category);

            // Confirm mock and assert (not needed; exception expected)
        }
        #endregion

        #region Modify
        [TestMethod]
        public void TestModifyCategorySucceeds()
        {
            // Fabricate test data
            Category testCateogry = _testData.Categories.Where(c => !c.IsDefault).First();
            testCateogry.Name = "modifiedName";
            testCateogry.IsDisplayed = !testCateogry.IsDisplayed;

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ModifyCategory(testCateogry);

            // Confirm and assert (nothing to assert)
            _tdc.WMMContext.Verify(m => m.Categories, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyCategoryFailsNotFound()
        {
            // Fabricate test data
            Category testCateogry = _testData.CreateTestCategory(true, "testCategory", null, false);

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ModifyCategory(testCateogry);

            // Confirm and assert (no need, expecting exception)
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyCategoryFailsCannotBeDefault()
        {
            // Fabricate test data
            Category testCategory = _testData.Categories.Where(c => c.IsDefault).First();
            testCategory.Name = "modifedName";

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ModifyCategory(testCategory);

            // Confirm and call method (no need, expecting exception)
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyCategoryNameExists()
        {
            // Fabricate test data            
            Category testCategory = _testData.Categories.Where(c => !c.IsDefault).First();
            testCategory.Name = _testData.Categories.First(
                c => !c.IsDefault
                && c.UserId == testCategory.UserId
                && c.Id != testCategory.Id).Name;

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ModifyCategory(testCategory);

            // Confirm and call method (no need, expecting exception)
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(AppException))]
        public void TestModifyCategoryNameIsEmptyOrNull(string name)
        {
            // Fabricate test data            
            Category testCategory = _testData.Categories.Where(c => !c.IsDefault).First();
            testCategory.Name = name;

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ModifyCategory(testCategory);

            // Confirm and call method (no need, expecting exception)
        }
        #endregion

        #region Delete
        [TestMethod]
        public void TestDeleteCategorySucceeds()
        {            
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Category absorbing = GenerateCategoryWithTransactions("absorbingCat", userId, true);
            Category absorbed = GenerateCategoryWithTransactions("absorbedCat", userId, false);
            _tdc = new TestDataContext(_testData);
            _tdc.CategorySet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(absorbed);

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.DeleteCategory(absorbed.Id, absorbing.Id, userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
            _tdc.CategorySet.Verify(m => m.Remove(absorbed), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteCategoryAbsorbedDoesntExist()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Guid diffUserId = _testData.Users.Skip(1).First().Id;
            Category absorbing = GenerateCategoryWithTransactions("absorbingCat", userId, true);
            Category absorbed = GenerateCategoryWithTransactions("absorbedCat", diffUserId, false); // Diff user Id to prevent finding
            _tdc = new TestDataContext(_testData);
            _tdc.CategorySet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(absorbed);

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.DeleteCategory(absorbed.Id, absorbing.Id, userId);

            // Confirm mock and assert (not needed; exception expected)
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteCategoryAbsorbDoesntExist()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Guid diffUserId = _testData.Users.Skip(1).First().Id;
            Category absorbing = GenerateCategoryWithTransactions("absorbingCat", diffUserId, true);
            Category absorbed = GenerateCategoryWithTransactions("absorbedCat", userId, false); // Diff user Id to prevent finding
            _tdc = new TestDataContext(_testData);
            _tdc.CategorySet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(absorbed);

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.DeleteCategory(absorbed.Id, absorbing.Id, userId);

            // Confirm mock and assert (not needed; exception expected)
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteCategoryAbsorbedIsDefault()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Category absorbing = GenerateCategoryWithTransactions("absorbingCat", userId, true);
            Category absorbed = GenerateCategoryWithTransactions("absorbedCat", userId, true); // Diff user Id to prevent finding
            _tdc = new TestDataContext(_testData);
            _tdc.CategorySet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(absorbed);

            // Initialize and call method
            ICategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.DeleteCategory(absorbed.Id, absorbing.Id, userId);

            // Confirm mock and assert (not needed; exception expected)
        }
        #endregion

        #region PrivateHelpers
        private Category GenerateCategoryWithTransactions(string catName, Guid userId, bool isDefault)
        {
            Category cat = _testData.CreateTestCategory(true, catName, userId, isDefault);
            _testData.Categories = _testData.Categories.Concat(new List<Category> { cat });

            List<Transaction> transactions = new();
            for (int i = 0; i < 4; i++)
            {
                transactions.Add(_testData.CreateTestTransaction(
                    _testData.Accounts.Where(a => a.UserId == userId).First(),
                    true, 25.00M, cat.Id,
                    _testData.Vendors.Where(v => v.UserId == userId).Skip(2).First().Id
                    ));
            }
            _testData.Transactions = _testData.Transactions.Concat(transactions);

            return cat;
        }
        #endregion
    }
}
