<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.TraceRecords.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $('#file').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#fileMessge',
                successTarget: '#fileSuccess',
                multiple: true
            });

            $('#Contact').contactCrmPlus({
                required: true,
                isAdd: true
            });

            $("#FollowWay").fixedCombobx({
                type: "FollowWay",
               required: true,

            });

            $("#Name").clientCrmPlus({
               // value: model.Entity ?.Client ?.Name,
                onChange: function (Value) {
                    ////获取当前选中的值，返回json
                    var json = $('#Name').clientCrmPlus('getValue');
                    $('#Contact').contactCrmPlus('setEnterpriseID', json);
                }
            });

            $('#Readers').combobox({
                data: model.Readers,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,

            });

            $("#btnSaleChance").click(function () {

                $.myDialog({
                    title: "新增销售机会",
                    url: './SalesChances/List.aspx?&id=' + model.ID, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "50%",
                    height: "60%",
                })
            });

            $("#btnContact").click(function () {
                var enterpriseid = $("#Name").combobox("getValue");
                if (!enterpriseid) {
                    $.messager.alert("消息", "请先选择客户");
                    return;
                }
                $.myDialog({
                    title: "新增联系人",
                    url: '../Client/Contacts/Edit.aspx?&id=' + enterpriseid, onClose: function () {
                        window.location.reload();
                    },
                    width: "50%",
                    height: "60%",
                })
            });

        })
    </script>
    <script type="text/javascript">
        function onSelect1(sd) {
            $('#NextDate').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }
        function onSelect2(ed) {
            $('#TraceDate').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
                }
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <table class="liebiao">
            <tr>
                <td>客户名称：</td>
                <td>
                    <input id="Name" name="Name" style="width: 200px"  /></td>
                <td>跟进方式：</td>
                <td>
                    <input  id="FollowWay" name="FollowWay" style="width: 200px"  />
                </td>
            </tr>
            <tr>
                <td>跟进日期：</td>
                <td>
                    <input id="TraceDate" name="TraceDate" class="easyui-datebox" style="width: 200px" data-options="required:true,editable:false,prompt:'开始时间',onSelect:onSelect1" />
                </td>
                <td>下次跟进日期:</td>
                <td>
                    <input id="NextDate" name="NextDate" class="easyui-datebox" style="width: 200px" data-options="required:true, editable:false,prompt:'结束时间',onSelect:onSelect2" />
                </td>
            </tr>
            <tr>
                <td>原厂陪同人员：</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="SupplierStaffs" name="SupplierStaffs" data-options="required:false" style="width: 600px; height: 25px;" /></td>
            </tr>
          <%--  <tr>
                <td>本公司陪同人员：</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="CompanyStaffs" name="CompanyStaffs" data-options="required:false" style="width: 600px; height: 25px;" /></td>
            </tr>--%>
            <tr>
                <td>指定阅读人：</td>
                <td colspan="3">
                    <input class="easyui-combobox" id="Readers" name="Readers" style="width: 600px" data-options="required:false,validType:'length[1,50]'" /></td>
            </tr>
            <tr>
                <td>客户联系人：</td>
                <td colspan="3">
                    <input  id="Contact" name="Contact" style="width: 600px; height: 25px" />
            </tr>
            <tr>
                <td>跟进内容：</td>
                <td colspan="3">
                    <input class="easyui-textbox input" id="Context" name="Context"
                        data-options="required:true, multiline:true,validType:'length[1,350]',tipPosition:'right'" style="width: 600px; height: 80px" />
                </td>
            </tr>
            <tr>
                <td>下一步计划：</td>
                <td colspan="3">
                    <input class="easyui-textbox input" id="NextPlan" name="NextPlan"
                        data-options="required:true, multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 600px; height: 80px" />
                </td>
            </tr>
            <tr>
                <td>附件上传：</td>
                <td colspan="3">
                    <div>
                        <a id="file">上传</a>
                        <div id="fileMessge" style="display: inline-block; width: 350px;"></div>
                    </div>
                    <div id="fileSuccess"></div>
                </td>
                <%--  <td>销售机会：</td>
                <td><a id="btnSaleChance" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add',prompt:'请先提交，再添加销售机会'">添加机会</a></td>--%>
            </tr>
             <%--<tr>
                <td>指定阅读人：</td>
               <td> 
                <uc1:Admins runat="server" id="Admins" RoleIds="<%# new string[]{FixedRole.SaleManager.GetFixedID(), FixedRole.PM.GetFixedID()}%>" />
               </td>
            </tr>--%>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>

