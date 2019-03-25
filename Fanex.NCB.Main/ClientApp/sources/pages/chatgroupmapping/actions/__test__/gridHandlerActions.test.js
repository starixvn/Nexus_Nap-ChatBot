import thunk from 'redux-thunk'
import configureMockStore from 'redux-mock-store'
import * as actions from '../gridHandlerActions'
import * as types from '../types'
import axios from 'axios'
import "babel-polyfill"

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe("gridHandlerActions", () => { 
    describe("openEditPopup", () => {
        it("create the correct action", () => {
            // Arrange
            let item = {
                groupID: 1,
                conversationID: 1,
                mappingStatusID: 2,
                mappingStatusName: "Mapped",
                botName: "Bot 1",
                groupName: "Group 1",
                recipientTypeID: 1,
                recipientIDs: "1,2",
                remark: "",
                recipientStatusNames: ""
            };
            let popupData = {
                isShowed: true,
                editingItem: {
                    groupID: item.groupID,
                    conversationID: item.conversationID,
                    mappingStatusId: item.mappingStatusID,
                    mappingStatusName: item.mappingStatusName,
                    botName: item.botName,
                    groupName: item.groupName,
                    recipientTypeId: {
                        value: item.recipientTypeID,
                        isValid: item.recipientTypeID != 0
                    },
                    recipientIds: {
                        value: item.recipientIDs,
                        isValid: item.recipientIDs != ""
                    },
                    recipientStatusNames: item.recipientStatusNames,
                    remark: {
                        value: item.remark,
                        isValid: item.remark != null
                    }
                }
            };
            let expected = {
                type: types.UPDATE_POPUP_DATA,
                data: popupData
            };

            // Act
            let result = actions.openEditPopup(item);

            // Assert
            expect(result).toEqual(expected);
        })
    })

    describe("closeEditPopup", () => {
        it("create the correct action", () => {
            // Arrange            
            let popupData = {
                isShowed: false,
                editingItem: {}
            };
            let expected = {
                type: types.UPDATE_POPUP_DATA,
                data: popupData
            };

            // Act
            let result = actions.closeEditPopup();

            // Assert
            expect(result).toEqual(expected);
        })
    })
})