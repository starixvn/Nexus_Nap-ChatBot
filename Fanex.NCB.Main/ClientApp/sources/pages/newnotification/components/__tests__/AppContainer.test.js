import React from 'react';
import { shallow, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import AppContainerRedux, { AppContainer } from '../appContainer';
import ShallowRenderer from 'react-test-renderer/shallow';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({ adapter: new Adapter() });

describe('AppContainer', () => {
    describe('redux', () => {
        const initialState = {
            notificationTypes: [{ notificationTypeID: 1, notificationTypeName: 'a' }, { id: 2, name: 'b' }],
            products: [{ id: 1, name: 'p1' }, { id: 2, name: 'p2' }],
            notificationTemplates: [
                { notificationTemplateId: 1, notificationTemplateName: 'template 1' },
                { notificationTemplateId: 2, notificationTemplateName: 'template 2' }
            ],
            template: {
                title: 'title hh {UM date}',
                content: 'good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time} <br/> {a very long parameter here} {ha ha} cc'
            },
            confimrationPopup: {
                isOpen: false
            },
            resetConfimrationPopup: {
                isOpen: false
            },
            editConfimrationPopup: {
                isOpen: false
            },
            markdownRulesPopup: {
                isOpen: false
            }
        };
        const store = mockStore(initialState);
        const wrapper = shallow(<AppContainerRedux store={store} />);

        it('should map state to props correctly', () => {
            expect(wrapper.props().notificationTypes).toEqual(initialState.notificationTypes);
            expect(wrapper.props().products).toEqual(initialState.products);
            expect(wrapper.props().notificationTemplates).toEqual(initialState.notificationTemplates);
        });
    });

    describe('render', () => {
        it('should render null with props default', () => {
            // Arrange
            const props = {
                notificationTypes: null,
                products: null,
            };

            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);

            // Assert
            expect(appContainer).toMatchSnapshot();
        });
    });
});