export const mappingStatus = {
    'BotRemoved': 0,
    'NotMapped': 1,
    'Mapped': 2
};

export const PAGING_SIZES = [20, 50, 100, 500];

export const ErrorCodeTemplateMessage = [
    {
        code: 106,
        message: 'Recipient is not mapped or removed:'
    },
    {
        code: 107,
        message: 'Licensee is Inactive:'
    },
    {
        code: 108,
        message: 'Licensee is not offered:'
    },
    {
        code: 109,
        message: 'There are recipients in the exception list:'
    }
];

export const NotificationStatus = {
    Planned: 0,
    Sent: 1,
    Error: 2,
    Sending: 3,
    Cancelled: 4,
    Resolved: 5
}