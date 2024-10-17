using System.Security.Cryptography;
using System.Text;

namespace SGS.OAD.DB
{
    public class DecryptService : IDecryptService
    {
        /// <summary>
        /// 解密使用者資料
        /// </summary>
        /// <param name="encryptedUserInfo">加密的使用者資料</param>
        /// <returns>解密的使用者資料</returns>
        public UserInfo DecryptUserInfo(UserInfo encryptedUserInfo)
        {
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
        private static string Decrypt(string encryptedText)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText))
                    return "";

                string result = "";
                string key = ConfigHelper.GetValue("DES_KEY");
                string vector = ConfigHelper.GetValue("DES_VECTOR");

                byte[] keys = Encoding.ASCII.GetBytes(key);
                byte[] iv = Encoding.ASCII.GetBytes(vector);

                if (keys.Length != 8)
                    throw new ArgumentException("Key length must be 8");
                if (iv.Length != 8)
                    throw new ArgumentException("Vector length must be 8");

                byte[] dataByteArray = Convert.FromBase64String(encryptedText);

                using (var des = DES.Create())
                {
                    des.Key = keys;
                    des.IV = iv;

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
