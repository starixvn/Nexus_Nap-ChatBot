import viewLog, {
    setParamsPageSystemMessage,
    setParamsPageRecipientList,
    setParamsPageConversationMessage,
    setNotificationSystemMessage,
    setRecipientList,
    setConversationMessageLog
} from '../viewLogSlice';

describe('viewLogSlice reducer', () => {
    it('should handle initial state', () => {
        expect(viewLog(undefined, {})).toEqual({
            paramsPageSystemMessage: {
                pageIndex: 1,
                pageSize: 100
            },
            paramsPageRecipientList: {
                pageIndex: 1,
                pageSize: 100
            },
            paramsPageConversationMessage: {
                pageIndex: 1,
                pageSize: 100
            },
            systemMessages: {},
            recipients: {},
            conversationMessages: {}
        });
    });

    it('should handle setParamsPageSystemMessage', () => {
        expect(
            viewLog({}, {
                type: setParamsPageSystemMessage.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            paramsPageSystemMessage: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });

    it('should handle setParamsPageRecipientList', () => {
        expect(
            viewLog({}, {
                type: setParamsPageRecipientList.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            paramsPageRecipientList: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });

    it('should handle setParamsPageConversationMessage', () => {
        expect(
            viewLog({}, {
                type: setParamsPageConversationMessage.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            paramsPageConversationMessage: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });

    it('should handle setNotificationSystemMessage', () => {
        expect(
            viewLog({}, {
                type: setNotificationSystemMessage.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            systemMessages: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });

    it('should handle setRecipientList', () => {
        expect(
            viewLog({}, {
                type: setRecipientList.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            recipientList: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });

    it('should handle setConversationMessageLog', () => {
        expect(
            viewLog({}, {
                type: setConversationMessageLog.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            conversationMessage: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });
})