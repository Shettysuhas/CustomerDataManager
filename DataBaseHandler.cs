namespace Datastructure;
using Microsoft.Data.Sqlite;

public class DataBaseHandler
{
    public static void CreateDatabaseAndTable()
    {
        using var connection = new SqliteConnection(Program.DatabasePath);
        connection.Open();

        var createCustomerCmd = connection.CreateCommand();
        createCustomerCmd.CommandText = "CREATE TABLE IF NOT EXISTS Customer (Id INTEGER PRIMARY KEY, Name TEXT)";
        createCustomerCmd.ExecuteNonQuery();

        var createOrdersCmd = connection.CreateCommand();
        createOrdersCmd.CommandText = "CREATE TABLE IF NOT EXISTS Orders (OrderNumber INTEGER PRIMARY KEY AUTOINCREMENT,Id INTEGER, Name TEXT, CustomerId INTEGER, FOREIGN KEY(CustomerId) REFERENCES Customer(Id))";
        createOrdersCmd.ExecuteNonQuery();
    }
    public static void InsertCustomerId(int id )
    {
        var connectionString = Program.DatabasePath;
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Customer (Id) VALUES ($id)";
            command.Parameters.AddWithValue("$id",id);
            command.ExecuteNonQuery();
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            Console.WriteLine($"Duplicate ID error: A customer with ID {id} already exists.");
            Program.AddCustomer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting customer: {ex.Message}");
        }
    }
    public static void InsertCustomerdata(Customer customer)
    {
        var connectionString = Program.DatabasePath;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = command.CommandText = "UPDATE Customer SET Name = $name WHERE Id = $id";;
            command.Parameters.AddWithValue("$id", customer.id);
            command.Parameters.AddWithValue("$name", customer.name);
            command.ExecuteNonQuery();
    }
    public static void InsertOrders(int customerId, List<orders> ordersList)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        foreach (var order in ordersList)
        {
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Orders (Id, Name, CustomerId) VALUES ($id, $name, $customerId)";
            command.Parameters.AddWithValue("$id", order.id);
            command.Parameters.AddWithValue("$name", order.name);
            command.Parameters.AddWithValue("$customerId", customerId);
            command.ExecuteNonQuery();
        }
    }
    public static List<Customer> GetCustomersWithOrders()
    {
        var customers = new List<Customer>();
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var customerCmd = connection.CreateCommand();
        customerCmd.CommandText = "SELECT Id, Name FROM Customer";
        using var reader = customerCmd.ExecuteReader();
        while (reader.Read())
        {
            var customer = new Customer
            {
                id = reader.GetInt32(0),
                name = reader.GetString(1),
                order = new List<orders>()
            };
            customers.Add(customer);
        }

        foreach (var customer in customers)
        {
            var orderCmd = connection.CreateCommand();
            orderCmd.CommandText = "SELECT Id, Name FROM Orders WHERE CustomerId = $customerId";
            orderCmd.Parameters.AddWithValue("$customerId", customer.id);
            using var orderReader = orderCmd.ExecuteReader();
            while (orderReader.Read())
            {
                customer.order.Add(new orders
                {
                    id = orderReader.GetInt32(0),
                    name = orderReader.GetString(1)
                });
            }
        }

        return customers;
    }
    public static List<string> CustomerNames()
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText="SELECT Name FROM Customer";
        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;
        var customerNames = new List<string>();
        while (reader.Read())
        {
            customerNames.Add(reader.GetString(0));
            
        }

        return customerNames;
    }
    public static Customer GetCustomerWithOrdersById(int id)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var customerCmd = connection.CreateCommand();
        customerCmd.CommandText = "SELECT Id, Name FROM Customer WHERE Id = $id";
        customerCmd.Parameters.AddWithValue("$id", id);

        using var reader = customerCmd.ExecuteReader();
        if (!reader.Read())
            return null;

        var customer = new Customer
        {
            id = reader.GetInt32(0),
            name = reader.GetString(1),
            order = new List<orders>()
        };

        var orderCmd = connection.CreateCommand();
        orderCmd.CommandText = "SELECT Id, Name FROM Orders WHERE CustomerId = $customerId";
        orderCmd.Parameters.AddWithValue("$customerId", customer.id);
        using var orderReader = orderCmd.ExecuteReader();
        while (orderReader.Read())
        {
            customer.order.Add(new orders
            {
                id = orderReader.GetInt32(0),
                name = orderReader.GetString(1)
            });
        }

        return customer;
    }

    public static void DeleteCustomer(int id)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var orderdeleteCmd = connection.CreateCommand();
        orderdeleteCmd.CommandText = "DELETE FROM Orders WHERE CustomerId = $id";
        orderdeleteCmd.Parameters.AddWithValue("$id", id);
        try
        {
            orderdeleteCmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error deleting orders for customer ID {id}: {ex.Message}");
        }
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Customer WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        
        try
        {
            command.ExecuteNonQuery();
            Console.WriteLine($"Customer with ID {id} deleted successfully.");
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error deleting customer: {ex.Message}");
          
        }
        
    }

    public static void DeleteOrder(int customerid, int id)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();  
        var orderdeleteCmd = connection.CreateCommand();
        orderdeleteCmd.CommandText = "DELETE FROM Orders WHERE CustomerId = $customerid AND Id = $id";
        orderdeleteCmd.Parameters.AddWithValue("$customerid", customerid);
        orderdeleteCmd.Parameters.AddWithValue("$id", id);
        
        try
        {
            orderdeleteCmd.ExecuteNonQuery();
            Console.WriteLine($"Order with ID {id} for customer ID {customerid} deleted successfully.");
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error deleting orders for customer ID {id}: {ex.Message}");
        }

    }
    public static bool CheckIfCustomerExists(int id)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM Customer WHERE Id = $id)";
        command.Parameters.AddWithValue("$id", id);
        var result = command.ExecuteScalar();
        return Convert.ToInt64(result) == 1;
    }
    public static bool CheckIfCustomerWithOrderExists(int customerid,int id)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM Orders WHERE  CustomerId = $customerid AND Id = $id)";
        command.Parameters.AddWithValue("$customerid", customerid);
        command.Parameters.AddWithValue("$id", id);
        var result = command.ExecuteScalar();
        return Convert.ToInt64(result) == 1;
    }
    public static void UpdateCustomerName(int id, string newName)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Customer SET Name = $name WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$name", newName);
        
        try
        {
            command.ExecuteNonQuery();
            Console.WriteLine($"Customer with ID {id} updated successfully.");
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Error updating customer: {ex.Message}");
        }
    }

    public static List<orders> DisplayAllOrdersList()
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Orders";
        using var reader = command.ExecuteReader();
        var ordersList = new List<orders>();
        while (reader.Read())
        {
            var order = new orders
            {
                orderNumber = reader.GetInt32(0),
                id = reader.GetInt32(1),
                name = reader.GetString(2)
            };
            ordersList.Add(order);
        }

        return ordersList;
    }
    public static bool OrderExists(int OrderNo)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText="SELECT EXISTS(SELECT 1 FROM Orders WHERE OrderNumber = $OrderNo)";
        command.Parameters.AddWithValue("$OrderNo", OrderNo);
        var result = command.ExecuteScalar();
        return Convert.ToInt64(result) == 1;
    }

    public static void EditOrderDetails(int orderNo, int? id, string name)
    {
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var setClauses = new List<string>();
        var parameters = new List<SqliteParameter>();

        if (id.HasValue)
        {
            setClauses.Add("Id = $id");
            parameters.Add(new SqliteParameter("$id", id.Value));
        }
        if (name != null)
        {
            setClauses.Add("Name = $name");
            parameters.Add(new SqliteParameter("$name", name));
        }

        if (setClauses.Count == 0)
            return; // Nothing to update

        var command = connection.CreateCommand();
        command.CommandText = $"UPDATE Orders SET {string.Join(", ", setClauses)} WHERE OrderNumber = $OrderNo";
        command.Parameters.AddWithValue("$OrderNo", orderNo);
        foreach (var param in parameters)
            command.Parameters.Add(param);

        command.ExecuteNonQuery();
    }

    public static List<Customer> SearchCustomerByName(string name)
    {
        var customers = new List<Customer>();
        var connectionString = Program.DatabasePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Customer WHERE Name LIKE $name";
        command.Parameters.AddWithValue("$name", "%" + name + "%");

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var customer = new Customer
            {
                id = reader.GetInt32(0),
                name = reader.GetString(1),
                order = new List<orders>()
            };
            customers.Add(customer);
        }

        foreach (var customer in customers)
        {
            var orderCmd = connection.CreateCommand();
            orderCmd.CommandText = "SELECT Id, Name FROM Orders WHERE CustomerId = $customerId";
            orderCmd.Parameters.AddWithValue("$customerId", customer.id);
            using var orderReader = orderCmd.ExecuteReader();
            while (orderReader.Read())
            {
                customer.order.Add(new orders
                {
                    id = orderReader.GetInt32(0),
                    name = orderReader.GetString(1)
                });
            }
        }

        return customers;
    }
    
}
//     Update Customer Details: Allow updating a customer's name or other info. done
//     Update Order Details: Allow updating an order's name or other info.done
//     Search Customers by Name: Find customers using partial or full name matches.done
//     List Orders for a Customer: List all orders for a specific customer.
//     Prevent Duplicate Orders: Check for duplicate order IDs before inserting.
//     Export Data: Export customers and orders to a CSV or JSON file.
//     Import Data: Import customers and orders from a file.
//     Pagination: Show customers/orders in pages if there are many.
//     Statistics: Show stats like total customers, total orders, average orders per customer.
//     Error Logging: Log errors to a file for debugging.


/*
 INSERT INTO Customer (Id, Name) VALUES
   (1, 'Alice'),
   (2, 'Bob'),
   (3, 'Charlie'),
   (4, 'Diana'),
   (5, 'Ethan'),
   (6, 'Fiona'),
   (7, 'George'),
   (8, 'Hannah'),
   (9, 'Ian'),
   (10, 'Julia'),
   (11, 'Kevin'),
   (12, 'Laura'),
   (13, 'Mike'),
   (14, 'Nina'),
   (15, 'Oscar'),
   (16, 'Paula'),
   (17, 'Quentin'),
   (18, 'Rachel'),
   (19, 'Steve'),
   (20, 'Tina');
   
 INSERT INTO Orders (Id, Name, CustomerId) VALUES
 (1, 'Order A', 1),
(2, 'Order B', 1),
(3, 'Order C', 2),
(4, 'Order D', 2),
(5, 'Order E', 3),
(6, 'Order F', 3),
(7, 'Order G', 4),
(8, 'Order H', 4),
(9, 'Order I', 5),
(10, 'Order J', 5),
(11, 'Order K', 6),
(12, 'Order L', 7),
(13, 'Order M', 8),
(14, 'Order N', 9),
(15, 'Order O', 10),
(16, 'Order P', 11),
(17, 'Order Q', 12),
(18, 'Order R', 13),
(19, 'Order S', 14),
(20, 'Order T', 15);
*/