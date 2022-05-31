import types from '../types';
import axios from 'axios';
import thunk from 'redux-thunk';
import configureMockStore from 'redux-mock-store';
import Site from '../../../lib/site';

import {
    setSettingTypes,
    getSettingTypes,
    setJobInfo,
    setJobInfoData,
    getJobInfo,
    setJobInfoFlag,
    setGridParams,
    setGridData,
    getGridData,
    openDeleteMappingPopup,
    closeDeleteMappingPopup,
    deleteMapping,
    setEditMappingPopup,
    setEditMappingResult,
    getMapping,
    editMapping,
    refreshUpdateData,
    setCreateMappingPopup,
    setCreateMappingResult,
    createMapping,
    setNotificationTypes,
    getNotificationTypes,
    setMsgContentsTypes,
    getMsgContentsTypes,
    setNotificationTypeRef,
    getNotificationTypeRef
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
                data: [{ id: 1, messageId: 1074533320 }],
             };
            const expectedActions = [{ type: types.SETTING_TYPES_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getSettingTypes());

            // Assert
            expect(axios.get).toHaveBeenCalledWith('SettingConfiguration/GetSettingTypes');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('setJobInfo', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.JOB_INFO_DATA,
                data: data
            };

            // Act
            const actual = setJobInfo(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getJobInfo', () => {
        it('should handle getJobInfo when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: [{ id: 1, messageId: 1074533320 }],
             };
            const expectedActions = [{ type: types.JOB_INFO_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getJobInfo(1));

            // Assert
            expect(axios.get).toHaveBeenCalledWith('SettingConfiguration/GetJobInfo?jobId=1');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('setJobInfoFlag', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.JOB_INFO_FLAG,
                data: data
            };

            // Act
            const actual = setJobInfoFlag(data);

            // Assert
            expect(actual).toEqual(expected);
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
                    data: [{ id: 1, messageId: 1074533320 }]
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
            expect(axios.get).toHaveBeenCalledWith('SettingConfiguration/GetMappings', { params: { pageIndex: 2, pageSize: 100 } });
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('openDeleteMappingPopup', () => {
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
            const actual = openDeleteMappingPopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('closeDeleteMappingPopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const expected = {
                type: types.DELETE_POPUP_DATA,
                data: { show: false }
            };

            // Act
            const actual = closeDeleteMappingPopup();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setEditMappingPopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.EDIT_POPUP_DATA,
                data: data
            };

            // Act
            const actual = setEditMappingPopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setEditMappingResult', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.EDIT_RESULT_DATA,
                data: data
            };

            // Act
            const actual = setEditMappingResult(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('refreshUpdateData', () => {
        it('should return type and data exactly', () => {
            // Act
            const actual = refreshUpdateData();

            // Assert
            expect(actual).toEqual(expect.any(Function));
        });
    });

    describe('setCreateMappingPopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.CREATE_POPUP_DATA,
                data: data
            };

            // Act
            const actual = setCreateMappingPopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setCreateMappingResult', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.CREATE_RESULT_DATA,
                data: data
            };

            // Act
            const actual = setCreateMappingResult(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setNotificationTypes', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.NOTIFICATION_TYPE_DATA,
                data: data
            };

            // Act
            const actual = setNotificationTypes(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getNotificationTypes', () => {
        it('should handle getNotificationTypes when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: {
                    data: [{ id: 1, messageId: 1074533320 }]
                }
            };

            const expectedActions = [{ type: types.NOTIFICATION_TYPE_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getNotificationTypes());

            // Assert
            expect(axios.get).toHaveBeenCalledWith('Common/GetNotificationTypes');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('setMsgContentsTypes', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.MSG_CONTENT_TYPE_DATA,
                data: data
            };

            // Act
            const actual = setMsgContentsTypes(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getMsgContentsTypes', () => {
        it('should handle getMsgContentsTypes when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: {
                    data: [{ id: 1, messageId: 1074533320 }]
                }
            };
            const params = { id: 1 };

            const expectedActions = [{ type: types.MSG_CONTENT_TYPE_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getMsgContentsTypes(params));

            // Assert
            expect(axios.get).toHaveBeenCalledWith('SettingConfiguration/GetMsgContentsTypes', { params: params });
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('setNotificationTypeRef', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.NOTIFICATION_TYPE_REF,
                data: data
            };

            // Act
            const actual = setNotificationTypeRef(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getNotificationTypeRef', () => {
        it('should handle getNotificationTypeRef when fetching data have been done', async () => {
            // Arrange
            const response = { 
                data: {
                    data: [{ id: 1, messageId: 1074533320 }]
                }
            };

            const expectedActions = [{ type: types.NOTIFICATION_TYPE_REF, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getNotificationTypeRef(1));

            // Assert
            expect(axios.get).toHaveBeenCalledWith('SettingConfiguration/GetNotificationTypeData?notificationTypeId=1');
            expect(store.getActions()).toEqual(expectedActions);
        });
    });
});