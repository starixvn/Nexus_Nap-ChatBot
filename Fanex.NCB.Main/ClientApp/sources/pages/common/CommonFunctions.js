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