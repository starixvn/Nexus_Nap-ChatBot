import * as actions from '../gridHandlerActions';
import * as types from '../types';

describe("gridHanderActions", () => {
    describe("updateActiveRowIndex", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.ACTIVE_ROW_INDEX,
                data: data
            };

            // Act
            let result = actions.updateActiveRowIndex(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("openDeletePopup", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.DELETE_POPUP_DATA,
                data: {
                    isShowed: true,
                    content: data
                }
            };

            // Act
            let result = actions.openDeletePopup(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("closeDeletePopup", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.DELETE_POPUP_DATA,
                data: {
                    isShowed: false,
                    content: ""
                }
            };

            // Act
            let result = actions.closeDeletePopup();

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("openEditPopup", () => {
        it("create the correct action", () => {
            // Arrange
            let item = {
                notificationTypeName: "name 1",
                recipientTypeId: 1,
                systemId: 2
            };
            let newEditPopupData = {
                isShowed: true,
                editingItem: {
                    notificationTypeName: {
                        value: item.notificationTypeName,
                        isValid: true
                    },
                    recipientTypeId: {
                        value: item.recipientTypeId,
                        isValid: true
                    },
                    systemId: {
                        value: item.systemId,
                        isValid: true
                    }
                },
                popupTitle: "Edit Notification Type",
                successMsg: "Edit Notification Type Successfully",
                isNew: false
            };
            let expected = {
                type: types.UPDATE_POPUP_DATA,
                data: newEditPopupData
            };

            // Act
            let result = actions.openEditPopup(item);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("openNewPopup", () => {
        it("create the correct action", () => {
            // Arrange          
            let newEditPopupData = {
                isShowed: true,
                editingItem: {
                    notificationTypeName: {
                        value: '',
                        isValid: false
                    },
                    recipientTypeId: {
                        value: -1,
                        isValid: false
                    },
                    systemId: {
                        value: -1,
                        isValid: false
                    }
                },
                popupTitle: "Create New Notification Type",
                successMsg: "Create New Notification Type Successfully",
                isNew: true
            };
            let expected = {
                type: types.UPDATE_POPUP_DATA,
                data: newEditPopupData
            };

            // Act
            let result = actions.openNewPopup();

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("closeNewEditPopup", () => {
        it("create the correct action", () => {
            // Arrange
            let newEditPopupData = {
                isShowed: false,
                editingItem: {},
                popupTitle: "",
                successMsg: ""
            };
            let expected = {
                type: types.UPDATE_POPUP_DATA,
                data: newEditPopupData
            };

            // Act
            let result = actions.closeNewEditPopup();

            // Assert
            expect(result).toEqual(expected);
        });
    });
});