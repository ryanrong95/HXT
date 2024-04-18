<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Approval.List" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>会员列表-总经理</title>
    <uc:EasyUI runat="server" />
     <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        var CurrentName = '<%=this.Model.CurrentName%>';

        //数据初始化
        $(function () {
            //下拉框数据初始化
            var status = eval('(<%=this.Model.Status%>)');
            var serviceManager = eval('(<%=this.Model.ServiceManager%>)');
            var DepartmentType = eval('(<%=this.Model.DepartmentType%>)');

            $('#Status').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: status
            });
            //业务员
            $('#ServiceManager').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: serviceManager,
            });

            //业务员部门
            $('#DepartmentType').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: DepartmentType,
                onChange: function (e) {
                    debugger
                    if (e != null && e != "") {
                        $.post('?action=getAdminsByDepartmentId', { DepartmentName: e }, function (data) {
                            console.log(data);
                            data = eval(data.data);
                            $('#ServiceManager').combobox({
                                valueField: 'Key',
                                textField: 'Value',
                                data: data,
                            });
                        })

                    } else {
                        $("#ServiceManager").combobox("loadData", serviceManager);
                    }
                    // var id = $(this).combobox("setValue", e);

                }
            });

            //订单列表初始化
            $('#clients').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
            });
            //复选框被勾选后自动筛选 数据
            $("#Unclaim").click(function () {
                Search();
            });

            $("#HasReturn").click(function () {
                Search();
            });

            $("#IsSAEleUpload").click(function () {
                Search();
            });
        });

        //超过7天之后标红
        function SetStyle(val, row, index) {

            if (row.RegisterDays >= 7 && row.StatusValue ==<%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%> && row.IsSpecified != true) {
                return '<span style="color:red;">' + val + '</span>';
            } else {
                return val;
            }
        }


        function ViewSummary(val, row, index) {
            var status = parseInt(row.StatusValue);
            if (status ==<%=Needs.Ccs.Services.Enums.ClientStatus.Returned.GetHashCode()%>) {
                return '<a href="javascript:void(0);" onclick="Reason(\'' + row.Summary + '\')" >' + val + '</a>'

            } else {
                return val;
            }
        }

        //超期未换汇
        function ViewUnPayExchange(val, row, index) {
            if (row.ServiceType == '代仓储') {
                return "-";
            }
            else {
                if (val >= 500000) {
                    return "<label style='color:red'>" + val + "</label>";
                }
                return val;
            }
        }
        //近期报关金额
        function ViewDeclare(val, row, index) {
            if (row.ServiceType == '代仓储') {
                return "-";
            }
            else {
                return val;
            }
        }
        //近期付汇金额
        function ViewPayExchange(val, row, index) {
            if (row.ServiceType == '代仓储') {
                return "-";
            }
            else {
                return val;
            }
        }

        function Reason(Summary) {
            $.messager.alert("退回原因", Summary);
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            switch (row.StatusValue) {
                case <%=Needs.Ccs.Services.Enums.ClientStatus.Confirmed.GetHashCode()%>:
                    //if (row.IsStorageValid == false && row.ServiceType == "双服务" && CurrentName == '平学考') {
                    //    buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:5px" onclick="StorageApprove(\'' + row.ID + '\')" group >' +
                    //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    //        '<span class="l-btn-text">代仓储业务审批</span>' +
                    //        '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    //        '</span>' +
                    //        '</a>';
                    //} else {
                    //if (row.DepartmentCode == '业务一部' && CurrentName == '张庆永') {
                    buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">编辑</span>' +
                        '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                    buttons += '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Assign(\'' + row.ID + '\',\'' + row.ServiceType + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">分配人员</span>' +
                        '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                    //}
                    //if (row.DepartmentCode == '业务二部' && CurrentName == '张令金') {
                    //    buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                    //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    //        '<span class="l-btn-text">编辑</span>' +
                    //        '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    //        '</span>' +
                    //        '</a>';
                    //    buttons += '<a id="btnAssign" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Assign(\'' + row.ID + '\',\'' + row.ServiceType + '\')" group >' +
                    //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    //        '<span class="l-btn-text">分配人员</span>' +
                    //        '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    //        '</span>' +
                    //        '</a>';
                    //}
                    //}
                    break;
                case <%=Needs.Ccs.Services.Enums.ClientStatus.Auditing.GetHashCode()%>||<%=Needs.Ccs.Services.Enums.ClientStatus.Returned.GetHashCode()%>:
                    if (row.RegisterDays >= 7 && row.IsSpecified != true) {
                        //if ((row.DepartmentCode == '业务二部' && CurrentName == '张令金') || (row.DepartmentCode == '业务一部' && CurrentName == '张庆永')) {
                        buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="AssignService(\'' + row.ID + '\')" group >' +
                            '<span class =\'l-btn-left l-btn-icon-left\'>' +
                            '<span class="l-btn-text">指定业务</span>' +
                            '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                            '</span>' +
                            '</a>';
                        //}
                    }
                    break;
                case <%=Needs.Ccs.Services.Enums.ClientStatus.WaitingApproval.GetHashCode()%>:
                    //if ((row.DepartmentCode == '业务二部' && CurrentName == '张令金') || (row.DepartmentCode == '业务一部' && CurrentName == '张庆永')) {
                    buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:5px" onclick="Approve(\'' + row.ID + '\',\'' + row.ServiceType + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">审批</span>' +
                        '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                    //}
                    //else if (row.ServiceType == "代仓储" && CurrentName == '平学考') {
                    //    buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:5px" onclick="StorageApprove(\'' + row.ID + '\')" group >' +
                    //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    //        '<span class="l-btn-text">代仓储业务审批</span>' +
                    //        '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    //        '</span>' +
                    //        '</a>';
                    //}
                    //else {
                    //    //双服务
                    //    if (row.IsValid == false && row.ServiceType == "双服务" && CurrentName == "张庆永") {
                    //        buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:5px" onclick="Approve(\'' + row.ID + '\')" group >' +
                    //            '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    //            '<span class="l-btn-text">报关业务审批</span>' +
                    //            '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    //            '</span>' +
                    //            '</a>';
                    //    }
                    //    if (row.IsStorageValid == false && row.ServiceType == "双服务" && CurrentName == '平学考') {
                    //        buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:5px" onclick="StorageApprove(\'' + row.ID + '\')" group >' +
                    //            '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    //            '<span class="l-btn-text">代仓储业务审批</span>' +
                    //            '<span class="l-btn-icon icon-man">&nbsp;</span>' +
                    //            '</span>' +
                    //            '</a>';
                    //    }
                    //}
                    break;

            }

            return buttons;
        }
        //加载报关业务类型
        function SetServiceType(val, row, index) {
            if (row.ServiceType == "代报关" || row.ServiceType == "双服务") {
                if (row.ServiceManagerName == null) {
                    var str = "-";
                }
                else {
                    var str = row.ServiceManagerName;
                }
                if (row.MerchandiserName == null) {
                    str += "/-";
                }
                else {
                    str += "/" + row.MerchandiserName;
                }
                if (row.ReferrerName == null) {
                    str += "/-";
                }
                else {
                    str += "/" + row.ReferrerName;
                }
            }
            else {
                var str = "";
            }
            return str;
        }
        //加载仓储业务类型
        function SetStorageServiceType(val, row, index) {
            if (row.ServiceType == "代仓储" || row.ServiceType == "双服务") {
                if (row.StorageServiceManagerName == null) {
                    var str = "-";
                }
                else {
                    var str = row.StorageServiceManagerName;
                }
                if (row.StorageMerchandiserName == null) {
                    str += "/-";
                }
                else {
                    str += "/" + row.StorageMerchandiserName;
                }
                if (row.StorageReferrerName == null) {
                    str += "/-";
                }
                else {
                    str += "/" + row.StorageReferrerName;
                }
            }
            else {
                var str = "";
            }
            return str;
        }
        //详情
        function Edit(id) {
            if (id) {
                var url = location.pathname.replace(/Approval\/List.aspx/ig, 'Index.aspx') + '?Source=Assign&ID=' + id;
                window.location = url;
            }
        }

        //详情
        function View(id) {
            if (id) {
                var url = location.pathname.replace(/Approval\/List.aspx/ig, 'Index.aspx') + '?Source=ApproveView&ID=' + id;
                window.location = url;
            }
        }


        function Approve(id, ServiceType) {
            if (ServiceType == '代仓储') {
                var url = location.pathname.replace(/List.aspx/ig, './Edit.aspx')
                    + '?ID=' + id;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '代报关业务审批',
                    width: 1000,
                    height: 780,
                    onClose: function () {
                        Search();
                    }
                });
            }
            else {
                //20230421 深圳-良宏总、金总要求，总额度超过100W 指定专人审批，暂定良宏总
                MaskUtil.mask();
                $.post('?action=CheckIsExcced', {
                    ID: id
                }, function (result) {
                    MaskUtil.unmask();
                    var rel = JSON.parse(result);
                    if (rel.success) {
                        //小于100W或有权限
                        var url = location.pathname.replace(/List.aspx/ig, './Edit.aspx')
                            + '?ID=' + id;
                        top.$.myWindow({
                            iconCls: "",
                            url: url,
                            noheader: false,
                            title: '代报关业务审批',
                            width: 1000,
                            height: 780,
                            onClose: function () {
                                Search();
                            }
                        });
                    } else {
                        //无权限
                        $.messager.alert('消息', '垫款超过额度，当前账号无审批权限', 'info', function () {

                        });
                    }
                });
            }
            //end 


            //var url = location.pathname.replace(/List.aspx/ig, './Edit.aspx')
            //    + '?ID=' + id;
            //top.$.myWindow({
            //    iconCls: "",
            //    url: url,
            //    noheader: false,
            //    title: '代报关业务审批',
            //    width: 1000,
            //    height: 780,
            //    onClose: function () {
            //        Search();
            //    }
            //});
        }

        function StorageApprove(id) {
            var url = location.pathname.replace(/List.aspx/ig, './StorageEdit.aspx')
                + '?ID=' + id;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '代仓储业务审批',
                width: 1000,
                height: 780,
                onClose: function () {
                    Search();
                }
            });
        }

        //指定业务员
        function AssignService(id) {
            if (id) {
                var url = location.pathname.replace(/List.aspx/ig, 'AssignServiceManager.aspx') + '?ID=' + id;
                top.$.myWindow({
                    iconCls: "icon-man",
                    url: url,
                    noheader: false,
                    title: '指定业务',
                    width: '400px',
                    height: '350px',
                    onClose: function () {
                        $('#clients').datagrid('reload');
                    }
                });
            }
        }

        //分配人员
        function Assign(id, serviceType) {
            if (id) {
                var url = location.pathname.replace(/List.aspx/ig, '../Assign/Assign.aspx') + '?ID=' + id + "&ServiceType=" + serviceType;;
                top.$.myWindow({
                    iconCls: "icon-man",
                    url: url,
                    noheader: false,
                    title: '分配人员',
                    width: '700px',
                    height: '450px',
                    onClose: function () {
                        $('#clients').datagrid('reload');
                    }
                });
            }
        }

        //查询
        function Search() {
            var CompanyName = $('#CompanyName').textbox('getValue'); 
            var ClientCode = $('#ClientCode').textbox('getValue');
            var UnPayExchangeAmount = $('#UnPayExchangeAmount').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var Status = $('#Status').combobox('getValue');
            var servicemanager = $("#ServiceManager").combobox('getValue');
            var DepartmentType = $("#DepartmentType").combobox('getValue');

            var claimStatus = false;
            var returnedStatus = false;
            var isSAEleUpload = false;
            if ($('#Unclaim').is(':checked')) { //内单
                claimStatus = true;
            }
            if ($('#HasReturn').is(':checked')) { //退回历史
                returnedStatus = true;
            }
            if ($('#IsSAEleUpload').is(':checked')) { //是否上传协议
                isSAEleUpload = true;
            }
            var parm = {
                CompanyName: CompanyName,
                ClientCode: ClientCode,
                UnPayExchangeAmount: UnPayExchangeAmount,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                Status: Status,
                Servicemanager: servicemanager,
                DepartmentType: DepartmentType,
                ClaimStatus: claimStatus,
                ReturnedStatus: returnedStatus,
                IsSAEleUpload: isSAEleUpload
            };
            $('#clients').myDatagrid('search', parm);


        }
        //重置查询条件
        function Reset() {
            $('#CompanyName').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#UnPayExchangeAmount').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#Status').combobox('setValue', null);
            $('#ServiceManager').combobox('setValue', null);
            $('#DepartmentType').combobox('setValue', null);
            $('#Unclaim').attr('checked', false);
            $('#HasReturn').attr('checked', false);
            $('#IsSAEleUpload').attr('checked', false);


            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">客户名称:</span>
                    <input class="easyui-textbox search" id="CompanyName" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" />
                    <span class="lbl">状态: </span>
                    <input class="easyui-combobox search" id="Status" />
                    <span class="lbl">超期未付汇金额: </span>
                    <input class="easyui-textbox" id="UnPayExchangeAmount" />
                    <input type="checkbox" name="Unclaimed" value="0" id="Unclaim" title="未认领" class="checkbox checkboxlist" /><label for="Unclaim" style="margin-right: 20px">未认领</label>
                    <input type="checkbox" name="HasReturned" value="0" id="HasReturn" title="退回历史" class="checkbox checkboxlist" /><label for="HasReturn" style="margin-right: 20px">退回历史</label>
                    <input type="checkbox" name="IsSAEleUploaded" value="0" id="IsSAEleUpload" title="协议未上传" class="checkbox checkboxlist" /><label for="IsSAEleUpload" style="margin-right: 20px">协议未上传</label>
                    <br />
                    <span class="lbl">创建日期:</span>
                    <input type="text" class="easyui-datebox search" id="CreateDateFrom" />
                    <span class="lbl">至: </span>
                    <input type="text" class="easyui-datebox search" id="CreateDateTo" />
                    <span class="lbl">业务员部门:</span>
                    <input class="easyui-combobox search" id="DepartmentType" />
                    <span class="lbl">业务员:</span>
                    <input class="easyui-combobox search" id="ServiceManager" />


                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="clients" data-options="singleSelect:true,border:false,fit:true,nowrap:false,scrollbarSize:0" title="会员列表" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 15%;">公司名称</th>
                    <th data-options="field:'ClientCode',align:'center'" style="width: 4%;">会员编号</th>
                    <th data-options="field:'ClientRank',align:'center'" style="width: 4%;">会员等级</th>
                    <th data-options="field:'ContactName',align:'center'" style="width: 5%;">联系人</th>
                    <th data-options="field:'ContactTel',align:'center'" style="width: 6%;">手机号码</th>
                    <th data-options="field:'DepartmentCode',align:'center'" style="width: 6%;">业务员部门</th>
                    <th data-options="field:'ServiceManagerName',align:'center'" style="width: 5%;">业务员</th>
                    <th data-options="field:'MerchandiserName',align:'center'" style="width: 5%;">跟单员</th>
                    <th data-options="field:'ReferrerName',align:'center'" style="width: 5%;">引荐人</th>
                    <%--                    <th data-options="field:'StorageDepartmentCode',align:'center'" style="width: 5.5%;">仓储业务员部门</th>
                    <th data-options="field:'StorageServiceManagerName',align:'left',formatter:SetStorageServiceType" style="width: 9%;">仓储业务员/跟单/引荐人</th>
                    <th data-options="field:'StorageMerchandiserName',align:'center'" style="width: 6%;">代仓储跟单员</th>
                    <th data-options="field:'StorageReferrerName',align:'center'" style="width: 6%;">代仓储引荐人</th>--%>
                    <th data-options="field:'ServiceType',align:'center'" style="width: 5%;">业务类型</th>
                    <th data-options="field:'CreateDate',align:'center',formatter:SetStyle" style="width: 6%;">注册时间</th>
                    <th data-options="field:'Status',align:'center',formatter:ViewSummary" style="width: 5%;">状态</th>
                    <th data-options="field:'UnPayExchangeAmount',align:'center',formatter:ViewUnPayExchange,sortable:true" style="width: 5%;">超期未换汇</th>
                    <th data-options="field:'DeclareAmount',align:'center',formatter:ViewDeclare,sortable:true" style="width: 5%;">近期报关</th>
                    <th data-options="field:'PayExchangeAmount',align:'center',formatter:ViewPayExchange,sortable:true" style="width: 5%;">近期付汇</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
