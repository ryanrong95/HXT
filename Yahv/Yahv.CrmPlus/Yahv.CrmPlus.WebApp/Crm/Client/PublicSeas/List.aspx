<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fixedCombobx.js"></script>
    <script>
        $(function () {

            $("#clientType").fixedCombobx({
                isAll:true,
                type: 'ClientType'
            })

            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getValue")),
                    clientType: $("#clientType").fixedCombobx('getValue'),
                    startDate: $('#s_startdate').datebox("getValue"),
                    endDate: $('#s_enddate').datebox("getValue"),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        });
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnClaim" href="#" particle="Name:\'认领\',jField:\'btnClaim\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-assign\'" onclick="claim(\'' + rowData.ID + '\')">认领</a> ');
            if (rowData.Status == '<%=Yahv.Underly.AuditStatus.Normal.GetHashCode()%>') {

                arry.push('<a id="btnDetail" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rowData.ID + '\')">详情</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }

        function claim(id) {
            $.myDialog({
                title: "认领",
                url: 'Claim.aspx?ID=' + id,
                width: "60%",
                height: "80%",
            });

        };


        function detail(id) {
            $.myWindow({
                title: '综合信息',
                url: '../Index.aspx?id=' + id + "&Source=fromPublic",
                width: '80%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }

        function showAddPage() {
            $.myDialog({
                title: "新增",
                url: 'Add.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "450px",
                height: "200px",
            });

        }
        function nameformatter(value, rowData) {
            var result = "";
            result += '<span class="vip' + rowData.Vip + '"></span>';
            result += '<span class="level' + rowData.Grade + '"></span>';
            if (rowData.IsMajor) {
                result += '<span class="star"></span>';
            }
            result += rowData.Name;
            return result;
        }
        function isformatter(value, rowData) {
            return value ? "是" : "否";
        }
    </script>

    <script type="text/javascript">
        function onSelect1(sd) {
            $('#s_enddate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }
        function onSelect2(ed) {
            $('#s_startdate').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
                }
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 100px;">客户名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'客户名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td>客户类型</td>
                    <td>
                       <input id="clientType" name="clientType"  />
                     <%--   <select id="clientType" name="clientType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>--%>
                    </td>
                    <td style="width: 100px;">注册时间</td>

                    <td>
                        <input id="s_startdate" class="easyui-datebox" data-options="editable:false,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_enddate" class="easyui-datebox" data-options="editable:false,prompt:'结束时间', onSelect:onSelect2 " />
                    </td>
                    <td colspan="3">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <a id="btnCreator" particle="Name:'新增',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">新增</a>
                        <em class="toolLine"></em>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'Ck',checkbox:true"></th>
                <th data-options="field:'Name',formatter:nameformatter,width:280">客户名称</th>
                <th data-options="field:'ClientType',width:120">客户类型</th>
                <th data-options="field:'StatusDes',width:100">审核状态</th>
                <th data-options="field:'District',width:120">国别地区</th>
                <th data-options="field:'Industry',width:120">所属行业</th>
                <th data-options="field:'IsSpecial',formatter:isformatter,width:80">是否特殊</th>
                <th data-options="field:'IsInternational',formatter:isformatter,width:80">是否国际</th>
                <th data-options="field:'CreateDate',width:150">认领时间</th>
                <th data-options="field:'CreateDate',width:150">注册时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
            </tr>
        </thead>
    </table>

</asp:Content>
