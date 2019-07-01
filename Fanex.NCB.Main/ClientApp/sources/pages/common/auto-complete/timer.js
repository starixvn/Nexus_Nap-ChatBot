const DEFAULT_TIME_DELAY_MILLISECOND = 500;

export default class Timer {
    static debounce(func, wait) {
        let timeout;
        return function (...theArgs) {
            const context = this,
            args = theArgs;

            const later = () => {
                func.apply(context, args);
            };

            clearTimeout(timeout);
            timeout = setTimeout(later, wait || DEFAULT_TIME_DELAY_MILLISECOND);
        };
    }
}