using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OuterCalculate : BaseCalculate
    {
        public OuterCalculate(List<string> packNos) : base(packNos)
        {

        }
        public override int Calculate()
        {
            try
            {
                //外单多个订单一个箱子到货，使用特殊箱号 2022-02-21 ryan
                //处理特殊箱号：WL06-1   WL06-01
                var packs = new List<string>();
                foreach (var code in this.PackNos)
                {
                    //正确连续信号：WL01-WL03
                    if (Regex.Split(code, "-WL").Length > 1)
                    {
                        packs.Add(code);
                        continue;
                    }

                    //特殊箱号：WL06-1,只取-前的有效箱号部分
                    if (code.Split('-').Length > 1)
                    {
                        packs.Add(code.Split('-')[0]);
                        continue;
                    }

                    //单独箱号WL08
                    packs.Add(code);
                }

                Regex regex_number = new Regex(@"^(\D*)(\d+)(.*)$", RegexOptions.Singleline);
                HashSet<string> sets = new HashSet<string>();
                foreach (var code in packs)
                {
                    string boxcode = code;

                    MatchCollection kuohaoResults = Regex.Matches(code, @"\(.*?\)", RegexOptions.Singleline);
                    if (kuohaoResults.Count > 0)
                    {
                        //生成不要的下标号 exceptIndexes
                        List<int> exceptIndexes = new List<int>();
                        for (int i = 0; i < kuohaoResults.Count; i++)
                        {
                            if (kuohaoResults[i].Success)
                            {
                                exceptIndexes.AddRange(Enumerable.Range(kuohaoResults[i].Index, kuohaoResults[i].Length).ToArray());
                            }
                        }
                        //得到 newChars
                        char[] originChars = boxcode.ToCharArray();
                        List<char> newChars = new List<char>();
                        for (int i = 0; i < originChars.Length; i++)
                        {
                            if (!exceptIndexes.Contains(i))
                            {
                                newChars.Add(originChars[i]);
                            }
                        }

                        boxcode = new string(newChars.ToArray());
                    }

                    var splits = boxcode.Split('-');
                    if (splits.Length == 1)
                    {
                        sets.Add(regex_number.Match(splits.First()).Groups[0].Value.Trim());
                        continue;
                    }

                    //以组的方式主要是利用切断
                    var prex = regex_number.Match(splits.First()).Groups[1].Value;
                    var first = int.Parse(regex_number.Match(splits.First()).Groups[2].Value);
                    var last = int.Parse(regex_number.Match(splits.Last()).Groups[2].Value);

                    //这样写主要就是为可以报错！
                    for (int index = first; index <= last; index++)
                    {
                        sets.Add(prex + index.ToString());
                    }
                }
                return sets.Count();
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
    }
}
