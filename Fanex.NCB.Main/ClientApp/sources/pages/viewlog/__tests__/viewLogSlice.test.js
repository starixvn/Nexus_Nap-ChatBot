import viewLog, {
    setParamsPageSystemMessage,
    setParamsPageRecipientList,
    setParamsPageConversationMessage,
    setSystemMessage,
    setRecipientList,
    setConversationMessage
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
            systemMessage: {},
            recipientList: {},
            conversationMessage: {}
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

    it('should handle setSystemMessage', () => {
        expect(
            viewLog({}, {
                type: setSystemMessage.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            systemMessage: { notificationId: 188, pageIndex: 1, pageSize: 100  }
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

    it('should handle setConversationMessage', () => {
        expect(
            viewLog({}, {
                type: setConversationMessage.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            conversationMessage: { notificationId: 188, pageIndex: 1, pageSize: 100  }
        });
    });
})