namespace GenericRepository.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<ProductCategory> ProductCategories { get; set;}
    }
}
