<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.TraceComments.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

           $("#FollowWay").fixedCombobx({
                isAll: true,
                type:"FollowWay",
            });

            $("#Follower").combobox({
                data: model.Follower,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Ownerid);
                    }
                }
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    name: $.trim($('#Name').textbox("getValue")),
                    followWay: $("#FollowWay").combobox("getValue"),
                    follower: $("#Follower").combobox("getValue"),
                    s_startdate: $("#s_startdate").datebox("getValue"),
                    s_enddate: $("#s_enddate").datebox("getValue"),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            });
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });


        });

        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.TraceRecordID + '\')">点评</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function showEditPage(id) {
            $.myWindow({
                title: "点评",
                url: 'Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
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
                    <td style="width: 100px;">公司名称</td>
                    <td>
                        <input id="Name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">跟进方式</td>
                    <td>
                        <input id="FollowWay" name="FollowWay" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" />
                    </td>
                    <td style="width: 100px;">跟进人 </td>
                    <td>
                        <input id="Follower" name="Follower" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px;">跟进日期</td>
                    <td>
                        <input id="s_startdate" class="easyui-datebox" data-options="editable:false,prompt:'开始时间',onSelect:onSelect1" />
                        -
                        <input id="s_enddate" class="easyui-datebox" data-options="editable:false,prompt:'结束时间',onSelect:onSelect2" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                     <%--   <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a>
                        <a id="btnComment" class="easyui-linkbutton" data-options="iconCls:''" onclick="Comment()">点评</a>--%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%" data-option="true">
        <thead>
            <tr>
                <th data-options="field:'Ck',checkbox:true"></th>
                <th data-options="field:'ClientName',width:200">客户名称</th>
                <th data-options="field:'FollowWay',width:120">跟进方式</th>
                <th data-options="field:'TraceDate',width:120">跟进日期</th>
                <th data-options="field:'NextDate',width:120">下次跟进日期</th>
                <th data-options="field:'Owner',width:80">跟进人姓名</th>
                <th data-options="field:'SupplierStaffs',width:250">原厂陪同人</th>
                <th data-options="field:'CompanyStaffs',width:200">本公司陪同人</th>
                <th data-options="field:'Reader',width:200">指定阅读人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>