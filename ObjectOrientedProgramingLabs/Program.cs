using System;
using System.Collections.Generic;
using System.Linq;

public class VendingMachine
{
    private static int nextSerialNumber = 1; // Static field to track the next serial number

    public int SerialNumber { get; private set; }
    public string Barcode { get; }

    private Dictionary<int, int> MoneyFloat { get; }
    public Dictionary<string, Product> Inventory { get; private set; }

    public VendingMachine(string barcode)
    {
        try
        {
            if (string.IsNullOrEmpty(barcode))
            {
                throw new ArgumentException("Barcode cannot be empty.");
            }

            Barcode = barcode;
            SerialNumber = nextSerialNumber++; // Assigning and incrementing the serial number
            MoneyFloat = new Dictionary<int, int>
            {
                { 1, 10 },
                { 2, 20 },
                { 5, 10 },
                { 10, 20 },
                { 20, 20 }
            };
            Inventory = new Dictionary<string, Product>();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error creating VendingMachine: {ex.Message}");
        }
    }

    public void StockItem(Product product, int quantity)
    {
        try
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }

            if (Inventory.ContainsKey(product.Code))
            {
                Inventory[product.Code].Quantity += quantity;
            }
            else
            {
                Inventory.Add(product.Code, product);
            }

            Console.WriteLine($"Stocked {product.Name} - Code: {product.Code}, Price: ${product.Price:F2}, Quantity: {quantity}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error stocking item: {ex.Message}");
        }
    }

    public void StockFloat(int moneyDenomination, int quantity)
    {
        try
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }

            if (MoneyFloat.ContainsKey(moneyDenomination))
            {
                MoneyFloat[moneyDenomination] += quantity;
            }
            else
            {
                MoneyFloat.Add(moneyDenomination, quantity);
            }

            Console.WriteLine($"Stocked ${moneyDenomination} coins - Quantity: {quantity}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error stocking float: {ex.Message}");
        }
    }

    public string VendItem(string code, List<int> money)
    {
        try
        {
            if (!Inventory.ContainsKey(code))
            {
                return $"Error: No item with code {code}";
            }

            Product selectedProduct = Inventory[code];

            if (selectedProduct.Quantity == 0)
            {
                return "Error: Item is out of stock.";
            }

            double totalPrice = selectedProduct.Price;
            int totalMoney = money.Sum();

            if (totalMoney < totalPrice)
            {
                return "Error: Insufficient money provided.";
            }

            double changeAmount = totalMoney - totalPrice;
            Dictionary<int, int> coinsToReturn = GetCoinsToReturn(changeAmount);

            if (coinsToReturn == null)
            {
                return "Error: The machine does not have enough change.";
            }

            selectedProduct.Quantity--;

            foreach (KeyValuePair<int, int> coin in coinsToReturn)
            {
                MoneyFloat[coin.Key] -= coin.Value;
            }

            string changeMessage = FormatChangeMessage(coinsToReturn);

            return $"Vended {selectedProduct.Name} - Code: {selectedProduct.Code}\nTotal Money: ${totalMoney:F2}\nChange: ${changeAmount:F2}\n{changeMessage}";
        }
        catch (Exception ex)
        {
            return $"Error vending item: {ex.Message}";
        }
    }

    private Dictionary<int, int> GetCoinsToReturn(double changeAmount)
    {
        Dictionary<int, int> coinsToReturn = new Dictionary<int, int>();

        foreach (KeyValuePair<int, int> coin in MoneyFloat.OrderByDescending(x => x.Key))
        {
            int denomination = coin.Key;
            int quantity = coin.Value;

            if (changeAmount >= denomination && quantity > 0)
            {
                int coinsNeeded = (int)(changeAmount / denomination);
                int coinsToTake = Math.Min(coinsNeeded, quantity);

                coinsToReturn.Add(denomination, coinsToTake);

                // Update change amount and deduct the coins from the float
                changeAmount -= coinsToTake * denomination;
                MoneyFloat[denomination] -= coinsToTake;
            }
        }

        return (changeAmount == 0) ? coinsToReturn : null;
    }


    private string FormatChangeMessage(Dictionary<int, int> coinsToReturn)
    {
        string changeMessage = "Change: ";

        foreach (KeyValuePair<int, int> coin in coinsToReturn.OrderByDescending(x => x.Key))
        {
            int denomination = coin.Key;
            int quantity = coin.Value;

            changeMessage += $"{quantity} x ${denomination}, ";
        }

        return changeMessage.TrimEnd(',', ' ');
    }
}

public class Product
{
    public string Name { get; private set; }
    public double Price { get; private set; }
    public string Code { get; private set; }
    public int Quantity { get; set; }

    public Product(string name, double price, string code, int quantity)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty.");
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Code cannot be empty.");
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be positive.");
            }

            Name = name;
            Price = price;
            Code = code;
            Quantity = quantity;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error creating Product: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        VendingMachine vendingMachine = new VendingMachine("123");

        Product product1 = new Product("Chocolate-covered Beans", 2.0, "A11", 3);
        Product product2 = new Product("Sour Gummies", 1.0, "A12", 5);
        Product product3 = new Product("Soda Drink", 5.0, "A13", 5);
        Product product4 = new Product("Doritos", 10.0, "A14", 5);
        Product product5 = new Product("Cheetos", 9.0, "A15", 5);
        Product product6 = new Product("Apple Juice", 4.0, "A16", 5);
        Product product7 = new Product("Chocolate-covered Beans", 2.0, "A11", 5);
        Product product8 = new Product("Chocolate-covered Beans", 2.0, "A11", 5);

        vendingMachine.StockItem(product1, 3);
        vendingMachine.StockItem(product2, 5);
        vendingMachine.StockItem(product3, 5);
        vendingMachine.StockItem(product4, 5);
        vendingMachine.StockItem(product5, 5);
        vendingMachine.StockItem(product6, 5);

        vendingMachine.StockFloat(1, 10);
        vendingMachine.StockFloat(2, 20);
        vendingMachine.StockFloat(5, 10);
        vendingMachine.StockFloat(10, 20);
        vendingMachine.StockFloat(20, 20);

        Console.WriteLine("Welcome to the Vending Machine!");
        Console.WriteLine("Available products:");
        foreach (Product product in vendingMachine.Inventory.Values)
        {
            Console.WriteLine($"Code: {product.Code}, Name: {product.Name}, Price: ${product.Price:F2}, Quantity: {product.Quantity}");
        }

        Console.WriteLine("Please enter the code of the product you want to buy:");
        string code = Console.ReadLine();

        Console.WriteLine("Please enter the money denomination one by one (enter 'done' to finish):");
        List<int> money = new List<int>();
        string input = Console.ReadLine();

        while (input != "done")
        {
            if (int.TryParse(input, out int denomination))
            {
                money.Add(denomination);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid denomination.");
            }

            input = Console.ReadLine();
        }

        string result = vendingMachine.VendItem(code, money);
        Console.WriteLine(result);
    }
}
