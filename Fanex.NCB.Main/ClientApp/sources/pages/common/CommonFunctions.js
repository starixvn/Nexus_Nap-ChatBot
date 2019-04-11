import { mappingStatus } from '../constants/constants';

export function getMappingClass(mappingStatusId){
    switch (mappingStatusId) {
        case mappingStatus.BotRemoved:
            return "badge-grey";
        case mappingStatus.NotMapped:
            return "badge-red";
        case mappingStatus.Mapped:
            return "badge-green";
        default:
            return "";
    }
}

export function isBotRemoved(mappingStatusId) {
    return mappingStatusId === mappingStatus.BotRemoved;
}

export function getLicenseeStatus(recipientStatusNames) {
    return recipientStatusNames === '' ? 'N/A' : recipientStatusNames;
}