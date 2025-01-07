class InventoryManagement
{
    static List<string> itemNames = new List<string>();
    static List<double> itemPrices = new List<double>();
    static List<int> itemQuantities = new List<int>();

    static void Main(string[] args)
    {
        while (true)
        {
            DisplayMenu();
            string option = Console.ReadLine();
            HandleMenuOption(option);
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("***************************************");
        Console.WriteLine("**       Inventory Management        **");
        Console.WriteLine("***************************************");
        Console.WriteLine("Welcome to the Inventory Management System!");
        Console.WriteLine("Please select an option:");
        Console.WriteLine("1. Add an item");
        Console.WriteLine("2. Update an item");
        Console.WriteLine("3. Display all items");
        Console.WriteLine("4. Remove an item");
        Console.WriteLine("5. Exit");
    }

    static void HandleMenuOption(string option)
    {
        switch (option)
        {
            case "1":
                AddItem();
                break;
            case "2":
                if (itemNames.Count > 0)
                {
                    UpdateItem();
                }
                else
                {
                    Console.WriteLine("No items available to update.");
                }
                break;
            case "3":
                if (itemNames.Count > 0)
                {
                    DisplayItems();
                }
                else
                {
                    Console.WriteLine("No items available to display.");
                }
                break;
            case "4":
                if (itemNames.Count > 0)
                {
                    RemoveItem();
                }
                else
                {
                    Console.WriteLine("No items available to remove.");
                }
                break;
            case "5":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }

    static void AddItem()
    {
        string itemName = GetInput("Enter the item name: ");
        double itemPrice = GetDoubleInput("Enter the item price: ");
        int itemQuantity = GetIntInput("Enter the item quantity: ");

        if (itemPrice < 0 || itemQuantity < 0)
        {
            Console.WriteLine("Price and quantity must be non-negative.");
            return;
        }

        itemNames.Add(itemName);
        itemPrices.Add(itemPrice);
        itemQuantities.Add(itemQuantity);

        Console.WriteLine("Item added successfully!");
    }

    static void UpdateItem()
    {
        DisplayItemNames();
        int itemNumber = GetIntInput("Enter the number of the item to update: ") - 1;

        if (IsValidItemNumber(itemNumber))
        {
            Console.WriteLine("Select the process type:");
            Console.WriteLine("1. Restock");
            Console.WriteLine("2. Sell");
            string processType = Console.ReadLine();

            switch (processType)
            {
                case "1":
                    RestockItem(itemNumber);
                    break;
                case "2":
                    SellItem(itemNumber);
                    break;
                default:
                    Console.WriteLine("Invalid process type.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid item number.");
        }
    }

    static void RestockItem(int itemNumber)
    {
        int quantity = GetIntInput("Enter the quantity to restock: ");

        if (quantity < 0)
        {
            Console.WriteLine("Quantity must be non-negative.");
            return;
        }

        itemQuantities[itemNumber] += quantity;
        Console.WriteLine("Item restocked successfully!");
    }

    static void SellItem(int itemNumber)
    {
        int quantity = GetIntInput("Enter the quantity to sell: ");

        if (quantity < 0)
        {
            Console.WriteLine("Quantity must be non-negative.");
            return;
        }

        if (quantity > itemQuantities[itemNumber])
        {
            Console.WriteLine("Not enough stock to sell.");
        }
        else
        {
            itemQuantities[itemNumber] -= quantity;
            Console.WriteLine("Item sold successfully!");
        }
    }

    static void DisplayItems()
    {
        for (int i = 0; i < itemNames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {itemNames[i]} - Price: {itemPrices[i]}, Quantity: {itemQuantities[i]}");
        }
    }

    static void RemoveItem()
    {
        DisplayItemNames();
        int itemNumber = GetIntInput("Enter the number of the item to remove: ") - 1;

        if (IsValidItemNumber(itemNumber))
        {
            itemNames.RemoveAt(itemNumber);
            itemPrices.RemoveAt(itemNumber);
            itemQuantities.RemoveAt(itemNumber);

            Console.WriteLine("Item removed successfully!");
        }
        else
        {
            Console.WriteLine("Invalid item number.");
        }
    }

    static void DisplayItemNames()
    {
        for (int i = 0; i < itemNames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {itemNames[i]}");
        }
    }

    static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    static double GetDoubleInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out double result))
            {
                if (result < double.MaxValue)
                {
                    return result;
                }
                Console.WriteLine("Input is too large. Please enter a smaller number.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }

    static int GetIntInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                if (result < int.MaxValue)
                {
                    return result;
                }
                Console.WriteLine("Input is too large. Please enter a smaller number.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }

    static bool IsValidItemNumber(int itemNumber)
    {
        return itemNumber >= 0 && itemNumber < itemNames.Count;
    }
}