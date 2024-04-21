<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Index2.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Home.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        body {
            background: #f4f4f4;
            position: relative;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div>系统首页开发中...</div>
    <%--<p>
        本处开发个人首页
    </p>
    <p>
        如果没有个人首页，可以直接跳转到实际期望的地址
    </p>
    <code>&lt;script&gt;
                   location.href = '[期望的url]';
       &lt;/script&gt;
    </code>--%>
    <%--<div class="home_bg">
        <div class="home_header clearfix">
            <img src="http://fixed2.b1b.com/Yahv/customs-easyui/Image/home_user_pic.png" />
            <p>您好，<span>张三</span>，欢迎回来！</p>
        </div>
        <div class="home_content">
            <div class="home_left">
                <div class="home_item_box todolist">
                    <div class="home_item_title clearfix">
                        <i></i>
                        <span class="home_item_title_text">待办事项</span>
                        <span class="home_item_title_num">(<em>36</em>)</span>
                        <a href="#">更多》</a>
                    </div>
                    <table>
                        <tr>
                            <th>事项标题</th>
                            <th>发起人</th>
                            <th>阅读状态</th>
                            <th>阅读时间</th>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#" onclick="openTodolist()">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_unprocessed"><span>待办理</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#" onclick="openTodolist()">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_unprocessed"><span>待办理</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#" onclick="openTodolist()">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_processed"><span>已办理</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#" onclick="openTodolist()">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_unprocessed"><span>待办理</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                    </table>
                </div>
                <div class="home_item_box salary">
                    <div class="home_item_title clearfix">
                        <i></i>
                        <span class="home_item_title_text">我的工资</span>
                        <a href="#">更多》</a>
                    </div>
                    <table>
                        <tr>
                            <th>发放日期</th>
                            <th>奖金</th>
                            <th>金额</th>
                        </tr>
                        <tr>
                            <td>2019-07-03 14:44</td>
                            <td>无</td>
                            <td>￥10000.00</td>
                        </tr>
                        <tr>
                            <td>2019-07-03 14:44</td>
                            <td>无</td>
                            <td>￥10000.00</td>
                        </tr>
                        <tr>
                            <td>2019-07-03 14:44</td>
                            <td>无</td>
                            <td>￥10000.00</td>
                        </tr>
                    </table>
                </div>
                <div class="home_item_box announcement">
                    <div class="home_item_title clearfix">
                        <i></i>
                        <span class="home_item_title_text">个人公告</span>
                        <span class="home_item_title_num">(<em>36</em>)</span>
                        <a href="#">更多》</a>
                    </div>
                    <table>
                        <tr>
                            <th>事项标题</th>
                            <th>发起人</th>
                            <th>阅读状态</th>
                            <th>阅读时间</th>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">周报周报周报周报周报周报周报周报周报周报</a></td>
                            <td>严**</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                            <td>2019-07-03 14:44</td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="home_right">
                <div class="home_item_box title">
                    <div class="home_item_title clearfix">
                        <i></i>
                        <span class="home_item_title_text">个人工作汇报</span>
                        <span class="home_item_title_num">(<em>36</em>)</span>
                        <a href="#">更多》</a>
                    </div>
                    <table>
                        <tr>
                            <th>标题</th>
                            <th>日期</th>
                            <th>状态</th>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_processed"><span>已阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                        <tr>
                            <td class="home_item_table_tl"><a href="#">关于***项目的工作汇报</a></td>
                            <td>2019-07-03 14:44</td>
                            <td class="home_item_table_unprocessed"><span>未阅读</span></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>--%>
    

    <%--<div class="home_shadow">
        <div class="home_shadow_box">
            <div class="home_shadow_title">
                <i></i><span>待办事项</span><i></i>
            </div>
            <div class="home_shadow_content">
                <div>
                    <p>订单处理需要立即处理</p>
                    <span>发起人：严**</span>
                </div>
                <p>订单处理需要立即处理，需要中午12:00之前完成。处理完成及时给与我回复，谢谢！</p>
                <div>2019-07-03 14:44</div>
            </div>
            <button type="button">确定</button>
        </div>
    </div>--%>
    <%--<script>
        function openTodolist() {
            $.myDialog({
                url: 'TodoList.aspx',
                isHaveOk: false,
                title: '待办事项',
                width: 440,
                height: 264
            });
        }
    </script>--%>
</asp:Content>
