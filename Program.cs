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
        int choice;
        while (true)
        {
            var input = Console.ReadLine();
            if(Int32.TryParse(input,out choice))
                break;
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        
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
    
    public static void DeleteOrder()
    {
        int customerid;
        while (true)
        {
            Console.WriteLine("Enter id of Customer {i}:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out customerid))
                if (DataBaseHandler.CheckIfCustomerExists(customerid))
                    break;
                else
                {
                    Console.WriteLine($"Customer with ID {customerid} does not exist. Please enter a valid customer ID.");
                    continue;
                }
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        int orderid;
        while (true)
        {
            Console.WriteLine($"Enter order id of Customer {customerid}:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out orderid))
                if (DataBaseHandler.CheckIfCustomerWithOrderExists(customerid,orderid))
                    break;
                else
                {
                    Console.WriteLine($"Customer with ID Order of Orderid{orderid} does not exist. Please enter a valid customer ID.");
                    continue;
                }
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        DataBaseHandler.DeleteOrder(customerid,orderid);
        MenuScreen();
    }

    public static void DeleteCustomer()
    {
        int id;
        while (true)
        {
            Console.WriteLine("Enter id of Customer {i}:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out id))
                break;
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        DataBaseHandler.DeleteCustomer(id);
        MenuScreen();
    }
    public static void GetAllCustomers()
    {
        var customers = DataBaseHandler.CustomerNames();
        if (customers != null && customers.Count > 0)
        {
            Console.WriteLine($"<<<<<CUSTOMERS>>>>>>");
            foreach (var customer in customers)
            {
                Console.WriteLine(customer);
            }
        }
        else
        {
            Console.WriteLine("No customers found.");
        }
        MenuScreen();
    }
    
    public static void GetCustomerWithOrdersById()
    {
        int id;
        while (true)
        {
            Console.WriteLine("Enter id of Customer {i}:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out id))
                break;
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        var customer = DataBaseHandler.GetCustomerWithOrdersById(id);
        if (customer != null)
        {
            Console.WriteLine($"Customer ID: {customer.id}, Name: {customer.name}");
            foreach (var order in customer.order)
            {
                Console.WriteLine($"  Order ID: {order.id}, Name: {order.name}");
            }
        }
        else
        {
            Console.WriteLine("Customer not found.");
        }
        MenuScreen();
    }

    public static void GetAllCustomersWithOrders()
    {
        var customersWithOrders = DataBaseHandler.GetCustomersWithOrders();
        if (customersWithOrders.Count == 0)
        {
            Console.WriteLine($" No customers found.");
        }
        else
        {
            foreach (var cdata in customersWithOrders)
            {
                Console.WriteLine($"Customer ID: {cdata.id}, Name: {cdata.name}");
                foreach (var order in cdata.order)
                {
                    Console.WriteLine($"  Order ID: {order.id}, Name: {order.name}");
                }
            }
        }
        MenuScreen();
    }

    public static void AddOrdersToCustomer()
    {
        int id;
        while (true)
        {
            Console.WriteLine("Enter id of Customer {i}:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out id))
                break;
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
     List<orders> ordersList = new List<orders>();
     int orderCount;
     while (true)
     {
         Console.WriteLine("Enter the number of orders for this customer:");
         var input = Console.ReadLine();
         if (int.TryParse(input, out orderCount))
             break;
         Console.WriteLine("Invalid input. Please enter a valid integer.");
     }
     for (int j = 0; j < orderCount; j++)
     {
         int orderid;
         while (true)
         {
             Console.WriteLine("Enter id of order {i}:");
             var input = Console.ReadLine();
             if (int.TryParse(input, out orderid))
                 break;
             Console.WriteLine("Invalid input. Please enter a valid integer.");
         }
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
        int id;
        while (true)
        {
            Console.WriteLine("Enter id of Customer {i}:");
            var input = Console.ReadLine();
            if (int.TryParse(input, out id))
                break;
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
        
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
