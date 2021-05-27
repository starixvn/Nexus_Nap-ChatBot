import chatGroupMapping, {
    setSettings,
    setGridParams,
    setGridData,
    filterRecipients,
    setEditPopupData,
    setEditResult
} from '../chatGroupMappingSlice';

describe('chatGroupMapping reducer', () => {
    it('should handle initial state', () => {
        expect(chatGroupMapping(undefined, {})).toEqual({
            settings: {},
            gridParams: {},
            gridData: null,
            filteredRecipients: [],
            editPopupData: { isShowed: false },
            editResult: null
        });
    });

    it('should handle setSettings', () => {
        expect(
            chatGroupMapping({}, {
                type: setSettings.type,
                payload: { channels: [], bots: [] }
            })
        ).toEqual({
            settings: { channels: [], bots: [] }
        });
    });

    it('should handle setGridParams', () => {
        expect(
            chatGroupMapping({}, {
                type: setGridParams.type,
                payload: [{
                    channelIds: '',
                    botIds: '',
                    pageIndex: 1,
                    pageSize: 100
                }]
            })
        ).toEqual({
            gridParams: [{
                channelIds: '',
                botIds: '',
                pageIndex: 1,
                pageSize: 100
            }]
        });
    });

    it('should handle setGridData', () => {
        expect(
            chatGroupMapping({}, {
                type: setGridData.type,
                payload: { totalItems: 0, data: [] }
            })
        ).toEqual({
            gridData: { totalItems: 0, data: [] }
        });
    });

    it('should handle filterRecipients', () => {
        expect(
            chatGroupMapping({}, {
                type: filterRecipients.type,
                payload: {
                    recipientTypeId: 1,
                    recipients: [
                        { recipientTypeId: 1, id: 1, name: '123bet' },
                        { recipientTypeId: 2, id: 2, name: '365online' }
                    ]
                }
            })
        ).toEqual({
            filteredRecipients: [{ recipientTypeId: 1, id: 1, name: '123bet' }]
        });
    });

    it('should handle setEditPopupData', () => {
        expect(
            chatGroupMapping({}, {
                type: setEditPopupData.type,
                payload: { isShowed: false }
            })
        ).toEqual({
            editPopupData: { isShowed: false }
        });
    });

    it('should handle setEditResult', () => {
        expect(
            chatGroupMapping({}, {
                type: setEditResult.type,
                payload: { isSuccessful: false, message: '' }
            })
        ).toEqual({
            editResult: { isSuccessful: false, message: '' }
        });
    });
});