using BackupSoftware;

Console.WriteLine("Welcome to the Backup Software!");

// Get the backup path input from the user
Console.Write("Enter the backup directory path: ");
string backupPath = Console.ReadLine();

// Validate the backup path
if (string.IsNullOrWhiteSpace(backupPath))
{
    Console.WriteLine("Invalid backup path. Application will exit.");
    return;
}

// Ensure the backup directory exists
if (!Directory.Exists(backupPath))
{
    Directory.CreateDirectory(backupPath);
    Console.WriteLine($"Backup directory created at: {backupPath}");
}

Console.Write("Enter the directory to watch: ");
string directoryToWatch = Console.ReadLine();

if (string.IsNullOrWhiteSpace(directoryToWatch) || !Directory.Exists(directoryToWatch))
{
    Console.WriteLine("Invalid directory to watch. Application will exit.");
    return;
}

// Initialize the FileWatcher
FileWatcher watcher = new FileWatcher(directoryToWatch)
{
    BackupPath = backupPath
};

// Run FileWatcher on a separate thread
Task watcherTask = Task.Run(() => watcher.Start());

Console.WriteLine("FileWatcher is running. Press Enter to stop.");
Console.ReadLine();
