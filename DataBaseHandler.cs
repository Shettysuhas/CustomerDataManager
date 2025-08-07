namespace Datastructure;
using Microsoft.Data.Sqlite;

public class DataBaseHandler
{
    // public static void InitializeDB()
    // {
    //     string dbPath = "/Volumes/Worksapce/C#Project/Datastructure/mydb.db";
    //     if (File.Exists(dbPath))
    //     {
    //         File.Delete(dbPath);
    //     }
    // }
     public static void InsertCustomer(Customer customer)
    {
        var connectionString = "Data Source=/Volumes/Worksapce/C#Project/Datastructure/mydb.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Customer (Id, Name) VALUES ($id, $name)";
        command.Parameters.AddWithValue("$id", customer.id);
        command.Parameters.AddWithValue("$name", customer.name);
        command.ExecuteNonQuery();
    }
    public static void InsertOrders(int customerId, List<orders> ordersList)
    {
        var connectionString = "Data Source=/Volumes/Worksapce/C#Project/Datastructure/mydb.db";
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
        using var connection = new SqliteConnection("Data Source=/Volumes/Worksapce/C#Project/Datastructure/mydb.db");
        connection.Open();

        var createCustomerCmd = connection.CreateCommand();
        createCustomerCmd.CommandText = "CREATE TABLE IF NOT EXISTS Customer (Id INTEGER PRIMARY KEY, Name TEXT)";
        createCustomerCmd.ExecuteNonQuery();

        var createOrdersCmd = connection.CreateCommand();
        createOrdersCmd.CommandText = "CREATE TABLE IF NOT EXISTS Orders (Id INTEGER PRIMARY KEY, Name TEXT, CustomerId INTEGER, FOREIGN KEY(CustomerId) REFERENCES Customer(Id))";
        createOrdersCmd.ExecuteNonQuery();
    }
    public static List<Customer> GetCustomersWithOrders()
    {
        var customers = new List<Customer>();
        var connectionString = "Data Source=/Volumes/Worksapce/C#Project/Datastructure/mydb.db";
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
    public static Customer? GetCustomerWithOrdersById(int id)
    {
        var connectionString = "Data Source=/Volumes/Worksapce/C#Project/Datastructure/mydb.db";
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
    
}