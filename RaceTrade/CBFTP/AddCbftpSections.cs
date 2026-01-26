using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RaceTrade
{
    public partial class AddCbftpSection : Form
    {
        private const string SectionFilePath = "sections/cbftp_sections.json";
        private Dictionary<string, string> cbftpSections;

        public AddCbftpSection()
        {
            InitializeComponent();
            EnsureSectionFileExists();
            LoadCbftpSections();
        }

        /// <summary>
        /// Ensures the sections directory and cbftp_sections.json file exist.
        /// Creates them with default empty structure if they don't exist.
        /// </summary>
        private void EnsureSectionFileExists()
        {
            try
            {
                string directoryPath = "sections";

                // Create directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    LogManager.Info($"Created directory: {directoryPath}");
                }

                // Create file with empty config if it doesn't exist
                if (!File.Exists(SectionFilePath))
                {
                    var emptyData = new SectionData
                    {
                        Sections = new Dictionary<string, string>(),
                        CbftpSections = new Dictionary<string, string>()
                    };
                    var emptyJson = JsonConvert.SerializeObject(emptyData, Formatting.Indented);
                    File.WriteAllText(SectionFilePath, emptyJson);
                    LogManager.Info($"Created sections file: {SectionFilePath}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Failed to ensure sections file exists");
                MessageBox.Show($"Error creating sections file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCbftpSections()
        {
            try
            {
                // Ensure file exists before loading
                EnsureSectionFileExists();

                // Read and deserialize the JSON file
                var jsonContent = File.ReadAllText(SectionFilePath);
                var data = JsonConvert.DeserializeObject<SectionData>(jsonContent);

                // Initialize cbftpSections as a dictionary
                cbftpSections = data?.CbftpSections ?? new Dictionary<string, string>();

                // Populate the ListBox with the section values
                List_cbftp_sections.Items.Clear();
                foreach (var section in cbftpSections.Values)
                {
                    List_cbftp_sections.Items.Add(section);
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error loading sections");
                MessageBox.Show($"Error loading sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbftpSections = new Dictionary<string, string>(); // Initialize an empty dictionary in case of failure
            }
        }

        private void Add_Section_cbftp_Click(object sender, EventArgs e)
        {
            string newSection = cbftp_Section_name_field.Text.Trim();

            if (string.IsNullOrEmpty(newSection))
            {
                MessageBox.Show("Please enter a valid section name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbftpSections.Values.Contains(newSection))
            {
                MessageBox.Show("The section already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Generate a unique key for the new section
                string newKey = $"cbftp_section{cbftpSections.Count + 1}";

                // Add the new section
                cbftpSections[newKey] = newSection;

                // Save to the JSON file
                SaveCbftpSections();

                // Reload the sections
                LoadCbftpSections();

                cbftp_Section_name_field.Clear();
                MessageBox.Show($"Section '{newSection}' added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error adding section");
                MessageBox.Show($"Error adding section: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Delete_cbftp_Section_Click(object sender, EventArgs e)
        {
            if (List_cbftp_sections.SelectedItem == null)
            {
                MessageBox.Show("Please select a section to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedSection = List_cbftp_sections.SelectedItem.ToString();

            try
            {
                // Find the key associated with the selected value and remove it
                var keyToRemove = cbftpSections.FirstOrDefault(kvp => kvp.Value == selectedSection).Key;
                if (keyToRemove != null)
                {
                    cbftpSections.Remove(keyToRemove);

                    // Save to the JSON file
                    SaveCbftpSections();

                    // Reload the sections
                    LoadCbftpSections();

                    cbftp_Section_name_field.Clear();
                    MessageBox.Show($"Section '{selectedSection}' deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Section '{selectedSection}' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error deleting section");
                MessageBox.Show($"Error deleting section: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCbftpSections()
        {
            try
            {
                // Ensure directory exists before saving
                EnsureSectionFileExists();

                // Serialize the sections to JSON
                var data = new SectionData { CbftpSections = cbftpSections };
                var jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);

                // Write to the JSON file
                File.WriteAllText(SectionFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error saving sections");
                MessageBox.Show($"Error saving sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void List_sections_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Populate the TextBox when a section is selected in the ListBox
            if (List_cbftp_sections.SelectedItem != null)
            {
                cbftp_Section_name_field.Text = List_cbftp_sections.SelectedItem.ToString();
            }
        }
    }

    // JSON data model for sections
    public class SectionData
    {
        [JsonProperty("sections")]
        public Dictionary<string, string> Sections { get; set; }

        [JsonProperty("cbftp_sections")]
        public Dictionary<string, string> CbftpSections { get; set; }
    }
}