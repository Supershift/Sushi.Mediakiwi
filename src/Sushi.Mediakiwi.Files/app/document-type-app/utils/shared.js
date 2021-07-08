export const shared = {
    data() {
        return {
        };
    },
    methods: {
        concat() {
            let _concat = "";

            for (var i = 0; i < arguments.length; i++)
                _concat += arguments[i];

            return _concat;
        },
        //fieldClass(field) {
        //    let _class = '';//this.isHalfField(field.expression);
        //    if (field.className)
        //        _class += ` ${field.className}`;
        //    _class += this.errorClass(field);
        //    return _class;
        //},
        //errorClass(field) {
        //    if (field.error)
        //        return ' error';
        //    return ''
        //},
        //buttonClass(button) {
        //    let _class = "abbr submit";
        //    if (button.className)
        //        _class += ` ${button.className}`;
        //    return _class;
        //},
        //eventHandler(field, methodName) {
        //    let events = {};
        //    events[field.event] = methodName; // nameof the method
        //    return Object.entries(events).reduce((acc, [eventName, methodName]) => {
        //        acc[eventName] = this[methodName];
        //        return acc;
        //    }, {});
        //},
        askConfirm(label) {
            // ??   
        }
    }
}