using Microsoft.AspNetCore.Http;
using WMMAPI.Services.VendorService.VendorModels;

namespace WMMAPITests.UnitTests.ControllerTests
{
    [TestClass]
    public class VendorControllerTests
    {
        private TestData _td;
        private Mock<ILogger<VendorController>> _mockLogger;
        private Mock<IVendorService> _mockVendorService;

        [TestInitialize]
        public void Initialize()
        {
            _td = new TestData();
            _mockLogger = new Mock<ILogger<VendorController>>();
            _mockVendorService = new Mock<IVendorService>();
        }

        #region Get
        [TestMethod]
        public void GetShouldReturnViewModel()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testVendors = GenerateTestVendorModels(userId);
            _mockVendorService.Setup(m => m.GetList(userId)).Returns(testVendors);
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetVendors();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(IList<VendorModel>));
            var vendors = (IList<VendorModel>)obj.Value;
            Assert.AreEqual(5, vendors.Count);
        }

        [TestMethod]
        public void GetAppExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            _mockVendorService.Setup(m => m.GetList(userId)).Throws(new AppException());
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetVendors();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void GetExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            _mockVendorService.Setup(m => m.GetList(userId)).Throws(new Exception());
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetVendors();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void GetEmptyUserGuidHandled()
        {
            // Arrange test
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.GetVendors();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Add
        [TestMethod]
        public void AddShouldReturnViewModel()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var model = GenerateTestAddVendorModel();            
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.AddVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = (ObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(VendorModel));
            var vendor = (VendorModel)obj.Value;
            Assert.AreEqual(model.Name, vendor.Name);
        }

        [TestMethod]
        public void AddAppExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var model = GenerateTestAddVendorModel();
            _mockVendorService.Setup(m => m.AddVendor(It.IsAny<Vendor>())).Throws(new AppException());
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.AddVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void AddExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var model = GenerateTestAddVendorModel();
            _mockVendorService.Setup(m => m.AddVendor(It.IsAny<Vendor>())).Throws(new Exception());
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.AddVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void AddEmptyUserGuidHandled()
        {
            // Arrange test
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.AddVendor(new AddVendorModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Modify
        [TestMethod]
        public void ModifyVendorReturnsEmptyOk()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            UpdateVendorModel model = GenerateTestModifyVendorModel(_td.CreateTestVendor(true));
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;
            
            // Call action
            var result = controller.ModifyVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var obj = (StatusCodeResult)result;
            Assert.AreEqual(StatusCodes.Status204NoContent, obj.StatusCode);
        }

        [TestMethod]
        public void ModifyAppExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            UpdateVendorModel model = GenerateTestModifyVendorModel(_td.CreateTestVendor(true));
            _mockVendorService.Setup(m => m.ModifyVendor(It.IsAny<Vendor>())).Throws(new AppException());
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.ModifyVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void ModifyExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            UpdateVendorModel model = GenerateTestModifyVendorModel(_td.CreateTestVendor(true));
            _mockVendorService.Setup(m => m.ModifyVendor(It.IsAny<Vendor>())).Throws(new Exception());
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.ModifyVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void ModifyEmptyUserGuidHandled()
        {
            // Arrange test
            UpdateVendorModel model = GenerateTestModifyVendorModel(_td.CreateTestVendor(true));
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.ModifyVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteVendorReturnsEmptyOk()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            DeleteVendorModel model = GenerateDeleteVendorModel();
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.DeleteVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var obj = (StatusCodeResult)result;
            Assert.AreEqual(StatusCodes.Status204NoContent, obj.StatusCode);
        }

        [TestMethod]
        public void DeleteVendorAppExceptionHandled()
        {
            // Arrange test
            DeleteVendorModel model = GenerateDeleteVendorModel();
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = Guid.NewGuid();
            _mockVendorService.Setup(m => m.DeleteVendor(model.AbsorbedVendor, model.AbsorbingVendor, controller.UserId))
                .Throws(new AppException());

            // Call action
            var result = controller.DeleteVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void DeleteVendorExceptionHandled()
        {
            // Arrange test
            DeleteVendorModel model = GenerateDeleteVendorModel();
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = Guid.NewGuid();
            _mockVendorService.Setup(m => m.DeleteVendor(model.AbsorbedVendor, model.AbsorbingVendor, controller.UserId))
                .Throws(new Exception());

            // Call action
            var result = controller.DeleteVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void DeleteVendorEmptyUserGuidHandled()
        {
            // Arrange test
            DeleteVendorModel model = GenerateDeleteVendorModel();
            VendorController controller = new(_mockLogger.Object, _mockVendorService.Object);
            controller.UserId = Guid.Empty;
            
            // Call action
            var result = controller.DeleteVendor(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region TestHelpers
        private IList<VendorModel> GenerateTestVendorModels(Guid userId)
        {
            IList<VendorModel> models = new List<VendorModel>();
            for (int i = 0; i < 5; i++)
            {
                models.Add(new VendorModel(_td.CreateTestVendor(true, userId: userId)));
            }
            return models;
        }

        private AddVendorModel GenerateTestAddVendorModel()
        {
            return new AddVendorModel
            {
                Name = "TestVendorName",
                IsDisplayed = true
            };
        }

        private UpdateVendorModel GenerateTestModifyVendorModel(Vendor vendor)
        {     
            return new UpdateVendorModel
            {
                Id = vendor.Id,
                Name = vendor.Name,
                IsDisplayed = vendor.IsDisplayed
            };
        }

        private DeleteVendorModel GenerateDeleteVendorModel()
        {
            return new DeleteVendorModel()
            {
                AbsorbedVendor = Guid.NewGuid(),
                AbsorbingVendor = Guid.NewGuid()
            };
        }
        #endregion
    }
}
