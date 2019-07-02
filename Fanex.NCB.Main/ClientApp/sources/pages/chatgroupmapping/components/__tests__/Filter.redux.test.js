import React from 'react';
import { shallow, render, configure } from 'enzyme';
import { shallowToJson } from 'enzyme-to-json';
import Adapter from 'enzyme-adapter-react-16';
import Filter from '../Filter';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({ adapter: new Adapter() });

describe('Filter redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = { settingData: { data: "some data" } };
        store = mockStore(initialState);

        wrapper = shallow(
            <Filter store={store} />
        );
    });

    it('should map state to props correctly', () => {
        expect(wrapper.props().settingData).toEqual({ data: "some data" });
    });
});