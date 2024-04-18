using Needs.Wl.CustomsTool.WinForm.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Statement
{
    /// <summary>
    /// 表达式分析
    /// </summary>
    public class Parser
    {
        private string ExceptionConfigPath
        {
            get { return ConfigurationManager.AppSettings["ExceptionConfigPath"]; }
        }

        private List<Statement> _statements = new List<Statement>();

        public Parser()
        {
            string[] configFileNames = new string[] { "RestartCustomsConfig", "ResendMsgConfig", "RemindHimConfig", "OtherExceptionConfig" };

            foreach (var configFileName in configFileNames)
            {
                LoadExceptionConfig(this._statements, configFileName);
            }
        }

        public void ClearStatement()
        {
            this._statements.Clear();
        }

        /// <summary>
        /// 分析得出处理方式
        /// </summary>
        /// <returns></returns>
        public ExceptionHandlerEnum Analyse(string text)
        {
            ExceptionHandlerEnum result = ExceptionHandlerEnum.UnTreated;

            if (this._statements == null || !this._statements.Any())
            {
                return result;
            }

            foreach (var statement in this._statements)
            {
                if (statement.Match(text))
                {
                    switch (statement.Type)
                    {
                        case "RestartCustomsConfig":
                            result = ExceptionHandlerEnum.RestartCustomsConfig;
                            break;
                        case "ResendMsgConfig":
                            result = ExceptionHandlerEnum.ResendMsgConfig;
                            break;
                        case "RemindHimConfig":
                            result = ExceptionHandlerEnum.RemindHimConfig;
                            break;
                        case "OtherExceptionConfig":
                            result = ExceptionHandlerEnum.OtherExceptionConfig;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 加载异常配置
        /// </summary>
        /// <param name="listConfig"></param>
        /// <param name="configFileName"></param>
        private void LoadExceptionConfig(List<Statement> listConfig, string configFileName)
        {
            string fileFullName = ExceptionConfigPath + @"\" + configFileName + ".txt";

            if (!File.Exists(fileFullName))
            {
                return;
            }

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
                            listConfig.Add(new Statement(line, configFileName));
                        }
                    }
                }
            }
        }
    }
}
