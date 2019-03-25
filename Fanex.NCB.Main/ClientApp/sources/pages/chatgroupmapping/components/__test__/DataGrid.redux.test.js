import React from 'react';
import { shallow, render, configure } from 'enzyme';
import { shallowToJson } from 'enzyme-to-json';
import Adapter from 'enzyme-adapter-react-16';
import DataGrid from '../DataGrid';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({ adapter: new Adapter() });

describe('DataGrid redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = {
            gridParams: "grid param", gridData: "grid data", settingData: "setting data" }
        store = mockStore(initialState);

        wrapper = shallow(
            <DataGrid store={store} />
        )
    })

    it('should map state to props correctly', () => {
        expect(wrapper.props().gridParams).toEqual("grid param");
        expect(wrapper.props().gridData).toEqual("grid data");
        expect(wrapper.props().settingData).toEqual("setting data");
    })
})