namespace GenericRepository.Models.Product
{
    public class ProductCreate : ProductBase
    {
        public List<int> Categories { get; set; }
    }
}
