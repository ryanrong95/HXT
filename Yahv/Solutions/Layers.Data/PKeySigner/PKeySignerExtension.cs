using Layers.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace Layers.Data
{
    /// <summary>
    /// 拓展
    /// </summary>
    static public class PKeySignerExtension
    {
        /// <summary>
        /// 获取指定泛型T对象主键序列
        /// </summary>
        /// <param name="pkeyType">主键类型</param>
        /// <typeparam name="T">主键类型</typeparam>
        /// <param name="quantity">生成个数</param>
        /// <param name="addName">追加名</param>
        /// <param name="orderBy">排序</param>
        /// <returns>主键序列</returns>
        static public string[] Series<T>(this LinqReponsitory reponsitory, T pkeyType, int quantity, string addName = null, PKeySigner.OrderBy orderBy = PKeySigner.OrderBy.None) where T : struct, IComparable, IFormattable, IConvertible
        {
            if (quantity <= 0)
            {
                //throw new ArgumentException("quantity", "必须:quantity＞0");

                //默认给一个1
                quantity = 1;
            }

            var pker = PKeySigner.GetPkInfo(pkeyType);
            //var reponsitory = Activator.CreateInstance(pker.Repository) as LinqReponsitory;
            if (reponsitory == null)
            {
                throw new NotSupportedException($"\"{nameof(pker.Repository)}\" type output in \"{nameof(PKeySigner.Declares)}\" is not supported");
            }

            //using (reponsitory)
            //{
            string sql = @"DECLARE @TODAY INT; 
DECLARE @VAL INT;
DECLARE @DAY INT;
DECLARE @NAME VARCHAR(10);
DECLARE @PRIMARYKEYTYPE INT;
DECLARE @LENGTH INT;
SET @TODAY=DAY(GETDATE());
SET @NAME={0};
SET @PRIMARYKEYTYPE={1};
SET @LENGTH={2};

SELECT @VAL=COUNT(1) FROM [PRIMARYKEYS] WHERE NAME =@NAME;
IF @VAL=0
BEGIN
	INSERT INTO [PRIMARYKEYS]
		([NAME],[TYPE],[LENGTH],[VALUE],[DAY])
	VALUES
		(@NAME,@PRIMARYKEYTYPE,@LENGTH,1,@TODAY);
END

SELECT @VAL=VALUE ,@DAY = [DAY] FROM [PRIMARYKEYS] WHERE NAME =@NAME;

IF	@PRIMARYKEYTYPE = {3} and @DAY != @TODAY
	BEGIN
		SET @VAL=1;
	END
ELSE
	BEGIN
		SET @VAL=@VAL+{4};
	END
	
UPDATE [PRIMARYKEYS] SET 
		VALUE=@VAL , 
		[DAY]=@TODAY,
		[TYPE]=@PRIMARYKEYTYPE,
		[LENGTH]=@LENGTH  
	WHERE NAME =@NAME;
	
select @VAL;";
            string name = pker.Attribute.Name;

            if (!string.IsNullOrWhiteSpace(addName))
            {
                name = name + addName;
            }

            int length = pker.Attribute.Length;
            int val = reponsitory.Query<int>(sql, name, (int)pker.Attribute.Type, pker.Attribute.Length, (int)PKeySigner.Mode.Date, quantity).Single();

            string[] keys = new string[quantity];
            for (int index = 0; index < quantity; index++)
            {
                if (pker.Attribute.Type == PKeySigner.Mode.Date)
                {
                    keys[index] = string.Concat(name, DateTime.Now.ToString("yyyyMMdd"), (val - index).ToString().PadLeft(length, '0'));
                }
                else
                {
                    keys[index] = string.Concat(name, (val - index).ToString().PadLeft(length, '0'));
                }
            }

            if (orderBy == PKeySigner.OrderBy.Ascending)
                return keys.OrderBy(item => item).ToArray();

            if (orderBy == PKeySigner.OrderBy.Descending)
                return keys.OrderByDescending(item => item).ToArray();

            return keys;
            //}
        }
    }



}
