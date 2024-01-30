using System;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Censor
{
    internal class CensorWordList : IEnumerable<string>
    {
        private List<string> censorWords;
        private ListBox censorWordListBox;

        public CensorWordList(ListBox censorWordListBox)
        {
            this.censorWords = new List<string>();
            this.censorWordListBox = censorWordListBox;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.censorWords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.censorWords.GetEnumerator();
        }

        public void Add(string censorWord)
        {
            if (!this.censorWords.Contains(censorWord))
            {
                this.censorWordListBox.Items.Add(ListBoxItems.CreateCensorWordItem(censorWord));
                this.censorWords.Add(censorWord);
            }
        }

        public void AddRange(List<string> censorWords)
        {
            foreach (string censorWord in censorWords)
            {
                if (!this.censorWords.Contains(censorWord))
                {
                    this.censorWordListBox.Items.Add(ListBoxItems.CreateCensorWordItem(censorWord));
                    this.censorWords.Add(censorWord);
                }
            }
        }

        public void Remove(string censorWord)
        {
            if (this.censorWords.Contains(censorWord))
            {
                this.censorWordListBox.Items.Remove(ListBoxItems.CreateCensorWordItem(censorWord));
                this.censorWords.Remove(censorWord);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index <= this.censorWords.Count)
            {
                this.censorWordListBox.Items.RemoveAt(index);
                this.censorWords.RemoveAt(index);
            }
        }

        public void Clear()
        {
            if (this.censorWords.Count > 0)
            {
                this.censorWordListBox.Items.Clear();
                this.censorWords.Clear();
            }
        }

        public int Count()
        {
            return this.censorWords.Count;
        }
    }
}
