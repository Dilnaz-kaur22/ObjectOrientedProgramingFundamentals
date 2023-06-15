using System;
using System.Collections.Generic;
using System.Text;

class VendingMachine
{
    private int serialNumber;
    private Dictionary<string, int> moneyFloat;
    private Dictionary<string, int> inventory;

    public VendingMachine(int serialNumber)
    {
        this.serialNumber = serialNumber;
        moneyFloat = new Dictionary<string, int>();
        inventory = new Dictionary<string, int>();
    }

    // Method to add a product to the inventory
    public string AddProduct(string productName, int quantity, string code)
    {
        if (inventory.ContainsKey(code))
        {
            // If the product already exists in the inventory, increase the quantity
            inventory[code] += quantity;
        }
        else
        {
            // If the product is not present in the inventory, add it with the given quantity
            inventory.Add(code, quantity);
        }
        return $"Added {quantity} {productName}(s) with code {code} to the inventory.";
    }

    // Method to add coins to the money float
    public string AddCoins(string coinName, int quantity)
    {
        if (moneyFloat.ContainsKey(coinName))
        {
            // If the coin already exists in the money float, increase the quantity
            moneyFloat[coinName] += quantity;
        }
        else
        {
            // If the coin is not present in the money float, add it with the given quantity
            moneyFloat.Add(coinName, quantity);
        }
        return $"Added {quantity} {coinName}(s) to the money float.";
    }

    // Method to vend an item based on the given code and inserted coins
    public string VendItem(string code, List<string> coins)
    {
        int totalPrice = 0;
        int totalChange = 0;
        StringBuilder changeCoins = new StringBuilder();

        // Check if the requested item is in the inventory
        if (!inventory.ContainsKey(code))
        {
            return $"Error: No item with code '{code}' in the inventory.";
        }

        // Calculate the total price based on the inserted coins and update the money float
        foreach (string coin in coins)
        {
            if (moneyFloat.ContainsKey(coin))
            {
                // Increase the total price by the value of the inserted coin
                totalPrice += GetCoinValue(coin);
                moneyFloat[coin]++;
            }
        }

        // Check if the item is out of stock
        if (inventory[code] <= 0)
        {
            return $"Error: Item with code '{code}' is out of stock.";
        }

        // Check if the inserted money is sufficient to purchase the item
        if (totalPrice < GetItemPrice(code))
        {
            return $"Error: Insufficient money provided to purchase item with code '{code}'.";
        }

        // Calculate the total change required
        totalChange = totalPrice - GetItemPrice(code);

        // Generate the change string
        if (totalChange > 0)
        {
            changeCoins.Append("Your change: ");
            foreach (KeyValuePair<string, int> coin in moneyFloat)
            {
                int coinValue = GetCoinValue(coin.Key);
                int numCoins = totalChange / coinValue;
                if (numCoins > 0 && coin.Value >= numCoins)
                {
                    // Deduct the change coins from the money float
                    totalChange -= numCoins * coinValue;
                    changeCoins.Append($"{numCoins} {coin.Key}(s), ");
                    moneyFloat[coin.Key] -= numCoins;
                }
            }
        }

        // Reduce the quantity of the purchased item from the inventory
        inventory[code]--;

        // Build the vending result string
        string vendingResult = $"Enjoy your item with code '{code}'. {changeCoins.ToString()}";

        // Check if there is remaining change and add it to the vending result
        if (totalChange > 0)
        {
            vendingResult += $"{totalChange} cents.";
        }

        return vendingResult;
    }

    // Method to retrieve the price of an item based on the given code
    private int GetItemPrice(string code)
    {
        // In a real implementation, this method would retrieve the price of the item from a database or other data source.
        // For the sake of simplicity, we'll use a switch statement here.
        switch (code)
        {
            case "A12":
                return 2;
            case "B34":
                return 3;
            case "C56":
                return 1;
            default:
                return 0;
        }
    }

    // Method to retrieve the value of a coin based on the given coin name
    private int GetCoinValue(string coinName)
    {
        // In a real implementation, this method would retrieve the value of the coin from a database or other data source.
        // For the sake of simplicity, we'll use a switch statement here.
        switch (coinName)
        {
            case "Quarter":
                return 25;
            case "Dime":
                return 10;
            case "Nickel":
                return 5;
            default:
                return 0;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Create a new vending machine instance with serial number 1234
        VendingMachine vendingMachine = new VendingMachine(1234);

        // Add a product to the inventory
        Console.WriteLine(vendingMachine.AddProduct("Chocolate-covered Beans", 3, "A12"));

        // Add coins to the money float
        Console.WriteLine(vendingMachine.AddCoins("Quarter", 4));

        // Specify the inserted coins
        List<string> insertedCoins = new List<string> { "Quarter", "Dime" };

        // Vend an item with the given code and inserted coins
        Console.WriteLine(vendingMachine.VendItem("A12", insertedCoins));
    }
}
