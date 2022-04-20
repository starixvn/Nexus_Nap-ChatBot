import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import Site from '../../lib/site';
import DataService from '../../lib/dataService';
import { ActionCreators } from '../actions';
import {
    setCurrentPreviousMaxMessage,
    setNewPreviousMaxMessage,
    setMissingMessageCsd,
    setMissingMessageInfo,
    setMissingProblemAccount
} from '../resetLastMessageIdSlice';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('actionCreators', () => {
    beforeAll(() => {
        Site.resolveClientUrl = jest.fn((url) => url);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    describe('getCurrentPreviousMaxMessage', () => {
        it('should handle getCurrentPreviousMaxMessage when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ jobIdentify: 1, messageId: 1074533320 }],
                messageTypeInfos: [{ jobIdentify: 1, msgContentsType: 101 }]
             };
            const expectedActions = [{ type: setCurrentPreviousMaxMessage.type, payload: response.data }];
            const store = mockStore({ currentMessages: null });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getCurrentPreviousMaxMessage());

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('ResetLastMessageId/GetCurrentPreviousMaxMessage');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getNewPreviousMaxMessage', () => {
        it('should handle getNewPreviousMaxMessage when fetching data have been done', async () => {
            // Arrange
            const response = { 
                messagePostTimeInfos: [{ messageId: 1074533222, postTime: "2021-10-11T03:31:53.687" }],
                newMaxMessageId: 1074533470
            };
            const expectedParams = { 
                messages: [1074533320, 1074533461, 1074533222] 
            };
            const expectedActions = [{ type: setNewPreviousMaxMessage.type, payload: response.data }];
            const store = mockStore({ newMessages: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getNewPreviousMaxMessage(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ResetLastMessageId/GetNewPreviousMaxMessage', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getMissingMessageCsd', () => {
        it('should handle getMissingMessageCsd when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ messageId: 1074533470, requestId: 59625 }]
            };
            const expectedParams = {
                fromMessageId: 1074533461,
                postTime: "2021-10-28T01:43:09.47",
                toMessageId: 1074533470
            };
            const expectedActions = [{ type: setMissingMessageCsd.type, payload: response.data }];
            const store = mockStore({ missingMessageCsd: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getMissingMessageCsd(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ResetLastMessageId/GetMissingMessageCsd', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getMissingMessageInfo', () => {
        it('should handle getMissingMessageInfo when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ content: "Attn:[Soccer]", jobIdentify: 1, messageId: 1074533456, postTime: "2021-10-28T00:53:58.637" }]
            };
            const expectedParams = { 
                messages: [{ jobIdentify: 1, fromMessageId: 1074533320, toMessageId: 1074533470 }],
                msgContentTypes: [{ jobIdentify: 1, msgContentsType: 101 }],
                requests: [{ messageId: 1074533470, requestId: 59625 }]
            };
            const expectedActions = [{ type: setMissingMessageInfo.type, payload: response.data }];
            const store = mockStore({ missingMessages: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getMissingMessageInfo(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ResetLastMessageId/GetMissingMessageInfo', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getMissingProblemAccount', () => {
        it('should handle getMissingProblemAccount when fetching data have been done', async () => {
            // Arrange
            const response = { 
                logId: [{ fromLogId: 37666, toLogId: 37676 }],
                data: [{ messageId: 1074533470, requestId: 59625 }]
            };
            const expectedParams = { fromLogId: 34464 };
            const expectedActions = [{ type: setMissingProblemAccount.type, payload: response.data }];
            const store = mockStore({ missingProblemAccount: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getMissingProblemAccount(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ResetLastMessageId/GetMissingProblemAccount', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('createMissingMessage', () => {
        it('should return response when request has been done', async () => {
            // Arrange
            const response = { 
                code: 0, isSuccessful: true, message: ""
            };
            const expectedParams = { 
                messages: [{ jobIdentify: 1, messageId: 1074533470 }],
                requests: [{ messageId: 1074533471, requestId: 59625 }]
            };
            const store = mockStore();
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.createMissingMessage(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ResetLastMessageId/CreateMissingMessage', expectedParams);
        });
    });

    describe('editMessageInfo', () => {
        it('should return response when request has been done', async () => {
            // Arrange
            const response = { 
                code: 0, isSuccessful: true, message: ""
            };
            const expectedParams = { 
                jobInfoList: [{ jobIdentify: 1, messageId: 1074533470 }]
            };
            const store = mockStore();
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.editMessageInfo(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ResetLastMessageId/EditMessageInfo', expectedParams);
        });
    });
});