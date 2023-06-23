﻿using Microsoft.IdentityModel.Tokens;
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
    /// Логика взаимодействия для ComponentWindow.xaml
    /// </summary>
    public partial class ComponentWindow : Window
    {
        public ComponentWindow()
        {
            InitializeComponent();
            FillDataMainGrid();
        }

        private void AddComponentButton_Click(object sender, RoutedEventArgs e)
        {
            AddComponentWindow addComponentWindow = new AddComponentWindow();
            addComponentWindow.ShowDialog();
        }
        public void FillDataMainGrid()
        {
            MainDataGrid.ItemsSource = DbUtils.db.Components.Select(p => new
            {
                p.CompId,
                p.CompOemId,
                p.CompOemVer,
                p.CompOemName,
                p.SwVer
            }).ToList();
        }

        private void FilteringButton_Click(object sender, RoutedEventArgs e)
        {
            var dataComponents = DbUtils.db.Components.Select(p => new
            {
                p.CompOemId,
                p.CompOemVer,
                p.CompOemName,
                p.SwVer
            });

            if (!OemIdBox.Text.IsNullOrEmpty())
                dataComponents = dataComponents.Where(w => w.CompOemId == OemIdBox.Text);
            if (!OemVerBox.Text.IsNullOrEmpty())
                dataComponents = dataComponents.Where(w => w.CompOemVer == OemVerBox.Text);
            if (!OemNameBox.Text.IsNullOrEmpty())
                dataComponents = dataComponents.Where(w => w.CompOemName == OemNameBox.Text);
            if (!SwVerBox.Text.IsNullOrEmpty())
                dataComponents = dataComponents.Where(w => w.SwVer == SwVerBox.Text);

            MainDataGrid.ItemsSource = dataComponents.ToList();


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
            var cellInfo = MainDataGrid.SelectedCells[0];
            int content = Convert.ToInt16((cellInfo.Column.GetCellContent(cellInfo.Item) as TextBlock).Text);
            var table = DbUtils.db.Components.Where(x => x.CompId == content).FirstOrDefault();
            if(table != null)
            {
                AddTaskWindow addTaskWindow = new AddTaskWindow(table);
                addTaskWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("error");
                FillDataMainGrid();
                return;
            }
        }
    }
}
