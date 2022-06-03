import notificationType, {
    setSettings,
    setGridParams,
    setGridData,
    setActiveRowIndex,
    setCreateEditPopupData,
    setCreateEditResult,
    setDeletePopupData,
    setDeleteResult
} from '../notificationTypeSlice';

describe('notificationTypeSlice reducer', () => {
    it('should handle initial state', () => {
        expect(notificationType(undefined, {})).toEqual({
            settings: {},
            gridParams: {},
            gridData: null,
            activeRowIndex: -1,
            createEditPopupData: { isShowed: false },
            createEditResult: null,
            deletePopupData: { isShowed: false },
            deleteResult: null
        });
    });

    it('should handle setSettings', () => {
        expect(
            notificationType({}, {
                type: setSettings.type,
                payload: [{ settingId: 1, settingName: 'Delete' }]
            })
        ).toEqual({
            settings: [{ settingId: 1, settingName: 'Delete' }]
        });
    });

    it('should handle setGridParams', () => {
        expect(
            notificationType({}, {
                type: setGridParams.type,
                payload: [{ id: 1, name: 'test' }]
            })
        ).toEqual({
            gridParams: [{ id: 1, name: 'test' }]
        });
    });

    it('should handle setGridData', () => {
        expect(
            notificationType({}, {
                type: setGridData.type,
                payload: [{ id: 1, name: 'test' }]
            })
        ).toEqual({
            gridData: [{ id: 1, name: 'test' }]
        });
    });

    it('should handle setActiveRowIndex', () => {
        expect(
            notificationType({}, {
                type: setActiveRowIndex.type,
                payload: 1
            })
        ).toEqual({
            activeRowIndex: 1
        });
    });

    it('should handle setCreateEditPopupData', () => {
        expect(
            notificationType({}, {
                type: setCreateEditPopupData.type,
                payload: { isShowed: true }
            })
        ).toEqual({
            createEditPopupData: { isShowed: true }
        });
    });

    it('should handle setCreateEditResult', () => {
        expect(
            notificationType({}, {
                type: setCreateEditResult.type,
                payload: [{ code: 0, isSuccessful: true }]
            })
        ).toEqual({
            createEditResult: [{ code: 0, isSuccessful: true }]
        });
    });

    it('should handle setDeletePopupData', () => {
        expect(
            notificationType({}, {
                type: setDeletePopupData.type,
                payload: { isShowed: true }
            })
        ).toEqual({
            deletePopupData: { isShowed: true }
        });
    });

    it('should handle setDeleteResult', () => {
        expect(
            notificationType({}, {
                type: setDeleteResult.type,
                payload: [{ code: 0, isSuccessful: true }]
            })
        ).toEqual({
            deleteResult: [{ code: 0, isSuccessful: true }]
        });
    });
});