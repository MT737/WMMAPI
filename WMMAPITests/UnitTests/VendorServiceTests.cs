using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
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

        #region ServiceHelpers     
        [TestMethod]
        public void GetAmountSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetAmountWhenNoTransactions()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestNameExistsSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestAbsorptionSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCreateDefaultsSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestCreateDefaultsDefaultsAlreadyExistNoSave()
        {
            throw new NotImplementedException();
        }
        
        [TestMethod]
        public void TestDefaultsExistTheyDo()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDefaultsExistTheyDoNot()
        {
            throw new NotImplementedException();
        }
        
        [TestMethod]
        public void TestValidateVendorSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateVendorFailsNameExists()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateVendorFailsNameEmptyString()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ServiceTests
        [TestMethod]
        public void TestGetSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetDoesntExistReturnsNull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetListSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetListDoesntExistReturnsEmpty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestAddVendor()
        {
            throw new NotImplementedException();

            // Failures covered in validation tests
        }

        [TestMethod]
        public void TestModifyVendorSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorVendorDoesntExist()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorDefault()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDeleteVendorSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteVendorVendorDoesntExist()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteVendorDefaultVendor()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteAbsorbingVendorDoesntExist()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
