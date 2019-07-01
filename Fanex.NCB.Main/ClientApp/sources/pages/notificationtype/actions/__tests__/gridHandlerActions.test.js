import actions from '../gridHandlerActions';
import types from '../types';

describe('gridHanderActions', () => {
    describe('updateActiveRowIndex', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.ACTIVE_ROW_INDEX,
                data: data
            };

            // Act
            const result = actions.updateActiveRowIndex(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('openDeletePopup', () => {
        it('create the correct action', () => {
            // Arrange
            const notificationTypeId = 1;
            const notificationTypeName = 'abc';
            const deletePopupData = {
                isShowed: true,
                notificationTypeId: 1,
                content: 'abc'
            };
            const expected = {
                type: types.DELETE_POPUP_DATA,
                data: deletePopupData
            };

            // Act
            const result = actions.openDeletePopup(notificationTypeId, notificationTypeName);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('closeDeletePopup', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.DELETE_POPUP_DATA,
                data: {
                    isShowed: false,
                    content: ''
                }
            };

            // Act
            const result = actions.closeDeletePopup();

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("openEditPopup", () => {
        it('create the correct action', () => {
            // Arrange
            const item = {
                notificationTypeName: 'name 1',
                recipientTypes: [{ id: 1 }, {id: 2}],
                systemId: 2
            };
            const newEditPopupData = {
                isShowed: true,
                editingItem: {
                    notificationTypeName: {
                        value: item.notificationTypeName,
                        isValid: true
                    },
                    recipientTypeIds: {
                        value: "1,2",
                        isValid: true
                    },
                    systemId: {
                        value: item.systemId,
                        isValid: true
                    }
                },
                popupTitle: 'Edit Notification Type',
                successMsg: 'Edit Notification Type Successfully',
                isNew: false
            };
            const expected = {
                type: types.UPDATE_POPUP_DATA,
                data: newEditPopupData
            };

            // Act
            const result = actions.openEditPopup(item);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('openNewPopup', () => {
        it('create the correct action', () => {
            // Arrange          
            const newEditPopupData = {
                isShowed: true,
                editingItem: {
                    notificationTypeName: {
                        value: '',
                        isValid: false
                    },
                    recipientTypeIds: {
                        value: "",
                        isValid: false
                    },
                    systemId: {
                        value: -1,
                        isValid: false
                    }
                },
                popupTitle: 'Create New Notification Type',
                successMsg: 'Create New Notification Type Successfully',
                isNew: true
            };
            const expected = {
                type: types.UPDATE_POPUP_DATA,
                data: newEditPopupData
            };

            // Act
            const result = actions.openNewPopup();

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('closeNewEditPopup', () => {
        it('create the correct action', () => {
            // Arrange
            const newEditPopupData = {
                isShowed: false,
                editingItem: {},
                popupTitle: '',
                successMsg: ''
            };
            const expected = {
                type: types.UPDATE_POPUP_DATA,
                data: newEditPopupData
            };

            // Act
            const result = actions.closeNewEditPopup();

            // Assert
            expect(result).toEqual(expected);
        });
    });
});