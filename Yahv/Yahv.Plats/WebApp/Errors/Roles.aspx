<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="WebApp.Errors.Roles" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>权限不足</title>
    <meta charset="utf-8" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Styles/reset.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Styles/main.css" rel="stylesheet" />
    <style> 
        .roles {
            width: 700px;
            height: 500px;
            position: fixed;
            left: 50%;
            top: 50%;
            margin-left: -350px;
            margin-top: -250px;
            font-size:20px;
            text-align:center;
        }
        .roles i,
        .roles span {
            display:block;    
        }
        .roles i {
            margin: 60px auto;
            color: #0094ff;
            font-size: 90px;
        }
        .roles span {
            margin-bottom:40px;
            color:#666666;
        }
        .roles a {
            color: #0094ff;
            text-decoration: none;
        }
        .roles a:hover {
            text-decoration:underline;    
        }
    </style>
</head>
<body>
    <div class="roles">
        <i class="iconfont">&#xe60b;</i>
        <span>不好意思，您的权限不足，无法登录系统，您可以联系管理员。</span>
        <div><a href="/Login.aspx">返回登录页面</a></div>
    </div>
</body>
</html>

