import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import Site from '../../lib/site';
import DataService from '../../lib/dataService';
import { ActionCreators } from '../actions';
import {
    setSettings,
    setGridParams,
    setGridData,
    setActiveRowIndex,
    setCreateEditPopupData,
    setCreateEditResult,
    setDeletePopupData,
    setDeleteResult
} from '../notificationTypeSlice';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('actionCreators', () => {
    beforeAll(() => {
        Site.resolveClientUrl = jest.fn((url) => url);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    describe('getSettings', () => {
        it('should handle getSettings when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ settingId: 1, settingName: 'test' }]
             };
            const expectedActions = [{ type: setSettings.type, payload: response.data }];
            const store = mockStore({ settings: null });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getSettings());

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('NotificationType/GetSettings');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('getGridData', () => {
        it('should handle getGridData when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ id: 1, name: 'test' }]
             };
            const expectedActions = [{ type: setGridData.type, payload: response.data }];
            const store = mockStore({ gridData: null });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getGridData({ id: 1}));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('NotificationType/GetAll', { id: 1});
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('createEditNotificationType', () => {
        it('should handle createEditNotificationType when fetching data have been done and isNew true', async () => {
            // Arrange
            const response = { 
                data: [{ id: 1, name: 'test', show: true }]
             };
            const expectedActions = [{ type: setCreateEditResult.type, payload: response.data }];
            const store = mockStore({ createEditPopupData: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.createEditNotificationType({ id: 1}, true));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('NotificationType/Add', { id: 1});
            expect(store.getActions()).toEqual(expectedActions);
        });

        it('should handle createEditNotificationType when fetching data have been done and isNew false', async () => {
            // Arrange
            const response = { 
                data: [{ id: 1, name: 'test', show: true }]
             };
            const expectedActions = [{ type: setCreateEditResult.type, payload: response.data }];
            const store = mockStore({ createEditPopupData: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.createEditNotificationType({ id: 1}, false));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('NotificationType/Edit', { id: 1});
            expect(store.getActions()).toEqual(expectedActions);
        });
    });
    
    describe('deleteNotificationType', () => {
        it('should handle deleteNotificationType when fetching data have been done and isNew true', async () => {
            // Arrange
            const response = { 
                data: [{ id: 1, name: 'test', show: true }]
             };
            const expectedActions = [{ type: setDeleteResult.type, payload: response.data }];
            const store = mockStore({ deleteResult: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.deleteNotificationType(1, true));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('NotificationType/Delete/1');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });
});