$(function () {
    var container = document.getElementById('jsoneditor');

    var options = {
        mode: 'code',
        modes: ['code', 'form', 'text', 'tree', 'view'], // allowed modes
        error: function (err) {
            alert(err.toString());
        }
    };


    var editor = new JSONEditor(container, options);

    var resultEditor = new JSONEditor(document.getElementById('result'), {
        mode: "tree",
        modes: ['code', 'form', 'text', 'tree', 'view'], // allowed modes
    });

    var setResult = function (json) {
        resultEditor.set(json);
    }
    

    $("#sendRequest").click(function () {
        var requestUrl = $("#url").val();
        try {
            var json = editor.get();
            $.ajax({
                type: "post",
                url: "../Interface/SendRequest.ashx",
                dataType: "json",
                data: {
                    requestUrl: requestUrl,
                    content: encodeURI(JSON.stringify(json))
                },
                success: function (result) {
                    setResult(result);
                },
                error: function (a, b, c) {
                    setResult(a);
                }
            });
        }
        catch (e) {
            setResult("格式错误");
        }
    });

    $("#menu").click(function () {
        $("#url").val("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=ACCESS_TOKEN");
    });

    $("#userList").click(function () {
        $("#url").val("https://api.weixin.qq.com/cgi-bin/user/get?access_token=ACCESS_TOKEN");
    });

    $("#getMenu").click(function () {
        $("#url").val("https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info?access_token=ACCESS_TOKEN");
    });
});