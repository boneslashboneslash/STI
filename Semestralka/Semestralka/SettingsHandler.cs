﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Semestralka
{
    public class SettingsHandler
    {
        private TextBox urlTB;
        private TextBox storageTB;
        /// <summary>
        /// Sets default properties rows from project settings
        /// </summary>
        public SettingsHandler(StackPanel sp_settings)
        {
            foreach (SettingsProperty item in Semestralka.Properties.Settings.Default.Properties)
            {
                sp_settings.Children.Add(settingRow(item.Name));
            }
        }
      
        /// <summary>
        /// Define autosave row
        /// </summary>
        private Grid settingRow(string Name)
        {
            try
            {
                //new grid row for controls
                Grid result = new Grid();

                //init column, row width
                result.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(80)
                });
                result.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(400)
                });

                //init new label
                Label lb = new Label();
                lb.Name = Name + "_label";
                lb.Content = Name + " :";
                lb.FontSize = 15;
                lb.HorizontalContentAlignment = HorizontalAlignment.Left;

                //binding dat and init ui element type
                Binding bd = new Binding();
                bd.Source = Semestralka.Properties.Settings.Default;
                bd.Path = new PropertyPath(Name);
                TextBox tx = new TextBox();
                tx.SetBinding(TextBox.TextProperty, bd);
                tx.FontSize = 15;
                tx.Name = Name + "_TextBox";
                //alignment
                tx.TextAlignment = TextAlignment.Left;
                //event lost focus
                tx.LostFocus += tx_LostFocus;

                //place label and element
                Grid.SetColumn(lb, 0);
                result.Children.Add(lb);
                Grid.SetColumn(tx, 1);
                result.Children.Add(tx);
                if (tx.Name == "url_TextBox")
                {
                    urlTB = tx;
                }

                if (tx.Name == "storage_TextBox")
                {
                    storageTB = tx;
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Getter urlTB text
        /// </summary>
        public string getUrlTB()
        {
            return urlTB.Text;
        }

        /// <summary>
        /// Getter storageTB text
        /// </summary>
        public string getStorageTB()
        {
            return storageTB.Text;
        }

        /// <summary>
        /// Event for save rows
        /// </summary>
        void tx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Semestralka.Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Error saving changes in settings!");
            }
        }
    }
}