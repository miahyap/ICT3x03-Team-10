using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace forumx_server.Helper
{
    public class EncryptedJsonConfigurationProvider : JsonConfigurationProvider
    {
        private byte[] _decKey;

        public EncryptedJsonConfigurationProvider(EncryptedJsonConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            base.Load(stream);

            if (_decKey == null)
            {
                var keyPath = Data["key:file"];
                _decKey = File.ReadAllBytes(keyPath);
                File.Delete(keyPath);
                Data.Remove("key:file");
            }

            var dataCopy = new Dictionary<string, string>(Data);

            foreach (var kv in dataCopy) Data[kv.Key] = CryptoConfigHelper.Decrypt(kv.Value, _decKey);
        }
    }

    public class EncryptedJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EncryptedJsonConfigurationProvider(this);
        }
    }

    public static class EncryptedJsonConfiguration
    {
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path,
            bool optional,
            bool reloadOnChange)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("File path must be a non-empty string.");

            var source = new EncryptedJsonConfigurationSource
            {
                FileProvider = null,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }
    }

    public static class CryptoConfigHelper
    {
        public static string Decrypt(string base64CipherText, byte[] key)
        {
            var convertedBytes = Convert.FromBase64String(base64CipherText);
            var iv = convertedBytes[..12];
            var tag = convertedBytes[12..28];
            var cipherText = convertedBytes[28..];
            var plainText = new byte[cipherText.Length];
            var aesGcm = new AesGcm(key);
            aesGcm.Decrypt(iv, cipherText, tag, plainText);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string Encrypt(string plainText, byte[] key)
        {
            var rngCsp = new RNGCryptoServiceProvider();
            var iv = new byte[12];
            rngCsp.GetBytes(iv);
            var tag = new byte[16];
            var ptBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherText = new byte[ptBytes.Length];
            new AesGcm(key).Encrypt(iv, ptBytes, cipherText, tag);
            var output = iv.Concat(tag).Concat(cipherText).ToArray();
            return Convert.ToBase64String(output);
        }
    }
}