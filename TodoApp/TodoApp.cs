namespace TodoApp;

public class TodoApp
{
    string todoDirectory = "";
    
    public void run(string[] args)
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

        if (Directory.Exists(currentDirectory + "/.todo"))
        {
            todoDirectory = currentDirectory + "/.todo";
        }
        else
        {
            todoDirectory = "";
        }
        
        ChooseTodoMenu();
    }

    private void ChooseTodoMenu()
    {
        int optionIndex = 0;
        string homeTodoListDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.todo";
        
        while (true)
        {
            bool todoExists = todoDirectory != "";

            Console.Clear();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Todo list app");

            int nextOptionIndex = 0;

            List<string> options = new List<string>();

            if (todoExists)
            {
                Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + "Pretend like this shows todo list information");
                options.Insert(nextOptionIndex, "use:" + todoDirectory);
            }
            else
            {
                Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + "Create todo list in current directory (" + Directory.GetCurrentDirectory() + ")");
                options.Insert(nextOptionIndex, "create:" + Directory.GetCurrentDirectory());
            }
            
            nextOptionIndex++;
            
            Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + "Create global todo list");
            options.Insert(nextOptionIndex, "create:global");
    
            nextOptionIndex++;
            
            if (Directory.Exists(homeTodoListDir))
            {
                Console.WriteLine("");
                Console.WriteLine("Global lists");
                string[] globals = Directory.GetDirectories(homeTodoListDir);
                if (globals.Length == 0)
                {
                    Console.WriteLine("No global lists!");
                }
                else
                {
                    foreach (string path in globals)
                    {
                        Console.WriteLine((optionIndex == nextOptionIndex ? "> " : "") + path);
                        options.Insert(nextOptionIndex, "use:" + path);
                        nextOptionIndex++;
                    }
                }
            }
            
            // make choice stuff
            nextOptionIndex--;
            if (optionIndex > nextOptionIndex) optionIndex = nextOptionIndex-1;
            
            Console.WriteLine($"{optionIndex} : {nextOptionIndex}");
            
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                if (optionIndex > 1) optionIndex--;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (optionIndex < nextOptionIndex) optionIndex++;
            }
        }
    }
}