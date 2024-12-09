using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

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
                        Console.WriteLine("Please enter a valid value.");
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
        private void GetInfoTask(TaskEntity task)
        {
            Console.WriteLine($"\nID: {task.Id}\nTitle: {task.Title}\nDescription: {task.Description}\nCompleted: {task.IsCompleted}\nType: {task.TaskType}\nCreated: {task.CreatedDate}");
        }

        public async Task CreateTaskAsync()
        {
            try
            {
                Console.Write("\nEnter a title: ");
                string inputTitle = Console.ReadLine();
                if (string.IsNullOrEmpty(inputTitle))
                {
                    Console.WriteLine("Title cannot be empty.");
                    return;
                }

                Console.Write("Enter a description: ");
                string inputDescription = Console.ReadLine();

                Console.Write("Select the task status (true/false): ");
                bool inputIsCompleted;

                while (!bool.TryParse(Console.ReadLine(), out inputIsCompleted))
                {
                    Console.WriteLine("Please enter a valid boolean value (true/false)");
                }

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
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");  
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
                    GetInfoTask(task);
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
                    GetInfoTask(task);
                }
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        public async Task SortTaskAsync()
        {
            Console.WriteLine("1 - Sort by completed.");
            Console.WriteLine("2 - Sort by type.");
            Console.WriteLine("3 - Sort by date.");
            Console.Write("Select the sort type: ");
            var selectSort = Console.ReadLine();
            switch (selectSort)
            {
                case "1":
                    var sortIsCompleted = await _context.Tasks.OrderBy(t => t.IsCompleted).ToListAsync();
                    foreach (var task in sortIsCompleted)
                    {
                        GetInfoTask(task);
                    }
                    break;
                case "2":
                    var sortType = await _context.Tasks.OrderBy(t => t.TaskType).ToListAsync();
                    foreach (var task in sortType)
                    {
                        GetInfoTask(task);
                    }
                    break;
                case "3":
                    var sortDate = await _context.Tasks.OrderBy(t => t.CreatedDate).ToListAsync();
                    foreach (var task in sortDate)
                    {
                        GetInfoTask(task);
                    }
                    break;
                default:
                    Console.WriteLine("Please enter a valid value.");
                    break;
            }
        }
    }
}
