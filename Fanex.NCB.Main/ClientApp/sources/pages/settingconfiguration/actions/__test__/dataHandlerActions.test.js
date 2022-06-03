import types from '../types';
import axios from 'axios';
import thunk from 'redux-thunk';
import configureMockStore from 'redux-mock-store';
import Site from '../../../lib/site';

import {
    setSettingTypes,
    getSettingTypes,
    setGridParams,
    setGridData,
    getGridData,
    setCreateSettingPopup,
    setCreateSettingTypeResult,
    createSettingType,
    setEditSettingTypePopup,
    setEditSettingTypeResult,
    getSettingType,
    editSettingType,
    deleteSettingType,
    openDeleteSettingTypePopup,
    closeDeleteSettingTypePopup
} from '../dataHandlerActions';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('dataHandlerActions', () => {
    beforeAll(() => {
        Site.resolveClientUrl = jest.fn((url) => url);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    describe('setSettingTypes', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.SETTING_TYPES_DATA,
                data: data
            };

            // Act
            const actual = setSettingTypes(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getSettingTypes', () => {
        it('should handle getSettingTypes when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ id: 1, name: 'test'  }]
            };
            const expectedActions = [{ type: types.SETTING_TYPES_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getSettingTypes());

            // Assert
            expect(axios.get).toHaveBeenCalledWith('Common/GetSettingTypes');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('setGridParams', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.GRID_PARAMS,
                data: data
            };

            // Act
            const actual = setGridParams(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setGridData', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.GRID_DATA,
                data: data
            };

            // Act
            const actual = setGridData(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getGridData', () => {
        it('should handle getGridData when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: {
                    data: [{ id: 1, name: 'test'  }]
                }
            };
            const params = { pageIndex: 2, pageSize: 100 };
            const setParamsWhenDelete = jest.fn();
            const expectedActions = [{ type: types.GRID_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getGridData(params, setParamsWhenDelete));

            // Assert
            expect(axios.get).toHaveBeenCalledWith('SettingConfiguration/GetSettings', { params: { pageIndex: 2, pageSize: 100 } });
            expect(store.getActions()).toEqual(expectedActions);
        });

        it('should handle getGridData when not data', async () => {
            // Arrange
            const response = { 
                data: {
                    data: []
                }
            };
            const params = { pageIndex: 1, pageSize: 100 };
            const setParamsWhenDelete = jest.fn();
            const expectedActions = [{ type: types.GRID_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getGridData(params, setParamsWhenDelete));

            // Assert
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('openDeleteSettingTypePopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.DELETE_POPUP_DATA,
                data: {
                    show: true,
                    ...data
                }
            };

            // Act
            const actual = openDeleteSettingTypePopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('closeDeleteSettingTypePopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const expected = {
                type: types.DELETE_POPUP_DATA,
                data: { show: false }
            };

            // Act
            const actual = closeDeleteSettingTypePopup();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setEditSettingTypePopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.EDIT_POPUP_DATA,
                data: data
            };

            // Act
            const actual = setEditSettingTypePopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setEditSettingTypeResult', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.EDIT_RESULT_DATA,
                data: data
            };

            // Act
            const actual = setEditSettingTypeResult(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setCreateSettingPopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.CREATE_POPUP_DATA,
                data: data
            };

            // Act
            const actual = setCreateSettingPopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setCreateSettingTypeResult', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.CREATE_SETTING_TYPE_RESULT_DATA,
                data: data
            };

            // Act
            const actual = setCreateSettingTypeResult(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });
});