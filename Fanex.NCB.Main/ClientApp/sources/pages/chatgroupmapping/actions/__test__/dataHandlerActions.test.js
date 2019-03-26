import thunk from 'redux-thunk';
import configureMockStore from 'redux-mock-store';
import * as actions from '../dataHandlerActions';
import * as types from '../types';
import axios from 'axios';
import "babel-polyfill";

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe("dataHanderActions", () => { 
    describe("setSettings", () => { 
        it("create the correct action", () => { 
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.SETTING_DATA,
                data: data
            };

            // Act
            let result = actions.setSettings(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("getSettings", () => { 
        it("axios.get with correct params", () => { 
            // Arrange
            axios.get = jest.fn(() => Promise.resolve(""));
            let store = mockStore();
            let expectedUrl = '/ChatGroupMapping/GetSettings';

            // Act
            store.dispatch(actions.getSettings());

            // Assert
            expect(axios.get).toHaveBeenCalledWith(expectedUrl);
        });

        it("dispatch the correct action", (done) => {
            // Arrange
            let fakeResponse = { data: "some data" };
            axios.get = jest.fn(() => Promise.resolve(fakeResponse));
            let store = mockStore();
            let expectedAction = [
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

    describe("setGridParams", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.GRID_PARAMS,
                data: data
            };

            // Act
            let result = actions.setGridParams(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("setGridData", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.GRID_DATA,
                data: data
            };

            // Act
            let result = actions.setGridData(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("getGridData", () => {
        it("axios.get with correct params", () => {
            // Arrange
            axios.get = jest.fn(() => Promise.resolve(""));
            let store = mockStore();
            let expectedUrl = '/ChatGroupMapping/GetAll';
            let expectedParams = "some param";

            // Act
            store.dispatch(actions.getGridData(expectedParams));

            // Assert
            expect(axios.get).toHaveBeenCalledWith(expectedUrl, {params: expectedParams});
        });

        it("dispatch the correct action", (done) => {
            // Arrange
            let fakeResponse = { data: "some data" };
            axios.get = jest.fn(() => Promise.resolve(fakeResponse));
            let store = mockStore();
            let expectedAction = [
                {
                    type: types.GRID_DATA,
                    data: fakeResponse.data
                }
            ];

            // Act
            store.dispatch(actions.getGridData());

            // Assert
            axios.get().then(() => {
                expect(store.getActions()).toEqual(expectedAction);
                done();
            });
        });
    });

    describe("setFilterRecipients", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.FILTERED_RECIPIENT,
                data: data
            };

            // Act
            let result = actions.setFilterRecipients(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("filterRecipients", () => {
        it("dispatch the correct action", (done) => {
            // Arrange            
            let recipients = [
                {
                    recipientId: 1,
                    recipientTypeId: 1                    
                },
                {
                    recipientId: 2,
                    recipientTypeId: 2                    
                },
                {
                    recipientId: 3,
                    recipientTypeId: 2
                }                
            ];
            let recipientTypeId = 2;
            let fakeResponse = { data: [
                {
                    recipientId: 2,
                    recipientTypeId: 2
                },
                {
                    recipientId: 3,
                    recipientTypeId: 2
                }   
            ]};
            let store = mockStore();
            let expectedAction = [
                {
                    type: types.FILTERED_RECIPIENT,
                    data: fakeResponse.data
                }
            ];

            // Act
            store.dispatch(actions.filterRecipients(recipients, recipientTypeId));

            // Assert
            expect(store.getActions()).toEqual(expectedAction);
            done();
        });
    });


    describe("setUpdateData", () => {
        it("create the correct action", () => {
            // Arrange
            let data = ["some data"];
            let expected = {
                type: types.UPDATE_DATA,
                data: data
            };

            // Act
            let result = actions.setUpdateData(data);

            // Assert
            expect(result).toEqual(expected);
        });
    });

    describe("update", () => {
        it("axios.post with correct params", (done) => {
            // Arrange
            let fakeResponse = { data: "some data" };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            let store = mockStore();
            let expectedUrl = '/ChatGroupMapping/Edit';
            let expectedParams = "data";

            // Act & Assert
            store.dispatch(actions.update(expectedParams)).then(() => {
                expect(axios.post).toHaveBeenCalledWith(expectedUrl, JSON.stringify(expectedParams), { "headers": { "Content-Type": "application/json" } });
                done();
            });
        });

        it("dispatch the correct action", (done) => {
            // Arrange
            let fakeResponse = { data: { "addedresult": true } };
            axios.post = jest.fn(() => Promise.resolve(fakeResponse));
            let store = mockStore();
            let expectedAction = [
                {
                    type: types.UPDATE_DATA,
                    data: fakeResponse.data
                }
            ];
            let expectedParams = "data";

            // Act & Assert
            store.dispatch(actions.update(expectedParams)).then(() => {
                expect(store.getActions()).toEqual(expectedAction);
                done();
            });
        });
    });

    describe("refreshUpdateData", () => {
        it("dispatch the correct action", (done) => {
            // Arrange     
            let fakeResponse = {
                data: { addedresult: false }
            };
            let store = mockStore();
            let expectedAction = [
                {
                    type: types.UPDATE_DATA,
                    data: fakeResponse.data
                }
            ];

            // Act
            store.dispatch(actions.refreshUpdateData());

            // Assert
            expect(store.getActions()).toEqual(expectedAction);
            done();
        });
    });
});