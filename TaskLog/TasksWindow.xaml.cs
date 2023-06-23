﻿using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.IdentityModel.Tokens;
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
using TaskLog.Entities;

namespace TaskLog
{
    /// <summary>
    /// Логика взаимодействия для TasksWindow.xaml
    /// </summary>
    public partial class TasksWindow : Window
    {
        public long UserId { get; set; }
        public TasksWindow()
        {
            InitializeComponent();
            FillDataMainGrid();
            dateFromBox.DisplayDateEnd = DateTime.Now;
            dateToBox.DisplayDateEnd = DateTime.Now;
        }

        public void FillDataMainGrid()
        {
            MainDataGrid.ItemsSource = DbUtils.db.Tasks.Select(p => new
            {
                p.TaskId,
                DbUtils.db.Components.FirstOrDefault(x => x.CompId == p.CompId).CompOemName,
                DbUtils.db.Users.FirstOrDefault(x => x.UserId == p.UserId).UserName,
                DateTime = $"{DbUtils.db.EventLog.FirstOrDefault(x => x.TaskId == p.TaskId).EventTimestamp:f}",
                DbUtils.db.EventLog.Where(x => x.TaskId == p.TaskId).OrderByDescending(x => x.EventTimestamp).FirstOrDefault().EventType
            }).ToList();
        }

        private void FilteringButton_Click(object sender, RoutedEventArgs e)
        {
            var dataTasks = DbUtils.db.Tasks.Select(p => new
            {
                p.TaskId,
                DbUtils.db.Components.FirstOrDefault(x => x.CompId == p.CompId).CompOemName,
                DbUtils.db.Users.FirstOrDefault(x => x.UserId == p.UserId).UserName,
                DateTimeNotFormat = DbUtils.db.EventLog.FirstOrDefault(x => x.TaskId == p.TaskId).EventTimestamp,
                DbUtils.db.EventLog.Where(x => x.TaskId == p.TaskId).OrderByDescending(x => x.EventTimestamp).FirstOrDefault().EventType
            });

            if(!componentBox.Text.IsNullOrEmpty())
                dataTasks = dataTasks.Where(w => w.CompOemName == componentBox.Text);
            if (!creatorBox.Text.IsNullOrEmpty())
                dataTasks = dataTasks.Where(w => w.UserName.Contains(creatorBox.Text));
            if (!dateFromBox.Text.IsNullOrEmpty())
                dataTasks = dataTasks.Where(w => w.DateTimeNotFormat.Date >= Convert.ToDateTime(dateFromBox.Text).Date);
            if (!dateToBox.Text.IsNullOrEmpty())
                dataTasks = dataTasks.Where(w => w.DateTimeNotFormat.Date <= Convert.ToDateTime(dateToBox.Text).Date);
            if (!typeBox.Text.IsNullOrEmpty())
                dataTasks = dataTasks.Where(w => w.EventType == typeBox.Text);

            MainDataGrid.ItemsSource = dataTasks.Select(p => new {
                p.TaskId,
                p.CompOemName,
                p.UserName,
                DateTime = $"{p.DateTimeNotFormat:f}",
                p.EventType
            }).ToList();
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void MainDataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            foreach (var col in MainDataGrid.Columns)
            {
                col.Header = TranslatorForDataGridColumns.Translate(col.Header.ToString());
            }
        }

        private void MainDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MainDataGrid.SelectedCells.Count() == 0) return; 
            var cellInfo = MainDataGrid.SelectedCells[0];
            int content = Convert.ToInt16((cellInfo.Column.GetCellContent(cellInfo.Item) as TextBlock).Text);
            var table = DbUtils.db.Tasks.Where(x => x.TaskId == content).FirstOrDefault();
            if (table != null) 
            {
                TaskViewWindow taskViewWindow = new TaskViewWindow(table);
                taskViewWindow.ShowDialog();
                FillDataMainGrid();
            }
            else
            {
                MessageBox.Show("error");
                FillDataMainGrid();
                return;
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow();
            addTaskWindow.Show();
            addTaskWindow.UserId = UserId;
            this.Close();
        }
    }
}
