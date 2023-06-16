using System;
using System.Collections.Generic;
using System.Linq;

public class VendingMachine
{
    // Unique barcode for the machine
    public string Barcode { get; private set; }

    // Serial number for the machine
    public int SerialNumber { get; private set; }

    // Static variable to track the next serial number
    private static int nextSerialNumber = 1;

    // Private fields to store money float and inventory
    private Dictionary<int, int> MoneyFloat;
    private Dictionary<string, Product> Inventory;

    // Constructor to initialize the vending machine with a barcode
    public VendingMachine(string barcode)
    {
        Barcode = barcode;
        SerialNumber = nextSerialNumber++;
        MoneyFloat = new Dictionary<int, int>
        {
            { 1, 10 },
            { 2, 20 },
            { 5, 10 },
            { 10, 10 },
            { 20, 20 }
        };
        Inventory = new Dictionary<string, Product>();
    }

    // Method to add coins to the money float
    public void AddCoins(int moneyDenomination, int quantity)
    {
        if (MoneyFloat.ContainsKey(moneyDenomination))
        {
            // If denomination is already in the float, increase quantity
            MoneyFloat[moneyDenomination] += quantity;
        }
        else
        {
            // If denomination is not in the float, add it with quantity
            MoneyFloat.Add(moneyDenomination, quantity);
        }

        // Print the information about the stocked coins
        Console.WriteLine($"Stocked ${moneyDenomination} coins - Quantity: {quantity}");
    }

    // Method to stock a product in the vending machine's inventory
    public void StockItem(Product product, int quantity)
    {
        if (Inventory.ContainsKey(product.Code))
        {
            // If product is already in inventory, increase quantity
            Inventory[product.Code].Quantity += quantity;
        }
        else
        {
            // If product is not in inventory, add
            Inventory.Add(product.Code, product);
        }

        // Print the information about the stocked product
        Console.WriteLine($"Stocked {product.Name} - Code: {product.Code}, Price: ${product.Price:F2}, Quantity: {quantity}");
    }

    // Method to perform the vending operation
    public string PurchaseProduct(string code, List<int> money)
    {
        if (!Inventory.ContainsKey(code))
        {
            // If product code is not found in inventory, return an error message
            return ($"Error: No item with code {code}");
        }

        Product selectedProduct = Inventory[code];

        if (selectedProduct.Quantity == 0)
        {
            // If product is out of stock, return error message
            return "Error: Item is out of stock.";
        }

        double totalPrice = selectedProduct.Price;
        int totalMoney = money.Sum();

        if (totalMoney < totalPrice)
        {
            // If provided money is not enough to buy product, return error message
            return "Error: Insufficient money provided.";
        }

        double changeAmount = totalMoney - totalPrice;

        // Get the coins to return as change
        Dictionary<int, int> coinsToReturn = GetCoinsToReturn(changeAmount);

        if (coinsToReturn == null)
        {
            // If the machine does not have enough change, return an error message
            return "Error: The machine does not have enough change.";
        }

        // Reduce quantity of selected product in the inventory
        selectedProduct.Quantity--;

        // Update money float by deducting the coins returned as change
        foreach (KeyValuePair<int, int> coin in coinsToReturn)
        {
            MoneyFloat[coin.Key] -= coin.Value;
        }

        // Format change message to be displayed
        string changeMessage = FormatChangeMessage(coinsToReturn);

        // Return vending operation result with the change message
        return $"Please enjoy your '{selectedProduct.Name}' and take your change of ${changeAmount:F2}. {changeMessage}";
    }

    // Calculate coins to return as change for the given change amount
    private Dictionary<int, int> GetCoinsToReturn(double changeAmount)
    {
        Dictionary<int, int> coinsToReturn = new Dictionary<int, int>();

        // Iterate over each coin in the MoneyFloat dictionary, starting from the highest denomination
        foreach (KeyValuePair<int, int> coin in MoneyFloat.OrderByDescending(c => c.Key))
        {
            int denomination = coin.Key;
            int availableQuantity = coin.Value;

            // Calculate number of coins needed to cover remaining change amount
            int coinsNeeded = (int)(changeAmount / denomination);

            // Determine the actual number of coins to return (limited by available quantity and coins needed)
            int coinsToReturnQuantity = Math.Min(coinsNeeded, availableQuantity);

            // If there are coins to return, add them to coinsToReturn dictionary and update the change amount
            if (coinsToReturnQuantity > 0)
            {
                coinsToReturn.Add(denomination, coinsToReturnQuantity);
                changeAmount -= coinsToReturnQuantity * denomination;
            }

            // If change amount has been fully covered, return the coinsToReturn dictionary
            if (changeAmount <= 0)
            {
                // Found enough coins to cover the change amount
                return coinsToReturn;
            }
        }

        // The machine does not have sufficient change
        return null;
    }

    // Format the change message to be displayed
    private string FormatChangeMessage(Dictionary<int, int> coinsToReturn)
    {
        List<string> changeParts = new List<string>();

        foreach (KeyValuePair<int, int> coin in coinsToReturn)
        {
            string coinDescription = $"{coin.Key} piece{(coin.Value > 1 ? "s" : "")}";
            changeParts.Add($"{coinDescription}: {coin.Value} {(coin.Key > 1 ? "coins" : "coin")}");
        }

        return string.Join(", ", changeParts);
    }
}

// Representing a product in the vending machine
public class Product
{
    public string Name { get; private set; }
    public double Price { get; private set; }
    public string Code { get; private set; }
    public int Quantity { get; set; }

    // Constructor to initialize product properties
    public Product(string name, double price, string code, int quantity)
    {
        Name = name;
        Price = price;
        Code = code;
        Quantity = quantity;
    }
}

public class Program
{
    public static void Main()
    {
        // Creating a new vending machine with a barcode
        VendingMachine vendingMachine = new VendingMachine("VM123");

        // Stocking the vending machine with items and money float
        vendingMachine.AddCoins(1, 4);
        vendingMachine.StockItem(new Product("Chocolate-covered Beans", 2.0, "A11", 3), 4);
        vendingMachine.StockItem(new Product("Sour Candies", 5.0, "A12", 3), 5);
        vendingMachine.StockItem(new Product("Soda Drink", 3.0, "A13", 3), 4);
        vendingMachine.StockItem(new Product("Doritos", 10.0, "A14", 3), 5);
        vendingMachine.StockItem(new Product("Cheetos", 9.0, "A15", 3), 4);
        vendingMachine.StockItem(new Product("Apple Juice", 4.0, "A16", 3), 5);

        // Prompting the user for the inputs
        Console.WriteLine("Enter the amount of money:");
        double moneyAmount = double.Parse(Console.ReadLine());

        Console.WriteLine("Enter the item code:");
        string itemCode = Console.ReadLine();

        List<int> insertedMoney = new List<int>();
        if (moneyAmount > 0)
        {
            Console.WriteLine("Enter the money denominations one by one (enter 'done' to finish):");
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "done")
                {
                    break;
                }
                else
                {
                    int denomination = int.Parse(input);
                    insertedMoney.Add(denomination);
                }
            }
        }

        // Performing the vending operation
        string result = vendingMachine.PurchaseProduct(itemCode, insertedMoney);

        // Displaying the result
        Console.WriteLine(result);
    }
}
