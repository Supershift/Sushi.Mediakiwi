/*
 * Shorten, a jQuery plugin to automatically shorten text to fit in a block or a pre-set width and configure how the text ends.
 * Copyright (C) 2009-2011  Marc Diethelm
 * License: (GPL 3, http://www.gnu.org/licenses/gpl-3.0.txt) see license.txt
 */

(function (a) {
    function s(g, c) { return c.measureText(g).width }
    function t(g, c) {
        c.text(g);
        return c.width()
    }
    var q = false, o, j, k; a.fn.shorten = function () {
        var g = {},
        c = arguments, r = c.callee;
        if (c.length)
            if (c[0].constructor == Object) g = c[0]; else
                if (c[0] == "options") return a(this).eq(0).data("shorten-options");
                else g = {
                    width: parseInt(c[0]),
                    tail: c[1]
                };
        this.css("visibility", "hidden");
        var h = a.extend({},
        r.defaults, g);
        return this.each(function () {
            var e = a(this),
            d = e.text(),
            p = d.length, i, f = a("<span/>").html(h.tail).text(),
            l = { shortened: false, textOverflow: false };
            i = e.css("float") != "none" ? h.width || e.width() : h.width || e.parent().width();

            if (i < 0) return true; e.data("shorten-options", h);
            this.style.display = "block"; this.style.whiteSpace = "nowrap";
            if (o) {
                var b = a(this),
                n = document.createElement("canvas");
                ctx = n.getContext("2d");
                b.html(n);
                ctx.font = b.css("font-style") + " " + b.css("font-variant") + " " + b.css("font-weight") + " " + Math.ceil(parseFloat(b.css("font-size"))) + "px " + b.css("font-family");
                j = ctx; k = s
            }
            else {
                b = a('<table style="padding:0; margin:0; border:none; font:inherit;width:auto;zoom:1;position:absolute;"><tr style="padding:0; margin:0; border:none; font:inherit;"><td style="padding:0; margin:0; border:none; font:inherit;white-space:nowrap;"></td></tr></table>');
                $td = a("td", b);
                a(this).html(b);
                j = $td; k = t
            }
            b = k.call(this, d, j);

            if (b < i) {
                e.text(d);
                this.style.visibility = "visible"; e.data("shorten-info", l);
                return true
            }

            if (h.tooltip) {
                this.setAttribute("title", d);
                this.setAttribute("class", "dot");
            }

            if (r._native && !g.width) {
                n = a("<span>" + h.tail + "</span>").text();

                if (n.length == 1 && n.charCodeAt(0) == 8230) {
                    e.text(d);
                    this.style.overflow = "hidden"; this.style[r._native] = "ellipsis"; this.style.visibility = "visible"; l.shortened = true; l.textOverflow = "ellipsis"; e.data("shorten-info", l);
                    return true
                }
            }
            f = k.call(this, f, j);
            i -= f; f = i * 1.15;
            if (b - f > 0) {
                f = d.substring(0, Math.ceil(p * (f / b)));

                if (k.call(this, f, j) > i) { d = f; p = d.length }
            }
            do { p--; d = d.substring(0, p) }
            while (k.call(this, d, j) >= i);
            e.html(a.trim(a("<span/>").text(d).html()) + h.tail);
            this.style.visibility = "visible"; l.shortened = true; e.data("shorten-info", l);
            return true
        })
    };
    var m = document.documentElement.style;
    if ("textOverflow" in m) q = "textOverflow"; else
        if ("OTextOverflow" in m) q = "OTextOverflow";
    if (typeof Modernizr != "undefined" && Modernizr.canvastext) o = Modernizr.canvastext; else {
        m = document.createElement("canvas");
        o = !!(m.getContext && m.getContext("2d") && typeof m.getContext("2d").fillText === "function")
    }
    a.fn.shorten._is_canvasTextSupported = o; a.fn.shorten._native = q; a.fn.shorten.defaults = { tail: "&hellip;", tooltip: true }
})(jQuery);

