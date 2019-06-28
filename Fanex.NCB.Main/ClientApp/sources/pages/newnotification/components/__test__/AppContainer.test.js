import React from 'react'
import { shallow, configure } from 'enzyme'
import Adapter from 'enzyme-adapter-react-16'
import AppContainerRedux, { AppContainer } from '../appContainer'
import ShallowRenderer from 'react-test-renderer/shallow'
import configureMockStore from 'redux-mock-store'

const mockStore = configureMockStore();
configure({ adapter: new Adapter() });

describe('AppContainer', () => {
    describe('redux', () => {
        const initialState = {
            settingData:  {
                notificationTypes: [{ id: 1, name: 'a' }, { id: 2, name: 'b' }],
                products: [{ id: 1, name: 'p1' }, { id: 2, name: 'p2' }]
            }, 

            notificationTemplates: [{ id: 1, name: 'template 1' }, { id: 2, name: 'template 2' }],
            template: {
                title: 'title hh {UM date}',
                content: 'good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time} <br/> {a very long parameter here} {ha ha} cc'
            }
        }
        const store = mockStore(initialState);
        const wrapper = shallow(<AppContainerRedux store={store} />)

        it('should map state to props correctly', () => {
            expect(wrapper.props().settingData.notificationTypes).toEqual([{ id: 1, name: 'a' }, { id: 2, name: 'b' }])
            expect(wrapper.props().settingData.products).toEqual([{ id: 1, name: 'p1' }, { id: 2, name: 'p2' }])
            expect(wrapper.props().notificationTemplates).toEqual([{ id: 1, name: 'template 1' }, { id: 2, name: 'template 2' }])
            expect(wrapper.props().template).toEqual({
                title: 'title hh {UM date}',
                content: 'good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time} <br/> {a very long parameter here} {ha ha} cc'
            })
        })
    })

    describe('render', () => {
        it('should render null with props default', () => {
            // Arrange
            const props = {
                settingData:  null,
                notificationTemplates: null,
                template: null
            }

            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer />)

            // Assert
            expect(appContainer).toMatchSnapshot()
        })

        it('correct with props', () => {
            // Arrange
            const props = {
                settingData: {
                    notificationTypes: [{ id: 1, name: 'a' }, { id: 2, name: 'b' }],
                    products: [{ id: 1, name: 'p1' }, { id: 2, name: 'p2' }]
                },
                notificationTemplates: [{ id: 1, name: 'template 1' }, { id: 2, name: 'template 2' }],
                template: null
            }
            
            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />)

            // Assert
            expect(appContainer).toMatchSnapshot()
        })
    })
})