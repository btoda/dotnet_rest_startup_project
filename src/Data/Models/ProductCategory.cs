using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    // Notice we inherit from Metadata, which is a class containing audit fields
    public class ProductCategory : Metadata
    {
        // Notice the attributes here - they specify an integer auto-increment primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        // Notice the attribute here specifying a text column with at most 100 characters
        [MaxLength(100)]
        public string Name { get; set; }

        // No attribute means a text column with a maximum length that the db allows
        public string Description { get; set; }

        public int ProductCategoryTypeId { get; set; }
        [ForeignKey("ProductCategoryTypeId")]
        public ProductCategoryType ProductCategoryType { get; set; }
    }
}