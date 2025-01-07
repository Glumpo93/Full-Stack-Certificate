using System;

public class Program
{
    public static void Main()
    {
        double radius;
        double circleArea;
        double trapezoidheight;
        double trapezoidwidth;
        double trapezoidArea;
        Console.WriteLine("Enter the radius of the circle: ");
        radius = double.Parse(Console.ReadLine());
        circleArea = CalculateCircleArea(radius);
        Console.WriteLine("The result of the area is: " + circleArea);
        Console.WriteLine("Enter the hight of the trapezoid: ");
        trapezoidheight = double.Parse(Console.ReadLine());
        Console.WriteLine("Enter the width of the trapezoid: ");
        trapezoidwidth = double.Parse(Console.ReadLine());
        trapezoidArea = CalculateTrapezoidArea(trapezoidheight, trapezoidwidth);
        Console.WriteLine("The area of the trapezoid is: " + trapezoidArea);
    }
    static double CalculateCircleArea(double radius)
    {
        double result = 0;
        result = Math.PI * radius * radius;
        return result;
    }
    static double CalculateTrapezoidArea(double height, double width)
    {
        double result = 0;
        result += height * width;
        result /= 2;
        result *= height;
        return result;
    }
}