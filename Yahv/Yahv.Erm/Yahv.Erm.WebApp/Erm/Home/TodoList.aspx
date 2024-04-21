<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TodoList.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Home.TodoList" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <title>消息</title>

    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/reset.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />

    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon-jl-cool.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myDialog.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.myWindow.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/customs-easyui/Scripts/easyui.tabExtend.js"></script>

</head>
<body>
    <div id="todolist" class="home_todolist">
        <div class="home_todolist_title clearfix">
            <p>订单处理需要立即处理</p>
            <span>发起人：严**</span>
        </div>
        <p class="home_todolist_content">订单处理需要立即处理，需要中午12:00之前完成。处理完成及时给与我回复，谢谢！</p>
        <div class="home_todolist_time">2019-07-03 14:44</div>
    </div>
</body>
</html>
