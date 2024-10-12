using SGS.OAD.DB.Models;
using SGS.OAD.DB.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SGS.OAD.DB.Services.Implements
{
    public class DecryptService : IDecryptService
    {
        /// <summary>
        /// 解密使用者資料
        /// </summary>
        /// <param name="encryptedUserInfo"></param>
        /// <returns></returns>
        public UserInfo DecryptUserInfo(UserInfo encryptedUserInfo)
        {
            // 实现解密逻辑
            // 例如，假设加密的 UserId 和 Password 是 Base64 编码的字符串
            var decryptedUserId = Decrypt(encryptedUserInfo.UserId);
            var decryptedPassword = Decrypt(encryptedUserInfo.Password);

            return new UserInfo
            {
                UserId = decryptedUserId,
                Password = decryptedPassword
            };
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="encryptedText">加密字串</param>
        /// <returns>解密字串</returns>
        private string Decrypt(string encryptedText)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText))
                    return "";

                string result = "";
                string key = "23928467";
                string vector = "22993279";

                byte[] keys = Encoding.ASCII.GetBytes(key);
                byte[] iv = Encoding.ASCII.GetBytes(vector);

                if (keys.Length != 8)
                    throw new ArgumentException("Key length must be 8");
                if (iv.Length != 8)
                    throw new ArgumentException("Vector length must be 8");

                byte[] dataByteArray = Convert.FromBase64String(encryptedText);

                // 创建 DES 对象
                using (var des = DES.Create())
                {
                    des.Key = keys;
                    des.IV = iv;

                    // 创建解密器
                    ICryptoTransform decryptor = des.CreateDecryptor();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(dataByteArray, 0, dataByteArray.Length);
                            cs.FlushFinalBlock();

                            result = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

    }
}
