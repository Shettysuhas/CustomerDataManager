using System.Reflection.Metadata;
using Datastructure;

public class Program
{
    public const String  DatabasePath = "Data Source=/Volumes/Worksapce/C#Project/Datastructure/mydb.db";
    public static void Main(string[] args)
    {
      Initialize();
      MenuScreen();
    }
    public static void MenuScreen()
    {
        Console.WriteLine("________________Menu________________\n" +
                          "1.Add Customer\n" +
                          "2.Add orders to customers\n" +
                          "3.Get All Customers with Orders\n" +
                          "4.Get Customer with Orders by Id\n" +
                          "5.Get All Customers\n" +
                          "6.Delete Customer\n" +
                          "7.Delete order\n"+
                          "8.EXIT");;
        int  choice = int.Parse(Console.ReadLine());
        switch (choice)
        {
            case 1:AddCustomer();
                break;
            case 2:AddOrdersToCustomer();
                break;
            case 3:GetAllCustomersWithOrders();
                break;
            case 4:GetCustomerWithOrdersById();
                break;
            case 5:GetAllCustomers();
                break;
            case 6:DeleteCustomer();
                break;
            case 7:DeleteOrder();
                break;
            case 8:Console.WriteLine("Exiting...");
                return;
            default:
                Console.WriteLine("Invalid choice, please try again.");
                MenuScreen();
                break;
        }
    }

    public static void DeleteCustomer()
    {
        
    }

    public static void GetAllCustomers()
    {
    }

    public static void DeleteOrder()
    {
    }

    public static void GetCustomerWithOrdersById()
    {
    }

    public static void GetAllCustomersWithOrders()
    {
        var customersWithOrders = DataBaseHandler.GetCustomersWithOrders();
        foreach (var cdata in customersWithOrders)
        {
            Console.WriteLine($"Customer ID: {cdata.id}, Name: {cdata.name}");
            foreach (var order in cdata.order)
            {
                Console.WriteLine($"  Order ID: {order.id}, Name: {order.name}");
            }
        }
        MenuScreen();
    }

    public static void AddOrdersToCustomer()
    {
        Console.WriteLine("Enter id of Customer {i}:");
     var id = int.Parse(Console.ReadLine() ?? "0");
     List<orders> ordersList = new List<orders>();
     Console.WriteLine("Enter the number of orders for this customer:");
     int orderCount = int.Parse(Console.ReadLine() ?? "0");
     for (int j = 0; j < orderCount; j++)
     {
         Console.WriteLine("Enter id  of order {i}:");
         var orderid = int.Parse(Console.ReadLine() ?? "0");
         Console.WriteLine("Enter Name of order {i}:");
         var ordername = Console.ReadLine();
         var order = orders.Parse(orderid, ordername);
         ordersList.Add(order);
     }
        DataBaseHandler.InsertOrders(id, ordersList);
        MenuScreen();
    }

    public static void AddCustomer()
    {
        Console.WriteLine("Enter id of Customer {i}:");
        var id = int.Parse(Console.ReadLine() ?? "0");
        DataBaseHandler.InsertCustomerId(id);
        Console.WriteLine("Enter Name of Customer {i}:");
        var name = Console.ReadLine();
        var customer = Customer.Parse(id, name);
        Console.WriteLine("Enter the number of orders for this customer:");
        int orderCount = int.Parse(Console.ReadLine() ?? "0");
        for (int j = 0; j < orderCount; j++)
        {
            Console.WriteLine("Enter id  of order {i}:");
            var orderid = int.Parse(Console.ReadLine() ?? "0");
            Console.WriteLine("Enter Name of order {i}:");
            var ordername = Console.ReadLine();
            customer.order.Add(orders.Parse(orderid, ordername));
        }
        DataBaseHandler.InsertOrders(customer.id,customer.order);
        MenuScreen();
    }

    private static void Initialize()
    {
        DataBaseHandler.CreateDatabaseAndTable();
    }
}
