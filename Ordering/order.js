
//  $.feat.nativeTouchScroll=true;
$(document).ready(function () {
    $.ui.launch();

    order.bindEvents();
    

});

var order = {
    load: {
        page_detail_load: function () {
            $.ui.hide
        }
    },
    currentId: "",
    bindEvents: function () {
        addOrder.addEventListener("touchstart", function (ev) {
            $.ui.loadContent("page_detail_header");
        });
    }
}