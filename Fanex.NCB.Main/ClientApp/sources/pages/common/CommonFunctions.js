import { mappingStatus, ErrorCodeTemplateMessage } from '../constants/constants';

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
}