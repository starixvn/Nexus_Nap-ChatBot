import { mappingStatus } from '../constants/constants';

export function getMappingClass(mappingStatusId){
    switch (mappingStatusId) {
        case mappingStatus.BotRemoved:
            return "badge-grey";
            break;
        case mappingStatus.NotMapped:
            return "badge-red";
            break;
        case mappingStatus.Mapped:
            return "badge-green";
            break;
        default:
            return "";
            break;
    }
}