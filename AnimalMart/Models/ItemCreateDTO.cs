namespace AnimalMart.Models
{
    //DTO only used in CreateProduct pagemodel, to avoid exposing the entire model to users on frontend
    public class ItemCreateDTO
    {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Price { get; set; }
            public int? Stock { get; set; }
        }
}
