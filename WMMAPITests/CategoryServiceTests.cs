using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

namespace WMMAPITests
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
            _tdc.WMMContext.Setup(m => m.Set<Category>()).Returns(_tdc.CategorySet.Object);
        }

        #region TestingHelperMethods
        [TestMethod]
        public void TestNameExistsFalse()
        {
            // Fabricate test category
            Category cat = _testData.Categories.First();
            Category testCategory = _testData.CreateTestCategory(true, cat.Name, cat.UserId);
            testCategory.Id = cat.Id;

            // Initialize service and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            bool result = service.NameExists(testCategory);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void TestNameExistsTrue()
        {
            // Fabricate test category
            Category cat = _testData.Categories.First();
            Category testCategory = _testData.CreateTestCategory(true, cat.Name, cat.UserId);

            // Initialize service and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            bool result = service.NameExists(testCategory);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(1126.80, Globals.DefaultCategories.Entertainment)]
        public void TestGetCategorySpending(double spending, string category)
        {
            // Fabricate test transactions for the given category
            Category cat = _testData.Categories.First(c => c.Name == category);
            List<Account> accounts = _testData.Accounts.Where(a => a.UserId == cat.UserId).ToList();
            List<Vendor> vendors = _testData.Vendors.Where(v => v.UserId == cat.UserId).ToList();

            List<Transaction> trans = new();
            for (int i = 0; i < 4; i++)
            {
                Account account = accounts.ElementAt(TestData._random.Next(accounts.Count()));
                Guid vendorId = vendors.ElementAt(TestData._random.Next(vendors.Count())).Id;

                trans.Add(_testData.CreateTestTransaction(account, true, (decimal)spending / 4, cat.Id, vendorId));
            }
            _testData.Transactions = _testData.Transactions.Concat(trans);

            // Initialize service and call method
            _tdc = new(_testData);
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.GetCategorySpending(cat.Id, cat.UserId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            Assert.AreEqual((decimal)spending, result);
        }

        [TestMethod]
        public void TestAbsorption()
        {
            // Fabricate test categories and transactions
            Guid userId = _testData.Users.First().Id;
            Category absorbCat = GenerateCategoryWithTransactions("absorbingCat", userId, false);
            Category absorbedCat = GenerateCategoryWithTransactions("absorbedCat", userId, false);
                        
            // Initialize service and call method
            _tdc = new TestDataContext(_testData);
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.Absorption(absorbedCat.Id, absorbCat.Id, userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(_testData.Transactions.Any(t => t.CategoryId == absorbCat.Id));
            Assert.IsFalse(_testData.Transactions.Any(t => t.CategoryId == absorbedCat.Id));
        }

        [TestMethod]
        public void TestCreateDefaults()
        {
            // Fabricate test data
            User user = _testData.CreateTestUser();

            // Initialize service and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.CreateDefaults(user.Id);

            // Verifty set category add and save changes
            int defaultCatCount = Globals.DefaultCategories.GetAllDefaultCategories().Count();
            _tdc.CategorySet.Verify(m => m.Add(It.IsAny<Category>()), Times.Exactly(defaultCatCount));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TestDefaultsExistFalse()
        {
            // Fabricate test data; throw in a couple non-default categories for good measure
            User testUser = _testData.CreateTestUser();
            List<Category> categories = new List<Category>();
            for (int i = 0; i < 2; i++)
            {
                categories.Add(_testData.CreateTestCategory(true, null, testUser.Id, false));
            }
            _testData.Categories = _testData.Categories.Concat(categories);
            _testData.Users = _testData.Users.Concat(new List<User> { testUser });

            // Initialize service and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.DefaultsExist(testUser.Id);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestDefaultsExistTrue()
        {
            // Data already fabricated in initialization

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.DefaultsExist(_testData.Users.First().Id);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateCategoryFailsNameExists()
        {
            // Fabricate test data
            var cat = _testData.Categories.First();
            Category category = _testData.CreateTestCategory(true, cat.Name, cat.UserId, false);

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ValidateCategory(category);

            // Confirm mock -- No assertion, exception expected
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateCategoryFailsNameEmpty()
        {
            // Fabricate test data
            var cat = _testData.Categories.First();
            Category category = _testData.CreateTestCategory(true, "", cat.UserId, false);

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ValidateCategory(category);

            // Confirm mock -- No assertion, exception expected
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
        }

        [TestMethod]
        public void TestValidateCategorySucceeds()
        {
            // Fabricate test data
            Category cat = _testData.Categories.First();
            Category category = _testData.CreateTestCategory(true, "NewCategory", cat.UserId, false);

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ValidateCategory(category);

            // Confirm mock; no Assert necessary
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
        }
        #endregion

        #region TestingServiceMethods
        [TestMethod]
        public void TestGetSucceeds()
        {
            // Fabricate test data
            Category cat = _testData.Categories.First();

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.Get(cat.Id, cat.UserId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.AreEqual(cat.Id, result.CategoryId);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetFailsNotFound()
        {
            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.Get(Guid.NewGuid(), Guid.NewGuid());

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
        }

        [TestMethod]
        public void TestGetList()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
        public void TestGetListShouldBeEmpty()
        {
            // Fabricate test data
            Guid userId = Guid.NewGuid();

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            var result = service.GetList(userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Categories, Times.Once());
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void TestAddCategory()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Category category = _testData.CreateTestCategory(true, "testCategory", userId, false);

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.AddCategory(category);

            // Confirm mock and assert (nothing ot assert)
            _tdc.CategorySet.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void TestModifyCategorySucceeds()
        {
            // Fabricate test data
            Category testCateogry = _testData.Categories.Where(c => !c.IsDefault).First();
            testCateogry.Name = "modifiedName";
            testCateogry.IsDisplayed = !testCateogry.IsDisplayed;

            // Initialize and call method
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
            service.ModifyCategory(testCategory);

            // Confirm and call method (no need, expecting exception)
        }

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
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
            CategoryService service = new CategoryService(_tdc.WMMContext.Object);
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
