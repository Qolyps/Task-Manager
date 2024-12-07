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
        static async Task Main(string[] args)
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
                Console.WriteLine("5 - Sort tasks.");
                Console.WriteLine("6 - Exit.");
                Console.Write("\nSelect the command: ");
                string userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        await manager.CreateTaskAsync();
                        break;
                    case "2":
                        await manager.DeleteTaskAsync();
                        break;
                    case "3":
                        await manager.ShowAllTasksAsync();
                        break;
                    case "4":
                        await manager.FindTaskAsync();
                        break;
                    case "5":
                        await manager.SortTaskAsync();
                        break;
                    case "6":
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

    public abstract class BaseTask : IActive
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public TypeTask TypeTask { get; set; }
        public DateTime CreatedAt { get; set; }

        public BaseTask(int id, string title, string description, bool isCompleted, DateTime createdAt, TypeTask typeTask)
        {
            Id = id;
            Title = title;
            Description = description;
            IsCompleted = isCompleted;
            TypeTask = typeTask;
            CreatedAt = createdAt;
        }
    }

    public class PersonalTask : BaseTask
    {
        public PersonalTask(int id, string title, string description, bool isCompleted, DateTime createdAt, TypeTask typeTask)
            : base(id, title, description, isCompleted, createdAt, typeTask) { }
    }

    public class TeamTask : BaseTask
    {
        public TeamTask(int id, string title, string description, bool isCompleted, DateTime createdAt, TypeTask typeTask)
            : base(id, title, description, isCompleted, createdAt, typeTask) { }
    }

    public class ManagerTask
    {
        private ToDoContext _context = new ToDoContext();

        public async Task CreateTaskAsync()
        {
            try
            {
                Console.Write("\nEnter a title: ");
                string inputTitle = Console.ReadLine();
                Console.Write("Enter a description: ");
                string inputDescription = Console.ReadLine();
                Console.Write("Select the task status (true/false): ");
                bool inputIsCompleted = Convert.ToBoolean(Console.ReadLine());
                DateTime dateCreate = DateTime.Now;
                Console.Write("Select the type task (0 - Personal / 1 - Team): ");
                TypeTask inputTypeTask = (TypeTask)Enum.Parse(typeof(TypeTask), Console.ReadLine());

                TaskEntity newTask = new TaskEntity
                {
                    Title = inputTitle,
                    Description = inputDescription,
                    IsCompleted = inputIsCompleted,
                    CreatedDate = dateCreate,    
                    TaskType = inputTypeTask,
                };

                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();
                Console.WriteLine("The task has been created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteTaskAsync()
        {
            Console.Write("Enter the taks ID to delete: ");
            int inputDelete = Convert.ToInt32(Console.ReadLine());
            var deleteTask = await _context.Tasks.FindAsync(inputDelete);

            if (deleteTask != null)
            {
                _context.Tasks.Remove(deleteTask);
                await _context.SaveChangesAsync();
                Console.WriteLine("Task deleted successfully.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        public async Task ShowAllTasksAsync()
        {
            var tasks = await _context.Tasks.ToListAsync();
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

        public async Task FindTaskAsync()
        {
            Console.Write("Enter the title of the task: ");
            string inputTask = Console.ReadLine();

            var resultTask = await _context.Tasks.Where(t => t.Title.Contains(inputTask)).ToListAsync();

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

        public async Task SortTaskAsync()
        {
            Console.Write("1 - Sort by completed (true/false): ");
            var sortCompleted = Convert.ToBoolean(Console.ReadLine());
            var resultCompleted =  await _context.Tasks.Where(t => t.IsCompleted == sortCompleted).ToListAsync();

            if (resultCompleted.Any())
            {
                foreach(var task in resultCompleted)
                {
                    Console.WriteLine($"\nID: {task.Id}\nTitle: {task.Title}\nDescription: {task.Description}\nCompleted: {task.IsCompleted}\nType: {task.TaskType}\nCreated: {task.CreatedDate}");
                }
            }
            else
            {
                Console.WriteLine("No completed tasks");
            }
        }
    }
}





