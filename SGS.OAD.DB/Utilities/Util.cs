using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SGS.OAD.DB
{
    public class Util
    {
        /// <summary>
        /// Replace variables in the template with the values in the object[].
        /// </summary>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReplaceVariables(string template, params object[] parameters)
        {
            string pattern = @"\{(\w+)\}";

            // 取出樣板中所有 {varb}
            var variableNames = Regex.Matches(template, pattern)
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .ToList();

            // 檢查傳入參數是否足夠
            if (parameters.Length < variableNames.Count)
            {
                throw new ArgumentException("parameters are less than {variables}");
            }

            // convert parameters to dictionary
            var variables = new Dictionary<string, object>();
            for (int i = 0; i < variableNames.Count; i++)
            {
                variables[variableNames[i]] = parameters[i];
            }

            return ReplaceVariables(template, variables);
        }

        /// <summary>
        /// Replace variables in the template with the values in the tuple[].
        /// </summary>
        /// <param name="template"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ReplaceVariables(string template, params (string Name, object Value)[] parameters)
        {
            var variables = parameters.ToDictionary(p => p.Name, p => p.Value);
            return ReplaceVariables(template, variables);
        }

        /// <summary>
        /// Replace variables in the template with the values in the dictionary.
        /// </summary>
        /// <param name="template">string template</param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static string ReplaceVariables(string template, Dictionary<string, object> variables)
        {
            string pattern = @"\{(\w+)\}";

            // 替換 {varb}
            string result = Regex.Replace(template, pattern, match =>
            {
                string key = match.Groups[1].Value;

                if (variables.ContainsKey(key))
                {
                    string replacement = variables[key]?.ToString() ?? string.Empty;
                    return replacement;
                }

                return match.Value;
            });

            return result;
        }

        /// <summary>
        /// Deserialize JSON string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Serialize object to JSON string
        /// </summary>
        /// <typeparam name="T">物件的類型</typeparam>
        /// <param name="obj">要序列化的物件</param>
        /// <returns>JSON 字串</returns>
        public static string SerializeJson<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
