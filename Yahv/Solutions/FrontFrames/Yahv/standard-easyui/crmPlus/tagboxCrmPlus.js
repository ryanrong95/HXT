(function($){
    //接口地址
    var AjaxUrl = {
        getAdmins: '/crmplusapi/Admins/AdminLists'
    }

    
   $.fn.tagboxCrmPlus=function(opt,param){
       //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.tagboxCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("tagboxCrmPlus.该插件没有这个方法：" + opt)
            }
        }
        var options = opt || {};
        options = $.extend(true, {}, $.fn.tagboxCrmPlus.defaults, options);
         return this.each(function () {
             var sender = $(this);
            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'tagboxCrmPlus_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            var setTimeoutAjax = 0;
            //首次默认值查询


            var formName = sender.prop('name');
            if (!formName) {
                alert('tagboxCrmPlus.控件name不能为空！');
            }
            var value = sender.val();
            if (value) {
                options.value = value;
            }

                var initdata = [];
               
          
            //创建form返回值
            sender.tagbox({
                valueField: 'ID',
                textField: 'Name',
                mode: 'remote',
                prompt: options.prompt,
                required: options.required,
                value: options.value,
               // onChange: options.onChange,
                data: initdata,
                validType: 'tagboxCrmPlus["' + sender_id + '"]',
                onChange: function(newValue, oldValue) {
                if(newValue.length>0)
                 {
                var realname=$(this).tagbox("getValue");
                      

                  }
                    $.ajax({
                        async: false,
                        url: AjaxUrl.getAdmins,
                        dataType: 'json',
                        type: 'GET',
                        data: { name: JSON.stringify(realname) },
                        success: function (json) {
                                initdata = json.Data;
                                sender.data('items', initdata);
                               if (json.length == 1) {
                                    sender.tagbox('setValue', items[0].ID);
                                }
                                else {
                                    sender.tagbox('isValid');
                                }
                             
                        },
                        error: function (err) {
                            alert('tagboxCrmPlus.error:' + JSON.stringify(err));
                        }
                    });
                }
            });

            //结束
        });

   };
     $.fn.tagboxCrmPlus.defaults = {
        prompt: '请输入...', 
        lable:"添加一个Tag",
        required: false,
        limitToList: true,
        hasDownArrow: true,
        onChange: function (newValue, oldValue) {
        } //默认值也需要有意触发
    };

 //内部公司插件对外的方法
    $.fn.tagboxCrmPlus.methods = {
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },
        //获取数据
        setValue: function (jq, value) {
            var sender = $(jq);

            var oldValue = sender.tagbox('getValue');
            sender.tagbox('setValue', value);
            sender.tagbox('reload');
            var options = sender.data('options');

            if (options.onChange) {
                options.onChange(value, oldValue);
            }

            return sender;
        }
    };
  //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('tagboxCrmPlus');
})(jQuery);