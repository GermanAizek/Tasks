﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Threading;

//Todo: Startup Services as well.

namespace Tasks {
    public partial class frmStartupPrograms : Form {
        public frmStartupPrograms() 
        {
            InitializeComponent();
            RenderStartupsOnListWiew();
            CheckTheme();
        }

        private void RefreshList() {
            StartupProcesses.Items.Clear();
            RenderStartupsOnListWiew();
        }

        private void RenderStartupsOnListWiew() {
            foreach (ManagementObject strt in (new ManagementClass("Win32_StartupCommand").GetInstances())) { 
                StartupProcesses.Items.Add(strt["Name"].ToString(), 0).SubItems.Add(strt["Location"].ToString()); 
            }
        }

        private void frmStartupPrograms_Load(object sender, EventArgs e) {
            txtTargetPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\";
            CheckTheme();
        }

        private void StartupProcesses_SelectedIndexChanged(object sender, EventArgs e) {}


        private void button1_Click(object sender, EventArgs e) {
            string fileStartup = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\" + StartupProcesses.SelectedItems[0].SubItems[0].Text + ".exe";

            if (StartupProcesses.SelectedItems[0].SubItems[1].Text == "Startup")
            {
                try
                {
                    File.Delete(fileStartup);
                    RefreshList();
                }
                catch
                {
                    MessageBox.Show("An error has occurred.");
                }
            }


            if(StartupProcesses.SelectedItems[0].SubItems[1].Text == "HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")
            {
                try
                {
                    string keyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Run\\";
                    string Value = StartupProcesses.SelectedItems[0].SubItems[0].Text;

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, false))
                    {
                        if (key == null)
                        {
                            MessageBox.Show("This key does not exist or equals null.");
                            Debug.Print("Key Path: " + keyName + "(/)" + Value);
                        }
                        else
                        {
                            key.DeleteValue(keyName + Value);
                            RefreshList();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error. " + ex.Message);
                }
            }
        }
        
        private void button2_Click(object sender, EventArgs e) {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Executables|*.exe" }) {

                if (ofd.ShowDialog() == DialogResult.OK)  {
                    try
                    {
                        string program = ofd.FileName.ToString();

                        txtFileName.Text = ofd.FileName;
                        FileInfo fileInfo = new FileInfo(txtFileName.Text);
                        txtTargetPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup", fileInfo.Name);
                        File.Copy(txtFileName.Text, txtTargetPath.Text, true);
                        RefreshList();
                    }
                    catch
                    {
                        MessageBox.Show("There was an unexpected error.");
                    }
                  
                } else RefreshList();
            }
        }
        
        private void button3_Click(object sender, EventArgs e) {
            try 
            {
                Process.Start("explorer.exe", @Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
            } 
            catch(Exception ex) 
            {
                MessageBox.Show("An error occurred. " + ex.Message); 
            }
        }

        private void button4_Click(object sender, EventArgs e) {}
        
        public class StartUpProgram {
            public string Name { get; set; }
            public string Path { get; set; }            
            public override string ToString() { return Name; }
        }
        private void button4_Click_1(object sender, EventArgs e) { RefreshList(); }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileStartup = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\" + StartupProcesses.SelectedItems[0].SubItems[0].Text + ".exe";
            if (StartupProcesses.SelectedItems[0].SubItems[1].Text == "Startup")
            {
                try
                {
                    File.Delete(fileStartup);
                    RefreshList();
                }
                catch
                {
                    MessageBox.Show("An error has occurred.");
                }
            }
        }

        private void moreInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Currently being worked on.");
        }

        public void CheckTheme()
        {
            if (Properties.Settings.Default.Theme == "dark")
            {

           
            }

            if (Properties.Settings.Default.Theme == "light")
            {
                StartupProcesses.BackColor = Color.White;
                StartupProcesses.ForeColor = Color.Black;
                this.BackColor = Color.White;
            }
        }


    }
}
