
function _instanceof(left, right) { if (right != null && typeof Symbol !== "undefined" && right[Symbol.hasInstance]) { return right[Symbol.hasInstance](left); } else { return left instanceof right; } }

function _typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

/**
 * @file A simple and easy-to-use JS image compression tools
 * @author wangyulue(wangyulue@gmail.com)
 */
(function (factory) {
    window.imageConversion = factory();
})(function () {
    var methods = {};
    methods.urltoImage = function (url) {
        return new Promise(function (resolve, reject) {
            var img = new Image();

            img.onload = function () {
                return resolve(img);
            };

            img.onerror = function () {
                return reject(new Error('urltoImage(): Image failed to load, please check the image URL'));
            };

            img.src = url;
        });
    };
    methods.urltoBlob = function (url) {
        return fetch(url).then(function (response) {
            return response.blob();
        });
    };

    methods.imagetoCanvas = async function (image) {
        debugger;
        var config = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};
        var cvs = document.createElement('canvas');
        var ctx = cvs.getContext('2d');
        var height;
        var width; // 设置宽高

        for (var i in config) {
            if (Object.prototype.hasOwnProperty.call(config, i)) {
                config[i] = Number(config[i]);
            }
        }

        if (!config.scale) {
            width = config.width || config.height * image.width / image.height || image.width;
            height = config.height || config.width * image.height / image.width || image.height;
        } else {
            // 缩放比例0-10，不在此范围则保持原来图像大小
            var scale = config.scale > 0 && config.scale < 10 ? config.scale : 1;
            width = image.width * scale;
            height = image.height * scale;
        } // 当顺时针或者逆时针旋转90时，需要交换canvas的宽高


        if ([5, 6, 7, 8].some(function (i) {
            return i === config.orientation;
        })) {
            cvs.height = width;
            cvs.width = height;
        } else {
            cvs.height = height;
            cvs.width = width;
        } // 设置方向


        switch (config.orientation) {
            case 3:
                ctx.rotate(180 * Math.PI / 180);
                ctx.drawImage(image, -cvs.width, -cvs.height, cvs.width, cvs.height);
                break;

            case 6:
                ctx.rotate(90 * Math.PI / 180);
                ctx.drawImage(image, 0, -cvs.width, cvs.height, cvs.width);
                break;

            case 8:
                ctx.rotate(270 * Math.PI / 180);
                ctx.drawImage(image, -cvs.height, 0, cvs.height, cvs.width);
                break;

            case 2:
                ctx.translate(cvs.width, 0);
                ctx.scale(-1, 1);
                ctx.drawImage(image, 0, 0, cvs.width, cvs.height);
                break;

            case 4:
                ctx.translate(cvs.width, 0);
                ctx.scale(-1, 1);
                ctx.rotate(180 * Math.PI / 180);
                ctx.drawImage(image, -cvs.width, -cvs.height, cvs.width, cvs.height);
                break;

            case 5:
                ctx.translate(cvs.width, 0);
                ctx.scale(-1, 1);
                ctx.rotate(90 * Math.PI / 180);
                ctx.drawImage(image, 0, -cvs.width, cvs.height, cvs.width);
                break;

            case 7:
                ctx.translate(cvs.width, 0);
                ctx.scale(-1, 1);
                ctx.rotate(270 * Math.PI / 180);
                ctx.drawImage(image, -cvs.height, 0, cvs.height, cvs.width);
                break;

            default:
                ctx.drawImage(image, 0, 0, cvs.width, cvs.height);
        }

        return cvs;
    };
    /**
     * 将一个canvas对象转变为一个File（Blob）对象
     * 该方法可以做压缩处理
     *
     * @param {canvas} canvas
     * @param {number=} quality - 传入范围 0-1，表示图片压缩质量，默认0.92
     * @param {string=} type - 确定转换后的图片类型，选项有 "image/png", "image/jpeg", "image/gif",默认"image/jpeg"
     * @returns {Promise(Blob)}
     */


    methods.canvastoFile = function (canvas, quality) {
        var type = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : 'image/jpeg';
        return new Promise(function (resolve) {
            return canvas.toBlob(function (blob) {
                return resolve(blob);
            }, type, quality);
        });
    };
    /**
     * 将一个Canvas对象转变为一个dataURL字符串
     * 该方法可以做压缩处理
     *
     * @param {canvas} canvas
     * @param {number=} quality - 传入范围 0-1，表示图片压缩质量，默认0.92
     * @param {string=} type - 确定转换后的图片类型，选项有 "image/png", "image/jpeg", "image/gif",默认"image/jpeg"
     * @returns {Promise(string)} Promise含有一个dataURL字符串参数
     */


    methods.canvastoDataURL = async function (canvas, quality, type) {
        if (!checkImageType(type)) {
            type = 'image/jpeg';
        }

        return canvas.toDataURL(type, quality);
    };
    /**
     * 将File（Blob）对象转变为一个dataURL字符串
     *
     * @param {Blob} file
     * @returns {Promise(string)} Promise含有一个dataURL字符串参数
     */


    methods.filetoDataURL = function (file) {
        return new Promise(function (resolve) {
            var reader = new FileReader();

            reader.onloadend = function (e) {
                return resolve(e.target.result);
            };

            reader.readAsDataURL(file);
        });
    };
    /**
     * 将dataURL字符串转变为image对象
     *
     * @param {srting} dataURL - dataURL字符串
     * @returns {Promise(Image)}
     */


    methods.dataURLtoImage = function (dataURL) {
        return new Promise(function (resolve, reject) {
            var img = new Image();

            img.onload = function () {
                return resolve(img);
            };

            img.onerror = function () {
                return reject(new Error('dataURLtoImage(): dataURL is illegal'));
            };

            img.src = dataURL;
        });
    };
    /**
     * 将一个dataURL字符串转变为一个File（Blob）对象
     * 转变时可以确定File对象的类型
     *
     * @param {string} dataURL
     * @param {string=} type - 确定转换后的图片类型，选项有 "image/png", "image/jpeg", "image/gif"
     * @returns {Promise(Blob)}
     */


    methods.dataURLtoFile = async function (dataURL, type) {
        var arr = dataURL.split(',');
        var mime = arr[0].match(/:(.*?);/)[1];
        var bstr = atob(arr[1]);
        var n = bstr.length;
        var u8arr = new Uint8Array(n);

        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }

        if (checkImageType(type)) {
            mime = type;
        }

        return new Blob([u8arr], {
            type: mime
        });
    };
    /**
     * 将图片下载到本地
     *
     * @param {Blob} file - 一个File（Blob）对象
     * @param {string=} fileName - 下载后的文件名（可选参数，不传以时间戳命名文件）
     */


    methods.downloadFile = function (file, fileName) {
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(file);
        link.download = fileName || Date.now().toString(36);
        document.body.appendChild(link);
        var evt = document.createEvent('MouseEvents');
        evt.initEvent('click', false, false);
        link.dispatchEvent(evt);
        document.body.removeChild(link);
    };
    /** *以下是进一步封装** */

    /**
     * 压缩File（Blob）对象
     * @param {Blob} file - 一个File（Blob）对象
     * @param {(number|object)} config - 如果传入是number类型，传入范围 0-1，表示图片压缩质量,默认0.92；也可以传入object类型，以便更详细的配置
     * @example
     * 		imageConversion.compress(file,0.8)
     *
     * 		imageConversion.compress(file,{
     * 			quality: 0.8, //图片压缩质量
     * 			type："image/png", //转换后的图片类型，选项有 "image/png", "image/jpeg", "image/gif"
     * 			width: 300, //生成图片的宽度
     * 			height：200， //生产图片的高度
     * 			scale: 0.5， //相对于原始图片的缩放比率,设置config.scale后会覆盖config.width和config.height的设置；
     * 			orientation:2, //图片旋转方向
     * 		})
     *
     * @returns {Promise(Blob)}
     */


    methods.compress = async function (file) {
        var config = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};

        if (!_instanceof(file, Blob)) {
            throw new Error('compress(): First arg must be a Blob object or a File object.');
        }

        if (_typeof(config) !== 'object') {
            config = Object.assign({
                quality: config
            });
        }

        config.quality = Number(config.quality);

        if (Number.isNaN(config.quality)) {
            return file;
        }

        var dataURL = await methods.filetoDataURL(file);
        var originalMime = dataURL.split(',')[0].match(/:(.*?);/)[1]; // 原始图像图片类型

        var mime = 'image/jpeg'; // 默认压缩类型

        if (checkImageType(config.type)) {
            mime = config.type;
            originalMime = config.type;
        }

        var image = await methods.dataURLtoImage(dataURL);
        var canvas = await methods.imagetoCanvas(image, Object.assign({}, config));
        var compressDataURL = await methods.canvastoDataURL(canvas, config.quality, mime);
        var compressFile = await methods.dataURLtoFile(compressDataURL, originalMime);
        return compressFile;
    };
    /**
     * 根据体积压缩File（Blob）对象
     *
     * @param {Blob} file - 一个File（Blob）对象
     * @param {(number|object)} config - 如果传入是number类型，则指定压缩图片的体积,单位Kb;也可以传入object类型，以便更详细的配置
     * 		@param {number} size - 指定压缩图片的体积,单位Kb
     * 		@param {number} accuracy - 相对于指定压缩体积的精确度，范围0.8-0.99，默认0.95；
     *        如果设置 图片体积1000Kb,精确度0.9，则压缩结果为900Kb-1100Kb的图片都算合格；
     * @example
     *  	imageConversion.compress(file,100) //压缩后图片大小为100kb
     *
     * 		imageConversion.compress(file,{
     * 			size: 100, //图片压缩体积，单位Kb
     * 			accuracy: 0.9, //图片压缩体积的精确度，默认0.95
     * 			type："image/png", //转换后的图片类型，选项有 "image/png", "image/jpeg", "image/gif"
     * 			width: 300, //生成图片的宽度
     * 			height: 200, //生产图片的高度
     * 			scale: 0.5, //相对于原始图片的缩放比率,设置config.scale后会覆盖config.width和config.height的设置；
     * 			orientation:2, //图片旋转方向
     * 		})
     *
     * @returns {Promise(Blob)}
     */


    methods.compressAccurately = async function (file) {
        var config = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};

        if (!_instanceof(file, Blob)) {
            throw new Error('compressAccurately(): First arg must be a Blob object or a File object.');
        }

        if (_typeof(config) !== 'object') {
            config = Object.assign({
                size: config
            });
        } // 如果指定体积不是数字或者数字字符串，则不做处理


        config.size = Number(config.size);

        if (Number.isNaN(config.size)) {
            return file;
        } // 如果指定体积大于原文件体积，则不做处理；


        if (config.size * 1024 > file.size) {
            return file;
        }

        config.accuracy = Number(config.accuracy);

        if (!config.accuracy || config.accuracy < 0.8 || config.accuracy > 0.99) {
            config.accuracy = 0.95; // 默认精度0.95
        }

        var resultSize = {
            max: config.size * (2 - config.accuracy) * 1024,
            accurate: config.size * 1024,
            min: config.size * config.accuracy * 1024
        };
        var dataURL = await methods.filetoDataURL(file);
        var originalMime = dataURL.split(',')[0].match(/:(.*?);/)[1]; // 原始图像图片类型

        var mime = 'image/jpeg';

        if (checkImageType(config.type)) {
            mime = config.type;
            originalMime = config.type;
        } // const originalSize = file.size;
        // console.log('原始图像尺寸：', originalSize); //原始图像尺寸
        // console.log('目标尺寸：', config.size * 1024);
        // console.log('目标尺寸max：', resultSize.max);
        // console.log('目标尺寸min：', resultSize.min);


        var image = await methods.dataURLtoImage(dataURL);
        var canvas = await methods.imagetoCanvas(image, Object.assign({}, config));
        /**
         * 经过测试发现，blob.size与dataURL.length的比值约等于0.75
         * 这个比值可以同过dataURLtoFile这个方法来测试验证
         * 这里为了提高性能，直接通过这个比值来计算出blob.size
         */

        var proportion = 0.75;
        var imageQuality = 0.5;
        var compressDataURL;
        var tempDataURLs = [null, null];
        /**
         * HTMLCanvasElement.toBlob()以及HTMLCanvasElement.toDataURL()压缩参数
         * 的最小细粒度为0.01，而2的7次方为128，即只要循环7次，则会覆盖所有可能性
         */

        for (var x = 1; x <= 7; x++) {
            // console.group();
            // console.log("循环次数：", x);
            // console.log("当前图片质量", imageQuality);
            compressDataURL = await methods.canvastoDataURL(canvas, imageQuality, mime);
            var CalculationSize = compressDataURL.length * proportion; // console.log("当前图片尺寸", CalculationSize);
            // console.log("当前压缩率", CalculationSize / originalSize);
            // console.log("与目标体积偏差", CalculationSize / (config.size * 1024) - 1);
            // console.groupEnd();
            // 如果到循环第七次还没有达到精确度的值，那说明该图片不能达到到此精确度要求
            // 这时候最后一次循环出来的dataURL可能不是最精确的，需要取其周边两个dataURL三者比较来选出最精确的；

            if (x === 7) {
                if (resultSize.max < CalculationSize || resultSize.min > CalculationSize) {
                    compressDataURL = [compressDataURL].concat(tempDataURLs).filter(function (i) {
                        return i;
                    }) // 去除null
                        .sort(function (a, b) {
                            return Math.abs(a.length * proportion - resultSize.accurate) - Math.abs(b.length * proportion - resultSize.accurate);
                        })[0];
                }

                break;
            }

            if (resultSize.max < CalculationSize) {
                tempDataURLs[1] = compressDataURL;
                imageQuality -= 0.5 ** (x + 1);
            } else if (resultSize.min > CalculationSize) {
                tempDataURLs[0] = compressDataURL;
                imageQuality += 0.5 ** (x + 1);
            } else {
                break;
            }
        }

        var compressFile = await methods.dataURLtoFile(compressDataURL, originalMime); // console.log("最终图片大小：", compressFile.size);
        // 如果压缩后体积大于原文件体积，则返回源文件；

        if (compressFile.size > file.size) {
            return file;
        }

        return compressFile;
    };

    function checkImageType(type) {
        return ['image/png', 'image/jpeg', 'image/gif'].some(function (i) {
            return i === type;
        });
    }

    return methods;
});