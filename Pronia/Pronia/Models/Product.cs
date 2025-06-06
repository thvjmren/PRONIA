﻿namespace Pronia.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }

        //relational
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductImg> ProductImgs { get; set; }
    }
}
