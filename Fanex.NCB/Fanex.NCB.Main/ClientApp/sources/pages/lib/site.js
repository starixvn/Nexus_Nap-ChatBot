const _rootUrl = window.rootUrl || "/";
let _xsrfToken = null;
export default class Site {
    static getXsrfToken() {
        if (!_xsrfToken) {
            _xsrfToken = document.querySelector("input[name=__RequestVerificationToken]").value;
        }

        return _xsrfToken;
    }

    static resolveClientUrl(relativeUrl) {
        return _rootUrl + relativeUrl;
    }

    static convertDateToString(date) {
        return typeof date === "string" ? date : date.format("MM/DD/YYYY");
    }

    static asNull(text) {
        if (text != null) {
            text = text.trim();
            if (text === '') {
                return null;
            }
        }

        return text;
    }

    static downloadFile(data, fileName){
        const url = window.URL.createObjectURL(new Blob([data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        link.parentNode.removeChild(link);
    }
}