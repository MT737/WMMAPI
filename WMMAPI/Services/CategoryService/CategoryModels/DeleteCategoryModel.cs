using System;
using System.ComponentModel.DataAnnotations;

namespace WMMAPI.Services.CategoryServices.CategoryModels
{
    public class DeleteCategoryModel
    {
        [Required]
        public Guid AbsorbedId { get; set; }

        [Required]
        public Guid AbsorbingId { get; set; }
    }
}
