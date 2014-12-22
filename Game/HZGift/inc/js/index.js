$(function () {
    HZGift.getOpenId(function (openid) {
        HZGift.openid = openid;
        HZGift.refresh();
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
    refresh: function (callback) {
        HZGift.getUserInfo(HZGift.openid, function (data) {
            if (data.length > 0) {
                HZGift.userInfo = data[0];
                HZGift.buildSubscribeUser();
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
        if ($.isFunction(callback)) {
            callback($.getUrlParam("openid"));
        }

    },
    getPromoteId: function () {
        return $.getUrlParam("promoteid");
    },
    listenSendAppMessage: function () {
        var sendAppMessageOpts = {
            "appid": HZGift.openid,
            "img_url": window.location.origin + "/weixintest/Game/HZGift/themes/default/images/ChristmasTitle.jpg",
            "img_width": "200",
            "img_height": "200",
            "link": window.location.origin + window.location.pathname + "?promoteid=" + HZGift.openid,
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
            $.ajax({
                type: "post",
                url: "../../Interface/GetUserInfo.ashx",
                dataType: "json",
                data: {
                    openid: openid,
                    promoteId: HZGift.getPromoteId(),
                    subscribe: true
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
        else
        {
            if ($.isFunction(callback)) {
                callback([]);
            }
        }
    },
    buildLotteryObj: function () {
        var button = $("#lottery");
        if (button.length > 0) {
            button.remove();
        }
        button = $('<button id="lottery" class="large red button">开始抽奖</button>').prependTo($("div.info"));
        button.click(function (e) {
            $(e.currentTarget).attr("disabled", "disabled");
            $(e.currentTarget).text("等待...");
            HZGift.lottery(function (data) {
                alert(data);
                HZGift.refresh();
            });
        });

        var label = $("#lotteryCount");
        if (label.length == 0) {
            label = $('<label id="lotteryCount"/>').appendTo($("div.info"));
        }

        var countNum = 0;
        if (HZGift.userInfo) {
            countNum = parseInt(HZGift.userInfo["lotteryChance"]) -
            parseInt(HZGift.userInfo["lotteryCount"]);
        }

        label.text("剩余次数:" + countNum);
    },
    lottery: function (callback) {
        $.ajax({
            type: "post",
            url: "../../Interface/Lottery.ashx",
            dataType: "json",
            data: {
                openid: HZGift.openid
            },
            success: function (result) {
                if ($.isFunction(callback)) {
                    callback(result.data);
                }
            },
            error: function (a, b, c) {
                alert(a + b + c);
            }
        });
    },
    getGift: function () {

    },
    buildSubscribeUser: function () {
        HZGift.buildLotteryObj();
        //<button class="large red button">开始抽奖</button><label>剩余次数:</label><label>0</label>
    },
    buildNotSubscribeUser: function () {
        HZGift.userInfo = undefined;
    }
};

(function ($) {
    $.getUrlParam = function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return null;
    }
})(jQuery);