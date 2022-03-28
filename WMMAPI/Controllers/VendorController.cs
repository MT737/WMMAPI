using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.VendorModels;
using static WMMAPI.Helpers.Globals.ErrorMessages;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly ILogger<VendorController> _logger;
        private readonly IVendorService _vendorService;

        public Guid UserId { get; set; }

        public VendorController(ILogger<VendorController> logger, IVendorService vendorRepo)
        {
            _logger = logger;
            _vendorService = vendorRepo;

            UserId = UserId = User != null ? Guid.Parse(User.Identity.Name) : Guid.Empty;
        }

        [HttpGet]
        public IActionResult GetVendors()
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                IList<VendorModel> vendors = _vendorService.GetList(UserId);
                return Ok(vendors);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPost]
        public IActionResult AddVendor([FromBody] AddVendorModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                var dbModel = model.ToDB(UserId);
                _vendorService.AddVendor(dbModel);
                return Ok(new VendorModel(dbModel));
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPut]
        public IActionResult ModifyVendor([FromBody] VendorModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                _vendorService.ModifyVendor(model.ToDB(UserId));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpDelete]
        public IActionResult DeleteVendor([FromBody] DeleteVendorModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                _vendorService.DeleteVendor(
                    model.AbsorbedVendor, 
                    model.AbsorbingVendor, 
                    UserId);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

    }
}
