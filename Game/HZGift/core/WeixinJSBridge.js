var WXJS = {
    bridgeReady: function (callback) {
        var onBridgeReady = function () {
            if ($.isFunction(callback)) {
                callback();
            }
        }

        if (typeof WeixinJSBridge == "undefined") {
            if (document.addEventListener) {
                document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);
            } else if (document.attachEvent) {
                document.attachEvent('WeixinJSBridgeReady', onBridgeReady);
                document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);
            }
        } else {
            onBridgeReady();
        }
    },
    sendAppMessage: function (opts, callback) {
        WXJS.bridgeReady(function () {
            WeixinJSBridge.invoke('sendAppMessage', opts, function (res) {
                if ($.isFunction(callback)) {
                    callback(res);
                }
            });
        })
    },
    onSendAppMessage: function (opts, callback) {
        WXJS.bridgeReady(function () {
            WeixinJSBridge.on('menu:share:appmessage', function (argv) {
                WXJS.sendAppMessage(opts, callback);
            });
        });
    },
    addContact: function (opts, callback) {
        WXJS.bridgeReady(function () {
            var k = "";
            for (var key in WeixinJSBridge) {
                k += key + ",";
            }
            alert(k);
            WeixinJSBridge.invoke("addContact", opts, function (e) {
                if ($.isFunction(callback)) {
                    callback(e);
                }
            });
        });
    },
    hideOptionMenu: function () {
        WXJS.bridgeReady(function () {
            WeixinJSBridge.call('hideOptionMenu');
        });
    },
    imagePreview: function (urls, current) {
        WXJS.bridgeReady(function () {
            WeixinJSBridge.invoke("imagePreview", {
                "urls": urls,
                "current": current
            });
        });
    },
    jump: function () {
        WeixinJSBridge.invoke("jumpToBizProfile", {
            "tousername": "gh_4de04c099266"
        },
        function (e) {
            alert(e.err_msg);
        });
    },
    close: function () {
        WXJS.bridgeReady(function () {
            WeixinJSBridge.invoke('closeWindow', {}, function (res) {

                //alert(res.err_msg);

            });
        });
    }
}

