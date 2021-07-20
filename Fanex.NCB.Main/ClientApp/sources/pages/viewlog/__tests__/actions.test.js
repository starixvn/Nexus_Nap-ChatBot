import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import Site from '../../lib/site';
import DataService from '../../lib/dataService';
import { ActionCreators } from '../actions';
import {
    setParamsPageSystemMessage,
    setParamsPageRecipientList,
    setParamsPageConversationMessage,
    setSystemMessage,
    setRecipientList,
    setConversationMessage
} from '../viewLogSlice';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('actionCreators', () => {
    beforeAll(() => {
        Site.resolveClientUrl = jest.fn((url) => url);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    describe('getParamsPageSystemMessage', () => {
        it('should handle getParamsPageSystemMessage when fetching data have been done', () => {
            // Arrange
            const expectedParams = {
                notificationId: 188,
                pageIndex: 2,
                pageSize: 200
            };
            const response = {
                data: {
                    notificationId: 188,
                    pageIndex: 2,
                    pageSize: 200
                }
            };
            const expectedActions = [{ type: setParamsPageSystemMessage.type, payload: response.data }];
            const store = mockStore({ paramsPageSystemMessage: {} });

            // Act
            store.dispatch(ActionCreators.getParamsPageSystemMessage(expectedParams));

            // Assert
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getParamsPageRecipientList', () => {
        it('should handle getParamsPageRecipientList when fetching data have been done', () => {
            // Arrange
            const expectedParams = {
                notificationId: 188,
                pageIndex: 2,
                pageSize: 200
            };
            const response = {
                data: {
                    notificationId: 188,
                    pageIndex: 2,
                    pageSize: 200
                }
            };
            const expectedActions = [{ type: setParamsPageRecipientList.type, payload: response.data }];
            const store = mockStore({ paramsPageRecipientList: {} });

            // Act
            store.dispatch(ActionCreators.getParamsPageRecipientList(expectedParams));

            // Assert
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getParamsPageConversationMessage', () => {
        it('should handle getParamsPageConversationMessage when fetching data have been done', () => {
            // Arrange
            const expectedParams = {
                notificationId: 188,
                pageIndex: 2,
                pageSize: 200
            };
            const response = {
                data: {
                    notificationId: 188,
                    pageIndex: 2,
                    pageSize: 200
                }
            };
            const expectedActions = [{ type: setParamsPageConversationMessage.type, payload: response.data }];
            const store = mockStore({ paramsPageConversationMessage: {} });

            // Act
            store.dispatch(ActionCreators.getParamsPageConversationMessage(expectedParams));

            // Assert
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getSystemMessage', () => {
        it('should handle getSystemMessage when fetching data have been done', async () => {
            // Arrange
            const expectedParams = {
                notificationId: 188,
                pageIndex: 1,
                pageSize: 100
            };
            const response = {
                data: {
                    total: 1,
                    data: [{ messageID: 1074491061, notificationID: 188, createdDate: '2020-01-08T05:10:22', checkMsg: '- Can not map to any Recipient(s): mansion88, MARS'}]
                }
            };
            const expectedActions = [{ type: setSystemMessage.type, payload: response.data }];
            const store = mockStore({ systemMessage: {} });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getSystemMessage(expectedParams));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('ViewLog/GetSystemMessage', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getRecipientList', () => {
        it('should handle getRecipientList when fetching data have been done', async () => {
            // Arrange
            const expectedParams = {
                notificationId: 188,
                pageIndex: 1,
                pageSize: 100
            };
            const response = {
                data: {
                    total: 1,
                    data: [{ notificationID: 188, name: 'tim - new group', chatGroupTypeName: 'Normal Group', recipientName: '67BET', sentTime: '0001-01-01T00:00:00', status: 'Sending', retryTime: 0, errMsg: null}]
                }
            };
            const expectedActions = [{ type: setRecipientList.type, payload: response.data }];
            const store = mockStore({ recipientList: {} });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getRecipientList(expectedParams));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('ViewLog/GetRecipientList', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getConversationMessage', () => {
        it('should handle getConversationMessage when fetching data have been done', async () => {
            // Arrange
            const expectedParams = {
                notificationId: 188,
                pageIndex: 1,
                pageSize: 100
            };
            const response = {
                data: {
                    total: 1,
                    data: [{ notificationID: 188, botName: 'tim - new group', conversationGroupID: 358, conversationGroupName: 'tim - new group', recipientName: '67BET', sentTime: '0001-01-01T00:00:00', isSuccessful: 1, errorMessage: null, content: 'Anything ok'}]
                }
            };
            const expectedActions = [{ type: setConversationMessage.type, payload: response.data }];
            const store = mockStore({ conversationMessage: {} });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getConversationMessage(expectedParams));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('ViewLog/GetConversationMessage', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });
});