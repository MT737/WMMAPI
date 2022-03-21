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
    public class VendorServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;

        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();
            _tdc = new TestDataContext(_testData);
            _tdc.WMMContext.Setup(m => m.Set<Vendor>()).Returns(_tdc.VendorSet.Object);
        }

        #region CreateDefaults
        [TestMethod]
        public void TestCreateDefaultsSucceeds()
        {
            // Fabricate test data
            Guid userId = Guid.NewGuid(); // New user so no defaults

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.CreateDefaults(userId);

            // Verify mock and assert (just verify, nothing to assert)
            int defaultVendCount = Globals.DefaultVendors.GetAllDevaultVendors().Count();
            _tdc.VendorSet.Verify(m => m.Add(It.IsAny<Vendor>()), Times.Exactly(defaultVendCount));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        #endregion

        #region Get
        [TestMethod]
        public void TestGetSucceeds()
        {
            // Fabricate test
            Vendor testVend = _testData.Vendors.First();

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            Vendor result = service.Get(testVend.Id, testVend.UserId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.AreEqual(testVend.Id, result.Id);
            Assert.AreEqual(testVend.UserId, result.UserId);
            Assert.AreEqual(testVend.Name, result.Name);
        }

        [TestMethod]
        public void TestGetDoesntExistReturnsNull()
        {
            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            Vendor result = service.Get(Guid.NewGuid(), Guid.NewGuid());

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsNull(result);
        }
        #endregion

        #region GetList
        [TestMethod]
        public void TestGetListSucceeds()
        {
            // Fabricate test
            User user = _testData.Users.First();

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            IList<Vendor> vendors = service.GetList(user.Id);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsTrue(vendors.Count > 0);
            Assert.IsFalse(vendors.Any(v => v.UserId != user.Id));
        }

        [TestMethod]
        public void TestGetListDoesntExistReturnsEmpty()
        {
            // Fabricate test
            Guid userId = Guid.NewGuid();

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            IList<Vendor> vendors = service.GetList(userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsTrue(vendors.Count == 0);
        }
        #endregion

        #region Add
        [TestMethod]
        public void TestAddVendorSucceeds() // Failures covered in validation tests
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor vendor = _testData.CreateTestVendor(true, user.Id, false, "TestAddVendor");

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.AddVendor(vendor);

            // Confirm mock and assert (no assert required, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            _tdc.VendorSet.Verify(m => m.Add(It.IsAny<Vendor>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestAddVendorNameExists()
        {
            // Fabricate test 
            Vendor existingVendor = _testData.Vendors.First();
            Vendor testVendor = _testData.CreateTestVendor(
                true, existingVendor.UserId, false, existingVendor.Name);

            // Initialize service and call metho
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.AddVendor(testVendor);

            // Confirm mock and assert (not needed; excpetion expected)
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(AppException))]
        public void TestAddVendorNameNullOrEmpty(string name)
        {
            // Fabricate test
            Vendor vendor = _testData.CreateTestVendor(true);
            vendor.Name = name;

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.AddVendor(vendor);

            // Confirm mock and assert (not needed; excpetion expected)
        }
        #endregion

        #region Modify
        [TestMethod]
        public void TestModifyVendorSucceeds()
        {
            // Fabricate test
            Vendor vend = _testData.Vendors.First(v => !v.IsDefault);
            vend.Name = "Modified name";
            vend.IsDisplayed = !vend.IsDisplayed;

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(vend);

            // Confirm mock and assert (no assert necessary, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorVendorDoesntExist()
        {
            // Fabricate test
            Vendor vend = _testData.CreateTestVendor(true, Guid.NewGuid());
            vend.Name = "modified Name";

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(vend);

            // Confirm mock and assert (no assert necessary, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorDefault()
        {
            // Fabricate test
            Vendor vend = _testData.Vendors.First(v => v.IsDefault);
            vend.Name = "Modified name";

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(vend);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyNameExists()
        {
            // Fabricate test 
            Vendor existingVendor = _testData.Vendors.First();
            Vendor testVendor = _testData.Vendors
                .First(v => !v.IsDefault && v.UserId == existingVendor.UserId);
            testVendor.Name = existingVendor.Name;

            // Initialize service and call metho
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(testVendor);

            // Confirm mock and assert (not needed; excpetion expected)
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorNameNullOrEmpty(string name)
        {
            // Fabricate test
            Vendor testVendor = _testData.Vendors.First(v => !v.IsDefault);
            testVendor.Name = name;

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.AddVendor(testVendor);

            // Confirm mock and assert (not needed; excpetion expected)
        }
        #endregion

        #region Delete
        [TestMethod]
        public void TestDeleteVendorSucceeds()
        {
            // Fabricate test
            User user = _testData.Users.First();
            List<Vendor> vendors = _testData.Vendors.Where(v => !v.IsDefault && v.UserId == user.Id).Take(2).ToList();
            Vendor absorb = vendors.First();
            Vendor absorbed = vendors.Last();

            // Initialize service and call method
            _tdc.VendorSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(absorbed);
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert (no need to assert, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Exactly(2)); // TODO Need to return to this and settle on a single save call
            _tdc.VendorSet.Verify(m => m.Find(absorbed.Id), Times.Once());
            _tdc.VendorSet.Verify(m => m.Remove(absorbed), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteAbsorbVendorDoesntExist()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor absorb = _testData.CreateTestVendor(true, user.Id, false);
            Vendor absorbed = _testData.Vendors.First(v => !v.IsDefault && v.UserId == user.Id);

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert ( no need to confirm mock or assert)
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteVendorDefaultVendor()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor absorb = _testData.Vendors.First(v => v.IsDefault && v.UserId == user.Id);
            Vendor absorbed = _testData.Vendors.Where(v => v.IsDefault && v.UserId == user.Id).Skip(1).First();

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert (no need to confirm mock or assert)
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteAbsorbedVendorDoesntExist()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor absorb = _testData.Vendors.First(v => v.IsDefault && v.UserId == user.Id);
            Vendor absorbed = _testData.CreateTestVendor(true, user.Id, false);

            // Initialize service and call method
            IVendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert (no need to confirm mock or assert)
        }
        #endregion

        #region PrivateHelpers
        private Vendor GenerateVendorWithTransactions(string vendName, Guid userId, bool isDefault)
        {
            Vendor vend = _testData.CreateTestVendor(true, userId, isDefault, vendName);
            _testData.Vendors = _testData.Vendors.Concat(new List<Vendor> { vend });

            List<Transaction> transactions = new();
            for (int i = 0; i < 4; i++)
            {
                transactions.Add(_testData.CreateTestTransaction(
                    _testData.Accounts.Where(a => a.UserId == userId).First(),
                    true, 25.00M, 
                    _testData.Categories.Where(c => c.UserId == userId).Skip(2).First().Id,
                    vend.Id
                    ));
            }
            _testData.Transactions = _testData.Transactions.Concat(transactions);

            return vend;
        }
        #endregion
    }
}
