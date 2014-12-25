$(function () {
    HZGift.getOpenId(function (openid) {
        HZGift.init(function () {
            HZGift.listenSendAppMessage();
        });
    });


    HZGift.layout();
    $(window).resize(function () {
        HZGift.layout();
    });

    makeSnow("snow");
});


var HZGift = {
    openid: "",
    promoteId: "",
    userInfo: undefined,
    infoDiv: '<div class="info popup_box delay"/>',
    init: function (callback) {
        $("div.info").remove();
        $("div.santaGift").remove();
        $("#presentBox").remove();

        HZGift.getUserInfo(HZGift.openid, function (data) {
            if (data.length > 0) {
                HZGift.userInfo = data[0];

                if (HZGift.userInfo.subscribe === true) {
                    HZGift.buildSubscribeUser();
                }
                else {
                    HZGift.buildNotSubscribeUser();
                }
            }
            else {
                HZGift.userInfo = undefined;
                HZGift.buildNotSubscribeUser();
            }

            if ($.isFunction(callback)) {
                callback();
            }
        });
    },
    update: function () {
        HZGift.getUserInfo(HZGift.openid, function (data) {
            if (data.length > 0) {
                HZGift.userInfo = data[0];
                if (HZGift.userInfo.subscribe === true) {
                    $("div.info").children("button").prop("disabled", false).text("开始抽奖");
                }
            }
        });
    },
    layout: function () {
        $("body").height(document.documentElement.clientHeight);
        var giftDiv = $("div.gift");
        var width = giftDiv.width();
        giftDiv.height(width);


        canvas = document.getElementById("snow");
        canvas.width = width;
        canvas.height = width;
    },
    buttonShowHide: function (sub) {
        if (sub) {
            $("#subscribe").css("display", "none");
            $("#send").css("display", "block");
        }
        else {
            $("#subscribe").css("display", "block");
            $("#send").css("display", "none");
        }
    },
    getOpenId: function (callback) {
        var openid = $.getCookie("openid");
        if (openid) {
            HZGift.openid = openid;
            $.setCookie("openid", HZGift.openid, 30);
            if ($.isFunction(callback)) {
                callback();
            }
        }
        else {
            var paramCallback = function () {
                HZGift.openid = $.getUrlParam("openid");
                $.setCookie("openid", HZGift.openid, 30);
                if ($.isFunction(callback)) {
                    callback();
                }
            };
            var code = $.getUrlParam("code");
            if (code) {
                $.ajax({
                    type: "post",
                    url: "../../Interface/GetopenidBycode.ashx",
                    dataType: "json",
                    data: {
                        code: $.getUrlParam("code")
                    },
                    success: function (result) {
                        if (result.data) {
                            HZGift.openid = result.data;
                            $.setCookie("openid", HZGift.openid, 30);
                            if ($.isFunction(callback)) {
                                callback();
                            }
                        }
                        else {
                            paramCallback();
                        }
                    },
                    error: function (a, b, c) {
                        alert(a + b + c);
                        paramCallback();
                    }
                });
            }
            else {
                paramCallback();
            }
        }
    },
    getPromoteId: function () {
        return $.getUrlParam("promoteId");
    },
    listenSendAppMessage: function () {
        var ext = "";
        if (HZGift.userInfo) {
            ext = "?promoteId=" + HZGift.userInfo.id;
        }
        var sendAppMessageOpts = {
            //"appid": "",
            "img_url": window.location.origin + "/weixintest/Game/HZGift/themes/default/images/ChristmasTitle.jpg",
            "img_width": "200",
            "img_height": "200",
            "link": window.location.origin + "/weixintest/Interface/Promote.ashx" + ext,
            "desc": "通过连接进入抽奖，朋友与您均获得抽奖机会",
            "title": "好友推荐"
        };
        var sendAppMessageCallback = function (re) {
            alert(JSON.stringify(re) + "@" + sendAppMessageOpts.link);
        };
        WXJS.onSendAppMessage(sendAppMessageOpts, sendAppMessageCallback);
    },
    getUserInfo: function (openid, callback) {
        if (openid) {
            var promoteId = HZGift.getPromoteId();
            $.ajax({
                type: "post",
                url: "../../Interface/GetUserInfo.ashx",
                dataType: "json",
                data: {
                    openid: openid,
                    promoteId: promoteId
                },
                success: function (result) {
                    if ($.isFunction(callback)) {
                        callback(result.data);
                    }
                },
                error: function (a, b, c) {
                    alert(a + b + c);
                    if ($.isFunction(callback)) {
                        callback([]);
                    }
                }
            });
        }
        else {
            if ($.isFunction(callback)) {
                callback([]);
            }
        }
    },
    buildLotteryObj: function () {
        var infoDiv = $(HZGift.infoDiv);

        button = $('<button class="large red button">开始抽奖</button>').prependTo(infoDiv);
        button.bind("touchstart", function (e) {
            $(e.currentTarget).prop("disabled", true);
            HZGift.lottery("", function (data) {
                if (!data) {
                    $(e.currentTarget).text("点击礼物袋");
                    HZGift.buildGift();
                }
                else {
                    alert(data);
                    $(e.currentTarget).prop("disabled", false);
                    $(e.currentTarget).text("开始抽奖");
                }
            });
        });

        var label = $("#lotteryCount");
        if (label.length == 0) {
            label = $('<label id="lotteryCount"/>').appendTo(infoDiv);
        }

        var countNum = 0;
        if (HZGift.userInfo) {
            countNum = parseInt(HZGift.userInfo["lotteryChance"]) -
            parseInt(HZGift.userInfo["lotteryCount"]);
        }

        label.text("剩余次数:" + countNum);

        infoDiv.append('<div id="send" class="large blue button">' +
        '通过右上角菜单发送给朋友，朋友初次抽奖后，您将获取更多的抽奖机会</div>');
        infoDiv.appendTo($("body"));
    },
    lottery: function (action, callback) {
        $.ajax({
            type: "post",
            url: "../../Interface/Lottery.ashx",
            dataType: "json",
            data: {
                openid: HZGift.openid,
                action: action
            },
            success: function (result) {
                if ($.isFunction(callback)) {
                    callback(result.data);
                }
            },
            error: function (a, b, c) {
                alert(a + b + c);
                if (action == "get") {
                    if ($.isFunction(callback)) {
                        callback("");
                    }
                }
            }
        });
    },
    getGift: function (callback) {
        HZGift.lottery("get", function (result) {
            if ($.isFunction(callback)) {
                callback(result);
            }
        });
    },
    buildSubscribeUser: function () {
        HZGift.buildLotteryObj();
        //<button class="large red button">开始抽奖</button><label>剩余次数:</label><label>0</label>
    },
    buildNotSubscribeUser: function () {
        HZGift.userInfo = undefined;
        var infoDiv = $(HZGift.infoDiv);
        infoDiv.append('<div id="send" class="large orangellow button">' +
        '通过右上角查看公众号菜单关注，获取抽奖机会</div>');
        infoDiv.appendTo($("body"));
    },
    buildGift: function () {
        $("div.santaGift").remove();
        $("#presentBox").remove();
        var divSantaBottom = $('<div class="santaGift popup_box"/>');
        var width = $("div.gift").width() / 2;
        divSantaBottom.width(width);
        divSantaBottom.height(width);
        var $santaCanvas = $('<canvas width="' + width + '" height="' + width + '">浏览器不支持canvas</canvas>').appendTo(divSantaBottom);
        var santaContext = $santaCanvas[0].getContext("2d");
        santaContext.globalCompositeOperation = "destination-over";
        var image = new Image();
        image.src = "themes/default/images/santa.png";
        image.onload = function () {
            santaContext.drawImage(this, 0, 0, this.width, this.height, 0, 0, width, width);
        }

        var divGiftBottom = $('<div class="santaGift popup_box"/>');
        var $giftCanvas = $('<canvas width="' + width + '" height="' + width + '">浏览器不支持canvas</canvas>').appendTo(divGiftBottom);
        var giftContext = $giftCanvas[0].getContext("2d");
        giftContext.globalCompositeOperation = "destination-over";
        image = new Image();
        image.src = "themes/default/images/gift.png";
        image.onload = function () {
            giftContext.drawImage(this, 0, 0, this.width, this.height, 0, 0, width, width);
        }

        var getting = false;

        $giftCanvas.bind("touchstart", function (e) {
            if (getting) {
                return;
            }
            else {
                getting = true;
                divSantaBottom[0].addEventListener("webkitAnimationEnd", function (animation) {
                    if (animation.animationName == "remove") {
                        animation.target.parentNode.removeChild(animation.target);
                    }
                });

                divGiftBottom[0].addEventListener("webkitAnimationEnd", function (animation) {
                    if (animation.animationName == "remove") {
                        animation.target.parentNode.removeChild(animation.target);
                    }
                    HZGift.getGift(function (result) {
                        if (result) {
                            try {
                                var num = parseInt(result);
                                if (isNaN(num)) {
                                    alert(result);
                                    getting = false;
                                    return;
                                }
                                var divPresent = $('<div id="presentBox" class="santaGift present popup_box clickable"/>');
                                image = new Image();
                                image.src = "themes/default/images/present.png";
                                image.onload = function () {
                                    divPresent.width(this.width);
                                    divPresent.height(this.height);
                                    var $presentCanvas = $('<canvas width="' + this.width + '" height="' + this.height + '">浏览器不支持canvas</canvas>').appendTo(divPresent);
                                    var presentContext = $presentCanvas[0].getContext("2d");
                                    presentContext.drawImage(this, 0, 0, this.width, this.height, 0, 0, this.width, this.height);

                                    divPresent[0].addEventListener("webkitAnimationEnd", function (animation) {
                                        if (animation.animationName == "remove") {
                                            animation.target.parentNode.removeChild(animation.target);
                                            alert("恭喜获取" + num + "等奖");
                                            HZGift.update();
                                        }
                                    });
                                    divPresent.appendTo($("div.gift"));
                                    var done = false;
                                    divPresent[0].addEventListener("touchstart", function () {
                                        if (!done) {
                                            done = true;
                                            divPresent.addClass("remove");
                                        }
                                    });
                                }
                            }
                            catch (ex) {
                                alert(ex.message);
                                getting = false;
                            }
                        }
                    });
                });

                divSantaBottom.addClass("remove");
                divGiftBottom.addClass("remove");


            }
        });

        divSantaBottom.appendTo($("div.gift"));

        divGiftBottom.appendTo($("div.gift"));
    }
};

(function ($) {
    $.getUrlParam = function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return "";
    }

    $.getCookie = function (c_name) {
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=")
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1
                c_end = document.cookie.indexOf(";", c_start)
                if (c_end == -1) c_end = document.cookie.length
                return unescape(document.cookie.substring(c_start, c_end))
            }
        }
        return ""
    }

    $.setCookie = function (c_name, value, expiredays) {
        var exdate = new Date()
        exdate.setDate(exdate.getDate() + expiredays)
        document.cookie = c_name + "=" + escape(value) +
        ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
    }
})(jQuery);

