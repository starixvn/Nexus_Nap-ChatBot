import React from 'react';
import {shallow, render, configure} from 'enzyme';
import {shallowToJson} from 'enzyme-to-json';
import Adapter from 'enzyme-adapter-react-16';
import AppContainer from '..//AppContainer';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({adapter: new Adapter()});

describe('AppContainer redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = {notificationTypeListData: 2, deleteNotificationTypeData: 3, getNotificationTypes: jest.fn()}
        store = mockStore(initialState);

        wrapper = shallow(
            <AppContainer store={store} />
        )
    })

    it('should map state to props correctly', () => {
        expect(wrapper.props().notificationTypeListData).toBe(2);
        expect(wrapper.props().deleteNotificationTypeData).toBe(3);
    })

    it('shoud ok', () => {
        //var wrapper = render( <NotificationType store={store}  />);
       // wrapper.setState({isShowPopup: true});
        //console.log(wrapper);
        //expect(wrapper).toMatchSnapshot();
    })
})