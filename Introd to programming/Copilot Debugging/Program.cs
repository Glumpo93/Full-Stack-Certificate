using System;
using System.Collections.Generic;  // Needed for List<T>

class Program
{
    static void Main(string[] args)
    {
        List<string> tasks = new List<string>();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("1. View Tasks");
            Console.WriteLine("2. Add Task");
            Console.WriteLine("3. Mark Task Complete");
            Console.WriteLine("4. Remove Task");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");
            
            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        ViewTasks(tasks);
                        break;

                    case 2:
                        AddTask(tasks);
                        break;

                    case 3:
                        MarkTaskComplete(tasks);
                        break;

                    case 4:
                        RemoveTask(tasks);
                        break;

                    case 5:
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option, please enter a number between 1 and 5.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input, please enter a valid number.");
            }
        }
    }

    static void ViewTasks(List<string> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
        }
        else
        {
            int index = 1;
            foreach (var task in tasks)
            {
                Console.WriteLine($"{index}. {task}");
                index++;
            }
        }
    }

    static void AddTask(List<string> tasks)
    {
        Console.Write("Enter the task: ");
        string task = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(task))
        {
            Console.WriteLine("Task cannot be empty.");
        }
        else
        {
            tasks.Add(task);
            Console.WriteLine("Task added.");
        }
    }

    static void MarkTaskComplete(List<string> tasks)
    {
        Console.Write("Enter the task number to mark complete: ");
        int taskNumber;
        if (int.TryParse(Console.ReadLine(), out taskNumber))
        {
            if (taskNumber > 0 && taskNumber <= tasks.Count)
            {
                tasks[taskNumber - 1] += " [Complete]";
                Console.WriteLine("Task marked as complete.");
            }
            else
            {
                Console.WriteLine("Invalid task number, please enter a number between 1 and " + tasks.Count);
            }
        }
        else
        {
            Console.WriteLine("Invalid input, please enter a valid number.");
        }
    }

    static void RemoveTask(List<string> tasks)
    {
        Console.Write("Enter the task number to remove: ");
        int removeTaskNumber;
        if (int.TryParse(Console.ReadLine(), out removeTaskNumber))
        {
            if (removeTaskNumber > 0 && removeTaskNumber <= tasks.Count)
            {
                tasks.RemoveAt(removeTaskNumber - 1);
                Console.WriteLine("Task removed.");
            }
            else
            {
                Console.WriteLine("Invalid task number, please enter a number between 1 and " + tasks.Count);
            }
        }
        else
        {
            Console.WriteLine("Invalid input, please enter a valid number.");
        }
    }
}