﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Category absorbCat = _testData.CreateTestCategory(true, "absorbingCat", userId, false);
            Category absorbedCat = _testData.CreateTestCategory(true, "absorbedCat", userId, false);
            List<Category> cats = new List<Category> { absorbCat, absorbedCat };
            _testData.Categories = _testData.Categories.Concat(cats);

            List<Transaction> transactions = new();
            foreach (Category cat in cats)
            {
                // Loop 4 times for each category
                for (int i = 0; i < 4; i++)
                {
                    transactions.Add(_testData.CreateTestTransaction(
                        _testData.Accounts.Where(a => a.UserId == userId).First(),
                        true, 25.00M, cat.Id,
                        _testData.Vendors.Where(v => v.UserId == userId).Skip(2).First().Id
                        ));
                }
            }
            _testData.Transactions = _testData.Transactions.Concat(transactions);

            // Initialize and call method
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
