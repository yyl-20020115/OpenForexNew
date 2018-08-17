using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using ForexPlatform;

namespace MT4Adapter
{
    /// <summary>
    /// The deployment relies on having the assemblies registered to GAC already.
    /// </summary>
    [UserFriendlyName("MetaTrader4 Integration Wizard")]
    public partial class MT4IntegrationWizardControl : WizardControl
    {
        Platform _platform;

        List<string> _foldersHistory = new List<string>();

        /// <summary>
        /// UI compatibility constructor.
        /// </summary>
        public MT4IntegrationWizardControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Operational constructor.
        /// </summary>
        public MT4IntegrationWizardControl(Platform platform)
        {
            InitializeComponent();

            _platform = platform;

            // See if we have old deployment folders stored in the platform.
            if (_platform.UISerializationInfo.ContainsValue("MT4IntegrationWizardControl.foldersHistory"))
            {
                _foldersHistory.AddRange(_platform.UISerializationInfo.GetValue<string[]>("MT4IntegrationWizardControl.foldersHistory"));
            }
        }

        private void MT4IntegrationWizardControl_Load(object sender, EventArgs e)
        {
            foreach (string folder in _foldersHistory)
            {
                listViewHistory.Items.Add(folder).Checked = true;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (Directory.Exists(@"c:\Program files\"))
            {
                dialog.SelectedPath = @"c:\Program files\";
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxMT4Directory.Text = dialog.SelectedPath;
            }
        }

        private void buttonDeploy_Click(object sender, EventArgs e)
        {
            if (textBoxMT4Directory.Text.EndsWith("\\") == false)
            {
                textBoxMT4Directory.Text += "\\";
            }
            string operationResultMessage;
            if (Deploy(textBoxMT4Directory.Text, out operationResultMessage))
            {// Deployment OK.
                MessageBox.Show(operationResultMessage, "Success", MessageBoxButtons.OK);
            }
            else
            {// Deployment error.
                MessageBox.Show(operationResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool Deploy(string folder, out string operationResultMessage)
        {
            string filesFolder = _platform.Settings.GetMappedPath("FilesFolder");
            if (filesFolder.EndsWith("\\") == false)
            {
                filesFolder += "\\";
            }

            if (folder.EndsWith("\\") == false)
            {
                folder += "\\";
            }

            string mq4FileName = "OFxP.mq4";
            string integrationDllFileName = "MT4Integration.dll";
            string[] integrationDependencyDllFileNames = new string[] { "msvcm90.dll", "msvcp90.dll", "msvcr90.dll" };

            if (Directory.Exists(filesFolder) == false
                || File.Exists(Path.Combine(filesFolder, mq4FileName)) == false
                || File.Exists(Path.Combine(Application.StartupPath, integrationDllFileName)) == false
                || File.Exists(Path.Combine(filesFolder, integrationDependencyDllFileNames[0])) == false
                || File.Exists(Path.Combine(filesFolder, integrationDependencyDllFileNames[1])) == false
                || File.Exists(Path.Combine(filesFolder, integrationDependencyDllFileNames[2])) == false
                )
            {
                operationResultMessage = "Can not find Files folder or required source files.";
                return false;
            }

            // Check if the directory pointed actualy holds a MT4 installation.
            if (Directory.Exists(folder) == false
                || File.Exists(folder + "terminal.exe") == false
                || Directory.Exists(folder + "Experts") == false
                || Directory.Exists(folder + "Experts\\Libraries") == false)
            {
                operationResultMessage = "The selected directory does not seem to contain a MT4 integration.";
                return false;
            }

            try
            {
                // Finally copy over the files.
                File.Copy(Path.Combine(filesFolder, mq4FileName), folder + "Experts\\" + mq4FileName, true);
                File.Copy(Path.Combine(Application.StartupPath, integrationDllFileName), folder + "Experts\\Libraries\\" + integrationDllFileName, true);

                foreach (string name in integrationDependencyDllFileNames)
                {
                    File.Copy(Path.Combine(filesFolder, name), folder + "Experts\\Libraries\\" + name, true);
                }

                operationResultMessage = "Deployment finished successfully.";
            }
            catch (Exception ex)
            {
                operationResultMessage = "Deployment error [" + ex.Message + "].";
                return false;
            }

            if (_foldersHistory.Contains(folder.ToLower()) == false)
            {// Remember folder in history.
                _foldersHistory.Add(folder.ToLower());

                // Store in platform.
                _platform.UISerializationInfo.AddValue("MT4IntegrationWizardControl.foldersHistory", _foldersHistory.ToArray());

                // Also add to list.
                listViewHistory.Items.Add(folder);
            }

            return true;
        }

        private void buttonUnregister_Click(object sender, EventArgs e)
        {
        }

        private void textBoxMT4Directory_TextChanged(object sender, EventArgs e)
        {
            buttonDeploy.Enabled = Directory.Exists(textBoxMT4Directory.Text);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void buttonDeploySelected_Click(object sender, EventArgs e)
        {
            bool success = true;
            string combinedMessage = "";
            foreach (ListViewItem item in listViewHistory.Items)
            {
                if (item.Checked)
                {
                    
                    string operationResultMessage;
                    if (Deploy(item.Text, out operationResultMessage))
                    {
                        combinedMessage += "[Success] " + item.Text + Environment.NewLine;
                    }
                    else
                    {
                        success = false;
                        combinedMessage += "[Failure: " + operationResultMessage + "] " + item.Text + Environment.NewLine;
                    }
                }
            }

            if (success)
            {
                MessageBox.Show(combinedMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(combinedMessage, "Errors in Deployment", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonClearHistory_Click(object sender, EventArgs e)
        {
            //while (listViewHistory.SelectedItems.Count > 0)
            //{
            //    listViewHistory.SelectedItems[0].Remove();
            //}

            listViewHistory.Items.Clear();

            _foldersHistory.Clear();

            // Store in platform.
            _platform.UISerializationInfo.AddValue("MT4IntegrationWizardControl.foldersHistory", _foldersHistory.ToArray());
        }

    }
}
