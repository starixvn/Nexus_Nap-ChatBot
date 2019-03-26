import React from 'react';
import {shallow, render, configure} from 'enzyme';
import {shallowToJson} from 'enzyme-to-json';
import Adapter from 'enzyme-adapter-react-16';
import DeletePopup from '../DeletePopup';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({adapter: new Adapter()});

describe('DeletePopup redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = {deletePopupData: {isShowed: true}};
        store = mockStore(initialState);

        wrapper = shallow(
            <DeletePopup store={store} />
        );
    });

    it('should map state to props correctly', () => {
        expect(wrapper.props().deletePopupData).toEqual({isShowed: true});        
    });   
});