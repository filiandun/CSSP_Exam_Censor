using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Censor
{
    internal static class Report
    {
        public async static Task<string> GenerateReport(FoundFileList foundFileList, CensorWordList censorWordList)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("Отчёт по файлам: ");

            await Task.Run(() =>
            {
                foreach (FoundFile foundFile in foundFileList)
                {
                    report.AppendLine($"\nФайл: {foundFile.FileInfo.Name} ({foundFile.FileInfo.FullName})");
                    foreach (string censorWord in censorWordList)
                    {
                        report.AppendLine($"\tСлово \"{censorWord}\" повторяется {foundFile.CountCensorWord(censorWord)} раз(а).");
                    }
                }
            });

            return report.ToString();
        }

        public static void SaveReportFile(FlowDocument flowDocument)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Текстовый файл (*.txt)|*.txt",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                using (FileStream fileStream = File.Create(saveFileDialog.FileName))
                {
                    if (Path.GetExtension(saveFileDialog.FileName).ToLower() == ".txt")
                    {
                        textRange.Save(fileStream, DataFormats.Text);
                    }
                }
            }
        }
    }
}
