export const shared = {
    data() {
        return {
            // Corresponding to the mediakiwi enum
            outputExpression: Object.freeze({
                FullWidth: 0,
                Left: 1,
                Right: 2,
                Alternating: 3
            })
        };
    },
    methods: {
        concat() {
            let _concat = "";

            for (var i = 0; i < arguments.length; i++)
                _concat += arguments[i];

            return _concat;
        },
        isHalfField(expression) {
            let _class = "";
            if (expression !== this.outputExpression.FullWidth)
                _class += " half";
            else
                _class += " long";

            return _class;
        },
        expressCell(expression) {
            let _class = "";
            if (expression !== this.outputExpression.FullWidth)
                _class += "vhalf";
            else
                _class += "full";

            return _class;
        },
        expressionClass(expression) {
            let _class = this.isHalfField(expression);

            switch (expression) {
                case 1:
                    _class += " left";
                    break;
                case 2:
                    _class += " right";
                    break;
            }
            return _class;
        },
        fieldClass(field) {
            let _class = this.isHalfField(field.expression);
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
        buttonClass(button) {
            let _class = "abbr submit";
            if (button.className)
                _class += ` ${button.className}`;
            return _class;
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
        askConfirm(label) {
            
        }
    }
}