import React from 'react';
import {shallow, configure} from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import NotificationType from '..//NotificationType';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({adapter: new Adapter()});

describe('NotificationType', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = {notificationTypeListData: 2, deleteNotificationTypeData: 3}
        store = mockStore(initialState);

        wrapper = shallow(
            <NotificationType store={store} />
        )
    })

    it('should map state to props correctly', () => {
        expect(wrapper.props().notificationTypeListData).toBe(2);
        expect(wrapper.props().deleteNotificationTypeData).toBe(3);
    })
})
