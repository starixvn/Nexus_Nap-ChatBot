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
        var title = 'Previous Max';
        var postFix = 'ID';

        if (jobId === 2) {
            postFix = 'Voiding History Collection ID'
        }
        if (jobId === 1 || jobId === 3) {
            postFix = 'MessageID';
        }
        if (jobId === 6 || jobId === 7) {
            postFix = 'MsgTargetID';
        }
        return `${title} ${postFix}:`
    }

    static blockUI() {
        document.getElementById('loadingProcess').style.display = 'block';
    }

    static unblockUI() {
        document.getElementById('loadingProcess').style.display = 'none';
    }

    static renderMaxMessageId (jobId, jobInfo) {
        switch (jobId) {
            case 1:
            case 3:
                return jobInfo.previousMaxMessageId;
            case 2:
                return jobInfo.lastVoidingHistoryCollectionId;
            case 4:
            case 5:
                return jobInfo.lastLogId;
            case 6:
            case 7:
                return jobInfo.lastMsgTargetId;
            default:
                return;
        }
    };

    static renderLabelMaxMessageId (jobId) {
        switch (jobId) {
            case 1:
            case 3:
                return 'previousMaxMessageId';
            case 2:
                return 'lastVoidingHistoryCollectionId';
            case 4:
            case 5:
                return 'lastLogId';
            case 6:
            case 7:
                return 'lastMsgTargetId';
            default:
                return;
        }
    };
}