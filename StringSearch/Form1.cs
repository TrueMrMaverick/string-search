using SearchResultsSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringSearch
{


    public partial class Form1 : Form
    {
        public string fileName = "";
        public List<SearchResults> searchResults;
        public string text;


        public Form1()
        {
            InitializeComponent();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\Users\Kathrine\Desktop";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = theDialog.FileName.ToString();
                text = File.ReadAllText(fileName, Encoding.GetEncoding(1251));
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            string pattern = patternField.Text;
            searchResults = new List<SearchResults>();
            searchResults.Add(nativSearch(this.text, pattern));
            searchResults.Add(knpSearch(this.text, pattern));
            searchResults.Add(bmSearch(this.text, pattern));
            searchResults.Add(shiftAndSearch(this.text, pattern));
        }

        private SearchResults nativSearch(string text, string pattern)
        {
            int count = 0;
            int time = 0;
            var before = DateTime.Now;

            for (var i = 0; i < text.Length - pattern.Length; i++)
            {
                if (text.Substring(i, pattern.Length) == pattern)
                {
                    count++;
                }
            }
       

            var after = DateTime.Now;
            time = (int) (dateToMs(after) - dateToMs(before));
            return new SearchResults
            {
                Name = "Нативный алгоритм",
                Time = time / 1000,
                Count = count
            };
        }

        private SearchResults knpSearch(string text, string pattern)
        {
            int count = 0;
            int time = 0;
            var before = DateTime.Now;


            var pf = prefixFunction(pattern + '@' + text);
            for (var i = 0; i < text.Length - 1; i++)
            {
                if (pf[pattern.Length + i + 1] == pattern.Length)
                {
                    count++;
                }
            }

            var after = DateTime.Now;
            time = (int)(dateToMs(after) - dateToMs(before));
            return new SearchResults
            {
                Name = "Алгоритм Кнута-Морриса-Пратта",
                Time = time / 1000,
                Count = count
            };
        }

        private int[] prefixFunction(String str)
        {
            int[] prefixFunc = new int[str.Length];
            prefixFunc[0] = 0;
            for (var i = 1; i < str.Length - 1; i++)
            {
                var k = prefixFunc[i - 1];
                while (k > 0 && str[i] != str[k])
                {
                    k = prefixFunc[k - 1];
                }
                if (str[i] == str[k])
                {
                    k++;
                }
                prefixFunc[i] = k;
            }
            return prefixFunc;
        }

        private SearchResults bmSearch(string text, string pattern)
        {
            int count = 0;
            int time = 0;
            var before = DateTime.Now;

            var textLen = text.Length;
            var patternLen = pattern.Length;
            if (textLen < patternLen)
            {
                return null;
            }
            var map = new Dictionary<char, int>();
            for (var t = 0; t < patternLen - 1; t++)
            {
                map.Add(pattern[t], patternLen - t - 1);
            }
            var i = patternLen - 1;
            var j = i;
            var k = i;
            while (i <= textLen - 1)
            {
                j = patternLen - 1;
                k = i;
                while (j >= 0 && text[k] == pattern[j])
                {
                    j--;
                    k--;
                }
                if (j == -1)
                {
                    count++;
                }
                i += map.ContainsKey(text[i]) ? map[text[i]] : patternLen;
            }

            var after = DateTime.Now;
            time = (int)(dateToMs(after) - dateToMs(before));
            return new SearchResults
            {
                Name = "Алгоритм Кнута-Морриса-Пратта",
                Time = time / 1000,
                Count = count
            };
        }

        private SearchResults shiftAndSearch(string text, string pattern)
        {
            int count = 0;
            int time = 0;
            var before = DateTime.Now;
            using (MD5 md5Hash = MD5.Create())
            {
                var hashText = GetMd5Hash(md5Hash, text.Substring(0, pattern.Length));
                var hashPattern = GetMd5Hash(md5Hash, pattern);
                for (var i = 0; i < text.Length - pattern.Length; i++)
                {
                    if (VerifyMd5Hash(md5Hash, hashText, hashPattern))
                    {
                        count++;
                    }
                    hashText = GetMd5Hash(md5Hash, text.Substring(i, pattern.Length));
                }
            }
           

            var after = DateTime.Now;
            time = (int)(dateToMs(after) - dateToMs(before));
            return new SearchResults
            {
                Name = "Алгоритм Кнута-Морриса-Пратта",
                Time = time / 1000,
                Count = count
            };
        }


        private long dateToMs(DateTime date)
        {
            return (long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }


        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string hash1, string hash2)
        {
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hash1, hash2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

namespace SearchResultsSpace
{
    public class SearchResults
    {
        public String Name { get; set; }
        public int Time { get; set; }
        public int Count { get; set; }
    }
}
