$(document).ready(function () {


    function megaHoverOver() {
        $(this).find(".sub").show();

        //Calculate width of all ul's
        (function ($) {
            jQuery.fn.calcSubWidth = function () {
                rowWidth = 0;
                //Calculate row
                $(this).find("menu").each(function () {
                    rowWidth += $(this).width() + 7;
                });
            };
        })(jQuery);

        if ($(this).find(".row").length > 0) { //If row exists...
            var biggestRow = 0;
            //Calculate each row
            $(this).find(".row").each(function () {
                $(this).calcSubWidth();
                //Find biggest row
                if (rowWidth > biggestRow) {
                    biggestRow = rowWidth;
                }
            });
            //Set width
            $(this).find(".sub").css({ 'width':'220px'});
            $(this).find("menu.last").css({ 'margin': '-120px!important' });

        } else { //If row does not exist...

            $(this).calcSubWidth();
            //Set Width
            $(this).find(".sub").css({ 'width': rowWidth });

        }
    }

    function megaHoverOut() {
        $(this).find(".sub").hide();
    }


    var config = {
        sensitivity: 1, // number = sensitivity threshold (must be 1 or higher)    
        interval: 10, // number = milliseconds for onMouseOver polling interval    
        over: megaHoverOver, // function = onMouseOver callback (REQUIRED)    
        timeout: 0, // number = milliseconds delay before onMouseOut    
        out: megaHoverOut // function = onMouseOut callback (REQUIRED)    
    };

    $("menu#mainMenu li").hoverIntent(config);

});