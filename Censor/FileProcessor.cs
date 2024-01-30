using System;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

namespace Censor
{
    internal class FileProcessor
    {
        private FoundFileList foundFileList;

        public FileProcessor(FoundFileList foundFileList)
        {
            this.foundFileList = foundFileList;
        }

        public async Task FileFinderAsync(string startingDirectory, string fileFormat = "*.txt")
        {
            try
            {
                await Task.Run(async () =>
                {
                    // Это для неправильно работающего ProgressBar, который ещё и замедляет поиск в два раза
                    string[] files = Directory.GetDirectories(startingDirectory);
                    int totalFiles = files.Length;
                    int processedFiles = 0;

                    // Поиск файлов в начальной директории
                    FileFinder(startingDirectory, fileFormat);

                    // Получение поддиректорий из начальной директории
                    foreach (string subdirectory in Directory.EnumerateDirectories(startingDirectory))
                    {
                        // Рекурсивно вызывается для поддикторий
                        await FileFinderAsync(subdirectory, fileFormat);

                        // Тоже для ProgressBar 
                        processedFiles++;
                        ProgressManager.ReportProgress((int)(((double)processedFiles / totalFiles) * 100));
                    }
                });
            }
            catch (UnauthorizedAccessException uae)
            {
                // Компилятор ругается и хочет, чтобы ErrorBox открывался в главном потоке
                //ErrorBox.Show($"Внимание", uae.Message);
            }
            catch (Exception ex)
            {
                InfoBox.Show(ex.Message, "Критическая ошибка");
            }
        }

        private void FileFinder(string subDirectory, string fileFormat)
        {
            Task.Run(() =>
            {
                foreach (string filePath in Directory.EnumerateFiles(subDirectory, fileFormat))
                {
                    // Обновление UI можно выполнять только в основном потоке
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.foundFileList.Add(new FoundFile(filePath));
                    });
                }
            });
        }
    }
}
