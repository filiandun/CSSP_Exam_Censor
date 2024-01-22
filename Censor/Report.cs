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
    internal class Report
    {
        private List<FoundFile> foundFiles;
        private List<string> censorWords;

        public Report(List<FoundFile> foundFiles, List<string> censorWords)
        {
            this.foundFiles = foundFiles;
            this.censorWords = censorWords;
        }

        // ТАК НЕ ДЕЛАЕТСЯ.
        // Под каждое действие, будь это добавление элементов или получение информации о файле,
        // должен быть свой метод, чтобы напрямую к массивам foundFiles и censorWord доступа не было.
        public List<FoundFile> FoundFiles 
        {
            get => this.foundFiles;
        }

        public List<string> CensorWords
        {
            get => this.censorWords;
        }

        public string GenerateReport()
        {
            StringBuilder report = new StringBuilder();

            report.AppendLine("Отчёт по файлам: ");
            foreach (FoundFile foundFile in this.foundFiles)
            {
                report.AppendLine($"\nФайл: {foundFile.FileInfo.Name} ({foundFile.FileInfo.FullName})");
                foreach (string censorWord in this.censorWords)
                {
                    report.AppendLine($"\tСлово \"{censorWord}\" повторяется {foundFile.CountCensorWord(censorWord)} раз(а).");
                }
            }

            return report.ToString();
        }

        public void SaveReportFile(FlowDocument flowDocument)
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
