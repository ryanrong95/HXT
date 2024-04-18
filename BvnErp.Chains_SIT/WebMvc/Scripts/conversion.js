
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
        var width; // ���ÿ��

        for (var i in config) {
            if (Object.prototype.hasOwnProperty.call(config, i)) {
                config[i] = Number(config[i]);
            }
        }

        if (!config.scale) {
            width = config.width || config.height * image.width / image.height || image.width;
            height = config.height || config.width * image.height / image.width || image.height;
        } else {
            // ���ű���0-10�����ڴ˷�Χ�򱣳�ԭ��ͼ���С
            var scale = config.scale > 0 && config.scale < 10 ? config.scale : 1;
            width = image.width * scale;
            height = image.height * scale;
        } // ��˳ʱ�������ʱ����ת90ʱ����Ҫ����canvas�Ŀ��


        if ([5, 6, 7, 8].some(function (i) {
            return i === config.orientation;
        })) {
            cvs.height = width;
            cvs.width = height;
        } else {
            cvs.height = height;
            cvs.width = width;
        } // ���÷���


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
     * ��һ��canvas����ת��Ϊһ��File��Blob������
     * �÷���������ѹ������
     *
     * @param {canvas} canvas
     * @param {number=} quality - ���뷶Χ 0-1����ʾͼƬѹ��������Ĭ��0.92
     * @param {string=} type - ȷ��ת�����ͼƬ���ͣ�ѡ���� "image/png", "image/jpeg", "image/gif",Ĭ��"image/jpeg"
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
     * ��һ��Canvas����ת��Ϊһ��dataURL�ַ���
     * �÷���������ѹ������
     *
     * @param {canvas} canvas
     * @param {number=} quality - ���뷶Χ 0-1����ʾͼƬѹ��������Ĭ��0.92
     * @param {string=} type - ȷ��ת�����ͼƬ���ͣ�ѡ���� "image/png", "image/jpeg", "image/gif",Ĭ��"image/jpeg"
     * @returns {Promise(string)} Promise����һ��dataURL�ַ�������
     */


    methods.canvastoDataURL = async function (canvas, quality, type) {
        if (!checkImageType(type)) {
            type = 'image/jpeg';
        }

        return canvas.toDataURL(type, quality);
    };
    /**
     * ��File��Blob������ת��Ϊһ��dataURL�ַ���
     *
     * @param {Blob} file
     * @returns {Promise(string)} Promise����һ��dataURL�ַ�������
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
     * ��dataURL�ַ���ת��Ϊimage����
     *
     * @param {srting} dataURL - dataURL�ַ���
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
     * ��һ��dataURL�ַ���ת��Ϊһ��File��Blob������
     * ת��ʱ����ȷ��File���������
     *
     * @param {string} dataURL
     * @param {string=} type - ȷ��ת�����ͼƬ���ͣ�ѡ���� "image/png", "image/jpeg", "image/gif"
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
     * ��ͼƬ���ص�����
     *
     * @param {Blob} file - һ��File��Blob������
     * @param {string=} fileName - ���غ���ļ�������ѡ������������ʱ��������ļ���
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
    /** *�����ǽ�һ����װ** */

    /**
     * ѹ��File��Blob������
     * @param {Blob} file - һ��File��Blob������
     * @param {(number|object)} config - ���������number���ͣ����뷶Χ 0-1����ʾͼƬѹ������,Ĭ��0.92��Ҳ���Դ���object���ͣ��Ա����ϸ������
     * @example
     * 		imageConversion.compress(file,0.8)
     *
     * 		imageConversion.compress(file,{
     * 			quality: 0.8, //ͼƬѹ������
     * 			type��"image/png", //ת�����ͼƬ���ͣ�ѡ���� "image/png", "image/jpeg", "image/gif"
     * 			width: 300, //����ͼƬ�Ŀ��
     * 			height��200�� //����ͼƬ�ĸ߶�
     * 			scale: 0.5�� //�����ԭʼͼƬ�����ű���,����config.scale��Ḳ��config.width��config.height�����ã�
     * 			orientation:2, //ͼƬ��ת����
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
        var originalMime = dataURL.split(',')[0].match(/:(.*?);/)[1]; // ԭʼͼ��ͼƬ����

        var mime = 'image/jpeg'; // Ĭ��ѹ������

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
     * �������ѹ��File��Blob������
     *
     * @param {Blob} file - һ��File��Blob������
     * @param {(number|object)} config - ���������number���ͣ���ָ��ѹ��ͼƬ�����,��λKb;Ҳ���Դ���object���ͣ��Ա����ϸ������
     * 		@param {number} size - ָ��ѹ��ͼƬ�����,��λKb
     * 		@param {number} accuracy - �����ָ��ѹ������ľ�ȷ�ȣ���Χ0.8-0.99��Ĭ��0.95��
     *        ������� ͼƬ���1000Kb,��ȷ��0.9����ѹ�����Ϊ900Kb-1100Kb��ͼƬ����ϸ�
     * @example
     *  	imageConversion.compress(file,100) //ѹ����ͼƬ��СΪ100kb
     *
     * 		imageConversion.compress(file,{
     * 			size: 100, //ͼƬѹ���������λKb
     * 			accuracy: 0.9, //ͼƬѹ������ľ�ȷ�ȣ�Ĭ��0.95
     * 			type��"image/png", //ת�����ͼƬ���ͣ�ѡ���� "image/png", "image/jpeg", "image/gif"
     * 			width: 300, //����ͼƬ�Ŀ��
     * 			height: 200, //����ͼƬ�ĸ߶�
     * 			scale: 0.5, //�����ԭʼͼƬ�����ű���,����config.scale��Ḳ��config.width��config.height�����ã�
     * 			orientation:2, //ͼƬ��ת����
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
        } // ���ָ������������ֻ��������ַ�������������


        config.size = Number(config.size);

        if (Number.isNaN(config.size)) {
            return file;
        } // ���ָ���������ԭ�ļ��������������


        if (config.size * 1024 > file.size) {
            return file;
        }

        config.accuracy = Number(config.accuracy);

        if (!config.accuracy || config.accuracy < 0.8 || config.accuracy > 0.99) {
            config.accuracy = 0.95; // Ĭ�Ͼ���0.95
        }

        var resultSize = {
            max: config.size * (2 - config.accuracy) * 1024,
            accurate: config.size * 1024,
            min: config.size * config.accuracy * 1024
        };
        var dataURL = await methods.filetoDataURL(file);
        var originalMime = dataURL.split(',')[0].match(/:(.*?);/)[1]; // ԭʼͼ��ͼƬ����

        var mime = 'image/jpeg';

        if (checkImageType(config.type)) {
            mime = config.type;
            originalMime = config.type;
        } // const originalSize = file.size;
        // console.log('ԭʼͼ��ߴ磺', originalSize); //ԭʼͼ��ߴ�
        // console.log('Ŀ��ߴ磺', config.size * 1024);
        // console.log('Ŀ��ߴ�max��', resultSize.max);
        // console.log('Ŀ��ߴ�min��', resultSize.min);


        var image = await methods.dataURLtoImage(dataURL);
        var canvas = await methods.imagetoCanvas(image, Object.assign({}, config));
        /**
         * �������Է��֣�blob.size��dataURL.length�ı�ֵԼ����0.75
         * �����ֵ����ͬ��dataURLtoFile���������������֤
         * ����Ϊ��������ܣ�ֱ��ͨ�������ֵ�������blob.size
         */

        var proportion = 0.75;
        var imageQuality = 0.5;
        var compressDataURL;
        var tempDataURLs = [null, null];
        /**
         * HTMLCanvasElement.toBlob()�Լ�HTMLCanvasElement.toDataURL()ѹ������
         * ����Сϸ����Ϊ0.01����2��7�η�Ϊ128����ֻҪѭ��7�Σ���Ḳ�����п�����
         */

        for (var x = 1; x <= 7; x++) {
            // console.group();
            // console.log("ѭ��������", x);
            // console.log("��ǰͼƬ����", imageQuality);
            compressDataURL = await methods.canvastoDataURL(canvas, imageQuality, mime);
            var CalculationSize = compressDataURL.length * proportion; // console.log("��ǰͼƬ�ߴ�", CalculationSize);
            // console.log("��ǰѹ����", CalculationSize / originalSize);
            // console.log("��Ŀ�����ƫ��", CalculationSize / (config.size * 1024) - 1);
            // console.groupEnd();
            // �����ѭ�����ߴλ�û�дﵽ��ȷ�ȵ�ֵ����˵����ͼƬ���ܴﵽ���˾�ȷ��Ҫ��
            // ��ʱ�����һ��ѭ��������dataURL���ܲ����ȷ�ģ���Ҫȡ���ܱ�����dataURL���߱Ƚ���ѡ���ȷ�ģ�

            if (x === 7) {
                if (resultSize.max < CalculationSize || resultSize.min > CalculationSize) {
                    compressDataURL = [compressDataURL].concat(tempDataURLs).filter(function (i) {
                        return i;
                    }) // ȥ��null
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

        var compressFile = await methods.dataURLtoFile(compressDataURL, originalMime); // console.log("����ͼƬ��С��", compressFile.size);
        // ���ѹ�����������ԭ�ļ�������򷵻�Դ�ļ���

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