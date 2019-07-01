import thunk from 'redux-thunk';
import configureMockStore from 'redux-mock-store';
import actions from '../dataHandlerActions';
import types from '../types';
import axios from 'axios';
import "babel-polyfill";

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('dataHanderActions', () => {
    describe('setGridParams', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.GRID_PARAMS,
                data: data
            };

            // Act
            const result = actions.setGridParams(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('setGridData', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.GRID_DATA,
                data: data
            };

            // Act
            const result = actions.setGridData(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('getGridData', () => {
        it('axios.get with correct params', () => {
            // Arrange
            axios.get = jest.fn(() => Promise.resolve(''));
            const store = mockStore();
            const expectedUrl = '/NotificationType/GetAll';
            const expectedParams = 'some param';

            // Act
            store.dispatch(actions.getGridData(expectedParams));

            // Assert
            expect(axios.get).toHaveBeenLastCalledWith(expectedUrl, { params: expectedParams });
        });

        it('dispatch the correct action', (done) => {
            // Arrange
            const fakeResponse = { data: 'some data' };
            axios.get = jest.fn(() => Promise.resolve(fakeResponse));
            const store = mockStore();
            const expected = [{
                type: types.GRID_DATA,
                data: fakeResponse.data
            }];

            // Act
            store.dispatch(actions.getGridData({}));

            // Assert
            axios.get().then(() => {
                expect(store.getActions()).toEqual(expected);
                done();
            });
        });
    });

    describe('setSettings', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.SETTING_DATA,
                data: data
            };

            // Act
            const result = actions.setSettings(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('getSettings', () => {
        it('axios.get with correct params', () => {
            // Arrange
            axios.get = jest.fn(() => Promise.resolve(''));
            const store = mockStore();
            const expectedUrl = '/NotificationType/GetSettings';

            // Act
            store.dispatch(actions.getSettings());

            // Assert
            expect(axios.get).toHaveBeenCalledWith(expectedUrl);
        });

        it('dispatch the correct action', (done) => {
            // Arrange
            const fakeResponse = { data: "some data" };
            axios.get = jest.fn(() => Promise.resolve(fakeResponse));
            const store = mockStore();
            const expectedAction = [
                {
                    type: types.SETTING_DATA,
                    data: fakeResponse.data
                }
            ];

            // Act
            store.dispatch(actions.getSettings());

            // Assert
            axios.get().then(() => {
                expect(store.getActions()).toEqual(expectedAction);
                done();
            });
        });
    });

    describe('setDeleteData', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.DELETE_DATA,
                data: data
            };

            // Act
            const result = actions.setDeleteData(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('deleteItem', () => {
        it('axios.post with correct params', (done) => {
            // Arrange
            const fakeResponse = { data: 'some data' };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            const store = mockStore();
            const expectedUrl = '/NotificationType/Delete';

            // Act & Assert
            store.dispatch(actions.deleteItem(1)).then(() => {
                expect(axios.post).toHaveBeenCalledWith(expectedUrl, 1, { 'headers': { 'Content-Type': 'application/json' } });
                done();
            });
        });

        it('dispatch the correct action', (done) => {
            // Arrange
            const fakeResponse = { data: 'some data' };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            const store = mockStore();
            const expectedAction = [
                {
                    type: types.DELETE_DATA,
                    data: fakeResponse.data
                }
            ];

            // Act & Assert
            store.dispatch(actions.deleteItem(1)).then(() => {
                expect(store.getActions()).toEqual(expectedAction);
                done();
            });
        });
    });

    describe('setUpdateParams', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.UPDATE_PARAMS,
                data: data
            };

            // Act
            const result = actions.setUpdateParams(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('setUpdateData', () => {
        it('create the correct action', () => {
            // Arrange
            const data = ['some data'];
            const expected = {
                type: types.UPDATE_DATA,
                data: data
            };

            // Act
            const result = actions.setUpdateData(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe('refreshUpdateData', () => {
        it('dispatch the correct action', () => {
            // Arrange
            const store = mockStore();
            const expectedAction = [{
                type: types.UPDATE_DATA,
                data: { addedresult: false }
            }];

            // Act
            store.dispatch(actions.refreshUpdateData());

            // Asssert
            expect(store.getActions()).toEqual(expectedAction);
        });
    });

    describe('update', () => {
        it('when isNew true - call axios.post with the correct params', (done) => {
            // Arrange
            const store = mockStore();
            const fakeResponse = { data: 'some data' };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            const expectedUrl = '/NotificationType/Add';
            const expectedParams = 'data';

            // Act & Assert
            store.dispatch(actions.update(expectedParams, true)).then(() => {
                expect(axios.post).toHaveBeenCalledWith(expectedUrl, JSON.stringify(expectedParams), { 'headers': { 'Content-Type': 'application/json' } });
                done();
            });
        });

        it('when isNew false - call axios.post with the correct params', (done) => {
            // Arrange
            const store = mockStore();
            const fakeResponse = { data: 'some data' };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            const expectedUrl = '/NotificationType/Edit';
            const expectedParams = 'data';

            // Act & Assert
            store.dispatch(actions.update(expectedParams, false)).then(() => {
                expect(axios.post).toHaveBeenCalledWith(expectedUrl, JSON.stringify(expectedParams), { 'headers': { 'Content-Type': 'application/json' } });
                done();
            });
        });

        it('dispatch the correct action', (done) => {
            // Arrange
            const store = mockStore();
            let fakeResponse = { data: 'some data' };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            const expectedAction = [
                {
                    type: types.UPDATE_DATA,
                    data: { addedresult: fakeResponse.data }
                }
            ];

            // Act & Assert
            store.dispatch(actions.update({}, false)).then(() => {
                expect(store.getActions()).toEqual(expectedAction);
                done();
            });
        });
    });
});