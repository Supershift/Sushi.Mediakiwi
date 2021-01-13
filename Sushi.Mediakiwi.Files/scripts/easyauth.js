(function () {
    var url = window.location.href;
    if (url.includes('#')) {
        var uri = url.split('#');
        var hash = uri[1].split('%22');
        if (hash.length > 2) {
            if (hash[1] === 'authenticationToken') {
                var token = hash[3];
                var date = new Date();
                date.setTime(date.getTime() + (30 * 1000));
                var expires = '; expires=' + date.toUTCString();
                document.cookie = '.authtoken=' + (token || '') + expires + '; path=/';
                window.location.replace(uri[0]);
            }
        }
    }
}());
