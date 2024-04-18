/// <reference path="../jquery-easyui-1.5.3/jquery.min.js" />
(function ($) {
    $.fn.bvfile = function (options) {
        var settings = $.extend({
            url: this.attr('url')
        }, options);
        var win_content = $('<p style="font-size:15px;"></p>');
        var win_id = 'win_456748954156456313';
        var validSize = function (input, maxLength) {
            if (!maxLength || isNaN(maxLength)) {
                return true;
            }
            for (var i = 0; i < input.files.length; i++) {
                var file = input.files[i];
                if (file.size > maxLength) {
                    $('#' + win_id).window({
                        content: file.name + '超过' + (maxLength / 1024) + "kb<br/>" + ",上传失败",
                        closable: true
                    });
                    return false;
                }
            }
            return true;
        }
        var validExtension = function (input, extensions) {
            if (!extensions) {
                return true;
            }
            var arry = extensions.split('|');
            for (var i = 0; i < input.files.length; i++) {
                var file = input.files[i], extension = file.name.substring(file.name.lastIndexOf('.') + 1).toLowerCase();
                if (arry.filter(function (item) { return extension == item; }).length == 0) {
                    $('#' + win_id).window({
                        content: file.name + '不是' + extensions + ",上传失败",
                        closable: true
                    });;
                    return false;
                }
            }
            return true;
        }
        this.parents('form').submit(function () {
            $('[type="file"]').each(function () {
                var input_file = $(this);
                var sname = input_file.prop('name');
                var name = sname + '_toback';
                var toback = $('input[name="' + name + '"]');
                toback.prop('name', sname);
                input_file.remove();
            });
        });
        this.filebox({
            onChange: function (newValue, oldValue) {
                var span = $(this).next('span');
                if ($('#' + win_id).length == 0) {
                    $('body').append('<div id="' + win_id + '"></div>');
                    $('#' + win_id).window({
                        title: '上传文件',
                        width: 300,
                        height: 100,
                        modal: true,
                        minimizable: false,
                        maximizable: false,
                        closable: false,
                        collapsible: false
                    });
                };

                var file = span.find('[type="file"]');
                //格式验证
                if (!validExtension(file[0], $(this).attr('extensions'))) {
                    return;
                }
                //大小验证                
                if (!validSize(file[0], $(this).attr('maxlength'))) {
                    return;
                }

                $('#' + win_id).window({ content: win_content.css('color', 'black').text('正在上传......') });
                var form = $('<form>');
                form.prop('enctype', 'multipart/form-data');
                form.prop('method', 'post');
                form.prop('action', settings.url);
                form.prop('style', 'display:none;');
                form.append(file.clone());
                $('body').append(form);
                form.form('submit', {
                    success: function (data) {
                        try {
                            data = /^\{|^\[/i.test(data) ? eval('(' + data + ')') : data;
                            $('#' + win_id).window({ content: win_content.css('color', 'green').text('上传成功'), closable: true });
                            var name = file.attr('name') + '_toback', hidden = $('[name="' + name + '"]');
                            if (hidden.length > 0) {
                                hidden.remove();
                            }
                            file.after('<input type="hidden" name="' + name + '" value="' + data + '">');
                        } catch (e) {
                            $('#' + win_id).window({ content: win_content.css('color', 'red').text('上传失败'), closable: true });
                        }
                        form.remove();
                    }
                });
            }
        });
        this.each(function () {
            var selctor = $(this);
            var value = selctor.attr('value');
            if (!!value) {
                selctor.filebox('setText', value);
                var file = selctor.next('span').find('[type="file"]')
                file.after('<input type="hidden" name="' + file.attr('name') + '_toback" value="' + value + '">');
            }
        });
    }
})(jQuery);

