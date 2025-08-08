namespace Datastructure;

public class orders
{
    public int orderNumber;
    public int id;
    public string name;
    
    public static orders Parse(int id, string name)
    {
        var order = new orders();
        order.orderNumber = 0; 
        order.name = name;
        order.id = id;
        return order;
    }
}