﻿/* This file is part of AHD ID3 Tag Editor (AITE)
 * A program that edit and create ID3 Tag.
 *
 * Copyright © Alaa Ibrahim Hadid 2012 - 2022
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AHD.ID3;
using AHD.ID3.Types;
using AHD.ID3.Frames;
using AHD.ID3.Editor.Base;
namespace AHD.ID3.Editor.GUI
{
    public partial class C_TermsOfUseFrame : EditorControl
    {
        public C_TermsOfUseFrame()
        {
            InitializeComponent();
            comboBoxControl_language.comboBox1.Items.Add("");
            foreach (string lang in ID3FrameConsts.Languages)
                comboBoxControl_language.comboBox1.Items.Add(lang);
            comboBoxControl_language.comboBox1.SelectedItem = ID3FrameConsts.GetLanguage("ENG");
            comboBoxControl_language.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            DisableControls();
        }
        private TermsOfUseFrame frame;
        private void EnableControls()
        {
            toolStrip1.Enabled = comboBoxControl_language.Enabled = richTextBox1.Enabled = true;
            Label1.Text = "";
        }
        private void DisableControls()
        {
            toolStrip1.Enabled = comboBoxControl_language.Enabled = richTextBox1.Enabled = false;
            Label1.Text = "";
        }
        public override string[] SelectedFiles
        {
            get
            {
                return base.SelectedFiles;
            }
            set
            {
                base.SelectedFiles = value; ReloadFrame(null, null);
            }
        }
        public override void ClearFields()
        {
            comboBoxControl_language.comboBox1.SelectedItem = ID3FrameConsts.GetLanguage("ENG");
            richTextBox1.Text = "";
        }
        public override void DisableSaving()
        {
            base.DisableSaving();
            toolStripButton1.Enabled = false;
        }
        private void ReloadFrame(object sender, EventArgs e)
        {
            ClearFields();
            if (files == null)
            {
                DisableControls();
                return;
            }
            if (files.Length == 0)
            {
                DisableControls();
                return;
            }
            if (files.Length > 1)
            {
                DisableControls();
                Label1.Text = "You can only edit 1 file at a time.";
                return;
            }
            //load frames
            LoadFrames(files[0]);
        }
        /// <summary>
        /// Fill fields from file
        /// </summary>
        /// <param name="file">The file to load</param>
        public void LoadFrames(string file)
        {
            ID3v2 v2 = new ID3v2();
            if (v2.Load(file) == Result.Success)
            {
                LoadFrames(v2);
            }
        }
        /// <summary>
        /// Fill fields using ID3V2 object.
        /// </summary>
        /// <param name="v2">The ID3V2 object (MUST be loaded a file with load success status)</param>
        public void LoadFrames(ID3v2 v2)
        {
            EnableControls();
            if (v2.TagVersion.Major == 2)
            {
                DisableControls();
                frame = null;
                Label1.Text = "Terms Of Use frame doesn't exist in ID3 Tag version 2.2";
            }
            else
            {
                frame = (TermsOfUseFrame)v2.GetFrameLoaded("USER");
                Label1.Text = "";
            }
            ViewData();
        }
        public void SaveFramesToTag(ID3v2 v2)
        {
            // remove all frames
            v2.RemoveFrameAll("USER");

            frame = new TermsOfUseFrame("USER", "Terms of use", null, 0);
            if (richTextBox1.Text.Length > 0)
            {
                // apply data
                frame.LanguageID = ID3FrameConsts.GetLanguageID(comboBoxControl_language.comboBox1.SelectedItem.ToString());
                frame.Text = richTextBox1.Text;
                
                v2.Frames.Add(frame);
            }
        }
        public override void LoadTag(ID3v2 v2)
        {
            LoadFrames(v2);
        }
        public override void SaveTag(ID3v2 v2)
        {
            SaveFramesToTag(v2);
        }
        private void ViewData()
        {
            if (frame != null)
            {
                EnableControls();
                comboBoxControl_language.comboBox1.SelectedItem = ID3FrameConsts.GetLanguage(frame.LanguageID);
                richTextBox1.Text = frame.Text;
            }
            else
            {
                ClearFields();
            }
        }
        private void SaveChanges(object sender, EventArgs e)
        {
            if (files == null)
            {
                MessageBox.Show("No file selected.");
                return;
            }
            if (files.Length == 0)
            {
                MessageBox.Show("No file selected.");
                return;
            }
            if (files.Length > 1)
            {
                MessageBox.Show("Only one file can be saved at a time");
                return;
            }
            // stop media and clear it
            OnClearMediaRequest();
            // load the original file. Success or not doesn't matter.
            ID3v2 v2 = new ID3v2();
            bool hadTag = v2.Load(files[0]) == Result.Success;

            // we need to make sure about version. If we are creating id3v2, use default version.
            if (!hadTag)
                v2.TagVersion = new ID3Version((byte)ID3TagSettings.ID3V2Version, 0);

            // set flags
            v2.Compression = false;
            v2.Experimental = false;
            if (ID3TagSettings.DropExtendedHeader)
                v2.ExtendedHeader = false;
            v2.Footer = ID3TagSettings.WriteFooter;
            v2.SavePadding = ID3TagSettings.KeepPadding;
            v2.Unsynchronisation = ID3TagSettings.UseUnsynchronisation;
            //save frames
            SaveFramesToTag(v2);

            // Add events
            v2.SaveStart += v2_SaveStart;
            v2.SaveFinished += v2_SaveFinished;
            v2.Progress += v2_Progress;
            // save !
            v2.Save(files[0]);

            OnUpdateRequired();

            // reload media
            OnReloadMediaRequest();
        }
        private void v2_Progress(object sender, ProgressArg e)
        {
            OnProgress(e.Status, e.Progress);
        }
        private void v2_SaveFinished(object sender, EventArgs e)
        {
            OnProgress("Saved.", 100);
            OnProgressFinish();
        }
        private void v2_SaveStart(object sender, EventArgs e)
        {
            OnProgressStart();
        }
    }
}
