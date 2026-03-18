using System;

/* Initial Code (Assisted using VS Code auto-completion):

public class ToDoList
{
    string[] tasks = new string[10];
    int taskCount = 0;

    public void AddTask(string task)
    {
        if (taskCount < tasks.Length)
        {
            tasks[taskCount] = task;
            taskCount++;
            Console.WriteLine("Task added: " + task);
        }
        else
        {
            Console.WriteLine("To-Do List is full. Cannot add more tasks.");
        }
    }

    public void ViewTasks()
    {
        for (int i = 0; i < taskCount; i++)
        {
            Console.WriteLine((i + 1) + ". " + tasks[i]);
        }
    }

        public static void CompleteTask() // Updated CompleteTask method
    {
        Console.WriteLine("Enter the number of the task to mark as complete:");
        int taskNumber = int.Parse(Console.ReadLine()) - 1;

        if (taskNumber >= 0 && taskNumber < taskCount)
        {
            tasks[taskNumber] = tasks[taskNumber] + " (Completed)";
            Console.WriteLine("Task marked as complete.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }

    public static void Main(string[] args) // Updated Main method
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Add a task");
            Console.WriteLine("2. View tasks");
            Console.WriteLine("3. Mark a task as complete");
            Console.WriteLine("4. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ViewTasks();
                    break;
                case "3":
                    CompleteTask();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}

*/

/* Answer:

public class ToDoList
{

    public static string[] tasks = new string[10];
    public static int taskCount = 0;

   public static void AddTask()
    {
        Console.WriteLine("Enter a new task:");
        tasks[taskCount] = Console.ReadLine();
        taskCount++;
    }

public static void ViewTasks()
    {
        for (int i = 0; i < taskCount; i++)
        {
            Console.WriteLine((i + 1) + ". " + tasks[i]);
        }
    }

public static void CompleteTask()
    {
        Console.WriteLine("Enter the number of the task to mark as complete:");
        int taskNumber = int.Parse(Console.ReadLine()) - 1;

        if (taskNumber >= 0 && taskNumber < taskCount)
        {
            tasks[taskNumber] = tasks[taskNumber] + " (Completed)";
            Console.WriteLine("Task marked as complete.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }

    public static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Add a task");
            Console.WriteLine("2. View tasks");
            Console.WriteLine("3. Mark a task as complete");
            Console.WriteLine("4. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ViewTasks();
                    break;
                case "3":
                    CompleteTask();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}

*/