using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Interfaces;
using WMMAPI.Models.CategoryModels;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _categoryRepository = categoryRepo;
        }

        //Get list

        [HttpGet("userCategories/{id}")]
        public IActionResult GetCategories(string id)
        {
            // Confirm user is the same requesting
            if (User.Identity.Name != id)
                return BadRequest(new { message = "Passed userId does not match id of authenticated user." });

            Guid userId = Guid.Parse(id);
            try
            {
                var catList = _categoryRepository.GetList(userId);
                List<CategoryModel> categories = catList.Select(c => new CategoryModel(c)).ToList();
                return Ok(categories);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            //TODO: Additional exception catching?
        }

        [HttpPost("{id}")]
        public IActionResult AddCategory(string id, AddCategoryModel model)
        {
            // Confirm user is the same requesting
            if (User.Identity.Name != id)
                return BadRequest(new { message = "Passed userId does not match id of authenticated user." });

            Guid userId = Guid.Parse(id);
            try
            {
                var category = model.ToDB(userId);
                _categoryRepository.AddCategory(category);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Edit

        //Delete
            //This needs to implement absorbtion stuffs
    }
}
