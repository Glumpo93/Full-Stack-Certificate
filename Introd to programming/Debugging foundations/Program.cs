using System;
using System.Collections.Generic;

class TaskManager
{
    static List<string> tasks = new List<string>();
    static List<bool> taskStatus = new List<bool>();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Task Manager");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Completed");
            Console.WriteLine("3. View Tasks");
            Console.WriteLine("4. Exit");
            Console.WriteLine("What would you like to do? (choose 1-4)");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    CompleteTask();
                    break;
                case "3":
                    ViewTasks();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }

    static void AddTask()
    {
        Console.WriteLine("Enter task description:");
        string task = Console.ReadLine();
        tasks.Add(task);
        taskStatus.Add(false);  // Marking as not completed by default
        Console.WriteLine("Task added successfully.");
    }

    static void CompleteTask()
    {
        Console.WriteLine("Enter task number to mark as completed:");
        int taskNumber;
        int.TryParse(Console.ReadLine(), out taskNumber);
        taskNumber -= 1;

        if (taskNumber < 0 || taskNumber >= tasks.Count)
        {
            Console.WriteLine("Invalid task number.");
            return;
        }

        taskStatus[taskNumber] = true;
        Console.WriteLine($"Task '{tasks[taskNumber]}' marked as completed.");
    }

    static void ViewTasks()
    {
        Console.WriteLine("Tasks:");
        for (int i = 0; i < tasks.Count; i++)
        {
            string status = taskStatus[i] ? "Completed" : "Pending";
            Console.WriteLine($"{i + 1}. {tasks[i]} - {status}");
        }
    }
}