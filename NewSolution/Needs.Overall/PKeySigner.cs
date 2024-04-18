using System;
using System.Linq;
using System.Reflection;
using Layer.Linq;

namespace Needs.Overall
{
    /// <summary>
    /// 主键调用库
    /// </summary>
    public class PKeySigner
    {
        /// <summary>
        /// 私有构造（防止实例化）
        /// </summary>
        PKeySigner()
        {
            throw new NotSupportedException("不支持如此调用！");
        }

        /// <summary>
        /// 获取指定泛型T库主键
        /// </summary>
        /// <param name="pkeyType">主键类型</param>
        /// <returns>主键值</returns>
        public static string Pick<T>(T pkeyType) where T : struct, IComparable, IFormattable, IConvertible
        {
            var pker = GetPkInfo(pkeyType);

            var reponsitory = Activator.CreateInstance(pker.Repository) as LinqReponsitory;

            if (reponsitory == null)
            {
                throw new NotSupportedException($"\"{nameof(pker.Repository)}\" type output in \"{nameof(Declares)}\" is not supported");
            }

            using (reponsitory)
            {
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
		SET @VAL=@VAL+1;
	END
	
UPDATE [PRIMARYKEYS] SET 
		VALUE=@VAL , 
		[DAY]=@TODAY,
		[TYPE]=@PRIMARYKEYTYPE,
		[LENGTH]=@LENGTH  
	WHERE NAME =@NAME;
	
select @VAL;";
                string name = pker.Attribute.Name;
                int length = pker.Attribute.Length;
                int val = reponsitory.Query<int>(sql, name, (int)pker.Attribute.Type, pker.Attribute.Length, (int)Mode.Time).Single();

                if (pker.Attribute.Type == Mode.Time)
                {
                    return string.Concat(name, DateTime.Now.ToString("yyyyMMdd"), val.ToString().PadLeft(length, '0'));
                }
                else
                {
                    return string.Concat(name, val.ToString().PadLeft(length, '0'));
                }
            }
        }

        public static int Cull<T>(T pkeyType) where T : struct, IComparable, IFormattable, IConvertible
        {
            var pker = GetPkInfo(pkeyType);

            var reponsitory = Activator.CreateInstance(pker.Repository) as LinqReponsitory;

            if (reponsitory == null)
            {
                throw new NotSupportedException($"\"{nameof(pker.Repository)}\" type output in \"{nameof(Declares)}\" is not supported");
            }

            using (reponsitory)
            {
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
		SET @VAL=@VAL+1;
	END
	
UPDATE [PRIMARYKEYS] SET 
		VALUE=@VAL , 
		[DAY]=@TODAY,
		[TYPE]=@PRIMARYKEYTYPE,
		[LENGTH]=@LENGTH  
	WHERE NAME =@NAME;
	
select @VAL;";
                string name = pker.Attribute.Name;
                int length = pker.Attribute.Length;
                return reponsitory.Query<int>(sql, name, (int)pker.Attribute.Type, pker.Attribute.Length, (int)Mode.Time).Single();
            }
        }


        static Declares GetPkInfo<T>(T pkeyType) where T : struct, IComparable, IFormattable, IConvertible
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new NotSupportedException("Only support enumerated types!");
            }

            string name = Enum.GetName(pkeyType.GetType(), pkeyType);
            MemberInfo[] mis = pkeyType.GetType().GetMember(name);

            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var member = mis.Single();

            var rpstory = member.GetCustomAttribute<RepositoryAttribute>() as RepositoryAttribute;
            var primkey = member.GetCustomAttribute<PKeyAttribute>() as PKeyAttribute;

            return new Declares(rpstory == null ? null : rpstory.Type, primkey);
        }

        /// <summary>
        /// 定义
        /// </summary>
        internal class Declares
        {
            public Type Repository { get; private set; }
            public PKeyAttribute Attribute { get; private set; }
            internal Declares(Type repository, PKeyAttribute attribute)
            {
                this.Repository = repository;
                this.Attribute = attribute;
            }
        }

        /// <summary>
        /// 主键类型
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// 正常
            /// </summary>
            Normal = 1,
            /// <summary>
            /// 与时间有关
            /// </summary>
            Time = 2
        }
    }
}
