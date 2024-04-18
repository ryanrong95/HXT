<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="msg.aspx.cs" Inherits="WebApp.msg" %>

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
    <div id="readNotice">
        <div id="readNoticeContent">
            <ul></ul>
        </div>
    </div>
    <script>
        $(function () {
            openReadList();
            //点击消息列表的某一项查看消息内容
            $("#readNoticeContent ul").on("click", ".readed_title", function () {
                if (!$(this).parent().hasClass("readed")) {
                    var id = $(this).parent().attr("val");
                    var $that = $(this);
                    $.ajax({
                        type: "get",
                        url: "/api/Notices.ashx?action=read&id=" + id,
                        success: function (data) {
                            if (data.success) {
                                $that.parent().addClass("readed");
                                useGetMsgList();
                            }
                        },
                        error: function () {
                            console.log('fail');
                        }
                    });
                }
                if ($(this).parent().hasClass("expand")) {
                    $(this).parent().removeClass("expand");
                    $(this).parent().find("p").slideUp();
                } else {
                    $("#readNoticeContent ul li").removeClass("expand");
                    $("#readNoticeContent ul li").find("p").slideUp();
                    $(this).parent().addClass("expand");
                    $(this).parent().find("p").slideDown();
                }
            })
        })

        //获取消息列表数据
        function useGetMsgList() {
            getMsgList(function (data) {
                noReadedCount = 0;
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Readed == false) {
                        noReadedCount++;
                    }
                }
                $(".header-right-msg", top.window.document).find("span").find("b").text(noReadedCount);
                if (noReadedCount==0) {
                    $(".header-right-msg", top.window.document).removeClass("blue");
                    $(".header-right-msg", top.window.document).addClass("gray");
                }
            })
        }
        //获取消息列表数据
        function getMsgList(cb) {
            $.ajax({
                type: "get",
                url: "/api/Notices.ashx",
                success: function (data) {
                    cb(data);
                },
                error: function (err) {
                   console.log(err)
                }
            })
        }
        //生成消息数据列表
        function openReadList() {
            getMsgList(function (data) {
                $("#readNoticeContent ul li").remove();
                var noReadedCount = 0;
                for (var i = 0; i < data.length; i++) {
                    var $li = $('<li></li>');
                    var $readed_title = $('<div class="readed_title"><i class="iconfont">&#xe643;</i></div>');
                    var $h3 = $('<h3>' + data[i].Title + '</h3>');
                    var $p = $('<p>' + data[i].Context + '</p>');
                    $li.attr("val", data[i].ID);
                    if (data[i].Readed == false) {
                        var $span = $('<span class="readed_blue">[未读]</span>');
                        $h3.append($span)
                    } else {
                        $li.addClass("readed");
                    }
                    $readed_title.append($h3);
                    $li.append($readed_title);
                    $li.append($p);
                    $("#readNoticeContent ul").append($li);
                }
            })
        }
    </script>
</body>
</html>
