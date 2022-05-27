import types from '../types';
import axios from 'axios';
import thunk from 'redux-thunk';
import configureMockStore from 'redux-mock-store';
import Site from '../../../lib/site';
import errorResponseHandler from '../../../lib/errorResponseHandler';
import {
    setNotificationTypes,
    getNotificationTypes,
    setGridParams,
    setGridData,
    getGridData,
    setCreateTemplatePopup,
    setCreateTemplateResult,
    createTemplate,
    setUpdateData,
    refreshUpdateData,
    setEditTemplatePopup,
    setEditTemplateResult,
    getTemplate,
    editTemplate,
    deleteTemplate,
    openDeleteTemplatePopup,
    closeDeleteTemplatePopup,
    openMarkdownRulesPopup,
    closeMarkdownRulesPopup
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

    describe('setNotificationTypes', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.NOTIFICATIONTYPE_DATA,
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
                data: [{ id: 1, messageId: 1074533320 }],
             };
            const expectedActions = [{ type: types.NOTIFICATIONTYPE_DATA, data: response.data }];
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
                data: [{ id: 1, messageId: 1074533320 }]
             };
            const expectedActions = [{ type: types.GRID_DATA, data: response.data }];
            const store = mockStore();
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            // Act
            await store.dispatch(getGridData({ id: 123 }));

            // Assert
            expect(axios.get).toHaveBeenCalledWith('NotificationTemplate/Get', { params: { id: 123 } });
            expect(store.getActions()).toEqual(expectedActions);
        });
    });

    describe('openDeleteTemplatePopup', () => {
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
            const actual = openDeleteTemplatePopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('closeDeleteTemplatePopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const expected = {
                type: types.DELETE_POPUP_DATA,
                data: { show: false }
            };

            // Act
            const actual = closeDeleteTemplatePopup();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('deleteTemplate', () => {
        it('should handle deleteTemplate when fetching data have been done', async () => {
            // Arrange
            const store = mockStore();

            // Act
            await store.dispatch(deleteTemplate(123));
        });
    });

    describe('setEditTemplatePopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.EDIT_POPUP_DATA,
                data: data
            };

            // Act
            const actual = setEditTemplatePopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setEditTemplateResult', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.EDIT_RESULT_DATA,
                data: data
            };

            // Act
            const actual = setEditTemplateResult(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getTemplate', () => {
        it('should handle getTemplate when fetching data have been done', async () => {
            // Arrange
            const store = mockStore();
    
            // Act
            await store.dispatch(getTemplate(123));
        });
    });

    describe('editTemplate', () => {
        it('should handle editTemplate when fetching data have been done', async () => {
            // Arrange
            const store = mockStore();
    
            // Act
            await store.dispatch(editTemplate(123));
        });
    });

    describe('setCreateTemplatePopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.CREATE_POPUP_DATA,
                data: data
            };

            // Act
            const actual = setCreateTemplatePopup(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('setCreateTemplateResult', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const data = [{ id: 1, name: 'test' }];
            const expected = {
                type: types.CREATE_TEMPLATE_RESULT_DATA,
                data: data
            };

            // Act
            const actual = setCreateTemplateResult(data);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('createTemplate', () => {
        it('should handle createTemplate when fetching data have been done', async () => {
            // Arrange
            const store = mockStore();
    
            // Act
            await store.dispatch(createTemplate(123));
        });
    });

    describe('openMarkdownRulesPopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const expected = {
                type: types.MARKDOWNRULE_POPUP_DATA,
                data: { show: true }
            };

            // Act
            const actual = openMarkdownRulesPopup();

            // Assert
            expect(actual).toEqual(expected);
        });
    });


    describe('closeMarkdownRulesPopup', () => {
        it('should return type and data exactly', () => {
            // Arrange
            const expected = {
                type: types.MARKDOWNRULE_POPUP_DATA,
                data: { show: false }
            };

            // Act
            const actual = closeMarkdownRulesPopup();

            // Assert
            expect(actual).toEqual(expected);
        });
    });
});