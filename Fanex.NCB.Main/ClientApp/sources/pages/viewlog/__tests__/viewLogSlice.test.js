import viewLog, {
    setParamsPage,
    setSystemMessage,
    setRecipientList,
    setConversationMessage
} from '../viewLogSlice';

describe('viewLogSlice reducer', () => {
    it('should handle initial state', () => {
        expect(viewLog(undefined, {})).toEqual({
            paramsPage: {
                pageIndex: 1,
                pageSize: 100
            },
            systemMessage: {},
            recipientList: {},
            conversationMessage: {}
        });
    });

    it('should handle setParamsPage', () => {
        expect(
            viewLog({}, {
                type: setParamsPage.type,
                payload: { notificationId: 188, pageIndex: 1, pageSize: 100 }
            })
        ).toEqual({
            paramsPage: { notificationId: 188, pageIndex: 1, pageSize: 100  }
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