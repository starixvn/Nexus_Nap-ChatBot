import React from 'react';
import { shallow, render, configure } from 'enzyme';
import { shallowToJson } from 'enzyme-to-json';
import Adapter from 'enzyme-adapter-react-16';
import AppContainer from '../AppContainer';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({ adapter: new Adapter() });

describe('AppContainer redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = { gridParams: { pageIndex: 2 }, updatePopupData: { isShowed: true } }
        store = mockStore(initialState);

        wrapper = shallow(
            <AppContainer store={store} />
        )
    })

    it('should map state to props correctly', () => {
        expect(wrapper.props().gridParams).toEqual({ pageIndex: 2 });
        expect(wrapper.props().updatePopupData).toEqual({ isShowed: true });
    })
})