using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaceTrade;

namespace RaceTrader
{
    public partial class FtpClientForm : Form
    {
        private string currentLeftPath = "/";
        private string currentRightPath = "/";
        private Dictionary<string, SiteInfo> availableSites = new Dictionary<string, SiteInfo>();
        private int leftSortColumn = -1;
        private int rightSortColumn = -1;
        public FtpClientForm()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            this.listViewLeft.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewLeft_ColumnClick);
            this.listViewRight.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewRight_ColumnClick);

            // Initialize logs
            LogToConsole("Console Ready", Color.Cyan);
            LogToTransfer("Transfer Log Ready", Color.Cyan);
        }

        private void FtpClientForm_Load(object sender, EventArgs e)
        {
            LoadSites();
        }

        private void LoadSites()
        {
            try
            {
                UpdateStatus("Loading sites from configuration...");
                ShowProgress(true);

                // Get sites from your SiteConfigManager
                var sites = GetConfiguredSites();

                cboLeftSite.Items.Clear();
                cboRightSite.Items.Clear();

                foreach (var site in sites)
                {
                    cboLeftSite.Items.Add(site.Name);
                    cboRightSite.Items.Add(site.Name);
                    availableSites[site.Name] = site;
                }

                // Don't auto-select - let user choose sites manually
                UpdateStatus($"Loaded {sites.Count} sites. Select sites to browse.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading sites: {ex.Message}");
                MessageBox.Show($"Failed to load sites:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private List<SiteInfo> GetConfiguredSites()
        {
            var sites = new List<SiteInfo>();

            try
            {
                string sitesDir = "sites";

                LogToConsole($"Looking for sites in directory: {Path.GetFullPath(sitesDir)}", Color.Gray);

                if (!Directory.Exists(sitesDir))
                {
                    LogToConsole($"Sites directory not found: {Path.GetFullPath(sitesDir)}", Color.Yellow);
                    return sites;
                }

                var siteFiles = Directory.GetFiles(sitesDir, "*.json");
                LogToConsole($"Found {siteFiles.Length} site files", Color.Gray);

                foreach (var siteFile in siteFiles)
                {
                    try
                    {
                        var siteName = Path.GetFileNameWithoutExtension(siteFile);

                        // Let SiteConfigManager handle everything (blocking, loading, caching)
                        if (SiteConfigManager.TryGetSiteConfig(siteName, out var siteConfig))
                        {
                            LogToConsole($"✓ Loaded: {siteName}", Color.LimeGreen);
                            sites.Add(new SiteInfo
                            {
                                Name = siteName,
                                Config = siteConfig
                            });
                        }
                        else
                        {
                            LogToConsole($"⊗ Skipped: {siteName}", Color.Gray);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogToConsole($"✗ Failed to load {Path.GetFileName(siteFile)}: {ex.Message}", Color.Red);
                    }
                }

                sites = sites.OrderBy(s => s.Name).ToList();
                LogToConsole($"Total sites loaded: {sites.Count}", Color.Cyan);
            }
            catch (Exception ex)
            {
                LogToConsole($"✗ Failed to get configured sites: {ex.Message}", Color.Red);
            }

            return sites;
        }


        // Sorting

        private class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            private SortOrder order;

            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                ListViewItem itemX = (ListViewItem)x;
                ListViewItem itemY = (ListViewItem)y;

                var fileX = itemX.Tag as FileItem;
                var fileY = itemY.Tag as FileItem;

                // Always keep ".." at the top
                if (fileX?.Name == "..")
                    return -1;
                if (fileY?.Name == "..")
                    return 1;

                int returnVal = 0;

                switch (col)
                {
                    case 0: // Name column
                            // Sort directories before files, then by name
                        if (fileX.IsDirectory && !fileY.IsDirectory)
                            returnVal = -1;
                        else if (!fileX.IsDirectory && fileY.IsDirectory)
                            returnVal = 1;
                        else
                            returnVal = string.Compare(fileX.Name, fileY.Name, StringComparison.OrdinalIgnoreCase);
                        break;

                    case 1: // Size column
                            // Directories should be together
                        if (fileX.IsDirectory && !fileY.IsDirectory)
                            returnVal = -1;
                        else if (!fileX.IsDirectory && fileY.IsDirectory)
                            returnVal = 1;
                        else if (fileX.IsDirectory && fileY.IsDirectory)
                            returnVal = string.Compare(fileX.Name, fileY.Name, StringComparison.OrdinalIgnoreCase);
                        else
                            returnVal = fileX.Size.CompareTo(fileY.Size);
                        break;

                    case 2: // Modified date column
                        returnVal = fileX.Modified.CompareTo(fileY.Modified);
                        break;
                }

                // Determine whether the sort order is descending
                if (order == SortOrder.Descending)
                    returnVal = -returnVal;

                return returnVal;
            }
        }


        // event handlers (wire them up in InitializeComponent or Designer)
        private void listViewLeft_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the sorted column
            if (e.Column == leftSortColumn)
            {
                // Reverse the current sort direction for this column
                if (listViewLeft.Sorting == SortOrder.Ascending)
                    listViewLeft.Sorting = SortOrder.Descending;
                else
                    listViewLeft.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending
                leftSortColumn = e.Column;
                listViewLeft.Sorting = SortOrder.Ascending;
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer object
            listViewLeft.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewLeft.Sorting);

            // Call the sort method to manually sort
            listViewLeft.Sort();
        }

        private void listViewRight_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the sorted column
            if (e.Column == rightSortColumn)
            {
                // Reverse the current sort direction for this column
                if (listViewRight.Sorting == SortOrder.Ascending)
                    listViewRight.Sorting = SortOrder.Descending;
                else
                    listViewRight.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending
                rightSortColumn = e.Column;
                listViewRight.Sorting = SortOrder.Ascending;
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer object
            listViewRight.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewRight.Sorting);

            // Call the sort method to manually sort
            listViewRight.Sort();
        }






        private async void cboLeftSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLeftSite.SelectedItem == null)
                return;

            string leftSite = cboLeftSite.SelectedItem.ToString();
            string rightSite = cboRightSite.SelectedItem?.ToString();

            // Prevent selecting the same site on both sides
            if (!string.IsNullOrEmpty(rightSite) && leftSite == rightSite)
            {
                MessageBox.Show($"Cannot select the same site ({leftSite}) on both sides.\n\nPlease choose a different site.",
                    "Same Site Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Reset to previous selection or first different site
                cboLeftSite.SelectedIndexChanged -= cboLeftSite_SelectedIndexChanged;
                cboLeftSite.SelectedIndex = -1;
                cboLeftSite.SelectedIndexChanged += cboLeftSite_SelectedIndexChanged;

                listViewLeft.Items.Clear();
                UpdateStatus("Select a different site for left side.");
                return;
            }

            // Auto-browse when site is selected
            currentLeftPath = "/";
            txtLeftPath.Text = currentLeftPath;
            await BrowseLeftSiteAsync(currentLeftPath);
        }

        private async void cboRightSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboRightSite.SelectedItem == null)
                return;

            string rightSite = cboRightSite.SelectedItem.ToString();
            string leftSite = cboLeftSite.SelectedItem?.ToString();

            // Prevent selecting the same site on both sides
            if (!string.IsNullOrEmpty(leftSite) && rightSite == leftSite)
            {
                MessageBox.Show($"Cannot select the same site ({rightSite}) on both sides.\n\nPlease choose a different site.",
                    "Same Site Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Reset to previous selection or first different site
                cboRightSite.SelectedIndexChanged -= cboRightSite_SelectedIndexChanged;
                cboRightSite.SelectedIndex = -1;
                cboRightSite.SelectedIndexChanged += cboRightSite_SelectedIndexChanged;

                listViewRight.Items.Clear();
                UpdateStatus("Select a different site for right side.");
                return;
            }

            // Auto-browse when site is selected
            currentRightPath = "/";
            txtRightPath.Text = currentRightPath;
            await BrowseRightSiteAsync(currentRightPath);
        }

        private async void btnLeftRefresh_Click(object sender, EventArgs e)
        {
            await BrowseLeftSiteAsync(currentLeftPath);
        }

        private async void btnRightRefresh_Click(object sender, EventArgs e)
        {
            await BrowseRightSiteAsync(currentRightPath);
        }

        private async void btnLeftUp_Click(object sender, EventArgs e)
        {
            var parentPath = GetParentPath(currentLeftPath);
            if (!string.Equals(parentPath, currentLeftPath, StringComparison.Ordinal))
                await BrowseLeftSiteAsync(parentPath);
        }

        private async void btnRightUp_Click(object sender, EventArgs e)
        {
            var parentPath = GetParentPath(currentRightPath);
            if (!string.Equals(parentPath, currentRightPath, StringComparison.Ordinal))
                await BrowseRightSiteAsync(parentPath);
        }

        private async void txtLeftPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                var requestedPath = NormalizePath(txtLeftPath.Text);
                if (!await BrowseLeftSiteAsync(requestedPath))
                    txtLeftPath.Text = currentLeftPath;
            }
        }

        private async void txtRightPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                var requestedPath = NormalizePath(txtRightPath.Text);
                if (!await BrowseRightSiteAsync(requestedPath))
                    txtRightPath.Text = currentRightPath;
            }
        }

        private async void listViewLeft_DoubleClick(object sender, EventArgs e)
        {
            if (listViewLeft.SelectedItems.Count > 0)
            {
                var item = listViewLeft.SelectedItems[0];
                var fileItem = item.Tag as FileItem;

                if (fileItem != null && fileItem.IsDirectory)
                {
                    if (fileItem.Name == "..")
                    {
                        btnLeftUp_Click(sender, e);
                    }
                    else
                    {
                        var nextPath = ResolveItemPath(currentLeftPath, fileItem);

                        if (fileItem.IsSymlink && !string.IsNullOrEmpty(fileItem.LinkTarget))
                            LogManager.Info($"Following symlink: {fileItem.Name} -> {fileItem.LinkTarget} (resolved to: {nextPath})");

                        await BrowseLeftSiteAsync(nextPath);
                    }
                }
            }
        }

        private async void listViewRight_DoubleClick(object sender, EventArgs e)
        {
            if (listViewRight.SelectedItems.Count > 0)
            {
                var item = listViewRight.SelectedItems[0];
                var fileItem = item.Tag as FileItem;

                if (fileItem != null && fileItem.IsDirectory)
                {
                    if (fileItem.Name == "..")
                    {
                        btnRightUp_Click(sender, e);
                    }
                    else
                    {
                        var nextPath = ResolveItemPath(currentRightPath, fileItem);

                        if (fileItem.IsSymlink && !string.IsNullOrEmpty(fileItem.LinkTarget))
                            LogManager.Info($"Following symlink: {fileItem.Name} -> {fileItem.LinkTarget} (resolved to: {nextPath})");

                        await BrowseRightSiteAsync(nextPath);
                    }
                }
            }
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "/";

            path = path.Trim().Replace('\\', '/');
            if (string.IsNullOrEmpty(path))
                return "/";

            var isAbsolute = path.StartsWith("/");
            var segments = new List<string>();

            foreach (var segment in path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (segment == ".")
                    continue;

                if (segment == "..")
                {
                    if (segments.Count > 0)
                        segments.RemoveAt(segments.Count - 1);
                    continue;
                }

                segments.Add(segment);
            }

            var normalized = "/" + string.Join("/", segments);
            if (!isAbsolute && segments.Count == 0)
                return "/";

            return normalized.Length == 0 ? "/" : normalized;
        }

        private string ResolveRemotePath(string basePath, string childPath)
        {
            if (string.IsNullOrWhiteSpace(childPath))
                return NormalizePath(basePath);

            childPath = childPath.Trim().Replace('\\', '/');
            if (childPath.StartsWith("/"))
                return NormalizePath(childPath);

            return NormalizePath($"{NormalizePath(basePath).TrimEnd('/')}/{childPath}");
        }

        private string ResolveItemPath(string currentPath, FileItem fileItem)
        {
            if (fileItem == null)
                return NormalizePath(currentPath);

            if (fileItem.IsSymlink && !string.IsNullOrWhiteSpace(fileItem.LinkTarget))
                return ResolveRemotePath(currentPath, fileItem.LinkTarget);

            if (!string.IsNullOrWhiteSpace(fileItem.FullPath))
                return NormalizePath(fileItem.FullPath);

            return ResolveRemotePath(currentPath, fileItem.Name);
        }

        private string GetParentPath(string path)
        {
            path = NormalizePath(path);
            if (path == "/")
                return "/";

            var trimmed = path.TrimEnd('/');
            var lastSlash = trimmed.LastIndexOf('/');
            if (lastSlash <= 0)
                return "/";

            return NormalizePath(trimmed.Substring(0, lastSlash));
        }

        private string GetLeafName(string path)
        {
            path = NormalizePath(path).TrimEnd('/');
            if (path == "/")
                return string.Empty;

            var lastSlash = path.LastIndexOf('/');
            return lastSlash >= 0 ? path.Substring(lastSlash + 1) : path;
        }

        private string NormalizeEntryFullPath(string currentPath, string candidatePath)
        {
            if (string.IsNullOrWhiteSpace(candidatePath))
                return null;

            candidatePath = candidatePath.Trim().Replace('\\', '/');
            if (!candidatePath.Contains("/"))
                return null;

            if (candidatePath.StartsWith("/"))
                return NormalizePath(candidatePath);

            var currentFirstSegment = NormalizePath(currentPath)
                .Trim('/')
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();
            var candidateFirstSegment = candidatePath
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(currentFirstSegment) &&
                string.Equals(currentFirstSegment, candidateFirstSegment, StringComparison.OrdinalIgnoreCase))
            {
                return NormalizePath(candidatePath);
            }

            return ResolveRemotePath(currentPath, candidatePath);
        }

        private async Task<bool> BrowseLeftSiteAsync(string path)
        {
            if (cboLeftSite.SelectedItem == null) return false;

            path = NormalizePath(path);

            try
            {
                UpdateStatus($"Browsing {cboLeftSite.SelectedItem} - {path}...");
                LogToConsole($"Browsing left: {cboLeftSite.SelectedItem} [{path}]");
                ShowProgress(true);

                var files = await GetDirectoryListingAsync(cboLeftSite.SelectedItem.ToString(), path);
                PopulateListView(listViewLeft, files, path != "/");
                currentLeftPath = path;
                txtLeftPath.Text = currentLeftPath;

                // Calculate total size (files only, not directories)
                long totalSize = files.Where(f => !f.IsDirectory).Sum(f => f.Size);
                int fileCount = files.Count(f => !f.IsDirectory);
                int dirCount = files.Count(f => f.IsDirectory);

                // Show size info in status bar (only if not in root)
                if (path != "/" && !path.Contains("/0DAY") && !path.Contains("/PRE"))
                {
                    UpdateStatus($"Left: {dirCount} dir(s), {fileCount} file(s), {FormatFileSize(totalSize)} (current folder only)");
                    LogToConsole($"Left: {dirCount} dir(s), {fileCount} file(s), {FormatFileSize(totalSize)}", Color.LimeGreen);
                }
                else
                {
                    UpdateStatus($"Left: Loaded {files.Count} items from {cboLeftSite.SelectedItem}");
                    LogToConsole($"Left: Loaded {files.Count} items", Color.LimeGreen);
                }

                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error browsing left site: {ex.Message}");
                LogToConsole($"Error browsing left: {ex.Message}", Color.Red);
                txtLeftPath.Text = currentLeftPath;
                MessageBox.Show($"Failed to browse directory:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private async Task<bool> BrowseRightSiteAsync(string path)
        {
            if (cboRightSite.SelectedItem == null) return false;

            path = NormalizePath(path);

            try
            {
                UpdateStatus($"Browsing {cboRightSite.SelectedItem} - {path}...");
                LogToConsole($"Browsing right: {cboRightSite.SelectedItem} [{path}]");
                ShowProgress(true);

                var files = await GetDirectoryListingAsync(cboRightSite.SelectedItem.ToString(), path);
                PopulateListView(listViewRight, files, path != "/");
                currentRightPath = path;
                txtRightPath.Text = currentRightPath;

                // Calculate total size (files only, not directories)
                long totalSize = files.Where(f => !f.IsDirectory).Sum(f => f.Size);
                int fileCount = files.Count(f => !f.IsDirectory);
                int dirCount = files.Count(f => f.IsDirectory);

                // Show size info in status bar (only if not in root)
                if (path != "/" && !path.Contains("/0DAY") && !path.Contains("/PRE"))
                {
                    UpdateStatus($"Right: {dirCount} dir(s), {fileCount} file(s), {FormatFileSize(totalSize)} (current folder only)");
                    LogToConsole($"Right: {dirCount} dir(s), {fileCount} file(s), {FormatFileSize(totalSize)}", Color.LimeGreen);
                }
                else
                {
                    UpdateStatus($"Right: Loaded {files.Count} items from {cboRightSite.SelectedItem}");
                    LogToConsole($"Right: Loaded {files.Count} items", Color.LimeGreen);
                }

                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error browsing right site: {ex.Message}");
                LogToConsole($"Error browsing right: {ex.Message}", Color.Red);
                txtRightPath.Text = currentRightPath;
                MessageBox.Show($"Failed to browse directory:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                ShowProgress(false);
            }
        }
        private async Task<List<FileItem>> GetDirectoryListingAsync(string siteName, string path)
        {
            try
            {
                path = NormalizePath(path);

                var config = GetCbftpConfig();
                if (config == null)
                    throw new Exception("No CBFTP configuration available");

                string endpoint = BuildEndpoint(config.Host, config.Port);

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };

                // Basic auth: username ignored, password required => ":password"
                var byteArray = Encoding.UTF8.GetBytes(":" + config.Password); // UTF8: ASCII mangles non-ASCII passwords to "?"
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"{endpoint}/path?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(path)}&timeout=60";
                LogToConsole($"Requesting: {url}", Color.Gray);

                var response = await client.GetAsync(url);
                var responseText = await response.Content.ReadAsStringAsync();

                LogToConsole($"Response: {response.StatusCode}", Color.Gray);
                if (MainApp.DebugEnabled)
                    LogToConsole($"Response body: {responseText}", Color.DarkGray);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}\n{responseText}");

                // Parse JSON deterministically (avoid dynamic binder surprises)
                JToken root;
                try
                {
                    root = JToken.Parse(responseText);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to parse CBFTP JSON response: {ex.Message}\n{responseText}");
                }

                // CBFTP /path can return:
                // 1) Top-level array: [ {...}, {...} ]
                // 2) Object with entries/files/items/list arrays
                JArray entriesArray = ExtractEntriesArray(root);

                if (entriesArray == null)
                {
                    LogToConsole($"No entries array found in response. Root type: {root.Type}", Color.Yellow);
                    return new List<FileItem>();
                }

                LogToConsole($"Processing {entriesArray.Count} entries", Color.Gray);

                var files = new List<FileItem>(capacity: entriesArray.Count);

                foreach (var token in entriesArray)
                {
                    if (token.Type != JTokenType.Object)
                        continue;

                    var entry = (JObject)token;

                    string name = entry.Value<string>("name");
                    string fullPath =
                        entry.Value<string>("path") ??
                        entry.Value<string>("full_path") ??
                        entry.Value<string>("fullPath") ??
                        entry.Value<string>("remote_path") ??
                        entry.Value<string>("remotePath");

                    if (string.IsNullOrWhiteSpace(fullPath) && !string.IsNullOrWhiteSpace(name))
                        fullPath = NormalizeEntryFullPath(path, name);
                    else
                        fullPath = NormalizeEntryFullPath(path, fullPath);

                    if (!string.IsNullOrWhiteSpace(name) && name.Contains("/"))
                        name = GetLeafName(name);

                    if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(fullPath))
                        name = GetLeafName(fullPath);

                    if (string.IsNullOrWhiteSpace(name) || name == "." || name == "..")
                        continue;

                    // --- TYPE / DIR / LINK detection ---
                    // Your CBFTP returns: type = "DIR", "FILE", "LINK" (seen in your sample)
                    string typeRaw = entry.Value<string>("type");
                    string type = (typeRaw ?? "").Trim().ToUpperInvariant();

                    bool isLink = type == "LINK";
                    bool isDir = type == "DIR";

                    // Fallbacks if type is missing or different
                    // Some installations might return "directory"/"file" etc.
                    if (!isDir && !isLink && !string.IsNullOrEmpty(type))
                    {
                        if (type == "DIRECTORY" || type == "DIR" || type == "D")
                            isDir = true;
                    }

                    // Link target field in your JSON is: "link_target"
                    string linkTarget =
                        entry.Value<string>("link_target") ??
                        entry.Value<string>("linkTarget") ??
                        entry.Value<string>("target") ??
                        entry.Value<string>("symlink") ??
                        entry.Value<string>("link");

                    // If API says LINK but target is missing, still treat it as link
                    if (isLink && string.IsNullOrWhiteSpace(linkTarget))
                        linkTarget = null;

                    // If we have a link target but type didn't say LINK, still treat as link
                    if (!isLink && !string.IsNullOrWhiteSpace(linkTarget))
                        isLink = true;

                    // In your UI you want to be able to navigate into links like directories.
                    if (isLink && !string.IsNullOrWhiteSpace(linkTarget))
                        isDir = true;

                    // --- Size ---
                    long size = entry.Value<long?>("size") ?? 0;

                    // --- Modified date ---
                    // Your response uses: "last_modified": "2026-01-13 17:03"
                    // Your old code only looked at "modified"/"timestamp"/"time"/"date".
                    DateTime modified = DateTime.MinValue;

                    string lastModifiedStr = entry.Value<string>("last_modified");
                    string modifiedStr = entry.Value<string>("modified");
                    string timeStr = entry.Value<string>("time");
                    string dateStr = entry.Value<string>("date");
                    long? timestamp = entry.Value<long?>("timestamp");

                    if (MainApp.DebugEnabled)
                    {
                        LogToConsole(
                            $"Date fields for {name}: last_modified={lastModifiedStr}, modified={modifiedStr}, timestamp={timestamp}, time={timeStr}, date={dateStr}",
                            Color.Gray);
                    }

                    // Prefer last_modified, then modified, then timestamp
                    if (!TryParseCbftpDate(lastModifiedStr, out modified) &&
                        !TryParseCbftpDate(modifiedStr, out modified))
                    {
                        if (timestamp.HasValue && timestamp.Value > 0)
                        {
                            try
                            {
                                modified = DateTimeOffset.FromUnixTimeSeconds(timestamp.Value).LocalDateTime;
                            }
                            catch
                            {
                                modified = DateTime.MinValue;
                            }
                        }
                        else if (!TryParseCbftpDate(timeStr, out modified))
                        {
                            TryParseCbftpDate(dateStr, out modified);
                        }
                    }

                    files.Add(new FileItem
                    {
                        Name = name,
                        FullPath = fullPath,
                        IsDirectory = isDir,
                        IsSymlink = isLink,
                        LinkTarget = linkTarget,
                        Size = size,
                        Modified = modified
                    });
                }

                LogToConsole($"✓ Parsed {files.Count} items", Color.LimeGreen);
                return files.OrderBy(f => !f.IsDirectory).ThenBy(f => f.Name).ToList();
            }
            catch (Exception ex)
            {
                LogToConsole($"✗ Failed to get directory listing for {siteName} {path}: {ex.Message}", Color.Red);
                throw;
            }
        }

        private static JArray ExtractEntriesArray(JToken root)
        {
            if (root == null)
                return null;

            if (root.Type == JTokenType.Array)
                return (JArray)root;

            if (root.Type == JTokenType.Object)
            {
                var obj = (JObject)root;

                // common field names
                var candidates = new[] { "entries", "files", "items", "list" };
                foreach (var key in candidates)
                {
                    var t = obj[key];
                    if (t != null && t.Type == JTokenType.Array)
                        return (JArray)t;
                }

                // If the object itself isn't shaped as expected, return null.
                return null;
            }

            return null;
        }

        private static bool TryParseCbftpDate(string s, out DateTime dt)
        {
            dt = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(s))
                return false;

            s = s.Trim();

            // CBFTP example: "2026-01-13 17:03"
            // Accept a few common variants safely, in an invariant way.
            string[] formats =
            {
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd",
        "yyyy/MM/dd HH:mm",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy/MM/dd"
    };

            if (DateTime.TryParseExact(
                    s,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out dt))
            {
                return true;
            }

            // Fallback: permissive parse (still useful if server changes format)
            if (DateTime.TryParse(
                    s,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out dt))
            {
                return true;
            }

            return false;
        }


        private void PopulateListView(ListView listView, List<FileItem> files, bool showUpDir)
        {
            listView.BeginUpdate();
            listView.Items.Clear();

            // Add ".." for going up
            if (showUpDir)
            {
                var upItem = new ListViewItem("..");
                upItem.SubItems.Add("");
                upItem.SubItems.Add("");
                upItem.ForeColor = Color.FromArgb(0, 168, 255); // Blue for directories
                upItem.Tag = new FileItem { Name = "..", IsDirectory = true };
                listView.Items.Add(upItem);
            }

            foreach (var file in files)
            {
                var displayName = file.Name;

                // Show symlink target in the name
                if (file.IsSymlink && !string.IsNullOrEmpty(file.LinkTarget))
                {
                    displayName = $"{file.Name} → {file.LinkTarget}";
                }

                var item = new ListViewItem(displayName);
                item.SubItems.Add(file.IsDirectory ? "<DIR>" : FormatFileSize(file.Size));

                // Show date only if it's valid (not MinValue), otherwise show empty
                string dateStr = "";
                if (file.Modified != DateTime.MinValue)
                {
                    dateStr = file.Modified.ToString("yyyy-MM-dd HH:mm");
                }
                item.SubItems.Add(dateStr);

                // Color: Cyan for symlinks, Blue for directories, White for files
                if (file.IsSymlink)
                    item.ForeColor = Color.FromArgb(0, 255, 255); // Cyan for symlinks
                else if (file.IsDirectory)
                    item.ForeColor = Color.FromArgb(0, 168, 255); // Blue for directories
                else
                    item.ForeColor = Color.White; // White for files

                item.Tag = file;
                listView.Items.Add(item);
            }

            listView.EndUpdate();
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private async void fxpToRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await PerformFxpTransfer(true); // Left to Right
        }

        private async void fxpToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await PerformFxpTransfer(false); // Right to Left
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await DeleteSelectedItems(listViewLeft, cboLeftSite.SelectedItem?.ToString(), currentLeftPath);
        }

        private async void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            await DeleteSelectedItems(listViewRight, cboRightSite.SelectedItem?.ToString(), currentRightPath);
        }

        private async Task DeleteSelectedItems(ListView listView, string siteName, string currentPath)
        {
            if (listView.SelectedItems.Count == 0 || string.IsNullOrEmpty(siteName))
                return;

            var selectedItems = listView.SelectedItems.Cast<ListViewItem>()
                .Select(item => item.Tag as FileItem)
                .Where(f => f != null && f.Name != "..")
                .ToList();

            if (!selectedItems.Any())
                return;

            var itemNames = string.Join("\n  • ", selectedItems.Select(f => f.Name));
            bool hasDirectories = selectedItems.Any(f => f.IsDirectory);
            var result = MessageBox.Show(
                $"Delete {selectedItems.Count} item(s) from {siteName}?\n\n  • {itemNames}\n\n{(hasDirectories ? "Directories will be deleted recursively with all contents!\n" : "")}This cannot be undone!",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                UpdateStatus($"Deleting {selectedItems.Count} item(s) from {siteName}...");
                ShowProgress(true);

                int successCount = 0;
                int failCount = 0;

                var config = GetCbftpConfig();
                if (config == null)
                {
                    throw new Exception("No CBFTP configuration available");
                }

                string endpoint = BuildEndpoint(config.Host, config.Port);

                foreach (var item in selectedItems)
                {
                    try
                    {
                        // NEVER resolve symlinks when deleting — ResolveItemPath returns the
                        // link TARGET, so deleting a symlink would recursively wipe the real
                        // directory it points at. Delete the link entry itself instead.
                        string fullPath = item.IsSymlink
                            ? ResolveRemotePath(currentPath, item.Name)
                            : ResolveItemPath(currentPath, item);

                        // Always delete directories recursively (but never through a symlink)
                        if (item.IsDirectory && !item.IsSymlink)
                        {
                            LogToConsole($"Deleting directory (recursive): {item.Name}", Color.Yellow);
                            bool deleted = await DeleteDirectoryRecursive(siteName, fullPath, endpoint, config);
                            if (deleted)
                            {
                                successCount++;
                                LogToConsole($"✓ Deleted: {item.Name}", Color.LimeGreen);
                            }
                            else
                            {
                                failCount++;
                                LogToConsole($"✗ Failed to delete: {item.Name}", Color.Red);
                            }
                            continue;
                        }

                        // Regular delete for files - use DELETE /path API
                        LogToConsole($"Deleting: {item.Name}");

                        using var handler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                        };

                        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
                        var byteArray = Encoding.UTF8.GetBytes(":" + config.Password); // UTF8: ASCII mangles non-ASCII passwords to "?"
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                        // Use DELETE /path API endpoint
                        var url = $"{endpoint}/path?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(fullPath)}&type=OWN";

                        var response = await client.DeleteAsync(url);
                        var responseText = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            successCount++;
                            LogToConsole($"✓ Deleted: {item.Name}", Color.LimeGreen);
                        }
                        else
                        {
                            failCount++;
                            LogToConsole($"✗ Failed to delete: {item.Name} - {response.StatusCode}", Color.Red);
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        LogToConsole($"✗ Error deleting {item.Name}: {ex.Message}", Color.Red);
                    }
                }

                UpdateStatus($"Delete: {successCount} succeeded, {failCount} failed");

                // Add a small delay before refreshing to ensure server has processed deletes
                if (successCount > 0)
                {
                    LogToConsole($"Waiting before refresh...", Color.Gray);
                    await Task.Delay(1000); // Wait 1 second
                }

                // Refresh the view
                LogToConsole($"Refreshing {siteName}...", Color.Cyan);

                if (listView == listViewLeft)
                {
                    await BrowseLeftSiteAsync(currentPath);
                    LogToConsole($"✓ Left view refreshed", Color.LimeGreen);
                }
                else
                {
                    await BrowseRightSiteAsync(currentPath);
                    LogToConsole($"✓ Right view refreshed", Color.LimeGreen);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Delete error: {ex.Message}");
                LogToConsole($"✗ Delete error: {ex.Message}", Color.Red);
                MessageBox.Show($"Failed to delete items:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private async Task<bool> DeleteDirectoryRecursive(string siteName, string directoryPath, string endpoint, dynamic config)
        {
            try
            {
                // Use CBFTP's proper DELETE /path endpoint
                LogToConsole($"  Deleting: {directoryPath}", Color.Gray);

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
                var byteArray = Encoding.UTF8.GetBytes(":" + config.Password); // UTF8: ASCII mangles non-ASCII passwords to "?"
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Use DELETE /path endpoint with type=OWN to delete only our files
                var url = $"{endpoint}/path?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(directoryPath)}&type=OWN";

                LogToConsole($"    DELETE {url}", Color.DarkGray);

                var response = await client.DeleteAsync(url);
                var responseText = await response.Content.ReadAsStringAsync();

                LogToConsole($"    Response: {response.StatusCode} - {responseText}", Color.DarkGray);

                if (response.IsSuccessStatusCode)
                {
                    LogToConsole($"  ✓ Deleted", Color.LimeGreen);
                    return true;
                }
                else
                {
                    LogToConsole($"  ✗ Delete failed: {response.StatusCode} - {responseText}", Color.Red);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogToConsole($"✗ Error deleting directory: {ex.Message}", Color.Red);
                return false;
            }
        }



        private async Task PerformFxpTransfer(bool leftToRight)
        {
            try
            {
                string sourceSite, destSite, sourcePath, destPath;
                ListView sourceListView;

                if (leftToRight)
                {
                    if (cboLeftSite.SelectedItem == null || cboRightSite.SelectedItem == null)
                    {
                        MessageBox.Show("Please select both source and destination sites.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    sourceSite = cboLeftSite.SelectedItem.ToString();
                    destSite = cboRightSite.SelectedItem.ToString();
                    sourcePath = currentLeftPath;
                    destPath = currentRightPath;
                    sourceListView = listViewLeft;
                }
                else
                {
                    if (cboRightSite.SelectedItem == null || cboLeftSite.SelectedItem == null)
                    {
                        MessageBox.Show("Please select both source and destination sites.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    sourceSite = cboRightSite.SelectedItem.ToString();
                    destSite = cboLeftSite.SelectedItem.ToString();
                    sourcePath = currentRightPath;
                    destPath = currentLeftPath;
                    sourceListView = listViewRight;
                }

                if (sourceListView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select files or folders to transfer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedItems = new List<FileItem>();
                foreach (ListViewItem item in sourceListView.SelectedItems)
                {
                    var fileItem = item.Tag as FileItem;
                    if (fileItem != null && fileItem.Name != "..") // Don't transfer the up directory
                    {
                        selectedItems.Add(fileItem);
                    }
                }

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("No valid items selected for transfer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // REMOVED: Confirmation dialog - just start the transfer immediately

                var direction = leftToRight ? "→" : "←";

                LogToTransfer($"=== FXP Transfer Started ===", Color.Cyan);
                LogToTransfer($"Source: {sourceSite} [{sourcePath}]");
                LogToTransfer($"Destination: {destSite} [{destPath}]");
                LogToTransfer($"Items: {selectedItems.Count}");

                UpdateStatus($"Starting FXP transfer: {sourceSite} {direction} {destSite}...");
                ShowProgress(true);

                int successCount = 0;
                int failCount = 0;

                foreach (var fileItem in selectedItems)
                {
                    string sourceItemPath = ResolveItemPath(sourcePath, fileItem);
                    string releaseName = GetLeafName(sourceItemPath);
                    string srcParentPath = GetParentPath(sourceItemPath);
                    string dstParentPath = NormalizePath(destPath);

                    if (string.IsNullOrWhiteSpace(releaseName))
                        releaseName = fileItem.Name;

                    if (fileItem.IsSymlink && !string.IsNullOrEmpty(fileItem.LinkTarget))
                    {
                        LogToTransfer($"Symlink: {fileItem.Name} → {sourceItemPath}", Color.Yellow);
                        LogToConsole($"Symlink FXP: {fileItem.Name} → resolved to {sourceItemPath}", Color.Gray);
                        LogToConsole($"  src_path: {srcParentPath}, name: {releaseName}", Color.Gray);
                    }

                    UpdateStatus($"Queuing FXP: {releaseName} ({successCount + failCount + 1}/{selectedItems.Count})");
                    LogToTransfer($"Queuing: {releaseName}");

                    LogToConsole($"FXP Transfer: {sourceSite} [{srcParentPath}] → {destSite} [{dstParentPath}] :: {releaseName}", Color.Cyan);

                    // Use CbftpRacer.StartTransferJobFxp
                    // srcIsSection = false (we're using paths, not section names)
                    var transferResult = await CbftpRacer.StartTransferJobFxp(
                        srcSite: sourceSite,
                        srcSectionOrPath: srcParentPath,
                        srcIsSection: false,
                        dstSite: destSite,
                        dstPath: dstParentPath,
                        releaseName: releaseName
                    );

                    if (transferResult.Success)
                    {
                        successCount++;
                        LogToTransfer($"✓ Queued: {releaseName}", Color.LimeGreen);
                        LogToConsole($"✓ FXP queued: {releaseName}", Color.LimeGreen);

                        // Monitor progress in background
                        _ = MonitorTransferProgressAsync(releaseName, sourceSite, destSite);
                    }
                    else
                    {
                        failCount++;
                        LogToTransfer($"✗ Failed: {releaseName} - {transferResult.ErrorMessage}", Color.Red);
                        LogToConsole($"✗ FXP failed to queue: {releaseName} - {transferResult.ErrorMessage}", Color.Red);
                    }
                }

                UpdateStatus($"FXP: {successCount} queued, {failCount} failed");
                LogToTransfer($"=== Transfer Complete: {successCount} queued, {failCount} failed ===", Color.Cyan);

                // For single transfer, wait for completion before refreshing
                if (successCount == 1)
                {
                    var firstItem = selectedItems.First();
                    LogToConsole($"Waiting for transfer to complete: {firstItem.Name}", Color.Gray);

                    // Wait for transfer to finish (with timeout)
                    bool completed = false;
                    for (int i = 0; i < 60 && !completed; i++) // Max 5 minutes
                    {
                        await Task.Delay(5000); // Check every 5 seconds

                        var stats = await CbftpRacer.GetTransferJobStats(firstItem.Name);
                        if (stats != null)
                        {
                            string status = stats.Status?.ToUpperInvariant() ?? "UNKNOWN";

                            if (status == "DONE")
                            {
                                LogToConsole($"Transfer completed successfully", Color.LimeGreen);
                                completed = true;
                            }
                            else if (status == "FAILED" || status == "TIMEOUT")
                            {
                                LogToConsole($"Transfer failed: {status}", Color.Red);
                                completed = true;
                            }
                            // else still RUNNING, keep waiting
                        }
                        else
                        {
                            // Can't get status, assume it finished
                            LogToConsole($"Cannot check transfer status, assuming complete", Color.Yellow);
                            completed = true;
                        }
                    }

                    if (!completed)
                    {
                        LogToConsole($"Transfer still running after timeout, refreshing anyway", Color.Yellow);
                    }
                }
                else if (successCount > 1)
                {
                    // Multiple transfers - just wait a bit
                    LogToConsole($"Multiple transfers queued, waiting 5 seconds...", Color.Gray);
                    await Task.Delay(5000);
                }

                // Refresh BOTH sides to show changes
                try
                {
                    LogToConsole($"Refreshing source: {sourceSite}", Color.Cyan);
                    if (leftToRight)
                    {
                        await BrowseLeftSiteAsync(sourcePath);
                    }
                    else
                    {
                        await BrowseRightSiteAsync(sourcePath);
                    }
                }
                catch (Exception refreshEx)
                {
                    LogToConsole($"Failed to refresh source: {refreshEx.Message}", Color.Yellow);
                }

                try
                {
                    LogToConsole($"Refreshing destination: {destSite}", Color.Cyan);
                    if (leftToRight)
                    {
                        await BrowseRightSiteAsync(destPath);
                    }
                    else
                    {
                        await BrowseLeftSiteAsync(destPath);
                    }
                }
                catch (Exception refreshEx)
                {
                    LogToConsole($"Failed to refresh destination: {refreshEx.Message}", Color.Yellow);
                }

                // Show notification
                if (successCount > 0)
                {
                    LogToConsole($"✓ FXP: {successCount} transfer(s) completed", Color.LimeGreen);
                }

                if (failCount > 0)
                {
                    LogToConsole($"✗ FXP: {failCount} transfer(s) failed to queue", Color.Red);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"FXP transfer error: {ex.Message}");
                LogToConsole($"✗ FXP transfer error: {ex.Message}", Color.Red);
                MessageBox.Show($"Failed to initiate FXP transfer:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private async Task MonitorTransferProgressAsync(string releaseName, string srcSite, string dstSite)
        {
            try
            {
                LogToTransfer($"Monitoring: {releaseName}", Color.Cyan);
                await Task.Delay(2000); // Wait a bit for transfer to start

                for (int i = 0; i < 60; i++) // Monitor for up to 5 minutes
                {
                    var stats = await CbftpRacer.GetTransferJobStats(releaseName);

                    if (stats == null)
                    {
                        await Task.Delay(5000);
                        continue;
                    }

                    string status = stats.Status?.ToUpperInvariant() ?? "UNKNOWN";

                    // Convert speed to MB/s if needed (check what unit CbftpRacer returns)
                    double speedMBps = stats.AverageSpeed; // Assuming it's already in MB/s

                    // If speed seems too small, it might be in KB/s
                    if (speedMBps > 0 && speedMBps < 1 && stats.BytesTransferred > 1024 * 1024)
                    {
                        speedMBps = stats.AverageSpeed / 1024.0; // Convert KB/s to MB/s
                    }

                    if (status == "DONE")
                    {
                        string speedStr = speedMBps > 0 ? $"{speedMBps:F1} MB/s" : "unknown speed";
                        UpdateStatus($"✓ Transfer complete: {releaseName} ({FormatFileSize(stats.BytesTransferred)}, {speedStr})");
                        LogToTransfer($"✓ Complete: {releaseName} - {FormatFileSize(stats.BytesTransferred)} @ {speedStr}", Color.LimeGreen);
                        LogToConsole($"✓ FXP completed: {releaseName} - {FormatFileSize(stats.BytesTransferred)} in {stats.TimeElapsed:mm\\:ss} @ {speedStr}", Color.LimeGreen);
                        break;
                    }
                    else if (status == "FAILED" || status == "TIMEOUT")
                    {
                        UpdateStatus($"✗ Transfer failed: {releaseName}");
                        LogToTransfer($"✗ Failed: {releaseName} - {status}", Color.Red);
                        LogToConsole($"✗ FXP failed: {releaseName} - {status}", Color.Red);
                        break;
                    }
                    else if (status == "RUNNING")
                    {
                        string speedStr = speedMBps > 0 ? $"{speedMBps:F1} MB/s" : "calculating...";
                        UpdateStatus($"→ Transferring: {releaseName} ({FormatFileSize(stats.BytesTransferred)}, {speedStr})");

                        // Log progress to console every update
                        LogToTransfer($"→ Progress: {releaseName} - {FormatFileSize(stats.BytesTransferred)} @ {speedStr}", Color.Yellow);
                    }

                    await Task.Delay(5000); // Check every 5 seconds
                }
            }
            catch (Exception ex)
            {
                LogToConsole($"✗ Monitor error: {ex.Message}", Color.Red);
            }
        }

        private CbftpConfig GetCbftpConfig()
        {
            try
            {
                const string configPath = "cbftp/cbftp_config.json";

                if (!File.Exists(configPath))
                {
                    return null;
                }

                var jsonContent = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<CbftpRacer.MainConfig>(jsonContent);

                if (config?.CbftpServers == null || config.CbftpServers.Count == 0)
                {
                    return null;
                }

                // Use first server
                var server = config.CbftpServers[0];
                return new CbftpConfig
                {
                    Host = server.Host,
                    Port = server.Port,
                    Password = SecureConfig.Decrypt(server.Password)
                };
            }
            catch (Exception ex)
            {
                LogToConsole($"✗ Error loading CBFTP config: {ex.Message}", Color.Red);
                return null;
            }
        }

        private string BuildEndpoint(string host, string port)
        {
            if (host.Contains("://"))
                return host.EndsWith($":{port}") ? host : $"{host}:{port}";
            else
                return $"https://{host}:{port}";
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(message)));
                return;
            }

            toolStripStatusLabel.Text = message;
        }

        private void ShowProgress(bool show)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowProgress(show)));
                return;
            }

            toolStripProgressBar.Visible = show;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnLeftRefresh_Click(sender, e);
        }

        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnRightRefresh_Click(sender, e);
        }

        private void LogToConsole(string message, Color? color = null)
        {
            LogToRichTextBox(txtConsoleLog, message, color);
        }

        private void LogToTransfer(string message, Color? color = null)
        {
            LogToRichTextBox(txtTransferLog, message, color);
        }

        private void LogToRichTextBox(RichTextBox textBox, string message, Color? color = null)
        {
            if (textBox == null)
                return;

            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() => LogToRichTextBox(textBox, message, color)));
                return;
            }

            textBox.SelectionStart = textBox.TextLength;
            textBox.SelectionColor = color ?? Color.White;
            textBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            textBox.ScrollToCaret();

            // Keep console from growing too large (keep last 500 lines)
            if (textBox.Lines.Length > 500)
            {
                var lines = textBox.Lines.Skip(100).ToArray();
                textBox.Lines = lines;
            }
        }

        private async void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ViewSelectedFile(listViewLeft, cboLeftSite.SelectedItem?.ToString(), currentLeftPath);
        }

        private async void viewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            await ViewSelectedFile(listViewRight, cboRightSite.SelectedItem?.ToString(), currentRightPath);
        }

        private async Task ViewSelectedFile(ListView listView, string siteName, string currentPath)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            var selectedItem = listView.SelectedItems[0];
            var fileItem = selectedItem.Tag as FileItem;

            if (fileItem == null || fileItem.IsDirectory)
            {
                LogToConsole("Please select a file to view", Color.Yellow);
                return;
            }

            string fileName = fileItem.Name;
            string fullPath = ResolveItemPath(currentPath, fileItem);
            string extension = Path.GetExtension(fileName).ToLower();

            // Text-based extensions
            string[] textExtensions = { ".nfo", ".sfv", ".txt", ".diz", ".log", ".m3u", ".md", ".xml", ".json", ".ini", ".cfg" };
            // Image extensions
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

            try
            {
                if (textExtensions.Contains(extension))
                {
                    await ViewTextFile(siteName, fullPath, fileName);
                }
                else if (imageExtensions.Contains(extension))
                {
                    await ViewImageFile(siteName, fullPath, fileName);
                }
                else
                {
                    LogToConsole($"Unsupported file type: {extension}", Color.Yellow);
                    MessageBox.Show($"File type '{extension}' is not supported for viewing.\n\nSupported types:\nText: {string.Join(", ", textExtensions)}\nImages: {string.Join(", ", imageExtensions)}",
                        "Unsupported File Type", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogToConsole($"Error viewing file: {ex.Message}", Color.Red);
                MessageBox.Show($"Failed to view file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ViewTextFile(string siteName, string filePath, string fileName)
        {
            LogToConsole($"Fetching file: {fileName}...", Color.Cyan);

            var config = GetCbftpConfig();
            if (config == null)
            {
                throw new Exception("No CBFTP configuration available");
            }

            string endpoint = BuildEndpoint(config.Host, config.Port);

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
            var byteArray = Encoding.UTF8.GetBytes(":" + config.Password); // UTF8: ASCII mangles non-ASCII passwords to "?"
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var url = $"{endpoint}/file?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(filePath)}&timeout=30";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch file: {response.StatusCode} - {errorContent}");
            }

            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            string content;
            string extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".nfo" || extension == ".diz")
            {
                var cp437 = Encoding.GetEncoding(437);
                content = cp437.GetString(contentBytes);
            }
            else
            {
                content = Encoding.UTF8.GetString(contentBytes);
            }

            var viewerForm = new Form
            {
                Text = $"View: {fileName}",
                Size = new Size(1000, 700),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.Black,
                ShowIcon = false
            };

            // Use custom panel with tight line spacing
            var panel = new NFOViewerPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Content = content
            };

            viewerForm.Controls.Add(panel);
            viewerForm.Show();

            LogToConsole($"✓ Opened: {fileName}", Color.LimeGreen);
        }

        
        private class NFOViewerPanel : Panel
        {
            public string Content { get; set; }
            private Font font = new Font("Cascadia Mono", 10f);
            //private Bitmap buffer;

            public NFOViewerPanel()
            {
                this.DoubleBuffered = true;
                this.AutoScroll = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (string.IsNullOrEmpty(Content))
                    return;

                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                // Measure single character for consistent spacing
                var charSize = TextRenderer.MeasureText(g, "M", font, Size.Empty, TextFormatFlags.NoPadding);

                // Tight line spacing: just the character height with minimal padding
                int lineHeight = charSize.Height;

                var lines = Content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                int y = 10;
                int x = 10;

                // Calculate total height for scrolling
                int totalHeight = (lines.Length * lineHeight) + 20;
                this.AutoScrollMinSize = new Size(800, totalHeight);

                // Get scroll position
                int scrollY = this.AutoScrollPosition.Y;

                foreach (var line in lines)
                {
                    // Only draw visible lines
                    int drawY = y + scrollY;
                    if (drawY > -lineHeight && drawY < this.Height)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            TextRenderer.DrawText(g, line, font, new Point(x, drawY),
                                Color.White, Color.Black, TextFormatFlags.NoPadding);
                        }
                    }
                    y += lineHeight;
                }
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                this.Invalidate();
            }
        }
        
        private async Task ViewImageFile(string siteName, string filePath, string fileName)
        {
            LogToConsole($"Downloading image: {fileName}...", Color.Cyan);

            var config = GetCbftpConfig();
            if (config == null)
            {
                throw new Exception("No CBFTP configuration available");
            }

            string endpoint = BuildEndpoint(config.Host, config.Port);

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
            var byteArray = Encoding.UTF8.GetBytes(":" + config.Password); // UTF8: ASCII mangles non-ASCII passwords to "?"
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Use CBFTP's /file endpoint to fetch file content
            var url = $"{endpoint}/file?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(filePath)}&timeout=30";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch image: {response.StatusCode} - {errorContent}");
            }

            // Get image bytes
            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            // Show in image viewer form
            var viewerForm = new Form
            {
                Text = $"View: {fileName}",
                Size = new Size(1200, 800),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(13, 16, 24),
                ShowIcon = false  // Remove icon
            };

            var pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(13, 16, 24)
            };

            // GDI+ requires the stream to stay alive for the Image's lifetime — disposing
            // it here caused "A generic error occurred in GDI+" on repaint. Clone into a
            // Bitmap instead so nothing else needs to be kept alive.
            using (var ms = new System.IO.MemoryStream(imageBytes))
            using (var img = Image.FromStream(ms))
            {
                pictureBox.Image = new Bitmap(img);
            }

            viewerForm.FormClosed += (s, args) =>
            {
                pictureBox.Image?.Dispose();
                pictureBox.Dispose();
            };

            viewerForm.Controls.Add(pictureBox);
            viewerForm.Show();

            LogToConsole($"✓ Opened: {fileName}", Color.LimeGreen);
        }


        private class FileItem
        {
            public string Name { get; set; }
            public string FullPath { get; set; }
            public bool IsDirectory { get; set; }
            public bool IsSymlink { get; set; }
            public string LinkTarget { get; set; }
            public long Size { get; set; }
            public DateTime Modified { get; set; }
        }

        private class SiteInfo
        {
            public string Name { get; set; }
            public dynamic Config { get; set; } // SiteConfig object from SiteConfigManager
        }

        private class CbftpConfig
        {
            public string Host { get; set; }
            public string Port { get; set; }
            public string Password { get; set; }
        }
    }
}
