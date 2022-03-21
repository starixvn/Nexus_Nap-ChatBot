import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import Site from '../../lib/site';
import DataService from '../../lib/dataService';
import { ActionCreators } from '../actions';
import {
    setNotificationTypes,
    setNotificationTemplates,
    setProducts,
    setRecipients,
    setCreateNotificationResult
} from '../newNotificationSlice';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('actionCreators', () => {
    beforeAll(() => {
        Site.resolveClientUrl = jest.fn((url) => url);
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('getNotificationTypes', () => {
        it('should handle getNotificationTypes when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ notificationTypeId: 1, notificationTypeName: 'test' }]
             };
            const expectedActions = [{ type: setNotificationTypes.type, payload: response.data }];
            const store = mockStore({ notificationTypes: [] });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getNotificationTypes());

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('Common/GetNotificationTypes');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getNotificationTemplates', () => {
        it('should handle getNotificationTemplates when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ notificationTemplateId: 1, notificationTemplateName: 'test', title: 'General Template' }]
            };
            const params = { notificationTypeId: 1 };
            const expectedActions = [{ type: setNotificationTemplates.type, payload: response.data }];
            const store = mockStore({ notificationTemplates: [] });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getNotificationTemplates(1));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('NewNotification/GetTemplates', params);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getProducts', () => {
        it('should handle getProducts when fetching data have been done', async () => {
            // Arrange
            const response = {
                data: [{ id: 11, name: 'AG Casino' }]
            };
            const expectedActions = [{ type: setProducts.type, payload: response.data }];
            const store = mockStore({ products: [] });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getProducts());

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('Common/GetProducts');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getRecipients', () => {
        it('should handle getRecipients when fetching data have been done', async () => {
            // Arrange
            const response = {
                data: [{ id: 11, name: 'test' }]
            };
            const params = {
                notificationTypeId: 1, 
                platforms : '1,2',
                recipientIds: '1,2',
                productId: 2,
                channelIds: '1,2'
            };
            const expectedActions = [{ type: setRecipients.type, payload: response.data }];
            const store = mockStore({ recipients: [] });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getRecipients(1,'1,2','1,2',2,'1,2'));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('NewNotification/GetRecipients', params);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getPlatforms', () => {
        it('should handle getPlatforms when fetching data have been done', async () => {
            // Arrange
            const response = {
                data: [{ platformId: 1, platformName: 'test' }]
            };
            const params = { notificationTypeId: 1 };

            const store = mockStore({ platforms: [] });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getPlatforms(1));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('Common/GetPlatforms', params);
        });
    });

    describe('getResendNotification', () => {
        it('should handle getResendNotification when fetching data have been done', async () => {
            // Arrange
            const response = {
                data: [{ id: 1, name: 'test' }]
            };
            const params = {
                notificationId : 1,
                recipientIds: '1,2',
                isResendAll: true
            };
            const store = mockStore();
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getResendNotification(1,'1,2',true));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('NewNotification/GetResendNotification', params);
        });
    });

    describe('createNewNotification', () => {
        it('should handle createNewNotification when fetching data have been done', async () => {
            // Arrange
            const response = {
                data: [{ isSuccessful: 1, message: 'success' }]
            };
            const params = { notificationTypeId: 1 };
            const expectedActions = [{ type: setCreateNotificationResult.type, payload: response.data }];
            const store = mockStore({ createNotificationResult: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.createNewNotification(params));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('NewNotification/Create', params);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });
});