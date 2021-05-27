import React from 'react';
import { shallow, render, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import EditPopup from '../EditPopup';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({ adapter: new Adapter() });

describe('Filter redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = { settingData: "data 1", updatePopupData: "data 2", updateData: "data 3", filteredRecipients: "data 4" };
        store = mockStore(initialState);

        wrapper = shallow(
            <EditPopup store={store} />
        );
    });

    it('should map state to props correctly', () => {
        expect(wrapper.props().settingData).toEqual("data 1");
        expect(wrapper.props().updatePopupData).toEqual("data 2");
        expect(wrapper.props().updateData).toEqual("data 3");
        expect(wrapper.props().filteredRecipients).toEqual("data 4");
    });
});