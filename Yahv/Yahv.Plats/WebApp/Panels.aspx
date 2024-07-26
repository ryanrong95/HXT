<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Panels.Master" AutoEventWireup="true" CodeBehind="Panels.aspx.cs" Inherits="WebApp.Panels" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
        });
    </script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/styles/plugin.css" rel="stylesheet" />

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/timeouts.js"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphForm" runat="server">

    <!--多系统切换-->
     <div id="systemSwitch_select" style="width:170px;height:100%;position:absolute;top:30px;left:0;z-index:1000;background:#fff;display:none;">
        <ul class="clearfix">
            
        </ul>
    </div>

    <div class="easyui-layout" data-options="fit:true" id="toplayer">
        <!--头部-->
        <div data-options="region:'north'" class="header-box">
            <div class="header clearfix">
                <!--头部左侧-->
                <div class="header-left">
                    <div class="fl header-left-logo">
                        <img src="" class="LogoUrl" />
                    </div>
                    <div class="header-left-comName">
                        <h1></h1>
                        <div class="header-left-business header-left-business-single clearfix">
                            <span></span>
                        </div>
                        <div class="header-left-business header-left-business-multiple clearfix">
                            <span></span><i></i>
                        </div>
                    </div>
                </div>
                <!--隐藏侧边栏-->
              <%--  <div class="toggleSide open" onclick="toggleFun()">
                    <i class="iconfont open">&#xe716;</i>
                    <i class="iconfont close">&#xe624;</i>
                    <span class="msg-light"></span>
                    <!--<span style="display:none;">隐藏侧边栏</span>-->
                </div>--%>
                <!--头部右侧-->
                <div class="header-right clearfix">
                    <%--<div class="header-right-msg fl" onclick="openReadList()">
                        <i class="iconfont">&#xe678;</i>
                        <span>(<b>0</b>)</span>
                    </div>
                    <div class="header-right-helpdoc fl">
                        <a href="#" class="HelpUrl"><i class="iconfont">&#xe717;</i>帮助文档</a>
                    </div>
                    <div class="header-right-fullscreen fl">
                        <div class="header-right-fullscreen-open">
                            <i class="iconfont">&#xe62b;</i>
                             <span>开启全屏</span>
                        </div>
                        <div class="header-right-fullscreen-close">
                            <i class="iconfont">&#xe608;</i>
                            <span>退出全屏</span>
                        </div>
                    </div>
                    <div class="header-right-line fl"></div>
                    <div class="header-right-componey fl">
                        <!--<i></i>-->
                        <span onclick="selectCom()" class="selectCom"></span>
                    </div>--%>
                    <div class="header-right-person fl">
                        <i class="iconfont">&#xe647;</i>
                        <span class="blue" runat="server" id="spanUsername"></span>
                        <div class="user-box">
                            <ul>
                                <li>
                                    <i class="iconfont">&#xe60f;</i>
                                    <span id="userIphone" runat="server"></span>
                                </li>
                                <li>
                                    <i class="iconfont">&#xe616;</i>
                                    <span id="userEmail" runat="server"></span>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="header-right-changePsd fl">
                        <span onclick="openChangePsd()">修改密码</span>
                    </div>
            <%--        <div class="header-right-changePsd fl">
                        <a href="#" onclick="bindWx()">绑定微信</a>
                    </div>--%>
                    <div class="header-right-line fl"></div>
                    <div class="header-right-exit fl" onclick="exitLogin()">
                        <span><i class="iconfont">&#xe682;</i><%--<span>退出</span>--%></span>
                    </div>
                </div>
            </div>
        </div>
        <!-- 左侧菜单 -->
        <div data-options="region:'west',split:false,collapsedSize:-1" class="nav-left">
            <div id="sm"></div>
        </div>
        <!-- 主体iframe部分 -->
        <div data-options="region:'center'" style="background-color: #ffffff; padding-left: 1px; padding-top: 1px;">
            <div class="overflowStyle" id="tabs">
                <div title="首页" style="overflow: hidden; background: #f4f4f4;">
                    <iframe src="" style="width: 100%; height: 100%; padding: 10px 10px 0 10px;" class="workSpace FirstUrl"></iframe>
                </div>
            </div>
        </div>
    </div>
    <!-- 选择业务 -->
    <%--<ul class="header-business-list"></ul>--%>
    <!--选择公司、职位弹出层-->
    <div id="selectCom">
        <ul class="easyui-tree" id="comMsg"></ul>
    </div>
    <!--多系统切换-->
    <%--<div id="systemSwitch">
        <ul class="clearfix">
        </ul>
    </div>--%>

   
   
</asp:Content>
<%-- 适合第三种邮箱的那种 --%>