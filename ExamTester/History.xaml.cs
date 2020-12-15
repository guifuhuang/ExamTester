using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExamTester
{
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : Window
    {
        public History()
        {
            InitializeComponent();
        }
        private static History _instance;

        public static History Instance()
        {
            if (_instance == null)
            {
                _instance = new History();
            }
            return _instance;
        }
        public List<string> PathList { set; get; }
        public string SelectedPath { get; set; }
        private void initList()
        {
            foreach (string path in this.PathList)
            {
                this.lv.Items.Add(path);
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedPath = e.Source.ToString();
            this.DialogResult = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = this.PathList.IndexOf(((Button)sender).Tag.ToString());
            if (index >= 0 && index < this.lv.Items.Count)
            {
                this.lv.Items.RemoveAt(index);
                this.PathList.RemoveAt(index);
            }
            //if (this.lv.SelectedIndex >= 0 && this.lv.SelectedIndex < this.lv.Items.Count)
            //{
            //    this.lv.Items.RemoveAt(this.lv.SelectedIndex);
            //}
            //if (this.lv.SelectedIndex >= 0 && this.lv.SelectedIndex < this.PathList.Count)
            //{
            //    this.PathList.RemoveAt(this.lv.SelectedIndex);
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.PathList = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.lv.Items.Count; i++)
            {
                this.PathList.Add(this.lv.Items[i].ToSafeString());
            }
            //e.Cancel = true;
            //this.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.initList();
        }

        private void lv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SelectedPath = this.PathList[this.lv.SelectedIndex];
            _instance.DialogResult = true;
        }
    }
}
