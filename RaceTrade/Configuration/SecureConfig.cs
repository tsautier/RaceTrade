using System;
using System.Security.Cryptography;
using System.Text;

namespace RaceTrade
{
    /// <summary>
    /// Provides encryption and decryption for sensitive data using Windows DPAPI.
    /// Data is encrypted per-user and cannot be decrypted by other users or on other machines.
    /// </summary>
    public static class SecureConfig
    {
        // Prefix to identify encrypted data
        private const string ENCRYPTED_PREFIX = "ENC:";

        // App-specific DPAPI entropy. Without it, ANY process running as the same Windows
        // user can Unprotect these secrets; with it, an attacker also needs this constant.
        // Legacy data was written with null entropy — Decrypt falls back to that so old
        // configs keep working (see Decrypt).
        private static readonly byte[] Entropy =
            Encoding.UTF8.GetBytes("RaceTrade.SecureConfig.v1.entropy");

        /// <summary>
        /// Encrypts a plaintext string using Windows DPAPI.
        /// </summary>
        /// <param name="plainText">The text to encrypt (e.g., password)</param>
        /// <returns>Base64-encoded encrypted string with "ENC:" prefix</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            try
            {
                // Convert string to bytes
                byte[] data = Encoding.UTF8.GetBytes(plainText);

                // Encrypt using DPAPI (CurrentUser scope) with app-specific entropy.
                byte[] encryptedData = ProtectedData.Protect(
                    data,
                    Entropy,
                    DataProtectionScope.CurrentUser // Only this user can decrypt
                );

                // Convert to Base64 and add prefix
                return ENCRYPTED_PREFIX + Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                LogManager.Error($"Encryption failed: {ex.Message}");
                throw new CryptographicException("Failed to encrypt data", ex);
            }
        }

        /// <summary>
        /// Decrypts an encrypted string using Windows DPAPI.
        /// If the string is not encrypted (no ENC: prefix), returns it as-is.
        /// </summary>
        /// <param name="encryptedText">The encrypted text (with "ENC:" prefix) or plaintext</param>
        /// <returns>Decrypted plaintext string</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return encryptedText;
            }

            // If not encrypted, return as-is (backward compatibility with plaintext
            // configs). Surface it at Debug so an unmigrated secret is at least traceable
            // without flooding the log on every key/password load.
            if (!encryptedText.StartsWith(ENCRYPTED_PREFIX))
            {
                LogManager.Debug("SecureConfig.Decrypt: value is not encrypted (plaintext passthrough).");
                return encryptedText;
            }

            // Remove prefix and decode from Base64. If the payload isn't valid Base64 it
            // was never our ciphertext (e.g. a genuine password that happens to start with
            // "ENC:") — return the original string as plaintext instead of throwing.
            string base64Data = encryptedText.Substring(ENCRYPTED_PREFIX.Length);
            byte[] encryptedData;
            try
            {
                encryptedData = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                LogManager.Debug("SecureConfig.Decrypt: ENC-prefixed value is not valid Base64; treating as plaintext.");
                return encryptedText;
            }

            // Try current (entropy) first, then legacy (null entropy) for data written
            // before entropy was introduced. Only a genuine failure of BOTH throws.
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    encryptedData, Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (CryptographicException)
            {
                try
                {
                    byte[] legacy = ProtectedData.Unprotect(
                        encryptedData, null, DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(legacy);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Decryption failed: {ex.Message}");
                    throw new CryptographicException(
                        "Failed to decrypt data. Data may be corrupted or encrypted by another user.", ex);
                }
            }
        }

        /// <summary>
        /// Checks if a string is encrypted (has the ENC: prefix).
        /// </summary>
        public static bool IsEncrypted(string text)
        {
            return !string.IsNullOrEmpty(text) && text.StartsWith(ENCRYPTED_PREFIX);
        }

        /// <summary>
        /// Encrypts a password only if it's not already encrypted.
        /// </summary>
        public static string EncryptIfNeeded(string text)
        {
            if (string.IsNullOrEmpty(text) || IsEncrypted(text))
            {
                return text;
            }

            return Encrypt(text);
        }

        /// <summary>
        /// Batch encrypts all sensitive fields in a configuration file.
        /// USE THIS to migrate existing configs to encrypted format.
        /// </summary>
        public static void EncryptConfigFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"Configuration file not found: {filePath}");
            }

            try
            {
                var jsonContent = System.IO.File.ReadAllText(filePath);
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonContent);

                bool modified = false;

                // Encrypt server passwords
                if (config.server?.password != null)
                {
                    string pwd = config.server.password.ToString();
                    if (!IsEncrypted(pwd))
                    {
                        config.server.password = Encrypt(pwd);
                        modified = true;
                        LogManager.Info($"Encrypted server password in {filePath}");
                    }
                }

                // Encrypt Blowfish keys
                if (config.site_settings != null)
                {
                    if (config.site_settings.blowfish_key1 != null)
                    {
                        string key = config.site_settings.blowfish_key1.ToString();
                        if (!string.IsNullOrEmpty(key) && !IsEncrypted(key))
                        {
                            config.site_settings.blowfish_key1 = Encrypt(key);
                            modified = true;
                            LogManager.Info($"Encrypted blowfish_key1 in {filePath}");
                        }
                    }

                    if (config.site_settings.blowfish_key2 != null)
                    {
                        string key = config.site_settings.blowfish_key2.ToString();
                        if (!string.IsNullOrEmpty(key) && !IsEncrypted(key))
                        {
                            config.site_settings.blowfish_key2 = Encrypt(key);
                            modified = true;
                            LogManager.Info($"Encrypted blowfish_key2 in {filePath}");
                        }
                    }

                    if (config.site_settings.blowfish_key3 != null)
                    {
                        string key = config.site_settings.blowfish_key3.ToString();
                        if (!string.IsNullOrEmpty(key) && !IsEncrypted(key))
                        {
                            config.site_settings.blowfish_key3 = Encrypt(key);
                            modified = true;
                            LogManager.Info($"Encrypted blowfish_key3 in {filePath}");
                        }
                    }
                }

                // Save if modified
                if (modified)
                {
                    var updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                    AtomicFile.WriteAllText(filePath, updatedJson);
                    LogManager.Success($"Encrypted sensitive data in {filePath}");
                }
                else
                {
                    LogManager.Info($"No encryption needed for {filePath} (already encrypted)");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to encrypt config file {filePath}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Encrypts all sensitive data in all site configuration files.
        /// </summary>
        public static void EncryptAllSiteConfigs()
        {
            string sitesDirectory = "sites";

            if (!System.IO.Directory.Exists(sitesDirectory))
            {
                LogManager.Warning($"Sites directory '{sitesDirectory}' not found.");
                return;
            }

            var jsonFiles = System.IO.Directory.GetFiles(sitesDirectory, "*.json");
            int encryptedCount = 0;

            foreach (var file in jsonFiles)
            {
                try
                {
                    EncryptConfigFile(file);
                    encryptedCount++;
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Failed to process {file}: {ex.Message}");
                }
            }

            LogManager.Success($"Processed {encryptedCount} of {jsonFiles.Length} config file(s).");
        }

        /// <summary>
        /// Encrypts passwords in main_config.json (CBFTP passwords).
        /// </summary>
        public static void EncryptMainConfig()
        {
            string mainConfigPath = "cbftp/cbftp_config.json";

            // Ensure directory exists
            string configDir = System.IO.Path.GetDirectoryName(mainConfigPath);
            if (!string.IsNullOrEmpty(configDir) && !System.IO.Directory.Exists(configDir))
            {
                LogManager.Warning($"Config directory \'{configDir}\' does not exist. Creating it...");
                System.IO.Directory.CreateDirectory(configDir);
            }

            if (!System.IO.File.Exists(mainConfigPath))
            {
                LogManager.Warning($"Main config file '{mainConfigPath}' not found.");
                return;
            }

            try
            {
                var jsonContent = System.IO.File.ReadAllText(mainConfigPath);
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonContent);

                bool modified = false;

                // Encrypt CBFTP server passwords
                if (config.cbftp_servers != null)
                {
                    foreach (var server in config.cbftp_servers)
                    {
                        if (server.password != null)
                        {
                            string pwd = server.password.ToString();
                            if (!IsEncrypted(pwd))
                            {
                                server.password = Encrypt(pwd);
                                modified = true;
                                LogManager.Info($"Encrypted CBFTP password for server {server.id}");
                            }
                        }
                    }
                }

                if (modified)
                {
                    var updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                    AtomicFile.WriteAllText(mainConfigPath, updatedJson);
                    LogManager.Success($"Encrypted passwords in {mainConfigPath}");
                }
                else
                {
                    LogManager.Info($"No encryption needed for {mainConfigPath}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to encrypt main config: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Encrypts passwords in PreBot configuration files.
        /// </summary>
        public static void EncryptPreBotConfigs()
        {
            string prebotsDirectory = "pre_bots";

            if (!System.IO.Directory.Exists(prebotsDirectory))
            {
                LogManager.Warning($"PreBots directory '{prebotsDirectory}' not found.");
                return;
            }

            var jsonFiles = System.IO.Directory.GetFiles(prebotsDirectory, "*.json");
            int encryptedCount = 0;

            foreach (var file in jsonFiles)
            {
                try
                {
                    var jsonContent = System.IO.File.ReadAllText(file);
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonContent);

                    bool modified = false;

                    // Encrypt ZNC password
                    if (config.ZncServer?.Password != null)
                    {
                        string pwd = config.ZncServer.Password.ToString();
                        if (!IsEncrypted(pwd))
                        {
                            config.ZncServer.Password = Encrypt(pwd);
                            modified = true;
                            LogManager.Info($"Encrypted ZNC password in {file}");
                        }
                    }

                    // Encrypt Blowfish keys in PreBot configs too
                    if (config.SiteSettings?.BlowfishKey1 != null)
                    {
                        string key = config.SiteSettings.BlowfishKey1.ToString();
                        if (!string.IsNullOrEmpty(key) && !IsEncrypted(key))
                        {
                            config.SiteSettings.BlowfishKey1 = Encrypt(key);
                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        var updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                        AtomicFile.WriteAllText(file, updatedJson);
                        encryptedCount++;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Failed to process {file}: {ex.Message}");
                }
            }

            LogManager.Success($"Processed {encryptedCount} PreBot config file(s).");
        }
    }
}