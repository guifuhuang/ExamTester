﻿using System;
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
using System.Windows.Xps.Packaging;

namespace ExamTester
{
    /// <summary>
    /// ViewAnswerForm.xaml 的交互逻辑
    /// </summary>
    public partial class ViewAnswerForm : Window
    {
        public ViewAnswerForm()
        {
            InitializeComponent();
        }
        public ViewAnswerForm(XpsDocument xpsDoc)
        {
            InitializeComponent();
            if (xpsDoc != null)
            {
                docViewer.Document = xpsDoc.GetFixedDocumentSequence();
                docViewer.FitToWidth();
            }
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
            }
        }
    }
}
