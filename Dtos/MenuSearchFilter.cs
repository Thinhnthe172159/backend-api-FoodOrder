namespace FoodOrder.Dtos
{
    public class MenuSearchFilter
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
