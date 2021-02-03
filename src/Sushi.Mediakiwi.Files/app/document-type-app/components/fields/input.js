export const shared = {    
    methods: {
        //expressCell(expression) {
        //    let _class = "";
        //    if (expression !== this.outputExpression.FullWidth)
        //        _class += "vhalf";
        //    else
        //        _class += "full";

        //    return _class;
        //},
        fieldClass(field) {
            let _class = '';//this.isHalfField(field.expression);
            if (field.className)
                _class += ` ${field.className}`;
            _class += this.errorClass(field);
            return _class;
        },
        errorClass(field) {
            if (field.error)
                return ' error';
            return ''
        },
        eventHandler(field, methodName) {
            let events = {};
            events[field.event] = methodName; // nameof the method
            return Object.entries(events).reduce((acc, [eventName, methodName]) => {
                acc[eventName] = this[methodName];
                return acc;
            }, {});
        },
        suffix(field) {
            return (typeof (field.suffix) !== 'undefined' && field.suffix) ? field.suffix : undefined;
        },
        prefix(field) {
            return (typeof (field.prefix) !== 'undefined' && field.prefix) ? field.prefix : undefined;
        },
    }
}