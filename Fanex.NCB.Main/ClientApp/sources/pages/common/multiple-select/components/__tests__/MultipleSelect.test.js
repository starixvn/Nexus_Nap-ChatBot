import React from 'react';
import { shallow } from 'enzyme';

import MultipleSelect from '../MultipleSelect';

const defaultProps = {
    dataSource: [],
    keyField: 'key',
    valueField: 'value',
    statusField: 'checked',
    maxDisplayItemCount: 3,
    onChange: jest.fn(),
    hasAllOption: true,
    texts: {
        All: 'All',
        SelectedItemCount: '# selected',
        SelectOptions: 'Select options'
    }
}

const defaultOption = { code: 'SB', name: 'Sportsbook', status: true };
const defaultDataSource = [defaultOption];

describe('MultipleSelect ', () => {
    afterAll(() => {
        jest.resetAllMocks();
    });

    describe('Initial State', () => {
        it('has id prop - id value should be correct', () => {
            const expectedResult = `multiple-select-10`;
            const multipleSelect = shallow(<MultipleSelect id={10} {...defaultProps} />);
            const instance = multipleSelect.instance();

            const result = instance.id;

            expect(result).toBe(expectedResult);
        });

        it('showOptionList should be false', () => {
            const multipleSelect = new MultipleSelect(defaultProps);

            const result = multipleSelect.state.showOptionList;

            expect(result).toBeFalsy();
        });

        //it('dataSource prop should convert correctly', () => {
        //    // Arrange
        //    const expectedResult = [
        //        { key: defaultOption.code, value: defaultOption.name, checked: defaultOption.status }
        //    ];
        //    const props = {
        //        ...defaultProps,
        //        dataSource: defaultDataSource,
        //        keyField: 'code',
        //        valueField: 'name',
        //        statusField: 'status'
        //    };
        //    const container = shallow(<MultipleSelect {...props} />);

        //    // Act
        //    const result = container.state.dataSource;

        //    // Assert
        //    expect(result).toEqual(expectedResult);
        //});
    })

    describe('selectedItems', () => {
        it('should return correct data', () => {
            // Arrange
            const sportsbookOption = { key: 'SB', value: 'Sportsbook', checked: true, visible: true };
            const expectedResult = [sportsbookOption];
            const dataSource = [sportsbookOption, { key: 'BA', value: 'BA', checked: false }];
            const props = { ...defaultProps, dataSource };
            const container = shallow(<MultipleSelect {...props} />).instance();

            // Act
            const result = container.selectedItems;

            // Assert
            expect(result).toEqual(expectedResult);
        });
    });

    describe('onChangeHandler', () => {
        let multipleSelect;

        const sportsbookOption = { key: 'SB', value: 'Sportsbook', checked: true, visible: true };
        const changedItem = { key: 'BA', value: 'BA', checked: true, visible: true };

        const dataSource = [sportsbookOption, { ...changedItem, checked: false }];
        const newDataSource = [sportsbookOption, changedItem];

        beforeEach(() => {
            const props = { ...defaultProps, dataSource };
            multipleSelect = shallow(<MultipleSelect {...props} />);

            multipleSelect.instance()._callBackToParent = jest.fn();
            multipleSelect.setState = jest.fn();

            multipleSelect.instance().onChangeHandler(changedItem);
        });

        //it('_callBackToParent should be called with correct parameter', () => {
        //    expect(multipleSelect.instance()._callBackToParent).toHaveBeenCalledWith(changedItem);
        //});

        it('setState should be called with correct parameter', () => {
            const newState = { dataSource: newDataSource };

            expect(multipleSelect.setState).toHaveBeenCalledWith(newState);
        });
    })

    describe('onToggle', () => {
        it('new State should be correct', () => {
            let callback;
            const multipleSelect = shallow(<MultipleSelect {...defaultProps} />);
            multipleSelect.setState = jest.fn((cb) => callback = cb);

            multipleSelect.instance().onToggle();

            let result = callback({ showOptionList: false });
            expect(result).toEqual({ showOptionList: true });

            result = callback({ showOptionList: true });
            expect(result).toEqual({ showOptionList: false });
        });
    });

    //describe('checkAllHandler', () => {
    //    let multipleSelect;
    //    const sportsbookOption = { key: 'SB', value: 'Sportsbook', checked: true, visible: true };
    //    const baOption = { key: 'BA', value: 'BA', checked: true, visible: true };

    //    const testCheckAll = (checked) => {
    //        const dataSource = [
    //            { ...sportsbookOption, checked: !checked },
    //            { ...baOption, checked: !checked }
    //        ];

    //        const props = { ...defaultProps, dataSource };
    //        multipleSelect = shallow(<MultipleSelect {...props} />);

    //        multipleSelect.instance()._callBackToParent = jest.fn();
    //        multipleSelect.setState = jest.fn();

    //        multipleSelect.instance().checkAllHandler(checked);
    //    };

    //    it('_callBackToParent should be called with correct parameter', () => {
    //        const checked = true;

    //        testCheckAll(checked);

    //        expect(multipleSelect.instance()._callBackToParent).toHaveBeenCalledWith(null);
    //    });

    //    it('uncheck All - setState should be called with correct parameter', () => {
    //        const checked = false;
    //        const newDataSource = [
    //            { ...sportsbookOption, checked },
    //            { ...baOption, checked }
    //        ];

    //        const newState = { dataSource: newDataSource };

    //        testCheckAll(checked);

    //        expect(multipleSelect.setState).toHaveBeenCalledWith(newState);
    //    });

    //    it('check All - setState should be called with correct parameter', () => {
    //        const checked = true;
    //        const newDataSource = [
    //            { ...sportsbookOption, checked },
    //            { ...baOption, checked }
    //        ];

    //        const newState = { dataSource: newDataSource };

    //        testCheckAll(checked);

    //        expect(multipleSelect.setState).toHaveBeenCalledWith(newState);
    //    });
    //});

    //describe('onChangeSearchText', () => {
    //    let element;
    //    let setStateCallback;
    //    beforeEach(() => {
    //        element = shallow(<MultipleSelect {...defaultProps} />);
    //        element.originalDataSource = [{ value: 'test123' }];

    //        element.setState = jest.fn((callback) => {
    //            setStateCallback = callback;
    //        });
    //    });

    //    it('input value does not match any value in data source: setState() with correct param', () => {
    //        element.onChangeSearchText('xxx');
    //        const result = setStateCallback();

    //        expect(result).toEqual({ 'dataSource': [], 'searchText': 'xxx' });
    //    });

    //    it('input value matches values in data source: setState() with correct param', () => {
    //        element.onChangeSearchText('test');
    //        const result = setStateCallback();

    //        expect(result).toEqual({ 'dataSource': [{ 'value': 'test123' }], 'searchText': 'test' });
    //    });
    //});

    //describe('onClearSearch', () => {
    //    it('call setState() and onChangeSearchText() with correct parameters', () => {
    //        const multipleSelect = new MultipleSelect(defaultProps);
    //        let setStateData;
    //        let setStateCallback;
    //        multipleSelect.setState = jest.fn((data, callback) => {
    //            setStateData = data;
    //            setStateCallback = callback;
    //        });
    //        multipleSelect.onChangeSearchText = jest.fn();

    //        multipleSelect.onClearSearch();
    //        setStateCallback();

    //        expect(setStateData).toEqual({ searchText: '' });
    //        expect(multipleSelect.onChangeSearchText).toHaveBeenCalledWith('');
    //    });
    //});

    describe('_callBackToParent', () => {
        it('call onChange function in props with correct parameters', () => {
            const expectedSelectedItemKey = 'SB,BA';
            const expectedSelectedItem = { key: 'Key', value: 'Value', checked: true };
            const multipleSelect = new MultipleSelect(defaultProps);

            multipleSelect._getSelectedItemKey = jest.fn().mockReturnValueOnce(expectedSelectedItemKey);
            multipleSelect.props.onChange = jest.fn();

            const dataSource = [];
            multipleSelect._callBackToParent(expectedSelectedItem, dataSource);

            expect(multipleSelect.props.onChange).toHaveBeenCalledWith(expectedSelectedItem, expectedSelectedItemKey);
        });
    });

    //describe('_getSelectedItemKey', () => {
    //    it('get correct key of items that checked', () => {
    //        const expectedResult = 'SB,BA';
    //        const dataSource = [
    //            { key: 'SB', value: 'Value SB', checked: true },
    //            { key: 'BA', value: 'Value BA', checked: true },
    //            { key: 'RC', value: 'Value RC', checked: false }
    //        ];

    //        const componentInstance = new MultipleSelect(defaultProps);

    //        const result = componentInstance._getSelectedItemKey(dataSource);

    //        expect(result).toBe(expectedResult);
    //    });
    //});

    describe('close', () => {
        it('setState function should be called with correct parameter', () => {
            const expectedParam = { showOptionList: false };

            const componentInstance = new MultipleSelect(defaultProps);
            componentInstance.setState = jest.fn();

            componentInstance._close();

            expect(componentInstance.setState).toHaveBeenCalledWith(expectedParam);
        });
    });

    describe('_handleDocumentClick', () => {
        it('wrapper element is null - do nothing', () => {
            const instance = new MultipleSelect(defaultProps);
            instance._close = jest.fn();

            const clickEvent = new Event('click');
            document.dispatchEvent(clickEvent);

            expect(instance._close).not.toHaveBeenCalled();
        });

        it('showOptionList is false - do nothing', () => {
            const component = shallow(<MultipleSelect {...defaultProps} />);
            const instance = component.instance();

            instance._close = jest.fn();

            const clickEvent = new Event('click');
            document.dispatchEvent(clickEvent);

            expect(instance._close).not.toHaveBeenCalled();
        });

        //it('showOptionList is true - call _close function', () => {
        //    const component = shallow(<MultipleSelect {...defaultProps} />);
        //    component.setState({ showOptionList: true });
        //    const instance = component.instance();

        //    instance._close = jest.fn();

        //    const clickEvent = new Event('click');
        //    document.dispatchEvent(clickEvent);

        //    expect(instance._close).toHaveBeenCalled();
        //});
    });

    describe('_renderOptionAll', () => {
        it('hassAllOption in props is false - return null', () => {
            const newProps = Object.assign({}, defaultProps, { hasAllOption: false });
            const componentInstance = new MultipleSelect(newProps);

            const result = componentInstance._renderOptionAll();

            expect(result).toBe(null);
        });

        it('hasAllOption in props is true - render OptionAll component with correct props', () => {
            const dataSource = [
                { key: 'SB', value: 'Value SB', checked: true },
                { key: 'BA', value: 'Value BA', checked: true },
                { key: 'RC', value: 'Value RC', checked: true }
            ];
            const newProps = Object.assign({}, defaultProps, { dataSource });
            const componentInstance = new MultipleSelect(newProps);
            const optionAllComponent = componentInstance._renderOptionAll();

            const { checked } = optionAllComponent.props;

            expect(checked).toBe(true);
        });
    });

    //describe('render', () => {
    //    it('showOptionList = false - should match snapshot', () => {
    //        const component = shallow(<MultipleSelect {...defaultProps} />);

    //        const tree = component.html();

    //        expect(tree).toMatchSnapshot();
    //    });

    //    it('showOptionList = true - should match snapshot', () => {
    //        const component = shallow(<MultipleSelect {...defaultProps} />);
    //        const instance = component.instance();
    //        instance.setState({ showOptionList: true });

    //        const tree = component.html();

    //        expect(tree).toMatchSnapshot();
    //    });
    //});

    describe('componentWillUnmount', () => {
        it('document.removeEventListener function should be called with correct event name', () => {
            const originalRemoveEventListenerFunction = document.removeEventListener;
            const componentInstance = new MultipleSelect(defaultProps);
            document.removeEventListener = jest.fn();

            componentInstance.componentWillUnmount();

            expect(document.removeEventListener.mock.calls[0][0]).toBe('click');

            document.removeEventListener = originalRemoveEventListenerFunction;
        });
    });

    //describe('componentWillReceiveProps', () => {
    //    it('dataSource is the same - do nothing', () => {
    //        const nextProps = Object.assign({}, defaultProps);
    //        const multipleSelect = new MultipleSelect(defaultProps);
    //        multipleSelect.setState = jest.fn();

    //        multipleSelect.componentWillReceiveProps(nextProps);

    //        expect(multipleSelect.setState).not.toHaveBeenCalled();
    //    });

    //    it('dataSource is updated - setState should be called with correct parameter', () => {
    //        const props = Object.assign({}, defaultProps, { dataSource: defaultDataSource });
    //        const nextProps = Object.assign({}, props);
    //        const multipleSelect = new MultipleSelect(defaultProps);
    //        multipleSelect.setState = jest.fn();
    //        multipleSelect._convertDataSourceToState = jest.fn(() => defaultDataSource);
    //        multipleSelect._callBackToParent = jest.fn();

    //        const expectedState = {
    //            dataSource: defaultDataSource
    //        };

    //        multipleSelect.componentWillReceiveProps(nextProps);

    //        expect(multipleSelect.setState).toHaveBeenCalledWith(expectedState);
    //    });

    //    it('dataSource is updated - _callBackToParent should be called with correct parameter', () => {
    //        const props = Object.assign({}, defaultProps, { dataSource: defaultDataSource });
    //        const nextProps = Object.assign({}, props);
    //        const multipleSelect = new MultipleSelect(defaultProps);
    //        multipleSelect.setState = jest.fn();
    //        multipleSelect._convertDataSourceToState = jest.fn(() => defaultDataSource);
    //        multipleSelect._callBackToParent = jest.fn();

    //        multipleSelect.componentWillReceiveProps(nextProps);

    //        expect(multipleSelect._callBackToParent).toHaveBeenCalledWith(null);
    //    });
    //});
});