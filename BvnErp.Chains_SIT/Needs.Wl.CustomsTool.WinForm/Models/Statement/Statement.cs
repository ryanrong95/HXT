using Needs.Utils.Linq;
using Needs.Wl.CustomsTool.WinForm.Enums.Statement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.Statement
{
    /// <summary>
    /// 配置语句
    /// </summary>
    public class Statement
    {
        /// <summary>
        /// 语句
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 词汇列表
        /// </summary>
        public Word[] Words;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEffective { get; set; } = true;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// 树结构
        /// </summary>
        private TreeNode Tree { get; set; }

        /// <summary>
        /// lamda 表达式
        /// </summary>
        public Expression<Func<string, bool>> Expression { get; set; }

        public Statement(string state, string type)
        {
            state = state.Trim();
            type = type.Trim();

            this.State = state;
            this.Type = type;

            //1. 生成词汇列表
            this.Words = GenerateWords(state);

            //2. 检查语句是否有效 (1) 括号是否能够匹配  (2) 抽去所有括号，看是否是值和运算符交叉排列
            //   无效的语句赋值“错误信息”
            string error = string.Empty;
            this.IsEffective = CheckGrammar(out error);
            this.Error = error;

            //有效的语句
            if (this.IsEffective)
            {
                //3. 根据 this.Words 生成“树结构”
                this.Tree = BuildTree(0, this.Words.Length - 1);

                //4. 根据“树结构”赋值“lamda 表达式”
                this.Expression = GenExpression(this.Tree);
            }
        }

        public bool Match(string text)
        {
            string[] texts = { text };

            var linq = (from t in texts.ToList()
                        select t);

            return linq.Any(this.Expression.Compile());
        }

        /// <summary>
        /// 生成词汇列表
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private Word[] GenerateWords(string state)
        {
            string[] words = state.Split();
            words = (from word in words
                     select word.Trim()).ToArray();
            words = words.Where(t => !string.IsNullOrEmpty(t)).ToArray();

            List<Word> listWord = new List<Word>();
            for (int i = 0; i < words.Length; i++)
            {
                listWord.Add(new Word(words[i]) { SelfPos = i, });
            }

            return listWord.ToArray();
        }

        /// <summary>
        /// 检查语句是否有效
        /// (1) 括号是否能够匹配  (2) 抽去所有括号，看是否是值和运算符交叉排列
        /// 无效的语句赋值“错误信息”
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool CheckGrammar(out string error)
        {
            //(1) 括号是否能够匹配
            Stack stack = new Stack();

            Word[] words = this.Words.ToArray();
            Word[] flags = words.Where(t => t.IsFlag).OrderBy(t => t.SelfPos).ToArray();

            if (flags.Length % 2 != 0)
            {
                error = "表达式中括号不能成对";
                return false;
            }


            foreach (var flag in flags)
            {
                if (flag.Text == "{{")
                {
                    stack.Push(flag);
                }

                if (flag.Text == "}}")
                {
                    Word pairWord = (Word)stack.Pop();
                    if (pairWord != null)
                    {
                        flag.PairPos = pairWord.SelfPos;
                        pairWord.PairPos = flag.SelfPos;
                    }
                    else
                    {
                        error = "表达式中括号不能成对";
                        return false;
                    }
                }
            }

            //(2) 抽去所有括号，看是否是值和运算符交叉排列
            Word[] normal = words.Where(t => !t.IsFlag).OrderBy(t => t.SelfPos).ToArray();
            for (int i = 0; i < normal.Length; i++)
            {
                if (i % 2 == 0)
                {
                    //值位置

                }
                else
                {
                    //运算符位置
                    if (normal[i].Text.ToLower() != "and" && normal[i].Text.ToLower() != "or")
                    {
                        error = normal[i].SelfPos + " 位置处应该为运算符";
                        return false;
                    }
                }
            }

            error = string.Empty;
            return true;
        }

        private TreeNode BuildTree(int beginIndex, int endIndex)
        {
            if (beginIndex == endIndex)
            {
                return new TreeNode()
                {
                    Info = new NodeInfo() { Word = this.Words[beginIndex], IsLeaf = true, }
                };
            }

            if (this.Words[beginIndex].PairPos == endIndex)
            {
                return BuildTree(beginIndex + 1, endIndex - 1);
            }

            for (int i = beginIndex; i <= endIndex; i++)
            {
                if (this.Words[i].Text.ToLower() == "and" || this.Words[i].Text.ToLower() == "or")
                {
                    if (CheckIsAppropriateOperator(CheckOperatorDirEnum.TurnLeft, beginIndex, i)
                        && CheckIsAppropriateOperator(CheckOperatorDirEnum.TurnRight, i, endIndex))
                    {
                        return new TreeNode()
                        {
                            Info = new NodeInfo() { Word = this.Words[i], IsOperator = true, },
                            Left = BuildTree(beginIndex, i - 1),
                            Right = BuildTree(i + 1, endIndex)
                        };
                    }
                }
            }

            throw new Exception("暂未知情况");
        }

        private bool CheckIsAppropriateOperator(CheckOperatorDirEnum dirEnum, int beginIndex, int endIndex)
        {
            if (dirEnum == CheckOperatorDirEnum.TurnLeft)
            {
                for (int i = endIndex; i >= beginIndex; i--)
                {
                    if (this.Words[i].IsFlag && this.Words[i].Type == WordTypeEnum.Left)
                    {
                        return false;
                    }
                    else if (this.Words[i].IsFlag && this.Words[i].Type == WordTypeEnum.Right)
                    {
                        return true;
                    }
                }
            }
            else if (dirEnum == CheckOperatorDirEnum.TurnRight)
            {
                for (int i = beginIndex; i <= endIndex; i++)
                {
                    if (this.Words[i].IsFlag && this.Words[i].Type == WordTypeEnum.Right)
                    {
                        return false;
                    }
                    else if (this.Words[i].IsFlag && this.Words[i].Type == WordTypeEnum.Left)
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 生成 lamdas
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private Expression<Func<string, bool>> GenExpression(TreeNode tree)
        {
            PostOrder(tree);
            return this.Tree.Info.Expression;
        }

        private void PostOrder(TreeNode treeNode)
        {
            if (treeNode.Info.IsLeaf)
            {
                return;
            }

            PostOrder(treeNode.Left);
            PostOrder(treeNode.Right);


            Expression<Func<string, bool>> thisNodeLamda = null;
            Expression<Func<string, bool>> leftLamda = null;
            Expression<Func<string, bool>> rightLamda = null;

            if (treeNode.Left.Info.IsOperator)
            {
                leftLamda = treeNode.Left.Info.Expression;
            }
            else
            {
                leftLamda = (Expression<Func<string, bool>>)(t => t.Contains(treeNode.Left.Info.Word.Text));
            }

            if (treeNode.Right.Info.IsOperator)
            {
                rightLamda = treeNode.Right.Info.Expression;
            }
            else
            {
                rightLamda = (Expression<Func<string, bool>>)(t => t.Contains(treeNode.Right.Info.Word.Text));
            }


            switch (treeNode.Info.Word.Text.ToLower())
            {
                case "and":
                    thisNodeLamda = leftLamda.And(rightLamda);
                    break;
                case "or":
                    thisNodeLamda = leftLamda.Or(rightLamda);
                    break;
                default:
                    break;
            }

            treeNode.Info.Expression = thisNodeLamda;

        }



    }
}
