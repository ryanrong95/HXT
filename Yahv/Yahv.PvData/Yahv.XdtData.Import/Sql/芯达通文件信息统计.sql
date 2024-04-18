USE [foricScCustoms_ol]

SELECT Name FROM SysObjects Where XType = 'U' ORDER BY Name;
SELECT Name FROM SysObjects Where XType = 'U' AND NAME LIKE '%File%';

--SELECT * FROM OrderWhesPremiumFiles;
--SELECT * FROM PaymentNoticeFiles;
SELECT * FROM ClientFiles;
--SELECT * FROM TemporaryFiles;
SELECT * FROM ExitNoticeFiles;
--SELECT * FROM VoyageFiles;
SELECT * FROM MainOrderFiles;
SELECT * FROM DecHeadFiles;
SELECT * FROM OrderFiles;
SELECT * FROM PayExchangeApplyFiles  where userid is not null;

--SELECT DISTINCT FileType FROM OrderWhesPremiumFiles;
--SELECT DISTINCT FileType FROM PaymentNoticeFiles;
SELECT DISTINCT FileType FROM ClientFiles;
--SELECT DISTINCT FileType FROM TemporaryFiles;
SELECT DISTINCT FileType FROM ExitNoticeFiles;
--SELECT DISTINCT FileType FROM VoyageFiles;
SELECT DISTINCT FileType FROM MainOrderFiles;
SELECT DISTINCT FileType FROM DecHeadFiles;
SELECT DISTINCT FileType FROM OrderFiles;
SELECT DISTINCT FileType FROM PayExchangeApplyFiles;

select count(*) from OrderFiles where (filetype = 1 or filetype = 3 or filetype = 5) and url in (select url from MainOrderFiles);

-- ClientFiles��
--		7:  Ӫҵִ��

-- ExitNoticeFiles
--		23: �ͻ��ջ�ȷ�ϵ� -- �󶩵�

-- MainOrderFiles
--		1:  ���˵�
--		3:  ������ί����
--		5:  ��ͬ��Ʊ

-- DecHeadFiles:
--		16: ���ص��ļ�
--		17: ���ص���˰��Ʊ
--		18: ���ص���ֵ˰��Ʊ

-- OrderFiles:
--		1:  ���˵�
--		3:  ������ί����
--		5:  ��ͬ��Ʊ
--		12: 3C��֤����

-- PayExchangeApplyFiles:
--		2:  ����ί����  -- 
--		11: ����PI�ļ�

SELECT * FROM PayExchangeApplyFiles WHERE PayExchangeApplyID = 'PEA20200312001';
SELECT * FROM PayExchangeApplyItems WHERE PayExchangeApplyID = 'PEA20200312001';

SELECT * FROM PayExchangeApplyFiles WHERE URL IN (SELECT URL FROM MainOrderFiles);
SELECT * FROM MainOrderFiles WHERE URL = 'Order/201912/03/19101015474243_83_128_PI.pdf';


-- //foricerp0.ic360.cn/wladmin/Files/Order/202003/25/0331567921842.png

select * from ExitNotices where id = 'N20200319002';