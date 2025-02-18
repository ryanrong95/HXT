--根据表结构，生成属性
SELECT  'public ' AS 'key' ,
        CASE WHEN systypes.name = 'varchar' THEN 'string'
             WHEN systypes.name = 'nvarchar' THEN 'string'
             WHEN systypes.name = 'datetime' THEN 'DateTime'
             ELSE systypes.name
        END AS type ,
        syscolumns.name ,
        ' { get; set; }' AS 'word' ,
        sys.syscolumns.name + '=this.' + sys.syscolumns.name + ',' AS 'enter'
FROM    syscolumns ,
        systypes
WHERE   syscolumns.xusertype = systypes.xusertype
        AND syscolumns.id = OBJECT_ID('表名');
        
       