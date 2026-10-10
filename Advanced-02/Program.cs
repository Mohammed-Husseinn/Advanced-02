#region Task 

using System;
using System.Collections.Generic;
using System.Linq;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; } // "Electronics", "Clothing", "Food", "Books"
    public double Price { get; set; }
    public int Stock { get; set; }
}

public interface IProductFilter
{
    bool Matches(Product product);
}

public class CategoryFilter : IProductFilter
{
    private readonly string _category;

    public CategoryFilter(string category)
    {
        _category = category;
    }

    public bool Matches(Product product)
    {
        return product.Category == _category;
    }
}

public class PriceBelowFilter : IProductFilter
{
    private readonly double _maxPrice;

    public PriceBelowFilter(double maxPrice)
    {
        _maxPrice = maxPrice;
    }

    public bool Matches(Product product)
    {
        return product.Price < _maxPrice;
    }
}

public class InStockFilter : IProductFilter
{
    public bool Matches(Product product)
    {
        return product.Stock > 0;
    }
}

public class AndFilter : IProductFilter
{
    private readonly List<IProductFilter> _filters;

    public AndFilter(params IProductFilter[] filters)
    {
        _filters = filters.ToList();
    }

    public bool Matches(Product product)
    {
        return _filters.All(filter => filter.Matches(product));
    }
}

public class Program
{
    public static void PrintProducts(string title, List<Product> products, IProductFilter filter)
    {
        Console.WriteLine($"--- {title} ---");

        foreach (Product product in products.Where(filter.Matches))
        {
            Console.WriteLine($"{product.Name} - ${product.Price} (Stock: {product.Stock})");
        }

        Console.WriteLine();
    }

    // Action<Product> is used because this method performs an action
    // on each product and does not return a value.
    public static void PrintReport(List<Product> products, Action<Product> action)
    {
        foreach (Product product in products)
        {
            action(product);
        }
    }

    // Func<Product, TResult> is used because this method transforms
    // each product into another value and returns the transformed list.
    public static List<TResult> TransformProducts<TResult>(
        List<Product> products,
        Func<Product, TResult> transform)
    {
        List<TResult> results = new();

        foreach (Product product in products)
        {
            results.Add(transform(product));
        }

        return results;
    }

    // Predicate<Product> is used because this method checks each product
    // against a true/false condition.
    public static List<Product> FilterProducts(
        List<Product> products,
        Predicate<Product> predicate)
    {
        List<Product> results = new();

        foreach (Product product in products)
        {
            if (predicate(product))
            {
                results.Add(product);
            }
        }

        return results;
    }

    public static void Main()
    {
        List<Product> catalog = new()
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200, Stock = 10 },
            new Product { Id = 2, Name = "Phone", Category = "Electronics", Price = 800, Stock = 25 },
            new Product { Id = 3, Name = "T-Shirt", Category = "Clothing", Price = 30, Stock = 100 },
            new Product { Id = 4, Name = "Jeans", Category = "Clothing", Price = 60, Stock = 50 },
            new Product { Id = 5, Name = "Chocolate", Category = "Food", Price = 5, Stock = 200 },
            new Product { Id = 6, Name = "Coffee Beans", Category = "Food", Price = 15, Stock = 80 },
            new Product { Id = 7, Name = "C# Book", Category = "Books", Price = 45, Stock = 30 },
            new Product { Id = 8, Name = "Novel", Category = "Books", Price = 20, Stock = 60 },
            new Product { Id = 9, Name = "Headphones", Category = "Electronics", Price = 150, Stock = 40 },
            new Product { Id = 10, Name = "Jacket", Category = "Clothing", Price = 120, Stock = 15 }
        };

        PrintProducts("Electronics", catalog, new CategoryFilter("Electronics"));
        PrintProducts("Under $50", catalog, new PriceBelowFilter(50));
        PrintProducts("In Stock", catalog.Take(4).ToList(), new InStockFilter());

        PrintProducts(
            "Clothing Under $100",
            catalog,
            new AndFilter(
                new CategoryFilter("Clothing"),
                new PriceBelowFilter(100)
            )
        );

        Console.WriteLine("--- Short Report ---");
        PrintReport(
            catalog.Take(6).ToList(),
            product => Console.WriteLine($"{product.Name} - ${product.Price}")
        );

        Console.WriteLine();

        Console.WriteLine("--- Detailed Report ---");
        PrintReport(
            catalog.Take(7).ToList(),
            product => Console.WriteLine(
                $"[{product.Category}] {product.Name} | Price: ${product.Price} | Stock: {product.Stock}"
            )
        );

        Console.WriteLine();

        Console.WriteLine("--- Summary List ---");
        List<string> summaries = TransformProducts(
            catalog.Take(7).ToList(),
            product => $"{product.Name} (${product.Price})"
        );

        foreach (string summary in summaries)
        {
            Console.WriteLine(summary);
        }

        Console.WriteLine();

        Console.WriteLine("--- Price Labels ---");

        List<Product> labelProducts = new()
        {
            catalog[0],
            catalog[1],
            catalog[2],
            catalog[7],
            catalog[8],
            catalog[9]
        };

        List<string> labels = TransformProducts(
            labelProducts,
            product => product.Price > 100 ? "Expensive!" : "Affordable"
        );

        for (int i = 0; i < labelProducts.Count; i++)
        {
            Console.WriteLine($"{labelProducts[i].Name}: {labels[i]}");
        }

        Console.WriteLine();

        Console.WriteLine("--- Low-Stock Alert ---");

        List<Product> lowStockProducts = FilterProducts(
            catalog,
            product => product.Stock < 20
        );

        foreach (Product product in lowStockProducts)
        {
            Console.WriteLine($"[LOW STOCK] {product.Name}: only {product.Stock} left!");
        }
    }
}












#endregion