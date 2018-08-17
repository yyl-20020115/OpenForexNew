// -----
// GNU General Public License
// The Open Forex Platform is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version. 
// The Open Forex Platform is distributed in the hope that it will be useful, but without any warranty; without even the implied warranty of merchantability or fitness for a particular purpose.  
// See the GNU Lesser General Public License for more details.
// -----

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using ForexPlatform;
using System.Drawing;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Author: based on contributions by "bolo"
    /// </summary>
    public partial class ExpertEditorControl : CommonBaseControl
    {
        ExpertInformation _expertInformation = null;
        /// <summary>
        /// Set the expert container to show it source code (if available).
        /// </summary>
        public ExpertInformation ExpertInformation
        {
            get { return _expertInformation; }
            set 
            { 
                _expertInformation = value;
                UpdateUI(true);
            }
        }

        public delegate void ExpertUpdatedDelegate(ExpertEditorControl editorControl);
        public event ExpertUpdatedDelegate ExpertUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public ExpertEditorControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            codeEditorControl.Enabled = true;

            AIMS.Libraries.CodeEditor.SyntaxFiles.CodeEditorSyntaxLoader.SetSyntax(
                codeEditorControl, AIMS.Libraries.CodeEditor.SyntaxFiles.SyntaxLanguage.CSharp);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        /// <param name="updateDocumentText"></param>
        void UpdateUI(bool updateDocumentText)
        {
            this.Enabled = _expertInformation != null;

            if (_expertInformation == null)
            {
                syntaxDocument.Text = "";
                return;
            }

            //codeEditorControl.ReadOnly = _expertInformation.IsExternal || !toolStripButtonEdit.Checked;
            toolStripButtonCodeSnippets.Enabled = !codeEditorControl.ReadOnly;
            listViewMessages.Items.Clear();

            this.listViewMessages.Visible = !_expertInformation.IsExternal;
            toolStripButtonEdit.Enabled = !_expertInformation.IsExternal;
            toolStripButtonEdit.Checked = toolStripButtonEdit.Checked && toolStripButtonEdit.Enabled;
            toolStripButtonBuild.Enabled = !_expertInformation.IsExternal;
            toolStripButtonSave.Enabled = !_expertInformation.IsExternal;
            toolStripLabelFileName.Text = _expertInformation.FilePath;

            this.toolStripButtonSave.Enabled = !_expertInformation.IsSavedLocally;

            if (updateDocumentText)
            {
                if (_expertInformation.IsExternal)
                {
                    syntaxDocument.Text = "No source code available for this expert.";
                }
                else
                {
                    syntaxDocument.Text = _expertInformation.SourceCode;
                }
            }
        }

        private Assembly CompileSource(String source)
        {
            Dictionary<string, int> messages;
            Assembly assembly = CompilationHelper.CompileSourceToAssembly(source, out messages);

            this.listViewMessages.Items.Clear();
            if (assembly == null)
            {
                foreach (string message in messages.Keys)
                {
                    ListViewItem item = new ListViewItem(message, "error");
                    item.Tag = messages[message];
                    this.listViewMessages.Items.Add(item);
                }
            }
            else
            {
                ListViewItem item = new ListViewItem("Compiled succesfully", "ok");
                this.listViewMessages.Items.Add(item);
            }

            return assembly;
        }

        private void toolStripButtonBuild_Click(object sender, EventArgs e)
        {
            String source = codeEditorControl.Document.Text;
            CompileSource(source);
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            _expertInformation.SourceCode = syntaxDocument.Text;
            _expertInformation.SaveLocal();

            UpdateUI(false);
        }

        private void listViewMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewMessages.SelectedIndices.Count == 1
                && listViewMessages.SelectedItems[0].Tag != null)
            {
                int errorRow = (int)listViewMessages.SelectedItems[0].Tag;
                codeEditorControl.Selection.ClearSelection();
                codeEditorControl.Selection.Bounds.FirstRow = errorRow - 1;
                codeEditorControl.Selection.Bounds.LastRow = errorRow;
                codeEditorControl.Selection.Bounds.FirstColumn = 0;
                codeEditorControl.Selection.Bounds.LastColumn = 0;
                codeEditorControl.Refresh();
            }
        }

        private void toolStripButtonEdit_CheckStateChanged(object sender, EventArgs e)
        {
            //codeEditorControl.ReadOnly = !toolStripButtonEdit.Checked;
            //if (codeEditorControl.ReadOnly)
            //{
            //    codeEditorControl.BackColor = Color.WhiteSmoke;
            //}
            //else
            //{
            //    codeEditorControl.BackColor = SystemColors.Window;
            //}
        }

        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            if (listViewMessages.Columns.Count > 0)
            {
                listViewMessages.Columns[0].Width = -2;
            }
        }

        private void codeEditorControl_TextChanged(object sender, EventArgs e)
        {
            if (_expertInformation != null && codeEditorControl.ReadOnly == false)
            {
                _expertInformation.SourceCode = syntaxDocument.Text;
                UpdateUI(false);
            }
        }

        private void toolStripButtonProperties_Click(object sender, EventArgs e)
        {
            PropertiesForm form = new PropertiesForm("Expert Type Properties", this._expertInformation);
            form.ShowDialog();
            if (ExpertUpdateEvent != null)
            {
                ExpertUpdateEvent(this);
            }
        }

    }
}
