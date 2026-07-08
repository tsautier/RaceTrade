using System;
using System.Windows.Forms;
using RaceTrade;

namespace RaceTrade
{
    /// <summary>
    /// One-time utility to encrypt all existing passwords and Blowfish keys.
    /// Run this ONCE to encrypt your existing configuration files.
    /// </summary>
    public partial class EncryptionTool : Form
    {
        public EncryptionTool()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Encryption Tool - Secure Your Configs";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var lblTitle = new Label
            {
                Text = "🔒 Encrypt All Configuration Files",
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(550, 30)
            };

            var lblInfo = new Label
            {
                Text = "This tool will encrypt all passwords and Blowfish keys in your configuration files.\n\n" +
                       "✅ Site configs (sites/*.json)\n" +
                       "✅ CBFTP configs (cbftp/cbftp_config.json)\n" +
                       "✅ PreBot configs (pre_bots/*.json)\n\n" +
                       "⚠️ IMPORTANT: Make sure you have a backup before proceeding!\n\n" +
                       "Encryption is per-user. Only YOU on THIS computer can decrypt.",
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(550, 180),
                Font = new System.Drawing.Font("Arial", 10)
            };

            var btnEncrypt = new Button
            {
                Text = "🔐 Encrypt All Configs",
                Location = new System.Drawing.Point(20, 260),
                Size = new System.Drawing.Size(250, 40),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.Green,
                ForeColor = System.Drawing.Color.White
            };
            btnEncrypt.Click += BtnEncrypt_Click;

            var btnTest = new Button
            {
                Text = "🧪 Test Encryption",
                Location = new System.Drawing.Point(290, 260),
                Size = new System.Drawing.Size(250, 40),
                Font = new System.Drawing.Font("Arial", 10)
            };
            btnTest.Click += BtnTest_Click;

            var txtLog = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(20, 310),
                Size = new System.Drawing.Size(550, 40),
                ReadOnly = true
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInfo);
            this.Controls.Add(btnEncrypt);
            this.Controls.Add(btnTest);
            this.Controls.Add(txtLog);
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ WARNING: This will encrypt ALL passwords and keys in your config files!\n\n" +
                "Have you made a backup?\n\n" +
                "Click YES to proceed with encryption.",
                "Confirm Encryption",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    Console.WriteLine("===================================");
                    Console.WriteLine("🔒 Starting Encryption Process");
                    Console.WriteLine("===================================\n");

                    // Encrypt site configs
                    Console.WriteLine("📁 Encrypting site configurations...");
                    SecureConfig.EncryptAllSiteConfigs();
                    Console.WriteLine();

                    // Encrypt main config
                    Console.WriteLine("📁 Encrypting CBFTP configuration...");
                    SecureConfig.EncryptMainConfig();
                    Console.WriteLine();

                    // Encrypt PreBot configs
                    Console.WriteLine("📁 Encrypting PreBot configurations...");
                    SecureConfig.EncryptPreBotConfigs();
                    Console.WriteLine();

                    Console.WriteLine("===================================");
                    Console.WriteLine("✅ Encryption Complete!");
                    Console.WriteLine("===================================");

                    MessageBox.Show(
                        "✅ Encryption completed successfully!\n\n" +
                        "All passwords and Blowfish keys are now encrypted.\n\n" +
                        "Check the console output for details.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"❌ Encryption failed:\n\n{ex.Message}\n\n" +
                        $"Restore your backup and try again.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                // Test encryption/decryption
                string testPassword = "MySecretPassword123!";
                Console.WriteLine($"Original: {testPassword}");

                string encrypted = SecureConfig.Encrypt(testPassword);
                Console.WriteLine($"Encrypted: {encrypted}");

                string decrypted = SecureConfig.Decrypt(encrypted);
                Console.WriteLine($"Decrypted: {decrypted}");

                if (decrypted == testPassword)
                {
                    MessageBox.Show(
                        "✅ Encryption test PASSED!\n\n" +
                        $"Original: {testPassword}\n" +
                        $"Encrypted: {encrypted}\n" +
                        $"Decrypted: {decrypted}\n\n" +
                        "Encryption is working correctly!",
                        "Test Passed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "❌ Encryption test FAILED!\n\n" +
                        "Decrypted value doesn't match original.",
                        "Test Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Test failed:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // To run this tool:
        // 1. In your Program.cs, temporarily change Application.Run(new MainApp()) to:
        //    Application.Run(new EncryptionTool())
        // 2. Run the app
        // 3. Encrypt your configs
        // 4. Change back to Application.Run(new MainApp())
    }
}