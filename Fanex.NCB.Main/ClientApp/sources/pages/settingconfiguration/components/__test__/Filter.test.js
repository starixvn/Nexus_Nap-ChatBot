import React from 'react';
import { Filter } from '../Filter';
import { shallow } from 'enzyme';

describe('Filter', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        settingTypes: [{ value: 'test', type: 1 }],
        setGridParams: jest.fn(),
        getGridData: jest.fn()
    };

    describe('componentDidMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};
            const params = {
                settingType : 1,
                pageIndex: 1,
                pageSize: 10
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().getParams = jest.fn(() => params);
            container.instance().componentDidMount();

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(params);
            expect(props.getGridData).toHaveBeenCalledWith(params);
        });
    });

    describe('handleSettingTypeChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 'test' }
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().handleSettingTypeChange(mockEvent);

            // Assert
            expect(container.state().settingType).toEqual('test');
        });
    });

    describe('getParams', () => {
        it('should return exactly data', () => {
            // Arrange
            const props = {...initProps};
            const expected = {
                settingType : 1,
                pageIndex: 1,
                pageSize: 10
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.setState({ settingType: 1 });
            const actual = container.instance().getParams();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('submitHandler', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setGridParams: jest.fn(),
                getGridData: jest.fn()
            };
            const params = {
                settingType : 1,
                pageIndex: 1,
                pageSize: 10
            };
            const mockEvent = {
                preventDefault: jest.fn()
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().getParams = jest.fn(() => params);
            container.instance().submitHandler(mockEvent);

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(params);
            expect(props.getGridData).toHaveBeenCalledWith(params);
        });
    });

    describe('renderSettingTypeOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const props = {...initProps};
            const item = {
                value: 1,
                type: 'test'
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            const actual = container.instance().renderSettingTypeOption(item);

            // Assert
            expect(actual).toEqual(<option key={1} value={1}>test</option>);
        });
    });
});