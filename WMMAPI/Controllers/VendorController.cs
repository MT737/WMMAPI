using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.VendorModels;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly ILogger<VendorController> _logger;
        private readonly IVendorRepository _vendorRepository;

        public VendorController(ILogger<VendorController> logger, IVendorRepository vendorRepo)
        {
            _logger = logger;
            _vendorRepository = vendorRepo;
        }

        [HttpGet]
        public IActionResult GetVendors()
        {
            try
            {
                List<VendorModel> vendors = _vendorRepository
                    .GetList(Guid.Parse(User.Identity.Name))
                    .Select(v => new VendorModel(v)).ToList();
                return Ok(vendors);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddVendor([FromBody] AddVendorModel model)
        {
            try
            {
                var dbModel = model.ToDB(Guid.Parse(User.Identity.Name));
                _vendorRepository.AddVendor(dbModel);
                return Ok(new VendorModel(dbModel));
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ModifyVendor([FromBody] VendorModel model)
        {
            try
            {
                _vendorRepository.ModifyVendor(model.ToDB(Guid.Parse(User.Identity.Name)));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Delete Vendor
        [HttpDelete]
        public IActionResult DeleteVendor([FromBody] DeleteVendorModel model)
        {
            try
            {
                _vendorRepository.DeleteVendor(
                    model.AbsorbedVendor, 
                    model.AbsorbingVendor, 
                    Guid.Parse(User.Identity.Name));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
