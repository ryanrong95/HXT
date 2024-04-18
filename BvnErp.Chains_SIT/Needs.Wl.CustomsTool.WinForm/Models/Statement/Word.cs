using Needs.Wl.CustomsTool.WinForm.Enums.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Statement
{
    /// <summary>
    /// 词汇
    /// </summary>
    public class Word
    {
        /// <summary>
        /// 词汇文本
        /// </summary>
        private string _text { get; set; } = string.Empty;

        /// <summary>
        /// 是否标记
        /// </summary>
        private bool _isFlag { get; set; } = false;

        /// <summary>
        /// 类型
        /// </summary>
        private WordTypeEnum _type { get; set; } = WordTypeEnum.Normal;

        /// <summary>
        /// 词汇文本
        /// </summary>
        public string Text { get { return _text; } }

        /// <summary>
        /// 是否标记
        /// </summary>
        public bool IsFlag { get { return _isFlag; } }

        /// <summary>
        /// 类型
        /// </summary>
        public WordTypeEnum Type { get { return _type; } }

        /// <summary>
        /// 自身位置
        /// </summary>
        public int SelfPos { get; set; }

        /// <summary>
        /// 匹配位置
        /// </summary>
        public int PairPos { get; set; }

        public Word(string text)
        {
            this._text = text;

            switch (text)
            {
                case "{{":
                    this._isFlag = true;
                    this._type = WordTypeEnum.Left;
                    break;
                case "}}":
                    this._isFlag = true;
                    this._type = WordTypeEnum.Right;
                    break;
                default:
                    this._isFlag = false;
                    this._type = WordTypeEnum.Normal;
                    break;
            }
        }
    }
}
