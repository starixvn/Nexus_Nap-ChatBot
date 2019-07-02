import thunk from 'redux-thunk';
import configureMockStore from 'redux-mock-store';
import * as actions from '../dataHandlerActions';
import * as types from '../types';
import axios  from 'axios';
import "babel-polyfill";
import Site from '../../../lib/site';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('dataHanderActions', () => { 
    describe('setting', () => {
        describe('setSettings', () => { 
            it('create the correct action', () => { 
                // Arrange
                const data = {
                    notificationTypes: [{ id: 1, name: "a" }, { id: 2, name: "b" }],
                    products: [{ id: 1, name: "p1" }, { id: 2, name: "p2" }]
                }
    
                const expected = {
                    type: types.SETTING_DATA,
                    data: data
                }
    
                // Act
                let actual = actions.setSettings(data)
    
                // Assert
                expect(actual).toEqual(expected)
            })
        })
    
        describe('getSettings', () => { 
            it('should call with the correct url', async () => { 
                // Arrange
                axios.get = jest.fn(() => Promise.resolve())
                const store = mockStore({})
                const expectedUrl = Site.resolveClientUrl('NewNotification/GetSettings')
    
                // Act
                await store.dispatch(actions.getSettings())
    
                // Assert
                expect(axios.get).toHaveBeenCalledWith(expectedUrl)
            })
    
            it('async - should dispatch with the correct action', async () => { 
                // Arrange
                const response = {
                    data: {
                        notificationTypes: null,
                        products: null
                    }
                }
    
                let expected = [actions.setSettings(response.data)]
    
                axios.get = jest.fn(() => Promise.resolve(response))
                const store = mockStore({})
    
                // Act
                await store.dispatch(actions.getSettings())
    
                // Assert
                expect(store.getActions()).toEqual(expected)
            })
        })
    })

    describe('notificationType', () => {
        describe('setNotificationTypes', () => { 
            it('create the correct action', () => { 
                // Arrange
                const data = [{ id: 1, name: 'a' }, { id: 2, name: 'b' }]
    
                const expected = {
                    type: types.NOTIFICATION_TYPE_DATA,
                    data
                }
    
                // Act
                const actual = actions.setNotificationTypes(data)
    
                // Assert
                expect(actual).toEqual(expected)
            })
        })
    
        describe('getNotificationTypes', () => { 
            it('should call with the correct url', async () => { 
                // Arrange
                axios.get = jest.fn(() => Promise.resolve())
                const store = mockStore({})
                const expectedUrl = Site.resolveClientUrl('NewNotification/GetNotificationTypes')
    
                // Act
                await store.dispatch(actions.getNotificationTypes())
    
                // Assert
                expect(axios.get).toHaveBeenCalledWith(expectedUrl)
            })
    
            it('async - should dispatch with the correct action', async () => { 
                // Arrange
                const response = {
                    data:  [{ id: 1, name: 'a' }, { id: 2, name: 'b' }]
                }
                axios.get = jest.fn(() => Promise.resolve(response))
                const expected = [actions.setNotificationTypes(response.data)]
                const store = mockStore({})
    
                // Act
                await store.dispatch(actions.getNotificationTypes())
    
                // Assert
                expect(store.getActions()).toEqual(expected)
            })
        })
    })

    describe('templateType', () => {
        describe('setTemplateTypes', () => { 
            it('create the correct action', () => { 
                // Arrange
                const data = [{ id: 1, name: "template 1" }, { id: 2, name: "template 2" }];
    
                let expected = {
                    type: types.TEMPLATE_TYPE_DATA,
                    data
                }
    
                // Act
                let actual = actions.setTemplateTypes(data)
    
                // Assert
                expect(actual).toEqual(expected)
            })
        })
    
        describe('getTemplateTypes', () => { 
            it('should call with the correct url', async () => { 
                // Arrange
                axios.get = jest.fn(() => Promise.resolve())
                const store = mockStore({})
                const expectedUrl = Site.resolveClientUrl('NewNotification/GetNotificationTemplate')
    
                // Act
                await store.dispatch(actions.getTemplateTypes())
    
                // Assert
                expect(axios.get).toHaveBeenCalledWith(expectedUrl)
            })
    
            it('async - should dispatch with the correct action', async () => { 
                // Arrange
                const response = {
                    data: [{ id: 1, name: "template 1" }, { id: 2, name: "template 2" }]
                }
                axios.get = jest.fn(() => Promise.resolve(response))
                const expected = [actions.setTemplateTypes(response.data)]
                const store = mockStore({})
    
                // Act
                await store.dispatch(actions.getTemplateTypes())
    
                // Assert
                expect(store.getActions()).toEqual(expected)
            })
        })
    })

    describe('template', () => {
        describe('setTemplate', () => { 
            it('create the correct action', () => { 
                // Arrange
                const data = {
                    title: 'title hh {UM date}',
                    content: 'good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time} <br/> {a very long parameter here} {ha ha} cc'
                }
    
                const expected = {
                    type: types.TEMPLATE_DATA,
                    data
                }
    
                // Act
                const actual = actions.setTemplate(data)
    
                // Assert
                expect(actual).toEqual(expected)
            })
        })
    
        describe('getTemplate', () => { 
            it('should call with the correct url', async () => { 
                // Arrange
                axios.get = jest.fn(() => Promise.resolve())
                const store = mockStore({})
                const expectedUrl = Site.resolveClientUrl('NewNotification/GetTemplate')
    
                // Act
                await store.dispatch(actions.getTemplate())
    
                // Assert
                expect(axios.get).toHaveBeenCalledWith(expectedUrl)
            })
    
            it('async - should dispatch with the correct action', async () => { 
                // Arrange
                const response = {
                    data: {
                        title: '',
                        content: ''
                    }
                }
                axios.get = jest.fn(() => Promise.resolve(response))
                const expected = [actions.setTemplate(response.data)]
                const store = mockStore({})
    
                // Act
                await store.dispatch(actions.getTemplate())
    
                // Assert
                expect(store.getActions()).toEqual(expected)
            })
        })
    })
})