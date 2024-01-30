using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Censor
{
    internal static class ListBoxItems
    {
        public static Grid CreateCensorWordItem(string censorWord)
        {
            // Создание сетки
            Grid wordItemGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 42,
            };

            // Создание прямоугольника со скошенными углами
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(255, 15, 15, 15)),
                RadiusX = 5,
                RadiusY = 5,
            };

            wordItemGrid.Children.Add(rectangle);

            // Создание Label с словом
            Label fileNameLabel = new Label
            {
                Content = censorWord,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            wordItemGrid.Children.Add(fileNameLabel);

            // Возврат готового элемента
            return wordItemGrid;
        }

        public static Grid CreateFoundFileItem(FoundFile foundFile)
        {
            // Создание сетки
            Grid fileItemGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 42,
            };

            RowDefinition rowDefinition = new RowDefinition() { Height = GridLength.Auto };
            RowDefinition rowDefinition2 = new RowDefinition() { Height = new GridLength(20) };

            fileItemGrid.RowDefinitions.Add(rowDefinition);
            fileItemGrid.RowDefinitions.Add(rowDefinition2);

            ColumnDefinition columnDefinition = new ColumnDefinition { Width = new GridLength(40) };
            ColumnDefinition columnDefinition2 = new ColumnDefinition();

            fileItemGrid.ColumnDefinitions.Add(columnDefinition);
            fileItemGrid.ColumnDefinitions.Add(columnDefinition2);

            // Создание прямоугольника со скошенными углами
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(255, 15, 15, 15)),
                RadiusX = 5,
                RadiusY = 5,
            };

            Grid.SetRowSpan(rectangle, 2);
            Grid.SetColumnSpan(rectangle, 2);

            fileItemGrid.Children.Add(rectangle);

            // Создание Label с именем файла
            Label fileNameLabel = new Label
            {
                Content = foundFile.FileInfo.Name,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(fileNameLabel, 0);
            Grid.SetColumn(fileNameLabel, 1);

            fileItemGrid.Children.Add(fileNameLabel);

            // Создание Label с путём к файлу
            Label filePathLabel = new Label
            {
                Content = foundFile.FileInfo.FullName,
                FontSize = 10,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(filePathLabel, 1);
            Grid.SetColumn(filePathLabel, 1);

            fileItemGrid.Children.Add(filePathLabel);

            // Создание картинки с иконкой файла
            Image image = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Icons/txt.png")),
                Stretch = Stretch.UniformToFill,
                Height = 40,
                Width = 40,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            Grid.SetRowSpan(image, 2);

            fileItemGrid.Children.Add(image);

            // Возврат готового элемента
            return fileItemGrid;
        }
    }
}
