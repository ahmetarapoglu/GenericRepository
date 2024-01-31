namespace GenericRepository.Models.Product
{
    public class ProductGet : ProductBase
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CategoryInProduct> Categories { get; set; }
    }
    public class CategoryInProduct
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
