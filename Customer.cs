namespace Datastructure;

public class Customer
{
    public string name;
    public int id;
    public List<orders> order;
    
    public Customer()
    { 
        this.order = new List<orders>();
    }
    
    public static Customer Parse(int id, string name)
    {
        var customer = new Customer();
        customer.name = name;
        customer.id = id;
        DataBaseHandler.InsertCustomerdata(customer);
        return customer;
    }
  
}