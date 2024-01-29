using System;


namespace Censor
{
    internal static class InfoBox
    {
        public static void Show(string content, string title = "")
        {
            Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Width = AutoWidth(content, title),
                Height = AutoHeight(content),

                MinHeight = 0,
                MinWidth = 0,

                MaxHeight = 2000,
                MaxWidth = 2000,

                Title = title,
                Content = content,

                ShowFooter = false,
            };

            messageBox.ShowDialog();
        }

        public static void Show(string content, string title, int width, int height)
        {
            Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Width = width,
                Height = height,

                MinHeight = 0,
                MinWidth = 0,

                MaxHeight = 2000,
                MaxWidth = 2000,

                Title = title,
                Content = content,

                ShowFooter = false,
            };

            messageBox.ShowDialog();
        }

        private static int AutoWidth(string content, string title)
        {
            int max = 0;
            foreach (string str in content.Split('\n'))
            {
                if (max < str.Length)
                {
                    max = str.Length;
                }
            }

            if (title.Length > max)
            {
                max = title.Length;
            }

            return 20 + max * 7;
        }

        private static int AutoHeight(string content)
        {
            return content.Split('\n').Length <= 1 ? 100 : 80 + content.Split('\n').Length * 20;
        }
    }
}
