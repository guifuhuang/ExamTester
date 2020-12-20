using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.Threading.Tasks;
using System.Xml;

namespace ExamTester
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // 当前页数索引
        private int _currentTermIndex = 0;
        // 总计题数
        private int _termsCount = 0;
        // 题库目录
        private string _termsFolderPath = "";
        // 答案目录
        // 所有题的索引
        private Dictionary<int, FileInfo> _dicTerms = new Dictionary<int, FileInfo>();
        // 所有题的索引
        private Dictionary<int, XpsDocument> _dicTermDocs = new Dictionary<int, XpsDocument>();
        // 所有答案的索引
        private Dictionary<int, XpsDocument> _dicAnswerDocs = new Dictionary<int, XpsDocument>();
        // 所有历史path
        private List<String> _lstPath = new List<string>();
        // 
        //Microsoft.Office.Interop.Word.Application _wordApplication = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            //this._wordApplication = new Microsoft.Office.Interop.Word.Application();
            //加载保存过的记录
            this.LoadHistory();
        }
        /// <summary>
        /// 加载保存的历史记录
        /// </summary>
        private void LoadHistory()
        {
            string filename = "history.txt";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string cfgName = System.IO.Path.Combine(path, "ExamTester", filename);
            this._lstPath = new List<string>();
            if (File.Exists(cfgName))
            {
                string content = File.ReadAllText(cfgName);
                string[] arrPath = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (string str in arrPath)
                {
                    if (!string.IsNullOrWhiteSpace(str) && !this._lstPath.Contains(str))
                    {
                        this._lstPath.Add(str);
                    }
                }
            }
        }
        private void Loaddoc(string wordDocName)
        {
            docViewer.Document = ConvertWordToXPS(wordDocName).GetFixedDocumentSequence();
            docViewer.FitToWidth();
        }
        //private void LoaddocByIndex()
        //{
        //    docViewer.Document = ConvertWordToXPS(this._dicTerms[this._currentTermIndex].FullName).GetFixedDocumentSequence();
        //    docViewer.FitToWidth();
        //}
        private void LoaddocByIndex()
        {
            docViewer.Document = this._dicTermDocs[this._currentTermIndex].GetFixedDocumentSequence();
            docViewer.FitToWidth();

            this.SetProcessPage();
        }
        private async System.Threading.Tasks.Task LoadAllDocsAsync()
        {
            foreach (var key in this._dicTerms.Keys)
            {
                if (key == 0)
                {
                    await this.AddToListAsync(key);
                    this.LoaddocByIndex();
                }
                else
                {
                    this.AddToListAsync(key);
                }
            }
        }
        /// <summary>
        /// 加载答案
        /// </summary>
        private async void LoadAllAnswerDocsAsync()
        {
            List<System.Threading.Tasks.Task> lstTask = new List<System.Threading.Tasks.Task>();
            lstTask.Add(System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var key in this._dicTerms.Keys)
                {
                    this.AddAnswerDocToListAsync(key);
                }
            }));
            await System.Threading.Tasks.Task.WhenAll(lstTask.ToArray());
        }
        /// <summary>
        /// 将答案添加到Dictionary
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task AddAnswerDocToListAsync(int index)
        {
            string filePath = System.IO.Path.Combine(this._dicTerms[index].Directory.FullName, "a", "a" + this._dicTerms[index].Name);
            XpsDocument xpsDoc = await ConvertWordToXPSAsync(filePath);
            this._dicAnswerDocs.Add(index, xpsDoc);
        }

        /// <summary>
        /// 将试题添加到Dictionary
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task AddToListAsync(int index)
        {
            XpsDocument xpsDoc = await ConvertWordToXPSAsync(this._dicTerms[index].FullName);
            this._dicTermDocs.Add(index, xpsDoc);
        }
        private async System.Threading.Tasks.Task LoaddocByIndexAsync()
        {
            XpsDocument xpsDoc = await ConvertWordToXPSAsync(this._dicTerms[this._currentTermIndex].FullName);
            docViewer.Document = xpsDoc.GetFixedDocumentSequence();
            docViewer.FitToWidth();
        }
        private async System.Threading.Tasks.Task<XpsDocument> ConvertWordToXPSAsync(string wordDocName)
        {
            return await System.Threading.Tasks.Task.Run(() => {
                return this.ConvertWordToXPS(wordDocName);
            });
        }
        private XpsDocument ConvertWordToXPS(string wordDocName)
        {
            FileInfo fi = new FileInfo(wordDocName);
            XpsDocument result = null;
            //string xpsDocName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), fi.Name);
            string xpsFolderPath = System.IO.Path.Combine(this._termsFolderPath, "t");
            if (!Directory.Exists(xpsFolderPath))
            {
                Directory.CreateDirectory(xpsFolderPath);
            }
            string xpsDocName = System.IO.Path.Combine(xpsFolderPath, fi.Name);
            xpsDocName = xpsDocName.Replace(".docx", ".xps").Replace(".doc", ".xps");
            Microsoft.Office.Interop.Word.Application wordApplication = new Microsoft.Office.Interop.Word.Application();
            try
            {
                if (!File.Exists(xpsDocName))
                {
                    wordApplication.Documents.Add(wordDocName);
                    Document doc = wordApplication.ActiveDocument;
                    doc.ExportAsFixedFormat(xpsDocName, WdExportFormat.wdExportFormatXPS, false, WdExportOptimizeFor.wdExportOptimizeForPrint, WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentContent, true, true, WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false, Type.Missing);
                    result = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);
                    Console.WriteLine(string.Format("生成{0}",xpsDocName));
                }
                else
                {
                    result = new XpsDocument(xpsDocName, FileAccess.Read);
                    Console.WriteLine(string.Format("找到{0}", xpsDocName));
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                wordApplication.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }

            wordApplication.Quit(WdSaveOptions.wdDoNotSaveChanges);

            return result;
        }
        private async System.Threading.Tasks.Task<bool> ConvertAll()
        {
            Microsoft.Office.Interop.Word.Application wordApplication = new Microsoft.Office.Interop.Word.Application();
            bool rst = await System.Threading.Tasks.Task.Run(() => {
                try
                {
                    foreach (var key in this._dicTerms.Keys)
                    {
                        string filePath = System.IO.Path.Combine(this._dicTerms[key].Directory.FullName, this._dicTerms[key].Name);
                        XpsDocument xpsDoc = ConvertWordToXPS2(filePath, wordApplication);
                        this._dicTermDocs.Add(key, xpsDoc);
                        //if (key == 0)
                        //{
                        //    this._currentTermIndex = 0;
                        //    this.LoaddocByIndex();
                        //}
                        string answerFilePath = System.IO.Path.Combine(this._dicTerms[key].Directory.FullName, "a", "a" + this._dicTerms[key].Name);
                        XpsDocument xpsAnswerDoc = ConvertWordToXPS2(answerFilePath, wordApplication);
                        this._dicAnswerDocs.Add(key, xpsAnswerDoc);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    wordApplication.Quit(WdSaveOptions.wdDoNotSaveChanges);
                    return false;
                }
            });

            wordApplication.Quit(WdSaveOptions.wdDoNotSaveChanges);
            if (rst)
            {
                this._currentTermIndex = 0;
                this.LoaddocByIndex();
            }
            return rst;
        }
        /// <summary>
        /// Scrollviewer加上触屏滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SCManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        private XpsDocument ConvertWordToXPS2(string wordDocName, Microsoft.Office.Interop.Word.Application wordApplication)
        {
            FileInfo fi = new FileInfo(wordDocName);
            XpsDocument result = null;
            //string xpsDocName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), fi.Name);
            string xpsFolderPath = System.IO.Path.Combine(this._termsFolderPath, "t");
            if (!Directory.Exists(xpsFolderPath))
            {
                Directory.CreateDirectory(xpsFolderPath);
            }
            string xpsDocName = System.IO.Path.Combine(xpsFolderPath, fi.Name);
            xpsDocName = xpsDocName.Replace(".docx", ".xps").Replace(".doc", ".xps");
            if (!File.Exists(xpsDocName))
            {
                try
                {
                    Document doc = wordApplication.Documents.Add(wordDocName);
                    // 关闭Word的拼写检查就行了。
                    doc.SpellingChecked = false;
                    doc.ShowSpellingErrors = false;

                    
                    doc.ExportAsFixedFormat(xpsDocName, WdExportFormat.wdExportFormatXPS, false, WdExportOptimizeFor.wdExportOptimizeForPrint, WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentContent, true, true, WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false, Type.Missing);
                    result = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);
                    Console.WriteLine(string.Format("生成{0}", xpsDocName));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("生成{0},出错了:{1}", xpsDocName, ex.Message));
                }
            }
            else
            {
                result = new XpsDocument(xpsDocName, FileAccess.Read);
                Console.WriteLine(string.Format("找到{0}", xpsDocName));
            }
            return result;
        }
        private void SetProcessPage()
        {
            this.lblProcess.Content = string.Format("第{0}页/共{1}页", (this._currentTermIndex + 1).ToString(), this._termsCount.ToString());
            this.txtPage.Text = (this._currentTermIndex + 1).ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
        }
        /// <summary>
        /// 【事件处理】"打开试题目录"按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            History history = History.Instance();
            history.Owner = this;
            history.PathList = this._lstPath;
            bool? dialogResult = history.ShowDialog();
            if (dialogResult != null && (bool)dialogResult && !string.IsNullOrEmpty(history.SelectedPath) && this._lstPath.Contains(history.SelectedPath))
            {
                this._termsFolderPath = history.SelectedPath;
                this._currentTermIndex = 0;
                this._dicTerms = new Dictionary<int, FileInfo>();
                DirectoryInfo di = new DirectoryInfo(this._termsFolderPath);
                FileInfo[] files = di.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    this._dicTerms.Add(i, files[i]);
                }
                this._termsCount = this._dicTerms.Count;
                Task<bool> tsk = this.ConvertAll();
                //if (tsk.Result)
                //{
                //    this._currentTermIndex = 0;
                //    this.LoaddocByIndex();
                //}
                return;
            }

            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (this._lstPath.Contains(openFileDialog.SelectedPath))
                {
                    this._lstPath.Remove(openFileDialog.SelectedPath);
                }
                this._lstPath.Add(openFileDialog.SelectedPath);
                //StringBuilder sb = new StringBuilder();
                //foreach (string str in this._lstPath)
                //{
                //    sb.AppendLine(str);
                //}
                //string filename = "history.txt";
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //string cfgName = System.IO.Path.Combine(path,"ExamTester", filename);

                //IoHelper.SaveStringToFilePath(cfgName, sb.ToString());

                this._termsFolderPath = openFileDialog.SelectedPath;
                this._currentTermIndex = 0;
                this._dicTerms = new Dictionary<int, FileInfo>();
                DirectoryInfo di = new DirectoryInfo(this._termsFolderPath);
                FileInfo[] files = di.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    this._dicTerms.Add(i, files[i]);
                }
                this._termsCount = this._dicTerms.Count;
                Task<bool> tsk = this.ConvertAll();
                //if (tsk.Result)
                //{
                //    this._currentTermIndex = 0;
                //    this.LoaddocByIndex();
                //}
            }
        }

        /// <summary>
        /// 【事件处理】页数文本输入前触发事件
        /// 控制只输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPage_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void btnJump_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtPage.Text))
            {
                return;
            }
            int iPage = int.Parse(this.txtPage.Text.Trim());
            if (iPage <= this._termsCount && this._dicTermDocs.Keys.Contains(iPage-1))
            {
                this._currentTermIndex = iPage-1;
                this.LoaddocByIndex();
                //this.SetProcessPage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if ((this._currentTermIndex + 1) < this._termsCount && this._dicTermDocs.Keys.Contains(this._currentTermIndex + 1))
            {
                this._currentTermIndex++;
                this.LoaddocByIndex();
                //this.SetProcessPage();
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if ((this._currentTermIndex) > 0 && this._dicTermDocs.Keys.Contains(this._currentTermIndex-1))
            {
                this._currentTermIndex--;
                this.LoaddocByIndex();
                //this.SetProcessPage();
            }
        }

        private void txtPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.btnJump_Click(this.btnJump,null);
            }
        }

        private void btnViewAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (this._dicAnswerDocs.Keys.Contains(this._currentTermIndex))
            {
                ViewAnswerForm viewForm = new ViewAnswerForm(this._dicAnswerDocs[this._currentTermIndex]);
                viewForm.ShowDialog();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (this._wordApplication != null)
            //{
            //    this._wordApplication.Quit();
            //}
            StringBuilder sb = new StringBuilder();
            foreach (string str in this._lstPath)
            {
                sb.AppendLine(str);
            }
            string filename = "history.txt";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string cfgName = System.IO.Path.Combine(path, "ExamTester", filename);

            IoHelper.SaveStringToFilePath(cfgName, sb.ToString());
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                this.btnNext_Click(this.btnNext, null);
            }
            else if (e.Key == Key.Left)
            {
                this.btnPrev_Click(this.btnPrev, null);
            }
            else if(e.Key == Key.Enter)
            {
                this.btnViewAnswer_Click(this.btnViewAnswer, null);
            }
        }
    }
}
