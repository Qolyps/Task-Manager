using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ToDoList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool isOpen = true;
            ManagerTask manager = new ManagerTask();

            while (isOpen)
            {
                Console.WriteLine("Task Manager.\n");
                Console.WriteLine("1 - Create a task.");
                Console.WriteLine("2 - Delete a task.");
                Console.WriteLine("3 - Show all tasks.");
                Console.WriteLine("4 - Find a task.");
                Console.WriteLine("5 - Exit.");
                Console.Write("Select the command: ");
                string userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        manager.CreateTask();
                        break;
                    case "2":
                        manager.DeleteTask();
                        break;
                    case "3":
                        manager.ShowAllTasks();
                        break;
                    case "4":
                        manager.FindTask();
                        break;
                    case "5":
                        isOpen = false;
                        break;
                    default:
                        Console.WriteLine("Enter the correct command!");
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public TypeTask TaskType { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }

    public class ToDoContext : DbContext
    {
        public DbSet<TaskEntity> Tasks { get; set; }
        public ToDoContext() : base("name=ToDoDB") { }
    }

    public enum TypeTask
    {
        Personal,
        Team
    }

    public interface IActive
    {

    }

    public abstract class Task : IActive
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public TypeTask TypeTask { get; set; }
        public DateTime CreatedAt { get; set; }

        public Task(int id, string title, string description, bool isCompleted, DateTime createdAt, TypeTask typeTask)
        {
            Id = id;
            Title = title;
            Description = description;
            IsCompleted = isCompleted;
            TypeTask = typeTask;
            CreatedAt = createdAt;
        }
    }

    public class PersonalTask : Task
    {
        public PersonalTask(int id, string title, string description, bool isCompleted, DateTime createdAt, TypeTask typeTask)
            : base(id, title, description, isCompleted, createdAt, typeTask) { }
    }

    public class TeamTask : Task
    {
        public TeamTask(int id, string title, string description, bool isCompleted, DateTime createdAt, TypeTask typeTask)
            : base(id, title, description, isCompleted, createdAt, typeTask) { }
    }

    public class ManagerTask
    {
        private ToDoContext _context = new ToDoContext();

        public void CreateTask()
        {
            try
            {
                Console.Write("\nEnter a title: ");
                string inputTitle = Console.ReadLine();
                Console.Write("Enter a description: ");
                string inputDescription = Console.ReadLine();
                Console.Write("Select the task status (true/false): ");
                bool inputIsCompleted = Convert.ToBoolean(Console.ReadLine());
                Console.Write("Enter the creation date: ");
                DateTime inputDate = Convert.ToDateTime(Console.ReadLine());
                Console.Write("Select the type task (0 - Personal / 1 - Team): ");
                TypeTask inputTypeTask = (TypeTask)Enum.Parse(typeof(TypeTask), Console.ReadLine());

                TaskEntity newTask = new TaskEntity
                {
                    Title = inputTitle,
                    Description = inputDescription,
                    IsCompleted = inputIsCompleted,
                    CreatedDate = inputDate,
                    TaskType = inputTypeTask,
                };

                _context.Tasks.Add(newTask);
                _context.SaveChanges();
                Console.WriteLine("The task has been created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DeleteTask()
        {
            Console.Write("Enter the taks ID to delete: ");
            int inputDelete = Convert.ToInt32(Console.ReadLine());
            var deleteTask = _context.Tasks.Find(inputDelete);

            if (deleteTask != null)
            {
                _context.Tasks.Remove(deleteTask);
                _context.SaveChanges();
                Console.WriteLine("Task deleted successfully.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        public void ShowAllTasks()
        {
            var tasks = _context.Tasks.ToList();
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Console.WriteLine($"\nID: {task.Id}\nTitle: {task.Title}\nDescription: {task.Description}\nCompleted: {task.IsCompleted}\nType: {task.TaskType}\nCreated: {task.CreatedDate}");
                }
            }
            else
            {
                Console.WriteLine("No tasks available.");
            }
        }

        public void FindTask()
        {
            Console.Write("Enter the title of the task: ");
            string inputTask = Console.ReadLine();

            var resultTask = _context.Tasks.Where(t => t.Title.Contains(inputTask)).ToList();

            if (resultTask.Any())
            {
                foreach (var task in resultTask)
                {

                    Console.WriteLine($"\nID: {task.Id}\nTitle: {task.Title}\nDescription: {task.Description}\nCompleted: {task.IsCompleted}\nType: {task.TaskType}\nCreated: {task.CreatedDate}");
                }
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
    }
}





