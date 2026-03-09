namespace order_service.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public double TotalAmount { get; set; }
}
