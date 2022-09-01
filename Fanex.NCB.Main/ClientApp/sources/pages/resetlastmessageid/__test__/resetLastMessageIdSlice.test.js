import resetLastMessageId, {
    setSelected,
    setCurrentPreviousMaxMessage,
    setNewPreviousMaxMessage,
    setMissingMessageCsd,
    setMissingMessageInfo,
    setMissingProblemAccount,
    setMissingMonitoredPlayer,
    setEditNewMessagePopup
} from '../resetLastMessageIdSlice';

describe('ResetLastMessageIdSlice reducer', () => {
    it('should handle initial state', () => {
        expect(resetLastMessageId(undefined, {})).toEqual({
            selected: [
                { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 },
                { id: 4, name: 'Problem Player', checked: true, newMaxMessageId: 0 },
                { id: 5, name: 'Monitored Player', checked: true, newMaxMessageId: 0 }
            ],
            currentMessages: null,
            newMessages: null,
            missingMessageCsd: null,
            missingMessages: null,
            missingProblemAccount: null,
            missingMonitoredPlayer: null,
            editNewMessagePopup: { show: false }
        });
    });

    it('should handle setSelected', () => {
        expect(
            resetLastMessageId({}, {
                type: setSelected.type,
                payload: [{ id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 }]
            })
        ).toEqual({
            selected: [{ id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 }]
        });
    });

    it('should handle currentMessages', () => {
        expect(
            resetLastMessageId({}, {
                type: setCurrentPreviousMaxMessage.type,
                payload: [{ jobIdentify: 1, messageId: 1074533320 }]
            })
        ).toEqual({
            currentMessages: [{ jobIdentify: 1, messageId: 1074533320 }]
        });
    });

    it('should handle newMessages', () => {
        expect(
            resetLastMessageId({}, {
                type: setNewPreviousMaxMessage.type,
                payload: [{ messageId: 1074533222, postTime: "2021-10-11T03:31:53.687" }]
            })
        ).toEqual({
            newMessages: [{ messageId: 1074533222, postTime: "2021-10-11T03:31:53.687" }]
        });
    });

    it('should handle missingMessageCsd', () => {
        expect(
            resetLastMessageId({}, {
                type: setMissingMessageCsd.type,
                payload: [{ messageId: 1074533470, requestId: 59625 }]
            })
        ).toEqual({
            missingMessageCsd: [{ messageId: 1074533470, requestId: 59625 }]
        });
    });

    it('should handle missingMessages', () => {
        expect(
            resetLastMessageId({}, {
                type: setMissingMessageInfo.type,
                payload: { 
                    messages: [{ jobIdentify: 1, fromMessageId: 1074533320, toMessageId: 1074533470 }],
                    msgContentTypes: [{ jobIdentify: 1, msgContentsType: 101 }],
                    requests: [{ messageId: 1074533470, requestId: 59625 }]
                }
            })
        ).toEqual({
            missingMessages: { 
                messages: [{ jobIdentify: 1, fromMessageId: 1074533320, toMessageId: 1074533470 }],
                msgContentTypes: [{ jobIdentify: 1, msgContentsType: 101 }],
                requests: [{ messageId: 1074533470, requestId: 59625 }]
            }
        });
    });

    it('should handle missingProblemAccount', () => {
        expect(
            resetLastMessageId({}, {
                type: setMissingProblemAccount.type,
                payload: { 
                    logId: [{ fromLogId: 37666, toLogId: 37676 }],
                    data: [{ messageId: 1074533470, requestId: 59625 }]
                }
            })
        ).toEqual({
            missingProblemAccount: { 
                logId: [{ fromLogId: 37666, toLogId: 37676 }],
                data: [{ messageId: 1074533470, requestId: 59625 }]
            }
        });
    });

    it('should handle missingMonitoredPlayer', () => {
        expect(
            resetLastMessageId({}, {
                type: setMissingMonitoredPlayer.type,
                payload: { 
                    logId: [{ fromLogId: 37666, toLogId: 37676 }],
                    data: [{ messageId: 1074533470, requestId: 59625 }]
                }
            })
        ).toEqual({
            missingMonitoredPlayer: { 
                logId: [{ fromLogId: 37666, toLogId: 37676 }],
                data: [{ messageId: 1074533470, requestId: 59625 }]
            }
        });
    });

    it('should handle editNewMessagePopup', () => {
        expect(
            resetLastMessageId({}, {
                type: setEditNewMessagePopup.type,
                payload: { show: true }
            })
        ).toEqual({
            editNewMessagePopup: { show: true }
        });
    });
});