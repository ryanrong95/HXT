--根据岗位更新销售角色（将岗位：推广，贸易，销售 都改成销售角色）
UPDATE dbo.Admins SET RoleID='FRole001' WHERE 
id IN(
SELECT a.ID FROM dbo.Admins a 
LEFT JOIN dbo.Staffs s ON a.StaffID=s.ID
LEFT JOIN dbo.Postions p ON s.PostionID=p.ID
WHERE RoleID ='NRole000' AND
(p.Name LIKE '%推广%' OR p.Name LIKE '%贸易%' OR p.Name LIKE '%销售%')
)

--根据岗位更新采购（将包含采购的更新成采购角色）
UPDATE dbo.Admins SET RoleID='FRole004' WHERE id IN
(
SELECT a.ID FROM dbo.Admins a 
LEFT JOIN dbo.Staffs s ON a.StaffID=s.ID
LEFT JOIN dbo.Postions p ON s.PostionID=p.ID
WHERE RoleID ='NRole000' AND
(p.Name LIKE '%采购%' )
)
