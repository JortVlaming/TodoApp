using System.Text;
using System.Text.Json;

namespace TodoApp;

public class TodoApp
{
    private string _localTodoDirectory;
    private readonly string _homeTodoListDir;

    public TodoApp()
    {
        _localTodoDirectory = "";
        _homeTodoListDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.todo";
    }

    public void Run(string[] args)
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        if (args.Contains("-d"))
        {
            int i = Array.IndexOf(args, "-d")+1;
            if (i >= args.Length)
            {
                Console.WriteLine("Invalid directory argument");
                return;
            }
            string directory = args[i];
            Directory.SetCurrentDirectory(directory);
            currentDirectory = directory;
        }

        if (args.Contains("-debug"))
        {
            Thread.Sleep(5000);
        }

        if (!Directory.Exists(currentDirectory + "/.todo"))
        {
            Directory.CreateDirectory(currentDirectory + "/.todo");
        }
        _localTodoDirectory = currentDirectory + "/.todo";
        
        ChooseTodoMenu();
    }

    private void ChooseTodoMenu()
    {
        int optionIndex = 0;
        
        while (true)
        {
            Console.Clear();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Todo list app");

            int nextOptionIndex = 0;

            List<string> options = new List<string>();
            
            Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + "Create local todo list");
            options.Insert(nextOptionIndex, "create:local");
            nextOptionIndex++;
            
            Console.WriteLine("");
            Console.WriteLine("Local lists");
            string[] locals = Directory.GetDirectories(_localTodoDirectory);
            if (locals.Length == 0)
            {
                Console.WriteLine("No local lists!");
            }
            else
            {
                foreach (string list in locals)
                {
                    Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + list);
                    options.Insert(nextOptionIndex, "use:local:" + list);
                    nextOptionIndex++;
                }
            }
            
            Console.WriteLine("");
            Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + "Create global todo list");
            options.Insert(nextOptionIndex, "create:global");
    
            nextOptionIndex++;
            
            if (Directory.Exists(_homeTodoListDir))
            {
                Console.WriteLine("");
                Console.WriteLine("Global lists");
                string[] globals = Directory.GetDirectories(_homeTodoListDir);
                if (globals.Length == 0)
                {
                    Console.WriteLine("No global lists!");
                }
                else
                {
                    foreach (string path in globals)
                    {
                        Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + path.Replace("_", " "));
                        options.Insert(nextOptionIndex, "use:global:" + path);
                        nextOptionIndex++;
                    }
                }
            }
            
            // make choice stuff
            nextOptionIndex--;
            if (optionIndex > nextOptionIndex) optionIndex = nextOptionIndex-1;
            
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                if (optionIndex > 0) optionIndex--;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (optionIndex < nextOptionIndex) optionIndex++;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                string choice = options[optionIndex];
                string[] split = choice.Split(":");
                string operation = split[0];
                string arg = split[1];
                if (operation == "create")
                {
                    if (arg == "global")
                    {
                        CreateList(_homeTodoListDir);
                    }
                    else if (arg == "local")
                    {
                        CreateList(_localTodoDirectory);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid create operation {arg} : {choice} : {optionIndex}");
                    }
                }
                else if (operation == "user")
                {
                    
                }
                break;
            }
        }
    }
    
    private void CreateList(string basePath)
    { 
        Console.Clear();
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine("Creating global todo list");
        Console.WriteLine("");
        Console.Write("Todo list name: ");
        string name = "";

        while (name == "")
        {
            string? input = Console.ReadLine();
            if (input == null)
            {
                Console.WriteLine("Please enter a name");
                Console.Write("Todo list name: ");
                continue;
            }

            if (input.Trim() == "")
            {
                Console.WriteLine("Please enter a name");
                Console.Write("Todo list name: ");
                continue;
            }

            if (Directory.Exists(basePath + "/" + input))
            {
                Console.WriteLine($"Todo list '{input}' already exists");
                Console.Write("Todo list name: ");
                continue;
            }
            
            name = input;
        }

        string todoListDirectory = basePath + "/" + name.Replace(" ", "_");

        Directory.CreateDirectory(todoListDirectory);

        FileStream configStream = File.Create(todoListDirectory + "/CONFIG.json");

        TodoList list = new TodoList();
        list.Name = name;

        string json = JsonSerializer.Serialize(list);
        WriteStringToFileStream(configStream, json);
        
        configStream.Close();
        
        Console.WriteLine($"Created todo list: '{name}'");
        
        Thread.Sleep(1000);
        ChooseTodoMenu();
    }

    private void WriteStringToFileStream(FileStream fileStream, string content)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(content);
        fileStream.Write(info, 0, info.Length);
        fileStream.Flush();
    }
}