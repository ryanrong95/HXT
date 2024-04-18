using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestToolAbnormal.Enums;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    /// <summary>
    /// 表达式分析
    /// </summary>
    public class Statement
    {
        private string Text { get; set; } = string.Empty;

        private string ExceptionConfigPath
        {
            get { return ConfigurationManager.AppSettings["ExceptionConfigPath"]; }
        }

        private List<string> RestartCustomsConfig = new List<string>();
        private List<string> ResendMsgConfig = new List<string>();
        private List<string> RemindHimConfig = new List<string>();
        private List<string> OtherExceptionConfig = new List<string>();

        public Statement(string text)
        {
            this.Text = text;

            LoadExceptionConfig(RestartCustomsConfig, "RestartCustomsConfig");
            LoadExceptionConfig(ResendMsgConfig, "ResendMsgConfig");
            LoadExceptionConfig(RemindHimConfig, "RemindHimConfig");
            LoadExceptionConfig(OtherExceptionConfig, "OtherExceptionConfig");
        }

        /// <summary>
        /// 分析得出处理方式
        /// </summary>
        /// <returns></returns>
        public ExceptionHandlerEnum Analyse()
        {
            ExceptionHandlerEnum result = ExceptionHandlerEnum.UnTreated;

            if (Analyse(this.RestartCustomsConfig))
            {
                result = ExceptionHandlerEnum.RestartCustomsConfig;
            }
            else if (Analyse(this.ResendMsgConfig))
            {
                result = ExceptionHandlerEnum.ResendMsgConfig;
            }
            else if (Analyse(this.RemindHimConfig))
            {
                result = ExceptionHandlerEnum.RemindHimConfig;
            }
            else if (Analyse(this.OtherExceptionConfig))
            {
                result = ExceptionHandlerEnum.OtherExceptionConfig;
            }

            return result;
        }

        /// <summary>
        /// 分析得出该处理方式是否正确
        /// </summary>
        /// <returns></returns>
        private bool Analyse(List<string> listConfig)
        {
            if (listConfig == null && !listConfig.Any())
            {
                return false;
            }

            foreach (var config in listConfig)
            {
                if (Match(this.Text, config))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载异常配置
        /// </summary>
        /// <param name="listConfig"></param>
        /// <param name="configFileName"></param>
        public void LoadExceptionConfig(List<string> listConfig, string configFileName)
        {
            string fileFullName = ExceptionConfigPath + @"\" + configFileName + ".txt";

            if (!File.Exists(fileFullName))
            {
                return;
            }

            listConfig.Clear();

            string line = string.Empty;
            using (StreamReader reader = new StreamReader(fileFullName))
            {
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        line = line.Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            listConfig.Add(line);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断 text 文本和 rawexp 表达式是否匹配
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rawexp"></param>
        /// <returns></returns>
        private bool Match(string text, string rawexp)
        {
            string[] texts = { text };
            var lamdas = GenExpression(rawexp);

            var linq = (from t in texts.ToList()
                        select t);

            return linq.Any(lamdas.Compile());
        }

        /// <summary>
        /// 根据 rawexp 文本生成 lamdas
        /// </summary>
        /// <param name="rawexp"></param>
        /// <returns></returns>
        private Expression<Func<string, bool>> GenExpression(string rawexp)
        {
            string[] words = rawexp.Split();

            words = (from word in words
                     select word.Trim()).ToArray();

            words = words.Where(t => !string.IsNullOrEmpty(t)).ToArray();

            var lamdas = (Expression<Func<string, bool>>)(t => t.Contains(words[0]));

            for (int i = 1; i < words.Length; i = i + 2)
            {
                string keyWord = words[i + 1];
                var lamda2 = (Expression<Func<string, bool>>)(t => t.Contains(keyWord));

                switch (words[i].ToUpper())
                {
                    case "AND":
                        lamdas = lamdas.And(lamda2);
                        break;
                    case "OR":
                        lamdas = lamdas.Or(lamda2);
                        break;
                    default:
                        break;
                }
            }

            return lamdas;
        }
    }




}
