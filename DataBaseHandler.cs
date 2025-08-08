namespace Datastructure;
using Microsoft.Data.Sqlite;

public class DataBaseHandler
{
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

    public static void CreateDatabaseAndTable()
    {
        using var connection = new SqliteConnection(Program.DatabasePath);
        connection.Open();

        var createCustomerCmd = connection.CreateCommand();
        createCustomerCmd.CommandText = "CREATE TABLE IF NOT EXISTS Customer (Id INTEGER PRIMARY KEY, Name TEXT)";
        createCustomerCmd.ExecuteNonQuery();

        var createOrdersCmd = connection.CreateCommand();
        createOrdersCmd.CommandText = "CREATE TABLE IF NOT EXISTS Orders (Id INTEGER, Name TEXT, CustomerId INTEGER, FOREIGN KEY(CustomerId) REFERENCES Customer(Id))";
        createOrdersCmd.ExecuteNonQuery();
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

}