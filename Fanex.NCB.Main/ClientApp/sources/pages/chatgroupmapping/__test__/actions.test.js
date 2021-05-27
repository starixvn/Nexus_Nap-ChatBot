import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import Site from '../../lib/site';
import DataService from '../../lib/dataService';
import { ActionCreators } from '../actions';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('actionCreators', () => {
    beforeAll(() => {
        Site.resolveClientUrl = jest.fn((url) => url);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    describe('getGridData', () => {
        it('should handle setGridData when fetching data have been done', async () => {
            // Arrange
            const expectedParams = {
                channelIds: '',
                botIds: '',
                pageIndex: 1,
                pageSize: 100
            };
            const response = {
                data: {
                    totalItems: 1,
                    data: [{ botName: 'NCB1', channelId: 1, channelName: 'skype' }]
                }
            };
            const expectedActions = [{ type: ActionCreators.setGridData.type, payload: response.data }];
            const store = mockStore({ gridData: null });
            const promise = Promise.resolve(response);
            DataService.getDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.getGridData(expectedParams));

            // Assert
            expect(DataService.getDataAsync).toHaveBeenCalledWith('ChatGroupMapping/GetAll', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('editMapping', () => {
        it('should handle setEditResult when request has been done', async () => {
            // Arrange
            const expectedParams = { botName: 'NCB1', channelId: 1, channelName: 'skype' };
            const response = { data: { isSuccessful: true, message: '' } };
            const expectedActions = [{ type: ActionCreators.setEditResult.type, payload: response.data }];
            const store = mockStore({ editResult: null });
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            await store.dispatch(ActionCreators.editMapping(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ChatGroupMapping/Edit', expectedParams);
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('editActive', () => {
        it('should return response when request has been done', async () => {
            // Arrange
            const expectedParams = { groupId: 1, isActive: true };
            const store = mockStore({});
            const response = { data: { isSuccessful: true, message: '' } };
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            // Act
            const actual = await store.dispatch(ActionCreators.editActive(expectedParams));

            // Assert
            expect(DataService.postDataAsync).toHaveBeenCalledWith('ChatGroupMapping/UpdateActive', expectedParams);
            expect(actual).toEqual(response);
        });
    });
});