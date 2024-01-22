using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Documents;
using Microsoft.Win32;

using Winforms = System.Windows.Forms;
using System.Diagnostics;
using System.Threading;



namespace Censor
{
    public partial class MainWindow : Window
    {
        private Report report;

        public MainWindow()
        {
            InitializeComponent();

            Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark, Wpf.Ui.Appearance.BackgroundType.Tabbed, true);

            this.saveCopyFileOptionRadioButton.Visibility = Visibility.Hidden;
            this.replaceOriginalFileOptionRadioButton.Visibility = Visibility.Hidden;

            this.report = new Report(new List<FoundFile>(), new List<string>());

            this.GetAllDrive();
            this.GetFileFormats();
        }

        private void GetAllDrive()
        {
            this.directoryPathComboBox.Items.Add("Поиск на всех накопителях");
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                this.directoryPathComboBox.Items.Add(drive.Name);
            }
            this.directoryPathComboBox.Items.Add(@"D:\Downloads\Блокноты");
        }

        private void GetFileFormats()
        {
            this.fileFormatComboBox.Items.Add("Все перечисленные");
            this.fileFormatComboBox.Items.Add(".txt");
            this.fileFormatComboBox.Items.Add(".doc");
            this.fileFormatComboBox.Items.Add(".dot");
            this.fileFormatComboBox.Items.Add(".docx");
            this.fileFormatComboBox.Items.Add(".pdf");
            this.fileFormatComboBox.Items.Add(".html");
        }


        private async void startFindButton_Click(object sender, RoutedEventArgs e)
        {
            string path = this.directoryPathComboBox.Text;

            if (String.IsNullOrEmpty(path))
            {
                // TO DO
                Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Внимание",
                    Content = "Вы оставили поле с путём пустым!",
                    ButtonLeftName = "Сейчас исправлю",
                    ButtonRightName = "Начать поиск по всем накопителям"
                };
                messageBox.Show();

                return;
            }

            if (!Directory.Exists(path))
            {
                // TO DO
                Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Внимание",
                    Content = $"Указанный путь ({path}) не найден!",
                    ButtonLeftName = "Сейчас исправлю",
                    ButtonRightName = "Начать поиск по всем накопителям"
                };
                messageBox.Show();

                return;
            }

            try
            {
                // TO DO Тут нужно сделать умную многопоточность

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //List<Task> tasks = new List<Task>();
                //foreach (string pathNewFile in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories))
                //{
                //    tasks.Add(Task.Run(() => this.CreateListBoxItemFile(pathNewFile)));
                //}
                //await Task.WhenAll(tasks);

                foreach (string pathNewFile in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories))
                {
                    this.CreateListBoxItemFile(pathNewFile);
                }

                //Parallel.ForEach(Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories), pathNewFile =>
                //{
                //    this.CreateListBoxItemFile(pathNewFile);
                //});

                //int maxConcurrency = Environment.ProcessorCount; // или любое другое желаемое количество параллельных задач
                //SemaphoreSlim semaphoreSlim = new SemaphoreSlim(maxConcurrency);

                //List<Task> tasks = new List<Task>();
                //foreach (string pathNewFile in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories))
                //{
                //    tasks.Add(Task.Run(async () =>
                //    {
                //        await semaphoreSlim.WaitAsync();
                //        try
                //        {
                //            this.CreateListBoxItemFile(pathNewFile);
                //        }
                //        finally
                //        {
                //            semaphoreSlim.Release();
                //        }
                //    }));
                //}

                //await Task.WhenAll(tasks);

                stopwatch.Stop();

                MessageBox.Show($"Было затрачено: {stopwatch.ElapsedMilliseconds} мс");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stopFindButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void choiceDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog folderBrowserDialog = new Winforms.FolderBrowserDialog();
            Winforms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == Winforms.DialogResult.OK)
            {
                this.directoryPathComboBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void filesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.saveReportButton.Visibility == Visibility.Visible)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Вы уверены, что не хотите сохранить отчёт?", "Подтверждение", MessageBoxButton.YesNo);
                
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            // RADIOBUTTON
            this.radioButtonsUniformGrid.Visibility = Visibility.Visible;
            this.saveReportButton.Visibility = Visibility.Hidden;

            this.saveCopyFileOptionRadioButton.Visibility = Visibility.Hidden;
            this.replaceOriginalFileOptionRadioButton.Visibility = Visibility.Hidden;
            //

            this.ApplyRichTextBoxStyles();

            int indexSelectedFile = this.filesListBox.SelectedIndex;

            if (indexSelectedFile != -1)
            {
                string pathSelectedFile = this.report.FoundFiles[indexSelectedFile].FileInfo.FullName;

                try
                {
                    TextRange textRange = new TextRange(this.fileContentRichTextBox.Document.ContentStart, this.fileContentRichTextBox.Document.ContentEnd);

                    using (FileStream fileStream = new FileStream(pathSelectedFile, FileMode.Open))
                    {
                        if (Path.GetExtension(pathSelectedFile).ToLower() == ".txt")
                        {
                            textRange.Load(fileStream, DataFormats.Text);
                        }
                    }

                    // RADIOBUTTON
                    this.saveCopyFileOptionRadioButton.Visibility = Visibility.Visible;
                    this.replaceOriginalFileOptionRadioButton.Visibility = Visibility.Visible;

                    if (this.report.FoundFiles[indexSelectedFile].FileSaveOption == FileSaveOption.SaveCopy)
                    {
                        this.saveCopyFileOptionRadioButton.IsChecked = true;
                    }
                    else if (this.report.FoundFiles[indexSelectedFile].FileSaveOption == FileSaveOption.ReplaceOriginal)
                    {
                        this.replaceOriginalFileOptionRadioButton.IsChecked = true;
                    }
                    //
                }
                catch (Exception ex)
                {
                    Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Ошибка",
                        Content = ex.Message,
                    };
                    messageBox.Show();
                }
            }
        }



        private void addFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Текстовый файл (*.txt)|*.txt" };
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                string pathNewFile = openFileDialog.FileName;

                this.CreateListBoxItemFile(pathNewFile);
            }
        }

        private void delFileButton_Click(object sender, RoutedEventArgs e)
        {
            int indexSelectedFile = this.filesListBox.SelectedIndex;

            if (indexSelectedFile != -1)
            {
                this.filesListBox.Items.RemoveAt(indexSelectedFile);
                this.report.FoundFiles.RemoveAt(indexSelectedFile);
            }
        }

        private void clsFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.filesListBox.Items.Clear();
            this.report.FoundFiles.Clear();
        }




        private async void startCensorshipButton_Click(object sender, RoutedEventArgs e)
        {
            // TO DO
            // Тут тоже лютая многопоточка нужна
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = this.report.FoundFiles.Count * this.report.CensorWords.Count;
            this.progressBar.Value = 0;

            foreach (FoundFile foundFile in this.report.FoundFiles)
            {
                string filePath = foundFile.FileInfo.FullName;

                List<string> oldLines = File.ReadAllLines(filePath).Where(l => !String.IsNullOrWhiteSpace(l)).ToList();
                List<string> newLines = oldLines;

                foreach (string censorWord in this.report.CensorWords)
                {
                    newLines = this.CensorWordsSearch(newLines, censorWord, foundFile);

                    this.progressBar.Value++;
                    await Task.Delay(10);
                }

                if (newLines.Count > 0)
                {
                    //this.EditFile(newLines, foundFile); // сохранение файлов
                }
            }

            // TO DO
            // Хочу, чтобы в RichTextBox прям построчно добавлялся отчёт, но тогда нужно переделать
            this.ApplyRichTextBoxStyles();
            this.fileContentRichTextBox.AppendText(this.report.GenerateReport());

            this.radioButtonsUniformGrid.Visibility = Visibility.Hidden;
            this.saveReportButton.Visibility = Visibility.Visible;
            //
        }

        private List<string> CensorWordsSearch(List<string> oldLines, string censorWord, FoundFile foundFile)
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
            string filePath;

            // Параметры сохранения
            if (foundFile.FileSaveOption == FileSaveOption.ReplaceOriginal)
            {
                // Чтобы и копия не перезаписалась, если вдруг она существует
                int i = 0;
                do
                {
                    string fileName = Path.GetFileNameWithoutExtension(foundFile.FileInfo.FullName) + "_copy" + (i == 0 ? "" : $"{i}");
                    filePath = Path.Combine(foundFile.FileInfo.DirectoryName, fileName, foundFile.FileInfo.Extension);
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


        private void ApplyRichTextBoxStyles()
        {
            FlowDocument newFlowDocument = new FlowDocument();

            FontFamily fontFamily = new FontFamily("Segoe UI Variable");
            double fontSize = 14;

            newFlowDocument.FontFamily = fontFamily;
            newFlowDocument.FontSize = fontSize;

            this.fileContentRichTextBox.Document = newFlowDocument;
        }




        private void openFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                try
                {
                    string censorFilePath = openFileDialog.FileName;
                    List<string> allLines = File.ReadAllLines(censorFilePath).Where(l => !String.IsNullOrWhiteSpace(l)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                    this.report.CensorWords.AddRange(allLines);
                    this.censorWordListBox.ItemsSource = ListBoxItems.CreateCensorWordItems(this.report.CensorWords);
                }
                catch (Exception ex)
                {
                    Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Ошибка",
                        Content = ex.Message,
                    };
                    messageBox.Show();
                }
            }
        }
        private void addCensorWordButton_Click(object sender, RoutedEventArgs e)
        {
            this.censorWordListBox.ItemsSource = null;
            this.report.CensorWords.Add("Недоделал");
            this.censorWordListBox.ItemsSource = ListBoxItems.CreateCensorWordItems(this.report.CensorWords);
        }

        private void delCensorWordButton_Click(object sender, RoutedEventArgs e)
        {
            int indexSelectedCensorWord = this.censorWordListBox.SelectedIndex;

            if (indexSelectedCensorWord != -1)
            {
                this.censorWordListBox.ItemsSource = null;
                this.report.CensorWords.RemoveAt(indexSelectedCensorWord);
                this.censorWordListBox.ItemsSource = ListBoxItems.CreateCensorWordItems(this.report.CensorWords);
            }
        }

        private void clsCensorWordsListButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.censorWordListBox.Items.Count > 0)
            {
                this.censorWordListBox.ItemsSource = null;
                this.report.CensorWords.Clear();
                this.censorWordListBox.ItemsSource = ListBoxItems.CreateCensorWordItems(this.report.CensorWords);
            }
        }




        private async void CreateListBoxItemFile(string filePath)
        {
            //foreach (FoundFile foundFile in this.report.FoundFiles) // проверка на наличие файла в списке
            //{
            //    if (foundFile.FileInfo.FullName == filePath)
            //    {
            //        MessageBox.Show("Файл уже был добавлен.");
            //        return;
            //    }
            //}

            FoundFile newFoundFile = new FoundFile(filePath);
            this.report.FoundFiles.Add(newFoundFile);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.filesListBox.Items.Add(ListBoxItems.CreateFoundFileItem(newFoundFile));
            }));
        }





        private void fileOptionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.report != null)
            {
                int indexSelectedFile = this.filesListBox.SelectedIndex;

                if (this.saveCopyFileOptionRadioButton.IsChecked == true)
                {
                    this.report.FoundFiles[indexSelectedFile].FileSaveOption = FileSaveOption.SaveCopy;
                }
                else if (this.replaceOriginalFileOptionRadioButton.IsChecked == true)
                {
                    this.report.FoundFiles[indexSelectedFile].FileSaveOption = FileSaveOption.ReplaceOriginal;
                }
            }
        }

        private void saveReportButton_Click(object sender, RoutedEventArgs e)
        {
            this.report.SaveReportFile(this.fileContentRichTextBox.Document); // сохранение отчёта
        }
    }
}
