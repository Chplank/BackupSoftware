
namespace BackupSoftware
{
    public class FileWatcher
    {
        /// <summary>
        /// Watches a directory for changes and maintains a backup for specified file types.
        /// </summary>
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        /// <summary>
        /// The path where backup files are stored.
        /// </summary>
        public string BackupPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class to monitor a specified directory.
        /// </summary>
        /// <param name="path">The path to monitor for file changes.</param>
        public FileWatcher(string path)
        {
            _watcher.Path = path;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;

            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess;

            _watcher.Changed += async (s, e) => await OnChanged(s, e);
            _watcher.Created += async (s, e) => await OnCreated(s, e);
            _watcher.Deleted += async (s, e) => await OnDeleted(s, e);
            _watcher.Renamed += async (s, e) => await OnRenamed(s, e);
        }

        /// <summary>
        /// Starts monitoring the specified directory on a separate thread.
        /// </summary>
        public void Start()
        {
            Task.Run(() =>
            {
                Console.WriteLine("FileWatcher started. Press Enter to stop.");
                Console.ReadLine();
            });
        }
        /// <summary>
        /// OnChanged Function to Handle the onChanged Files in the backupFolder!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;

            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(e.FullPath);
                string extension = Path.GetExtension(e.FullPath);

                string backupFilePath = Path.Combine(BackupPath, $"{fileNameWithoutExtension}{extension}");

                // Copy the modified file to the backup directory
                if (File.Exists(e.FullPath))
                {
                    await Task.Run(() => File.Copy(e.FullPath, backupFilePath, overwrite: true));
                    Console.WriteLine($"Changed: {e.FullPath}, backed up to {backupFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling changed event: {ex.Message}");
            }
        }
        /// <summary>
        /// OnDelete Function to Handle the onDeleted Files in The BackupFolder!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Deleted) return;

            var info = new FileInfo(e.FullPath);


            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(e.FullPath);

            string extension = Path.GetExtension(e.FullPath);
            string newFilePath = Path.Combine(BackupPath, $"{fileNameWithoutExtension}{extension}");

            if (!File.Exists(newFilePath)) return;
            await Task.Run(() => File.Delete(newFilePath));

            Console.WriteLine("Deleted File in Backup Folder");

        }
        /// <summary>
        /// OnRenamed Function to Handle the onRenamed Files in The BackupFolder!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnRenamed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Renamed) return;

            if (e is RenamedEventArgs renamedEvent)
            {
                string oldFilePath = renamedEvent.OldFullPath;
                string newFilePath = renamedEvent.FullPath;

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFilePath);
                string extension = Path.GetExtension(newFilePath);

                string backupFilePath = Path.Combine(BackupPath, $"{fileNameWithoutExtension}{extension}");

                try
                {
                    if (File.Exists(backupFilePath)) return;

                    if (File.Exists(backupFilePath))
                    {
                        File.Delete(backupFilePath);
                    }

                   await Task.Run(() => File.Move(newFilePath, backupFilePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during file rename operation: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Event args are not of type RenamedEventArgs.");
            }
        }
        /// <summary>
        /// OnCreated Function to Handle the onCreated Files in The BackupFolder!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created) return;

            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(e.FullPath);
                string extension = Path.GetExtension(e.FullPath);

                string backupFilePath = Path.Combine(BackupPath, $"{fileNameWithoutExtension}{extension}");

                if (File.Exists(e.FullPath))
                {
                    await Task.Run(() =>File.Copy(e.FullPath, backupFilePath));
                    Console.WriteLine($"Created: {e.FullPath}, backed up to {backupFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling created event: {ex.Message}");
            }
        }
    }
}
