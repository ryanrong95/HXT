<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WxRepeatBind.aspx.cs" Inherits="WebApp.Errors.WxRepeatBind" %>



<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>绑定微信</title>
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
        <span>您微信已经绑定，不能重复绑定!</span>
        <div><a href="/Panels.aspx">返回</a></div>
    </div>
</body>
</html>

