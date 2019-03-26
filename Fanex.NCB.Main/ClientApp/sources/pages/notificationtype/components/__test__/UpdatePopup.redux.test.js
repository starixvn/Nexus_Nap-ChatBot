import React from 'react';
import {shallow, configure} from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import UpdatePopup from '../UpdatePopup';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({adapter: new Adapter()});

describe('UpdatePopup redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = {settingData: {data1: []}, updatePopupData: {data2: []}, updateData: {data3: []}};
        store = mockStore(initialState);

        wrapper = shallow(
            <UpdatePopup store={store} />
        );
    });

    it('should map state to props correctly', () => {
        expect(wrapper.props().settingData).toEqual({data1: []});        
        expect(wrapper.props().updatePopupData).toEqual({data2: []});        
        expect(wrapper.props().updateData).toEqual({data3: []});        
    });   
});