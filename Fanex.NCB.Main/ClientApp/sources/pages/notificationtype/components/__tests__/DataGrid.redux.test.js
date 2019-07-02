import React from 'react';
import {shallow, render, configure} from 'enzyme';
import {shallowToJson} from 'enzyme-to-json';
import Adapter from 'enzyme-adapter-react-16';
import DataGrid from '../DataGrid';
import configureMockStore from 'redux-mock-store';

const mockStore = configureMockStore();
configure({adapter: new Adapter()});

describe('DataGrid redux', () => {
    let wrapper, store;
    beforeEach(() => {
        const initialState = {gridParams: {pageIndex: 1}, gridData: {data: []}, activeRowIndex: 2};
        store = mockStore(initialState);

        wrapper = shallow(
            <DataGrid store={store} />
        );
    });

    it('should map state to props correctly', () => {
        expect(wrapper.props().gridParams).toEqual({pageIndex: 1});
        expect(wrapper.props().gridData).toEqual({data: []});
        expect(wrapper.props().activeRowIndex).toEqual(2);
    });   
});