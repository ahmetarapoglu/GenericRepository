namespace GenericRepository.Models.Product
{
    public class ProductUpdate : ProductBase
    {
        public int Id { get; set; }
        public List<int> Categories { get; set; }
    }
}
