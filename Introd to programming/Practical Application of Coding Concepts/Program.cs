using System;
using System.Runtime.CompilerServices;

public class Program
{
    public static void Main()
    {
        //Variable declaration
        string task1 = "";
        string task2 = "";
        string task3 = "";

        int taskCompleted;

        bool taskCompleted1 = false;
        bool taskCompleted2 = false;
        bool taskCompleted3 = false;

        do
        {


            //Ask for the task the user wants to add.
            Console.WriteLine("Enter the task you want to add: ");

            //Find if there is a task available and assign it.
            if (task1 == "")
            {
               task1 = Console.ReadLine();
            }
            else if (task2 == "")
            {
                task2 = Console.ReadLine();
            }
            else if (task3 == "")
            {
                task3 = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("There is no task field available");
                break;
            }
        } while (true);
        do
        {
            //Verify if there is a task to be completed.
            if (taskCompleted1 == true &&  taskCompleted2 == true && taskCompleted3 == true)
            {
                Console.WriteLine("All the tasks are completed. Congratyulations!");
                Console.WriteLine("1. " + task1 + "-- Completed --");
                Console.WriteLine("2. " + task2 + "-- Completed --");
                Console.WriteLine("3. " + task3 + "-- Completed --");
                break;
            }
            else
            {
                //Display the pending tasks
                Console.WriteLine("Please select the task to be completed 1, 2, or 3.");
                if (taskCompleted1 == true)
                {
                    Console.WriteLine("1. " + task1 + "-- Completed --");
                }
                else
                {
                    Console.WriteLine("1. " + task1 + "-- Pending --");
                }
                if (taskCompleted2 == true)
                {
                    Console.WriteLine("2. " + task2 + "-- Completed --");
                }
                else
                {
                    Console.WriteLine("2. " + task2 + "-- Pending --");
                }
                if (taskCompleted3 == true)
                {
                    Console.WriteLine("3. " + task3 + "-- Completed --");
                }
                else
                {
                    Console.WriteLine("3. " + task3 + "-- Pending --");
                }
                //Ask for the task that wants to be completed.
                taskCompleted = int.Parse(Console.ReadLine());
                if (taskCompleted == 1 && taskCompleted1 == false)
                {
                    taskCompleted1 = true;
                }
                else if (taskCompleted == 2 && taskCompleted2 == false)
                {
                    taskCompleted2 = true;
                }
                else if (taskCompleted == 3 && taskCompleted3 == false)
                {
                    taskCompleted3 = true;
                }
                else
                {
                    Console.WriteLine("Invalid task selected.");
                }
            }
        } while (true);
    }
}