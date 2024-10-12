using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace SGS.OAD.DB //namespace 要配合組態檔 config.xml
{
    public class ConfigHelper
    {
        private static readonly Lazy<Dictionary<string, string>> _config = new Lazy<Dictionary<string, string>>(() =>
        {
            return LoadConfig();
        });

        public static string GetValue(string key) => GetValue<string>(key);

        public static T GetValue<T>(string key)
        {
            if (_config.Value.TryGetValue(key, out var value))
            {
                try
                {
                    // 处理特殊类型，如枚举、GUID、TimeSpan
                    if (typeof(T).IsEnum)
                    {
                        return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
                    }
                    else if (typeof(T) == typeof(Guid))
                    {
                        return (T)(object)Guid.Parse(value);
                    }
                    else if (typeof(T) == typeof(TimeSpan))
                    {
                        return (T)(object)TimeSpan.Parse(value);
                    }
                    else if (typeof(T) == typeof(Uri))
                    {
                        return (T)(object)new Uri(value);
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(typeof(T));
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            return (T)converter.ConvertFromInvariantString(value);
                        }
                        else
                        {
                            return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Can't convert '{value}' to '{typeof(T).Name}'。", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Can't found '{key}'。");
            }
        }

        private static Dictionary<string, string> LoadConfig()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{typeof(ConfigHelper).Namespace}.config.xml"; // 确保命名空间和文件名匹配

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
                }

                var config = new Dictionary<string, string>();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);

                XmlNode root = xmlDoc.DocumentElement;
                foreach (XmlNode node in root.ChildNodes)
                {
                    config[node.Name] = node.InnerText;
                }

                return config;
            }
        }
    }
}
