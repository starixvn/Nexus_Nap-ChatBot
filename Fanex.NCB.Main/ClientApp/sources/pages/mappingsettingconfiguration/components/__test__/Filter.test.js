import React from 'react';
import { Filter } from '../Filter';
import { shallow } from 'enzyme';

describe('Filter', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        settingTypes: [{ id: 1, name: 'test' }],
        setGridParams: jest.fn(),
        getGridData: jest.fn(),
        getJobInfo: jest.fn(),
        setJobInfoFlag: jest.fn()
    };

    describe('componentDidMount', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};
            const submitQuery = jest.fn();

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().submitQuery = submitQuery;
            container.instance().componentDidMount();

            // Assert
            expect(submitQuery).toHaveBeenCalled();
        })
    });

    describe('handleSettingTypeChange', () => {
        it('should setSate exactly', () => {
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
        it('should return exactly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<Filter {...props}/>);
            const actual = container.instance().getParams();

            // Assert
            expect(actual).toEqual({jobId: 1, pageIndex: 1, pageSize: 10});
        });
    });

    describe('submitHandler', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                preventDefault: jest.fn()
            };
            const submitQuery = jest.fn();

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().submitQuery = submitQuery;
            container.instance().submitHandler(mockEvent);

            // Assert
            expect(mockEvent.preventDefault).toHaveBeenCalled();
            expect(submitQuery).toHaveBeenCalled();
        });
    });

    describe('submitQuery', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};
            const params = { jobId: 1, name: 'test' };

            // Act
            const container = shallow(<Filter {...props}/>);
            container.instance().getParams = jest.fn(() => params);
            container.instance().submitQuery();

            // Assert
            expect(props.getGridData).toHaveBeenCalledWith(params);
            expect(props.setGridParams).toHaveBeenCalledWith(params);
            expect(props.getJobInfo).toHaveBeenCalledWith(1);
            expect(props.setJobInfoFlag).toHaveBeenCalledWith({ isEdit: false });
        });
    });

    describe('renderSettingTypeOption', () => {
        it('should render template exactly', () => {
            // Arrange
            const props = {...initProps};
            const item = {
                id: 1,
                name: 'test'
            };

            // Act
            const container = shallow(<Filter {...props}/>);
            const actual = container.instance().renderSettingTypeOption(item);

            // Assert
            expect(actual).toEqual(<option key={1} value={1}>test</option>);
        });
    });
});