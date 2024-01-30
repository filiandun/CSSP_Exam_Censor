using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;

namespace Censor
{
    internal class CensorProcessor
    {
        private FoundFileList foundFileList;
        private CensorWordList censorWordList;

        public CensorProcessor(FoundFileList foundFileList, CensorWordList censorWordList)
        {
            this.foundFileList = foundFileList;
            this.censorWordList = censorWordList;
        }

        public async Task Censor()
        {
            try
            {
                int totalFiles = this.foundFileList.Count();
                int processedFile = 0;

                foreach (FoundFile foundFile in this.foundFileList)
                {
                    await Task.Run(() =>
                    {
                        string filePath = foundFile.FileInfo.FullName;

                        // TO DO
                        // Переделать, так как, если файл очень большой, то всё надолго виснет
                        List<string> oldLines = File.ReadAllLines(filePath).Where(l => !String.IsNullOrWhiteSpace(l)).ToList();
                        List<string> newLines = oldLines;

                        foreach (string censorWord in this.censorWordList)
                        {
                            newLines = FindingCensorWordsInFile(newLines, censorWord, foundFile);
                        }

                        //List<string> oldLines = File.ReadAllLines(filePath).Where(l => !String.IsNullOrWhiteSpace(l)).ToList();
                        //List<string> newLines = oldLines;

                        //foreach (string censorWord in this.censorWordList)
                        //{
                        //    newLines = FindingCensorWordsInFile(newLines, censorWord, foundFile);
                        //}

                        if (newLines.Count > 0)
                        {
                            EditFile(newLines, foundFile);
                        }

                        processedFile++;
                        ProgressManager.ReportProgress((int)(((double)processedFile / totalFiles) * 100));
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка");
            }
        }

        private List<string> FindingCensorWordsInFile(List<string> oldLines, string censorWord, FoundFile foundFile)
        {
            int count = 0;
            List<string> newLines = new List<string>();
            string newLine = "";

            foreach (string oldLine in oldLines)
            {
                foreach (string word in oldLine.Split(' '))
                {
                    if (word.IndexOf(censorWord, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        newLine += "******* ";
                        count++;
                    }
                    else
                    {
                        newLine += $"{word} ";
                    }
                }
                newLines.Add(newLine);
            }

            foundFile.AddCensorWord(censorWord, count);

            return newLines;
        }

        private void EditFile(List<string> newLines, FoundFile foundFile)
        {
            try
            {
                string filePath;

                // Параметры сохранения
                if (foundFile.FileSaveOption == FileSaveOption.SaveCopy)
                {
                    // Чтобы и копия не перезаписалась, если вдруг она существует
                    int i = 0;
                    do
                    {
                        string fileName = Path.GetFileNameWithoutExtension(foundFile.FileInfo.FullName) + "_copy" + (i == 0 ? "" : $"{i}");
                        filePath = Path.Combine(foundFile.FileInfo.DirectoryName, fileName + foundFile.FileInfo.Extension);
                        i++;
                    }
                    while (File.Exists(filePath));
                }
                else
                {
                    filePath = foundFile.FileInfo.FullName;
                }

                // Создание и запись в файл
                using (StreamWriter streamWriter = new StreamWriter(filePath))
                {
                    foreach (string line in newLines)
                    {
                        streamWriter.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Критическая ошибка");
            }
        }
    }
}
