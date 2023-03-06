import { mappingStatus, DateFormat, TimeFormat } from '../../constants/constants';
import Labels from '../Labels';
import { DateTime } from 'luxon';

export default class CommonFunctions {
    static getMappingClass(mappingStatusId) {
        switch (mappingStatusId) {
            case mappingStatus.BotRemoved:
                return 'badge-grey';
            case mappingStatus.NotMapped:
                return 'badge-red';
            case mappingStatus.Mapped:
                return 'badge-green';
            default:
                return '';
        }
    }

    static isBotRemoved(mappingStatusId) {
        return mappingStatusId === mappingStatus.BotRemoved;
    }

    static isMapped(mappingStatusId) {
        return mappingStatusId === mappingStatus.Mapped;
    }

    static getLicenseeStatus(recipientStatusNames) {
        return recipientStatusNames === '' ? 'N/A' : recipientStatusNames;
    }

    static isEmptyObject(obj) {
        return Object.keys(obj).length === 0;
    }

    static displayCopiedToClipboard() {
        const div = document.createElement('div');
        div.className = "copied-confirm";
        div.innerHTML = Labels.CopiedToClipboard;
        document.body.appendChild(div);
        setTimeout(() => {
            div.remove();
        }, 1000);
    }

    static convertTime2String(value) {
        const time = DateTime.fromISO(value);
        return `${time.toFormat(DateFormat)} at ${time.toFormat(TimeFormat)}`;
    }
    
    static debounce(func) {
        let timeout;
        return function () {
            const context = this,
                args = arguments;
    
            const later = function () {
                func.apply(context, args);
            };
    
            clearTimeout(timeout);
            timeout = setTimeout(later, 500);
        };
    }

    static renderPrivousIdTitle(jobId) {
        console.log(jobId);
        var title = 'Previous Max';
        var postFix = 'ID';

        if (jobId === 2) {
            postFix = 'Voiding History Collection ID'
        }
        if (jobId === 4 || jobId === 5) {
            postFix = 'MessageID';
        }
        return `${title} ${postFix}:`
    }
}