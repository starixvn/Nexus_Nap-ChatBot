import { commands } from 'react-mde';

export const mappingStatus = {
    'BotRemoved': 0,
    'NotMapped': 1,
    'Mapped': 2
};

export const PAGING_SIZES = [20, 50, 100, 500];

export const NotificationStatus = {
    Planned: 0,
    Sent: 1,
    Error: 2,
    Sending: 3,
    Cancelled: 4,
    Resolved: 5,
    Draft: 9,
	Invalid: 10
};

export const DateFormat = 'MM/dd/yyyy';

export const TimeFormat = 'hh:mm:ss a';

export const DateTimeFormat = 'MM/dd/yyyy hh:mm:ss a';

export const DateTimeFormatV2 = 'dd/mm/yyyy hh:MM:ss TT';

export const DateTimeFormatV3 = 'mm/dd/yyyy hh:MM:ss TT';

export const DateTimeModified = 'yyyy-mm-dd HH:MM:ss';

export const NotAvailabeId = -1000;

export const editorCommandList = [
    {
        commands: [
            commands.boldCommand,
            commands.italicCommand,
            commands.strikeThroughCommand,
            commands.linkCommand
        ]
    }
];

export const invalidFieldClass = 'is-invalid';

export const defaultFromDate = '08/01/2019';

export const defaultPageSize = 20;

export const errorId = 2;

export const noErrorNotification = 'No error notification';

export const Channels = {
    Skype: 1,
    Telegram: 2
};

export const internalCSDJobId = 2;

export const LastVoidingHistoryCollectionId = 'LastVoidingHistoryCollectionId';

export const LastMsgTargetId = 'LastMsgTargetId';