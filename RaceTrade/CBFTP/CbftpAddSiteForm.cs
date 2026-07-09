using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RaceTrade
{
    public partial class CbftpAddSiteForm : AntdUI.Window
    {
        public enum CbftpSiteFormMode
        {
            Add,
            Edit
        }

        private readonly CbftpSiteFormMode _mode;
        private readonly CbftpServer _preselectedServer;
        private readonly string _initialSiteName;

        // used when editing – which site are we patching on the API?
        private string _editingOriginalName;

        // === Skiplist helper model ===
        private class SkiplistEntry
        {
            public string Action { get; set; }
            public bool Dir { get; set; }
            public bool File { get; set; }
            public string Pattern { get; set; }
            public bool Regex { get; set; }
            public string Scope { get; set; }
        }

        // default: Add mode, no preselected server
        public CbftpAddSiteForm()
            : this(CbftpSiteFormMode.Add, null, null)
        {
        }

        // main constructor: you can call this for Add or Edit
        public CbftpAddSiteForm(CbftpSiteFormMode mode, CbftpServer preselectedServer, string initialSiteName = null)
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            _mode = mode;
            _preselectedServer = preselectedServer;
            _initialSiteName = initialSiteName;

            // basic UI tweaks
            btnSave.Text = (_mode == CbftpSiteFormMode.Add) ? "Create site" : "Save changes";
            Text = (_mode == CbftpSiteFormMode.Add) ? "Add Site to CBFTP" : "Edit Site on CBFTP";

            // wire combo events
            cbftpServerCombo.SelectedIndexChanged += cbftpServerCombo_SelectedIndexChanged;
            Edit_cbftp_sites.SelectedIndexChanged += Edit_cbftp_sites_SelectedIndexChanged;

            LoadCbftpServers();
        }



        /// <summary>
        /// Safely set a NumericUpDown value from a JToken (int), clamping to [Minimum, Maximum].
        /// Handles negative values like -1 that CBFTP may use for "unlimited".
        /// </summary>
        private void SetNumericFromToken(NumericUpDown control, JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return;

            try
            {
                // use decimal since NumericUpDown.Value is decimal
                decimal val = token.Value<decimal>();

                if (val < control.Minimum) val = control.Minimum;
                if (val > control.Maximum) val = control.Maximum;

                control.Value = val;
            }
            catch
            {
                // if something weird happens, just ignore and keep existing value
            }
        }




        // === Load CBFTP servers into combo ===
        private void LoadCbftpServers()
        {
            cbftpServerCombo.Items.Clear();

            string filePath = "cbftp/cbftp_config.json";
            if (!File.Exists(filePath))
            {
                lblServerStatus.Text = "No CBFTP servers configured. Please add one first.";
                lblServerStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                var jsonContent = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<Config>(jsonContent);

                if (config?.CbftpServers == null || !config.CbftpServers.Any())
                {
                    lblServerStatus.Text = "No CBFTP servers found in configuration.";
                    lblServerStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                foreach (var server in config.CbftpServers)
                {
                    cbftpServerCombo.Items.Add(server);
                }

                cbftpServerCombo.DisplayMember = "Id";

                if (cbftpServerCombo.Items.Count > 0)
                {
                    if (_preselectedServer != null)
                    {
                        var match = config.CbftpServers
                            .FirstOrDefault(s => s.Id == _preselectedServer.Id);

                        if (match != null)
                            cbftpServerCombo.SelectedItem = match;
                        else
                            cbftpServerCombo.SelectedIndex = 0;
                    }
                    else
                    {
                        cbftpServerCombo.SelectedIndex = 0;
                    }

                    lblServerStatus.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblServerStatus.Text = $"Error loading CBFTP servers: {ex.Message}";
                lblServerStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        // when server changes in EDIT mode, load list of sites
        private async void cbftpServerCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mode == CbftpSiteFormMode.Edit)
            {
                await LoadSitesForSelectedServerAsync();
            }
        }

        // when user picks a site in the edit combo, load its details
        private async void Edit_cbftp_sites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mode == CbftpSiteFormMode.Edit)
            {
                await LoadSelectedSiteDetailsAsync();
            }
        }

        // === load list of site names into Edit_cbftp_sites for selected server ===
        private async Task LoadSitesForSelectedServerAsync()
        {
            Edit_cbftp_sites.Items.Clear();
            _editingOriginalName = null;

            var server = cbftpServerCombo.SelectedItem as CbftpServer;
            if (server == null) return;

            try
            {
                string password = SecureConfig.Decrypt(server.Password);

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, ssl) => true
                };

                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var authBytes = Encoding.UTF8.GetBytes(":" + password);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                string endpoint = server.Host.Contains("://") ? server.Host : $"https://{server.Host}";
                if (!endpoint.EndsWith($":{server.Port}"))
                {
                    endpoint = $"{endpoint}:{server.Port}";
                }

                // GET /sites  ->  ["SITE1","SITE2",...]
                var response = await client.GetAsync($"{endpoint}/sites");
                if (!response.IsSuccessStatusCode)
                {
                    lblServerStatus.Text = $"Error loading sites: {(int)response.StatusCode} {response.ReasonPhrase}";
                    lblServerStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var sitesArray = JArray.Parse(json);

                foreach (var token in sitesArray)
                {
                    var name = token?.ToString();
                    if (!string.IsNullOrEmpty(name))
                        Edit_cbftp_sites.Items.Add(name);
                }

                if (Edit_cbftp_sites.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty(_initialSiteName))
                    {
                        int idx = Edit_cbftp_sites.Items.IndexOf(_initialSiteName);
                        Edit_cbftp_sites.SelectedIndex = idx >= 0 ? idx : 0;
                    }
                    else
                    {
                        Edit_cbftp_sites.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                lblServerStatus.Text = $"Error loading sites: {ex.Message}";
                lblServerStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        // === load full site JSON and populate all controls ===
        private async Task LoadSelectedSiteDetailsAsync()
        {
            var server = cbftpServerCombo.SelectedItem as CbftpServer;
            var siteName = Edit_cbftp_sites.SelectedItem as string;

            if (server == null || string.IsNullOrEmpty(siteName))
                return;

            try
            {
                string password = SecureConfig.Decrypt(server.Password);

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, ssl) => true
                };

                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var authBytes = Encoding.UTF8.GetBytes(":" + password);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                string endpoint = server.Host.Contains("://") ? server.Host : $"https://{server.Host}";
                if (!endpoint.EndsWith($":{server.Port}"))
                {
                    endpoint = $"{endpoint}:{server.Port}";
                }

                var escaped = Uri.EscapeDataString(siteName);
                var response = await client.GetAsync($"{endpoint}/sites/{escaped}");
                if (!response.IsSuccessStatusCode)
                {
                    DialogHelper.ShowError($"Failed to load site {siteName}:\n{(int)response.StatusCode} {response.ReasonPhrase}");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var obj = JObject.Parse(json);

                // 🔹 Use selected name as the "original" and textbox value
                _editingOriginalName = siteName;          
                txtName.Text = siteName;                  

                // If API *does* happen to send a name field, we can override:
                var apiName = obj["name"]?.ToString();    // OPTIONAL
                if (!string.IsNullOrEmpty(apiName))
                {
                    _editingOriginalName = apiName;
                    txtName.Text = apiName;
                }

                // BASIC
                txtAddresses.Text = string.Join(Environment.NewLine,
                    obj["addresses"]?.Select(a => (string)a) ?? Enumerable.Empty<string>());
                txtUser.Text = obj["user"]?.ToString() ?? "";
                txtPassword.Text = obj["password"]?.ToString() ?? "";
                txtBasePath.Text = obj["base_path"]?.ToString() ?? "/";
                chkDisabled.Checked = obj["disabled"]?.ToObject<bool?>() ?? false;
                cmbPriority.SelectedItem = obj["priority"]?.ToString() ?? "NORMAL";
                cmbListFrequency.SelectedItem = obj["list_frequency"]?.ToString() ?? "AUTO";


                // LIMITS
                SetNumericFromToken(nudMaxLogins, obj["max_logins"]);
                SetNumericFromToken(nudMaxSimUp, obj["max_sim_up"]);
                SetNumericFromToken(nudMaxSimDown, obj["max_sim_down"]);

                // TLS / TRANSFER
                cmbTlsMode.SelectedItem = obj["tls_mode"]?.ToString() ?? "AUTH_TLS";
                cmbTransferProtocol.SelectedItem = obj["transfer_protocol"]?.ToString() ?? "IPV4_ONLY";
                cmbTlsTransferPolicy.SelectedItem = obj["tls_transfer_policy"]?.ToString() ?? "PREFER_OFF";
                cmbTransferSourcePolicy.SelectedItem = obj["transfer_source_policy"]?.ToString() ?? "ALLOW";
                cmbTransferTargetPolicy.SelectedItem = obj["transfer_target_policy"]?.ToString() ?? "ALLOW";
                cmbListCommand.SelectedItem = obj["list_command"]?.ToString() ?? "STAT_L";

                SetNumericFromToken(nudMaxIdleTime, obj["max_idle_time"]);

                chkStayLoggedIn.Checked = obj["stay_logged_in"]?.ToObject<bool?>() ?? false;
                chkCepr.Checked = obj["cepr"]?.ToObject<bool?>() ?? false;
                chkSscn.Checked = obj["sscn"]?.ToObject<bool?>() ?? false;
                chkCpsv.Checked = obj["cpsv"]?.ToObject<bool?>() ?? false;
                chkBrokenPasv.Checked = obj["broken_pasv"]?.ToObject<bool?>() ?? false;
                chkForceBinaryMode.Checked = obj["force_binary_mode"]?.ToObject<bool?>() ?? false;
                chkLeaveFreeSlot.Checked = obj["leave_free_slot"]?.ToObject<bool?>() ?? false;
                chkPret.Checked = obj["pret"]?.ToObject<bool?>() ?? false;
                chkXdupe.Checked = obj["xdupe"]?.ToObject<bool?>() ?? false;

                // ALLOW
                cmbAllowDownload.SelectedItem = obj["allow_download"]?.ToString() ?? "YES";
                cmbAllowUpload.SelectedItem = obj["allow_upload"]?.ToString() ?? "YES";

                // PROXY
                cmbProxyType.SelectedItem = obj["proxy_type"]?.ToString() ?? "GLOBAL";
                txtProxyName.Text = obj["proxy_name"]?.ToString() ?? "";

                // AFFILS
                lstAffils.Items.Clear();
                var affils = obj["affils"] as JArray;
                if (affils != null)
                {
                    foreach (var a in affils)
                        lstAffils.Items.Add(a.ToString());
                }

                // SECTIONS
                lvSections.Items.Clear();
                var sections = obj["sections"] as JArray;
                if (sections != null)
                {
                    foreach (var s in sections)
                    {
                        var n = s["name"]?.ToString();
                        var p = s["path"]?.ToString();
                        if (!string.IsNullOrEmpty(n) && p != null)
                        {
                            lvSections.Items.Add(new ListViewItem(new[] { n, p }));
                        }
                    }
                }

                // SKIPLIST
                lvSkiplist.Items.Clear();
                var skip = obj["skiplist"] as JArray;
                if (skip != null)
                {
                    foreach (var s in skip)
                    {
                        var entry = new SkiplistEntry
                        {
                            Action = s["action"]?.ToString() ?? "DENY",
                            Dir = s["dir"]?.ToObject<bool?>() ?? false,
                            File = s["file"]?.ToObject<bool?>() ?? true,
                            Pattern = s["pattern"]?.ToString() ?? "",
                            Regex = s["regex"]?.ToObject<bool?>() ?? false,
                            Scope = s["scope"]?.ToString() ?? "ALL"
                        };

                        var item = new ListViewItem(new[] { entry.Action, entry.Scope, entry.Pattern })
                        {
                            Tag = entry
                        };
                        lvSkiplist.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError("Error loading site details:\n\n" + ex.Message);
            }
        }

        // === Button handlers wired from Designer ===

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAddAffil_Click(object sender, EventArgs e)
        {
            var affil = txtAffil.Text.Trim();
            if (affil.Length > 0 && !lstAffils.Items.Contains(affil))
            {
                lstAffils.Items.Add(affil);
                txtAffil.Clear();
            }
        }

        private void btnRemoveAffil_Click(object sender, EventArgs e)
        {
            var sel = lstAffils.SelectedItem;
            if (sel != null)
            {
                lstAffils.Items.Remove(sel);
            }
        }

        private void btnAddSection_Click(object sender, EventArgs e)
        {
            var name = txtSectionName.Text.Trim();
            var path = txtSectionPath.Text.Trim();
            if (name.Length == 0 || path.Length == 0) return;

            var item = new ListViewItem(new[] { name, path });
            lvSections.Items.Add(item);
            txtSectionName.Clear();
            txtSectionPath.Clear();
        }

        private void btnRemoveSection_Click(object sender, EventArgs e)
        {
            if (lvSections.SelectedItems.Count > 0)
            {
                lvSections.Items.Remove(lvSections.SelectedItems[0]);
            }
        }

        private void btnAddSkip_Click(object sender, EventArgs e)
        {
            var pattern = txtSkipPattern.Text.Trim();
            if (pattern.Length == 0) return;

            var entry = new SkiplistEntry
            {
                Action = cmbSkipAction.SelectedItem?.ToString() ?? "DENY",
                Scope = cmbSkipScope.SelectedItem?.ToString() ?? "ALL",
                Dir = chkSkipDir.Checked,
                File = chkSkipFile.Checked || !chkSkipDir.Checked,
                Regex = chkSkipRegex.Checked,
                Pattern = pattern
            };

            var item = new ListViewItem(new[] { entry.Action, entry.Scope, entry.Pattern })
            {
                Tag = entry
            };
            lvSkiplist.Items.Add(item);
            txtSkipPattern.Clear();
        }

        private void btnRemoveSkip_Click(object sender, EventArgs e)
        {
            if (lvSkiplist.SelectedItems.Count > 0)
                lvSkiplist.Items.Remove(lvSkiplist.SelectedItems[0]);
        }

        // === POST /sites (Add) or PATCH /sites/{name} (Edit) ===
        private async Task SaveAsync()
        {
            if (cbftpServerCombo.SelectedItem == null)
            {
                DialogHelper.ShowWarning("Please select a CBFTP server.");
                return;
            }

            var server = (CbftpServer)cbftpServerCombo.SelectedItem;

            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                DialogHelper.ShowWarning("Site name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddresses.Text))
            {
                DialogHelper.ShowWarning("At least one address is required.");
                return;
            }

            try
            {
                btnSave.Enabled = false;
                Cursor = Cursors.WaitCursor;

                string apiPassword = SecureConfig.Decrypt(server.Password);

                var addresses = txtAddresses.Text
                    .Split(new[] { '\r', '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .Where(a => a.Length > 0)
                    .ToList();

                var affils = lstAffils.Items.Cast<object>().Select(o => o.ToString()).ToList();

                var sections = lvSections.Items
                    .Cast<ListViewItem>()
                    .Select(it => new
                    {
                        name = it.SubItems[0].Text,
                        path = it.SubItems[1].Text
                    })
                    .ToList();

                var skiplistEntries = lvSkiplist.Items
                    .Cast<ListViewItem>()
                    .Select(it => (SkiplistEntry)it.Tag)
                    .Where(t => t != null)
                    .Select(t => new
                    {
                        action = t.Action,
                        dir = t.Dir,
                        file = t.File,
                        pattern = t.Pattern,
                        regex = t.Regex,
                        scope = t.Scope
                    })
                    .ToList();

                var body = new
                {
                    name = name,
                    addresses = addresses,
                    affils = affils,
                    allow_download = cmbAllowDownload.SelectedItem?.ToString() ?? "YES",
                    allow_upload = cmbAllowUpload.SelectedItem?.ToString() ?? "YES",
                    base_path = string.IsNullOrWhiteSpace(txtBasePath.Text) ? "/" : txtBasePath.Text.Trim(),
                    broken_pasv = chkBrokenPasv.Checked,
                    cepr = chkCepr.Checked,
                    cpsv = chkCpsv.Checked,
                    disabled = chkDisabled.Checked,
                    force_binary_mode = chkForceBinaryMode.Checked,
                    leave_free_slot = chkLeaveFreeSlot.Checked,
                    list_command = cmbListCommand.SelectedItem?.ToString() ?? "STAT_L",
                    max_idle_time = (int)nudMaxIdleTime.Value,
                    max_logins = (int)nudMaxLogins.Value,
                    max_sim_up = (int)nudMaxSimUp.Value,
                    max_sim_down = (int)nudMaxSimDown.Value,
                    password = txtPassword.Text,
                    pret = chkPret.Checked,
                    priority = cmbPriority.SelectedItem?.ToString() ?? "NORMAL",
                    list_frequency = cmbListFrequency.SelectedItem?.ToString() ?? "AUTO",
                    proxy_name = txtProxyName.Text.Trim(),
                    proxy_type = cmbProxyType.SelectedItem?.ToString() ?? "GLOBAL",
                    sections = sections,
                    skiplist = skiplistEntries,
                    sscn = chkSscn.Checked,
                    stay_logged_in = chkStayLoggedIn.Checked,
                    tls_mode = cmbTlsMode.SelectedItem?.ToString() ?? "AUTH_TLS",
                    tls_transfer_policy = cmbTlsTransferPolicy.SelectedItem?.ToString() ?? "PREFER_OFF",
                    transfer_protocol = cmbTransferProtocol.SelectedItem?.ToString() ?? "IPV4_ONLY",
                    transfer_source_policy = cmbTransferSourcePolicy.SelectedItem?.ToString() ?? "ALLOW",
                    transfer_target_policy = cmbTransferTargetPolicy.SelectedItem?.ToString() ?? "ALLOW",
                    user = txtUser.Text.Trim(),
                    xdupe = chkXdupe.Checked
                };

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, ssl) => true
                };

                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var authBytes = Encoding.UTF8.GetBytes(":" + apiPassword);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                string endpoint = server.Host.Contains("://") ? server.Host : $"https://{server.Host}";
                if (!endpoint.EndsWith($":{server.Port}"))
                {
                    endpoint = $"{endpoint}:{server.Port}";
                }

                string json = JsonConvert.SerializeObject(body, Formatting.Indented);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                LogManager.Debug((_mode == CbftpSiteFormMode.Add ? "POST" : "PATCH") + " /sites body:\n" + json);

                HttpResponseMessage response;
                string url;

                if (_mode == CbftpSiteFormMode.Add)
                {
                    url = $"{endpoint}/sites";
                    response = await client.PostAsync(url, content);
                }
                else
                {
                    var apiName = _editingOriginalName ?? name;
                    var escaped = Uri.EscapeDataString(apiName);
                    url = $"{endpoint}/sites/{escaped}";

                    var req = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                    {
                        Content = content
                    };
                    response = await client.SendAsync(req);
                }

                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    LogManager.Error($"Error saving site: HTTP {(int)response.StatusCode} {response.ReasonPhrase} - {responseText}");
                    DialogHelper.ShowError(
                        $"Failed to {(_mode == CbftpSiteFormMode.Add ? "create" : "update")} site.\n\n" +
                        $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}\n\n{responseText}");
                    return;
                }

                if (_mode == CbftpSiteFormMode.Add)
                {
                    LogManager.Success($"Successfully created site '{name}' on CBFTP {server.Id}");
                    DialogHelper.ShowSuccess($"Site '{name}' was created successfully on {server.Id}.");
                }
                else
                {
                    LogManager.Success($"Successfully updated site '{name}' on CBFTP {server.Id}");
                    DialogHelper.ShowSuccess($"Site '{name}' was updated successfully on {server.Id}.");
                }

                // Close correctly depending on how the form was opened
                if (this.Modal)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error saving site via CBFTP API");
                DialogHelper.ShowError("Error saving site:\n\n" + ex.Message);
            }
            finally
            {
                btnSave.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}
