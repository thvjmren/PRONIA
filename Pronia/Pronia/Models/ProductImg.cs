namespace Pronia.Models
{
    public class ProductImg : BaseEntity
    {
        public string Image { get; set; }
        public bool? IsPrimary { get; set; }

        //relational
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
