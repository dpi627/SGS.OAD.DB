using System.Text.RegularExpressions;

namespace SGS.OAD.DB.Utilities
{
    public class Util
    {
        public static string ReplaceVariables(string template, params object[] parameters)
        {
            // 使用正则表达式匹配 {变量名称} 的模式
            string pattern = @"\{(\w+)\}";

            // 提取模板中的变量名列表，按出现顺序排列
            var variableNames = Regex.Matches(template, pattern)
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .ToList();

            // 检查参数数量是否匹配
            if (parameters.Length < variableNames.Count)
            {
                throw new ArgumentException("提供的参数数量少于模板中需要替换的变量数量。");
            }

            // 构建变量名与参数值的字典
            var variables = new Dictionary<string, object>();
            for (int i = 0; i < variableNames.Count; i++)
            {
                variables[variableNames[i]] = parameters[i];
            }

            // 使用字典进行变量替换
            return ReplaceVariables(template, variables);
        }
        public static string ReplaceVariables(string template, params (string Name, object Value)[] parameters)
        {
            var variables = parameters.ToDictionary(p => p.Name, p => p.Value);
            return ReplaceVariables(template, variables);
        }
        public static string ReplaceVariables(string template, Dictionary<string, object> variables)
        {
            // 使用正则表达式匹配 {变量名称} 的模式
            string pattern = @"\{(\w+)\}";

            // 使用 Regex.Replace 逐一替换 {变量}
            string result = Regex.Replace(template, pattern, match =>
            {
                string key = match.Groups[1].Value;
                if (variables.ContainsKey(key))
                {
                    string replacement = variables[key]?.ToString() ?? string.Empty;
                    return replacement;
                }
                else
                {
                    // 如果变量未提供，保留原始占位符
                    return match.Value;
                }
            });

            return result;
        }
    }
}
